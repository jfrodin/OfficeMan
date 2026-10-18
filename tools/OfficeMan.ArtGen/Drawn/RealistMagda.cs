using OfficeMan.ArtGen.Raster;
using static OfficeMan.ArtGen.Drawn.PathTools;

namespace OfficeMan.ArtGen.Drawn;

/// <summary>
/// Approach 2: illustrated realism. Natural proportions and asymmetry, three-quarter view turned
/// to the viewer's right, the face of a 52-year-old who drinks a bit and wears too much blue
/// eyeshadow. Painted with soft, blended shapes and no outlines. Expressions are understated.
/// </summary>
public static class RealistMagda
{
    public const int W = 600, H = 760;

    private static readonly Rgb Skin = Rgb.Hex("e8b597");
    private static readonly Rgb SkinShade = Rgb.Hex("c7826a");
    private static readonly Rgb SkinDeep = Rgb.Hex("a3604f");
    private static readonly Rgb SkinLight = Rgb.Hex("f7d3b8");
    private static readonly Rgb Flush = Rgb.Hex("d86e64");
    private static readonly Rgb Hair = Rgb.Hex("7d2a3b");
    private static readonly Rgb HairDark = Rgb.Hex("471324");
    private static readonly Rgb HairLight = Rgb.Hex("b24f5c");
    private static readonly Rgb Roots = Rgb.Hex("9f9496");
    private static readonly Rgb Dress = Rgb.Hex("b52733");
    private static readonly Rgb DressDark = Rgb.Hex("6e1220");
    private static readonly Rgb Lipstick = Rgb.Hex("b13844");
    private static readonly Rgb LipDark = Rgb.Hex("6a1d28");
    private static readonly Rgb EyeShadow = Rgb.Hex("7590c6");
    private static readonly Rgb Brow = Rgb.Hex("5e3530");
    private static readonly Rgb Lash = Rgb.Hex("2a1a1c");
    private static readonly Rgb Gold = Rgb.Hex("cfa349");

    private sealed record Face(double Smile, double Open, double Thin, double EyeOpen, double LowerLift,
        double GazeX, double GazeY, double BrowInner, double BrowRaise, bool Wet);

    private static Face For(Mood m) => m switch
    {
        Mood.Happy => new(Smile: 1.4, Open: 0.65, Thin: 0, EyeOpen: 0.5, LowerLift: 1.5, GazeX: 0, GazeY: 0, BrowInner: 0.2, BrowRaise: 3, Wet: false),
        Mood.Irritated => new(Smile: -0.6, Open: 0, Thin: 1.3, EyeOpen: 0.58, LowerLift: 0.6, GazeX: -3, GazeY: 0, BrowInner: -1.7, BrowRaise: -3, Wet: false),
        _ => new(Smile: -1.1, Open: 0, Thin: 0.3, EyeOpen: 0.72, LowerLift: 0.1, GazeX: 2, GazeY: 4.5, BrowInner: 1.9, BrowRaise: 0, Wet: true),
    };

    public static Canvas Draw(Mood mood)
    {
        var f = For(mood);
        var s = new Sketch(W, H);
        s.Clear((x, y) =>
        {
            double n = Noise.Fbm(x * 0.006, y * 0.006, 21) * 0.5 + 0.5;
            var c = Rgb.Hex("d8c6b0").Mix(Rgb.Hex("9fb1ab"), Math.Clamp(y / (double)H * 0.9 + n * 0.25 - 0.1, 0, 1));
            return c.Mix(Rgb.Hex("efe0cc"), Math.Max(0, 0.5 - Math.Abs(x - 220) / 600.0));
        });
        s.SetView(1.35, new Pt(300, 330));

        // ── Hair behind ──
        List<Pt> hairBase = PathBuilder.From(300, 148).C(376, 146, 430, 194, 438, 266).C(446, 330, 430, 400, 410, 446)
            .C(400, 470, 386, 482, 372, 478).L(250, 500).C(220, 500, 190, 484, 182, 450)
            .C(168, 396, 164, 320, 176, 252).C(190, 186, 236, 150, 300, 148);
        var hair = Shapes.Scallop(hairBase, new Pt(300, 320), 6, 19, seed: 7);
        PaintHair(s, hair, seed: 1);

        // ── Neck, shoulders, dress ──
        List<Pt> body = PathBuilder.From(262, 470).C(262, 530, 256, 560, 238, 580).C(190, 600, 110, 612, 50, 656)
            .L(30, 760).L(570, 760).L(560, 650).C(500, 610, 420, 592, 380, 578).C(364, 560, 360, 520, 366, 462);
        s.Fill(body, Skin);
        s.SoftFill(PathBuilder.From(250, 480).C(290, 548, 350, 548, 380, 470).L(386, 570).C(330, 596, 280, 596, 244, 570), SkinShade, 0.85, 12, clip: body);
        s.SoftFill(Shapes.Ellipse(470, 640, 120, 50), SkinShade, 0.55, 22, clip: body);
        s.SoftFill(Shapes.Ellipse(150, 630, 80, 26), SkinLight, 0.5, 18, clip: body);
        s.Stroke(PathBuilder.From(182, 618).C(214, 606, 246, 608, 270, 620), 3, SkinShade, 0.35, taper: true); // collarbone

        List<Pt> dress = PathBuilder.From(30, 670).C(96, 630, 166, 622, 210, 636).C(230, 690, 262, 712, 302, 714)
            .C(340, 712, 372, 690, 392, 636).C(436, 624, 506, 632, 570, 672).L(570, 760).L(30, 760);
        s.Fill(dress, Dress);
        s.SoftFill(PathBuilder.From(392, 636).C(436, 624, 506, 632, 570, 672).L(570, 760).L(380, 760).C(392, 720, 380, 690, 360, 684), DressDark, 0.7, 18, clip: dress);
        s.SoftFill(PathBuilder.From(120, 660).C(150, 700, 160, 730, 156, 760).L(190, 760).C(190, 720, 176, 690, 150, 656), DressDark, 0.45, 8, clip: dress);
        s.SoftFill(Shapes.Ellipse(120, 668, 70, 14), Rgb.Hex("dc5a5a"), 0.5, 10, clip: dress);
        s.Stroke(PathBuilder.From(210, 636).C(230, 690, 262, 712, 302, 714).C(340, 712, 372, 690, 392, 636), 2.4, DressDark, 0.6);
        s.Stroke(PathBuilder.From(266, 556).C(282, 640, 338, 640, 356, 552), 1.4, Gold, 0.9);
        s.Fill(Shapes.Ellipse(311, 640, 4, 5), Gold);

        // ── Face ──
        List<Pt> face = PathBuilder.From(250, 232).C(214, 250, 198, 300, 202, 350).C(204, 400, 208, 444, 226, 476)
            .C(244, 504, 278, 520, 310, 520).C(334, 520, 352, 508, 364, 486).C(376, 462, 380, 430, 384, 400)
            .C(392, 370, 392, 340, 384, 316).C(380, 282, 372, 250, 350, 234).C(320, 220, 280, 220, 250, 232);
        // Soft double chin, painted as part of the face.
        var chin = PathBuilder.From(256, 500).C(282, 548, 342, 546, 362, 494).L(330, 490).L(270, 492).Points;
        s.Fill(chin, Skin);
        s.SoftFill(PathBuilder.From(262, 520).C(290, 548, 338, 546, 356, 512).L(352, 530).C(330, 552, 290, 552, 264, 532), SkinShade, 0.6, 6);
        s.Fill(face, Skin);
        PaintFace(s, face, f, mood);

        // ── The ear she shows, and its hoop ──
        var ear = PathBuilder.From(206, 360).C(190, 356, 186, 384, 194, 404).C(198, 418, 208, 424, 214, 414).Points;
        s.Fill(ear, Skin);
        s.SoftFill(Shapes.Ellipse(200, 388, 6, 14), SkinShade, 0.7, 3, clip: ear);

        // ── Fringe, swept towards her far side ──
        List<Pt> fringeBase = PathBuilder.From(204, 300).C(202, 240, 240, 196, 300, 190).C(360, 186, 410, 222, 420, 280)
            .C(400, 262, 380, 258, 360, 268).C(344, 250, 320, 246, 300, 258).C(282, 244, 256, 246, 240, 262)
            .C(228, 270, 214, 284, 204, 300);
        var fringe = Shapes.Scallop(fringeBase, new Pt(300, 230), 4, 16, seed: 9);
        s.SoftFill(Shift(fringe, 8, 10), SkinDeep, 0.45, 6, clip: face); // cast shadow on the forehead
        PaintHair(s, fringe, seed: 2);

        // Grey roots along the parting.
        for (int i = 0; i < 14; i++)
            s.Stroke(Pts(252 + i * 3.2, 178 - i * 1.4, 258 + i * 3.2, 192 - i * 1.0), 2.2, Roots, 0.55, taper: true);

        // Gold hoop, hanging from the visible ear.
        s.Stroke(Close(Shapes.Ellipse(204, 440, 13, 17)), 3.2, Gold);
        s.Stroke(Shapes.Ellipse(204, 440, 13, 17).Skip(28).Take(8).ToList(), 1.2, Rgb.Hex("fff0c8"));

        return s.ToCanvas(grain: 0.02);
    }

    private static void PaintHair(Sketch s, List<Pt> shape, int seed)
    {
        s.Fill(shape, Hair);
        // Big masses first: shadow away from the light (upper left), light on the crown.
        s.SoftFill(Shift(shape, 26, 22), HairDark, 0.7, 18, clip: shape);
        s.SoftFill(Shapes.Ellipse(250, 190, 90, 50), HairLight, 0.5, 20, clip: shape);
        // Then hundreds of small curl strokes, light where the light hits, dark in the hollows.
        var rng = new Random(seed * 101);
        double minX = shape.Min(p => p.X), maxX = shape.Max(p => p.X), minY = shape.Min(p => p.Y), maxY = shape.Max(p => p.Y);
        int count = (int)((maxX - minX) * (maxY - minY) / 70);
        for (int i = 0; i < count; i++)
        {
            double x = minX + rng.NextDouble() * (maxX - minX), y = minY + rng.NextDouble() * (maxY - minY);
            if (!Shapes.Inside(shape, x, y)) continue;
            double lit = 1 - Math.Clamp(((x - 220) * 0.6 + (y - 180)) / 360.0, 0, 1);
            bool light = rng.NextDouble() < lit * 0.75;
            double r = 3.5 + rng.NextDouble() * 3.5, a0 = rng.NextDouble() * Math.PI * 2;
            var arc = Enumerable.Range(0, 9).Select(k =>
            {
                double a = a0 + k / 8.0 * Math.PI * 1.2;
                return new Pt(x + Math.Cos(a) * r, y + Math.Sin(a) * r);
            }).ToList();
            s.Stroke(arc, 2.0 + rng.NextDouble(), light ? HairLight : HairDark, 0.35 + rng.NextDouble() * 0.3, taper: true, clip: shape);
        }
    }

    private static void PaintFace(Sketch s, List<Pt> face, Face f, Mood mood)
    {
        // Planes: far side turns away; the near cheekbone and forehead catch the evening light.
        s.SoftFill(PathBuilder.From(384, 300).C(396, 360, 386, 440, 360, 492).L(320, 520).C(350, 470, 362, 400, 354, 320), SkinShade, 1.0, 12, clip: face);
        s.SoftFill(PathBuilder.From(388, 320).C(394, 380, 384, 440, 362, 486).L(352, 480).C(370, 430, 376, 380, 372, 326), SkinDeep, 0.6, 6, clip: face);
        s.SoftFill(Shapes.Ellipse(246, 384, 26, 18), SkinLight, 0.7, 12, clip: face);
        s.SoftFill(Shapes.Ellipse(268, 262, 44, 18), SkinLight, 0.5, 14, clip: face);
        // Jowl and the soft weight under the jaw.
        s.SoftFill(PathBuilder.From(214, 440).C(228, 480, 260, 506, 300, 512).L(296, 500).C(262, 490, 238, 468, 226, 436), SkinShade, 0.55, 10, clip: face);
        // Eye sockets.
        s.SoftFill(Shapes.Ellipse(262, 350, 30, 16), SkinShade, 0.45, 9, clip: face);
        s.SoftFill(Shapes.Ellipse(340, 348, 24, 15), SkinShade, 0.55, 9, clip: face);
        // A drinker's warmth: cheeks and the end of the nose.
        double cheekLift = f.Smile > 0 ? -12 : 0;
        s.SoftFill(Shapes.Ellipse(250, 418 + cheekLift, 26, 16), Flush, mood == Mood.Sad ? 0.45 : 0.3, 14, clip: face);
        s.SoftFill(Shapes.Ellipse(348, 414 + cheekLift, 16, 14), Flush, 0.3, 12, clip: face);
        s.SoftFill(Shapes.Ellipse(316, 418, 10, 8), Flush, mood == Mood.Sad ? 0.5 : 0.35, 6, clip: face);

        // ── Nose ──
        s.SoftFill(PathBuilder.From(304, 352).C(310, 380, 316, 400, 324, 416).L(312, 420).C(306, 400, 300, 380, 298, 354), SkinShade, 0.75, 4, clip: face);
        s.SoftFill(Shapes.Ellipse(314, 412, 7, 6), SkinLight, 0.7, 4, clip: face);
        s.SoftFill(Shapes.Ellipse(314, 440, 16, 5), SkinShade, 0.7, 4, clip: face);
        s.Stroke(PathBuilder.From(290, 420).C(282, 428, 288, 438, 300, 436), 2.4, SkinDeep, 0.8, taper: true);
        s.Fill(PathBuilder.From(298, 432).C(304, 428, 312, 430, 314, 434).C(308, 436, 302, 436, 298, 432).Points, Rgb.Hex("5a2a26"), 0.85);
        s.Stroke(PathBuilder.From(326, 424).C(332, 430, 330, 436, 322, 436), 1.6, SkinDeep, 0.6, taper: true);

        // ── Folds of a lived-in face ──
        double fold = 0.35 + Math.Max(0, f.Smile) * 0.35;
        s.Stroke(PathBuilder.From(286, 432).C(272, 448, 268, 462, 272 - f.Smile * 2, 482), 2.6, SkinDeep, fold, taper: true);
        s.SoftFill(PathBuilder.From(286, 432).C(272, 448, 268, 462, 272, 482).L(262, 480).C(258, 460, 266, 444, 282, 430), SkinShade, 0.4, 5, clip: face);
        s.Stroke(PathBuilder.From(330, 434).C(340, 446, 342, 458, 340, 474), 2.2, SkinDeep, fold * 0.8, taper: true);
        s.Stroke(PathBuilder.From(278, 482).C(276, 492, 278, 500, 282, 508), 1.6, SkinDeep, 0.3, taper: true); // marionette

        // ── Eyes ──
        Eye(s, face, f, cx: 262, cy: 354, w: 23, h: 12, innerRight: true, near: true);
        Eye(s, face, f, cx: 344, cy: 352, w: 16, h: 10, innerRight: false, near: false);
        // Crow's feet at the near eye; deeper when she smiles.
        double crow = 0.3 + Math.Max(0, f.Smile) * 0.4;
        foreach (var (y0, y1) in new[] { (350.0, 344.0), (356.0, 358.0), (362.0, 370.0) })
            s.Stroke(Pts(236, y0, 226, y1), 1.4, SkinDeep, crow, taper: true);

        // ── Brows: thin, pencilled, never quite matching ──
        double inner = f.BrowInner * 9, raise = f.BrowRaise;
        s.Stroke(PathBuilder.From(232, 332 - raise).C(240, 320 - raise, 258, 316 - raise - inner * 0.3, 284 + (f.BrowInner < 0 ? 4 : 0), 324 - inner), 3, Brow, 0.85, taper: true);
        s.Stroke(PathBuilder.From(316 - (f.BrowInner < 0 ? 4 : 0), 322 - inner).C(330, 314 - raise - inner * 0.3, 348, 316 - raise, 362, 326 - raise), 2.6, Brow, 0.85, taper: true);
        if (mood == Mood.Irritated)
            foreach (double x in new[] { 298.0, 305.0 })
                s.Stroke(Pts(x, 326, x + 1, 340), 1.6, SkinDeep, 0.55, taper: true);
        if (mood == Mood.Sad)
            foreach (double y in new[] { 290.0, 300.0 })
                s.Stroke(PathBuilder.From(262, y + 4).C(280, y - 2, 310, y - 2, 330, y + 4), 1.4, SkinDeep, 0.3, taper: true);

        Mouth(s, face, f, mood);
    }

    private static void Eye(Sketch s, List<Pt> face, Face f, double cx, double cy, double w, double h, bool innerRight, bool near)
    {
        // Upper lid peak sits towards the inner corner.
        double peak = innerRight ? 0.2 : -0.2;
        double top = cy - h * f.EyeOpen * 1.6;
        var upper = PathBuilder.From(cx - w, cy + 1).C(cx - w * 0.5 + peak * w, top, cx + w * 0.3 + peak * w, top - 1, cx + w, cy - 1).Points;
        double lift = f.LowerLift * 3;
        var lower = PathBuilder.From(cx + w, cy - 1).C(cx + w * 0.4, cy + h * 0.6 - lift, cx - w * 0.4, cy + h * 0.65 - lift, cx - w, cy + 1).Points;
        var white = upper.Concat(lower.Skip(1)).ToList();

        // Eyeshadow: a bit too much, a bit too blue, slightly uneven.
        var lidArea = PathBuilder.From(cx - w - 2, cy).C(cx - w * 0.4, top - h * 1.2, cx + w * 0.4, top - h * 1.3, cx + w + 4, cy - 3).Points;
        s.SoftFill(lidArea, EyeShadow, near ? 0.6 : 0.5, 3, clip: face);

        s.Fill(white, Rgb.Hex("ebdcd2"));
        double ir = h * 0.75;
        var iris = new Pt(cx + f.GazeX + (innerRight ? 1 : -1), cy + f.GazeY);
        s.Fill(Shapes.Ellipse(iris.X, iris.Y, ir, ir), Rgb.Hex("5b3d28"), clip: white);
        s.Fill(Shapes.Ellipse(iris.X, iris.Y, ir * 0.42, ir * 0.42), Rgb.Hex("1c1210"), clip: white);
        s.SoftFill(Shapes.Ellipse(cx, top - 2, w, h * 0.6), SkinDeep, 0.6, 3, clip: white); // lid shadow on the eyeball
        s.Fill(Shapes.Ellipse(iris.X - ir * 0.3, iris.Y - ir * 0.35, 1.8, 1.8), Rgb.Hex("fbf6f0"), clip: white);
        if (f.Wet)
        {
            s.Stroke(lower.Take(lower.Count - 4).ToList(), 1.6, Rgb.Hex("dfeef6"), 0.85);
            double sx = cx + (innerRight ? -6 : 6);
            var streak = PathBuilder.From(sx - 2, cy + h * 0.6).C(sx - 3, cy + 18, sx + 1, cy + 30, sx - 1, cy + 40)
                .L(sx + 2, cy + 40).C(sx + 4, cy + 30, sx, cy + 18, sx + 2, cy + h * 0.6).Points;
            s.SoftFill(streak, Rgb.Hex("4c4458"), 0.45, 1.5, clip: face); // mascara, carried down by a tear
        }

        // Hooded lid: a crease of skin folding over.
        s.Stroke(PathBuilder.From(cx - w + 2, top - h * 0.5).C(cx - w * 0.3, top - h * 1.1, cx + w * 0.3, top - h * 1.1, cx + w - 1, top - h * 0.3), 1.8, SkinDeep, 0.6, taper: true);
        s.Stroke(upper, near ? 2.8 : 2.4, Lash);
        var outer = innerRight ? new Pt(cx - w, cy + 1) : new Pt(cx + w, cy - 1);
        double dir = innerRight ? -1 : 1;
        s.Stroke(Pts(outer.X, outer.Y, outer.X + dir * 5, outer.Y - 3), 2, Lash, taper: true);
        s.Stroke(lower, 1.1, SkinDeep, 0.7, taper: true);
        // Tired bag beneath.
        s.Stroke(PathBuilder.From(cx - w * 0.6, cy + h + 2 - lift).C(cx - w * 0.2, cy + h + 6 - lift, cx + w * 0.2, cy + h + 6 - lift, cx + w * 0.6, cy + h + 1 - lift), 1.4, SkinDeep, 0.35, taper: true);
    }

    private static void Mouth(Sketch s, List<Pt> face, Face f, Mood mood)
    {
        double seamY = 466;
        var nc = new Pt(274, 468 - f.Smile * 6);
        var fc = new Pt(340, 465 - f.Smile * 5);
        double gap = f.Open * 10;
        double upperT = 7 - f.Thin * 3, lowerT = 11 - f.Thin * 5;

        var topLine = PathBuilder.From(nc.X, nc.Y).C(290, seamY - upperT - 1, 300, seamY - upperT - 3, 306, seamY - upperT + 1)
            .C(312, seamY - upperT - 3, 326, seamY - upperT - 1, fc.X, fc.Y).Points;
        var seamTop = PathBuilder.From(fc.X, fc.Y).C(322, seamY - f.Smile * 2, 292, seamY - f.Smile * 2, nc.X, nc.Y).Points;
        var seamBottom = PathBuilder.From(nc.X, nc.Y).C(292, seamY + gap - f.Smile, 322, seamY + gap - f.Smile, fc.X, fc.Y).Points;
        var bottomLine = PathBuilder.From(fc.X, fc.Y).C(326, seamY + gap + lowerT, 292, seamY + gap + lowerT, nc.X, nc.Y).Points;

        var upperLip = topLine.Concat(seamTop.Skip(1)).ToList();
        var lowerLip = seamBottom.Concat(bottomLine.Skip(1)).ToList();
        var opening = seamTop.Concat(seamBottom.Skip(1)).ToList();

        // Lipstick feathers a little past the edge.
        s.SoftFill(upperLip.Concat(lowerLip).ToList(), Lipstick, 0.25, 2);
        if (gap > 0)
        {
            s.Fill(opening, Rgb.Hex("3e1418"));
            s.Fill(PathBuilder.From(nc.X + 4, nc.Y).C(292, seamY - 1, 322, seamY - 1, fc.X - 4, fc.Y).L(fc.X - 6, fc.Y + 4)
                .C(322, seamY + 5, 292, seamY + 5, nc.X + 6, nc.Y + 4).Points, Rgb.Hex("ece0cf"), clip: opening);
        }
        s.Fill(lowerLip, Lipstick);
        s.Fill(upperLip, Rgb.Hex("9c2f3a"));
        s.SoftFill(Shapes.Ellipse(312, seamY + gap + lowerT * 0.45, 9, 2.6), Rgb.Hex("e88a88"), 0.75, 2);
        s.Stroke(seamTop, 1.6, LipDark, 0.85);
        // Shadow under the lower lip, and the chin that puckers when she's trying not to cry.
        s.SoftFill(Shapes.Ellipse(310, seamY + gap + lowerT + 7, 16, 4), SkinShade, 0.6, 4, clip: face);
        if (mood == Mood.Sad) s.SoftFill(Shapes.Ellipse(318, 500, 10, 6), SkinShade, 0.5, 4, clip: face);
        if (mood == Mood.Irritated)
        {
            s.Stroke(Pts(nc.X - 2, nc.Y - 2, nc.X - 5, nc.Y + 3), 1.4, SkinDeep, 0.6, taper: true);
            s.Stroke(Pts(fc.X + 2, fc.Y - 1, fc.X + 4, fc.Y + 4), 1.2, SkinDeep, 0.5, taper: true);
        }
    }
}
