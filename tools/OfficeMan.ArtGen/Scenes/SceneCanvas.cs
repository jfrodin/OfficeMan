using OfficeMan.ArtGen.Raster;

namespace OfficeMan.ArtGen.Scenes;

/// <summary>
/// A scene painted as materials plus light. Shapes assign a material and a base brightness;
/// lights add to the brightness; the final pass quantises each pixel onto its material's ramp.
/// Keeping light separate from colour is what makes a scene feel lit by one sun.
/// </summary>
public sealed class SceneCanvas
{
    public int Width { get; }
    public int Height { get; }

    private readonly Ramp?[] _material;
    private readonly double[] _base;
    private readonly double[] _light;
    private readonly Rgb?[] _fixed;
    private readonly double[] _dither;
    private readonly byte[] _tag;

    /// <summary>
    /// The thing being drawn, as its number in the room's mask (0: nothing in particular). Every pixel painted
    /// while it's set belongs to that thing, so the game can light it up in its own shape.
    /// </summary>
    public byte Tag { get; set; }

    public SceneCanvas(int width, int height)
    {
        Width = width;
        Height = height;
        _material = new Ramp?[width * height];
        _base = new double[width * height];
        _light = new double[width * height];
        _fixed = new Rgb?[width * height];
        _dither = new double[width * height];
        _tag = new byte[width * height];
    }

    private bool In(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;

    /// <summary>Paints a material with a per-pixel base brightness (form shading).</summary>
    public void Paint(int x, int y, Ramp material, double baseLight, double dither = 0.6)
    {
        if (!In(x, y)) return;
        int i = y * Width + x;
        _material[i] = material;
        _base[i] = baseLight;
        _fixed[i] = null;
        _dither[i] = dither;
        _tag[i] = Tag;
    }

    /// <summary>Paints an unlit, exact colour (lit windows, clock digits, highlights).</summary>
    public void Emit(int x, int y, Rgb color)
    {
        if (!In(x, y)) return;
        _fixed[y * Width + x] = color;
        _tag[y * Width + x] = Tag;
    }

    public void AddLight(int x, int y, double amount)
    {
        if (In(x, y)) _light[y * Width + x] += amount;
    }

    public Ramp? MaterialAt(int x, int y) => In(x, y) ? _material[y * Width + x] : null;

    public void FillPolygon(IReadOnlyList<(double x, double y)> poly, Ramp material, Func<int, int, double> baseLight, double dither = 0.6)
    {
        ForEachInPolygon(poly, (x, y) => Paint(x, y, material, baseLight(x, y), dither));
    }

    public void FillRect(int x0, int y0, int x1, int y1, Ramp material, Func<int, int, double> baseLight, double dither = 0.6)
    {
        for (int y = y0; y < y1; y++)
            for (int x = x0; x < x1; x++)
                Paint(x, y, material, baseLight(x, y), dither);
    }

    public static void ForEachInPolygon(IReadOnlyList<(double x, double y)> poly, Action<int, int> action)
    {
        double minY = poly.Min(p => p.y), maxY = poly.Max(p => p.y);
        var xs = new List<double>();
        for (int y = (int)Math.Floor(minY); y <= (int)Math.Ceiling(maxY); y++)
        {
            double sy = y + 0.5;
            xs.Clear();
            for (int i = 0; i < poly.Count; i++)
            {
                var a = poly[i];
                var b = poly[(i + 1) % poly.Count];
                if ((a.y <= sy && b.y > sy) || (b.y <= sy && a.y > sy))
                    xs.Add(a.x + (sy - a.y) / (b.y - a.y) * (b.x - a.x));
            }
            xs.Sort();
            for (int k = 0; k + 1 < xs.Count; k += 2)
                for (int x = (int)Math.Ceiling(xs[k] - 0.5); x <= (int)Math.Floor(xs[k + 1] - 0.5); x++)
                    action(x, y);
        }
    }

    public static bool InsidePolygon(IReadOnlyList<(double x, double y)> poly, double px, double py)
    {
        bool inside = false;
        for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
        {
            var a = poly[i];
            var b = poly[j];
            if ((a.y > py) != (b.y > py) && px < (b.x - a.x) * (py - a.y) / (b.y - a.y) + a.x) inside = !inside;
        }
        return inside;
    }

    /// <summary>The mask: each pixel's thing number in red, black where it's nothing in particular.</summary>
    public Canvas Mask()
    {
        var c = new Canvas(Width, Height);
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                c[x, y] = new Rgb(_tag[y * Width + x], 0, 0);
        return c;
    }

    public Canvas Render()
    {
        var c = new Canvas(Width, Height);
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                int i = y * Width + x;
                if (_fixed[i] is Rgb f) { c[x, y] = f; continue; }
                if (_material[i] is not Ramp m) continue;
                c[x, y] = m.At(_base[i] + _light[i], x, y, _dither[i]);
            }
        return c;
    }
}
