using OfficeMan.ArtGen.Drawn;
using OfficeMan.ArtGen.Raster;

namespace OfficeMan.ArtGen.Icons;

/// <summary>
/// Proposals for the game's Windows icon (start menu, taskbar, desktop), 2026-10-27. Each is drawn in a 256-unit
/// square and rendered at any size; at 32 px and below a simpler version is drawn (thicker lines, no texture), so
/// the small icons stay clear instead of being blurred copies of the big one.
///   stamp         a blue rubber-stamp mark with the initials OM
///   stamp_paper   the same mark on a slip of paper, so it reads on a dark taskbar too
///   rubber_stamp  the stamp itself, wooden handle and all, over a blot of ink
///   head          Teo's head and shoulders as a silhouette on stamp blue
/// </summary>
public static class AppIcons
{
    public static readonly string[] Names = { "stamp", "stamp_paper", "rubber_stamp", "head" };
    public static readonly int[] Sizes = { 256, 48, 32, 16 };

    private const string StampBlue = "2f56b0", Paper = "f2e6cc";

    public static Canvas Draw(string name, int size)
    {
        var l = new Layer(size);
        bool small = size <= 32;
        switch (name)
        {
            case "stamp": Stamp(l, small); break;
            case "stamp_paper":
                l.Fill(Turn(4)(RoundSquare(14, 14, 242, 242, small ? 22 : 18)), Paper);
                Stamp(l, small, inset: true);
                break;
            case "rubber_stamp": RubberStamp(l, small); break;
            case "head": Head(l, small); break;
            default: throw new ArgumentException(name);
        }
        return l.ToCanvas();
    }

    // ── The designs ───────────────────────────────────

    private static void Stamp(Layer l, bool small, bool inset = false)
    {
        // Turned a little, as if stamped by hand; the ink thin in places.
        Func<double, double, double>? ink = small ? null : (x, y) =>
            Math.Clamp(0.92 + 0.7 * Noise.Fbm(x * 0.035, y * 0.035, 11) - (Noise.Value(x * 0.3, y * 0.3, 4) > 0.86 ? 0.8 : 0), 0, 1);
        var shrink = inset ? 0.84 : 1.0;
        var rotate = Turn(-9);
        Func<List<Pt>, List<Pt>> turn = pts => rotate(pts.Select(p => new Pt(128 + (p.X - 128) * shrink, 128 + (p.Y - 128) * shrink)).ToList());

        l.Stroke(turn(Closed(Shapes.Ellipse(128, 128, small ? 114 : 110, small ? 114 : 110, 96))), small ? 24 : 15, StampBlue, ink);
        if (!small)
        {
            l.Stroke(turn(Closed(Shapes.Ellipse(128, 128, 86, 86, 96))), 4, StampBlue, ink);
            for (int i = 0; i < 28; i++)
            {
                double a = i / 28.0 * Math.Tau;
                l.Fill(turn(Shapes.Ellipse(128 + Math.Cos(a) * 98, 128 + Math.Sin(a) * 98, 3, 3, 12)), StampBlue, ink);
            }
        }
        // O and M.
        double w = small ? 24 : 15;
        if (small)
        {
            l.Stroke(turn(Closed(Shapes.Ellipse(82, 128, 28, 46, 48))), w, StampBlue);
            l.Stroke(turn(Poly(124, 176, 126, 82, 158, 140, 190, 82, 192, 176)), w, StampBlue);
        }
        else
        {
            l.Stroke(turn(Closed(Shapes.Ellipse(90, 128, 26, 38, 48))), w, StampBlue, ink);
            l.Stroke(turn(Poly(132, 168, 134, 90, 160, 138, 186, 90, 188, 168)), w, StampBlue, ink);
        }
    }

    private static void RubberStamp(Layer l, bool small)
    {
        Func<double, double, double>? ink = small ? null : (x, y) => Math.Clamp(0.9 + 0.6 * Noise.Fbm(x * 0.05, y * 0.05, 3), 0, 1);
        // The blot it has just left.
        l.Fill(Shapes.Ellipse(128, 214, small ? 112 : 104, small ? 30 : 26, 64), StampBlue, ink);
        // Base: a wooden block, its top lit, rubber underneath.
        l.Fill(Poly(46, 150, 210, 150, 210, 192, 46, 192), "8a5a34");
        l.Fill(Poly(46, 150, 210, 150, 194, 134, 62, 134), "c08a56");
        l.Fill(Poly(50, 192, 206, 192, 204, 204, 52, 204), small ? "1c1c22" : "2a2a30");
        if (!small) l.Fill(Poly(46, 150, 62, 150, 62, 192, 46, 192), "6e4428");
        // Neck and the round handle.
        l.Fill(Poly(110, 92, 146, 92, 150, 140, 106, 140), "6e4428");
        if (!small) l.Fill(Poly(132, 92, 142, 92, 145, 140, 135, 140), "8a5a34");
        l.Fill(Shapes.Ellipse(128, 66, 50, 40, 64), "a8322e");
        l.Fill(Shapes.Ellipse(142, 52, small ? 16 : 18, small ? 10 : 11, 32), "d8605a");
    }

    private static void Head(Layer l, bool small)
    {
        l.Fill(RoundSquare(12, 12, 244, 244, small ? 40 : 48), StampBlue);
        // Shoulders with an open collar, the neck, the head, messy hair on top.
        var body = PathBuilder.From(36, 256).C(36, 196, 70, 182, 104, 176).L(128, 204).L(152, 176)
            .C(186, 182, 220, 196, 220, 256).Points;
        l.Fill(body, Paper, clip: RoundSquare(12, 12, 244, 244, small ? 40 : 48));
        l.Fill(Poly(110, 150, 146, 150, 150, 182, 128, 204, 106, 182), Paper);
        l.Fill(Shapes.Ellipse(128, 112, 42, 52, 64), Paper);
        var hair = Shapes.Scallop(Shapes.Ellipse(126, 82, small ? 50 : 48, small ? 32 : 30, 64).Where(p => p.Y < 92).ToList(),
            new Pt(126, 100), small ? 9 : 7, small ? 26 : 18, seed: 5);
        l.Fill(hair, Paper);
        if (!small)
        {
            // The collar's line, the only detail.
            l.Stroke(Poly(104, 176, 128, 204, 152, 176), 5, StampBlue);
        }
    }

    // ── Helpers ───────────────────────────────────────

    private static List<Pt> Poly(params double[] xy) =>
        Enumerable.Range(0, xy.Length / 2).Select(i => new Pt(xy[i * 2], xy[i * 2 + 1])).ToList();

    private static List<Pt> Closed(List<Pt> pts) => pts.Append(pts[0]).ToList();

    private static Func<List<Pt>, List<Pt>> Turn(double degrees)
    {
        double a = degrees * Math.PI / 180, c = Math.Cos(a), s = Math.Sin(a);
        return pts => pts.Select(p => new Pt(128 + (p.X - 128) * c - (p.Y - 128) * s, 128 + (p.X - 128) * s + (p.Y - 128) * c)).ToList();
    }

    private static List<Pt> RoundSquare(double x0, double y0, double x1, double y1, double r)
    {
        var pts = new List<Pt>();
        void Corner(double cx, double cy, double from)
        {
            for (int i = 0; i <= 8; i++)
            {
                double a = (from + i * 90 / 8.0) * Math.PI / 180;
                pts.Add(new Pt(cx + Math.Cos(a) * r, cy + Math.Sin(a) * r));
            }
        }
        Corner(x1 - r, y0 + r, 270);
        Corner(x1 - r, y1 - r, 0);
        Corner(x0 + r, y1 - r, 90);
        Corner(x0 + r, y0 + r, 180);
        return pts;
    }

    /// <summary>
    /// Paint plus coverage, so the icon gets real soft edges: colour is painted over black, coverage is painted white
    /// over black with the same alpha, and the colour is divided back out at the end.
    /// </summary>
    private sealed class Layer
    {
        private readonly int _size;
        private readonly Sketch _paint, _cover;
        private static readonly Rgb White = new(255, 255, 255), Black = new(0, 0, 0);

        public Layer(int size)
        {
            _size = size;
            _paint = new Sketch(size, size, 4);
            _cover = new Sketch(size, size, 4);
            _paint.Clear((_, _) => Black);
            _cover.Clear((_, _) => Black);
            _paint.SetView(size / 256.0, new Pt(0, 0));
            _cover.SetView(size / 256.0, new Pt(0, 0));
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

        public Canvas ToCanvas()
        {
            var colour = _paint.ToCanvas();
            var cover = _cover.ToCanvas();
            var c = new Canvas(_size, _size);
            for (int y = 0; y < _size; y++)
                for (int x = 0; x < _size; x++)
                {
                    int a = cover[x, y].R;
                    if (a < 3) continue;
                    var p = colour[x, y];
                    double k = 255.0 / a;
                    c.SetSoft(x, y, new Rgb((byte)Math.Min(255, p.R * k), (byte)Math.Min(255, p.G * k), (byte)Math.Min(255, p.B * k)), (byte)a);
                }
            return c;
        }
    }
}
