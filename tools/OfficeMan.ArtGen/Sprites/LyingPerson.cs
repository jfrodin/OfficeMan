using OfficeMan.ArtGen.Raster;

namespace OfficeMan.ArtGen.Sprites;

/// <summary>
/// Teo in bed, awake, looking at the ceiling: 64×30 frames, six to a sheet like the walking people.
/// Head on the pillow at the right, the blanket pulled up to his chin and rising as he breathes
/// (frames 0-1; 2-5 repeat them). Anchored at the bottom centre, which sits on the bed's blanket hem.
/// Drawn for the flat's bed: the frame's left edge is at x 333, its top at y 131 of the room.
/// </summary>
public static class LyingPerson
{
    public const int FrameW = 64, FrameH = 30;
    private static readonly Rgb Outline = Rgb.Hex("231a1e");

    public static Canvas Sheet(PersonSpec p)
    {
        var sheet = new Canvas(FrameW * PixelPeople.Frames, FrameH);
        for (int f = 0; f < PixelPeople.Frames; f++) sheet.Draw(Frame(p, f % 2), f * FrameW, 0);
        return sheet;
    }

    /// <summary>The blanket's upper edge: a long hump with the chest a little higher, rising on the in-breath.</summary>
    private static int BlanketTop(int x, int breath)
    {
        double hump = Math.Max(0, Math.Sin((x - 1) / 50.0 * Math.PI)) * 8 + (x > 29 ? 2 : 0);
        if (x > 20 && x < 46) hump += breath;
        return 19 - (int)hump;
    }

    private static Canvas Frame(PersonSpec p, int breath)
    {
        var body = new Canvas(FrameW, FrameH);   // the head: outlined like every person
        var cover = new Canvas(FrameW, FrameH);  // the blanket: no outline, it's bedding
        void Px(Canvas c, int x, int y, string hex) { if (c.InBounds(x, y)) c[x, y] = Rgb.Hex(hex); }
        void Rect(Canvas c, int x0, int y0, int x1, int y1, string hex)
        {
            for (int y = y0; y <= y1; y++)
                for (int x = x0; x <= x1; x++) Px(c, x, y, hex);
        }

        // Head on the pillow, face turned up: crown towards the headboard (right), nose to the ceiling.
        const int hx = 53, hy = 12;
        for (int y = hy - 5; y <= hy + 5; y++)
            for (int x = hx - 6; x <= hx + 6; x++)
            {
                double dx = (x - hx) / 6.2, dy = (y - hy) / 5.3;
                if (dx * dx + dy * dy <= 1) Px(body, x, y, y >= hy + 2 ? p.SkinShade : p.Skin);
            }
        Rect(body, hx + 2, hy - 5, hx + 6, hy + 3, p.HairColor);        // the back of the head, on the pillow
        Px(body, hx + 1, hy - 5, p.HairColor);
        Px(body, hx + 7, hy - 2, p.HairColor);                         // flattened, sticking out
        Px(body, hx + 7, hy - 4, p.HairShade);
        Rect(body, hx + 3, hy + 1, hx + 6, hy + 3, p.HairShade);
        Px(body, hx - 5, hy - 3, p.SkinShade);                         // the nose, pointing up
        Px(body, hx - 5, hy - 2, p.Skin);
        Px(body, hx - 2, hy - 3, "241418");                            // eyes open
        Px(body, hx - 3, hy - 4, p.SkinShade);
        Px(body, hx - 4, hy + 1, p.SkinShade);                         // mouth, flat
        Px(body, hx - 1, hy - 1, p.SkinShade);                         // stubble shadow under the eye

        // The blanket over the shape of a man who doesn't want to get up, up to his chin.
        for (int x = 1; x <= 47; x++)
        {
            int top = BlanketTop(x, breath);
            for (int y = top; y <= 28; y++)
            {
                string c = y == top ? "a0703e" : y - top < 4 ? "845a32" : y - top < 8 ? "6a4628" : "52341e";
                Px(cover, x, y, c);
            }
            Px(cover, x, 29, x / 6 % 2 == 0 ? "52341e" : "3a2416");     // the hem, hanging over the side
        }
        Rect(cover, 44, 13, 48, 18 - breath, "845a32");                   // pulled up to the chin
        Rect(cover, 44, 13, 48, 13, "a0703e");

        var mask = new Canvas(FrameW, FrameH);
        mask.Draw(body);
        body.Outline(mask, (_, _) => Outline);
        var frame = new Canvas(FrameW, FrameH);
        frame.Draw(cover);
        frame.Draw(body);
        return frame;
    }
}
