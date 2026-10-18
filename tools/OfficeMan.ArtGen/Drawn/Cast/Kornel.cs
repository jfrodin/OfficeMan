using OfficeMan.ArtGen.Raster;
using static OfficeMan.ArtGen.Drawn.PathTools;

namespace OfficeMan.ArtGen.Drawn.Cast;

/// <summary>
/// Kornel Vidrascu, 41, the Awkward Colleague. Long head, high forehead, a weak chin that gives up
/// early, ears that stick out, a combover that fools nobody, big glasses, a mole. Long neck with an
/// Adam's apple, sloping shoulders, short-sleeved shirt and a tie. Three-quarter view to the left.
/// </summary>
public sealed class Kornel : CastMember
{
    public override string Name => "kornel";

    /// <summary>Polite and ready, as if someone might ask him something.</summary>
    public override Rig Neutral => new()
    {
        Name = "neutral", CornerL = 1.5, CornerR = 2, BrowInL = 2, BrowInR = 2.5, BrowOutL = 1.5, BrowOutR = 2, OpenL = 1.05, OpenR = 1.05,
    };

    /// <summary>Someone said they can't wait to hear his poem.</summary>
    public override Rig Distinct => new()
    {
        Name = "overjoyed", Open = 0.8, CornerL = 11, CornerR = 13, Stretch = 6, BrowInL = 9, BrowInR = 10, BrowOutL = 7, BrowOutR = 8,
        OpenL = 1.25, OpenR = 1.25, LiftL = 0.15, LiftR = 0.15, CheekL = 4, CheekR = 4,
    };

    private static readonly Rgb Skin = Rgb.Hex("f0c9b2"), Shade = Rgb.Hex("d29b86"), Deep = Rgb.Hex("a56a5c");

    protected override void Paint(Sketch s, Rig r)
    {
        Backdrop(s, "ece4c8", "b8ad8a");
        var kit = new FaceKit(s, Ink, Shade, Deep);

        // Long neck, Adam's apple, sloping shoulders.
        List<Pt> body = PathBuilder.From(264, 492).C(268, 560, 264, 600, 246, 622).C(204, 640, 146, 652, 96, 690)
            .L(84, 760).L(516, 760).L(504, 690).C(456, 652, 398, 640, 362, 622).C(346, 600, 342, 560, 350, 486);
        s.Fill(body, Skin);
        s.SoftFill(PathBuilder.From(262, 496).C(290, 540, 330, 540, 352, 492).L(354, 540).C(330, 556, 290, 556, 262, 540), Shade, 0.8, 5, body);
        s.Stroke(PathBuilder.From(302, 552).C(310, 556, 312, 566, 306, 572), 2, Deep, 0.7, taper: true);
        s.Stroke(PathBuilder.From(264, 492).C(268, 560, 264, 600, 246, 622), 2.4, Ink, taper: true);
        s.Stroke(PathBuilder.From(350, 486).C(342, 560, 346, 600, 362, 622), 2.4, Ink, taper: true);

        // Short-sleeved pale blue shirt, brown tie, a pen in the pocket.
        List<Pt> shirt = PathBuilder.From(68, 760).L(86, 690).C(142, 652, 204, 636, 252, 626).L(304, 670).L(356, 624)
            .C(408, 634, 466, 652, 514, 690).L(536, 760);
        s.Fill(shirt, Rgb.Hex("b9cde0"));
        s.SoftFill(PathBuilder.From(86, 690).C(142, 652, 204, 636, 252, 626).L(270, 660).L(200, 760).L(68, 760), Rgb.Hex("8ea6be"), 0.8, 10, shirt);
        s.Stroke(Close(shirt), 2.4, Ink);
        foreach (var flap in new[] { P(252, 630, 240, 668, 290, 676, 304, 670), P(356, 628, 370, 666, 318, 676, 304, 670) })
        {
            s.Fill(flap, Rgb.Hex("c9dbec"));
            s.Stroke(Close(flap), 1.8, Ink);
        }
        var tie = P(296, 676, 312, 676, 318, 690, 330, 760, 278, 760, 290, 690);
        s.Fill(tie, Rgb.Hex("8a6a3a"));
        foreach (var (x, y) in new[] { (300.0, 702.0), (312.0, 720.0), (296.0, 738.0), (316.0, 750.0) })
            s.Fill(Shapes.Ellipse(x, y, 3, 2.4), Rgb.Hex("c9a46a"));
        s.Stroke(Close(tie), 1.8, Ink);
        s.Fill(P(294, 664, 314, 664, 312, 680, 296, 680), Rgb.Hex("7a5a30"));
        s.Stroke(Close(P(294, 664, 314, 664, 312, 680, 296, 680)), 1.6, Ink);
        s.Stroke(Close(P(388, 698, 434, 694, 436, 740, 390, 742)), 1.6, Ink, 0.7);
        s.Stroke(P(404, 684, 406, 712), 3, Rgb.Hex("2a3a7a"));

        // Face: long, high forehead, soft jaw, a chin that retreats.
        List<Pt> face = PathBuilder.From(232, 268).C(224, 320, 224, 380, 234, 420).C(244, 458, 262, 488, 286, 506)
            .C(304, 516, 330, 514, 350, 498).C(374, 474, 390, 438, 394, 398).C(398, 350, 396, 300, 384, 262)
            .C(368, 214, 330, 196, 298, 196).C(262, 196, 240, 224, 232, 268);
        s.Fill(face, Skin);
        s.SoftFill(PathBuilder.From(232, 268).C(224, 320, 224, 380, 234, 420).C(244, 458, 262, 488, 286, 506).C(262, 470, 250, 426, 248, 380).C(246, 330, 250, 290, 260, 262), Shade, 0.9, 7, face);
        s.SoftFill(Shapes.Ellipse(354, 410 - r.CheekR, 22, 12), Rgb.Hex("e8857a"), 0.45, 8, face);
        s.SoftFill(Shapes.Ellipse(252, 414 - r.CheekL, 13, 9), Rgb.Hex("e8857a"), 0.35, 6, face);
        s.Stroke(Close(face), 2.6, Ink);
        // The retreating chin.
        s.Stroke(PathBuilder.From(278, 486).C(290, 494, 304, 494, 316, 488), 1.8, Deep, 0.5, taper: true);
        s.Fill(Shapes.Ellipse(352, 402, 2.6, 2.6), Rgb.Hex("6a4034"));

        // Thin hair at the sides, and the combover.
        List<Pt> sideNear = PathBuilder.From(384, 262).C(398, 284, 404, 316, 398, 342).L(388, 336).C(390, 310, 388, 286, 376, 266);
        List<Pt> sideFar = PathBuilder.From(236, 272).C(228, 296, 226, 320, 230, 340).L(238, 334).C(236, 312, 238, 292, 244, 276);
        foreach (var side in new[] { sideNear, sideFar })
        {
            s.Fill(side, Rgb.Hex("5a4434"));
            s.Stroke(Close(side), 1.6, Ink, 0.8);
        }
        for (int i = 0; i < 9; i++)
        {
            double y = 212 + i * 3.5;
            s.Stroke(PathBuilder.From(244, y + 40).C(270, y, 330, y - 10, 382, y + 34), 2.2, Rgb.Hex("5a4434"), 0.85, taper: true, clip: face);
        }

        // Big near ear.
        var ear = PathBuilder.From(390, 326).C(420, 312, 430, 358, 416, 390).C(408, 406, 394, 404, 388, 394).Points;
        s.Fill(ear, Skin);
        s.SoftFill(Shapes.Ellipse(406, 360, 7, 18), Shade, 0.8, 3, ear);
        s.SoftFill(Shapes.Ellipse(410, 360, 9, 22), Rgb.Hex("e8857a"), 0.3, 4, ear);
        s.Stroke(ear, 2.4, Ink, taper: true);

        // Eyes, slightly magnified behind the lenses.
        var near = new EyeSpec(new Pt(340, 346), 18, 9, -1, Tilt: 1, Hood: 0.2, Iris: "6a7a52", LidWeight: 2.2, Bags: 0.3);
        var far = new EyeSpec(new Pt(258, 350), 14, 8, 1, Tilt: 1, Hood: 0.2, Iris: "6a7a52", LidWeight: 2, Bags: 0.25);
        kit.Eye(near, r.OpenR, r.LiftR, r.GazeX, r.GazeY, face);
        kit.Eye(far, r.OpenL, r.LiftL, r.GazeX, r.GazeY, face);
        kit.Brow(new BrowSpec(new Pt(318, 318), new Pt(366, 314), 5, 2.6, "5a4434"), r.BrowInR, r.BrowOutR, 7);
        kit.Brow(new BrowSpec(new Pt(280, 320), new Pt(242, 324), 4, 2.4, "5a4434"), r.BrowInL, r.BrowOutL, 8);
        if (r.BrowInL > 5)
            foreach (double y in new[] { 280.0, 292.0 })
                kit.Crease(PathBuilder.From(268, y + 3).C(290, y - 2, 330, y - 2, 352, y + 3), 0.4, 1.4);

        // Long nose, pointing to his right.
        s.SoftFill(PathBuilder.From(300, 352).C(296, 374, 290, 392, 282, 404).L(296, 410).C(298, 390, 302, 370, 306, 352), Shade, 0.7, 3, face);
        s.Stroke(PathBuilder.From(302, 350).C(298, 372, 290, 390, 278, 402), 1.8, Deep, 0.8, taper: true);
        s.Stroke(PathBuilder.From(276, 404).C(278, 414, 294, 416, 304, 410), 2.4, Ink, taper: true);
        s.Fill(Shapes.Ellipse(290, 410, 3.6, 1.8), Rgb.Hex("6a4034"));

        kit.Crease(PathBuilder.From(310, 416).C(320, 428, 324, 440, 322 + r.Stretch * 0.4, 452), 0.35 + r.CornerR * 0.03, 1.8);
        kit.Mouth(new MouthSpec(new Pt(292, 444), 21, 3, 5, "b8706a", "cc847a", Overbite: 4), r, face);

        // Big glasses, a little crooked.
        var lensNear = PathBuilder.From(312, 324).L(368, 322).C(372, 322, 374, 326, 372, 336).L(366, 368).C(364, 374, 360, 376, 354, 376).L(320, 376).C(314, 376, 310, 372, 310, 366).L(308, 332).C(308, 326, 310, 324, 312, 324).Points;
        var lensFar = PathBuilder.From(236, 328).L(280, 326).C(284, 326, 286, 330, 284, 338).L(280, 368).C(278, 374, 274, 376, 270, 376).L(242, 376).C(236, 376, 234, 372, 234, 366).L(232, 334).C(232, 330, 234, 328, 236, 328).Points;
        foreach (var lens in new[] { lensNear, lensFar })
        {
            s.Fill(lens, Rgb.Hex("dfeaf2"), 0.15);
            s.Stroke(Close(lens), 3.4, Rgb.Hex("3a2e28"));
        }
        s.Stroke(P(336, 330, 352, 344), 2, Rgb.Hex("ffffff"), 0.6);
        s.Stroke(P(250, 334, 262, 346), 1.8, Rgb.Hex("ffffff"), 0.5);
        s.Stroke(PathBuilder.From(284, 336).C(292, 332, 302, 332, 310, 334), 3, Rgb.Hex("3a2e28"));
        s.Stroke(PathBuilder.From(372, 330).C(384, 330, 392, 334, 400, 340), 3, Rgb.Hex("3a2e28"));
    }
}
