using OfficeMan.ArtGen.Raster;
using OfficeMan.ArtGen.Scenes;
using static OfficeMan.ArtGen.Rooms.RoomKit;

namespace OfficeMan.ArtGen.Rooms;

/// <summary>
/// Terasa Velka, the summer party: a riverside terrace, side-on, to the visual standard.
/// Left to right: the glass door from the street with the EU plaque, Magda's ticket table, the bar under a
/// striped awning (bottles, a mirror Ramona can watch Viktor in), Zdenka's table, a potted olive tree, the
/// railing over the river with string lights, two more tables, and the steps down to the river walk.
/// Behind: the river, the far bank's housing blocks and a chimney, the sky. Drawn unlit; the evening
/// (sun, dusk, string lights, night) is in content/rooms/terrace.json. The bar counter is a prop sprite
/// (<see cref="BarCounter"/>) so Laci can stand behind it and Teo in front of it.
/// </summary>
public static class TerraceRoom
{
    public const int W = 480, H = 270;
    private const int Base = 178, Horizon = 118;

    private static readonly Ramp Sky = Ramp.Of("5a5a78", "7a6e88", "a07e88", "c89480", "e2ac80", "f0c890", "f8e2b4");
    private static readonly Ramp Blocks = Ramp.Of("1e1e2a", "282836", "323244", "3e3e52", "4c4c60");
    private static readonly Ramp Trees = Ramp.Of("141a14", "1c261a", "263222", "30402a");
    private static readonly Ramp Water = Ramp.Of("1a2430", "22303e", "2c3e4e", "3a505e", "4e6670", "6a8088", "8a9c9c");
    private static readonly Ramp Stucco = Ramp.Of("4a4238", "5e5446", "746856", "8a7c66", "a29278", "bcaa8c");
    private static readonly Ramp Glass = Ramp.Of("1e2830", "2a3842", "3a4c56", "506670", "6c848c");
    private static readonly Ramp Metal = Ramp.Of("16181c", "22252a", "30343a", "444a52", "5c636c", "7c848c");
    private static readonly Ramp Brass = Ramp.Of("4a3612", "6e521e", "9a7630", "c49c48", "e0c070");
    private static readonly Ramp Canvas = Ramp.Of("5a1418", "7a1c22", "9e2a2e", "c03c3a");
    private static readonly Ramp CanvasLight = Ramp.Of("8a8070", "a89c88", "c6b8a0", "e0d4ba");
    private static readonly Ramp Wood = Ramp.Of("1e140e", "2c1e14", "3c2a1c", "503826", "664832", "7c5a40");
    private static readonly Ramp Deck = Ramp.Of("241810", "322218", "432e20", "56402c", "6a5038", "7e6244");
    private static readonly Ramp Cloth = Ramp.Of("8a8478", "a8a292", "c6c0ae", "e2dccc", "f4f0e4");
    private static readonly Ramp Bottle = Ramp.Of("10200c", "1c3414", "2c4a1e", "3e6a2a");
    private static readonly Ramp Amber = Ramp.Of("3a1a08", "5a2a0c", "7a4014", "9a5a20");
    private static readonly Ramp Leaf = Ramp.Of("1a2214", "26301c", "343f26", "445032", "56623e");
    private static readonly Ramp Pot = Ramp.Of("3a1a10", "54281a", "6e3a24", "8a4e32");
    private static readonly Ramp Stone = Ramp.Of("2a2826", "3a3734", "4c4844", "605a54", "766e66");

    public static RoomLayers Draw()
    {
        var a = new SceneCanvas(W, H);
        var e = new Canvas(W, H);
        var f = new Canvas(W, H);

        SkyAndRiver(a, e);
        Building(a, e);
        Railing(a);
        StringLights(a, e);
        Tables(a);
        Steps(a);
        DeckFloor(a);
        Foreground(f);

        return new RoomLayers(a.Render(), e, Glow(e), f);
    }

    // ── Behind the railing ─────────────────────────────────

    private static void SkyAndRiver(SceneCanvas a, Canvas e)
    {
        a.FillRect(0, 0, W, Horizon, Sky, (x, y) => 0.08 + y / (double)Horizon * 0.6, 0);   // the glow itself is a light (terrace.json), so night can put it out
        // A few long, thin evening clouds.
        foreach (var (x0, y0, len) in new[] { (180, 22, 90), (300, 34, 120), (240, 48, 60), (400, 18, 70) })
            for (int x = x0; x < x0 + len; x++)
                a.Paint(x, y0 + (x - x0) / 40, Sky, 0.95, 0);

        // The far bank: panel blocks, a chimney (the sewage plant), trees along the water.
        var rng = new Random(3);
        int bx = 150;
        while (bx < W)
        {
            int w = 22 + rng.Next(26), top = 64 + rng.Next(24);
            a.FillRect(bx, top, bx + w, Horizon, Blocks, (x, y) => 0.45 + (x == bx ? 0.2 : 0) - (y - top) / 200.0, 0);
            for (int y = top + 4; y < Horizon - 8; y += 5)
                for (int x = bx + 3; x < bx + w - 3; x += 4)
                {
                    a.FillRect(x, y, x + 1, y + 1, Blocks, (xx, yy) => 0.15, 0);
                    if (rng.NextDouble() < 0.35)
                    {
                        var c = rng.NextDouble() < 0.7 ? Rgb.Hex("ffd28a") : Rgb.Hex("a8c0ff");
                        Emit(e, x, y, x + 1, y + 1, c.Mix(Rgb.Hex("000000"), 0.45));
                        // Its reflection, broken by the water.
                        for (int ry = Horizon + 2 + (Horizon - y) / 3; ry < Horizon + 30; ry += 3)
                            if (rng.NextDouble() < 0.5) Emit(e, x, ry, x + 1, ry, c.Mix(Rgb.Hex("000000"), 0.8));
                    }
                }
            bx += w + 4 + rng.Next(10);
        }
        a.FillRect(424, 30, 430, Horizon, Stone, (x, y) => 0.4 + (x == 424 ? 0.2 : 0), 0);
        a.FillRect(423, 30, 431, 33, Stone, (x, y) => 0.6, 0);
        for (int x = 150; x < W; x++)
        {
            int top = 108 + (int)(Math.Sin(x * 0.21) * 2 + Math.Sin(x * 0.07) * 3);
            for (int y = top; y < Horizon; y++) a.Paint(x, y, Trees, 0.45 - (y - top) / 30.0, 0);
        }

        // The river: slow, brown-green, the light lying on it in long streaks.
        a.FillRect(0, Horizon, W, Base, Water, (x, y) => 0.55 - (y - Horizon) / 140.0, 0);
        for (int y = Horizon + 2; y < Base; y += 3)
            for (int x = (y * 37) % 23; x < W; x += 23 + (y % 7))
            {
                int len = 4 + (x * y) % 9;
                for (int i = 0; i < len && x + i < W; i++) a.Paint(x + i, y, Water, 0.75 - (y - Horizon) / 160.0, 0);
            }
    }

    private static void Building(SceneCanvas a, Canvas e)
    {
        // The restaurant: a stucco wall from the street door to the end of the bar.
        a.FillRect(0, 10, 160, Base, Stucco, (x, y) => 0.55 - (y - 10) / 500.0 + (x > 156 ? -0.2 : 0), 0);
        a.FillRect(0, 8, 162, 12, Stucco, (x, y) => 0.75, 0);
        a.FillRect(0, Base - 4, 160, Base, Stone, (x, y) => 0.35, 0);

        // The glass door from the street, and the brass plaque: Co-financed by the European Union.
        a.FillRect(2, 108, 32, Base, Metal, (x, y) => 0.3, 0);
        a.FillRect(5, 112, 29, Base, Glass, (x, y) => 0.45 + (x - 5) / 80.0, 0);
        Emit(e, 6, 114, 28, Base - 2, Rgb.Hex("ffd498").Mix(Rgb.Hex("000000"), 0.62));
        a.FillRect(16, 112, 17, Base, Metal, (x, y) => 0.4, 0);
        a.FillRect(36, 126, 46, 134, Brass, (x, y) => 0.6 + (y == 126 ? 0.25 : 0), 0);
        a.FillRect(38, 129, 44, 129 + 1, Brass, (x, y) => 0.2, 0);
        a.FillRect(38, 131, 42, 131 + 1, Brass, (x, y) => 0.2, 0);
        // The name over the door, in letters bolted to the wall.
        a.FillRect(3, 90, 60, 101, Brass, (x, y) => 0.65 + (y == 90 ? 0.2 : 0), 0);
        for (int i = 0; i < 11; i++)
            if (i != 6) a.FillRect(6 + i * 5, 93, 9 + i * 5, 98, Wood, (x, y) => 0.15, 0);

        // A big window into the dining room, lit.
        a.FillRect(6, 30, 60, 84, Metal, (x, y) => 0.3, 0);
        a.FillRect(8, 32, 58, 82, Glass, (x, y) => 0.5, 0);
        for (int y = 32; y < 82; y++)
            for (int x = 8; x < 58; x++)
            {
                bool curtain = x < 13 || x > 52 || (x > 29 && x < 36);
                bool diner = (x is > 15 and < 21 && y > 62) || (x is > 40 and < 46 && y > 60) || (Math.Pow((x - 18) / 3.0, 2) + Math.Pow((y - 58) / 3.5, 2) < 1) || (Math.Pow((x - 43) / 3.0, 2) + Math.Pow((y - 56) / 3.5, 2) < 1);
                double dim = curtain ? 0.8 : diner ? 0.9 : 0.62;
                e[x, y] = Rgb.Hex(curtain ? "e09060" : "ffcf88").Mix(Rgb.Hex("000000"), dim);
            }
        a.FillRect(32, 32, 33, 82, Metal, (x, y) => 0.35, 0);
        for (int x = 10; x < 56; x += 9) Emit(e, x, 36, x + 3, 38, Rgb.Hex("fff0c8").Mix(Rgb.Hex("000000"), 0.2));   // the chandeliers inside

        // Magda's ticket table by the door: a small table, an envelope, a cash tin.
        a.FillRect(36, 154, 56, 156, Cloth, (x, y) => 0.7, 0);
        a.FillRect(37, 156, 55, 160, Cloth, (x, y) => 0.55, 0);
        a.FillRect(45, 160, 47, Base, Metal, (x, y) => 0.35, 0);
        a.FillRect(41, 176, 51, Base, Metal, (x, y) => 0.3, 0);
        a.FillRect(39, 150, 46, 153, Cloth, (x, y) => 0.95, 0);
        a.FillRect(48, 150, 54, 153, Metal, (x, y) => 0.6, 0);

        // The bar's back wall under a striped awning: shelves of bottles and a long mirror.
        a.FillRect(64, 70, 160, 78, Canvas, (x, y) => (x / 6) % 2 == 0 ? 0.6 : 0, 0);
        for (int x = 64; x < 160; x++)
            for (int y = 70; y < 78; y++)
                if ((x / 6) % 2 == 1) a.Paint(x, y, CanvasLight, 0.6, 0);
        for (int x = 64; x < 160; x++)
        {
            int drop = 78 + (int)(Math.Abs(Math.Sin((x - 64) * Math.PI / 12)) * 4);
            for (int y = 78; y < drop; y++) a.Paint(x, y, (x / 6) % 2 == 0 ? Canvas : CanvasLight, 0.4, 0);
        }
        a.FillRect(96, 92, 140, 120, Glass, (x, y) => 0.55 + ((x + y) % 17 == 0 ? 0.25 : 0), 0);    // the mirror
        a.FillRect(94, 90, 142, 92, Wood, (x, y) => 0.5, 0);
        a.FillRect(94, 120, 142, 122, Wood, (x, y) => 0.5, 0);
        foreach (int shelfY in new[] { 108, 130 })
        {
            a.FillRect(70, shelfY, 158, shelfY + 1, Wood, (x, y) => 0.55, 0);
            for (int x = 72; x < 156; x += 4)
            {
                if (shelfY == 108 && x > 92 && x < 144) continue;
                int hgt = 7 + (x * 7) % 5;
                var ramp = (x / 4) % 3 == 0 ? Amber : (x / 4) % 3 == 1 ? Bottle : Glass;
                a.FillRect(x, shelfY - hgt, x + 2, shelfY - 1, ramp, (xx, yy) => 0.55 + (xx == x ? 0.25 : 0), 0);
                a.FillRect(x + 1, shelfY - hgt - 3, x + 1 + 1, shelfY - hgt - 1, ramp, (xx, yy) => 0.5, 0);
            }
        }
        // Beer taps on the back counter.
        for (int x = 100; x < 136; x += 8) a.FillRect(x, 138, x + 1, 146, Brass, (xx, y) => 0.75, 0);
    }

    // ── The terrace ────────────────────────────────────────

    private static void Railing(SceneCanvas a)
    {
        // Along the back of the deck: posts, a top rail, two thin rails; a gap at the right for the steps.
        const int x0 = 160, x1 = 458;
        a.FillRect(x0, 143, x1, 145, Metal, (x, y) => y == 143 ? 0.75 : 0.45, 0);
        foreach (int y in new[] { 157, 168 }) a.FillRect(x0, y, x1, y + 1, Metal, (x, yy) => 0.4, 0);
        for (int x = x0; x <= x1; x += 26) a.FillRect(x, 143, x + 1, Base, Metal, (xx, y) => 0.5 + (xx == x ? 0.15 : 0), 0);
        a.FillRect(x1 - 1, 143, x1 + 1, Base, Metal, (x, y) => 0.55, 0);
    }

    private static void StringLights(SceneCanvas a, Canvas e)
    {
        int[] poles = { 162, 272, 382, 458 };
        foreach (int px in poles) a.FillRect(px, 26, px + 1, Base, Wood, (x, y) => 0.45 + (x == px ? 0.15 : 0), 0);
        // From the awning's corner to each pole in turn, sagging; a bulb every eight pixels.
        var spans = new List<(int x0, int y0, int x1, int y1)> { (152, 70, 162, 28) };
        for (int i = 0; i + 1 < poles.Length; i++) spans.Add((poles[i], 28, poles[i + 1], 28));
        foreach (var (sx, sy, ex, ey) in spans)
            for (int x = sx; x <= ex; x++)
            {
                double t = (x - sx) / (double)Math.Max(1, ex - sx);
                int y = (int)(sy + (ey - sy) * t + Math.Sin(t * Math.PI) * 14);
                a.Paint(x, y, Metal, 0.25, 0);
                if ((x - sx) % 8 == 4)
                {
                    a.Paint(x, y + 1, Cloth, 0.95, 0);
                    a.Paint(x, y + 2, Cloth, 0.85, 0);
                    Emit(e, x, y + 1, x + 1, y + 2, Rgb.Hex("ffe0a0"));
                }
            }
    }

    private static void Tables(SceneCanvas a)
    {
        // Bistro tables against the railing (tops 24 px up, like the office desks). Zdenka's first.
        foreach (var (x0, x1, cloth) in new[] { (186, 206, true), (372, 392, false), (426, 446, false) })
        {
            if (cloth)
            {
                a.FillRect(x0, 154, x1, 156, Cloth, (x, y) => 0.8, 0);
                a.FillRect(x0 + 1, 156, x1 - 1, 162, Cloth, (x, y) => 0.6 - (x > x1 - 4 ? 0.15 : 0), 0);
            }
            else a.FillRect(x0, 154, x1, 156, Metal, (x, y) => y == 154 ? 0.7 : 0.45, 0);
            int mx = (x0 + x1) / 2;
            a.FillRect(mx - 1, cloth ? 162 : 157, mx, Base - 2, Metal, (x, y) => 0.4, 0);
            a.FillRect(mx - 6, Base - 2, mx + 6, Base, Metal, (x, y) => 0.35, 0);
        }
        // Zdenka's mineral water, with a slice of lemon; a reserved card on another table.
        a.FillRect(190, 148, 193, 153, Glass, (x, y) => 0.75, 0);
        a.Paint(191, 149, Brass, 0.9, 0);
        a.FillRect(378, 149, 385, 153, Cloth, (x, y) => 0.9, 0);
        a.FillRect(431, 146, 435, 153, Bottle, (x, y) => 0.55, 0);
        a.FillRect(437, 149, 441, 153, Glass, (x, y) => 0.7, 0);

        // The potted olive tree Kornel talks to (and someone pours zhivka into).
        a.FillRect(234, 160, 250, Base, Pot, (x, y) => 0.55 - (x > 246 ? 0.15 : 0) + (y == 160 ? 0.15 : 0), 0);
        a.FillRect(241, 132, 242, 160, Wood, (x, y) => 0.45, 0);
        var rng = new Random(11);
        for (int i = 0; i < 260; i++)
        {
            int x = 226 + rng.Next(32), y = 108 + rng.Next(32);
            if (Math.Pow((x - 242) / 15.0, 2) + Math.Pow((y - 124) / 15.0, 2) > 1) continue;
            a.FillRect(x, y, x + 2, y + 1, Leaf, (xx, yy) => 0.3 + rng.NextDouble() * 0.5 + (yy < 120 ? 0.15 : 0), 0);
        }
    }

    private static void Steps(SceneCanvas a)
    {
        // The steps down to the river walk, at the right end, below the deck; a handrail going down with them.
        for (int i = 0; i < 6; i++)
        {
            int x = 460 + i * 4, y = 150 + i * 5;
            a.FillRect(x, y, W, y + 1, Stone, (xx, yy) => 0.6, 0);
            a.FillRect(x, y + 2, W, y + 4, Stone, (xx, yy) => 0.35, 0);
        }
        for (int i = 0; i < 22; i++) a.Paint(460 + i, 140 + i * 5 / 4, Metal, 0.6, 0);
        a.FillRect(460, 140, 461, Base, Metal, (x, y) => 0.5, 0);
    }

    private static void DeckFloor(SceneCanvas a)
    {
        // Decking: long boards running left to right, staggered joints, darker near the railing.
        a.FillRect(0, Base, W, H, Deck, (x, y) =>
        {
            int d = y - Base, row = d / 6;
            bool seam = d % 6 == 0;
            bool joint = (x + row * 37) % 71 == 0;
            double contact = Math.Max(0, 1 - d / 10.0) * 0.25;
            double board = (row * 13 % 5) * 0.02;
            return 0.52 + board - contact - (seam ? 0.14 : 0) - (joint ? 0.1 : 0);
        }, 0);
    }

    private static void Foreground(Canvas f)
    {
        // A planter of lavender at the bottom left, in front of everything.
        for (int y = 246; y < H; y++)
            for (int x = 0; x < 46; x++)
                f[x, y] = Rgb.Hex(y < 249 ? "4a3a2e" : x > 40 ? "241a14" : "36281e");
        var rng = new Random(5);
        for (int i = 0; i < 26; i++)
        {
            int x = 3 + rng.Next(38), h = 8 + rng.Next(10);
            for (int y = 246 - h; y < 246; y++) f[x, y] = Rgb.Hex(y < 246 - h + 3 ? "6a5a9a" : "2a3a22");
        }
    }

    /// <summary>The bar counter as a prop: 92×34 per frame, six identical frames, anchored at its foot.</summary>
    public static Canvas BarCounter()
    {
        var a = new SceneCanvas(92, 34);
        a.FillRect(0, 0, 92, 3, Wood, (x, y) => y == 0 ? 0.85 : 0.65, 0);
        a.FillRect(2, 3, 90, 34, Wood, (x, y) => 0.4 + ((x - 2) % 15 == 0 ? -0.12 : 0) - (x > 86 ? 0.12 : 0), 0);
        a.FillRect(2, 30, 90, 34, Wood, (x, y) => 0.22, 0);
        a.FillRect(2, 22, 90, 23, Brass, (x, y) => 0.6, 0);      // the foot rail
        var one = a.Render();
        var sheet = new Canvas(92 * 6, 34);
        for (int i = 0; i < 6; i++) sheet.Draw(one, i * 92, 0);
        return sheet;
    }
}
