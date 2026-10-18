using OfficeMan.ArtGen.Raster;
using static OfficeMan.ArtGen.Drawn.PathTools;

namespace OfficeMan.ArtGen.Drawn.Cast;

/// <summary>
/// Stojan Petrovici, the neighbour. Late fifties, big, bald with a grey fringe, a thick moustache, a vest,
/// a cigarette. Loud, friendly, a little too familiar. Three-quarter view to the viewer's left.
/// Temporary portrait in the mature animation style (art direction paused).
/// </summary>
public sealed class Stojan : CastMember
{
    public override string Name => "stojan";

    public override Rig Neutral => new() { Name = "neutral", CornerL = 5, CornerR = 4, OpenL = 0.85, OpenR = 0.9, BrowOutL = 2 };

    /// <summary>The big laugh: "You fix this, you can marry my daughter."</summary>
    public override Rig Distinct => new()
    {
        Name = "laughing", CornerL = 12, CornerR = 11, Open = 0.9, Stretch = 6, OpenL = 0.12, OpenR = 0.12, LiftL = 0.9, LiftR = 0.9,
        CheekL = 6, CheekR = 6, BrowInL = 4, BrowInR = 4, BrowOutL = 5, BrowOutR = 5,
    };

    private static readonly Rgb Skin = Rgb.Hex("d9a07e"), Shade = Rgb.Hex("b4785c"), Deep = Rgb.Hex("8a5440");

    protected override void Paint(Sketch s, Rig r)
    {
        Backdrop(s, "d2cdbd", "8a8676");
        var kit = new FaceKit(s, Ink, Shade, Deep);

        // Thick neck straight into big shoulders; a white vest; hair on the shoulders.
        List<Pt> body = PathBuilder.From(232, 470).C(226, 540, 210, 572, 170, 592).C(110, 610, 50, 632, 10, 680)
            .L(0, 760).L(600, 760).L(590, 680).C(550, 630, 490, 608, 430, 590).C(392, 570, 378, 540, 374, 466);
        s.Fill(body, Skin);
        s.SoftFill(PathBuilder.From(228, 480).C(280, 560, 350, 560, 378, 476).L(386, 580).C(320, 610, 270, 610, 222, 580), Shade, 0.9, 8, body);
        var rng = new Random(15);
        for (int i = 0; i < 60; i++)
        {
            double x = 110 + rng.NextDouble() * 380, y = 600 + rng.NextDouble() * 60;
            if (x > 230 && x < 380) continue;
            s.Stroke(PathBuilder.From(x, y).C(x + 2, y - 2, x + 4, y + 1, x + 3, y + 4), 1.3, Rgb.Hex("5a4034"), 0.55, taper: true);
        }
        List<Pt> vest = PathBuilder.From(150, 760).L(176, 640).C(200, 620, 228, 620, 244, 640).C(270, 690, 336, 690, 362, 640)
            .C(378, 620, 410, 620, 428, 640).L(452, 760);
        s.Fill(vest, Rgb.Hex("ece8dc"));
        s.SoftFill(PathBuilder.From(150, 760).L(176, 640).C(200, 620, 228, 620, 244, 640).L(260, 760), Rgb.Hex("b8b2a2"), 0.8, 10, vest);
        s.Stroke(Close(vest), 2.4, Ink);
        s.Stroke(PathBuilder.From(232, 470).C(226, 540, 210, 572, 170, 592).C(110, 610, 50, 632, 10, 680), 2.6, Ink, taper: true);
        s.Stroke(PathBuilder.From(374, 466).C(378, 540, 392, 570, 430, 590).C(490, 608, 550, 630, 590, 680), 2.6, Ink, taper: true);

        // A big round head, heavy jaw, bald dome.
        List<Pt> face = PathBuilder.From(196, 300).C(186, 360, 190, 430, 214, 474).C(236, 510, 270, 528, 304, 530)
            .C(346, 530, 384, 508, 402, 468).C(420, 426, 424, 360, 414, 300).C(400, 220, 360, 182, 304, 180)
            .C(250, 180, 208, 220, 196, 300);
        s.Fill(face, Skin);
        s.SoftFill(PathBuilder.From(196, 300).C(186, 360, 190, 430, 214, 474).C(236, 510, 270, 528, 304, 530).C(270, 500, 238, 460, 228, 410).C(220, 360, 222, 320, 230, 280), Shade, 1, 8, face);
        s.SoftFill(Shapes.Ellipse(300, 214, 50, 20), Rgb.Hex("f2c8a6"), 0.8, 10, face);   // shine on the dome
        s.SoftFill(Shapes.Ellipse(250, 420, 22, 14), Rgb.Hex("e07a6a"), 0.4, 8, face);
        s.SoftFill(Shapes.Ellipse(368, 416, 26, 15), Rgb.Hex("e07a6a"), 0.45, 8, face);
        s.Stroke(Close(face), 2.8, Ink);

        // The grey fringe round the back of the head, the ears.
        var fringe = Lobes(Pts(200, 330, 196, 290, 206, 256, 222, 238), new Pt(260, 300), 0.4, close: false);
        s.Stroke(fringe, 12, Rgb.Hex("8a847c"), taper: true);
        var fringeR = Lobes(Pts(408, 256, 418, 290, 416, 330), new Pt(360, 300), 0.4, close: false);
        s.Stroke(fringeR, 10, Rgb.Hex("8a847c"), taper: true);
        var ear = PathBuilder.From(412, 352).C(436, 344, 442, 384, 428, 410).C(420, 420, 410, 418, 406, 408).Points;
        s.Fill(ear, Skin);
        s.SoftFill(Shapes.Ellipse(424, 380, 6, 16), Shade, 0.8, 3, ear);
        s.Stroke(ear, 2.4, Ink, taper: true);

        // Eyes small in a big face; bushy brows.
        var near = new EyeSpec(new Pt(346, 360), 20, 8.5, -1, Tilt: -1, Hood: 0.6, Iris: "4a3a2a", LidWeight: 2.6, Bags: 0.55);
        var far = new EyeSpec(new Pt(258, 364), 16, 8, 1, Tilt: -1, Hood: 0.6, Iris: "4a3a2a", LidWeight: 2.4, Bags: 0.5);
        kit.Eye(near, r.OpenR, r.LiftR, r.GazeX, r.GazeY, face);
        kit.Eye(far, r.OpenL, r.LiftL, r.GazeX, r.GazeY, face);
        kit.Brow(new BrowSpec(new Pt(318, 334), new Pt(374, 336), 6, 7, "6a625a", Hairs: 22), r.BrowInR, r.BrowOutR, 9);
        kit.Brow(new BrowSpec(new Pt(282, 338), new Pt(236, 344), 5, 6, "6a625a", Hairs: 18), r.BrowInL, r.BrowOutL, 10);

        // A broad nose, red at the end.
        s.SoftFill(PathBuilder.From(298, 372).C(292, 396, 280, 414, 276, 426).L(296, 432).C(298, 410, 302, 392, 304, 372), Shade, 0.7, 4, face);
        s.SoftFill(Shapes.Ellipse(290, 424, 14, 10), Rgb.Hex("d86a5a"), 0.4, 5, face);
        s.Stroke(PathBuilder.From(270, 426).C(266, 440, 282, 446, 296, 442).C(306, 446, 320, 442, 322, 432), 2.6, Ink, taper: true);
        s.Fill(Shapes.Ellipse(286, 438, 5, 2.4), Rgb.Hex("5a2e24"));

        kit.Crease(PathBuilder.From(320, 440).C(338, 456, 344, 474, 340, 494), 0.6, 2.4);
        kit.Crease(PathBuilder.From(264, 442).C(252, 456, 250, 470, 254, 488), 0.5, 2.2);
        kit.Mouth(new MouthSpec(new Pt(298, 482), 32, 3.5, 7, "9a5a48", "b06a56"), r, face);

        // The moustache: thick, greying, hiding the top lip.
        var tash = PathBuilder.From(256, 470).C(262, 452, 284, 448, 300, 456).C(316, 448, 340, 452, 348, 470)
            .C(330, 466, 316, 464, 300, 468).C(284, 464, 270, 466, 256, 470).Points;
        s.Fill(tash, Rgb.Hex("6a5e54"));
        for (int i = 0; i < 18; i++)
        {
            double x = 262 + i * 4.8;
            s.Stroke(new List<Pt> { new(x, 456 + Math.Abs(x - 300) * 0.05), new(x + 1, 468) }, 1.4, Rgb.Hex("9a9086"), 0.6, taper: true, clip: tash);
        }
        s.Stroke(Close(tash), 2, Ink, 0.8);

        // The cigarette, in the corner of his mouth.
        s.Stroke(new List<Pt> { new(340, 482), new(392, 496) }, 5, Rgb.Hex("f2ece0"));
        s.Stroke(new List<Pt> { new(388, 495), new(394, 497) }, 5, Rgb.Hex("d06a3a"));
        s.Stroke(new List<Pt> { new(340, 482), new(392, 496) }, 1.4, Ink, 0.6);
        for (int i = 0; i < 3; i++)
            s.Stroke(PathBuilder.From(396, 490 - i * 30).C(420, 470 - i * 30, 380, 450 - i * 30, 404, 430 - i * 30), 3, Rgb.Hex("dcd8d0"), 0.35 - i * 0.08, taper: true);
    }
}
