using OfficeMan.ArtGen.Raster;

namespace OfficeMan.ArtGen.Sprites;

public enum HairStyle { Messy, Perm, SweptBack, Bun, Combover, Bald }
public enum TopStyle { Shirt, ShirtTie, Cardigan, Dress, SuitOpen, Suit, Vest }

/// <summary>A small walking person, described rather than drawn.</summary>
public sealed record PersonSpec(
    string Name,
    string Skin, string SkinShade,
    HairStyle Hair, string HairColor, string HairShade,
    TopStyle Top, string TopColor, string TopShade,
    string Bottom, string BottomShade, string Shoes,
    int Build = 6, int Height = 2, bool Glasses = false, bool Earrings = false, bool Belly = false,
    string? Accent = null, string? GreyAt = null, string GlassesColor = "3a2e28", string? Moustache = null);

/// <summary>
/// Pixel people for the walkable world: 28×52 frames, facing right (flip for left).
/// Frames: idle 0–1, walk 2–5. The sheet is one row.
/// </summary>
public static class PixelPeople
{
    public const int FrameW = 28, FrameH = 52, Frames = 6;
    private static readonly Rgb Outline = Rgb.Hex("231a1e");

    public static readonly PersonSpec Teo = new("teo", "e8b896", "c98e72", HairStyle.Messy, "3a2a24", "241814",
        TopStyle.Shirt, "b9cbdc", "8ea4ba", "4a4e58", "363a42", "2a2420", Build: 6, Height: 2, Belly: true);

    public static readonly PersonSpec Magda = new("magda", "f2c4a6", "d4977e", HairStyle.Perm, "a3304a", "6e1a33",
        TopStyle.Cardigan, "e48aa0", "b8607a", "3c3a4a", "2a2836", "5a2a2a", Build: 8, Height: 0, Earrings: true, Belly: true);

    public static readonly PersonSpec Zdenka = new("zdenka", "ecd3c4", "c4a196", HairStyle.Bun, "a6a2a8", "77737a",
        TopStyle.Suit, "6c6e72", "4a4c50", "6c6e72", "4a4c50", "2a2828", Build: 5, Height: 4, Glasses: true, Accent: "ece7dc", GlassesColor: "c9a65a");

    public static readonly PersonSpec Kornel = new("kornel", "f0c9b2", "d29b86", HairStyle.Combover, "5a4434", "3a2c22",
        TopStyle.ShirtTie, "b9cde0", "8ea6be", "6a5a48", "4a3e32", "3a2a20", Build: 5, Height: 5, Glasses: true, Accent: "8a6a3a");

    public static readonly PersonSpec Viktor = new("viktor", "c98f6c", "a66a50", HairStyle.SweptBack, "3c3432", "241e1c",
        TopStyle.SuitOpen, "2c3546", "1c2230", "2c3546", "1c2230", "1a1414", Build: 8, Height: 3, Accent: "ece9e4", GreyAt: "a7a3a0");

    /// <summary>The neighbour: big, bald with a grey fringe, a moustache, a white vest, sweatpants and slippers.</summary>
    public static readonly PersonSpec Stojan = new("stojan", "d9a07e", "b07658", HairStyle.Bald, "8a847c", "5e5a54",
        TopStyle.Vest, "e8e4d8", "b8b2a2", "5a5e66", "3e424a", "6a3a2a", Build: 9, Height: 3, Belly: true, Moustache: "4a3a30");

    public static IEnumerable<PersonSpec> All => new[] { Teo, Magda, Zdenka, Kornel, Viktor, Stojan };

    public static Canvas Sheet(PersonSpec p)
    {
        var sheet = new Canvas(FrameW * Frames, FrameH);
        for (int f = 0; f < Frames; f++) sheet.Draw(Frame(p, f), f * FrameW, 0);
        return sheet;
    }

    public static Canvas Frame(PersonSpec p, int frame)
    {
        var c = new Canvas(FrameW, FrameH);
        void Px(int x, int y, string hex) { if (c.InBounds(x, y)) c[x, y] = Rgb.Hex(hex); }
        void Rect(int x0, int y0, int x1, int y1, string hex)
        {
            for (int y = y0; y <= y1; y++)
                for (int x = x0; x <= x1; x++) Px(x, y, hex);
        }

        bool walking = frame >= 2;
        int phase = frame - 2;                                    // 0..3
        int bob = frame == 1 || phase is 1 or 3 ? 1 : 0;          // breathe / step down
        int legStride = !walking ? 0 : phase switch { 0 => 2, 2 => -2, _ => 0 };

        const int cx = 13, foot = 51;
        int legLen = 15 + p.Height / 2, torsoLen = 14 + (p.Height + 1) / 2;
        int hip = foot - legLen, shoulder = hip - torsoLen + bob;
        int headBottom = shoulder - 2, headTop = headBottom - 10;
        int w = p.Build;

        // Legs: back leg darker. Stride moves them apart.
        int backX = cx - 2 - legStride, frontX = cx + 1 + legStride;
        Rect(backX - 1, hip, backX + 1, foot - 2, p.BottomShade);
        Rect(frontX - 1, hip, frontX + 1, foot - 2, p.Bottom);
        Rect(backX - 1, foot - 1, backX + 2, foot, p.Shoes);
        Rect(frontX - 1, foot - 1, frontX + 2, foot, p.Shoes);

        // Arms behind the body first (the far arm), swinging against the legs.
        int swing = walking ? -legStride / 2 : 0;
        Rect(cx - w - 1 + swing, shoulder + 1, cx - w + 1 + swing, hip + 1, p.TopShade);
        Px(cx - w + swing, hip + 2, p.SkinShade);

        // Torso.
        Rect(cx - w + 1, shoulder, cx + w - 1, hip, p.TopColor);
        Rect(cx - w + 1, shoulder, cx - w + 2, hip, p.TopShade);  // shaded far side
        if (p.Belly) { Rect(cx + w, shoulder + torsoLen / 2, cx + w, hip - 1, p.TopColor); }
        switch (p.Top)
        {
            case TopStyle.Dress:
                for (int y = hip; y <= hip + legLen * 55 / 100; y++)
                    Rect(cx - w + 1 - (y - hip) / 4, y, cx + w - 1 + (y - hip) / 4 + (p.Belly ? 1 : 0), y, p.TopColor);
                break;
            case TopStyle.Cardigan:
                Rect(cx + 1, shoulder + 1, cx + 2, hip, p.TopShade);  // open front
                Rect(cx - w + 1, hip - 1, cx + w - 1, hip, p.TopShade);
                for (int y = hip + 1; y <= hip + 5; y++) Rect(cx - w + 1, y, cx + w - 1 + (p.Belly ? 1 : 0), y, p.Bottom); // skirt
                break;
            case TopStyle.ShirtTie:
                Rect(cx + 1, shoulder + 1, cx + 1, shoulder + 9, p.Accent ?? "8a6a3a");
                Px(cx + 1, shoulder, p.Accent ?? "8a6a3a");
                break;
            case TopStyle.SuitOpen:
                Rect(cx, shoulder, cx + 2, shoulder + 5, p.Accent ?? "ece9e4");   // open collar
                Px(cx + 1, shoulder + 6, p.Accent ?? "ece9e4");
                Px(cx + 1, shoulder + 2, p.SkinShade);
                break;
            case TopStyle.Vest:
                Rect(cx - w + 1, shoulder, cx - w + 2, shoulder + 3, p.SkinShade);   // bare shoulder
                Rect(cx + w - 2, shoulder, cx + w - 1, shoulder + 3, p.Skin);
                Rect(cx - 1, shoulder, cx + 3, shoulder + 2, p.Skin);                // the scoop of the vest
                Px(cx + 1, shoulder + 3, p.SkinShade);
                break;
            case TopStyle.Suit:
                Rect(cx, shoulder, cx + 2, shoulder + 2, p.Accent ?? "ece7dc");   // blouse at the neck
                break;
        }
        // Belt line for trousers.
        if (p.Top is TopStyle.Shirt or TopStyle.ShirtTie or TopStyle.SuitOpen)
            Rect(cx - w + 1, hip, cx + w - 1, hip, p.BottomShade);

        // Near arm, swinging the other way, with a hand.
        Rect(cx + w - 1 - swing, shoulder + 1, cx + w + 1 - swing, hip + 1, p.Top is TopStyle.ShirtTie or TopStyle.Vest ? p.Skin : p.TopColor);
        if (p.Top is TopStyle.ShirtTie) Rect(cx + w - 1 - swing, shoulder + 1, cx + w + 1 - swing, shoulder + 3, p.TopColor); // short sleeve
        Px(cx + w - swing, hip + 2, p.Skin);
        Px(cx + w + 1 - swing, hip + 2, p.Skin);

        // Neck and head, three-quarter view facing right.
        Rect(cx, headBottom + 1, cx + 2, shoulder - 1, p.SkinShade);
        int hx = cx + 1;
        for (int y = headTop; y <= headBottom; y++)
            for (int x = hx - 5; x <= hx + 5; x++)
            {
                double dx = (x - hx) / 5.2, dy = (y - (headTop + headBottom) / 2.0) / 5.6;
                if (dx * dx + dy * dy <= 1) Px(x, y, x <= hx - 3 ? p.SkinShade : p.Skin);
            }
        int eyeY = headTop + 5;
        Px(hx + 3, eyeY, "241418");
        Px(hx, eyeY, "241418");
        Px(hx + 5, eyeY + 2, p.SkinShade);          // nose
        Px(hx + 3, eyeY + 4, p.SkinShade);          // mouth
        Px(hx - 3, eyeY + 1, p.SkinShade);          // ear
        if (p.Earrings) Px(hx - 3, eyeY + 3, "e2b24a");
        if (p.Glasses)
        {
            // Thin frames above each eye, a bridge, and a glint on the near lens.
            Px(hx - 1, eyeY - 1, p.GlassesColor); Px(hx + 1, eyeY - 1, p.GlassesColor);
            Px(hx + 2, eyeY - 1, p.GlassesColor); Px(hx + 4, eyeY - 1, p.GlassesColor);
            Px(hx + 4, eyeY, p.GlassesColor);
            Px(hx + 4, eyeY + 1, "dfeaf2");
        }

        DrawHair(p, Px, Rect, hx, headTop, headBottom);
        if (p.Moustache is not null) Rect(hx + 1, eyeY + 3, hx + 4, eyeY + 3, p.Moustache);
        if (p.Top is TopStyle.Vest) Rect(cx - w - 1 + swing, shoulder + 4, cx - w + 1 + swing, hip + 1, p.SkinShade); // far arm, bare

        // Outline everything from outside.
        var mask = new Canvas(FrameW, FrameH);
        mask.Draw(c);
        c.Outline(mask, (_, _) => Outline);
        return c;
    }

    private static void DrawHair(PersonSpec p, Action<int, int, string> px, Action<int, int, int, int, string> rect, int hx, int top, int bottom)
    {
        switch (p.Hair)
        {
            case HairStyle.Messy:
                rect(hx - 5, top - 1, hx + 4, top + 2, p.HairColor);
                rect(hx - 5, top + 3, hx - 3, top + 6, p.HairColor);
                foreach (int x in new[] { hx - 4, hx - 1, hx + 2 }) px(x, top - 2, p.HairColor);
                rect(hx - 5, top + 1, hx - 4, top + 5, p.HairShade);
                break;
            case HairStyle.Perm:
                for (int y = top - 3; y <= bottom + 1; y++)
                    for (int x = hx - 8; x <= hx + 7; x++)
                    {
                        double dx = (x - hx + 0.5) / 8.0, dy = (y - (top + 3)) / 7.5;
                        bool inBlob = dx * dx + dy * dy <= 1;
                        bool face = x > hx - 3 && y > top + 2;
                        if (!inBlob || face) continue;
                        bool lit = ((x * 3 + y * 5) % 7) == 0 && y < top + 3;
                        px(x, y, lit ? "d4606f" : ((x + y) % 5 == 0 ? p.HairShade : p.HairColor));
                    }
                rect(hx - 2, top - 1, hx + 4, top + 1, p.HairColor);   // fringe
                px(hx + 5, top + 2, p.HairColor);
                px(hx - 1, top - 3, "b7adae");                          // the grey roots
                break;
            case HairStyle.SweptBack:
                rect(hx - 5, top - 2, hx + 3, top + 1, p.HairColor);
                rect(hx - 6, top, hx - 3, top + 6, p.HairColor);
                rect(hx - 3, top - 2, hx + 2, top - 2, p.HairShade);
                if (p.GreyAt is not null) rect(hx - 3, top + 2, hx - 2, top + 5, p.GreyAt);
                break;
            case HairStyle.Bun:
                rect(hx - 5, top - 1, hx + 3, top + 1, p.HairColor);
                rect(hx - 6, top + 1, hx - 4, top + 5, p.HairColor);
                rect(hx - 9, top - 3, hx - 6, top + 1, p.HairColor);   // the bun
                px(hx - 8, top - 3, p.HairShade);
                px(hx - 9, top, p.HairShade);
                break;
            case HairStyle.Bald:
                rect(hx - 6, top + 3, hx - 4, top + 7, p.HairColor);   // the grey fringe round the back
                px(hx - 5, top + 2, p.HairShade);
                px(hx + 1, top + 1, "f0d0b0");                        // shine on the dome
                break;
            case HairStyle.Combover:
                rect(hx - 6, top + 2, hx - 4, top + 6, p.HairColor);   // the sides
                foreach (int x in new[] { hx - 3, hx - 1, hx + 1, hx + 3 }) px(x, top - 1 + (x - hx + 3) / 3, p.HairColor);
                break;
        }
    }
}
