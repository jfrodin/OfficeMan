using OfficeMan.ArtGen.Drawn;
using OfficeMan.ArtGen.Raster;

namespace OfficeMan.ArtGen.Icons;

/// <summary>
/// Paint plus coverage, so icons and logos get real soft edges: colour is painted over black, coverage is painted white
/// over black with the same alpha, and the colour is divided back out at the end.
/// </summary>
public sealed class SoftLayer
{
    private readonly int _w, _h;
    private readonly double _zoom;
    private readonly Sketch _paint, _cover;
    private static readonly Rgb White = new(255, 255, 255), Black = new(0, 0, 0);

    /// <param name="zoom">Pixels per drawing unit.</param>
    public SoftLayer(int width, int height, double zoom, int supersample = 4)
    {
        _w = width;
        _h = height;
        _zoom = zoom;
        _paint = new Sketch(width, height, supersample);
        _cover = new Sketch(width, height, supersample);
        _paint.Clear((_, _) => Black);
        _cover.Clear((_, _) => Black);
        _paint.SetView(zoom, new Pt(0, 0));
        _cover.SetView(zoom, new Pt(0, 0));
    }

    public void Fill(List<Pt> poly, string hex, Func<double, double, double>? alphaAt = null, List<Pt>? clip = null)
    {
        _paint.Fill(poly, Rgb.Hex(hex), 1, clip, alphaAt);
        _cover.Fill(poly, White, 1, clip, alphaAt);
    }

    public void Stroke(List<Pt> pts, double width, string hex, Func<double, double, double>? alphaAt = null)
    {
        if (alphaAt is null)
        {
            _paint.Stroke(pts, width, Rgb.Hex(hex));
            _cover.Stroke(pts, width, White);
            return;
        }
        // Textured ink: the stroke as a filled band, so the texture can thin it.
        var band = Band(pts, width / 2);
        _paint.Fill(band, Rgb.Hex(hex), 1, null, alphaAt);
        _cover.Fill(band, White, 1, null, alphaAt);
    }

    /// <summary>A closed loop of points either side of a closed polyline: a ring for Fill (even-odd keeps the hole).</summary>
    private static List<Pt> Band(List<Pt> pts, double half)
    {
        bool closed = (pts[0] - pts[^1]).Length < 0.01;
        var outer = new List<Pt>();
        var inner = new List<Pt>();
        for (int i = 0; i < pts.Count; i++)
        {
            var prev = pts[Math.Max(0, i - 1)];
            var next = pts[Math.Min(pts.Count - 1, i + 1)];
            if (closed && i == 0) prev = pts[^2];
            if (closed && i == pts.Count - 1) next = pts[1];
            var d = next - prev;
            double len = Math.Max(1e-6, d.Length);
            var n = new Pt(-d.Y / len * half, d.X / len * half);
            outer.Add(pts[i] + n);
            inner.Add(pts[i] - n);
        }
        inner.Reverse();
        return outer.Concat(inner).ToList();
    }

    /// <param name="texture">Optional 0-1 multiplier on opacity, in drawing units: ink that didn't take everywhere.</param>
    public Canvas ToCanvas(Func<double, double, double>? texture = null)
    {
        var colour = _paint.ToCanvas();
        var cover = _cover.ToCanvas();
        var c = new Canvas(_w, _h);
        for (int y = 0; y < _h; y++)
            for (int x = 0; x < _w; x++)
            {
                int a = cover[x, y].R;
                if (texture is not null && a > 0) a = (int)(a * Math.Clamp(texture((x + 0.5) / _zoom, (y + 0.5) / _zoom), 0, 1));
                if (a < 3) continue;
                var p = colour[x, y];
                double k = 255.0 / Math.Max(1, (int)cover[x, y].R);
                c.SetSoft(x, y, new Rgb((byte)Math.Min(255, p.R * k), (byte)Math.Min(255, p.G * k), (byte)Math.Min(255, p.B * k)), (byte)a);
            }
        return c;
    }
}
