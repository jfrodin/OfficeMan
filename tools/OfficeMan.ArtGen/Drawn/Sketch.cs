using OfficeMan.ArtGen.Raster;

namespace OfficeMan.ArtGen.Drawn;

public readonly record struct Pt(double X, double Y)
{
    public static Pt operator +(Pt a, Pt b) => new(a.X + b.X, a.Y + b.Y);
    public static Pt operator -(Pt a, Pt b) => new(a.X - b.X, a.Y - b.Y);
    public static Pt operator *(Pt a, double k) => new(a.X * k, a.Y * k);
    public double Length => Math.Sqrt(X * X + Y * Y);
}

/// <summary>Builds polylines from cubic Bézier segments, in drawing units.</summary>
public sealed class PathBuilder : IReadOnlyList<Pt>
{
    private readonly List<Pt> _pts = new();
    private Pt _cur;

    public static PathBuilder From(double x, double y)
    {
        var p = new PathBuilder { _cur = new Pt(x, y) };
        p._pts.Add(p._cur);
        return p;
    }

    public PathBuilder C(double x1, double y1, double x2, double y2, double x, double y, int steps = 24)
    {
        Pt p0 = _cur, p1 = new(x1, y1), p2 = new(x2, y2), p3 = new(x, y);
        for (int i = 1; i <= steps; i++)
        {
            double t = i / (double)steps, u = 1 - t;
            _pts.Add(p0 * (u * u * u) + p1 * (3 * u * u * t) + p2 * (3 * u * t * t) + p3 * (t * t * t));
        }
        _cur = p3;
        return this;
    }

    public PathBuilder L(double x, double y)
    {
        _cur = new Pt(x, y);
        _pts.Add(_cur);
        return this;
    }

    public List<Pt> Points => _pts.ToList();

    public int Count => _pts.Count;
    public Pt this[int index] => _pts[index];
    public IEnumerator<Pt> GetEnumerator() => _pts.GetEnumerator();
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    public static implicit operator List<Pt>(PathBuilder b) => b.Points;
}

public static class Shapes
{
    public static List<Pt> Ellipse(double cx, double cy, double rx, double ry, int n = 48) =>
        Enumerable.Range(0, n).Select(i =>
        {
            double a = i / (double)n * Math.PI * 2;
            return new Pt(cx + Math.Cos(a) * rx, cy + Math.Sin(a) * ry);
        }).ToList();

    /// <summary>
    /// Pushes a run of outline points outward into round bumps: the edge of a perm, a fur trim.
    /// Outward is away from <paramref name="centre"/>.
    /// </summary>
    public static List<Pt> Scallop(IReadOnlyList<Pt> pts, Pt centre, double amplitude, double bumpLength, int seed = 0)
    {
        var rng = new Random(seed);
        var result = new List<Pt>(pts.Count);
        double s = 0;
        double phaseJitter = rng.NextDouble();
        for (int i = 0; i < pts.Count; i++)
        {
            if (i > 0) s += (pts[i] - pts[i - 1]).Length;
            var dir = pts[i] - centre;
            double len = Math.Max(1e-6, dir.Length);
            double bump = Math.Abs(Math.Sin(Math.PI * (s / bumpLength + phaseJitter)));
            result.Add(pts[i] + dir * (amplitude * Math.Pow(bump, 0.7) / len));
        }
        return result;
    }

    public static bool Inside(IReadOnlyList<Pt> poly, double px, double py)
    {
        bool inside = false;
        for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
        {
            var a = poly[i];
            var b = poly[j];
            if ((a.Y > py) != (b.Y > py) && px < (b.X - a.X) * (py - a.Y) / (b.Y - a.Y) + a.X) inside = !inside;
        }
        return inside;
    }
}

/// <summary>Smooth 2D value noise for hand wobble and paper texture.</summary>
public static class Noise
{
    public static double Value(double x, double y, int seed)
    {
        int x0 = (int)Math.Floor(x), y0 = (int)Math.Floor(y);
        double fx = x - x0, fy = y - y0;
        double sx = fx * fx * (3 - 2 * fx), sy = fy * fy * (3 - 2 * fy);
        double a = H(x0, y0, seed), b = H(x0 + 1, y0, seed), c = H(x0, y0 + 1, seed), d = H(x0 + 1, y0 + 1, seed);
        return (a + (b - a) * sx) + ((c + (d - c) * sx) - (a + (b - a) * sx)) * sy;
    }

    /// <summary>Two octaves, roughly in [-1, 1].</summary>
    public static double Fbm(double x, double y, int seed) =>
        (Value(x, y, seed) * 2 - 1) * 0.67 + (Value(x * 2.1, y * 2.1, seed + 17) * 2 - 1) * 0.33;

    private static double H(int x, int y, int seed)
    {
        uint h = (uint)(x * 374761393 + y * 668265263 + seed * 1442695041);
        h = (h ^ (h >> 13)) * 1274126177;
        h ^= h >> 16;
        return (h & 0xFFFFFF) / (double)0xFFFFFF;
    }
}

/// <summary>
/// A supersampled RGB painting surface in drawing units. Everything is anti-aliased by
/// rendering at <c>scale</c>× and box-filtering down at the end.
/// </summary>
public sealed class Sketch
{
    public int Width { get; }
    public int Height { get; }
    public int Scale { get; }
    private readonly int _w, _h;
    private readonly double[] _r, _g, _b;
    private readonly float[] _cov;

    public Sketch(int width, int height, int scale = 3)
    {
        Width = width;
        Height = height;
        Scale = scale;
        _w = width * scale;
        _h = height * scale;
        _r = new double[_w * _h];
        _g = new double[_w * _h];
        _b = new double[_w * _h];
        _cov = new float[_w * _h];
    }

    public void Clear(Func<double, double, Rgb> paint)
    {
        for (int y = 0; y < _h; y++)
            for (int x = 0; x < _w; x++)
            {
                var c = paint(x / (double)Scale, y / (double)Scale);
                int i = y * _w + x;
                _r[i] = c.R; _g[i] = c.G; _b[i] = c.B;
            }
    }

    private void Blend(int i, Rgb c, double a)
    {
        _r[i] += (c.R - _r[i]) * a;
        _g[i] += (c.G - _g[i]) * a;
        _b[i] += (c.B - _b[i]) * a;
    }

    /// <summary>Multiplies like ink or graphite laid on paper (darkens, never lightens).</summary>
    private void Multiply(int i, Rgb c, double a)
    {
        _r[i] *= 1 - a + a * c.R / 255.0;
        _g[i] *= 1 - a + a * c.G / 255.0;
        _b[i] *= 1 - a + a * c.B / 255.0;
    }

    public bool MultiplyMode { get; set; }

    // ── View: zoom about a fixed point, so a drawing can be framed tighter without redrawing it ──
    private double _zoom = 1;
    private Pt _focus;

    /// <summary>Scales everything drawn afterwards by <paramref name="zoom"/> about <paramref name="focus"/>.</summary>
    public void SetView(double zoom, Pt focus)
    {
        _zoom = zoom;
        _focus = focus;
    }

    private Pt ToCanvas(Pt p) => new((p.X - _focus.X) * _zoom + _focus.X, (p.Y - _focus.Y) * _zoom + _focus.Y);

    private Pt ToDrawing(double x, double y) => new((x - _focus.X) / _zoom + _focus.X, (y - _focus.Y) / _zoom + _focus.Y);

    private IReadOnlyList<Pt> View(IReadOnlyList<Pt> pts) => _zoom == 1 ? pts : pts.Select(ToCanvas).ToList();

    private IReadOnlyList<Pt>? ViewOrNull(IReadOnlyList<Pt>? pts) => pts is null ? null : View(pts);

    private void Apply(int i, Rgb c, double a)
    {
        if (a <= 0) return;
        if (MultiplyMode) Multiply(i, c, a);
        else Blend(i, c, a);
    }

    /// <summary>Fills a polygon. Optional clip polygon; optional per-point alpha (gradients, texture).</summary>
    public void Fill(IReadOnlyList<Pt> poly, Rgb color, double alpha = 1, IReadOnlyList<Pt>? clip = null,
        Func<double, double, double>? alphaAt = null)
    {
        Func<double, double, double>? at = alphaAt is null ? null : (x, y) =>
        {
            var d = ToDrawing(x, y);
            return alphaAt(d.X, d.Y);
        };
        FillRaw(View(poly), color, alpha, ViewOrNull(clip), at);
    }

    private void FillRaw(IReadOnlyList<Pt> poly, Rgb color, double alpha, IReadOnlyList<Pt>? clip,
        Func<double, double, double>? alphaAt)
    {
        double minX = poly.Min(p => p.X), maxX = poly.Max(p => p.X);
        double minY = poly.Min(p => p.Y), maxY = poly.Max(p => p.Y);
        int y0 = Math.Max(0, (int)(minY * Scale)), y1 = Math.Min(_h - 1, (int)(maxY * Scale) + 1);
        var xs = new List<double>();
        for (int y = y0; y <= y1; y++)
        {
            double sy = (y + 0.5) / Scale;
            xs.Clear();
            for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
            {
                var a = poly[i];
                var b = poly[j];
                if ((a.Y <= sy && b.Y > sy) || (b.Y <= sy && a.Y > sy))
                    xs.Add(a.X + (sy - a.Y) / (b.Y - a.Y) * (b.X - a.X));
            }
            xs.Sort();
            for (int k = 0; k + 1 < xs.Count; k += 2)
            {
                int xa = Math.Max(0, (int)Math.Ceiling(xs[k] * Scale - 0.5));
                int xb = Math.Min(_w - 1, (int)Math.Floor(xs[k + 1] * Scale - 0.5));
                for (int x = xa; x <= xb; x++)
                {
                    double px = (x + 0.5) / Scale;
                    if (clip is not null && !Shapes.Inside(clip, px, sy)) continue;
                    double a = alpha * (alphaAt?.Invoke(px, sy) ?? 1);
                    Apply(y * _w + x, color, Math.Clamp(a, 0, 1));
                }
            }
        }
    }

    /// <summary>
    /// Strokes a polyline. Width in drawing units; taper thins both ends; wobble displaces the
    /// line with smooth noise, like a hand. Each stroke is applied once, so overlaps don't darken.
    /// </summary>
    public void Stroke(IReadOnlyList<Pt> pts, double width, Rgb color, double alpha = 1, bool taper = false,
        double wobble = 0, int seed = 0, IReadOnlyList<Pt>? clip = null)
    {
        if (pts.Count < 2) return;
        pts = View(pts);
        clip = ViewOrNull(clip);
        width *= _zoom;
        wobble *= _zoom;
        var line = pts;
        if (wobble > 0)
        {
            var w = new List<Pt>(pts.Count);
            double s = 0;
            for (int i = 0; i < pts.Count; i++)
            {
                if (i > 0) s += (pts[i] - pts[i - 1]).Length;
                w.Add(new Pt(pts[i].X + Noise.Fbm(s * 0.05, seed * 3.1, seed) * wobble,
                             pts[i].Y + Noise.Fbm(s * 0.05, seed * 3.1 + 40, seed + 1) * wobble));
            }
            line = w;
        }

        double total = 0;
        var cumulative = new double[line.Count];
        for (int i = 1; i < line.Count; i++) cumulative[i] = total += (line[i] - line[i - 1]).Length;

        int minX = _w, minY = _h, maxX = 0, maxY = 0;
        for (int i = 0; i < line.Count - 1; i++)
        {
            Pt a = line[i] * Scale, b = line[i + 1] * Scale;
            double wa = Width(cumulative[i]), wb = Width(cumulative[i + 1]);
            double r = Math.Max(wa, wb) * Scale / 2 + 1;
            int bx0 = Math.Max(0, (int)(Math.Min(a.X, b.X) - r)), bx1 = Math.Min(_w - 1, (int)(Math.Max(a.X, b.X) + r));
            int by0 = Math.Max(0, (int)(Math.Min(a.Y, b.Y) - r)), by1 = Math.Min(_h - 1, (int)(Math.Max(a.Y, b.Y) + r));
            var ab = b - a;
            double abLen2 = Math.Max(1e-9, ab.X * ab.X + ab.Y * ab.Y);
            for (int y = by0; y <= by1; y++)
                for (int x = bx0; x <= bx1; x++)
                {
                    var p = new Pt(x + 0.5, y + 0.5);
                    double t = Math.Clamp(((p.X - a.X) * ab.X + (p.Y - a.Y) * ab.Y) / abLen2, 0, 1);
                    var q = a + ab * t;
                    double d = (p - q).Length;
                    double half = (wa + (wb - wa) * t) * Scale / 2;
                    float cov = (float)Math.Clamp(half - d + 0.5, 0, 1);
                    int i2 = y * _w + x;
                    if (cov > _cov[i2]) _cov[i2] = cov;
                }
            minX = Math.Min(minX, bx0); maxX = Math.Max(maxX, bx1);
            minY = Math.Min(minY, by0); maxY = Math.Max(maxY, by1);
        }

        for (int y = minY; y <= maxY; y++)
            for (int x = minX; x <= maxX; x++)
            {
                int i = y * _w + x;
                if (_cov[i] <= 0) continue;
                if (clip is null || Shapes.Inside(clip, (x + 0.5) / Scale, (y + 0.5) / Scale))
                    Apply(i, color, alpha * _cov[i]);
                _cov[i] = 0;
            }

        double Width(double s)
        {
            if (!taper || total <= 0) return width;
            double u = s / total;
            return Math.Max(width * 0.15, width * Math.Pow(Math.Sin(Math.PI * Math.Clamp(u, 0, 1)), 0.55));
        }
    }

    /// <summary>
    /// Parallel hatching inside a polygon. Density 0..1 thins the lines; wobble bends them like a hand;
    /// gaps break lines up the way a pencil skips on paper.
    /// </summary>
    public void Hatch(IReadOnlyList<Pt> poly, double angleDeg, double spacing, double width, Rgb color,
        double alpha = 1, double wobble = 0, int seed = 0, IReadOnlyList<Pt>? clip = null, double gaps = 0,
        Func<double, double, bool>? mask = null)
    {
        double a = angleDeg * Math.PI / 180, ca = Math.Cos(a), sa = Math.Sin(a);
        spacing *= _zoom;
        width *= _zoom;
        wobble *= _zoom;
        FillRaw(View(poly), color, alpha, ViewOrNull(clip), (px, py) =>
        {
            if (mask is not null)
            {
                var dp = ToDrawing(px, py);
                if (!mask(dp.X, dp.Y)) return 0;
            }
            double u = px * ca + py * sa;
            double v = -px * sa + py * ca;
            if (wobble > 0) v += Noise.Fbm(u * 0.04, v * 0.02, seed) * wobble;
            double m = ((v % spacing) + spacing) % spacing;
            double d = Math.Min(m, spacing - m);
            double cov = Math.Clamp((width / 2 - d) * Scale + 0.5, 0, 1);
            if (gaps > 0 && Noise.Value(u * 0.08, Math.Floor(v / spacing) * 7.3, seed + 5) < gaps) cov *= 0.15;
            return cov;
        });
    }

    /// <summary>
    /// Fills a polygon with a soft, blurred edge, like a brush loaded with paint. Used for
    /// painterly shading: shapes are designed by hand, then their edges are melted.
    /// </summary>
    public void SoftFill(IReadOnlyList<Pt> poly, Rgb color, double alpha, double blur, IReadOnlyList<Pt>? clip = null)
    {
        poly = View(poly);
        clip = ViewOrNull(clip);
        blur *= _zoom;
        int r = Math.Max(1, (int)(blur * Scale));
        int x0 = Math.Max(0, (int)(poly.Min(p => p.X) * Scale) - r * 2), x1 = Math.Min(_w - 1, (int)(poly.Max(p => p.X) * Scale) + r * 2);
        int y0 = Math.Max(0, (int)(poly.Min(p => p.Y) * Scale) - r * 2), y1 = Math.Min(_h - 1, (int)(poly.Max(p => p.Y) * Scale) + r * 2);
        int bw = x1 - x0 + 1, bh = y1 - y0 + 1;
        var m = Rasterise(poly, x0, y0, bw, bh);
        var clipMask = clip is null ? null : Rasterise(clip, x0, y0, bw, bh);

        // Three box-blur passes approximate a Gaussian.
        var tmp = new float[bw * bh];
        for (int pass = 0; pass < 3; pass++)
        {
            BoxBlur(m, tmp, bw, bh, r, horizontal: true);
            BoxBlur(tmp, m, bw, bh, r, horizontal: false);
        }

        for (int y = 0; y < bh; y++)
            for (int x = 0; x < bw; x++)
            {
                float a = m[y * bw + x];
                if (a <= 0.002f) continue;
                if (clipMask is not null && clipMask[y * bw + x] == 0) continue;
                Apply((y0 + y) * _w + x0 + x, color, alpha * a);
            }
    }

    /// <summary>Scanline-fills a polygon (canvas units) into a 0/1 mask covering a sub-rectangle.</summary>
    private float[] Rasterise(IReadOnlyList<Pt> poly, int x0, int y0, int bw, int bh)
    {
        var m = new float[bw * bh];
        var xs = new List<double>();
        for (int y = 0; y < bh; y++)
        {
            double sy = (y0 + y + 0.5) / Scale;
            xs.Clear();
            for (int i = 0, j = poly.Count - 1; i < poly.Count; j = i++)
            {
                var a = poly[i];
                var b = poly[j];
                if ((a.Y <= sy && b.Y > sy) || (b.Y <= sy && a.Y > sy))
                    xs.Add(a.X + (sy - a.Y) / (b.Y - a.Y) * (b.X - a.X));
            }
            xs.Sort();
            for (int k = 0; k + 1 < xs.Count; k += 2)
            {
                int xa = Math.Max(0, (int)Math.Ceiling(xs[k] * Scale - 0.5) - x0);
                int xb = Math.Min(bw - 1, (int)Math.Floor(xs[k + 1] * Scale - 0.5) - x0);
                for (int x = xa; x <= xb; x++) m[y * bw + x] = 1;
            }
        }
        return m;
    }

    private static void BoxBlur(float[] src, float[] dst, int w, int h, int r, bool horizontal)
    {
        int len = horizontal ? w : h, lines = horizontal ? h : w;
        float inv = 1f / (2 * r + 1);
        for (int l = 0; l < lines; l++)
        {
            float acc = 0;
            int Index(int i) => horizontal ? l * w + Math.Clamp(i, 0, w - 1) : Math.Clamp(i, 0, h - 1) * w + l;
            for (int i = -r; i <= r; i++) acc += src[Index(i)];
            for (int i = 0; i < len; i++)
            {
                dst[Index(i)] = acc * inv;
                acc += src[Index(i + r + 1)] - src[Index(i - r)];
            }
        }
    }

    public Canvas ToCanvas(double grain = 0, int seed = 1)
    {
        var c = new Canvas(Width, Height);
        var rng = new Random(seed);
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                double r = 0, g = 0, b = 0;
                for (int j = 0; j < Scale; j++)
                    for (int i = 0; i < Scale; i++)
                    {
                        int k = (y * Scale + j) * _w + x * Scale + i;
                        r += _r[k]; g += _g[k]; b += _b[k];
                    }
                double n = Scale * Scale;
                double f = grain > 0 ? 1 + (rng.NextDouble() - 0.5) * 2 * grain : 1;
                c[x, y] = new Rgb(
                    (byte)Math.Clamp(r / n * f, 0, 255),
                    (byte)Math.Clamp(g / n * f, 0, 255),
                    (byte)Math.Clamp(b / n * f, 0, 255));
            }
        return c;
    }
}
