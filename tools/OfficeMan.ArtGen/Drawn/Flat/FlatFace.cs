using OfficeMan.ArtGen.Portraits;
using OfficeMan.ArtGen.Raster;

namespace OfficeMan.ArtGen.Drawn.Flat;

public enum FlatHair { SweptBack, Bun, Combover, BaldFringe, Curls }
public enum FlatClothes { SuitOpenCollar, SuitBlouse, ShirtTie, Vest, Scoop, Sweetheart }
public enum FlatGlasses { None, Round, Half }

/// <summary>
/// One cast member described for the flat illustration style of art/reference/drawn_flat.png.
/// All faces share the reference's page (600×760), its proportions (eyes at y 352, mouth near 468)
/// and its light (upper left), so they sit together as one set.
/// </summary>
public sealed record FlatSpec
{
    public required string Name { get; init; }

    // Skin
    public string Skin { get; init; } = "f2c6a8";
    public string SkinShade { get; init; } = "d8957e";
    public string SkinDeep { get; init; } = "bd7766";
    public string SkinLight { get; init; } = "fbdcc6";
    public string Blush { get; init; } = "ea8c86";
    public double BlushAmount { get; init; } = 0.3;

    // Head shape (half-widths from the centre line)
    public double Crown { get; init; } = 176;
    public double Temple { get; init; } = 118;
    public double Cheek { get; init; } = 120;
    public double JawY { get; init; } = 492;
    public double Jaw { get; init; } = 76;
    public double ChinY { get; init; } = 526;
    public double Chin { get; init; } = 26;
    public double Neck { get; init; } = 58;
    public double Shoulders { get; init; } = 270;

    /// <summary>0 young … 1 old: forehead lines, crow's feet, folds, neck.</summary>
    public double Age { get; init; }

    // Eyes
    public double EyeSpacing { get; init; } = 46;
    public double EyeWidth { get; init; } = 25;
    public double EyeY { get; init; } = 352;
    public string Iris { get; init; } = "6b4a2e";
    public double Hood { get; init; } = 0.5;
    public bool Bags { get; init; } = true;
    public string? EyeShadow { get; init; }

    // Brows
    public string BrowColor { get; init; } = "4a3028";
    public double BrowWidth { get; init; } = 5;
    /// <summary>Resting angle: positive tilts the inner ends down (stern), negative up (worried).</summary>
    public double BrowTilt { get; init; }

    // Nose and mouth
    public double NoseLength { get; init; } = 1;
    public double NoseWidth { get; init; } = 1;
    public double MouthY { get; init; } = 470;
    public double MouthWidth { get; init; } = 30;
    public string LipUpper { get; init; } = "b86a5e";
    public string LipLower { get; init; } = "cc7e70";
    public double LipFullness { get; init; } = 0.6;

    // Hair
    public FlatHair Hair { get; init; }
    public string HairColor { get; init; } = "3a2a24";
    public string HairShade { get; init; } = "241814";
    public string HairLight { get; init; } = "5a463c";
    public string? Grey { get; init; }

    public string? Moustache { get; init; }
    public double Stubble { get; init; }

    public FlatGlasses Glasses { get; init; }
    public string GlassesColor { get; init; } = "3a2e28";

    // Clothes
    public FlatClothes Clothes { get; init; }
    public string Cloth { get; init; } = "2c3546";
    public string ClothShade { get; init; } = "1c2230";
    public string Shirt { get; init; } = "ece9e4";
    public string ShirtShade { get; init; } = "c8c2b8";
    public string Tie { get; init; } = "8a6a3a";
    public string? Brooch { get; init; }

    public bool Cigarette { get; init; }
    public bool Earrings { get; init; }
    public bool Necklace { get; init; }
    public bool DoubleChin { get; init; }
    public bool ChestHair { get; init; }
}

/// <summary>
/// Draws a <see cref="FlatSpec"/> with an <see cref="Expression"/> in the flat illustration style:
/// two tones per colour (the side away from the light in the darker one), soft blush, a thin warm
/// line round the face, the evening sky behind, fine grain. Rules taken from DrawnStyles.Flat.
/// </summary>
public static class FlatFace
{
    public const int W = 600, H = 760;
    private const double Cx = 300;

    private static Rgb Hex(string h) => Rgb.Hex(h);
    private static List<Pt> Closed(List<Pt> pts) => pts.Append(pts[0]).ToList();
    private static List<Pt> Mirror(IEnumerable<Pt> pts) => pts.Select(p => new Pt(2 * Cx - p.X, p.Y)).ToList();
    private static List<Pt> Offset(IEnumerable<Pt> pts, double dx, double dy) => pts.Select(p => new Pt(p.X + dx, p.Y + dy)).ToList();
    /// <summary>
    /// The side of a shape turned away from the light. The reference places its shadows by hand on the
    /// right; nudging mostly sideways (a little down) gives the same right-hand crescents without a band
    /// along the bottom of every shape.
    /// </summary>
    private static Func<double, double, bool> Away(IReadOnlyList<Pt> shape, double reach) =>
        (x, y) => !Shapes.Inside(shape, x + reach, y + reach * 0.22);

    /// <summary>The same evening sky as the reference: warm at the top, cooling to teal.</summary>
    public static void Sky(Sketch s)
    {
        s.Clear((x, y) =>
        {
            double t = y / H;
            var c = Hex("f4d3ad").Mix(Hex("8db0aa"), Math.Clamp(t * 1.15 - 0.1, 0, 1));
            double glow = Math.Max(0, 1 - Math.Sqrt(Math.Pow((x - 230) / 330.0, 2) + Math.Pow((y - 260) / 330.0, 2)));
            return c.Mix(Hex("fbe7cc"), glow * 0.5);
        });
    }

    private static void TwoTone(Sketch s, List<Pt> shape, string baseHex, string shadeHex, double reach, IReadOnlyList<Pt>? clip = null)
    {
        s.Fill(shape, Hex(baseHex), clip: clip);
        var away = Away(shape, reach);
        s.Fill(shape, Hex(shadeHex), clip: clip, alphaAt: (x, y) => away(x, y) ? 1 : 0);
    }

    public static Canvas Draw(FlatSpec p, Expression e)
    {
        var s = new Sketch(W, H);
        Sky(s);

        var face = FaceShape(p);
        var body = BodyShape(p);

        if (p.Hair == FlatHair.Bun) DrawBun(s, p);
        if (p.Hair == FlatHair.Curls) DrawCurlsBehind(s, p);
        DrawBody(s, p, body);
        Ears(s, p);
        DrawFace(s, p, face, e);
        DrawHair(s, p, face);
        DrawEyes(s, p, e, face);
        DrawBrows(s, p, e);
        DrawNose(s, p);
        DrawMouth(s, p, e, face);
        if (p.Moustache is not null) DrawMoustache(s, p, e);
        if (p.Glasses != FlatGlasses.None) DrawGlasses(s, p);
        if (p.Cigarette) DrawCigarette(s, p, e);
        if (p.Earrings)
            foreach (int side in new[] { -1, 1 })
            {
                var hoop = Shapes.Ellipse(Cx + side * (p.Temple + 4), 448, 14, 17);
                s.Stroke(Closed(hoop), 3.2, Hex("d1a54a"));
                s.Stroke(hoop.Skip(28).Take(10).ToList(), 1.4, Hex("fff0c0"));
            }

        // A thin warm line to hold the face together, as in the reference.
        s.Stroke(Closed(face), 1.4, Hex("8a4a40"), 0.35);
        return s.ToCanvas(grain: 0.025);
    }

    // ── Shapes ─────────────────────────────────────────────

    private static List<Pt> FaceShape(FlatSpec p)
    {
        double t = p.Temple, c = p.Cheek, top = p.Crown;
        var right = PathBuilder.From(Cx, top)
            .C(Cx + t * 0.6, top, Cx + t, top + 50, Cx + t, 320)
            .C(Cx + c + 2, 370, Cx + c, 420, Cx + p.Jaw + (c - p.Jaw) * 0.35, p.JawY - 22)
            .C(Cx + p.Jaw, p.JawY, Cx + p.Chin + 14, p.ChinY, Cx + p.Chin, p.ChinY)
            .L(Cx, p.ChinY + 1).Points;
        var left = Mirror(Enumerable.Reverse(right).Skip(1));
        return right.Concat(left).ToList();
    }

    private static List<Pt> BodyShape(FlatSpec p)
    {
        double n = p.Neck, sh = p.Shoulders;
        return PathBuilder.From(Cx - n, p.JawY - 30)
            .C(Cx - n, p.ChinY + 20, Cx - n - 6, p.ChinY + 40, Cx - n - 30, p.ChinY + 52)
            .C(Cx - sh * 0.6, 600, Cx - sh, 620, Cx - sh - 20, 680)
            .L(Cx - sh - 40, 760).L(Cx + sh + 40, 760).L(Cx + sh + 20, 680)
            .C(Cx + sh, 620, Cx + sh * 0.6, 600, Cx + n + 30, p.ChinY + 52)
            .C(Cx + n + 6, p.ChinY + 40, Cx + n, p.ChinY + 20, Cx + n, p.JawY - 30);
    }

    // ── Body and clothes ───────────────────────────────────

    private static void DrawBody(Sketch s, FlatSpec p, List<Pt> body)
    {
        s.Fill(body, Hex(p.Skin));
        // The head shades the neck.
        var neckShadow = PathBuilder.From(Cx - p.Neck, p.JawY - 10).C(Cx - 20, p.ChinY + 40, Cx + 20, p.ChinY + 40, Cx + p.Neck, p.JawY - 10)
            .L(Cx + p.Neck + 4, p.ChinY + 50).C(Cx + 30, p.ChinY + 66, Cx - 30, p.ChinY + 66, Cx - p.Neck - 4, p.ChinY + 50).Points;
        s.Fill(neckShadow, Hex(p.SkinShade), clip: body);
        s.Fill(Shapes.Ellipse(Cx + p.Shoulders * 0.75, 650, 120, 60), Hex(p.SkinShade), 0.6, clip: body);
        if (p.Age > 0.4)
            foreach (double dy in new[] { 18.0, 34.0 })
                s.Stroke(PathBuilder.From(Cx - p.Neck + 14, p.ChinY + dy).C(Cx - 20, p.ChinY + dy + 6, Cx + 20, p.ChinY + dy + 6, Cx + p.Neck - 14, p.ChinY + dy), 1.4, Hex(p.SkinDeep), 0.35 * p.Age, taper: true);

        double sh = p.Shoulders, top = p.ChinY + 44;
        switch (p.Clothes)
        {
            case FlatClothes.Vest:
            {
                // A white tank top: two straps running up over the shoulders, deep armholes, a scoop neck.
                double sy = p.ChinY + 50;                                   // where the shoulders meet the neck
                var vest = PathBuilder.From(Cx - sh * 0.66, 760).C(Cx - sh * 0.64, 720, Cx - sh * 0.6, 690, Cx - sh * 0.5, 676)   // side, up to the armhole
                    .C(Cx - sh * 0.46, 640, Cx - p.Neck - 46, sy + 14, Cx - p.Neck - 40, sy - 4)                                  // armhole, up the strap
                    .L(Cx - p.Neck - 6, sy - 6)                                                                                    // over the shoulder
                    .C(Cx - p.Neck + 6, sy + 40, Cx - 50, top + 70, Cx, top + 74)                                                   // scoop neck
                    .C(Cx + 50, top + 70, Cx + p.Neck - 6, sy + 40, Cx + p.Neck + 6, sy - 6)
                    .L(Cx + p.Neck + 40, sy - 4)
                    .C(Cx + p.Neck + 46, sy + 14, Cx + sh * 0.46, 640, Cx + sh * 0.5, 676)
                    .C(Cx + sh * 0.6, 690, Cx + sh * 0.64, 720, Cx + sh * 0.66, 760).Points;
                if (p.ChestHair)
                {
                    var rng = new Random(15);
                    for (int i = 0; i < 70; i++)
                    {
                        double x = Cx - 120 + rng.NextDouble() * 240, y = top + 24 + rng.NextDouble() * 80;
                        s.Stroke(PathBuilder.From(x, y).C(x + 2, y - 2, x + 4, y + 1, x + 3, y + 4), 1.4, Hex("6a5a50"), 0.6, taper: true, clip: body);
                    }
                }
                TwoTone(s, vest, p.Cloth, p.ClothShade, 30);
                s.Stroke(PathBuilder.From(Cx - p.Neck - 6, sy - 6).C(Cx - p.Neck + 6, sy + 40, Cx - 50, top + 70, Cx, top + 74)
                    .C(Cx + 50, top + 70, Cx + p.Neck - 6, sy + 40, Cx + p.Neck + 6, sy - 6), 2, Hex(p.ClothShade), 0.9);
                // The rib of the fabric, and the strap's edge over the shoulder.
                for (int i = 0; i < 12; i++)
                {
                    double x = Cx - sh * 0.5 + i * sh / 11.0;
                    s.Stroke(new List<Pt> { new(x, top + 90), new(x * 0.98 + Cx * 0.02, 760) }, 1, Hex(p.ClothShade), 0.25, clip: vest);
                }
                break;
            }
            case FlatClothes.Scoop:
            {
                var scoop = PathBuilder.From(Cx - sh - 40, 760).L(Cx - sh - 20, 680).C(Cx - sh + 10, 640, Cx - sh * 0.7, 618, Cx - 120, 612)
                    .C(Cx - 90, 676, Cx - 40, 700, Cx, 702).C(Cx + 40, 700, Cx + 90, 676, Cx + 120, 612)
                    .C(Cx + sh * 0.7, 618, Cx + sh - 10, 640, Cx + sh + 20, 680).L(Cx + sh + 40, 760).Points;
                TwoTone(s, scoop, p.Cloth, p.ClothShade, 60);
                s.Stroke(PathBuilder.From(Cx - 120, 612).C(Cx - 90, 676, Cx - 40, 700, Cx, 702).C(Cx + 40, 700, Cx + 90, 676, Cx + 120, 612), 2, Hex(p.ClothShade), 0.8);
                s.Stroke(PathBuilder.From(Cx, 704).C(Cx + 1, 716, Cx, 726, Cx - 2, 736), 1.8, Hex(p.SkinShade), taper: true);
                if (p.Necklace)
                {
                    s.Stroke(PathBuilder.From(Cx - 40, p.ChinY + 30).C(Cx - 26, p.ChinY + 92, Cx + 26, p.ChinY + 92, Cx + 40, p.ChinY + 30), 1.6, Hex("d1a54a"));
                    s.Fill(Shapes.Ellipse(Cx, p.ChinY + 90, 6, 7), Hex("d1a54a"));
                    s.Fill(Shapes.Ellipse(Cx - 1.5, p.ChinY + 88, 1.6, 1.6), Hex("fff2c8"));
                }
                break;
            }
            case FlatClothes.Sweetheart:
            {
                // Bare shoulders; the dress curves over each side of the bust and dips between.
                const double n = 652;   // where the neckline meets the shoulders
                s.SoftFill(Shapes.Ellipse(Cx - 70, 704, 76, 44), Hex(p.SkinShade), 0.35, 16, body);
                s.SoftFill(Shapes.Ellipse(Cx + 70, 704, 76, 44), Hex(p.SkinShade), 0.5, 16, body);
                s.Stroke(PathBuilder.From(Cx - 1, n - 6).C(Cx - 4, n + 14, Cx + 2, n + 30, Cx, n + 48), 2.4, Hex(p.SkinDeep), 0.85, taper: true);
                s.Stroke(PathBuilder.From(Cx - 112, n - 2).C(Cx - 82, n - 18, Cx - 40, n - 16, Cx - 12, n + 4), 1.6, Hex(p.SkinShade), 0.7, taper: true);
                s.Stroke(PathBuilder.From(Cx + 112, n - 2).C(Cx + 82, n - 18, Cx + 40, n - 16, Cx + 12, n + 4), 1.6, Hex(p.SkinShade), 0.7, taper: true);
                var dress = PathBuilder.From(Cx - sh - 40, 760).L(Cx - sh - 10, n + 6).C(Cx - sh + 30, n - 6, Cx - 150, n - 4, Cx - 128, n + 4)
                    .C(Cx - 100, n - 8, Cx - 36, n - 2, Cx, n + 50).C(Cx + 36, n - 2, Cx + 100, n - 8, Cx + 128, n + 4)
                    .C(Cx + 150, n - 4, Cx + sh - 30, n - 6, Cx + sh + 10, n + 6).L(Cx + sh + 40, 760).Points;
                TwoTone(s, dress, p.Cloth, p.ClothShade, 60);
                // The fabric rounds over the bust: a light on the upper curve, a fold under it.
                s.SoftFill(Shapes.Ellipse(Cx - 74, n + 22, 46, 18), Hex("e8505c"), 0.55, 14, dress);
                s.SoftFill(Shapes.Ellipse(Cx + 66, n + 24, 40, 16), Hex("e8505c"), 0.3, 14, dress);
                s.Stroke(PathBuilder.From(Cx - 130, n + 78).C(Cx - 100, n + 92, Cx - 40, n + 92, Cx - 8, n + 70), 2, Hex(p.ClothShade), 0.7, taper: true);
                s.Stroke(PathBuilder.From(Cx + 130, n + 78).C(Cx + 100, n + 92, Cx + 40, n + 92, Cx + 8, n + 70), 2, Hex(p.ClothShade), 0.7, taper: true);
                s.Stroke(PathBuilder.From(Cx - 128, n + 4).C(Cx - 100, n - 8, Cx - 36, n - 2, Cx, n + 50).C(Cx + 36, n - 2, Cx + 100, n - 8, Cx + 128, n + 4), 2, Hex(p.ClothShade), 0.9);
                if (p.Necklace)
                {
                    s.Stroke(PathBuilder.From(Cx - 40, p.ChinY + 30).C(Cx - 26, p.ChinY + 96, Cx + 26, p.ChinY + 96, Cx + 40, p.ChinY + 30), 1.6, Hex("d1a54a"));
                    s.Fill(Shapes.Ellipse(Cx, p.ChinY + 94, 6, 7), Hex("d1a54a"));
                    s.Fill(Shapes.Ellipse(Cx - 1.5, p.ChinY + 92, 1.6, 1.6), Hex("fff2c8"));
                }
                break;
            }
            case FlatClothes.ShirtTie:
            {
                var shirt = PathBuilder.From(Cx - sh - 40, 760).L(Cx - sh - 20, 680).C(Cx - sh, 626, Cx - sh * 0.6, 606, Cx - p.Neck - 26, top - 4)
                    .L(Cx + p.Neck + 26, top - 4).C(Cx + sh * 0.6, 606, Cx + sh, 626, Cx + sh + 20, 680).L(Cx + sh + 40, 760).Points;
                TwoTone(s, shirt, p.Shirt, p.ShirtShade, 40);
                // Collar points and the tie, knotted a little too tight.
                var collarL = PathBuilder.From(Cx - p.Neck - 8, top - 22).L(Cx - 6, top + 8).L(Cx - 34, top + 36).L(Cx - p.Neck - 18, top + 6).Points;
                TwoTone(s, collarL, p.Shirt, p.ShirtShade, 6);
                TwoTone(s, Mirror(collarL), p.Shirt, p.ShirtShade, 6);
                s.Stroke(Closed(collarL), 1.4, Hex(p.ShirtShade), 0.9);
                s.Stroke(Closed(Mirror(collarL)), 1.4, Hex(p.ShirtShade), 0.9);
                var knot = PathBuilder.From(Cx - 13, top + 4).L(Cx + 13, top + 4).L(Cx + 9, top + 24).L(Cx - 9, top + 24).Points;
                var blade = PathBuilder.From(Cx - 9, top + 24).L(Cx + 9, top + 24).L(Cx + 20, 740).L(Cx, 760).L(Cx - 20, 740).Points;
                TwoTone(s, blade, p.Tie, "5e4422", 8);
                TwoTone(s, knot, p.Tie, "5e4422", 5);
                // A pen in the breast pocket.
                var pocket = PathBuilder.From(Cx + 104, 662).L(Cx + 168, 662).L(Cx + 168, 716).C(Cx + 150, 724, Cx + 122, 724, Cx + 104, 716).Points;
                s.Fill(PathBuilder.From(Cx + 124, 640).L(Cx + 131, 640).L(Cx + 131, 690).L(Cx + 124, 690).Points, Hex("2a3a7a"));
                s.Fill(PathBuilder.From(Cx + 122, 640).L(Cx + 133, 640).L(Cx + 133, 645).L(Cx + 122, 645).Points, Hex("c8c8d0"));
                s.Fill(pocket, Hex(p.Shirt));
                s.Stroke(Closed(pocket), 1.6, Hex(p.ShirtShade));
                break;
            }
            case FlatClothes.SuitOpenCollar:
            case FlatClothes.SuitBlouse:
            {
                bool blouse = p.Clothes == FlatClothes.SuitBlouse;
                // The shirt shows only below the collarbone: its edge follows the neck down into the jacket.
                var under = PathBuilder.From(Cx - p.Neck - 10, top + 14).C(Cx - 30, top + 4, Cx + 30, top + 4, Cx + p.Neck + 10, top + 14)
                    .L(Cx + 46, 760).L(Cx - 46, 760).Points;
                TwoTone(s, under, p.Shirt, p.ShirtShade, 14);
                if (!blouse)
                {
                    // Open collar: skin in a V, the shirt's points lying on the jacket.
                    var v = PathBuilder.From(Cx - 30, top - 4).L(Cx + 30, top - 4).L(Cx, top + 70).Points;
                    s.Fill(v, Hex(p.Skin));
                    s.Fill(v, Hex(p.SkinShade), 0.5);
                }
                else
                {
                    // High blouse collar, buttoned.
                    var collar = PathBuilder.From(Cx - p.Neck - 4, top - 30).C(Cx - 20, top - 16, Cx + 20, top - 16, Cx + p.Neck + 4, top - 30)
                        .L(Cx + p.Neck + 8, top + 4).C(Cx + 20, top + 14, Cx - 20, top + 14, Cx - p.Neck - 8, top + 4).Points;
                    TwoTone(s, collar, p.Shirt, p.ShirtShade, 8);
                }
                var lapelL = PathBuilder.From(Cx - sh - 40, 760).L(Cx - sh - 20, 680).C(Cx - sh, 626, Cx - sh * 0.6, 606, Cx - p.Neck - 28, top - 8)
                    .L(Cx - 34, top + 40).L(Cx - 52, top + 70).L(Cx - 30, 700).L(Cx - 40, 760).Points;
                TwoTone(s, lapelL, p.Cloth, p.ClothShade, 36);
                TwoTone(s, Mirror(lapelL), p.Cloth, p.ClothShade, 36);
                foreach (var side in new[] { lapelL, Mirror(lapelL) })
                    s.Stroke(side.Skip(3).Take(4).ToList(), 2, Hex(p.ClothShade), 0.9);
                if (!blouse)
                {
                    var pointL = PathBuilder.From(Cx - 30, top - 4).L(Cx - 64, top + 40).L(Cx - 50, top + 6).Points;
                    s.Fill(pointL, Hex(p.Shirt));
                    s.Fill(Mirror(pointL), Hex(p.Shirt));
                }
                if (p.Brooch is not null)
                {
                    s.Fill(Shapes.Ellipse(Cx - 104, top + 86, 9, 9), Hex(p.Brooch));
                    s.Fill(Shapes.Ellipse(Cx - 106, top + 83, 3, 3), Hex("f4ecd0"));
                }
                break;
            }
        }
    }

    private static void Ears(Sketch s, FlatSpec p)
    {
        if (p.Hair == FlatHair.Curls) return;   // under the hair
        foreach (int side in new[] { -1, 1 })
        {
            double x = Cx + side * (p.Temple + 2);
            var ear = PathBuilder.From(x - side * 4, 342).C(x + side * 22, 330, x + side * 26, 384, x + side * 14, 414)
                .C(x + side * 8, 426, x - side * 2, 424, x - side * 6, 410).Points;
            s.Fill(ear, Hex(p.Skin));
            if (side > 0) s.Fill(ear, Hex(p.SkinShade), 0.85);
            s.Stroke(PathBuilder.From(x + side * 6, 352).C(x + side * 16, 352, x + side * 16, 384, x + side * 8, 398), 2, Hex(p.SkinDeep), 0.7, taper: true);
        }
    }

    // ── Face ───────────────────────────────────────────────

    private static void DrawFace(Sketch s, FlatSpec p, List<Pt> face, Expression e)
    {
        s.Fill(face, Hex(p.Skin));
        foreach (int side in new[] { -1, 1 })
            s.Fill(Shapes.Ellipse(Cx + side * p.EyeSpacing, p.EyeY - 8, 38, 19), Hex(p.SkinShade), 0.35, clip: face);
        var away = Away(face, 26);
        s.Fill(face, Hex(p.SkinShade), clip: face, alphaAt: (x, y) => away(x, y) ? 1 : 0);
        // The nose's shadow falls to the right.
        double nl = 60 * p.NoseLength, nw = p.NoseWidth;
        var noseShadow = PathBuilder.From(Cx + 10, p.EyeY + 10).C(Cx + 16, p.EyeY + 30, Cx + 22 * nw, p.EyeY + nl - 4, Cx + 26 * nw, p.EyeY + nl + 8)
            .C(Cx + 18, p.EyeY + nl + 18, Cx + 10, p.EyeY + nl + 20, Cx + 4, p.EyeY + nl + 16).C(Cx + 12, p.EyeY + nl - 10, Cx + 10, p.EyeY + 30, Cx + 6, p.EyeY + 12).Points;
        s.Fill(noseShadow, Hex(p.SkinShade), clip: face);
        s.Fill(Shapes.Ellipse(Cx - 64, p.EyeY + 44, 24, 11), Hex(p.SkinLight), 0.9, clip: face);
        double blush = p.BlushAmount + e.Blush * 0.4;
        foreach (int side in new[] { -1, 1 })
        {
            var b = new Pt(Cx + side * 74, p.EyeY + 82);
            s.Fill(Shapes.Ellipse(b.X, b.Y, 38, 22), Hex(p.Blush), blush, clip: face,
                alphaAt: (x, y) => Math.Max(0, 1 - Math.Sqrt(Math.Pow((x - b.X) / 38, 2) + Math.Pow((y - b.Y) / 22, 2))));
        }
        if (p.Stubble > 0)
        {
            var jaw = PathBuilder.From(Cx - p.Cheek + 6, p.MouthY - 30).C(Cx - p.Cheek + 10, p.JawY, Cx - 40, p.ChinY, Cx, p.ChinY)
                .C(Cx + 40, p.ChinY, Cx + p.Cheek - 10, p.JawY, Cx + p.Cheek - 6, p.MouthY - 30)
                .C(Cx + 60, p.MouthY - 40, Cx - 60, p.MouthY - 40, Cx - p.Cheek + 6, p.MouthY - 30).Points;
            // Stubble: a soft-edged shadow over the jaw, not a patch.
            s.SoftFill(jaw, Hex("6a5a50"), p.Stubble * 0.3, 14, face);
        }

        // Age: forehead lines, crow's feet, the folds from nose to mouth, a softer jaw.
        double a = p.Age;
        if (a > 0)
        {
            var deep = Hex(p.SkinDeep);
            double lift = e.BrowRaise * 3;
            for (int i = 0; i < 3; i++)
            {
                double y = p.EyeY - 72 - i * 13 - lift;
                s.Stroke(PathBuilder.From(Cx - 54 + i * 6, y + 3).C(Cx - 20, y - 3, Cx + 20, y - 3, Cx + 54 - i * 6, y + 3), 1.5, deep, 0.45 * a, taper: true, clip: face);
            }
            foreach (int side in new[] { -1, 1 })
            {
                double x = Cx + side * (p.EyeSpacing + p.EyeWidth + 8);
                for (int i = -1; i <= 1; i++)
                    s.Stroke(PathBuilder.From(x, p.EyeY + i * 6).C(x + side * 6, p.EyeY + i * 8, x + side * 10, p.EyeY + i * 10, x + side * 14, p.EyeY + i * 13), 1.3, deep, 0.5 * a, taper: true, clip: face);
            }
        }
        if (p.DoubleChin)
            s.Stroke(PathBuilder.From(Cx - 50, p.ChinY - 6).C(Cx - 28, p.ChinY + 24, Cx + 28, p.ChinY + 24, Cx + 50, p.ChinY - 6), 2.2, Hex(p.SkinDeep), 0.7, taper: true);
        double fold = a * 0.85 + Math.Max(0, e.Smile) * (0.15 + a * 0.2);   // young faces only crease when they smile
        foreach (int side in new[] { -1, 1 })
            s.Stroke(PathBuilder.From(Cx + side * 26 * p.NoseWidth, p.EyeY + 60 * p.NoseLength + 8)
                .C(Cx + side * (44 + e.Smile * 6), p.MouthY - 26, Cx + side * (p.MouthWidth + 18 + e.Smile * 6), p.MouthY - 4, Cx + side * (p.MouthWidth + 16), p.MouthY + 16),
                1.8, Hex(p.SkinDeep), fold * 0.6, taper: true, clip: face);
    }

    private static void DrawEyes(Sketch s, FlatSpec p, Expression e, List<Pt> face)
    {
        double open = (0.45 + 0.55 * e.LidOpen) * (1 - e.Squint * 0.35);
        double gx = e.LookX * 5, gy = e.LookY * 4;
        double w = p.EyeWidth;
        foreach (int side in new[] { -1, 1 })
        {
            double cx = Cx + side * p.EyeSpacing, cy = p.EyeY;
            if (e.LidOpen < 0.2)
            {
                // Laughing: the eyes are just two happy arcs, with the cheeks pushed up under them.
                s.Stroke(PathBuilder.From(cx - w, cy + 2).C(cx - 10, cy - 12, cx + 10, cy - 12, cx + w, cy + 2), 3.4, Hex("2a1c18"), taper: true);
                s.Stroke(PathBuilder.From(cx - w + 6, cy + 12).C(cx - 6, cy + 16, cx + 6, cy + 16, cx + w - 6, cy + 11), 1.4, Hex(p.SkinDeep), 0.7, taper: true);
                continue;
            }
            double lidH = 17 * open;
            List<Pt> white = PathBuilder.From(cx - w, cy).C(cx - 12, cy - lidH, cx + 11, cy - lidH - 1, cx + w, cy - 2)
                .C(cx + 11, cy + 10 - e.Squint * 4, cx - 11, cy + 10 - e.Squint * 4, cx - w, cy);
            List<Pt> lid = PathBuilder.From(cx - w - 2, cy + 0.5).C(cx - 12, cy - lidH, cx + 11, cy - lidH - 1, cx + w + 2, cy - 1.5);
            double creaseY = cy - 12 - (1 - p.Hood) * 14;
            List<Pt> crease = PathBuilder.From(cx - w + 4, creaseY + 2).C(cx - 9, creaseY - 10, cx + 9, creaseY - 11, cx + w - 2, creaseY + 3);
            if (p.EyeShadow is not null)
                s.Fill(crease.Concat(Enumerable.Reverse(lid)).ToList(), Hex(p.EyeShadow), 0.85);
            s.Fill(white, Hex("f2ebe2"));
            var iris = new Pt(cx + gx, cy + gy + 1);
            s.Fill(Shapes.Ellipse(iris.X, iris.Y, 9.5, 9.5), Hex(p.Iris), clip: white);
            s.Fill(Shapes.Ellipse(iris.X, iris.Y, 4.2, 4.2), Hex("231612"), clip: white);
            s.Fill(Shapes.Ellipse(iris.X - 3.2, iris.Y - 3.2, 2, 2), Hex("ffffff"), clip: white);
            // A heavy upper lid shades the top of the eye.
            s.Fill(Offset(lid, 0, 3), Hex(p.SkinShade), 0.35 * p.Hood, clip: white);
            s.Stroke(lid, 3.2, Hex("2a1c18"));
            s.Stroke(crease, 1.4, Hex(p.SkinDeep), 0.8, taper: true);
            s.Stroke(PathBuilder.From(cx - w + 6, cy + 3).C(cx - 10, cy + 11 - e.Squint * 4, cx + 10, cy + 11 - e.Squint * 4, cx + w - 5, cy + 1), 1.2, Hex(p.SkinDeep), 0.8, taper: true);
            if (p.Bags)
                s.Stroke(PathBuilder.From(cx - 16, cy + 20).C(cx - 6, cy + 25, cx + 6, cy + 25, cx + 16, cy + 19), 1.3, Hex(p.SkinDeep), 0.5 + p.Age * 0.3, taper: true);
        }
    }

    private static void DrawBrows(Sketch s, FlatSpec p, Expression e)
    {
        // Inner ends: up when sad or worried, down when angry; the resting tilt is part of the face.
        double inner = e.BrowInner * 12 - p.BrowTilt * 6, raise = e.BrowRaise * 5;
        foreach (int side in new[] { -1, 1 })
        {
            double outerX = Cx + side * (p.EyeSpacing + 36), innerX = Cx + side * 12;
            double y = p.EyeY - 34 - raise;
            var brow = PathBuilder.From(outerX, y + 8).C(outerX - side * 14, y - 4, innerX + side * 22, y - 8 - inner * 0.5, innerX, y - 3 - inner).Points;
            s.Stroke(brow, p.BrowWidth, Hex(p.BrowColor), taper: true);
        }
    }

    private static void DrawNose(Sketch s, FlatSpec p)
    {
        double y0 = p.EyeY + 10, len = 60 * p.NoseLength, nw = p.NoseWidth, tip = y0 + len;
        s.Stroke(PathBuilder.From(Cx - 2, y0 + 4).C(Cx - 1, y0 + len * 0.4, Cx, y0 + len * 0.7, Cx, tip - 6), 1.4, Hex(p.SkinShade), 0.6, taper: true);
        foreach (int side in new[] { -1, 1 })
            s.Stroke(PathBuilder.From(Cx + side * 18 * nw, tip - 2).C(Cx + side * 26 * nw, tip + 10, Cx + side * 18 * nw, tip + 22, Cx + side * 6, tip + 18), 2, Hex(p.SkinDeep), taper: true);
        foreach (int side in new[] { -1, 1 })
            s.Fill(Shapes.Ellipse(Cx + side * 9 * nw, tip + 15, 5 * nw, 2.4), Hex("8a4a44"));
        s.Fill(Shapes.Ellipse(Cx - 4, tip - 2, 4.5 * nw, 3.2), Hex(p.SkinLight));
    }

    private static void DrawMouth(Sketch s, FlatSpec p, Expression e, List<Pt> face)
    {
        double my = p.MouthY, hw = p.MouthWidth + Math.Max(0, e.Smile) * 6;
        double corner = -e.Smile * 9, gap = e.MouthOpen * 26;
        double lc = my + corner, rc = my + corner - e.Lopsided * 6;
        double full = p.LipFullness;
        double l = Cx - hw, r = Cx + hw;
        var upper = PathBuilder.From(l, lc).C(l + 14, my - 10 * full - 2, Cx - 8, my - 12 * full - 2, Cx, my - 8 * full)
            .C(Cx + 8, my - 12 * full - 2, r - 14, my - 10 * full - 2, r, rc).C(r - 14, my, l + 14, my, l, lc).Points;
        var gapShape = PathBuilder.From(l + 3, lc).C(l + 16, my + 1, r - 16, my + 1, r - 3, rc)
            .C(r - 14, my + gap, l + 14, my + gap, l + 3, lc).Points;
        var lower = PathBuilder.From(l, lc + 0.5).C(l + 16, my + gap + 1, r - 16, my + gap + 1, r, rc + 0.5)
            .C(r - 10, my + gap + 16 * full + 2, l + 10, my + gap + 16 * full + 2, l, lc + 0.5).Points;
        if (gap > 2)
        {
            s.Fill(gapShape, Hex("4a1820"));
            // Upper teeth when the mouth is open in a smile or a laugh; a hint of tongue below.
            if (e.Smile > 0.2)
                s.Fill(PathBuilder.From(l + 8, lc + 1).C(l + 20, my + 2, r - 20, my + 2, r - 8, rc + 1).L(r - 12, my + gap * 0.35).C(r - 24, my + gap * 0.45, l + 24, my + gap * 0.45, l + 12, my + gap * 0.35).Points,
                    Hex("f2ece0"), clip: gapShape);
            s.Fill(Shapes.Ellipse(Cx, my + gap - 2, hw * 0.5, gap * 0.25), Hex("a84a52"), 0.9, clip: gapShape);
        }
        s.Fill(lower, Hex(p.LipLower));
        s.Fill(upper, Hex(p.LipUpper));
        s.Stroke(PathBuilder.From(l, lc).C(l + 16, my + gap * 0.15, r - 16, my + gap * 0.15, r, rc), 1.8, Hex("6a2a24"));
        s.Fill(Shapes.Ellipse(Cx + 7, my + gap + 9 * full, 8, 2.6), Hex("f2b0a0"), 0.6);
        s.Stroke(PathBuilder.From(Cx - 14, my + gap + 22 * full + 6).C(Cx - 5, my + gap + 26 * full + 6, Cx + 5, my + gap + 26 * full + 6, Cx + 14, my + gap + 22 * full + 6), 2, Hex(p.SkinShade), taper: true);
        // Corners: a little dimple that deepens with the smile, a downturn line when displeased.
        foreach (var (x, y, side) in new[] { (l, lc, -1), (r, rc, 1) })
        {
            if (e.Smile > 0.1) s.Stroke(PathBuilder.From(x + side * 2, y - 6).C(x + side * 6, y - 3, x + side * 6, y + 2, x + side * 3, y + 5), 1.6, Hex(p.SkinDeep), 0.4 + e.Smile * 0.4, taper: true);
            if (e.Smile < -0.2) s.Stroke(PathBuilder.From(x, y + 2).C(x + side * 4, y + 8, x + side * 6, y + 14, x + side * 6, y + 20), 1.6, Hex(p.SkinDeep), 0.5, taper: true);
        }
    }

    private static void DrawMoustache(Sketch s, FlatSpec p, Expression e)
    {
        double my = p.MouthY - 4 + -e.Smile * 3, hw = p.MouthWidth + 10;
        var tash = PathBuilder.From(Cx - hw, my + 8).C(Cx - hw + 4, my - 20, Cx - 18, my - 26, Cx, my - 20)
            .C(Cx + 18, my - 26, Cx + hw - 4, my - 20, Cx + hw, my + 8).C(Cx + hw - 14, my + 2, Cx + 18, my - 2, Cx, my - 1)
            .C(Cx - 18, my - 2, Cx - hw + 14, my + 2, Cx - hw, my + 8).Points;
        TwoTone(s, tash, p.Moustache!, "4a3e36", 6);
        var rng = new Random(9);
        for (int i = 0; i < 28; i++)
        {
            double x = Cx - hw + 6 + i * (2 * hw - 12) / 28.0 + rng.NextDouble() * 2;
            s.Stroke(new List<Pt> { new(x, my - 18 + Math.Abs(x - Cx) * 0.12), new(x + (x < Cx ? -2 : 2), my + 2) }, 1.3,
                Hex(i % 3 == 0 ? "a49a90" : "5a4e44"), 0.7, taper: true, clip: tash);
        }
    }

    private static void DrawGlasses(Sketch s, FlatSpec p)
    {
        var frame = Hex(p.GlassesColor);
        foreach (int side in new[] { -1, 1 })
        {
            double cx = Cx + side * p.EyeSpacing, cy = p.EyeY + 2;
            List<Pt> lens = p.Glasses == FlatGlasses.Round
                ? Shapes.Ellipse(cx, cy, p.EyeWidth + 9, p.EyeWidth + 3)
                : PathBuilder.From(cx - p.EyeWidth - 10, cy - 4).L(cx + p.EyeWidth + 10, cy - 4)
                    .C(cx + p.EyeWidth + 10, cy + 18, cx + p.EyeWidth, cy + 22, cx, cy + 22).C(cx - p.EyeWidth, cy + 22, cx - p.EyeWidth - 10, cy + 18, cx - p.EyeWidth - 10, cy - 4).Points;
            s.Fill(lens, Hex("dfeaf0"), 0.12);
            s.Fill(Shapes.Ellipse(cx - 12, cy - 6, 8, 4), Hex("ffffff"), 0.35, clip: lens);
            if (p.Glasses == FlatGlasses.Half)
            {
                // Half-moon reading glasses: a heavier line along the top only.
                s.Stroke(lens.Take(2).ToList(), 3.4, frame);
                s.Stroke(lens.Skip(1).Append(lens[0]).ToList(), 1.8, frame, 0.8);
            }
            else s.Stroke(Closed(lens), 2.6, frame);
            double templeX = Cx + side * (p.EyeSpacing + p.EyeWidth + 10);
            s.Stroke(new List<Pt> { new(templeX, cy - 2), new(Cx + side * (p.Temple + 2), cy - 6) }, 2.2, frame);
        }
        s.Stroke(PathBuilder.From(Cx - p.EyeSpacing + p.EyeWidth + 8, p.EyeY).C(Cx - 6, p.EyeY - 6, Cx + 6, p.EyeY - 6, Cx + p.EyeSpacing - p.EyeWidth - 8, p.EyeY), 2.2, frame);
        // Zdenka's kind: a chain from the arms, down past the jaw.
        if (p.Glasses == FlatGlasses.Half)
            foreach (int side in new[] { -1, 1 })
                s.Stroke(PathBuilder.From(Cx + side * (p.Temple + 2), p.EyeY - 4).C(Cx + side * (p.Temple + 10), p.JawY, Cx + side * (p.Neck + 20), p.ChinY + 40, Cx + side * (p.Neck + 30), p.ChinY + 100),
                    1.2, frame, 0.8);
    }

    private static void DrawCigarette(Sketch s, FlatSpec p, Expression e)
    {
        double y = p.MouthY + 4 - e.Smile * 9 - e.Lopsided * 6, x = Cx + p.MouthWidth - 6;
        var cig = PathBuilder.From(x, y - 3).L(x + 58, y + 10).L(x + 56, y + 17).L(x - 2, y + 4).Points;
        s.Fill(cig, Hex("f2ece0"));
        s.Fill(PathBuilder.From(x, y - 3).L(x + 14, y).L(x + 12, y + 7).L(x - 2, y + 4).Points, Hex("d8a060"));
        s.Fill(PathBuilder.From(x + 52, y + 9).L(x + 58, y + 10).L(x + 56, y + 17).L(x + 50, y + 15).Points, Hex("e0602a"));
        for (int i = 0; i < 3; i++)
            s.Stroke(PathBuilder.From(x + 60, y + 6 - i * 34).C(x + 84, y - 14 - i * 34, x + 44, y - 34 - i * 34, x + 68, y - 54 - i * 34), 3, Hex("e8e4dc"), 0.4 - i * 0.1, taper: true);
    }

    // ── Hair ───────────────────────────────────────────────

    private static void DrawBun(Sketch s, FlatSpec p)
    {
        var bun = Shapes.Ellipse(Cx + 6, p.Crown - 16, 48, 40);
        TwoTone(s, bun, p.HairColor, p.HairShade, 14);
        for (int i = 0; i < 5; i++)
        {
            double a0 = i * 1.25;
            var coil = new List<Pt>();
            for (int k = 0; k <= 16; k++)
            {
                double a = a0 + k / 16.0 * 2.2, rr = 34 - i * 5;
                coil.Add(new Pt(Cx + 6 + Math.Cos(a) * rr, p.Crown - 16 + Math.Sin(a) * rr * 0.8));
            }
            s.Stroke(coil, 1.6, Hex(p.HairShade), 0.7, taper: true, clip: bun);
        }
    }

    /// <summary>Loose curls to the shoulders: the mass behind the head.</summary>
    private static void DrawCurlsBehind(Sketch s, FlatSpec p)
    {
        double top = p.Crown, t = p.Temple;
        var mass = PathBuilder.From(Cx - 20, top - 26).C(Cx + 90, top - 34, Cx + t + 56, top + 20, Cx + t + 54, 330)
            .C(Cx + t + 60, 430, Cx + t + 50, 520, Cx + t + 10, 590).C(Cx + 60, 600, Cx + 40, 580, Cx + 30, 560)
            .L(Cx - 30, 560).C(Cx - 40, 580, Cx - 60, 600, Cx - t - 10, 590)
            .C(Cx - t - 50, 520, Cx - t - 60, 430, Cx - t - 54, 330).C(Cx - t - 56, top + 20, Cx - 110, top - 30, Cx - 20, top - 26).Points;
        var curly = Shapes.Scallop(mass, new Pt(Cx, 360), 12, 30, seed: 21);
        TwoTone(s, curly, p.HairColor, p.HairShade, 24);
        CurlStrokes(s, p, curly);
    }

    /// <summary>C-shaped curl strokes on a jittered grid inside an area, lit on the upper left (as in the reference).</summary>
    private static void CurlStrokes(Sketch s, FlatSpec p, List<Pt> area)
    {
        var rng = new Random(52);
        for (double y = 110; y < 600; y += 24)
            for (double x = 110; x < 494; x += 24)
            {
                double px = x + rng.NextDouble() * 14 - 7, py = y + rng.NextDouble() * 14 - 7;
                if (!Shapes.Inside(area, px, py)) continue;
                double rr = 6 + rng.NextDouble() * 4, a0 = rng.NextDouble() * Math.PI * 2;
                var curl = new List<Pt>();
                for (int k = 0; k <= 14; k++)
                {
                    double a = a0 + k / 14.0 * Math.PI * 1.35;
                    curl.Add(new Pt(px + Math.Cos(a) * rr, py + Math.Sin(a) * rr * 0.9));
                }
                s.Stroke(curl, 2.2, Hex(p.HairShade), 0.85, taper: true);
                if (px < 330 && py < 420) s.Stroke(Offset(curl.Take(8), -2, -2), 2, Hex(p.HairLight), 0.8, taper: true);
            }
    }

    private static void DrawHair(Sketch s, FlatSpec p, List<Pt> face)
    {
        if (p.Hair == FlatHair.Curls)
        {
            // A side-swept fringe of curls from a parting on the left, falling past the right temple.
            double ct = p.Crown, tw = p.Temple;
            var fringe = PathBuilder.From(Cx - tw - 8, 300).C(Cx - tw - 6, ct + 10, Cx - 40, ct - 22, Cx + 30, ct - 18)
                .C(Cx + tw, ct - 6, Cx + tw + 14, 260, Cx + tw + 8, 340)
                .C(Cx + tw - 8, 300, Cx + 70, 262, Cx + 10, 262).C(Cx - 40, 262, Cx - 80, 240, Cx - tw + 10, 300)
                .C(Cx - tw, 320, Cx - tw - 6, 320, Cx - tw - 8, 300).Points;
            var curly = Shapes.Scallop(fringe, new Pt(Cx, 230), 9, 24, seed: 22);
            s.Fill(Offset(curly, 5, 10), Hex(p.SkinShade), 0.6, clip: face);
            TwoTone(s, curly, p.HairColor, p.HairShade, 10);
            CurlStrokes(s, p, curly);
            return;
        }

        double top = p.Crown, t = p.Temple;
        switch (p.Hair)
        {
            case FlatHair.SweptBack:
            {
                // Thick, combed back off a high forehead with a lift at the front, greying at the temples.
                var hair = PathBuilder.From(Cx - t - 4, 352).C(Cx - t - 14, 290, Cx - t - 10, top - 4, Cx - 40, top - 30)
                    .C(Cx + 10, top - 46, Cx + t + 4, top - 26, Cx + t + 10, top + 30).C(Cx + t + 12, 300, Cx + t + 8, 330, Cx + t + 4, 352)
                    .C(Cx + t - 4, 320, Cx + t - 10, 292, Cx + t - 26, 266)
                    .C(Cx + 60, 236, Cx + 20, 240, Cx, 248).C(Cx - 24, 238, Cx - 60, 236, Cx - t + 26, 264)
                    .C(Cx - t + 10, 290, Cx - t + 4, 320, Cx - t - 4, 352).Points;
                TwoTone(s, hair, p.HairColor, p.HairShade, 16);
                for (int i = 0; i < 16; i++)
                {
                    double x = Cx - t + 22 + i * (2 * t - 44) / 15.0, y0 = 250 + Math.Abs(x - Cx) * 0.18;
                    s.Stroke(PathBuilder.From(x, y0).C(x + 6, y0 - 30, x + 16 + (x - Cx) * 0.15, top - 4, x + 26 + (x - Cx) * 0.3, top - 30),
                        1.8, Hex(i % 3 == 0 ? p.HairLight : p.HairShade), 0.8, taper: true, clip: hair);
                }
                if (p.Grey is not null)
                    foreach (int side in new[] { -1, 1 })
                        s.Fill(PathBuilder.From(Cx + side * (t + 5), 352).C(Cx + side * (t + 12), 316, Cx + side * (t + 8), 290, Cx + side * (t - 10), 270)
                            .C(Cx + side * (t - 6), 296, Cx + side * (t - 2), 324, Cx + side * (t - 4), 350).Points, Hex(p.Grey), 0.9);
                break;
            }
            case FlatHair.Bun:
            {
                // Grey hair pulled back hard from a centre parting.
                var cap = PathBuilder.From(Cx - t - 4, 352).C(Cx - t - 10, 280, Cx - t + 6, top - 10, Cx, top - 14)
                    .C(Cx + t - 6, top - 10, Cx + t + 10, 280, Cx + t + 4, 352)
                    .C(Cx + t - 6, 320, Cx + t - 20, 280, Cx + 40, 262).C(Cx + 14, 256, Cx - 14, 256, Cx - 40, 262)
                    .C(Cx - t + 20, 280, Cx - t + 6, 320, Cx - t - 4, 352).Points;
                TwoTone(s, cap, p.HairColor, p.HairShade, 12);
                foreach (int side in new[] { -1, 1 })
                    for (int i = 0; i < 7; i++)
                    {
                        double y = 262 + i * 13;
                        s.Stroke(PathBuilder.From(Cx + side * 6, top - 12 + i * 2).C(Cx + side * 50, top + i * 6, Cx + side * (t - 10), y - 30, Cx + side * (t - 2 + i * 0.6), y + 30),
                            1.3, Hex(i % 2 == 0 ? p.HairShade : p.HairLight), 0.6, taper: true, clip: cap);
                    }
                s.Stroke(new List<Pt> { new(Cx, top - 14), new(Cx + 2, 258) }, 1.6, Hex(p.HairShade), 0.8);
                break;
            }
            case FlatHair.Combover:
            {
                // Short sides; across the top, a few long strands doing their best over a shiny scalp.
                foreach (int side in new[] { -1, 1 })
                {
                    var sideHair = PathBuilder.From(Cx + side * (t + 4), 366).C(Cx + side * (t + 10), 320, Cx + side * (t + 6), 270, Cx + side * (t - 20), 240)
                        .C(Cx + side * (t - 14), 270, Cx + side * (t - 4), 320, Cx + side * (t - 6), 362).Points;
                    TwoTone(s, sideHair, p.HairColor, p.HairShade, 8);
                }
                s.Fill(Shapes.Ellipse(Cx - 30, top + 36, 40, 16), Hex(p.SkinLight), 0.9, clip: face);
                for (int i = 0; i < 9; i++)
                {
                    double y = top + 22 + i * 7;
                    s.Stroke(PathBuilder.From(Cx - t + 2, y + 26).C(Cx - 40, y - 8, Cx + 40, y - 12, Cx + t - 20, y + 4 + i), 2.2 - i * 0.1,
                        Hex(i % 3 == 1 ? p.HairLight : p.HairColor), 0.85, taper: true, clip: face);
                }
                break;
            }
            case FlatHair.BaldFringe:
            {
                // A shining dome; short grey hair only round the back and over the ears, a bit wiry.
                s.Fill(Shapes.Ellipse(Cx - 34, top + 40, 46, 20), Hex(p.SkinLight), 0.95, clip: face);
                s.Fill(Shapes.Ellipse(Cx - 46, top + 34, 14, 6), Hex("fff0e0"), 0.8, clip: face);
                foreach (int side in new[] { -1, 1 })
                {
                    var tuft = PathBuilder.From(Cx + side * (t - 4), 382).C(Cx + side * (t + 12), 370, Cx + side * (t + 18), 320, Cx + side * (t + 8), 284)
                        .C(Cx + side * (t + 2), 266, Cx + side * (t - 10), 262, Cx + side * (t - 14), 270)
                        .C(Cx + side * (t - 4), 300, Cx + side * (t - 2), 340, Cx + side * (t - 8), 380).Points;
                    var wiry = Shapes.Scallop(tuft, new Pt(Cx + side * (t - 20), 330), 4, 12, seed: side > 0 ? 7 : 8);
                    TwoTone(s, wiry, p.HairColor, p.HairShade, 8);
                    for (int i = 0; i < 9; i++)
                    {
                        double y = 276 + i * 11;
                        s.Stroke(PathBuilder.From(Cx + side * (t - 8), y).C(Cx + side * (t + 2), y + 4, Cx + side * (t + 8), y + 10, Cx + side * (t + 12), y + 18),
                            1.3, Hex(i % 2 == 0 ? p.HairLight : p.HairShade), 0.7, taper: true, clip: wiry);
                    }
                }
                break;
            }
        }
    }
}
