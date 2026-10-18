using OfficeMan.ArtGen.Raster;
using static OfficeMan.ArtGen.Drawn.PathTools;

namespace OfficeMan.ArtGen.Drawn;

/// <summary>
/// Approach 1: animation-inspired Magda. Exaggerated shape language: a soft pear of a face,
/// a cloud of curls built from a few big lobes, large heavy-lidded eyes, drawn-on brows and
/// over-lined lipstick. Three-quarter view turned to the viewer's left, lit from the upper right.
/// Expressions are pushed, the way animation pushes them.
/// </summary>
public static class CartoonMagda
{
    public const int W = 600, H = 760;

    private static readonly Rgb Line = Rgb.Hex("3a1d24");
    private static readonly Rgb Skin = Rgb.Hex("f6c4a4");
    private static readonly Rgb SkinShade = Rgb.Hex("e2987f");
    private static readonly Rgb Blush = Rgb.Hex("f2827f");
    private static readonly Rgb Hair = Rgb.Hex("a32f4a");
    private static readonly Rgb HairShade = Rgb.Hex("6c1932");
    private static readonly Rgb HairLight = Rgb.Hex("d4606f");
    private static readonly Rgb Roots = Rgb.Hex("b7adae");
    private static readonly Rgb Dress = Rgb.Hex("d9313e");
    private static readonly Rgb DressShade = Rgb.Hex("a01e2f");
    private static readonly Rgb Lipstick = Rgb.Hex("d6364a");
    private static readonly Rgb LipShade = Rgb.Hex("a2223a");
    private static readonly Rgb MouthDark = Rgb.Hex("5a1622");
    private static readonly Rgb EyeShadow = Rgb.Hex("6d93e0");
    private static readonly Rgb Iris = Rgb.Hex("5b3a26");
    private static readonly Rgb White = Rgb.Hex("fbf4ec");
    private static readonly Rgb Gold = Rgb.Hex("e2b24a");
    private static readonly Rgb TearBlue = Rgb.Hex("bfe0f4");

    public static Canvas Draw(Mood mood)
    {
        var s = new Sketch(W, H);
        s.Clear((x, y) =>
        {
            double d = Math.Sqrt(Math.Pow((x - 300) / 300.0, 2) + Math.Pow((y - 320) / 320.0, 2));
            return Rgb.Hex("f9ecdb").Mix(Rgb.Hex("ecd2b8"), Math.Clamp(d - 0.2, 0, 1));
        });

        // ── Hair behind: a cloud of big lobes, fuller on top and on the near side ──
        var hairCentre = new Pt(306, 330);
        var hair = Lobes(Pts(196, 486, 168, 424, 160, 352, 170, 284, 198, 226, 244, 180, 304, 160, 366, 168,
                             418, 202, 452, 258, 464, 328, 456, 398, 440, 462, 424, 506, 394, 530, 356, 534,
                             246, 534, 214, 518), hairCentre, 0.32);
        s.Fill(hair, Hair);
        // Shadow on the far side (away from the light), hugging the silhouette.
        s.Fill(hair, HairShade, alphaAt: (x, y) => Shapes.Inside(hair, x - 26, y + 6) ? 0 : 1);
        // Lit crescents on the near-side lobes.
        foreach (var (cx, cy, r) in new[] { (370.0, 196.0, 30.0), (420.0, 240.0, 26.0), (446.0, 312.0, 24.0), (312.0, 178.0, 26.0) })
            s.Fill(Shapes.Ellipse(cx, cy, r, r * 0.7), HairLight, clip: hair,
                alphaAt: (x, y) => Shapes.Inside(Shapes.Ellipse(cx - 7, cy + 6, r, r * 0.7), x, y) ? 0 : 1);
        s.Stroke(hair, 4.2, Line);

        // ── Neck, shoulders, dress ──
        List<Pt> body = PathBuilder.From(250, 500).C(248, 536, 240, 560, 220, 576).C(170, 596, 100, 610, 50, 650)
            .L(30, 760).L(570, 760).L(550, 650).C(500, 610, 430, 594, 382, 576).C(364, 560, 358, 536, 360, 494);
        s.Fill(body, Skin);
        List<Pt> neckShade = PathBuilder.From(244, 500).C(280, 562, 340, 562, 366, 500).L(370, 562).C(320, 588, 280, 588, 240, 562);
        s.Fill(neckShade, SkinShade, clip: body);
        s.Stroke(PathBuilder.From(250, 500).C(248, 536, 240, 560, 220, 576).C(170, 596, 100, 610, 50, 650), 3.4, Line, taper: true);
        s.Stroke(PathBuilder.From(360, 494).C(358, 536, 364, 560, 382, 576).C(430, 594, 500, 610, 550, 650), 3.4, Line, taper: true);

        List<Pt> dress = PathBuilder.From(40, 664).C(100, 624, 170, 614, 214, 628).C(232, 682, 262, 708, 296, 710)
            .C(332, 708, 362, 682, 380, 628).C(424, 614, 492, 624, 560, 664).L(560, 760).L(40, 760);
        s.Fill(dress, Dress);
        List<Pt> dressShade = PathBuilder.From(40, 664).C(100, 624, 170, 614, 214, 628).C(222, 662, 232, 690, 246, 702)
            .C(204, 720, 172, 740, 164, 760).L(40, 760);
        s.Fill(dressShade, DressShade, clip: dress);
        s.Stroke(PathBuilder.From(214, 628).C(232, 682, 262, 708, 296, 710).C(332, 708, 362, 682, 380, 628), 3.6, Line);
        s.Stroke(PathBuilder.From(40, 664).C(100, 624, 170, 614, 214, 628), 3.4, Line, taper: true);
        s.Stroke(PathBuilder.From(380, 628).C(424, 614, 492, 624, 560, 664), 3.4, Line, taper: true);
        s.Stroke(PathBuilder.From(296, 716).C(298, 730, 296, 742, 292, 752), 2.4, LipShade, taper: true);
        // A gold chain with a little heart.
        s.Stroke(PathBuilder.From(264, 568).C(278, 640, 330, 640, 348, 566), 2.2, Gold);
        var heart = PathBuilder.From(305, 640).C(296, 628, 284, 640, 305, 656).C(326, 640, 314, 628, 305, 640).Points;
        s.Fill(heart, Gold);
        s.Stroke(Close(heart), 1.6, Line);

        // ── Face: a soft pear, wider at the jowls ──
        List<Pt> face = PathBuilder.From(206, 300).C(196, 350, 196, 404, 210, 444).C(222, 484, 252, 514, 292, 520)
            .C(340, 524, 386, 502, 402, 458).C(416, 420, 416, 360, 404, 310).C(392, 250, 340, 222, 290, 222)
            .C(246, 222, 212, 252, 206, 300);
        s.Fill(face, Skin);
        List<Pt> farShade = PathBuilder.From(206, 300).C(196, 350, 196, 404, 210, 444).C(222, 484, 252, 514, 292, 520)
            .C(262, 500, 236, 470, 228, 432).C(220, 392, 222, 342, 232, 298);
        s.Fill(farShade, SkinShade, clip: face);

        // Cheeks: round blush, lifted when she smiles.
        double lift = mood == Mood.Happy ? -10 : 0;
        s.SoftFill(Shapes.Ellipse(356, 420 + lift, 30, 18), Blush, 0.55, 7, clip: face);
        s.SoftFill(Shapes.Ellipse(232, 426 + lift, 18, 13), Blush, 0.45, 6, clip: face);

        // Fringe in front throws a shadow onto the forehead.
        var fringe = Fringe();
        s.Fill(Shift(fringe, -8, 14), SkinShade, clip: face);
        s.Stroke(Close(face), 3.6, Line);
        // Soft double chin.
        s.Stroke(PathBuilder.From(256, 522).C(282, 548, 334, 548, 368, 512), 3.0, Line, taper: true);

        DrawEyes(s, mood);
        DrawBrows(s, mood);
        DrawNose(s);
        DrawMouth(s, mood);

        // ── Fringe and the grey roots she hasn't had done ──
        s.Fill(fringe, Hair);
        s.Fill(fringe, HairShade, alphaAt: (x, y) => Shapes.Inside(fringe, x - 16, y - 10) ? 0 : 1);
        s.Fill(Shapes.Ellipse(356, 214, 30, 11), HairLight, clip: fringe);
        s.Stroke(fringe.Skip(49).ToList(), 3.8, Line); // only the curly lower edge; the top melts into the hair
        foreach (var (x0, y0) in new[] { (296.0, 164.0), (306.0, 163.0), (316.0, 165.0), (288.0, 167.0) })
            s.Stroke(Pts(x0, y0, x0 + 3, y0 + 12), 3.2, Roots, taper: true);

        // A few spiral accents and lobe creases: just enough to say "perm".
        foreach (var (cx, cy, r) in new[] { (190.0, 330.0, 9.0), (186.0, 420.0, 8.0), (440.0, 380.0, 9.0), (240.0, 200.0, 8.0), (428.0, 452.0, 8.0) })
            s.Stroke(Spiral(cx, cy, r), 2.6, HairShade, taper: true);
        foreach (var (x0, y0, x1, y1) in new[] { (172.0, 284.0, 192.0, 296.0), (198.0, 226.0, 214.0, 244.0), (452.0, 258.0, 434.0, 270.0), (464.0, 328.0, 444.0, 334.0) })
            s.Stroke(Pts(x0, y0, x1, y1), 2.6, Line, taper: true);

        // ── Big gold hoops ──
        s.Stroke(Close(Shapes.Ellipse(414, 486, 22, 26)), 7, Line);
        s.Stroke(Close(Shapes.Ellipse(414, 486, 22, 26)), 4.4, Gold);
        s.Stroke(Shapes.Ellipse(414, 486, 22, 26).Skip(30).Take(10).ToList(), 1.6, Rgb.Hex("fff0c4"));
        s.Stroke(Shapes.Ellipse(190, 492, 15, 19).Skip(4).Take(22).ToList(), 6, Line);
        s.Stroke(Shapes.Ellipse(190, 492, 15, 19).Skip(4).Take(22).ToList(), 3.6, Gold);

        return s.ToCanvas(grain: 0.012);
    }

    private static List<Pt> Fringe()
    {
        var top = PathBuilder.From(204, 286).C(204, 226, 254, 194, 304, 192).C(360, 190, 408, 222, 414, 286).Points;
        var bottom = Lobes(Pts(414, 286, 396, 262, 368, 262, 344, 244, 316, 254, 290, 238, 262, 252, 234, 248, 204, 286), new Pt(300, 200), 0.36, close: false);
        return top.Concat(bottom.Skip(1)).ToList();
    }

    private static List<Pt> Spiral(double cx, double cy, double r)
    {
        var pts = new List<Pt>();
        for (int i = 0; i <= 30; i++)
        {
            double t = i / 30.0, a = t * Math.PI * 3.2;
            double rr = r * (1 - t * 0.75);
            pts.Add(new Pt(cx + Math.Cos(a) * rr, cy + Math.Sin(a) * rr));
        }
        return pts;
    }

    // ── Eyes ───────────────────────────────────────────────────

    private static void DrawEyes(Sketch s, Mood mood)
    {
        DrawEye(s, mood, cx: 334, cy: 354, w: 30, h: 21, near: true);
        DrawEye(s, mood, cx: 240, cy: 356, w: 22, h: 17, near: false);
    }

    private static void DrawEye(Sketch s, Mood mood, double cx, double cy, double w, double h, bool near)
    {
        // Eyeshadow: a generous sweep of blue from lid to brow, as she'd apply it.
        var shadow = PathBuilder.From(cx - w, cy + 2).C(cx - w * 0.6, cy - h * 2.0, cx + w * 0.6, cy - h * 2.2, cx + w + 6, cy - 6)
            .C(cx + w * 0.4, cy - h + 2, cx - w * 0.4, cy - h + 2, cx - w, cy + 2).Points;
        s.Fill(shadow, EyeShadow, 0.85);

        if (mood == Mood.Happy)
        {
            // Closed, smiling eyes: an upturned arc with lashes, cheeks pushed up beneath.
            var arc = PathBuilder.From(cx - w, cy + 6).C(cx - w * 0.4, cy - 12, cx + w * 0.4, cy - 13, cx + w, cy + 2).Points;
            s.Stroke(arc, 5.2, Line, taper: true);
            Lashes(s, cx + w - 2, cy + 1, near ? 1 : 0.8);
            s.Stroke(PathBuilder.From(cx - w * 0.6, cy + 20).C(cx - w * 0.2, cy + 15, cx + w * 0.2, cy + 15, cx + w * 0.6, cy + 20), 2.2, SkinShade, taper: true);
            return;
        }

        double lidTop = mood == Mood.Irritated ? cy - h * 0.25 : mood == Mood.Sad ? cy - h * 0.8 : cy - h;
        var upper = PathBuilder.From(cx - w, cy + 2).C(cx - w * 0.5, lidTop, cx + w * 0.4, lidTop - 2, cx + w, cy - 4).Points;
        var lower = PathBuilder.From(cx + w, cy - 4).C(cx + w * 0.4, cy + h * 0.6, cx - w * 0.5, cy + h * 0.65, cx - w, cy + 2).Points;
        var white = upper.Concat(lower.Skip(1)).ToList();
        s.Fill(white, White);

        double gx = mood == Mood.Irritated ? 9 : -2, gy = mood == Mood.Sad ? 4 : 1;
        double ir = h * 0.62;
        s.Fill(Shapes.Ellipse(cx + gx, cy + gy, ir, ir), Iris, clip: white);
        s.Fill(Shapes.Ellipse(cx + gx, cy + gy, ir * 0.45, ir * 0.45), Line, clip: white);
        s.Fill(Shapes.Ellipse(cx + gx - ir * 0.35, cy + gy - ir * 0.4, ir * 0.3, ir * 0.3), White, clip: white);
        if (mood == Mood.Sad)
        {
            // Wet eyes: a second highlight and a shining lower rim.
            s.Fill(Shapes.Ellipse(cx + gx + ir * 0.35, cy + gy + ir * 0.35, ir * 0.16, ir * 0.16), White, clip: white);
            s.Stroke(lower.Take(lower.Count - 3).ToList(), 2.2, TearBlue);
        }

        s.Stroke(upper, near ? 5.4 : 4.6, Line);
        s.Stroke(lower, 1.8, Line, 0.8, taper: true);
        Lashes(s, cx + w - 2, cy - 4, near ? 1 : 0.8);

        if (mood == Mood.Irritated)
            s.Stroke(PathBuilder.From(cx - w * 0.6, cy - h * 0.9).C(cx - w * 0.2, cy - h * 1.15, cx + w * 0.3, cy - h * 1.15, cx + w * 0.7, cy - h * 0.8), 2, SkinShade, taper: true);

        if (mood == Mood.Sad && near)
        {
            // A tear on the way down, dragging mascara with it.
            s.Stroke(PathBuilder.From(cx + w * 0.5, cy + h * 0.6).C(cx + w * 0.6, cy + 30, cx + w * 0.4, cy + 46, cx + w * 0.5, cy + 62), 3, Rgb.Hex("4a4868"), 0.7, taper: true);
            s.Fill(Shapes.Ellipse(cx + w * 0.5, cy + 64, 5, 7), TearBlue);
            s.Stroke(Close(Shapes.Ellipse(cx + w * 0.5, cy + 64, 5, 7)), 1.6, Line);
        }
    }

    private static void Lashes(Sketch s, double x, double y, double k)
    {
        for (int i = 0; i < 3; i++)
        {
            double a = -0.9 + i * 0.45;
            var tip = new Pt(x + 10 * k + Math.Cos(a) * 10 * k, y - 4 - i * 4 * k + Math.Sin(a) * 8 * k);
            s.Stroke(PathBuilder.From(x - 4 + i * 4, y - i * 2).C(x + 2 + i * 4, y - 4 - i * 3, tip.X - 4, tip.Y + 2, tip.X, tip.Y), 3, Line, taper: true);
        }
    }

    // ── Brows: thin, pencilled-on arches ───────────────────────

    private static void DrawBrows(Sketch s, Mood mood)
    {
        (List<Pt> far, List<Pt> near) = mood switch
        {
            Mood.Happy => (PathBuilder.From(214, 296).C(224, 280, 244, 274, 264, 282).Points,
                           PathBuilder.From(306, 292).C(326, 272, 352, 272, 374, 288).Points),
            Mood.Irritated => (PathBuilder.From(212, 294).C(222, 276, 244, 270, 264, 280).Points,   // the sceptical one, up
                               PathBuilder.From(306, 318).C(326, 308, 350, 298, 376, 298).Points),  // the cross one, down
            _ => (PathBuilder.From(214, 312).C(228, 302, 246, 292, 264, 284).Points,
                  PathBuilder.From(306, 288).C(322, 284, 350, 292, 374, 308).Points),
        };
        s.Stroke(far, 4, Rgb.Hex("5a2430"), taper: true);
        s.Stroke(near, 4.4, Rgb.Hex("5a2430"), taper: true);
    }

    private static void DrawNose(Sketch s)
    {
        s.SoftFill(Shapes.Ellipse(262, 414, 13, 9), Blush, 0.4, 4);
        s.Stroke(PathBuilder.From(272, 372).C(264, 394, 250, 408, 256, 418).C(262, 428, 278, 426, 284, 418), 3.2, Line, taper: true);
        s.Fill(Shapes.Ellipse(270, 419, 3.4, 2.2), Line);
    }

    // ── Mouth: big, lipsticked a little past the line ──────────

    private static void DrawMouth(Sketch s, Mood mood)
    {
        switch (mood)
        {
            case Mood.Happy:
            {
                var open = PathBuilder.From(248, 450).C(272, 454, 310, 450, 336, 442).C(334, 490, 302, 510, 282, 504)
                    .C(260, 498, 248, 476, 248, 450).Points;
                s.Fill(ScaleAbout(open, new Pt(292, 474), 1.14), Lipstick);
                s.Fill(open, MouthDark);
                var teeth = PathBuilder.From(252, 452).C(272, 458, 310, 454, 334, 446).L(330, 462).C(304, 468, 272, 470, 254, 464).Points;
                s.Fill(teeth, White, clip: open);
                s.Fill(Shapes.Ellipse(290, 496, 22, 12), Rgb.Hex("e0606a"), clip: open);
                s.Stroke(Close(open), 3, Line);
                s.Stroke(PathBuilder.From(244, 446).C(240, 440, 238, 434, 242, 428), 2.4, Line, taper: true);
                s.Stroke(PathBuilder.From(340, 438).C(346, 432, 348, 426, 346, 420), 2.4, Line, taper: true);
                break;
            }
            case Mood.Irritated:
            {
                // Pursed and pushed to one side.
                var lips = PathBuilder.From(278, 464).C(290, 450, 312, 450, 324, 458).C(320, 474, 294, 478, 278, 464).Points;
                s.Fill(ScaleAbout(lips, new Pt(300, 464), 1.12), Lipstick);
                s.Fill(PathBuilder.From(278, 464).C(290, 450, 312, 450, 324, 458).C(312, 462, 290, 464, 278, 464).Points, LipShade);
                s.Stroke(PathBuilder.From(280, 464).C(292, 462, 310, 461, 322, 458), 2.8, Line);
                s.Stroke(PathBuilder.From(328, 450).C(334, 456, 336, 462, 333, 470), 2.4, Line, taper: true);
                foreach (double dx in new[] { -4.0, 2.0, 8.0 })
                    s.Stroke(Pts(300 + dx, 452, 301 + dx, 456), 1.4, LipShade);
                break;
            }
            default:
            {
                // Closed, corners down, lower lip pushed out and trembling.
                var upper = PathBuilder.From(254, 472).C(270, 456, 308, 454, 328, 470).C(306, 466, 276, 466, 254, 472).Points;
                var lower = PathBuilder.From(260, 471).C(272, 494, 310, 494, 324, 470).C(306, 468, 278, 468, 260, 471).Points;
                s.Fill(ScaleAbout(lower, new Pt(292, 480), 1.1), Lipstick);
                s.Fill(ScaleAbout(upper, new Pt(292, 464), 1.1), LipShade);
                s.Fill(Shapes.Ellipse(298, 482, 9, 3), Rgb.Hex("f39a96"));
                var seam = PathBuilder.From(254, 474).C(266, 466, 280, 466, 290, 467).C(300, 465, 312, 466, 328, 474).Points;
                s.Stroke(seam, 2.8, Line, wobble: 0.8, seed: 3);
                s.Stroke(PathBuilder.From(278, 502).C(288, 507, 300, 507, 310, 501), 2, SkinShade, taper: true);
                break;
            }
        }
    }
}
