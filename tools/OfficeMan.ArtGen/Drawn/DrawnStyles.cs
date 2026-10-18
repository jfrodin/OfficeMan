using OfficeMan.ArtGen.Raster;

namespace OfficeMan.ArtGen.Drawn;

/// <summary>Three ways of putting the same authored drawing on paper.</summary>
public static class DrawnStyles
{
    private const int W = MagdaDrawing.W, H = MagdaDrawing.H;

    private static List<Pt> Closed(List<Pt> pts) => pts.Append(pts[0]).ToList();

    private static List<Pt> Offset(List<Pt> pts, double dx, double dy) => pts.Select(p => new Pt(p.X + dx, p.Y + dy)).ToList();

    // ════════════════════════════════════════════════════════════
    // 1. Flat illustration: warm evening light, two tones per colour.
    // ════════════════════════════════════════════════════════════

    public static Canvas Flat(MagdaDrawing d)
    {
        var s = new Sketch(W, H);
        Rgb Hex(string h) => Rgb.Hex(h);

        var skin = Hex("f2c6a8"); var skinShade = Hex("d8957e"); var skinDeep = Hex("bd7766"); var skinLight = Hex("fbdcc6");
        var hair = Hex("8a2c42"); var hairShade = Hex("5c1a2f"); var hairLight = Hex("b6536a");
        var dress = Hex("d0303d"); var dressShade = Hex("97192c");
        var gold = Hex("d1a54a"); var line = Hex("8a4a40");

        // Evening sky over the river: warm at the top, cooling to teal.
        s.Clear((x, y) =>
        {
            double t = y / H;
            var top = Hex("f4d3ad"); var bottom = Hex("8db0aa");
            var c = top.Mix(bottom, Math.Clamp(t * 1.15 - 0.1, 0, 1));
            double glow = Math.Max(0, 1 - Math.Sqrt(Math.Pow((x - 230) / 330.0, 2) + Math.Pow((y - 260) / 330.0, 2)));
            return c.Mix(Hex("fbe7cc"), glow * 0.5);
        });

        // Hair behind.
        s.Fill(d.HairBack, hair);
        var hairBackShade = MagdaDrawing.AwayFromLight(d.HairBack, 22);
        s.Fill(d.HairBack, hairShade, alphaAt: (x, y) => hairBackShade(x, y) ? 1 : 0);

        // Neck, shoulders, dress.
        s.Fill(d.Body, skin);
        s.Fill(d.NeckShadow, skinShade, clip: d.Body);
        s.Fill(Shapes.Ellipse(470, 640, 120, 60), skinShade, 0.6, clip: d.Body);
        s.Fill(d.Dress, dress);
        s.Fill(d.DressShadow, dressShade, clip: d.Dress);
        foreach (var fold in d.DressFolds) s.Stroke(fold, 3, dressShade, 0.9, taper: true, clip: d.Dress);
        s.Stroke(d.Neckline, 2, Hex("8a1626"), 0.8);
        s.Stroke(d.Cleavage, 1.8, skinShade, taper: true);
        s.Stroke(d.Necklace, 1.6, gold);
        s.Fill(Shapes.Ellipse(d.Pendant.X, d.Pendant.Y, 5, 6), gold);
        s.Fill(Shapes.Ellipse(d.Pendant.X - 1.5, d.Pendant.Y - 2, 1.6, 1.6), Hex("fff2c8"));

        // Face.
        s.Fill(d.Face, skin);
        foreach (var sock in d.EyeSockets) s.Fill(sock, skinShade, 0.35, clip: d.Face);
        s.Fill(d.FaceShadow, skinShade, clip: d.Face);
        s.Fill(d.JowlShadow, skinShade, 0.7, clip: d.Face);
        s.Fill(d.NoseShadow, skinShade, clip: d.Face);
        s.Fill(d.CheekLight, skinLight, 0.9);
        foreach (var b in d.BlushCentres)
            s.Fill(Shapes.Ellipse(b.X, b.Y, 38, 22), Hex("ea8c86"), 0.45, clip: d.Face,
                alphaAt: (x, y) => Math.Max(0, 1 - Math.Sqrt(Math.Pow((x - b.X) / 38, 2) + Math.Pow((y - b.Y) / 22, 2))));
        // The fringe casts a soft shadow on the forehead.
        s.Fill(Offset(d.Fringe, 5, 10), skinShade, 0.75, clip: d.Face);
        s.Stroke(d.DoubleChin, 2.2, skinDeep, 0.8, taper: true);
        s.Stroke(d.UnderLip, 2, skinShade, taper: true);

        DrawEyesFlat(s, d);
        foreach (var brow in d.Brows) s.Stroke(brow, 4.6, Hex("5a2430"), taper: true);
        s.Stroke(d.NoseBridge, 1.4, skinShade, 0.6, taper: true);
        foreach (var wing in d.NoseWings) s.Stroke(wing, 2, skinDeep, taper: true);
        foreach (var n in d.Nostrils) s.Fill(n, Hex("8a4a44"));
        s.Fill(Shapes.Ellipse(296, 418, 4, 3), skinLight);

        s.Fill(d.MouthGap, Hex("4a1820"));
        s.Fill(d.LowerLip, Hex("d45a5a"));
        s.Fill(d.UpperLip, Hex("b03842"));
        s.Stroke(d.LipSeam, 1.8, Hex("6a1a24"));
        s.Fill(Shapes.Ellipse(307, 478, 9, 3), Hex("f29a8e"), 0.8);
        foreach (var t in d.Tears) s.Stroke(t, 2.4, Hex("44405e"), 0.75, taper: true);

        // Hair in front.
        foreach (var front in new[] { d.LockLeft, d.LockRight, d.Fringe })
        {
            s.Fill(front, hair);
            var shade = MagdaDrawing.AwayFromLight(front, 10);
            s.Fill(front, hairShade, alphaAt: (x, y) => shade(x, y) ? 1 : 0);
        }
        foreach (var curl in d.Curls)
        {
            s.Stroke(curl, 2.2, hairShade, 0.85, taper: true);
            if (curl[0].X < 330 && curl[0].Y < 420)
                s.Stroke(Offset(curl.Take(8).ToList(), -2, -2), 2, hairLight, 0.8, taper: true);
        }

        foreach (var ear in d.Earrings)
        {
            s.Stroke(Closed(Shapes.Ellipse(ear.X, ear.Y, 14, 17)), 3.2, gold);
            s.Stroke(Shapes.Ellipse(ear.X, ear.Y, 14, 17).Skip(28).Take(10).ToList(), 1.4, Hex("fff0c0"));
        }

        // A thin warm line to hold the face together.
        s.Stroke(Closed(d.Face), 1.4, line, 0.35);
        return s.ToCanvas(grain: 0.025);
    }

    private static void DrawEyesFlat(Sketch s, MagdaDrawing d)
    {
        foreach (var eye in d.Eyes)
        {
            s.Fill(eye.Shadow, Rgb.Hex("7f9ad4"), 0.85);
            s.Fill(eye.White, Rgb.Hex("f6ede3"));
            s.Fill(Shapes.Ellipse(eye.Iris.X, eye.Iris.Y, eye.IrisR, eye.IrisR), Rgb.Hex("6b4a2e"), clip: eye.White);
            s.Fill(Shapes.Ellipse(eye.Iris.X, eye.Iris.Y, 4.5, 4.5), Rgb.Hex("231612"), clip: eye.White);
            s.Fill(Shapes.Ellipse(eye.Iris.X - 3.5, eye.Iris.Y - 3.5, 2.2, 2.2), Rgb.Hex("ffffff"), clip: eye.White);
            s.Stroke(eye.Lid, 3.6, Rgb.Hex("2a161c"));
            s.Stroke(eye.Wing, 2.6, Rgb.Hex("2a161c"), taper: true);
            s.Stroke(eye.Crease, 1.4, Rgb.Hex("b7786a"), 0.8, taper: true);
            s.Stroke(eye.LowerLid, 1.2, Rgb.Hex("c08a78"), taper: true);
            s.Stroke(eye.Bag, 1.2, Rgb.Hex("d39a84"), 0.7, taper: true);
        }
    }

    // ════════════════════════════════════════════════════════════
    // 2. Ink: black line and crosshatching on paper.
    // ════════════════════════════════════════════════════════════

    public static Canvas Ink(MagdaDrawing d)
    {
        var s = new Sketch(W, H);
        var paper = Rgb.Hex("f6f1e4");
        var ink = Rgb.Hex("1b1714");
        s.Clear((_, _) => paper);

        void Occlude(List<Pt> shape) { s.MultiplyMode = false; s.Fill(shape, paper); s.MultiplyMode = true; }
        void Line(List<Pt> pts, double w, double a = 1, bool taper = true, int seed = 0) =>
            s.Stroke(pts, w, ink, a, taper, wobble: 0.5, seed: seed);
        s.MultiplyMode = true;

        // Hair behind: curls, and hatching that deepens on the shadow side.
        s.Hatch(d.HairBack, 50, 6, 1.0, ink, 0.8, wobble: 1, seed: 1);
        s.Hatch(d.HairBack, -40, 4.5, 1.0, ink, 0.9, wobble: 1, seed: 2, mask: MagdaDrawing.AwayFromLight(d.HairBack, 22));
        foreach (var curl in d.Curls) Line(curl, 1.8, 0.95);
        Line(Closed(d.HairBack), 2.4, taper: false, seed: 3);

        // Body and dress.
        Occlude(d.Body);
        s.Hatch(d.NeckShadow, 45, 4.5, 0.9, ink, 0.9, wobble: 0.6, seed: 4, clip: d.Body);
        Line(PathBuilder.From(244, 480).C(244, 518, 240, 540, 224, 556).C(170, 580, 90, 594, 30, 646), 2.2);
        Line(PathBuilder.From(356, 480).C(356, 518, 360, 540, 376, 556).C(430, 580, 510, 594, 570, 646), 2.2);
        Occlude(d.Dress);
        s.Hatch(d.Dress, 45, 6.5, 0.9, ink, 0.85, wobble: 0.8, seed: 5);
        s.Hatch(d.DressShadow, -45, 4.5, 0.9, ink, 0.9, wobble: 0.8, seed: 6, clip: d.Dress);
        Line(d.Neckline, 2.4);
        foreach (var fold in d.DressFolds) Line(fold, 1.8);
        Line(d.Cleavage, 1.4);
        Line(d.Necklace, 1.0, taper: false);
        s.Stroke(Closed(Shapes.Ellipse(d.Pendant.X, d.Pendant.Y, 5, 6)), 1.2, ink);

        // Face: occlude, hatch the turned-away planes, then line.
        Occlude(d.Face);
        s.Hatch(d.FaceShadow, 45, 4.2, 0.8, ink, 0.9, wobble: 0.6, seed: 7, clip: d.Face);
        s.Hatch(d.JowlShadow, 45, 5, 0.7, ink, 0.7, wobble: 0.6, seed: 8, clip: d.Face);
        s.Hatch(d.NoseShadow, 45, 3.6, 0.7, ink, 0.9, seed: 9, clip: d.Face);
        s.Hatch(Offset(d.Fringe, 5, 10), 45, 3.8, 0.8, ink, 0.9, seed: 10, clip: d.Face);
        Line(Closed(d.Face), 2.4, taper: false, seed: 11);
        Line(d.DoubleChin, 1.8);
        Line(d.UnderLip, 1.4);

        foreach (var eye in d.Eyes)
        {
            s.Hatch(eye.Shadow, 80, 2.6, 0.6, ink, 0.7, seed: 12);
            Occlude(eye.White);
            s.Fill(Shapes.Ellipse(eye.Iris.X, eye.Iris.Y, eye.IrisR, eye.IrisR), ink, 0.85, clip: eye.White);
            s.MultiplyMode = false;
            s.Fill(Shapes.Ellipse(eye.Iris.X - 3.5, eye.Iris.Y - 3.5, 2.2, 2.2), paper, clip: eye.White);
            s.MultiplyMode = true;
            Line(eye.Lid, 3.4, taper: false);
            Line(eye.Wing, 2.4);
            Line(eye.Crease, 1.2);
            Line(eye.LowerLid, 1.0);
            Line(eye.Bag, 0.9, 0.7);
        }
        foreach (var brow in d.Brows) Line(brow, 4.2);
        Line(d.NoseBridge, 1.0, 0.6);
        foreach (var wing in d.NoseWings) Line(wing, 1.8);
        foreach (var n in d.Nostrils) s.Fill(n, ink);

        s.Fill(d.MouthGap, ink);
        s.Hatch(d.UpperLip, 45, 2.4, 0.8, ink, 0.9, seed: 13);
        s.Hatch(d.LowerLip, 45, 4.0, 0.7, ink, 0.6, seed: 14);
        Line(Closed(d.UpperLip), 1.6, taper: false);
        Line(d.LowerLip.Skip(25).ToList(), 1.6);
        Line(d.LipSeam, 2.0, taper: false);
        foreach (var t in d.Tears) Line(t, 1.6, 0.9);

        // Hair in front.
        foreach (var front in new[] { d.LockLeft, d.LockRight, d.Fringe })
        {
            Occlude(front);
            s.Hatch(front, 50, 6, 1.0, ink, 0.8, wobble: 1, seed: 15);
            s.Hatch(front, -40, 4.5, 1.0, ink, 0.9, wobble: 1, seed: 16, mask: MagdaDrawing.AwayFromLight(front, 10));
            foreach (var curl in d.Curls.Where(c => Shapes.Inside(front, c[0].X, c[0].Y))) Line(curl, 1.8, 0.95);
            Line(Closed(front), 2.2, taper: false, seed: 17);
        }

        foreach (var ear in d.Earrings) s.Stroke(Closed(Shapes.Ellipse(ear.X, ear.Y, 14, 17)), 2.0, ink);
        return s.ToCanvas(grain: 0.015);
    }

    // ════════════════════════════════════════════════════════════
    // 3. Pencil: graphite on paper, doubled wobbly lines, soft hatching.
    // ════════════════════════════════════════════════════════════

    public static Canvas Pencil(MagdaDrawing d)
    {
        var s = new Sketch(W, H);
        var paperBase = Rgb.Hex("efe8da");
        var lead = Rgb.Hex("45454e");
        s.Clear((x, y) =>
        {
            double tooth = Noise.Value(x * 0.9, y * 0.9, 3) * 0.06 + Noise.Value(x * 0.08, y * 0.08, 4) * 0.05;
            return paperBase.Mix(Rgb.Hex("cfc6b4"), tooth);
        });
        int seed = 0;

        void Occlude(List<Pt> shape)
        {
            s.MultiplyMode = false;
            s.Fill(shape, paperBase);
            s.MultiplyMode = true;
        }
        // A pencil line is drawn twice: a searching line and a firmer one.
        void Line(List<Pt> pts, double w, double a = 0.75)
        {
            s.Stroke(pts, w * 0.8, lead, Math.Min(1, a * 0.6), taper: true, wobble: 2.2, seed: ++seed);
            s.Stroke(pts, w * 1.2, lead, Math.Min(1, a * 1.3), taper: true, wobble: 1.0, seed: ++seed);
        }
        void Smudge(List<Pt> shape, double a, List<Pt>? clip = null) =>
            s.Fill(shape, lead, a * 1.5, clip, (x, y) => 0.6 + Noise.Value(x * 0.05, y * 0.05, 9) * 0.4);
        void Shade(List<Pt> shape, double spacing, double a, List<Pt>? clip = null, Func<double, double, bool>? mask = null) =>
            s.Hatch(shape, 38, spacing, 1.0, lead, Math.Min(1, a * 1.4), wobble: 1.6, seed: ++seed, clip: clip, gaps: 0.3, mask: mask);

        s.MultiplyMode = true;

        // Hair: loose curls, smudged and hatched on the shadow side.
        Smudge(d.HairBack, 0.10);
        Shade(d.HairBack, 4, 0.55, mask: MagdaDrawing.AwayFromLight(d.HairBack, 22));
        foreach (var curl in d.Curls) s.Stroke(curl, 1.5, lead, 0.75, taper: true, wobble: 1.2, seed: ++seed);
        Line(Closed(d.HairBack), 1.4, 0.5);

        Occlude(d.Body);
        Smudge(d.NeckShadow, 0.14, d.Body);
        Shade(d.NeckShadow, 4, 0.4, d.Body);
        Line(PathBuilder.From(244, 480).C(244, 518, 240, 540, 224, 556).C(170, 580, 90, 594, 30, 646), 1.4);
        Line(PathBuilder.From(356, 480).C(356, 518, 360, 540, 376, 556).C(430, 580, 510, 594, 570, 646), 1.4);

        Occlude(d.Dress);
        Smudge(d.Dress, 0.10);
        Shade(d.DressShadow, 4.5, 0.45, d.Dress);
        Line(d.Neckline, 1.6);
        foreach (var fold in d.DressFolds) Line(fold, 1.2, 0.5);
        Line(d.Cleavage, 1.0, 0.6);
        s.Stroke(d.Necklace, 0.8, lead, 0.6, wobble: 0.5, seed: ++seed);

        Occlude(d.Face);
        Smudge(d.FaceShadow, 0.12, d.Face);
        Shade(d.FaceShadow, 4, 0.4, d.Face);
        Smudge(d.JowlShadow, 0.08, d.Face);
        Smudge(d.NoseShadow, 0.14, d.Face);
        foreach (var sock in d.EyeSockets) Smudge(sock, 0.06, d.Face);
        Smudge(Offset(d.Fringe, 5, 10), 0.14, d.Face);
        Line(Closed(d.Face), 1.5, 0.6);
        Line(d.DoubleChin, 1.2, 0.55);
        Line(d.UnderLip, 1.0, 0.5);

        foreach (var eye in d.Eyes)
        {
            Smudge(eye.Shadow, 0.18);
            Occlude(eye.White);
            s.Fill(Shapes.Ellipse(eye.Iris.X, eye.Iris.Y, eye.IrisR, eye.IrisR), lead, 0.55, clip: eye.White);
            s.Fill(Shapes.Ellipse(eye.Iris.X, eye.Iris.Y, 4.5, 4.5), lead, 0.9, clip: eye.White);
            s.MultiplyMode = false;
            s.Fill(Shapes.Ellipse(eye.Iris.X - 3.5, eye.Iris.Y - 3.5, 2.2, 2.2), paperBase, clip: eye.White);
            s.MultiplyMode = true;
            Line(eye.Lid, 2.8, 0.95);
            Line(eye.Wing, 2.0, 0.9);
            Line(eye.Crease, 1.0, 0.5);
            Line(eye.LowerLid, 0.9, 0.55);
            Line(eye.Bag, 0.8, 0.35);
        }
        foreach (var brow in d.Brows) Line(brow, 3.4, 0.85);
        Line(d.NoseBridge, 0.9, 0.35);
        foreach (var wing in d.NoseWings) Line(wing, 1.4, 0.75);
        foreach (var n in d.Nostrils) s.Fill(n, lead, 0.8);

        s.Fill(d.MouthGap, lead, 0.9);
        Smudge(d.UpperLip, 0.35);
        Smudge(d.LowerLip, 0.2);
        Line(d.LipSeam, 1.6, 0.9);
        Line(d.LowerLip.Skip(25).ToList(), 1.0, 0.45);
        foreach (var t in d.Tears) s.Stroke(t, 1.6, lead, 0.7, taper: true, wobble: 0.8, seed: ++seed);

        foreach (var front in new[] { d.LockLeft, d.LockRight, d.Fringe })
        {
            Occlude(front);
            Smudge(front, 0.10);
            Shade(front, 4, 0.55, mask: MagdaDrawing.AwayFromLight(front, 10));
            foreach (var curl in d.Curls.Where(c => Shapes.Inside(front, c[0].X, c[0].Y)))
                s.Stroke(curl, 1.5, lead, 0.8, taper: true, wobble: 1.2, seed: ++seed);
            Line(Closed(front), 1.3, 0.5);
        }
        foreach (var ear in d.Earrings) Line(Closed(Shapes.Ellipse(ear.X, ear.Y, 14, 17)), 1.2, 0.6);

        return s.ToCanvas(grain: 0.02);
    }
}
