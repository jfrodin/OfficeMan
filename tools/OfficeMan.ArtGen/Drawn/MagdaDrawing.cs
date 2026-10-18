// REFERENCE ARTWORK SOURCE: produces art/reference/drawn_flat.png. Do not modify; the creator chose it as a visual reference (2026-10-18).
using OfficeMan.ArtGen.Portraits;

namespace OfficeMan.ArtGen.Drawn;

/// <summary>
/// Magda as an authored drawing: every contour, shadow shape and feature line placed by hand
/// on a 600×760 page. Renderers decide how to put it on paper (paint, ink, pencil).
/// Light comes from the upper left: a summer evening on the river terrace.
/// </summary>
public sealed class MagdaDrawing
{
    public const int W = 600, H = 760;

    // ── Big shapes ─────────────────────────────────────────────
    public List<Pt> HairBack { get; }
    public List<Pt> Body { get; }
    public List<Pt> Dress { get; }
    public List<Pt> Face { get; }
    public List<Pt> Fringe { get; }
    public List<Pt> LockLeft { get; }
    public List<Pt> LockRight { get; }

    // ── Shadow and light shapes ────────────────────────────────
    public List<Pt> FaceShadow { get; }
    public List<Pt> JowlShadow { get; }
    public List<Pt> NeckShadow { get; }
    public List<Pt> NoseShadow { get; }
    public List<Pt> DressShadow { get; }
    public List<Pt> HairShadow { get; }
    public List<Pt> CheekLight { get; }
    public List<List<Pt>> EyeSockets { get; }

    // ── Features ───────────────────────────────────────────────
    public List<Eye> Eyes { get; }
    public List<List<Pt>> Brows { get; }
    public List<Pt> NoseBridge { get; }
    public List<List<Pt>> NoseWings { get; }
    public List<List<Pt>> Nostrils { get; }
    public List<Pt> UpperLip { get; }
    public List<Pt> LowerLip { get; }
    public List<Pt> LipSeam { get; }
    public List<Pt> MouthGap { get; }
    public List<Pt> UnderLip { get; }
    public List<Pt> DoubleChin { get; }
    public List<List<Pt>> Tears { get; }
    public List<Pt> Necklace { get; }
    public Pt Pendant { get; }
    public List<Pt> Neckline { get; }
    public List<List<Pt>> DressFolds { get; }
    public List<Pt> Cleavage { get; }
    public List<Pt> Earrings { get; }
    public List<Pt> BlushCentres { get; }

    /// <summary>C-shaped curl strokes inside the visible hair, for texture.</summary>
    public List<List<Pt>> Curls { get; }

    /// <summary>
    /// True on the side of a shape turned away from the light (lower right): a point nudged
    /// further that way falls outside the shape. Gives curved shadows that hug the silhouette.
    /// </summary>
    public static Func<double, double, bool> AwayFromLight(IReadOnlyList<Pt> shape, double reach) =>
        (x, y) => !Shapes.Inside(shape, x + reach, y + reach * 0.7);

    public sealed record Eye(List<Pt> White, Pt Iris, double IrisR, List<Pt> Lid, List<Pt> Crease,
        List<Pt> Shadow, List<Pt> LowerLid, List<Pt> Bag, List<Pt> Wing);

    public MagdaDrawing(Expression e)
    {
        var centre = new Pt(300, 320);

        // Hair: a big permed mass, scalloped all round.
        List<Pt> hairBase = PathBuilder.From(300, 92)
            .C(405, 88, 472, 150, 480, 250)
            .C(488, 340, 476, 430, 470, 480)
            .C(466, 535, 430, 560, 400, 545)
            .L(200, 545)
            .C(170, 560, 134, 535, 130, 480)
            .C(124, 430, 112, 340, 120, 250)
            .C(128, 150, 195, 88, 300, 92);
        HairBack = Shapes.Scallop(hairBase, centre, 13, 34, seed: 1);

        Body = PathBuilder.From(244, 480)
            .C(244, 518, 240, 540, 224, 556)
            .C(170, 580, 90, 594, 30, 646)
            .L(30, 760).L(570, 760).L(570, 646)
            .C(510, 594, 430, 580, 376, 556)
            .C(360, 540, 356, 518, 356, 480);

        Dress = PathBuilder.From(16, 646)
            .C(80, 604, 150, 594, 192, 606)
            .C(216, 664, 256, 694, 300, 696)
            .C(344, 694, 384, 664, 408, 606)
            .C(450, 594, 520, 604, 584, 646)
            .L(584, 760).L(16, 760);
        Neckline = PathBuilder.From(192, 606).C(216, 664, 256, 694, 300, 696).C(344, 694, 384, 664, 408, 606);

        Face = PathBuilder.From(178, 330)
            .C(172, 402, 186, 462, 228, 496)
            .C(252, 518, 276, 524, 300, 524)
            .C(324, 524, 348, 518, 372, 496)
            .C(414, 462, 428, 402, 422, 330)
            .C(414, 250, 362, 212, 300, 212)
            .C(238, 212, 186, 250, 178, 330);

        // Fringe: a rounded top with a curly lower edge over the forehead.
        List<Pt> fringeTop = PathBuilder.From(182, 312).C(176, 236, 232, 190, 300, 188).C(370, 188, 426, 232, 420, 312);
        List<Pt> fringeBottom = PathBuilder.From(420, 312).C(372, 286, 336, 300, 300, 288).C(266, 300, 228, 286, 182, 312);
        Fringe = fringeTop.Concat(Shapes.Scallop(fringeBottom, centre with { Y = 200 }, 11, 26, seed: 2)).ToList();

        List<Pt> lockL = PathBuilder.From(186, 300).C(154, 352, 146, 424, 166, 486).C(174, 512, 192, 516, 202, 502).C(192, 452, 190, 382, 200, 322);
        List<Pt> lockR = PathBuilder.From(414, 300).C(446, 352, 454, 424, 434, 486).C(426, 512, 408, 516, 398, 502).C(408, 452, 410, 382, 400, 322);
        LockLeft = Shapes.Scallop(lockL, new Pt(220, 400), 7, 22, seed: 3);
        LockRight = Shapes.Scallop(lockR, new Pt(380, 400), 7, 22, seed: 4);

        // Shadows: the light is upper left, so the right side of everything turns away.
        FaceShadow = PathBuilder.From(422, 330)
            .C(428, 402, 414, 462, 372, 496)
            .C(348, 518, 324, 524, 300, 524)
            .C(336, 504, 370, 470, 386, 430)
            .C(398, 396, 400, 356, 400, 318);
        JowlShadow = PathBuilder.From(192, 438).C(204, 478, 232, 502, 262, 512).C(236, 490, 214, 464, 204, 432);
        NeckShadow = PathBuilder.From(242, 500).C(266, 556, 334, 556, 358, 500).L(362, 562).C(330, 580, 270, 580, 238, 562);
        NoseShadow = PathBuilder.From(310, 362).C(316, 386, 322, 406, 326, 420).C(318, 430, 310, 432, 304, 428).C(312, 410, 310, 386, 306, 364);
        DressShadow = PathBuilder.From(408, 606).C(450, 594, 520, 604, 584, 646).L(584, 760).L(390, 760).C(400, 720, 380, 690, 356, 684).C(380, 664, 398, 636, 408, 606);
        HairShadow = PathBuilder.From(420, 200).C(470, 250, 486, 360, 476, 470).C(466, 530, 430, 556, 400, 545).L(380, 540).C(420, 470, 430, 330, 380, 240);
        CheekLight = Shapes.Ellipse(236, 396, 24, 11);
        EyeSockets = new() { Shapes.Ellipse(256, 344, 38, 19), Shapes.Ellipse(344, 344, 38, 19) };

        // Eyes: heavy-lidded, blue shadow, mascara.
        double open = 0.45 + 0.55 * e.LidOpen;
        double gx = e.LookX * 5, gy = e.LookY * 4;
        Eyes = new() { MakeEye(256, 352, -1), MakeEye(344, 352, 1) };
        Eye MakeEye(double cx, double cy, int side)
        {
            const double w = 27;
            List<Pt> white = PathBuilder.From(cx - w, cy).C(cx - 13, cy - 17 * open, cx + 12, cy - 18 * open, cx + w, cy - 2)
                .C(cx + 12, cy + 11, cx - 12, cy + 11, cx - w, cy);
            List<Pt> lid = PathBuilder.From(cx - w - 2, cy + 0.5).C(cx - 13, cy - 17 * open, cx + 12, cy - 18 * open, cx + w + 2, cy - 1.5);
            List<Pt> crease = PathBuilder.From(cx - w + 4, cy - 14).C(cx - 10, cy - 27, cx + 10, cy - 28, cx + w - 2, cy - 13);
            var shadow = crease.Concat(Enumerable.Reverse(lid)).ToList();
            List<Pt> lower = PathBuilder.From(cx - w + 6, cy + 3).C(cx - 10, cy + 11, cx + 10, cy + 11, cx + w - 5, cy + 1);
            List<Pt> bag = PathBuilder.From(cx - 16, cy + 20).C(cx - 6, cy + 25, cx + 6, cy + 25, cx + 16, cy + 19);
            double ox = side < 0 ? cx - w - 2 : cx + w + 2;
            List<Pt> wing = PathBuilder.From(ox, cy - (side < 0 ? 0 : 1.5)).C(ox + side * 6, cy - 4, ox + side * 10, cy - 7, ox + side * 14, cy - 11);
            return new Eye(white, new Pt(cx + gx, cy + gy + 1), 10.5, lid, crease, shadow, lower, bag, wing);
        }

        // Brows: inner ends lift when she's sad.
        double rise = e.BrowInner * 12 + e.BrowRaise * 4;
        Brows = new()
        {
            PathBuilder.From(220, 320 - e.BrowRaise * 4).C(236, 306 - rise * 0.15, 262, 300 - rise * 0.5, 288, 303 - rise),
            PathBuilder.From(380, 320 - e.BrowRaise * 4).C(364, 306 - rise * 0.15, 338, 300 - rise * 0.5, 312, 303 - rise),
        };

        NoseBridge = PathBuilder.From(298, 362).C(299, 380, 300, 396, 300, 410);
        NoseWings = new()
        {
            PathBuilder.From(282, 418).C(274, 430, 282, 442, 294, 438),
            PathBuilder.From(318, 418).C(326, 430, 318, 442, 306, 438),
        };
        Nostrils = new() { Shapes.Ellipse(291, 435, 5, 2.4), Shapes.Ellipse(310, 435, 5, 2.4) };

        // Mouth: corners follow the smile; it can open.
        double my = 468, corner = -e.Smile * 9, openGap = e.MouthOpen * 12;
        double lc = my + corner, rc = my + corner - e.Lopsided * 5;
        UpperLip = PathBuilder.From(268, lc).C(282, my - 13, 292, my - 15, 300, my - 9).C(308, my - 15, 318, my - 13, 332, rc)
            .C(318, my - 1, 282, my - 1, 268, lc);
        LipSeam = PathBuilder.From(268, lc).C(284, my - 1 + openGap * 0.2, 316, my - 1 + openGap * 0.2, 332, rc);
        MouthGap = PathBuilder.From(272, lc).C(286, my, 314, my, 328, rc).C(314, my + openGap, 286, my + openGap, 272, lc);
        LowerLip = PathBuilder.From(268, lc + 0.5).C(284, my + openGap + 1, 316, my + openGap + 1, 332, rc + 0.5)
            .C(322, my + openGap + 19, 278, my + openGap + 19, 268, lc + 0.5);
        UnderLip = PathBuilder.From(284, my + openGap + 27).C(294, my + openGap + 31, 306, my + openGap + 31, 316, my + openGap + 27);
        DoubleChin = PathBuilder.From(250, 516).C(272, 552, 328, 552, 350, 516);

        Tears = new();
        if (e.Tears)
            foreach (var (startX, length, seed) in new[] { (250.0, 46, 1), (340.0, 38, 3) })
            {
                var t = new List<Pt>();
                for (int i = 0; i <= length; i += 3)
                    t.Add(new Pt(startX + Math.Sin(i * 0.11 + seed) * 2.2 + i * 0.03, 364 + i));
                Tears.Add(t);
            }

        Necklace = PathBuilder.From(262, 548).C(276, 612, 324, 612, 338, 548);
        Pendant = new Pt(300, 608);
        DressFolds = new()
        {
            PathBuilder.From(150, 650).C(160, 690, 170, 720, 168, 760),
            PathBuilder.From(232, 706).C(236, 730, 236, 748, 232, 760),
            PathBuilder.From(452, 640).C(440, 690, 436, 724, 440, 760),
        };
        Cleavage = PathBuilder.From(300, 700).C(301, 712, 300, 722, 298, 732);
        Earrings = new() { new Pt(176, 508), new Pt(424, 508) };
        BlushCentres = new() { new Pt(226, 434), new Pt(374, 434) };

        // Curls: C-shaped strokes on a jittered grid, only where hair is visible.
        Curls = new();
        var rng = new Random(52);
        for (double y = 100; y < 560; y += 24)
            for (double x = 110; x < 494; x += 24)
            {
                double px = x + rng.NextDouble() * 14 - 7, py = y + rng.NextDouble() * 14 - 7;
                bool inHair = Shapes.Inside(HairBack, px, py) || Shapes.Inside(Fringe, px, py)
                              || Shapes.Inside(LockLeft, px, py) || Shapes.Inside(LockRight, px, py);
                bool hidden = Shapes.Inside(Face, px, py) && !Shapes.Inside(Fringe, px, py)
                              && !Shapes.Inside(LockLeft, px, py) && !Shapes.Inside(LockRight, px, py);
                if (!inHair || hidden || Shapes.Inside(Body, px, py)) continue;
                double r = 6 + rng.NextDouble() * 4, a0 = rng.NextDouble() * Math.PI * 2;
                var curl = new List<Pt>();
                for (int k = 0; k <= 14; k++)
                {
                    double a = a0 + k / 14.0 * Math.PI * 1.35;
                    curl.Add(new Pt(px + Math.Cos(a) * r, py + Math.Sin(a) * r * 0.9));
                }
                Curls.Add(curl);
            }
    }
}
