using OfficeMan.ArtGen.Raster;

namespace OfficeMan.ArtGen.Rooms;

/// <summary>Layers every room produces for the room shader. See design/visual_standard.md.</summary>
public sealed record RoomLayers(Canvas Albedo, Canvas Emissive, Canvas Glow, Canvas Front);

/// <summary>Shared tools for drawing rooms to the visual standard.</summary>
public static class RoomKit
{
    public const int W = 480, H = 270;

    /// <summary>Bloom: the emissive layer blurred at two widths, so glows have a tight core and a wide halo.</summary>
    public static Canvas Glow(Canvas e)
    {
        double[] Channel(Func<Rgb, byte> pick) =>
            Enumerable.Range(0, W * H).Select(i => e.IsSet(i % W, i / W) ? pick(e[i % W, i / W]) / 255.0 : 0).ToArray();
        var rgb = new[] { Channel(c => c.R), Channel(c => c.G), Channel(c => c.B) };
        var tight = rgb.Select(c => Blur(c, 3, 3)).ToArray();
        var wide = rgb.Select(c => Blur(c, 12, 3)).ToArray();
        var g = new Canvas(W, H);
        for (int i = 0; i < W * H; i++)
        {
            byte Ch(int k) => (byte)Math.Clamp((tight[k][i] * 0.5 + wide[k][i] * 0.7) * 255, 0, 255);
            g[i % W, i / W] = new Rgb(Ch(0), Ch(1), Ch(2));
        }
        return g;
    }

    private static double[] Blur(double[] src, int r, int passes)
    {
        var a = (double[])src.Clone();
        var b = new double[a.Length];
        for (int p = 0; p < passes; p++)
        {
            for (int y = 0; y < H; y++)
            {
                double acc = 0;
                for (int x = -r; x <= r; x++) acc += a[y * W + Math.Clamp(x, 0, W - 1)];
                for (int x = 0; x < W; x++)
                {
                    b[y * W + x] = acc / (2 * r + 1);
                    acc += a[y * W + Math.Clamp(x + r + 1, 0, W - 1)] - a[y * W + Math.Clamp(x - r, 0, W - 1)];
                }
            }
            for (int x = 0; x < W; x++)
            {
                double acc = 0;
                for (int y = -r; y <= r; y++) acc += b[Math.Clamp(y, 0, H - 1) * W + x];
                for (int y = 0; y < H; y++)
                {
                    a[y * W + x] = acc / (2 * r + 1);
                    acc += b[Math.Clamp(y + r + 1, 0, H - 1) * W + x] - b[Math.Clamp(y - r, 0, H - 1) * W + x];
                }
            }
        }
        return a;
    }

    public static void ClearEmissive(Canvas e, int x0, int y0, int x1, int y1)
    {
        for (int y = y0; y < y1; y++)
            for (int x = x0; x < x1; x++) e.Clear(x, y);
    }

    public static void Emit(Canvas e, int x0, int y0, int x1, int y1, Rgb c)
    {
        for (int y = y0; y < y1; y++)
            for (int x = x0; x < x1; x++) e[x, y] = c;
    }

    /// <summary>Deterministic, small-amplitude variation in [-0.5, 0.5] for material texture, not decoration.</summary>
    public static double Noise(int x, int y, int seed)
    {
        uint h = (uint)(x * 374761393 + y * 668265263 + seed * 1442695041);
        h = (h ^ (h >> 13)) * 1274126177;
        return ((h ^ (h >> 16)) & 0xFFFF) / 65535.0 - 0.5;
    }

    public static IEnumerable<(int x, int y)> Circle(int cx, int cy, int r)
    {
        for (int y = cy - r; y <= cy + r; y++)
            for (int x = cx - r; x <= cx + r; x++)
                if ((x - cx) * (x - cx) + (y - cy) * (y - cy) <= r * r) yield return (x, y);
    }

    public static IEnumerable<(int x, int y)> Ring(int cx, int cy, int r)
    {
        for (int y = cy - r; y <= cy + r; y++)
            for (int x = cx - r; x <= cx + r; x++)
            {
                int d = (x - cx) * (x - cx) + (y - cy) * (y - cy);
                if (d <= r * r && d > (r - 1) * (r - 1)) yield return (x, y);
            }
    }

    /// <summary>Digits in a 3×5 pixel font, for clocks and door numbers.</summary>
    public static void Digits(Action<int, int> plot, int x, int y, string text)
    {
        var glyphs = new Dictionary<char, string[]>
        {
            ['0'] = new[] { "###", "#.#", "#.#", "#.#", "###" }, ['1'] = new[] { ".#.", "##.", ".#.", ".#.", "###" },
            ['2'] = new[] { "###", "..#", "###", "#..", "###" }, ['3'] = new[] { "###", "..#", ".##", "..#", "###" },
            ['4'] = new[] { "#.#", "#.#", "###", "..#", "..#" }, ['5'] = new[] { "###", "#..", "###", "..#", "###" },
            ['6'] = new[] { "###", "#..", "###", "#.#", "###" }, ['7'] = new[] { "###", "..#", "..#", "..#", "..#" },
            ['8'] = new[] { "###", "#.#", "###", "#.#", "###" }, ['9'] = new[] { "###", "#.#", "###", "..#", "###" },
            [':'] = new[] { "...", ".#.", "...", ".#.", "..." },
        };
        foreach (char ch in text)
        {
            if (!glyphs.TryGetValue(ch, out var g)) { x += 2; continue; }
            int w = ch == ':' ? 1 : 3, off = ch == ':' ? 1 : 0;
            for (int j = 0; j < 5; j++)
                for (int i = 0; i < w; i++)
                    if (g[j][i + off] == '#') plot(x + i, y + j);
            x += w + 1;
        }
    }
}
