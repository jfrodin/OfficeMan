using OfficeMan.ArtGen.Raster;

namespace OfficeMan.ArtGen.Portraits;

/// <summary>How a portrait is rendered: how many tones, how much dithering, which lines.</summary>
public sealed record PortraitStyle
{
    public string Name { get; init; } = "";

    /// <summary>Tones kept per colour ramp; null keeps the full ramp.</summary>
    public int? ToneSteps { get; init; }

    public double Dither { get; init; } = 0.35;
    public double HairTexture { get; init; } = 1.0;
    public bool InnerOutlines { get; init; }
    public bool Background { get; init; } = true;

    /// <summary>A: soft-lit pixel art (art test v1).</summary>
    public static readonly PortraitStyle Soft = new() { Name = "soft" };

    /// <summary>B: flat cel pixel art: few tones, hard shadow shapes, inner lines.</summary>
    public static readonly PortraitStyle Cel = new()
    {
        Name = "cel", ToneSteps = 3, Dither = 0, HairTexture = 0.9, InnerOutlines = true,
    };

    public Ramp Reduce(Ramp ramp)
    {
        if (ToneSteps is not int n || n >= ramp.Colors.Count) return ramp;
        // Keep evenly spaced tones, skipping the very darkest (that's for lines) when we can.
        int first = ramp.Colors.Count > n + 1 ? 1 : 0;
        var picked = Enumerable.Range(0, n)
            .Select(i => ramp.Colors[first + (int)Math.Round(i * (ramp.Colors.Count - 1 - first) / (double)(n - 1))])
            .ToArray();
        return new Ramp(picked);
    }
}

/// <summary>Styles produced by transforming a finished portrait.</summary>
public static class StyleFilters
{
    /// <summary>C: low-res chunky. Halves the resolution, keeping the most common colour of each 2×2 block.</summary>
    public static Canvas Chunky(Canvas src)
    {
        var dst = new Canvas(src.Width / 2, src.Height / 2);
        for (int y = 0; y < dst.Height; y++)
            for (int x = 0; x < dst.Width; x++)
            {
                var block = new List<Rgb>();
                for (int j = 0; j < 2; j++)
                    for (int i = 0; i < 2; i++)
                        if (src.IsSet(x * 2 + i, y * 2 + j)) block.Add(src[x * 2 + i, y * 2 + j]);
                if (block.Count == 0) continue;
                // Most common colour; ties go to the darkest so lines survive.
                dst[x, y] = block.GroupBy(c => c)
                    .OrderByDescending(g => g.Count())
                    .ThenBy(g => g.Key.Luma)
                    .First().Key;
            }
        return dst;
    }

    /// <summary>
    /// D: risograph print. Two inks (blue for darkness, fluorescent pink for warmth) as rotated
    /// halftone dots on cream paper, slightly misregistered like a real print run.
    /// </summary>
    public static Canvas Riso(Canvas src, int scale)
    {
        var paper = Rgb.Hex("f1ebdd");
        var blue = Rgb.Hex("2f5aa8");
        var pink = Rgb.Hex("ff5a8a");
        var dst = new Canvas(src.Width * scale, src.Height * scale);

        double Coverage(double sx, double sy, bool pinkInk)
        {
            int x = Math.Clamp((int)sx, 0, src.Width - 1), y = Math.Clamp((int)sy, 0, src.Height - 1);
            if (!src.IsSet(x, y))
            {
                // Background: a pink disc behind the head, poster-style.
                double d = Math.Sqrt(Math.Pow(sx - src.Width * 0.5, 2) + Math.Pow(sy - src.Height * 0.42, 2));
                return pinkInk ? (d < src.Width * 0.46 ? 0.35 : 0) : 0;
            }
            var c = src[x, y];
            double luma = c.Luma / 255.0;
            if (!pinkInk) return Math.Clamp((0.82 - luma) * 1.35, 0, 1);
            double warmth = (c.R - Math.Max(c.G, c.B)) / 255.0;
            return Math.Clamp(warmth * 1.3, 0, 1);
        }

        bool Dot(int px, int py, double angle, double cell, bool pinkInk, double ox, double oy)
        {
            double ca = Math.Cos(angle), sa = Math.Sin(angle);
            double x = px + ox, y = py + oy;
            double u = (x * ca + y * sa) / cell, v = (-x * sa + y * ca) / cell;
            double cu = Math.Floor(u) + 0.5, cv = Math.Floor(v) + 0.5;
            // Sample coverage at the dot's centre, back in image space.
            double cx = (cu * ca - cv * sa) * cell - ox, cy = (cu * sa + cv * ca) * cell - oy;
            double cov = Coverage(cx / scale, cy / scale, pinkInk);
            double r = Math.Sqrt(cov / Math.PI) * 1.25;
            return Math.Sqrt((u - cu) * (u - cu) + (v - cv) * (v - cv)) < r;
        }

        for (int y = 0; y < dst.Height; y++)
            for (int x = 0; x < dst.Width; x++)
            {
                bool b = Dot(x, y, 0.26, 3.2, false, 0, 0);
                bool p = Dot(x, y, 1.31, 3.2, true, 2, 1); // misregistered
                double r = paper.R / 255.0, g = paper.G / 255.0, bl = paper.B / 255.0;
                if (b) { r *= blue.R / 255.0; g *= blue.G / 255.0; bl *= blue.B / 255.0; }
                if (p) { r *= pink.R / 255.0; g *= pink.G / 255.0; bl *= pink.B / 255.0; }
                dst[x, y] = new Rgb((byte)(r * 255), (byte)(g * 255), (byte)(bl * 255));
            }
        return dst;
    }

    /// <summary>
    /// E: flat vector look. Smooths pixel edges with three passes of Scale2x (EPX), then
    /// box-filters down by two for anti-aliased, resolution-free-looking shapes.
    /// </summary>
    public static Canvas Smooth(Canvas src)
    {
        var c = src;
        for (int i = 0; i < 3; i++) c = Scale2x(c);
        var dst = new Canvas(c.Width / 2, c.Height / 2);
        for (int y = 0; y < dst.Height; y++)
            for (int x = 0; x < dst.Width; x++)
            {
                int r = 0, g = 0, b = 0, n = 0;
                for (int j = 0; j < 2; j++)
                    for (int i = 0; i < 2; i++)
                        if (c.IsSet(x * 2 + i, y * 2 + j))
                        {
                            var p = c[x * 2 + i, y * 2 + j];
                            r += p.R; g += p.G; b += p.B; n++;
                        }
                if (n > 0) dst[x, y] = new Rgb((byte)(r / n), (byte)(g / n), (byte)(b / n));
            }
        return dst;
    }

    private static Canvas Scale2x(Canvas s)
    {
        var d = new Canvas(s.Width * 2, s.Height * 2);
        Rgb At(int x, int y) => s[Math.Clamp(x, 0, s.Width - 1), Math.Clamp(y, 0, s.Height - 1)];
        for (int y = 0; y < s.Height; y++)
            for (int x = 0; x < s.Width; x++)
            {
                if (!s.IsSet(x, y)) continue;
                Rgb p = At(x, y), a = At(x, y - 1), b = At(x + 1, y), c = At(x - 1, y), dd = At(x, y + 1);
                Rgb e0 = p, e1 = p, e2 = p, e3 = p;
                if (c == a && c != dd && a != b) e0 = a;
                if (a == b && a != c && b != dd) e1 = b;
                if (dd == c && dd != b && c != a) e2 = c;
                if (b == dd && b != a && dd != c) e3 = dd;
                d[x * 2, y * 2] = e0;
                d[x * 2 + 1, y * 2] = e1;
                d[x * 2, y * 2 + 1] = e2;
                d[x * 2 + 1, y * 2 + 1] = e3;
            }
        return d;
    }
}
