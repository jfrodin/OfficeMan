using OfficeMan.ArtGen.Raster;
using static OfficeMan.ArtGen.Drawn.PathTools;

namespace OfficeMan.ArtGen.Drawn.Cast;

/// <summary>
/// Viktor Baranyi, 50, the Boss. Good-looking and knows it, a little past it and doesn't.
/// Square jaw going soft at the edges, a nose that was broken once, deep-set eyes, swept-back
/// hair greying at the temples, a tan, stubble, collar one button too open. Three-quarter view
/// to the viewer's right.
/// </summary>
public sealed class Viktor : CastMember
{
    public override string Name => "viktor";

    /// <summary>His resting face is a half-smile on one side.</summary>
    public override Rig Neutral => new() { Name = "neutral", CornerL = 4.5, CornerR = 0.5, OpenL = 0.85, OpenR = 0.85, BrowOutL = 1.5 };

    /// <summary>The calm when someone has crossed him: the smile stays, nothing else moves with it.</summary>
    public override Rig Distinct => new()
    {
        Name = "displeased", Press = 1, CornerL = 3, CornerR = -2, OpenL = 0.4, OpenR = 0.36, LiftL = 0.5, LiftR = 0.5,
        BrowInL = -9, BrowInR = -9, BrowOutL = -2, BrowOutR = -3,
    };

    private static readonly Rgb Skin = Rgb.Hex("c98f6c"), Shade = Rgb.Hex("a66a50"), Deep = Rgb.Hex("7c4838");

    protected override void Paint(Sketch s, Rig r)
    {
        Backdrop(s, "c9d2cf", "7f918f");
        var kit = new FaceKit(s, Ink, Shade, Deep);

        // Thick neck, broad shoulders.
        List<Pt> body = PathBuilder.From(244, 470).C(240, 540, 232, 580, 212, 600).C(160, 620, 90, 632, 30, 662)
            .L(20, 760).L(580, 760).L(574, 662).C(520, 630, 440, 614, 398, 598).C(376, 576, 372, 532, 380, 476);
        s.Fill(body, Skin);
        s.SoftFill(PathBuilder.From(240, 480).C(290, 560, 360, 560, 384, 480).L(390, 590).C(330, 616, 280, 616, 236, 590), Shade, 0.9, 8, body);
        // Chest hair in the open collar, and a thin gold chain.
        var rng = new Random(50);
        for (int i = 0; i < 26; i++)
        {
            double x = 296 + rng.NextDouble() * 34, y = 628 + rng.NextDouble() * 40;
            s.Stroke(PathBuilder.From(x, y).C(x + 2, y - 2, x + 4, y + 1, x + 3, y + 4), 1.3, Rgb.Hex("4a3a34"), 0.6, taper: true);
        }
        s.Stroke(PathBuilder.From(262, 590).C(280, 650, 344, 650, 362, 588), 1.6, Rgb.Hex("d7aa52"));

        // White shirt, open at the neck; navy jacket.
        List<Pt> shirt = PathBuilder.From(150, 626).C(196, 610, 236, 602, 268, 598).L(312, 690).L(356, 598)
            .C(388, 602, 428, 610, 472, 626).L(482, 760).L(140, 760);
        s.Fill(shirt, Rgb.Hex("ece9e4"));
        s.SoftFill(PathBuilder.From(140, 650).C(186, 626, 228, 612, 268, 602).L(300, 680).L(220, 760).L(140, 760), Rgb.Hex("b9bcc4"), 0.7, 10, shirt);
        foreach (var flap in new[] { P(268, 600, 246, 650, 298, 660, 312, 690), P(356, 600, 376, 650, 326, 660, 312, 690) })
        {
            s.Fill(flap, Rgb.Hex("f4f2ee"));
            s.Stroke(Close(flap), 2, Ink);
        }
        List<Pt> jacketL = PathBuilder.From(16, 664).C(70, 628, 140, 610, 204, 604).L(250, 692).L(268, 760).L(16, 760);
        List<Pt> jacketR = PathBuilder.From(584, 664).C(530, 628, 460, 610, 400, 604).L(370, 692).L(354, 760).L(584, 760);
        foreach (var j in new[] { jacketL, jacketR })
        {
            s.Fill(j, Rgb.Hex("2c3546"));
            s.Stroke(Close(j), 2.2, Ink);
        }
        s.SoftFill(Shapes.Ellipse(500, 690, 60, 20), Rgb.Hex("4a5670"), 0.6, 10, jacketR);

        // Face: square, heavy jaw, wide chin, cheekbones still there.
        List<Pt> face = PathBuilder.From(232, 250).C(214, 300, 212, 356, 218, 400).C(222, 440, 232, 478, 258, 506)
            .C(280, 526, 312, 536, 340, 532).C(364, 528, 380, 510, 388, 486).C(396, 452, 398, 410, 402, 372)
            .C(408, 340, 404, 300, 394, 268).C(376, 226, 330, 210, 290, 212).C(262, 214, 242, 228, 232, 250);
        s.Fill(face, Skin);
        // The far side turns away from the light; the jaw has weight under it.
        s.SoftFill(PathBuilder.From(394, 268).C(404, 300, 408, 340, 402, 372).C(398, 410, 396, 452, 388, 486).C(380, 510, 364, 528, 340, 532)
            .C(366, 500, 376, 452, 378, 410).C(380, 370, 382, 320, 372, 280), Shade, 1, 7, face);
        s.SoftFill(Shapes.Ellipse(258, 300, 30, 14), Rgb.Hex("dca684"), 0.6, 10, face);
        // Stubble: a cool shadow and a scatter of dots on the jaw, chin and lip.
        List<Pt> stubble = PathBuilder.From(222, 430).C(232, 476, 262, 512, 300, 530).C(332, 538, 368, 526, 388, 488).L(392, 440)
            .C(372, 452, 352, 448, 338, 440).C(326, 436, 304, 436, 286, 444).C(262, 452, 240, 446, 222, 430);
        s.SoftFill(stubble, Rgb.Hex("5e6672"), 0.28, 8, face);
        for (int i = 0; i < 420; i++)
        {
            double x = 220 + rng.NextDouble() * 175, y = 430 + rng.NextDouble() * 105;
            if (!Shapes.Inside(stubble, x, y) || !Shapes.Inside(face, x, y)) continue;
            s.Fill(Shapes.Ellipse(x, y, 0.8, 0.8, 6), Rgb.Hex("4a4e58"), 0.45);
        }
        s.Stroke(Close(face), 2.8, Ink);
        s.Stroke(P(338, 518, 340, 530), 1.6, Deep, 0.8); // cleft

        // The near ear.
        var ear = PathBuilder.From(220, 344).C(200, 338, 194, 370, 200, 394).C(204, 408, 216, 410, 224, 400).Points;
        s.Fill(ear, Skin);
        s.SoftFill(Shapes.Ellipse(208, 372, 5, 14), Shade, 0.8, 3, ear);
        s.Stroke(ear, 2.4, Ink, taper: true);

        // Eyes: deep-set, hooded, the tan stops at the crow's feet.
        var near = new EyeSpec(new Pt(268, 364), 20, 8, 1, Tilt: -1, Hood: 0.85, Iris: "3e2a1e", LidWeight: 2.8, Bags: 0.5);
        var far = new EyeSpec(new Pt(348, 362), 15, 7.5, -1, Tilt: -1, Hood: 0.85, Iris: "3e2a1e", LidWeight: 2.4, Bags: 0.45);
        s.SoftFill(Shapes.Ellipse(268, 356, 30, 14), Shade, 0.6, 6, face);
        s.SoftFill(Shapes.Ellipse(348, 354, 22, 13), Shade, 0.6, 6, face);
        kit.Eye(near, r.OpenL, r.LiftL, r.GazeX, r.GazeY, face);
        kit.Eye(far, r.OpenR, r.LiftR, r.GazeX, r.GazeY, face);
        kit.Brow(new BrowSpec(new Pt(292, 340), new Pt(240, 342), 4, 6, "3a2e2c", Hairs: 22), r.BrowInL, r.BrowOutL, 3);
        kit.Brow(new BrowSpec(new Pt(322, 338), new Pt(372, 342), 3, 5, "3a2e2c", Hairs: 18), r.BrowInR, r.BrowOutR, 4);
        foreach (var (y0, y1) in new[] { (360.0, 354.0), (366.0, 368.0), (372.0, 380.0) })
            kit.Crease(P(242, y0, 230, y1), 0.55, 1.4);
        foreach (double y in new[] { 276.0, 290.0 })
            kit.Crease(PathBuilder.From(266, y + 3).C(290, y - 2, 330, y - 2, 352, y + 3), 0.35 - r.BrowInL * 0.04, 1.5);
        if (r.BrowInL < -2)
            kit.Crease(P(306, 340, 304, 354), 0.7, 1.8);

        // Nose: strong, with the bump.
        s.SoftFill(PathBuilder.From(310, 352).C(316, 378, 320, 400, 328, 420).L(316, 424).C(312, 400, 306, 378, 302, 354), Shade, 0.8, 3, face);
        s.Stroke(PathBuilder.From(302, 350).C(306, 368, 304, 378, 310, 390).C(314, 400, 318, 412, 322, 420), 2, Deep, 0.85, taper: true);
        s.Stroke(PathBuilder.From(292, 422).C(286, 432, 294, 442, 306, 438).C(316, 444, 330, 440, 334, 430), 2.6, Ink, taper: true);
        s.Fill(Shapes.Ellipse(304, 436, 5, 2.4), Rgb.Hex("4a2a22"));

        // Deep folds: he smiles for a living.
        kit.Crease(PathBuilder.From(290, 432).C(274, 450, 268, 470, 272 - r.CornerL * 0.3, 494), 0.75, 2.4);
        kit.Crease(PathBuilder.From(336, 434).C(348, 450, 352, 468, 350, 490), 0.6, 2.2);

        kit.Mouth(new MouthSpec(new Pt(314, 472), 30, 3.5, 7, "8e5244", "a8685a", Sag: 0.5), r, face);

        // Hair: swept back with product, receding at the temples, grey at the sides.
        List<Pt> hair = PathBuilder.From(212, 330).C(204, 280, 214, 232, 246, 196).C(276, 164, 330, 154, 370, 166)
            .C(404, 178, 422, 212, 420, 252).C(418, 276, 410, 296, 402, 310).C(398, 282, 388, 256, 372, 242)
            .C(348, 226, 318, 226, 296, 234).C(276, 228, 256, 236, 246, 252).C(236, 268, 232, 292, 230, 318);
        s.Fill(hair, Rgb.Hex("3c3432"));
        s.SoftFill(PathBuilder.From(212, 330).C(204, 280, 214, 240, 236, 214).L(256, 250).C(240, 270, 234, 294, 230, 318), Rgb.Hex("a7a3a0"), 0.85, 6, hair);
        s.SoftFill(PathBuilder.From(410, 290).C(420, 260, 420, 230, 410, 210).L(396, 250), Rgb.Hex("8f8b88"), 0.7, 5, hair);
        foreach (var (x0, y0) in new[] { (262.0, 232.0), (284.0, 222.0), (306.0, 220.0), (328.0, 220.0), (350.0, 226.0) })
            s.Stroke(PathBuilder.From(x0, y0).C(x0 + 14, y0 - 30, x0 + 44, y0 - 52, x0 + 76, y0 - 46), 1.8, Rgb.Hex("6a605c"), 0.8, taper: true, clip: hair);
        s.SoftFill(PathBuilder.From(256, 196).C(290, 172, 340, 166, 380, 178).L(376, 186).C(340, 176, 296, 182, 262, 204), Rgb.Hex("d8d4d2"), 0.55, 4, hair);
        s.Stroke(Close(hair), 2.6, Ink);
    }
}
