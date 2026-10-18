using OfficeMan.ArtGen.Raster;
using OfficeMan.ArtGen.Scenes;

namespace OfficeMan.ArtGen.Rooms;

/// <summary>
/// The Permits Section, 480×270, in layers for the room shader:
/// albedo (unlit colour), emissive (what glows), glow (blurred emissive) and front (in front of people).
/// Lights live in content/rooms/office.json; positions here must match it.
/// Emissive things stay above y≈134 so people (heads from y≈136 down) never stand in front of them.
/// </summary>
public static class OfficeRoom
{
    public const int W = 480, H = 270;
    private const int Rail = 134, WallBottom = 176;

    private static readonly Ramp Plaster = Ramp.Of("3a342e", "4a433b", "5c534a", "70665a", "867a6b", "9d907e", "b6a892");
    private static readonly Ramp Panel = Ramp.Of("231912", "2f2219", "3e2d21", "4e392a", "604735", "765842");
    private static readonly Ramp Floor = Ramp.Of("26221e", "322c26", "403830", "4e453b", "5e5346", "6e6253");
    private static readonly Ramp Ceiling = Ramp.Of("1a1816", "24211e", "302c28");
    private static readonly Ramp Metal = Ramp.Of("26282c", "393d42", "52575d", "6e747a", "8e949a", "b4b8bc");
    private static readonly Ramp GreenMetal = Ramp.Of("232a26", "323c35", "455248", "5b6a5e", "748476");
    private static readonly Ramp Wood = Ramp.Of("2a1d14", "3b2a1d", "513a28", "684b34", "805d42", "9a7252");
    private static readonly Ramp Fabric = Ramp.Of("16181e", "20232b", "2c303a", "3a3f4b");
    private static readonly Ramp Paper = Ramp.Of("6e685c", "948d7e", "b9b2a0", "dcd5c2", "f2eddc");
    private static readonly Ramp Cork = Ramp.Of("4a3420", "634830", "7c5c3e", "96724e");
    private static readonly Ramp Glass = Ramp.Of("26323a", "34444e", "465a64", "5c727c", "7a929a");
    private static readonly Ramp Blind = Ramp.Of("7a7466", "9a937f", "b8b19a", "d2cbb2");
    private static readonly Ramp RedB = Ramp.Of("5a1022", "8a1a34", "b8284a", "e04a64");
    private static readonly Ramp BlueB = Ramp.Of("142a5a", "1e3e86", "2e58b4", "5080d8");
    private static readonly Ramp Leaf = Ramp.Of("10180c", "182614", "22361c", "2e4824", "3c5c2c");
    private static readonly Ramp Pot = Ramp.Of("3a1e14", "54301e", "6e422a");
    private static readonly Ramp Water = Ramp.Of("1e3a5a", "2e5a86", "4a82b0", "7aaad0");

    public sealed record Layers(Canvas Albedo, Canvas Emissive, Canvas Glow, Canvas Front);

    public static Layers Draw()
    {
        var a = new SceneCanvas(W, H);
        var e = new Canvas(W, H);
        var f = new Canvas(W, H);

        Shell(a);
        Windows(a, e);
        WallThings(a, e);
        ViktorsOffice(a, e);
        DoorCabinetCooler(a, e);
        Desks(a, e);
        FloorThings(a);
        Foreground(f);

        return new Layers(a.Render(), e, Glow(e), f);
    }

    // ── Structure ──────────────────────────────────────────────

    private static void Shell(SceneCanvas a)
    {
        a.FillRect(0, 0, W, 12, Ceiling, (x, y) => 0.5, 0);
        a.FillRect(0, 12, W, Rail, Plaster, (x, y) =>
            0.55 - (y - 12) / 400.0 + Noise(x, y, 3) * 0.05, dither: 0.35);
        a.FillRect(0, Rail - 2, W, Rail + 1, Wood, (x, y) => y == Rail - 2 ? 0.7 : 0.4, 0);
        a.FillRect(0, Rail + 1, W, WallBottom, Panel, (x, y) =>
            0.5 + ((x % 24) == 0 ? -0.25 : (x % 24) == 1 ? 0.15 : 0) - (y - Rail) / 160.0, dither: 0.25);
        a.FillRect(0, WallBottom - 3, W, WallBottom, Panel, (x, y) => 0.15, 0);

        // Linoleum: large tiles, a faint checker, darker where it meets the wall.
        a.FillRect(0, WallBottom, W, H, Floor, (x, y) =>
        {
            int tx = (x + (y - WallBottom) / 3) / 24, ty = (y - WallBottom) / 12;
            double checker = (tx + ty) % 2 == 0 ? 0.04 : 0;
            double seam = (y - WallBottom) % 12 == 0 || (x + (y - WallBottom) / 3) % 24 == 0 ? -0.08 : 0;
            double contact = Math.Max(0, 1 - (y - WallBottom) / 10.0) * 0.25;
            return 0.55 + checker + seam - contact + Noise(x, y, 7) * 0.04;
        }, dither: 0.3);

        // Fluorescent fittings; the tubes glow.
        for (int i = 0; i < 4; i++)
        {
            int x0 = 30 + i * 110;
            a.FillRect(x0, 3, x0 + 66, 10, Metal, (x, y) => 0.45, 0);
        }
    }

    private static void Windows(SceneCanvas a, Canvas e)
    {
        foreach (int wx in new[] { 30, 118, 206 })
        {
            // Glass: low morning sun, warm at the horizon; the block opposite; blinds over the top.
            for (int y = 28; y < 106; y++)
                for (int x = wx + 2; x < wx + 68; x++)
                {
                    double t = (y - 28) / 78.0;
                    var sky = Rgb.Hex("8fa8bc").Mix(Rgb.Hex("f4d8a8"), Math.Pow(t, 1.4));
                    bool building = y > 76 - (x - wx) / 12 && y > 70;
                    var c = building ? Rgb.Hex("3a3a44").Mix(Rgb.Hex("6a5a4a"), (y - 70) / 36.0) : sky;
                    if (building && (x % 6 == 2) && (y % 7 == 3) && ((x * 7 + y) % 5 == 0)) c = Rgb.Hex("f0c070");
                    e[x, y] = c.Mix(Rgb.Hex("000000"), 0.5);
                    a.Paint(x, y, Glass, 0.3, 0);
                }
            // Venetian blinds, half down: slats are lit surfaces; light only through the gaps.
            for (int y = 28; y < 62; y++)
            {
                bool slat = (y - 28) % 4 < 2;
                for (int x = wx + 2; x < wx + 68; x++)
                    if (slat)
                    {
                        a.Paint(x, y, Blind, 0.55 + ((y - 28) % 4 == 0 ? 0.2 : 0), 0);
                        e[x, y] = Rgb.Hex("20180e");
                    }
            }
            a.FillRect(wx + 60, 62, wx + 61, 80, Metal, (x, y) => 0.6, 0);   // the cord
            // Frames and sill.
            a.FillRect(wx, 26, wx + 70, 28, Metal, (x, y) => 0.6, 0);
            a.FillRect(wx, 106, wx + 70, 110, Metal, (x, y) => y == 106 ? 0.75 : 0.45, 0);
            a.FillRect(wx, 26, wx + 2, 106, Metal, (x, y) => 0.5, 0);
            a.FillRect(wx + 68, 26, wx + 70, 106, Metal, (x, y) => 0.5, 0);
            a.FillRect(wx + 34, 28, wx + 36, 106, Metal, (x, y) => 0.55, 0);
            ClearEmissive(e, wx + 34, 28, wx + 36, 106);
        }
        // Tubes.
        for (int i = 0; i < 4; i++)
            for (int x = 32 + i * 110; x < 94 + i * 110; x++)
            {
                e[x, 6] = Rgb.Hex("e8f2ff");
                e[x, 7] = Rgb.Hex("c8daf0");
            }
    }

    private static void WallThings(SceneCanvas a, Canvas e)
    {
        // Noticeboard: notices nobody reads, one photo of the 2019 Christmas party.
        a.FillRect(286, 38, 328, 86, Wood, (x, y) => 0.45, 0);
        a.FillRect(288, 40, 326, 84, Cork, (x, y) => 0.55 + Noise(x, y, 11) * 0.12, 0.3);
        foreach (var (x0, y0, w, h, ramp, v) in new[] { (292, 44, 10, 13, Paper, 0.9), (305, 43, 12, 9, Paper, 0.7), (319, 46, 6, 8, RedB, 0.8),
                                                         (293, 61, 12, 8, BlueB, 0.7), (308, 57, 14, 16, Paper, 0.85), (296, 72, 9, 9, Paper, 0.6) })
            a.FillRect(x0, y0, x0 + w, y0 + h, ramp, (x, y) => v - (y == y0 ? 0 : 0.05), 0);

        // The EU poster: a smiling family on a bridge we haven't built yet.
        a.FillRect(334, 34, 368, 86, Paper, (x, y) => 0.9, 0);
        a.FillRect(336, 36, 366, 66, Glass, (x, y) => 0.9 - (y - 36) / 60.0, 0.4);
        a.FillRect(336, 58, 366, 62, Metal, (x, y) => 0.55, 0);
        foreach (int fx in new[] { 343, 348, 354, 359 }) a.FillRect(fx, 50, fx + 2, 58, RedB, (x, y) => 0.6, 0);
        for (int i = 0; i < 12; i++) a.Paint(345 + i, 70, BlueB, 0.9, 0);   // the stars, roughly
        a.FillRect(340, 75, 362, 76, Metal, (x, y) => 0.4, 0);

        // Clock.
        foreach (var (x, y) in Circle(380, 40, 7)) a.Paint(x, y, Paper, 0.95, 0);
        foreach (var (x, y) in Ring(380, 40, 8)) a.Paint(x, y, Metal, 0.25, 0);
        a.FillRect(380, 35, 381, 41, Metal, (x, y) => 0.05, 0);
        a.FillRect(380, 40, 384, 41, Metal, (x, y) => 0.05, 0);
    }

    private static void ViktorsOffice(SceneCanvas a, Canvas e)
    {
        // Inside: his own window, his desk, his chair, the minister photo.
        a.FillRect(392, 12, W, WallBottom, Plaster, (x, y) => 0.6 - (y - 12) / 500.0, 0.3);
        for (int y = 26; y < 98; y++)
            for (int x = 440; x < 476; x++)
            {
                e[x, y] = Rgb.Hex("9ab0c0").Mix(Rgb.Hex("f0d4a0"), (y - 26) / 72.0).Mix(Rgb.Hex("000000"), 0.5);
                a.Paint(x, y, Glass, 0.3, 0);
            }
        a.FillRect(438, 24, 478, 26, Metal, (x, y) => 0.6, 0);
        a.FillRect(438, 98, 478, 101, Metal, (x, y) => 0.5, 0);
        a.FillRect(402, 42, 428, 62, Wood, (x, y) => 0.5, 0);              // minister photo
        a.FillRect(405, 45, 425, 59, Paper, (x, y) => 0.55, 0);
        a.FillRect(410, 50, 413, 57, Fabric, (x, y) => 0.6, 0);
        a.FillRect(416, 50, 419, 57, Fabric, (x, y) => 0.6, 0);
        a.FillRect(410, 118, 474, 123, Wood, (x, y) => y == 118 ? 0.8 : 0.6, 0);   // the good desk
        a.FillRect(412, 123, 472, 160, Wood, (x, y) => 0.4, 0.2);
        a.FillRect(448, 96, 468, 118, Fabric, (x, y) => 0.5, 0);             // executive chair
        a.FillRect(396, 120, 406, 160, Pot, (x, y) => 0.5, 0);
        foreach (var (x, y) in Circle(401, 108, 10)) a.Paint(x, y, Leaf, 0.45 + ((x + y) % 3) * 0.1, 0);
        a.FillRect(392, 160, W, WallBottom, Floor, (x, y) => 0.5, 0.2);

        // The glass wall, with its door always open.
        for (int y = 12; y < WallBottom; y++)
            for (int x = 392; x < W; x++)
            {
                bool frame = x is 392 or 393 or 430 or 431 || y is 12 or 13;
                if (frame) { a.Paint(x, y, Metal, 0.55, 0); continue; }
                bool streak = ((x - y * 0.6) % 37 + 37) % 37 < 3;
                if (streak && x < 430) a.Paint(x, y, Glass, 0.95, 0);
            }
    }

    private static void DoorCabinetCooler(SceneCanvas a, Canvas e)
    {
        a.FillRect(2, 58, 30, WallBottom, Wood, (x, y) => 0.45 + (x is 2 or 3 or 28 or 29 ? -0.15 : 0) + Noise(x, y, 2) * 0.05, 0.2);
        a.FillRect(24, 112, 27, 115, Metal, (x, y) => 0.9, 0);
        for (int x = 8; x < 22; x++)
            for (int y = 50; y < 55; y++) e[x, y] = Rgb.Hex("40d070");       // EXIT, more or less
        a.FillRect(7, 49, 23, 56, Metal, (x, y) => 0.3, 0);

        a.FillRect(34, 124, 56, WallBottom, GreenMetal, (x, y) => 0.55 - (x == 34 ? 0.15 : 0), 0.2);
        for (int y = 136; y < WallBottom; y += 13) a.FillRect(35, y, 55, y + 1, GreenMetal, (x, yy) => 0.25, 0);
        for (int y = 129; y < WallBottom; y += 13) a.FillRect(43, y, 47, y + 1, Metal, (x, yy) => 0.8, 0);
        a.FillRect(36, 116, 54, 124, Paper, (x, y) => 0.6 + (y % 2) * 0.1, 0);  // files on top

        a.FillRect(62, 140, 74, WallBottom, Paper, (x, y) => 0.75 - (x == 62 ? 0.2 : 0), 0);
        foreach (var (x, y) in Circle(68, 130, 6)) a.Paint(x, y, Water, 0.55 + (x < 68 ? 0.15 : 0), 0);
        a.FillRect(65, 150, 68, 152, Metal, (x, y) => 0.4, 0);
    }

    private static void Desks(SceneCanvas a, Canvas e)
    {
        int[][] desks = { new[] { 80, 142 }, new[] { 160, 222 }, new[] { 240, 302 }, new[] { 318, 380 } };
        foreach (var d in desks) Desk(a, d[0], d[1]);

        // Magda: balloons on strings, the GIFT tin, a photo of two small kids, a mug that says MUM.
        foreach (var (bx, by, ramp) in new[] { (92, 88, RedB), (104, 80, BlueB), (116, 90, RedB), (110, 100, BlueB) })
        {
            foreach (var (x, y) in Circle(bx, by, 6)) a.Paint(x, y, ramp, 0.55 + (bx - x + by - y) * 0.035, 0);
            a.Paint(bx - 2, by - 3, Paper, 1.0, 0);
            for (int y = by + 6; y < 140; y++) a.Paint(bx + (y / 5 % 2), y, Paper, 0.75, 0);
        }
        a.FillRect(126, 130, 134, 140, Metal, (x, y) => 0.85, 0);
        a.FillRect(84, 128, 94, 140, Wood, (x, y) => 0.6, 0);
        a.FillRect(86, 130, 92, 138, Paper, (x, y) => 0.7, 0);
        a.FillRect(98, 134, 103, 140, Paper, (x, y) => 0.95, 0);

        // Teo: a monitor, a pile, a mug.
        Monitor(a, e, 172);
        a.FillRect(198, 128, 214, 140, Paper, (x, y) => 0.6 + ((y - 128) % 2) * 0.12, 0);
        a.FillRect(204, 124, 216, 128, Paper, (x, y) => 0.5, 0);
        a.FillRect(166, 134, 171, 140, Paper, (x, y) => 0.85, 0);

        // Kornel: a monitor, stacks squared to the edge, a pen pot.
        Monitor(a, e, 252);
        a.FillRect(280, 132, 296, 140, Paper, (x, y) => 0.8, 0);
        a.FillRect(282, 130, 294, 132, Paper, (x, y) => 0.9, 0);
        a.FillRect(244, 132, 249, 140, Metal, (x, y) => 0.6, 0);
        for (int i = 0; i < 3; i++) a.FillRect(245 + i * 2, 127, 246 + i * 2, 132, RedB, (x, y) => 0.6, 0);

        // Zdenka: one file, one ruler, a lamp.
        a.FillRect(326, 134, 344, 140, Paper, (x, y) => 0.75, 0);
        a.FillRect(346, 138, 366, 139, Metal, (x, y) => 0.95, 0);
        a.FillRect(372, 118, 374, 140, Metal, (x, y) => 0.6, 0);
        a.FillRect(364, 114, 376, 120, GreenMetal, (x, y) => 0.6, 0);
        for (int x = 366; x < 375; x++) e[x, 120] = Rgb.Hex("ffd890");

        // Chairs pulled out beside each desk.
        foreach (var d in desks) Chair(a, d[1] + 4);
    }

    private static void Desk(SceneCanvas a, int x0, int x1)
    {
        a.FillRect(x0, 140, x1, 143, Wood, (x, y) => y == 140 ? 0.85 : 0.6, 0);
        a.FillRect(x0 + 2, 143, x1 - 2, WallBottom, Wood, (x, y) => 0.35 + (x == x0 + 2 ? -0.1 : 0) + Noise(x, y, 5) * 0.04, 0.2);
        a.FillRect(x0 + 6, 150, x1 - 6, 151, Wood, (x, y) => 0.2, 0);
        a.FillRect(x0 + 6, 160, x1 - 6, 161, Wood, (x, y) => 0.2, 0);
    }

    private static void Monitor(SceneCanvas a, Canvas e, int x0)
    {
        a.FillRect(x0, 114, x0 + 24, 132, Fabric, (x, y) => 0.5, 0);
        for (int y = 116; y < 130; y++)
            for (int x = x0 + 2; x < x0 + 22; x++)
                e[x, y] = Rgb.Hex("6aa0c8").Mix(Rgb.Hex("dceaf4"), (y % 3 == 0 ? 0.35 : 0.1) + (x - x0) / 80.0).Mix(Rgb.Hex("000000"), 0.35);
        a.FillRect(x0 + 10, 132, x0 + 14, 138, Fabric, (x, y) => 0.4, 0);
        a.FillRect(x0 + 4, 138, x0 + 20, 140, Fabric, (x, y) => 0.55, 0);
    }

    private static void Chair(SceneCanvas a, int x)
    {
        a.FillRect(x, 146, x + 12, 162, Fabric, (xx, y) => 0.55 - (y - 146) / 80.0, 0.2);
        a.FillRect(x - 2, 162, x + 14, 166, Fabric, (xx, y) => 0.45, 0);
        a.FillRect(x + 5, 166, x + 7, 176, Metal, (xx, y) => 0.4, 0);
        a.FillRect(x - 1, 176, x + 13, 178, Metal, (xx, y) => 0.3, 0);
    }

    private static void FloorThings(SceneCanvas a)
    {
        // The file trolley.
        a.FillRect(306, 156, 322, 158, Metal, (x, y) => 0.6, 0);
        for (int y = 160; y < 178; y += 4) a.FillRect(304, y, 324, y + 3, Paper, (x, yy) => 0.55 + (y % 8) * 0.03, 0);
        a.FillRect(304, 178, 306, 186, Metal, (x, y) => 0.4, 0);
        a.FillRect(322, 178, 324, 186, Metal, (x, y) => 0.4, 0);
        // A bin by Teo's desk, a cable that someone will trip on, a box nobody has unpacked since 2019.
        a.FillRect(226, 172, 236, 184, Metal, (x, y) => 0.45 - (x == 226 ? 0.1 : 0), 0);
        a.FillRect(228, 170, 234, 172, Paper, (x, y) => 0.7, 0);
        for (int x = 200; x < 250; x++) a.Paint(x, 180 + (int)(Math.Sin(x * 0.2) * 1.2), Fabric, 0.3, 0);
        a.FillRect(150, 166, 164, 180, Paper, (x, y) => 0.55 - (y == 166 ? -0.1 : 0), 0);
    }

    private static void Foreground(Canvas f)
    {
        // A big office plant at the left edge and the back of a chair at the right: depth, and something to walk behind.
        var leaf = new[] { "0c140a", "142212", "1c3018", "263e20" };
        var rng = new Random(7);
        for (int i = 0; i < 26; i++)
        {
            double ang = -Math.PI / 2 + (rng.NextDouble() - 0.5) * 2.2;
            double len = 26 + rng.NextDouble() * 30;
            for (double t = 0; t < len; t += 0.5)
            {
                int x = (int)(18 + Math.Cos(ang) * t), y = (int)(232 + Math.Sin(ang) * t + t * t * 0.01);
                double w = Math.Sin(t / len * Math.PI) * 3;
                for (int k = (int)-w; k <= (int)w; k++)
                    if (f.InBounds(x + k, y)) f[x + k, y] = Rgb.Hex(leaf[(int)(t / len * 3.9)]);
            }
        }
        for (int y = 232; y < H; y++)
            for (int x = 4; x < 34; x++)
                f[x, y] = Rgb.Hex(x < 10 ? "1e100a" : x < 26 ? "2e1a10" : "24140c");
        // The back of an office chair: padded back, a lighter top seam, the arm and the gas lift.
        for (int y = 214; y < H; y++)
            for (int x = 430; x < 476; x++)
            {
                double dx = (x - 453) / 23.0;
                if (y < 214 + dx * dx * 8) continue;
                bool seam = y < 218 + dx * dx * 8;
                bool panel = Math.Abs(x - 453) < 15 && y > 224 && y < 252;
                f[x, y] = Rgb.Hex(seam ? "3a3f4b" : panel ? "262a33" : x < 436 ? "121418" : "1c1f26");
            }
        for (int y = 240; y < 246; y++)
            for (int x = 420; x < 432; x++) f[x, y] = Rgb.Hex(y == 240 ? "3a3f4b" : "1c1f26");
        for (int y = 252; y < H; y++)
            for (int x = 450; x < 456; x++) f[x, y] = Rgb.Hex(x == 450 ? "52575d" : "393d42");
    }

    // ── Helpers ────────────────────────────────────────────────

    /// <summary>Bloom: the emissive layer blurred at two widths, so glows have a tight core and a wide halo.</summary>
    private static Canvas Glow(Canvas e)
    {
        double[] Channel(Func<Rgb, byte> pick) =>
            Enumerable.Range(0, W * H).Select(i => e.IsSet(i % W, i / W) ? pick(e[i % W, i / W]) / 255.0 : 0).ToArray();
        var rgb = new[] { Channel(c => c.R), Channel(c => c.G), Channel(c => c.B) };
        var tight = rgb.Select(c => Blur(c, 3, 3)).ToArray();
        var wide = rgb.Select(c => Blur(c, 12, 3)).ToArray();
        var g = new Canvas(W, H);
        for (int i = 0; i < W * H; i++)
        {
            double r = tight[0][i] * 0.5 + wide[0][i] * 0.7, gg = tight[1][i] * 0.5 + wide[1][i] * 0.7, b = tight[2][i] * 0.5 + wide[2][i] * 0.7;
            g[i % W, i / W] = new Rgb((byte)Math.Clamp(r * 255, 0, 255), (byte)Math.Clamp(gg * 255, 0, 255), (byte)Math.Clamp(b * 255, 0, 255));
        }
        return g;
    }

    private static double[] Blur(double[] src, int r, int passes)
    {
        var a = (double[])src.Clone();
        var b = new double[a.Length];
        for (int p = 0; p < passes; p++)
        {
            for (int y = 0; y < H; y++)
            {
                double acc = 0;
                for (int x = -r; x <= r; x++) acc += a[y * W + Math.Clamp(x, 0, W - 1)];
                for (int x = 0; x < W; x++)
                {
                    b[y * W + x] = acc / (2 * r + 1);
                    acc += a[y * W + Math.Clamp(x + r + 1, 0, W - 1)] - a[y * W + Math.Clamp(x - r, 0, W - 1)];
                }
            }
            for (int x = 0; x < W; x++)
            {
                double acc = 0;
                for (int y = -r; y <= r; y++) acc += b[Math.Clamp(y, 0, H - 1) * W + x];
                for (int y = 0; y < H; y++)
                {
                    a[y * W + x] = acc / (2 * r + 1);
                    acc += b[Math.Clamp(y + r + 1, 0, H - 1) * W + x] - b[Math.Clamp(y - r, 0, H - 1) * W + x];
                }
            }
        }
        return a;
    }

    private static void ClearEmissive(Canvas e, int x0, int y0, int x1, int y1)
    {
        for (int y = y0; y < y1; y++)
            for (int x = x0; x < x1; x++) e.Clear(x, y);
    }

    private static double Noise(int x, int y, int seed)
    {
        uint h = (uint)(x * 374761393 + y * 668265263 + seed * 1442695041);
        h = (h ^ (h >> 13)) * 1274126177;
        return ((h ^ (h >> 16)) & 0xFFFF) / 65535.0 - 0.5;
    }

    private static IEnumerable<(int x, int y)> Circle(int cx, int cy, int r)
    {
        for (int y = cy - r; y <= cy + r; y++)
            for (int x = cx - r; x <= cx + r; x++)
                if ((x - cx) * (x - cx) + (y - cy) * (y - cy) <= r * r) yield return (x, y);
    }

    private static IEnumerable<(int x, int y)> Ring(int cx, int cy, int r)
    {
        for (int y = cy - r; y <= cy + r; y++)
            for (int x = cx - r; x <= cx + r; x++)
            {
                int d = (x - cx) * (x - cx) + (y - cy) * (y - cy);
                if (d <= r * r && d > (r - 1) * (r - 1)) yield return (x, y);
            }
    }
}
