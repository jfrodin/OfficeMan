using OfficeMan.ArtGen.Drawn;
using OfficeMan.ArtGen.Raster;

namespace OfficeMan.ArtGen.Icons;

/// <summary>
/// Proposals for the game's logo (Steam, the website), 2026-10-27, with the byline "A game by Joakim Frödin".
/// Each is drawn on a transparent 1200×560 canvas; <see cref="Background"/> is the colour it's meant to be seen on.
///   stamp   the title in a rectangular rubber stamp, slightly crooked, ink thin in places
///   label   the title punched into embossing label tape, the office kind
///   plain   the title in clean letters beside the round stamp mark from the icon
/// </summary>
public static class Logos
{
    public static readonly string[] Names = { "stamp", "label", "plain" };
    public const int W = 1200, H = 560;
    private const string Title = "OFFICE MAN", Over = "THE MISADVENTURES OF", By = "A GAME BY JOAKIM FRÖDIN";
    private const string StampBlue = "2f56b0", Paper = "f2e6cc";

    public static string Background(string name) => name switch { "stamp" => "efe6d2", _ => "1c1e22" };

    public static Canvas Draw(string name) => name switch
    {
        "stamp" => Stamp(),
        "label" => Label(),
        "plain" => Plain(),
        _ => throw new ArgumentException(name),
    };

    // ── The designs ───────────────────────────────────

    private static Canvas Stamp()
    {
        var ink = new SoftLayer(W, H, 1, 3);
        double wBig = Lettering.Width(Title, 130, 24, 6), wSmall = Lettering.Width(Over, 34, 6.5, 9);
        double frameW = Math.Max(wBig, wSmall) + 110, top = 96, bottom = top + 290;
        double x0 = (W - frameW) / 2, x1 = x0 + frameW;
        var turn = Turn(-4, new Pt(W / 2.0, (top + bottom) / 2));

        ink.Stroke(turn(Rect(x0, top, x1, bottom)), 12, StampBlue);
        ink.Stroke(turn(Rect(x0 + 16, top + 16, x1 - 16, bottom - 16)), 4, StampBlue);
        Text(ink, Over, W / 2.0, top + 44, 34, 6.5, 9, StampBlue, turn);
        ink.Stroke(turn(P(W / 2.0 - wSmall / 2, top + 98, W / 2.0 + wSmall / 2, top + 98)), 4, StampBlue);
        Text(ink, Title, W / 2.0, top + 124, 130, 24, 6, StampBlue, turn);
        var c = ink.ToCanvas(InkTexture);

        var clean = new SoftLayer(W, H, 1, 3);
        Text(clean, By, W / 2.0, bottom + 58, 24, 4, 10, "3a3434");
        c.Draw(clean.ToCanvas());
        return c;
    }

    private static Canvas Label()
    {
        var c = new Canvas(W, H);
        // The small tape, black, a little crooked; the big one red, crooked the other way.
        Tape(c, Over, 34, 7, 8, "1d1d20", centre: new Pt(500, 150), degrees: -2.5, seed: 1);
        Tape(c, Title, 112, 20, 12, "b8262d", centre: new Pt(610, 300), degrees: 1.5, seed: 2);
        var clean = new SoftLayer(W, H, 1, 3);
        Text(clean, By, W / 2.0, 440, 24, 4, 10, "cfc6b4");
        c.Draw(clean.ToCanvas());
        return c;
    }

    private static void Tape(Canvas c, string text, double cap, double weight, double tracking, string colour, Pt centre, double degrees, int seed)
    {
        double w = Lettering.Width(text, cap, weight, tracking) + cap * 0.9, h = cap * 1.75;
        var turn = Turn(degrees, centre);
        double x0 = centre.X - w / 2, y0 = centre.Y - h / 2;

        var shadow = new SoftLayer(W, H, 1, 3);
        shadow.Fill(turn(Rect(x0 + 4, y0 + 8, x0 + w + 4, y0 + h + 8)), "000000", (_, _) => 0.35);
        c.Draw(shadow.ToCanvas());

        var tape = new SoftLayer(W, H, 1, 3);
        tape.Fill(turn(Rect(x0, y0, x0 + w, y0 + h)), colour);
        tape.Fill(turn(Rect(x0, y0 + h * 0.08, x0 + w, y0 + h * 0.13)), "ffffff", (_, _) => 0.08);
        tape.Fill(turn(Rect(x0, y0 + h * 0.86, x0 + w, y0 + h)), "000000", (_, _) => 0.18);
        c.Draw(tape.ToCanvas());

        // Raised letters: each pressed a little higher or lower, the plastic paler where it's stretched.
        var rng = new Random(seed);
        var letters = new SoftLayer(W, H, 1, 3);
        var placed = Lettering.Layout(text, centre.X - Lettering.Width(text, cap, weight, tracking) / 2, centre.Y - cap / 2, cap, weight, tracking);
        foreach (var letter in placed)
        {
            double dy = (rng.NextDouble() - 0.5) * cap * 0.06;
            Func<List<Pt>, List<Pt>> move = pts => turn(pts.Select(q => new Pt(q.X, q.Y + dy)).ToList());
            foreach (var line in letter.Lines) letters.Stroke(move(line), weight, "f4f1ea");
            foreach (var (pt, r) in letter.Dots) letters.Fill(move(Circle(pt, r)), "f4f1ea");
        }
        c.Draw(letters.ToCanvas((x, y) => 0.82 + 0.18 * Noise.Fbm(x * 0.08, y * 0.5, seed + 9)));
    }

    private static Canvas Plain()
    {
        // The mark and the words side by side, centred as one block.
        const int mark = 210;
        double wTitle = Lettering.Width(Title, 100, 19, 6), gap = 50, x0 = (W - (mark + gap + wTitle)) / 2, x = x0 + mark + gap;
        var c = new Canvas(W, H);
        c.Draw(AppIcons.Draw("stamp_paper", mark), (int)x0, 175);
        var l = new SoftLayer(W, H, 1, 3);
        TextLeft(l, Over, x, 186, 30, 5.5, 10.5, "cfc6b4");
        TextLeft(l, Title, x, 240, 100, 19, 6, Paper);
        TextLeft(l, By, x, 376, 20, 3.4, 10, "8a8478");
        c.Draw(l.ToCanvas());
        return c;
    }

    // ── Helpers ───────────────────────────────────────

    private static double InkTexture(double x, double y) =>
        0.95 + 0.75 * Noise.Fbm(x * 0.012, y * 0.012, 11) - (Noise.Value(x * 0.11, y * 0.11, 4) > 0.84 ? 0.9 : 0);

    private static void Text(SoftLayer l, string text, double cx, double y, double cap, double weight, double tracking, string hex,
        Func<List<Pt>, List<Pt>>? transform = null) =>
        TextLeft(l, text, cx - Lettering.Width(text, cap, weight, tracking) / 2, y, cap, weight, tracking, hex, transform);

    private static void TextLeft(SoftLayer l, string text, double x, double y, double cap, double weight, double tracking, string hex,
        Func<List<Pt>, List<Pt>>? transform = null)
    {
        transform ??= p => p;
        foreach (var letter in Lettering.Layout(text, x, y, cap, weight, tracking))
        {
            foreach (var line in letter.Lines) l.Stroke(transform(line), weight, hex);
            foreach (var (pt, r) in letter.Dots) l.Fill(transform(Circle(pt, r)), hex);
        }
    }

    private static List<Pt> Circle(Pt c, double r) => Shapes.Ellipse(c.X, c.Y, r, r, 24);

    private static List<Pt> P(params double[] xy) =>
        Enumerable.Range(0, xy.Length / 2).Select(i => new Pt(xy[i * 2], xy[i * 2 + 1])).ToList();

    private static List<Pt> Rect(double x0, double y0, double x1, double y1) => P(x0, y0, x1, y0, x1, y1, x0, y1, x0, y0);

    private static Func<List<Pt>, List<Pt>> Turn(double degrees, Pt about)
    {
        double a = degrees * Math.PI / 180, c = Math.Cos(a), s = Math.Sin(a);
        return pts => pts.Select(p => new Pt(about.X + (p.X - about.X) * c - (p.Y - about.Y) * s,
                                             about.Y + (p.X - about.X) * s + (p.Y - about.Y) * c)).ToList();
    }
}
