using OfficeMan.ArtGen.Raster;
using static OfficeMan.ArtGen.Drawn.PathTools;

namespace OfficeMan.ArtGen.Drawn.Cast;

/// <summary>
/// Zdenka Horák, about 62, the Veteran. Long, narrow face, hollow cheeks, a thin straight nose,
/// thin lips that turn down at rest. Grey hair scraped into a bun; half-moon reading glasses on a
/// chain, which she looks over. A face that measures you. Almost frontal.
/// </summary>
public sealed class Zdenka : CastMember
{
    public override string Name => "zdenka";

    public override Rig Neutral => new() { Name = "neutral", CornerL = -2, CornerR = -2.5, OpenL = 0.75, OpenR = 0.72, GazeY = -0.3 };

    /// <summary>The rare smile. The mouth does it; the eyes don't join in.</summary>
    public override Rig Distinct => new()
    {
        Name = "smile", CornerL = 6.5, CornerR = 8.5, Press = 0.6, Stretch = 4, OpenL = 0.72, OpenR = 0.7, LiftL = 0.1, LiftR = 0.1, GazeY = -0.3,
    };

    private static readonly Rgb Skin = Rgb.Hex("ecd3c4"), Shade = Rgb.Hex("c4a196"), Deep = Rgb.Hex("8f6c66");

    protected override void Paint(Sketch s, Rig r)
    {
        Backdrop(s, "d3d8cf", "8d978c");
        var kit = new FaceKit(s, Ink, Shade, Deep);

        // The bun, behind the head.
        var bun = Shapes.Ellipse(338, 176, 36, 28);
        s.Fill(bun, Rgb.Hex("9c989e"));
        s.SoftFill(Shift(bun, 10, 8), Rgb.Hex("6c6870"), 0.8, 6, bun);
        for (int i = 0; i < 5; i++)
            s.Stroke(PathBuilder.From(310, 168 + i * 4).C(326, 156 + i * 4, 352, 158 + i * 4, 368, 172 + i * 4), 1.2, Rgb.Hex("5c5860"), 0.6, taper: true, clip: bun);
        s.Stroke(Close(bun), 2.2, Ink);

        // A thin neck with its tendons; narrow shoulders.
        List<Pt> body = PathBuilder.From(266, 500).C(264, 556, 256, 584, 236, 602).C(196, 616, 140, 628, 96, 660)
            .L(80, 760).L(520, 760).L(506, 660).C(462, 628, 404, 616, 366, 602).C(346, 584, 338, 556, 338, 498);
        s.Fill(body, Skin);
        s.SoftFill(PathBuilder.From(262, 504).C(290, 548, 320, 548, 342, 502).L(346, 560).C(320, 580, 286, 580, 258, 560), Shade, 0.8, 6, body);
        kit.Crease(PathBuilder.From(284, 548).C(280, 570, 276, 590, 268, 604), 0.5, 2);
        kit.Crease(PathBuilder.From(320, 548).C(324, 570, 328, 590, 336, 604), 0.5, 2);

        // Blouse buttoned to the top, a grey suit, a brooch.
        List<Pt> blouse = PathBuilder.From(232, 604).C(260, 596, 340, 596, 370, 604).L(380, 640).L(222, 640);
        s.Fill(blouse, Rgb.Hex("ece7dc"));
        s.Stroke(Close(blouse), 2, Ink);
        s.Stroke(P(301, 604, 301, 640), 1.2, Deep, 0.5);
        List<Pt> suit = PathBuilder.From(76, 760).L(90, 658).C(136, 628, 190, 614, 234, 606).L(300, 700).L(366, 606)
            .C(410, 614, 464, 628, 510, 658).L(524, 760);
        s.Fill(suit, Rgb.Hex("6c6e72"));
        s.SoftFill(PathBuilder.From(90, 658).C(136, 628, 190, 614, 234, 606).L(260, 650).L(200, 760).L(76, 760), Rgb.Hex("46484c"), 0.8, 10, suit);
        s.Stroke(PathBuilder.From(234, 606).L(300, 700).L(366, 606), 2.2, Ink);
        s.Stroke(Close(suit), 2.4, Ink);
        s.Fill(Shapes.Ellipse(402, 668, 9, 10), Rgb.Hex("c9a14a"));
        s.Fill(Shapes.Ellipse(402, 668, 6, 7), Rgb.Hex("3c7a5a"));
        s.Fill(Shapes.Ellipse(400, 665, 2, 2), Rgb.Hex("9ad8b4"));

        // Face: long, narrow, high cheekbones, a pointed chin.
        List<Pt> face = PathBuilder.From(244, 252).C(232, 300, 228, 360, 234, 410).C(238, 450, 252, 490, 278, 516)
            .C(292, 530, 308, 534, 322, 528).C(348, 514, 364, 480, 370, 440).C(376, 396, 376, 330, 366, 278)
            .C(354, 234, 322, 212, 300, 212).C(274, 212, 254, 228, 244, 252);
        s.Fill(face, Skin);
        s.SoftFill(PathBuilder.From(244, 252).C(232, 300, 228, 360, 234, 410).C(238, 450, 252, 490, 278, 516).C(262, 478, 254, 430, 252, 380).C(250, 330, 254, 290, 262, 256), Shade, 0.9, 7, face);
        // Hollows under the cheekbones.
        s.SoftFill(PathBuilder.From(246, 410).C(258, 426, 270, 446, 278, 470).L(270, 472).C(258, 452, 248, 432, 242, 418), Shade, 0.9, 6, face);
        s.SoftFill(PathBuilder.From(358, 406).C(346, 422, 336, 444, 330, 468).L(338, 470).C(348, 450, 358, 430, 364, 414), Shade, 0.8, 6, face);
        s.Stroke(Close(face), 2.6, Ink);

        // Small ears, both visible.
        foreach (var (x, dir) in new[] { (236.0, -1), (368.0, 1) })
        {
            var ear = PathBuilder.From(x, 336).C(x + dir * 14, 332, x + dir * 16, 362, x + dir * 8, 380).C(x + dir * 4, 386, x, 384, x - dir * 2, 378).Points;
            s.Fill(ear, Skin);
            s.SoftFill(Shapes.Ellipse(x + dir * 7, 358, 4, 11), Shade, 0.8, 3, ear);
            s.Stroke(ear, 2.2, Ink, taper: true);
        }

        // Eyes: small, pale, drooping at the outer corners. No make-up worth mentioning.
        var left = new EyeSpec(new Pt(272, 358), 16, 7, 1, Tilt: -2.5, Hood: 0.7, Iris: "8aa0b0", LidWeight: 2.2, Bags: 0.65);
        var right = new EyeSpec(new Pt(330, 356), 16, 7, -1, Tilt: -2.5, Hood: 0.7, Iris: "8aa0b0", LidWeight: 2.2, Bags: 0.65);
        kit.Eye(left, r.OpenL, r.LiftL, r.GazeX, r.GazeY, face);
        kit.Eye(right, r.OpenR, r.LiftR, r.GazeX, r.GazeY, face);
        kit.Brow(new BrowSpec(new Pt(290, 338), new Pt(252, 342), 3, 2.4, "7e746e", Hairs: 14), r.BrowInL, r.BrowOutL, 5);
        kit.Brow(new BrowSpec(new Pt(312, 336), new Pt(350, 340), 4, 2.4, "7e746e", Hairs: 14), r.BrowInR, r.BrowOutR, 6);
        // The two lines between the brows that never go away.
        kit.Crease(P(296, 330, 297, 348), 0.75, 1.8);
        kit.Crease(P(305, 330, 306, 346), 0.6, 1.6);

        // Nose: long, straight, narrow.
        s.SoftFill(PathBuilder.From(306, 356).C(308, 384, 310, 404, 312, 420).L(304, 424).C(304, 404, 302, 384, 302, 356), Shade, 0.7, 3, face);
        s.Stroke(PathBuilder.From(292, 422).C(290, 430, 296, 434, 302, 432).C(308, 434, 314, 430, 312, 422), 2.2, Ink, taper: true);
        s.Fill(Shapes.Ellipse(297, 430, 3, 1.6), Rgb.Hex("6a4a44"));
        s.Fill(Shapes.Ellipse(308, 430, 3, 1.6), Rgb.Hex("6a4a44"));

        kit.Crease(PathBuilder.From(288, 434).C(278, 448, 276, 462, 278, 478), 0.55, 1.8);
        kit.Crease(PathBuilder.From(316, 434).C(326, 448, 328, 462, 326, 478), 0.55, 1.8);
        kit.Crease(PathBuilder.From(280, 480).C(278, 492, 280, 502, 284, 510), 0.5, 1.6); // marionette
        kit.Crease(PathBuilder.From(322, 480).C(324, 492, 322, 502, 318, 510), 0.5, 1.6);

        kit.Mouth(new MouthSpec(new Pt(301, 466), 21, 2.5, 4.5, "a47270", "bd8882", Sag: 1.5), r, face);

        // Hair: scraped back flat, a centre parting.
        List<Pt> hair = PathBuilder.From(234, 318).C(224, 232, 260, 180, 300, 176).C(344, 174, 380, 216, 370, 318)
            .C(366, 290, 354, 266, 332, 256).C(318, 250, 308, 250, 300, 256).C(292, 250, 282, 250, 268, 256)
            .C(248, 266, 238, 290, 234, 318);
        s.Fill(hair, Rgb.Hex("a6a2a8"));
        s.SoftFill(Shift(hair, -12, 8), Rgb.Hex("cfcbd2"), 0.7, 6, hair);
        for (int i = 0; i < 9; i++)
        {
            double t = i / 8.0;
            s.Stroke(PathBuilder.From(300 + (t - 0.5) * 8, 256).C(280 - t * 30, 236, 258 - t * 14, 210 + t * 40, 246 + t * 6, 280 + t * 34), 1.1, Rgb.Hex("77737a"), 0.55, taper: true, clip: hair);
            s.Stroke(PathBuilder.From(300 + (t - 0.5) * 8, 256).C(320 + t * 30, 236, 342 + t * 14, 210 + t * 40, 354 - t * 6, 280 + t * 34), 1.1, Rgb.Hex("77737a"), 0.55, taper: true, clip: hair);
        }
        s.Stroke(Close(hair), 2.4, Ink);

        // Half-moon reading glasses, low on the nose; she looks over them. A chain hangs from the arms.
        foreach (var (cx, dir) in new[] { (272.0, 1), (330.0, -1) })
        {
            var lens = PathBuilder.From(cx - 19, 372).L(cx + 19, 372).C(cx + 18, 394, cx - 18, 394, cx - 19, 372).Points;
            s.Fill(lens, Rgb.Hex("e6eef2"), 0.18);
            s.Stroke(Close(lens), 2, Rgb.Hex("8a7a5a"));
            s.Stroke(P(cx - 10, 380, cx - 4, 376), 1.4, Rgb.Hex("ffffff"), 0.7);
        }
        s.Stroke(PathBuilder.From(291, 374).C(296, 370, 306, 370, 311, 374), 2, Rgb.Hex("8a7a5a"));
        s.Stroke(PathBuilder.From(253, 372).C(244, 374, 240, 370, 236, 366), 1.8, Rgb.Hex("8a7a5a"));
        s.Stroke(PathBuilder.From(349, 372).C(358, 374, 362, 370, 366, 366), 1.8, Rgb.Hex("8a7a5a"));
        s.Stroke(PathBuilder.From(236, 368).C(220, 460, 236, 560, 272, 600), 1.1, Rgb.Hex("b89a5a"), 0.9);
        s.Stroke(PathBuilder.From(366, 368).C(382, 460, 368, 560, 332, 600), 1.1, Rgb.Hex("b89a5a"), 0.9);
    }
}
