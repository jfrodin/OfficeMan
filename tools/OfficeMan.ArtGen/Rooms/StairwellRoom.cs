using OfficeMan.ArtGen.Raster;
using OfficeMan.ArtGen.Scenes;
using static OfficeMan.ArtGen.Rooms.RoomKit;

namespace OfficeMan.ArtGen.Rooms;

/// <summary>
/// The fourth-floor landing of a 1979 panel block, side-on, to the visual standard.
/// Left to right: Teo's padded door (14), the electricity cabinet and its cables, Stojan's hand-made
/// NO SMOKING sign and the tin of cigarette ends beneath it, the stairs up to a half-landing window,
/// Stojan's door (15) with his slippers, an English WELCOME mat and a bag of empties.
/// Lit by one bare bulb (flickers, see stairwell.json) and, by day, the stair window.
/// Positions must match content/rooms/stairwell*.json.
/// </summary>
public static class StairwellRoom
{
    private const int Ceil = 22, Band = 112, Base = 178;

    private static readonly Ramp Whitewash = Ramp.Of("3a3830", "4e4b41", "646054", "7c776a", "948e7f", "aca594");
    private static readonly Ramp OilPaint = Ramp.Of("14241e", "1c3028", "263e34", "324e42", "406050", "50725f");
    private static readonly Ramp Concrete = Ramp.Of("1c1c1c", "262624", "32312e", "403e3a", "4e4c46");
    private static readonly Ramp Terrazzo = Ramp.Of("2a2826", "363330", "43403b", "524e48", "625d55", "736d64");
    private static readonly Ramp Leather = Ramp.Of("1e100c", "2c1810", "3c2216", "4e2e1e", "623c28");
    private static readonly Ramp RedLeather = Ramp.Of("240c0c", "361210", "4a1a16", "60241e", "763028");
    private static readonly Ramp Metal = Ramp.Of("26282c", "393d42", "52575d", "6e747a", "8e949a", "b4b8bc");
    private static readonly Ramp Brass = Ramp.Of("4a3612", "6e521e", "94702e", "bc9446");
    private static readonly Ramp Wood = Ramp.Of("2a1c12", "3c2a1a", "523a24", "6a4c30", "805e3c");
    private static readonly Ramp Paper = Ramp.Of("7a7466", "9a937f", "b8b19a", "d2cbb2", "e8e2cc");
    private static readonly Ramp Mat = Ramp.Of("1e1a14", "2c261c", "3c3424", "4e442e");
    private static readonly Ramp Glass = Ramp.Of("1e2a30", "2a3a42", "384c56", "485e6a");
    private static readonly Ramp Leaf = Ramp.Of("141c10", "1e2a16", "2a381c", "364624", "4a4a28");
    private static readonly Ramp Pot = Ramp.Of("34180e", "4e2414", "6a321c");
    private static readonly Ramp Bottle = Ramp.Of("10200c", "1c3414", "2a4a1e", "3c602a");
    private static readonly Ramp Slipper = Ramp.Of("2a1a14", "3e2820", "56382c", "6e4838");

    /// <param name="night">Night: the stair window is dark and only the bulb is lit.</param>
    public static RoomLayers Draw(bool night)
    {
        var a = new SceneCanvas(W, H);
        var e = new Canvas(W, H);
        var f = new Canvas(W, H);

        Shell(a);
        Stairs(a, e, night);
        TeosDoor(a, e);
        Cabinet(a);
        Sign(a);
        StojansDoor(a, e, night);
        Bulb(a, e);

        return new RoomLayers(a.Render(), e, Glow(e), f);
    }

    private static void Shell(SceneCanvas a)
    {
        a.FillRect(0, 0, W, Ceil, Concrete, (x, y) => 0.55 - y / 80.0, 0);
        // Upper walls: whitewash gone grey, darker towards the ceiling where nobody reaches.
        a.FillRect(0, Ceil, W, Band, Whitewash, (x, y) => 0.55 - Math.Max(0, 40 - (y - Ceil)) / 160.0 + Noise(x / 8, y / 6, 4) * 0.04, 0);
        // A damp stain spreading from the corner above the stairs.
        for (int y = Ceil; y < Ceil + 30; y++)
            for (int x = 236; x < 300; x++)
            {
                double d = Math.Sqrt(Math.Pow((x - 268) / 32.0, 2) + Math.Pow((y - Ceil) / 30.0, 2));
                double wobble = Noise(x / 5, y / 5, 9) * 0.25;
                if (d + wobble < 0.9) a.Paint(x, y, Whitewash, 0.38 + d * 0.15, 0);
            }
        // Dark green oil paint below, a black stripe between, shoe scuffs along the bottom.
        a.FillRect(0, Band, W, Band + 2, Concrete, (x, y) => 0.15, 0);
        a.FillRect(0, Band + 2, W, Base, OilPaint, (x, y) => 0.55 - (y - Band) / 240.0, 0);
        foreach (int x0 in new[] { 92, 118, 164, 430, 452 })
            a.FillRect(x0, 164 + x0 % 5, x0 + 7, 165 + x0 % 5, OilPaint, (x, y) => 0.25, 0);
        // Terrazzo floor in big cast tiles, laid the same way as the office's linoleum; a dark border strip
        // along the wall; a lighter path worn between the doors and the stairs.
        a.FillRect(0, Base, W, H, Terrazzo, (x, y) =>
        {
            int d = y - Base;
            if (d < 7) return d == 0 ? 0.12 : 0.24;                                  // the border strip
            int u = x + (d - 7) / 3, v = d - 7;
            bool seam = u % 30 == 0 || v % 14 == 0;
            double tile = ((u / 30 + v / 14) % 2 == 0) ? 0.03 : 0;
            double path = Math.Max(0, 1 - Math.Abs(y - 212) / 16.0) * 0.07;
            return 0.46 + tile + path - (seam ? 0.1 : 0);
        }, 0);
        // Cables along the ceiling, from the cabinet to the bulb and on.
        for (int x = 100; x < W; x++) a.Paint(x, Ceil + 3 + (x > 300 ? 1 : 0), Concrete, 0.12, 0);
        for (int x = 100; x < 200; x++) a.Paint(x, Ceil + 5, Concrete, 0.1, 0);
    }

    private static void TeosDoor(SceneCanvas a, Canvas e)
    {
        Door(a, 36, Leather, "14");
        a.FillRect(30, Base, 80, Base + 4, Mat, (x, y) => 0.4, 0);
        a.FillRect(78, 128, 82, 133, Paper, (x, y) => 0.55, 0);           // doorbell
        a.Paint(80, 130, Metal, 0.2, 0);
    }

    /// <summary>A padded leatherette door: diamond-studded, with a brass number and handle.</summary>
    private static void Door(SceneCanvas a, int x0, Ramp leather, string number)
    {
        a.FillRect(x0 - 3, Base - 69, x0 + 37, Base, Concrete, (x, y) => 0.3, 0);       // frame
        a.FillRect(x0, Base - 66, x0 + 34, Base, leather, (x, y) =>
        {
            int u = (x - x0 + (y / 8 % 2) * 4) % 8, v = (y - (Base - 66)) % 8;
            bool stud = u == 4 && v == 4;
            double quilt = (u is 0 or 1 || v is 0) ? -0.12 : 0.02;
            return 0.5 + quilt + (stud ? 0.35 : 0) - (x - x0) / 120.0;
        }, 0);
        a.FillRect(x0 + 28, Base - 34, x0 + 32, Base - 31, Brass, (x, y) => 0.8, 0);
        a.FillRect(x0 + 12, Base - 60, x0 + 22, Base - 54, Brass, (x, y) => 0.6, 0);
        Digits((x, y) => a.Paint(x, y, Brass, 0.15, 0), x0 + 13, Base - 60, number);
    }

    private static void Cabinet(SceneCanvas a)
    {
        a.FillRect(98, 56, 134, 98, Metal, (x, y) => 0.45 - (x > 130 ? 0.12 : 0), 0);
        a.FillRect(100, 58, 132, 96, Metal, (x, y) => 0.4, 0);
        a.FillRect(114, 74, 118, 80, Metal, (x, y) => 0.8, 0);                     // lock
        a.FillRect(102, 62, 112, 68, Paper, (x, y) => 0.75, 0);                    // the sticker with the warning bolt
        a.FillRect(105, 63, 108, 67, Brass, (x, y) => 0.7, 0);
        for (int y = Ceil + 3; y < 56; y++) { a.Paint(108, y, Concrete, 0.1, 0); a.Paint(122, y, Concrete, 0.12, 0); }
    }

    private static void Sign(SceneCanvas a)
    {
        // Stojan's own NO SMOKING sign, taped up crooked. He smokes under it.
        a.FillRect(150, 80, 176, 98, Paper, (x, y) => 0.8 - (x - 150) / 200.0, 0);
        foreach (var (x, y) in Ring(163, 89, 6)) a.Paint(x, y, RedLeather, 0.9, 0);
        for (int i = -4; i <= 4; i++) a.Paint(163 + i, 89 + i, RedLeather, 0.9, 0);
        a.FillRect(158, 88, 168, 89, Concrete, (x, y) => 0.1, 0);                 // the cigarette
        a.FillRect(150, 79, 154, 81, Paper, (x, y) => 0.95, 0);                   // tape
        a.FillRect(172, 97, 176, 99, Paper, (x, y) => 0.95, 0);
        // The tin can of ends, on the floor below it.
        a.FillRect(158, Base - 8, 166, Base, Metal, (x, y) => 0.55 - (x > 163 ? 0.15 : 0), 0);
        a.FillRect(159, Base - 10, 165, Base - 8, Paper, (x, y) => 0.6, 0);
    }

    private static void Stairs(SceneCanvas a, Canvas e, bool night)
    {
        // The half-landing window, with a neighbour's geranium that nobody waters.
        const int wx0 = 300, wx1 = 352, wy0 = 34, wy1 = 86;
        for (int y = wy0; y < wy1; y++)
            for (int x = wx0; x < wx1; x++)
            {
                double t = (y - wy0) / (double)(wy1 - wy0);
                var c = night ? Rgb.Hex("0c1428").Mix(Rgb.Hex("1a2240"), t) : Rgb.Hex("8ea2b8").Mix(Rgb.Hex("c8c4b0"), t);
                if ((x - wx0) % 13 == 0 || (y - wy0) % 13 == 0) c = c.Mix(Rgb.Hex("000000"), 0.3);   // wired glass
                e[x, y] = c.Mix(Rgb.Hex("000000"), night ? 0.25 : 0.62);
                a.Paint(x, y, Glass, 0.4, 0);
            }
        a.FillRect(wx0 - 3, wy0 - 3, wx1 + 3, wy0, Concrete, (x, y) => 0.45, 0);
        a.FillRect(wx0 - 3, wy1, wx1 + 3, wy1 + 4, Concrete, (x, y) => 0.5, 0);
        a.FillRect(wx0 - 3, wy0, wx0, wy1, Concrete, (x, y) => 0.45, 0);
        a.FillRect(wx1, wy0, wx1 + 3, wy1, Concrete, (x, y) => 0.45, 0);
        a.FillRect(336, wy1 - 10, 346, wy1, Pot, (x, y) => 0.5, 0);
        foreach (var (x, y) in Circle(341, wy1 - 14, 5)) a.Paint(x, y, Leaf, 0.35 + ((x + y) % 3) * 0.1, 0);
        a.Paint(339, wy1 - 18, RedLeather, 0.7, 0);

        // The flight up: ten steps from the landing to the half-landing, under them the dark.
        const int sx = 196, steps = 10, rise = 7, run = 10;
        for (int i = 0; i < steps; i++)
        {
            int x0 = sx + i * run, y0 = Base - (i + 1) * rise;
            a.FillRect(x0, y0, x0 + run + 1, y0 + 2, Concrete, (x, y) => 0.6, 0);            // tread nosing
            a.FillRect(x0, y0 + 2, x0 + run + 1, y0 + rise, Concrete, (x, y) => 0.42, 0);     // riser
            a.FillRect(x0, y0 + rise, x0 + run + 1, y0 + rise + 5, Concrete, (x, y) => 0.3, 0);                // the stringer
        }
        int top = Base - steps * rise;
        a.FillRect(sx + steps * run, top, 372, top + 3, Concrete, (x, y) => 0.6, 0);         // half-landing
        a.FillRect(sx + steps * run, top + 3, 372, top + 8, Concrete, (x, y) => 0.32, 0);                   // the slab's edge
        a.FillRect(366, top + 8, 372, Base, Concrete, (x, y) => 0.38, 0);                                       // its pillar
        // Railing: thin metal balusters, a worn wooden handrail.
        for (int i = 0; i <= steps; i++)
        {
            int x = sx + i * run + 3, yTop = Base - i * rise - 26, yBot = Base - i * rise - (i < steps ? 7 : 0);
            a.FillRect(x, yTop, x + 1, yBot, Metal, (xx, y) => 0.35, 0);
        }
        for (int x = sx; x <= sx + steps * run + 6; x++)
        {
            int y = Base - 26 - (int)((x - sx) * rise / (double)run);
            a.FillRect(x, y - 2, x + 1, y + 1, Wood, (xx, yy) => yy == y - 2 ? 0.7 : 0.45, 0);
        }
        // Someone scratched their initials into the paint by the stairs.
        foreach (var (x, y) in new[] { (186, 140), (187, 141), (188, 140), (190, 140), (190, 141), (190, 142), (192, 141), (193, 140), (194, 141) })
            a.Paint(x, y, OilPaint, 0.85, 0);
    }

    private static void StojansDoor(SceneCanvas a, Canvas e, bool night)
    {
        Door(a, 392, RedLeather, "15");
        if (night) Emit(e, 392, Base - 1, 426, Base, Rgb.Hex("6a4a28"));               // TV light under the door
        a.FillRect(386, Base, 432, Base + 5, Mat, (x, y) => 0.45, 0);
        for (int x = 392; x < 426; x += 5) a.FillRect(x, Base + 1, x + 3, Base + 3, Paper, (xx, y) => 0.45, 0);   // WELCOME, in English
        // His slippers, enormous; a bag of empties waiting to go down.
        a.FillRect(434, Base - 3, 446, Base, Slipper, (x, y) => 0.5, 0);
        a.FillRect(447, Base - 3, 459, Base, Slipper, (x, y) => 0.45, 0);
        a.FillRect(456, Base - 20, 474, Base, Mat, (x, y) => 0.45 + (x > 470 ? -0.1 : 0) + (y == Base - 20 ? 0.15 : 0), 0);
        foreach (int bx in new[] { 459, 464, 469 })
            a.FillRect(bx, Base - 30 + bx % 3, bx + 3, Base - 20, Bottle, (x, y) => 0.55 + (x == bx ? 0.2 : 0), 0);
    }

    private static void Bulb(SceneCanvas a, Canvas e)
    {
        a.FillRect(200, Ceil + 3, 201, 40, Concrete, (x, y) => 0.1, 0);
        a.FillRect(197, 40, 204, 43, Metal, (x, y) => 0.4, 0);
        foreach (var (x, y) in Circle(200, 47, 3)) { a.Paint(x, y, Paper, 1.0, 0); e[x, y] = Rgb.Hex("ffe2a8"); }
    }
}
