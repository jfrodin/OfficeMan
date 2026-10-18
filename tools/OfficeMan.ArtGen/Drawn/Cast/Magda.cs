using OfficeMan.ArtGen.Raster;
using static OfficeMan.ArtGen.Drawn.PathTools;

namespace OfficeMan.ArtGen.Drawn.Cast;

/// <summary>
/// Magda Pálffy, 52. Soft and heavy-set, a face that has laughed a lot and cried a fair amount.
/// Perm dyed burgundy with grey coming through, brows pencilled on not quite evenly, lipstick a
/// shade too bright. Three-quarter view to the viewer's left.
/// </summary>
public sealed class Magda : CastMember
{
    public override string Name => "magda";

    public override Rig Neutral => new() { Name = "neutral", CornerL = 1.5, CornerR = 2.5, OpenL = 0.85, OpenR = 0.9 };

    /// <summary>Laughing too loud, the way she does at Viktor's jokes.</summary>
    public override Rig Distinct => new()
    {
        Name = "laughing", CornerL = 13, CornerR = 15, Open = 1.1, Stretch = 8, OpenL = 0.12, OpenR = 0.12,
        LiftL = 0.9, LiftR = 0.9, CheekL = 6, CheekR = 7, BrowInL = 4, BrowInR = 5, BrowOutL = 6, BrowOutR = 7,
    };

    private static readonly Rgb Skin = Rgb.Hex("eebc9f"), Shade = Rgb.Hex("d4917a"), Deep = Rgb.Hex("a8604f");

    protected override void Paint(Sketch s, Rig r)
    {
        Backdrop(s, "eadacb", "b9a39a");
        var kit = new FaceKit(s, Ink, Shade, Deep);

        // Hair behind: a perm of many small lobes, fuller on top.
        var hair = Lobes(P(206, 470, 184, 420, 176, 364, 180, 306, 196, 254, 226, 210, 268, 182, 312, 172, 356, 178,
                           396, 200, 428, 236, 444, 286, 448, 342, 442, 398, 430, 448, 410, 488, 380, 506, 240, 508), new Pt(312, 340), 0.26);
        s.Fill(hair, Rgb.Hex("8e2f45"));
        s.Fill(hair, Rgb.Hex("5e1a30"), alphaAt: (x, y) => Shapes.Inside(hair, x - 24, y + 4) ? 0 : 1);
        foreach (var (cx, cy) in new[] { (300.0, 196.0), (350.0, 196.0), (394.0, 222.0), (420.0, 266.0) })
            s.SoftFill(Shapes.Ellipse(cx, cy, 20, 10), Rgb.Hex("bd5568"), 0.7, 4, hair);
        s.Stroke(Close(hair), 2.6, Ink);

        // Neck and shoulders: short neck, soft shoulders.
        List<Pt> body = PathBuilder.From(262, 506).C(258, 546, 248, 566, 226, 580).C(176, 598, 100, 612, 44, 652)
            .L(30, 760).L(570, 760).L(556, 652).C(504, 612, 432, 596, 388, 580).C(368, 566, 360, 544, 362, 502);
        s.Fill(body, Skin);
        s.SoftFill(PathBuilder.From(256, 510).C(290, 572, 350, 570, 368, 508).L(372, 572).C(330, 598, 284, 598, 250, 572), Shade, 0.9, 6, body);
        s.Stroke(PathBuilder.From(262, 506).C(258, 546, 248, 566, 226, 580).C(176, 598, 100, 612, 44, 652), 2.4, Ink, taper: true);
        s.Stroke(PathBuilder.From(362, 502).C(360, 544, 368, 566, 388, 580).C(432, 596, 504, 612, 556, 652), 2.4, Ink, taper: true);

        List<Pt> dress = PathBuilder.From(30, 670).C(96, 632, 166, 622, 212, 636).C(232, 690, 266, 712, 304, 714)
            .C(342, 712, 374, 690, 394, 636).C(440, 624, 508, 632, 570, 672).L(570, 760).L(30, 760);
        s.Fill(dress, Rgb.Hex("b3343f"));
        s.SoftFill(PathBuilder.From(30, 670).C(96, 632, 166, 622, 212, 636).C(220, 670, 232, 700, 246, 712).C(204, 730, 180, 750, 172, 760).L(30, 760), Rgb.Hex("7c1a28"), 0.8, 8, dress);
        s.Stroke(PathBuilder.From(212, 636).C(232, 690, 266, 712, 304, 714).C(342, 712, 374, 690, 394, 636), 2.4, Ink);
        s.Stroke(PathBuilder.From(272, 576).C(286, 650, 336, 650, 352, 572), 1.4, Rgb.Hex("cfa349"));

        // Face: round, soft, longer in the lower half than the cartoon was; jowls and a double chin.
        List<Pt> face = PathBuilder.From(216, 300).C(206, 352, 206, 412, 218, 454).C(232, 496, 262, 524, 300, 530)
            .C(346, 534, 388, 508, 404, 464).C(418, 426, 420, 364, 410, 314).C(398, 258, 352, 228, 304, 228)
            .C(256, 228, 224, 258, 216, 300);
        List<Pt> chin = PathBuilder.From(262, 520).C(290, 560, 352, 556, 382, 512).L(300, 520);
        s.Fill(chin, Skin);
        s.Fill(face, Skin);
        s.SoftFill(PathBuilder.From(216, 300).C(206, 352, 206, 412, 218, 454).C(232, 496, 262, 524, 300, 530).C(268, 506, 244, 470, 236, 430).C(228, 392, 230, 340, 240, 298), Shade, 1, 7, face);
        s.SoftFill(PathBuilder.From(380, 470).C(366, 500, 336, 520, 300, 530).L(310, 538).C(352, 534, 388, 508, 404, 464), Shade, 0.7, 6, face);
        double cheek = (r.CheekL + r.CheekR) * 0.5;
        s.SoftFill(Shapes.Ellipse(352, 424 - cheek, 26, 15), Rgb.Hex("e88378"), 0.45, 8, face);
        s.SoftFill(Shapes.Ellipse(244, 428 - cheek, 16, 11), Rgb.Hex("e88378"), 0.35, 6, face);
        s.Stroke(Close(face), 2.6, Ink);
        s.Stroke(PathBuilder.From(266, 528).C(292, 558, 350, 554, 380, 516), 2.2, Ink, 0.8, taper: true);

        // Eyes: smaller than the cartoon, hooded, too much blue.
        var near = new EyeSpec(new Pt(346, 360), 25, 11.5, -1, Tilt: -1.5, Hood: 0.55, Iris: "5c3a26", LidWeight: 3, Lashes: true, Shadow: "6f8fd8", Bags: 0.5);
        var far = new EyeSpec(new Pt(254, 364), 19, 10.5, 1, Tilt: -1, Hood: 0.55, Iris: "5c3a26", LidWeight: 2.6, Lashes: true, Shadow: "6f8fd8", Bags: 0.45);
        kit.Eye(near, r.OpenR, r.LiftR, r.GazeX, r.GazeY, face);
        kit.Eye(far, r.OpenL, r.LiftL, r.GazeX, r.GazeY, face);
        // Pencilled brows that don't quite match: the far one sits lower and flatter.
        kit.Brow(new BrowSpec(new Pt(314, 330), new Pt(374, 332), 9, 3.2, "5a2430"), r.BrowInR, r.BrowOutR, 1);
        kit.Brow(new BrowSpec(new Pt(278, 334), new Pt(232, 342), 6, 2.8, "5a2430"), r.BrowInL, r.BrowOutL, 2);
        // Crow's feet, more when she laughs.
        double crinkle = 0.3 + (r.LiftR > 0.5 ? 0.5 : 0);
        foreach (var (y0, y1) in new[] { (356.0, 350.0), (362.0, 364.0), (368.0, 376.0) })
            kit.Crease(P(372, y0, 384, y1), crinkle, 1.4);

        // Nose: a soft bulb, turned to her right.
        s.SoftFill(PathBuilder.From(290, 370).C(286, 392, 276, 408, 274, 420).L(290, 424).C(292, 404, 296, 388, 298, 370), Shade, 0.7, 4, face);
        s.Stroke(PathBuilder.From(292, 372).C(288, 394, 276, 408, 270, 418), 1.8, Deep, 0.7, taper: true);
        s.Stroke(PathBuilder.From(270, 420).C(268, 432, 280, 438, 292, 434).C(300, 438, 310, 436, 314, 428), 2.4, Ink, taper: true);
        s.Fill(Shapes.Ellipse(284, 431, 4, 2), Rgb.Hex("6a2e2a"));
        s.SoftFill(Shapes.Ellipse(284, 418, 9, 6), Rgb.Hex("e88378"), 0.35, 4, face);

        // Folds from nose to mouth, deepened by a laugh.
        double fold = 0.45 + Math.Max(0, (r.CornerL + r.CornerR) * 0.025);
        kit.Crease(PathBuilder.From(318, 432).C(332, 446, 338, 462, 334 + r.Stretch * 0.3, 484), fold, 2.2);
        kit.Crease(PathBuilder.From(264, 436).C(254, 450, 254, 464, 258 - r.Stretch * 0.3, 480), fold * 0.8, 2);

        // Mouth: lipstick a little brighter than it should be, the near corner sits higher.
        kit.Mouth(new MouthSpec(new Pt(296, 474), 33, 7, 10, "a92a3e", "cf3e4e", Asym: 1.5, Sag: 1), r, face);

        // Fringe: small curls swept to her right, grey at the roots.
        var fringe = Lobes(P(214, 300, 216, 270, 234, 248, 262, 236, 296, 230, 336, 232, 372, 244, 400, 266, 412, 300,
                             392, 284, 368, 290, 346, 276, 320, 286, 296, 270, 272, 282, 250, 272, 230, 292), new Pt(310, 250), 0.28);
        s.SoftFill(Shift(fringe, -6, 10), Shade, 0.9, 3, face);
        s.Fill(fringe, Rgb.Hex("8e2f45"));
        s.Fill(fringe, Rgb.Hex("5e1a30"), alphaAt: (x, y) => Shapes.Inside(fringe, x - 10, y - 8) ? 0 : 1);
        s.SoftFill(Shapes.Ellipse(352, 246, 22, 7), Rgb.Hex("bd5568"), 0.7, 3, fringe);
        s.Stroke(Close(fringe), 2.4, Ink);
        foreach (double x in new[] { 298.0, 306.0, 314.0 })
            s.Stroke(P(x, 178, x + 2, 190), 3, Rgb.Hex("b6acae"), 0.85, taper: true);

        // Gold hoops.
        s.Stroke(Close(Shapes.Ellipse(410, 488, 15, 19)), 4.4, Ink);
        s.Stroke(Close(Shapes.Ellipse(410, 488, 15, 19)), 2.6, Rgb.Hex("d9aa48"));
    }
}
