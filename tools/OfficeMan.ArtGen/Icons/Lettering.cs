using OfficeMan.ArtGen.Drawn;

namespace OfficeMan.ArtGen.Icons;

/// <summary>
/// The game's own capital letters, drawn as centre lines and stroked with round ends, like a narrow rubber-stamp or
/// sign-writer's alphabet. Only the letters the title and the byline need. Cap height 100 units, y downwards.
/// </summary>
public static class Lettering
{
    private sealed record Glyph(double Width, List<List<Pt>> Lines);

    private static List<Pt> P(params double[] xy) =>
        Enumerable.Range(0, xy.Length / 2).Select(i => new Pt(xy[i * 2], xy[i * 2 + 1])).ToList();

    /// <summary>Part of an ellipse; angles in degrees, 0 to the right, 90 down.</summary>
    private static List<Pt> Arc(double cx, double cy, double rx, double ry, double from, double to, int n = 40) =>
        Enumerable.Range(0, n + 1).Select(i =>
        {
            double a = (from + (to - from) * i / n) * Math.PI / 180;
            return new Pt(cx + Math.Cos(a) * rx, cy + Math.Sin(a) * ry);
        }).ToList();

    private static readonly Dictionary<char, Glyph> Glyphs = new()
    {
        ['A'] = new(56, new() { P(0, 100, 28, 0, 56, 100), P(12, 64, 44, 64) }),
        ['B'] = new(50, new()
        {
            PathBuilder.From(0, 48).L(0, 0).L(26, 0).C(48, 0, 48, 48, 26, 48).L(0, 48).Points,
            PathBuilder.From(0, 48).L(28, 48).C(52, 48, 52, 100, 28, 100).L(0, 100).L(0, 48).Points,
        }),
        ['C'] = new(50, new() { Arc(26, 50, 26, 50, 38, 322) }),
        ['D'] = new(52, new() { PathBuilder.From(0, 0).L(18, 0).C(56, 0, 56, 100, 18, 100).L(0, 100).L(0, 0).Points }),
        ['E'] = new(42, new() { P(42, 0, 0, 0, 0, 100, 42, 100), P(0, 49, 34, 49) }),
        ['F'] = new(40, new() { P(40, 0, 0, 0, 0, 100), P(0, 49, 32, 49) }),
        ['G'] = new(52, new() { Arc(26, 50, 26, 50, 30, 322), P(30, 56, 52, 56, 51, 75) }),
        ['H'] = new(48, new() { P(0, 0, 0, 100), P(48, 0, 48, 100), P(0, 49, 48, 49) }),
        ['I'] = new(0, new() { P(0, 0, 0, 100) }),
        ['J'] = new(38, new() { PathBuilder.From(38, 0).L(38, 70).C(38, 104, 0, 104, 0, 74).Points }),
        ['K'] = new(48, new() { P(0, 0, 0, 100), P(48, 0, 0, 60), P(15, 46, 48, 100) }),
        ['M'] = new(62, new() { P(0, 100, 0, 0, 31, 64, 62, 0, 62, 100) }),
        ['N'] = new(50, new() { P(0, 100, 0, 0, 50, 100, 50, 0) }),
        ['O'] = new(54, new() { Arc(27, 50, 27, 50, 0, 360, 64) }),
        ['R'] = new(50, new() { PathBuilder.From(0, 100).L(0, 0).L(26, 0).C(50, 0, 50, 52, 26, 52).L(0, 52).Points, P(24, 52, 50, 100) }),
        ['S'] = new(48, new()
        {
            PathBuilder.From(46, 14).C(40, 3, 32, 0, 24, 0).C(8, 0, 2, 12, 2, 25).C(2, 54, 48, 44, 48, 74)
                .C(48, 90, 38, 100, 24, 100).C(12, 100, 4, 94, 0, 84).Points,
        }),
        ['T'] = new(48, new() { P(0, 0, 48, 0), P(24, 0, 24, 100) }),
        ['U'] = new(50, new() { PathBuilder.From(0, 0).L(0, 66).C(0, 104, 50, 104, 50, 66).L(50, 0).Points }),
        ['V'] = new(54, new() { P(0, 0, 27, 100, 54, 0) }),
        ['Y'] = new(52, new() { P(0, 0, 26, 52, 52, 0), P(26, 52, 26, 100) }),
    };

    /// <summary>One laid-out letter: its strokes (already placed and scaled) and any dots, as filled circles.</summary>
    public sealed record Placed(List<List<Pt>> Lines, List<(Pt centre, double radius)> Dots);

    /// <summary>Width of a line of text at this cap height, with this stroke weight and extra letter spacing.</summary>
    public static double Width(string text, double cap, double weight, double tracking = 0)
    {
        Layout(text, 0, 0, cap, weight, tracking, out double width);
        return width;
    }

    /// <summary>
    /// Lays out a line of capitals with its top-left at (x, y). Letters are spaced by their stroke weight plus
    /// <paramref name="tracking"/>. Ö is O with two dots.
    /// </summary>
    public static List<Placed> Layout(string text, double x, double y, double cap, double weight, double tracking = 0) =>
        Layout(text, x, y, cap, weight, tracking, out _);

    public static List<Placed> Layout(string text, double x, double y, double cap, double weight, double tracking, out double width)
    {
        double k = cap / 100, pen = x + weight / 2;
        var placed = new List<Placed>();
        foreach (char raw in text.ToUpperInvariant())
        {
            if (raw == ' ') { pen += cap * 0.32 + tracking; continue; }
            char ch = raw == 'Ö' ? 'O' : raw;
            if (!Glyphs.TryGetValue(ch, out var g)) throw new ArgumentException($"No letter '{raw}' drawn yet.");
            double ox = pen, oy = y;
            var lines = g.Lines.Select(l => l.Select(q => new Pt(ox + q.X * k, oy + q.Y * k)).ToList()).ToList();
            var dots = new List<(Pt, double)>();
            if (raw == 'Ö')
                foreach (double dx in new[] { 0.28, 0.72 })
                    dots.Add((new Pt(ox + g.Width * k * dx, oy - cap * 0.17), weight * 0.6));
            placed.Add(new Placed(lines, dots));
            pen += g.Width * k + weight + cap * 0.1 + tracking;
        }
        width = pen - (cap * 0.1 + tracking) - weight / 2 - x;
        return placed;
    }
}
