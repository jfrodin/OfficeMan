using OfficeMan.ArtGen.Raster;

namespace OfficeMan.ArtGen.Scenes;

/// <summary>
/// Teo's flat, Tuesday, 06:47. A 1979 panel-block room in low summer sun.
/// The window is the only real light; everything else is cool ambient.
/// </summary>
public static class TeoFlat
{
    public const int W = 320, H = 180;

    // ── Materials ──────────────────────────────────────────────
    private static readonly Ramp Wallpaper = Ramp.Of("1f2624", "2c3430", "3c4540", "50584e", "686c5c", "868468", "a8a07c", "cbb98c");
    private static readonly Ramp WallStripe = Ramp.Of("1c2220", "28302c", "36403a", "4a5248", "606656", "7c7c62", "9c9674", "bfae84");
    private static readonly Ramp SideWall = Ramp.Of("1a201f", "252c2a", "323a36", "434a44", "585e52", "727260");
    private static readonly Ramp Ceiling = Ramp.Of("22262a", "2e3236", "3c4042", "4c4e4e", "60605c", "78766c");
    private static readonly Ramp Floor = Ramp.Of("1c120e", "2a1a12", "3c2618", "52341e", "6a4426", "86582e", "a6703a", "c88e4a");
    private static readonly Ramp Sky = Ramp.Of("41507c", "5c6390", "8a7094", "c08488", "e0a084", "f2c48e", "fbe2a8");
    private static readonly Ramp Blocks = Ramp.Of("232838", "2e3446", "3a4054", "4a4e60");
    private static readonly Ramp Frame = Ramp.Of("2a2a2c", "48484a", "6c6c68", "94928a", "bcb8aa", "e0dacb");
    private static readonly Ramp Radiator = Ramp.Of("26282a", "3a3c3e", "56585a", "76787a", "9c9c98", "c4c2b8");
    private static readonly Ramp Blanket = Ramp.Of("2a1e12", "3e2c18", "56401e", "735826", "907030", "b08c40", "d0ac58");
    private static readonly Ramp Sheet = Ramp.Of("33343e", "4a4c58", "686a76", "8e8e96", "b6b4b2", "dcd6c8", "f2ead8");
    private static readonly Ramp DarkWood = Ramp.Of("140e0c", "1f1612", "2c2018", "3c2c20", "50392a", "684a34");
    private static readonly Ramp Terracotta = Ramp.Of("2c1610", "46221a", "642e20", "843e28", "a65232", "c46a40");
    private static readonly Ramp Leaf = Ramp.Of("16200f", "223018", "324220", "485428", "626830", "7e7a38", "a09444");
    private static readonly Ramp Rug = Ramp.Of("1e1218", "2e1a22", "44222c", "5c2e34", "763c3e", "94504a");
    private static readonly Ramp Plastic = Ramp.Of("141416", "222226", "34343a", "4a4a52");

    private static readonly Rgb LitWindow = Rgb.Hex("f2c870");
    private static readonly Rgb DimWindow = Rgb.Hex("8a6a48");
    private static readonly Rgb ClockRed = Rgb.Hex("ff4a32");
    private static readonly Rgb CrackDark = Rgb.Hex("1a1c1e");

    // ── Room geometry (one-point perspective) ──────────────────
    private const double BackL = 36, BackR = 284, BackT = 22, BackB = 124;
    private const double WinL = 62, WinR = 128, WinT = 38, WinB = 96;

    public static Canvas Draw()
    {
        var s = new SceneCanvas(W, H);

        DrawShell(s);
        DrawWindow(s);
        DrawRadiator(s);
        DrawRug(s);
        DrawWardrobe(s);
        DrawBed(s);
        DrawNightstand(s);
        DrawFicus(s);
        DrawCrack(s);
        LightTheRoom(s);

        var canvas = s.Render();
        DrawEmissive(canvas);
        return canvas;
    }

    // ── Shell ──────────────────────────────────────────────────

    private static void DrawShell(SceneCanvas s)
    {
        // Back wall: tired striped wallpaper, darker towards the corners and the floor.
        s.FillRect((int)BackL, (int)BackT, (int)BackR, (int)BackB, Wallpaper, (x, y) =>
            0.42 - Corner(x, y) * 0.25 + (y - BackT) / (BackB - BackT) * -0.06, dither: 0.25);
        for (int x = (int)BackL; x < BackR; x++)
            if ((x - (int)BackL) % 12 is 0 or 1)
                for (int y = (int)BackT; y < BackB; y++)
                    s.Paint(x, y, WallStripe, 0.42 - Corner(x, y) * 0.25, 0.25);

        // A paler rectangle where a picture used to hang.
        s.FillRect(152, 44, 176, 66, Wallpaper, (x, y) => 0.5, dither: 0.3);

        // Side walls and ceiling.
        s.FillPolygon(new[] { (0.0, 0.0), (BackL, BackT), (BackL, BackB), (0.0, 180.0) }, SideWall, (x, y) => 0.22 + x / BackL * 0.12, dither: 0.3);
        s.FillPolygon(new[] { (320.0, 0.0), (BackR, BackT), (BackR, BackB), (320.0, 180.0) }, SideWall, (x, y) => 0.24 + (320 - x) / (320 - BackR) * 0.1, dither: 0.3);
        s.FillPolygon(new[] { (0.0, 0.0), (320.0, 0.0), (BackR, BackT), (BackL, BackT) }, Ceiling, (x, y) => 0.25 + y / BackT * 0.12, dither: 0.3);

        // Floor: planks running towards the back wall.
        s.FillPolygon(new[] { (BackL, BackB), (BackR, BackB), (320.0, 180.0), (0.0, 180.0) }, Floor, (x, y) =>
        {
            double depth = (y - BackB) / (H - BackB);
            // Plank seams converge on the vanishing point (160, 70).
            double u = (x - 160) / Math.Max(1, y - 70) * 54;
            double plank = ((u % 6) + 6) % 6;
            double seam = plank < 0.6 ? -0.14 : 0;
            int plankIndex = (int)Math.Floor(u / 6);
            double tone = ((plankIndex * 7919) % 5) * 0.015;
            return 0.28 + depth * 0.08 + seam + tone - Corner(x, y) * 0.1;
        }, dither: 0.3);

        // Skirting board.
        for (int x = (int)BackL; x < BackR; x++)
        {
            s.Paint(x, (int)BackB - 2, DarkWood, 0.35);
            s.Paint(x, (int)BackB - 1, DarkWood, 0.25);
        }
    }

    private static double Corner(double x, double y)
    {
        double dx = Math.Min(x - BackL, BackR - x) / 40;
        double dy = Math.Min(y - BackT, BackB - y) / 30;
        return Math.Clamp(1 - Math.Min(dx, dy), 0, 1);
    }

    // ── Window and the city ────────────────────────────────────

    private static void DrawWindow(SceneCanvas s)
    {
        // Sky: dawn, warm at the horizon.
        for (int y = (int)WinT; y < WinB; y++)
            for (int x = (int)WinL; x < WinR; x++)
                s.Paint(x, y, Sky, 0.95 - (y - WinT) / (WinB - WinT) * -0.0 - (WinB - y) / (WinB - WinT) * 0.75, dither: 1.0);

        // The low sun, just above the blocks.
        for (int y = 64; y < 84; y++)
            for (int x = 70; x < 92; x++)
            {
                double d = Math.Sqrt(Math.Pow(x - 80, 2) + Math.Pow(y - 74, 2));
                if (d < 4.2) s.Paint(x, y, Sky, 1.2, 0);
                else if (d < 9) s.AddLight(x, y, 0.25 * (1 - d / 9));
            }

        // Opposite panel blocks: flat silhouettes with a few lit windows.
        var rng = new Random(1979);
        int[] blockX = { 62, 78, 98, 114 };
        int[] blockTop = { 70, 58, 66, 74 };
        int[] blockW = { 18, 22, 18, 14 };
        for (int b = 0; b < blockX.Length; b++)
            for (int y = blockTop[b]; y < WinB; y++)
                for (int x = blockX[b]; x < Math.Min(WinR, blockX[b] + blockW[b]); x++)
                {
                    s.Paint(x, y, Blocks, 0.3 + (x == blockX[b] ? 0.15 : 0), 0);
                    bool windowCell = (x - blockX[b]) % 4 is 1 or 2 && (y - blockTop[b]) % 5 is 2 or 3;
                    if (windowCell && rng.NextDouble() < 0.18) s.Emit(x, y, rng.NextDouble() < 0.5 ? LitWindow : DimWindow);
                }

        // Frame: white-painted, chipped, with a mullion and transom.
        void FrameRect(int x0, int y0, int x1, int y1)
        {
            for (int y = y0; y < y1; y++)
                for (int x = x0; x < x1; x++)
                    s.Paint(x, y, Frame, 0.55 + (y == y0 ? 0.1 : 0), 0.3);
        }
        FrameRect((int)WinL - 4, (int)WinT - 4, (int)WinR + 4, (int)WinT);
        FrameRect((int)WinL - 4, (int)WinB, (int)WinR + 4, (int)WinB + 4);
        FrameRect((int)WinL - 4, (int)WinT, (int)WinL, (int)WinB);
        FrameRect((int)WinR, (int)WinT, (int)WinR + 4, (int)WinB);
        FrameRect(94, (int)WinT, 97, (int)WinB);
        FrameRect((int)WinL, 54, (int)WinR, 56);

        // Sill.
        for (int x = (int)WinL - 7; x < WinR + 7; x++)
        {
            s.Paint(x, (int)WinB + 4, Frame, 0.7, 0);
            s.Paint(x, (int)WinB + 5, Frame, 0.4, 0);
        }

        // Half-drawn curtain on the right, thin and faded.
        for (int y = (int)WinT - 6; y < 104; y++)
            for (int x = 120; x < 140; x++)
            {
                double fold = Math.Sin((x - 120) * 0.9) * 0.08;
                double sway = Math.Sin(y * 0.05) * 1.5;
                if (x < 122 + sway) continue;
                s.Paint(x, y, Rug, 0.38 + fold, 0.4);
            }
    }

    private static void DrawRadiator(SceneCanvas s)
    {
        for (int y = 104; y < 118; y++)
            for (int x = 66; x < 124; x++)
            {
                bool fin = (x - 66) % 4 != 3;
                s.Paint(x, y, Radiator, fin ? 0.38 + (y < 106 ? 0.12 : 0) : 0.18, 0.2);
            }
        // Pipe down to the floor.
        for (int y = 118; y < 128; y++) { s.Paint(118, y, Radiator, 0.3, 0); s.Paint(119, y, Radiator, 0.45, 0); }
    }

    // ── Furniture ──────────────────────────────────────────────

    private static void DrawRug(SceneCanvas s)
    {
        var rug = new[] { (70.0, 134.0), (196.0, 134.0), (214.0, 168.0), (48.0, 168.0) };
        s.FillPolygon(rug, Rug, (x, y) =>
        {
            bool border = y < 137 || y > 164 || !SceneCanvas.InsidePolygon(rug, x - 4, y) || !SceneCanvas.InsidePolygon(rug, x + 4, y);
            double pattern = ((x / 6 + y / 4) % 2 == 0) ? 0.04 : 0;
            return (border ? 0.22 : 0.32) + pattern;
        }, dither: 0.5);
    }

    private static void DrawWardrobe(SceneCanvas s)
    {
        // A tall veneered wardrobe on the left wall, in perspective.
        var front = new[] { (6.0, 26.0), (30.0, 38.0), (30.0, 134.0), (6.0, 150.0) };
        var side = new[] { (30.0, 38.0), (40.0, 36.0), (40.0, 128.0), (30.0, 134.0) };
        s.FillPolygon(side, DarkWood, (x, y) => 0.42);
        s.FillPolygon(front, DarkWood, (x, y) =>
        {
            double grain = Math.Sin(y * 0.4 + Math.Sin(x * 0.7) * 2) * 0.03;
            bool seam = Math.Abs(x - 18) < 0.6;
            return 0.28 + grain - (seam ? 0.15 : 0);
        });
        // Knobs.
        s.Paint(16, 84, Frame, 0.5, 0);
        s.Paint(20, 86, Frame, 0.5, 0);
    }

    private static void DrawBed(SceneCanvas s)
    {
        // Headboard against the back wall.
        s.FillRect(196, 84, 284, 112, DarkWood, (x, y) => 0.3 + (y < 87 ? 0.1 : 0));

        // Bed body in perspective: top surface and front face.
        var top = new[] { (196.0, 104.0), (284.0, 104.0), (304.0, 150.0), (176.0, 150.0) };
        var front = new[] { (176.0, 150.0), (304.0, 150.0), (304.0, 166.0), (176.0, 166.0) };
        s.FillPolygon(front, Blanket, (x, y) => 0.18 + (y < 152 ? 0.06 : 0));

        // Rumpled blanket: a lumpy height field so the sun can catch the folds.
        s.FillPolygon(top, Blanket, (x, y) =>
        {
            double lumps = Math.Sin(x * 0.11 + y * 0.18) * 0.07 + Math.Sin(x * 0.05 - y * 0.3) * 0.06;
            return 0.36 + lumps + (y - 104) / 46.0 * 0.04;
        });
        // Turned-back sheet and the dent where he slept.
        s.FillPolygon(new[] { (198.0, 108.0), (282.0, 108.0), (286.0, 118.0), (195.0, 118.0) }, Sheet, (x, y) => 0.45 + Math.Sin(x * 0.3) * 0.05);
        // Pillow.
        for (int y = 95; y < 112; y++)
            for (int x = 200; x < 254; x++)
            {
                double dx = (x - 227) / 25.0, dy = (y - 103) / 7.5;
                double r = Math.Pow(Math.Abs(dx), 3) + Math.Pow(Math.Abs(dy), 3); // squircle
                if (r > 1) continue;
                double dent = Math.Exp(-Math.Pow((x - 222) / 7.0, 2) - Math.Pow((y - 103) / 3.0, 2)) * 0.2;
                s.Paint(x, y, Sheet, 0.6 - dy * 0.12 - dx * 0.06 - dent - r * 0.12, 0.3);
            }
        // Blanket edge hanging over the front.
        for (int x = 176; x < 304; x++)
        {
            int hang = 150 + (int)(Math.Sin(x * 0.2) * 1.5 + 2);
            for (int y = 150; y < hang; y++) s.Paint(x, y, Blanket, 0.3, 0.3);
        }
    }

    private static void DrawNightstand(SceneCanvas s)
    {
        s.FillRect(166, 104, 192, 108, DarkWood, (x, y) => 0.5);        // top
        s.FillRect(166, 108, 192, 132, DarkWood, (x, y) => 0.3 + (Math.Abs(y - 118) < 0.6 ? -0.12 : 0)); // drawer
        s.Paint(178, 113, Frame, 0.5, 0);
        // Alarm clock, black plastic.
        s.FillRect(169, 95, 187, 104, Plastic, (x, y) => 0.4 - (y - 95) * 0.03);
        // Phone, face down.
        s.FillRect(185, 102, 191, 104, Plastic, (x, y) => 0.35);
    }

    private static void DrawFicus(SceneCanvas s)
    {
        // Pot.
        for (int y = 112; y < 130; y++)
        {
            double half = 9 - (y - 112) * 0.18;
            for (int x = (int)(150 - half); x <= 150 + half; x++)
                s.Paint(x, y, Terracotta, 0.38 - (x - 150) / half * 0.12 - (y == 112 || y == 113 ? -0.1 : 0), 0.4);
        }

        // Thin stems, then leaves that droop. Some yellowing, a few on the floor.
        var rng = new Random(32);
        var stems = new List<(double x, double y)>();
        for (int st = 0; st < 3; st++)
        {
            double x = 150 + (st - 1) * 2, y = 112;
            double lean = (st - 1) * 0.25 + 0.1;
            for (int i = 0; i < 38 - st * 6; i++)
            {
                x += lean + (rng.NextDouble() - 0.5) * 0.4;
                y -= 1;
                s.Paint((int)x, (int)y, DarkWood, 0.35, 0);
                if (i % 5 == 4) stems.Add((x, y));
            }
            stems.Add((x, y));
        }
        foreach (var (sx, sy) in stems)
        {
            int count = 2 + rng.Next(2);
            for (int l = 0; l < count; l++)
            {
                double dir = rng.NextDouble() < 0.5 ? -1 : 1;
                double sick = rng.NextDouble();
                double len = 5 + rng.NextDouble() * 3;
                for (double t = 0; t < len; t += 0.5)
                {
                    double lx = sx + dir * t;
                    double ly = sy + t * t * 0.08 + t * 0.3; // drooping
                    double width = Math.Sin(t / len * Math.PI) * 1.6;
                    for (double w = -width; w <= width; w += 0.5)
                        s.Paint((int)Math.Round(lx), (int)Math.Round(ly + w), Leaf, 0.32 + sick * 0.18 + (w < 0 ? 0.06 : 0), 0.3);
                }
            }
        }
        // Fallen leaves.
        foreach (var (fx, fy) in new[] { (138, 131), (162, 134), (171, 129) })
        {
            s.Paint(fx, fy, Leaf, 0.6, 0);
            s.Paint(fx + 1, fy, Leaf, 0.55, 0);
            s.Paint(fx + 2, fy + 1, Leaf, 0.5, 0);
        }
    }

    private static void DrawCrack(SceneCanvas s)
    {
        // "The ceiling has a crack shaped like Italy, or a leg."
        var path = new (int x, int y)[]
        {
            (196, 4), (198, 5), (199, 6), (201, 7), (202, 8), (204, 9), (205, 10), (207, 11), (209, 12),
            (211, 13), (212, 14), (212, 15), (211, 16), (213, 16), (215, 15), (216, 16), (214, 18),
            (210, 13), (208, 14), (207, 15),
        };
        foreach (var (x, y) in path) s.Paint(x, y, Ceiling, 0.02, 0);
    }

    // ── Light ──────────────────────────────────────────────────

    private static void LightTheRoom(SceneCanvas s)
    {
        // The sun comes in low from the left, through the window, and lands as a slanted
        // patch across the floor and the foot of the bed.
        var patch = new[] { (92.0, 128.0), (166.0, 128.0), (262.0, 178.0), (150.0, 178.0) };
        var beam = new[] { (WinL, WinT), (WinR, WinT), (262.0, 178.0), (150.0, 178.0), (92.0, 128.0) };

        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
            {
                var m = s.MaterialAt(x, y);
                if (m is null) continue;

                // Sunlit patch: anything on the floor plane or the bed inside the patch.
                if (SceneCanvas.InsidePolygon(patch, x + 0.5, y + 0.5) && y >= 128)
                    s.AddLight(x, y, 0.42);
                else if (SceneCanvas.InsidePolygon(patch, x + 0.5, y + 4.5) && y >= 124)
                    s.AddLight(x, y, 0.18); // soft edge

                // Dust in the air: a faint shaft from the window.
                if (SceneCanvas.InsidePolygon(beam, x + 0.5, y + 0.5) && y > WinB + 6)
                    s.AddLight(x, y, Bayer.At(x, y) < 0.35 ? 0.1 : 0.04);

                // Bounce light on the wall around the window.
                double d = Math.Sqrt(Math.Pow((x - 95) / 70.0, 2) + Math.Pow((y - 66) / 50.0, 2));
                if (d < 1 && m != Sky && m != Blocks) s.AddLight(x, y, 0.14 * (1 - d));

                // The bed's shadow on the floor.
                if (y > 166 && y < 174 && x > 178 && x < 306) s.AddLight(x, y, -0.15);
            }
    }

    private static void DrawEmissive(Canvas c)
    {
        // Alarm clock reading 06:47 in a 3x5 pixel font.
        string[] glyph0 = { "###", "#.#", "#.#", "#.#", "###" };
        string[] glyph6 = { "###", "#..", "###", "#.#", "###" };
        string[] glyph4 = { "#.#", "#.#", "###", "..#", "..#" };
        string[] glyph7 = { "###", "..#", "..#", "..#", "..#" };
        int cx = 171;
        foreach (var g in new[] { glyph0, glyph6, null, glyph4, glyph7 })
        {
            if (g is null) { c[cx, 99] = ClockRed; c[cx, 101] = ClockRed; cx += 2; continue; }
            for (int y = 0; y < 5; y++)
                for (int x = 0; x < 3; x++)
                    if (g[y][x] == '#') c[cx + x, 97 + y] = ClockRed;
            cx += 4;
        }
        // Its red glow on the wall above.
        for (int x = 169; x < 187; x++)
            if (c.IsSet(x, 93)) c[x, 93] = c[x, 93].Mix(ClockRed, 0.15);
    }
}
