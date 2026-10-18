namespace OfficeMan.ArtGen.Raster;

public readonly record struct Rgb(byte R, byte G, byte B)
{
    public static Rgb Hex(string hex)
    {
        hex = hex.TrimStart('#');
        return new Rgb(Convert.ToByte(hex[..2], 16), Convert.ToByte(hex[2..4], 16), Convert.ToByte(hex[4..6], 16));
    }

    public Rgb Mix(Rgb o, double t) => new(
        (byte)Math.Round(R + (o.R - R) * t),
        (byte)Math.Round(G + (o.G - G) * t),
        (byte)Math.Round(B + (o.B - B) * t));

    public double Luma => 0.299 * R + 0.587 * G + 0.114 * B;
}

/// <summary>An RGBA pixel buffer with a coverage mask, so layers can be composited and outlined.</summary>
public sealed class Canvas
{
    public int Width { get; }
    public int Height { get; }
    private readonly Rgb[] _px;
    private readonly bool[] _set;

    public Canvas(int width, int height)
    {
        Width = width;
        Height = height;
        _px = new Rgb[width * height];
        _set = new bool[width * height];
    }

    public bool InBounds(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;

    public Rgb this[int x, int y]
    {
        get => _px[y * Width + x];
        set
        {
            if (!InBounds(x, y)) return;
            _px[y * Width + x] = value;
            _set[y * Width + x] = true;
        }
    }

    public bool IsSet(int x, int y) => InBounds(x, y) && _set[y * Width + x];

    public void Clear(int x, int y)
    {
        if (InBounds(x, y)) _set[y * Width + x] = false;
    }

    public void Fill(Rgb c)
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                this[x, y] = c;
    }

    /// <summary>Copies every set pixel of <paramref name="layer"/> onto this canvas.</summary>
    public void Draw(Canvas layer, int ox = 0, int oy = 0)
    {
        for (int y = 0; y < layer.Height; y++)
            for (int x = 0; x < layer.Width; x++)
                if (layer.IsSet(x, y)) this[x + ox, y + oy] = layer[x, y];
    }

    /// <summary>
    /// Draws a one-pixel outline just outside the layer's silhouette, where it borders unset pixels.
    /// </summary>
    public void Outline(Canvas layer, Func<int, int, Rgb> color, bool includeDiagonals = false)
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                if (layer.IsSet(x, y)) continue;
                bool edge = layer.IsSet(x - 1, y) || layer.IsSet(x + 1, y) || layer.IsSet(x, y - 1) || layer.IsSet(x, y + 1);
                if (!edge && includeDiagonals)
                    edge = layer.IsSet(x - 1, y - 1) || layer.IsSet(x + 1, y - 1) || layer.IsSet(x - 1, y + 1) || layer.IsSet(x + 1, y + 1);
                if (edge) this[x, y] = color(x, y);
            }
    }

    public Canvas Scaled(int factor)
    {
        var big = new Canvas(Width * factor, Height * factor);
        for (int y = 0; y < big.Height; y++)
            for (int x = 0; x < big.Width; x++)
                big[x, y] = this[x / factor, y / factor];
        return big;
    }

    public Canvas Crop(int x0, int y0, int w, int h)
    {
        var c = new Canvas(w, h);
        for (int y = 0; y < h; y++)
            for (int x = 0; x < w; x++)
                if (IsSet(x0 + x, y0 + y)) c[x, y] = this[x0 + x, y0 + y];
        return c;
    }
}

/// <summary>A hand-picked colour ramp from darkest to lightest. Light is quantised onto it.</summary>
public sealed class Ramp(params Rgb[] colors)
{
    public IReadOnlyList<Rgb> Colors { get; } = colors;

    public static Ramp Of(params string[] hex) => new(hex.Select(Rgb.Hex).ToArray());

    public Rgb Darkest => Colors[0];
    public Rgb Lightest => Colors[^1];

    /// <summary>Picks a colour for light level t in [0,1], with ordered dithering between neighbouring steps.</summary>
    public Rgb At(double t, int x, int y, double ditherStrength = 1.0)
    {
        t = Math.Clamp(t, 0, 1) * (Colors.Count - 1);
        int i = (int)Math.Floor(t);
        if (i >= Colors.Count - 1) return Colors[^1];
        double frac = t - i;
        // Only dither near the middle of a step so flat areas stay clean.
        double threshold = 0.5 + (Bayer.At(x, y) - 0.5) * ditherStrength;
        return frac > threshold ? Colors[i + 1] : Colors[i];
    }

    public Rgb Step(int i) => Colors[Math.Clamp(i, 0, Colors.Count - 1)];
}

public static class Bayer
{
    private static readonly int[,] M =
    {
        { 0, 8, 2, 10 },
        { 12, 4, 14, 6 },
        { 3, 11, 1, 9 },
        { 15, 7, 13, 5 },
    };

    /// <summary>Threshold in (0,1) for ordered dithering.</summary>
    public static double At(int x, int y) => (M[((y % 4) + 4) % 4, ((x % 4) + 4) % 4] + 0.5) / 16.0;
}
