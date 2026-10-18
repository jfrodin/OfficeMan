using OfficeMan.ArtGen.Raster;

namespace OfficeMan.ArtGen.Drawn;

/// <summary>
/// Approach 3: hand-authored pixel art, 64×80. No lighting maths: shapes are placed as pixel
/// clusters with a fixed palette, shading is drawn as deliberate bands, and every facial feature
/// is a small hand-typed sprite per mood, the way pixel artists build expression sets.
/// </summary>
public static class PixelMagda
{
    public const int W = 64, H = 80;

    // One character per colour. '.' in a sprite means "leave what's there".
    private static readonly Dictionary<char, Rgb> Pal = new()
    {
        ['s'] = Rgb.Hex("fbd5b6"), ['S'] = Rgb.Hex("eeae8c"), ['z'] = Rgb.Hex("cc7d66"), ['Z'] = Rgb.Hex("8c4842"),
        ['h'] = Rgb.Hex("dc6479"), ['H'] = Rgb.Hex("a8324f"), ['j'] = Rgb.Hex("6e1b36"), ['J'] = Rgb.Hex("3c0c21"),
        ['d'] = Rgb.Hex("f05c5c"), ['D'] = Rgb.Hex("cf2f3e"), ['e'] = Rgb.Hex("8e1a2c"), ['E'] = Rgb.Hex("4c0d1a"),
        ['W'] = Rgb.Hex("f7f0e6"), ['I'] = Rgb.Hex("6a442a"), ['K'] = Rgb.Hex("241418"), ['B'] = Rgb.Hex("6b8fdc"),
        ['b'] = Rgb.Hex("a3bcf0"), ['u'] = Rgb.Hex("b62a44"), ['l'] = Rgb.Hex("e25a64"), ['k'] = Rgb.Hex("5c1220"),
        ['T'] = Rgb.Hex("f5ede0"), ['g'] = Rgb.Hex("ffe08a"), ['G'] = Rgb.Hex("d4a03c"), ['q'] = Rgb.Hex("8a6020"),
        ['R'] = Rgb.Hex("bab1b3"), ['P'] = Rgb.Hex("f28c88"), ['t'] = Rgb.Hex("cbe7f8"), ['m'] = Rgb.Hex("4c4a6a"),
        ['x'] = Rgb.Hex("ead6be"), ['y'] = Rgb.Hex("f4e4ce"),
    };

    public static Canvas Draw(Mood mood)
    {
        var c = new Canvas(W, H);
        void P(int x, int y, char k) { if (k != '.' && c.InBounds(x, y)) c[x, y] = Pal[k]; }
        void Sprite(int x0, int y0, string[] rows, bool mirror = false)
        {
            for (int y = 0; y < rows.Length; y++)
                for (int x = 0; x < rows[y].Length; x++)
                    P(mirror ? x0 + rows[y].Length - 1 - x : x0 + x, y0 + y, rows[y][x]);
        }
        bool InEllipse(int x, int y, double cx, double cy, double rx, double ry) =>
            Math.Pow((x + 0.5 - cx) / rx, 2) + Math.Pow((y + 0.5 - cy) / ry, 2) <= 1;

        // Background: flat warm wall and a pale disc behind her head.
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
                P(x, y, InEllipse(x, y, 32, 34, 29, 29) ? 'y' : 'x');

        // ── Hair: lobes of perm. Each lobe: base, a shadow crescent at lower left, a lit cluster at upper right ──
        var lobes = new (int cx, int cy, int r)[]
        {
            (32, 15, 12), (20, 19, 9), (44, 19, 9), (13, 29, 7), (51, 29, 7),
            (12, 40, 6), (52, 40, 6), (14, 49, 5), (50, 49, 5), (27, 9, 6), (38, 9, 6),
        };
        var hairMask = new bool[W, H];
        foreach (var (cx, cy, r) in lobes)
            for (int y = cy - r; y <= cy + r; y++)
                for (int x = cx - r; x <= cx + r; x++)
                {
                    if (!c.InBounds(x, y) || !InEllipse(x, y, cx + 0.5, cy + 0.5, r, r)) continue;
                    hairMask[x, y] = true;
                    bool shade = (x - cx) * -0.6 + (y - cy) * 0.8 > r * 0.45;
                    P(x, y, shade ? 'j' : 'H');
                }
        foreach (var (cx, cy, r) in lobes)
        {
            int hx = cx + r / 3, hy = cy - r / 2;
            P(hx, hy, 'h'); P(hx + 1, hy, 'h'); P(hx, hy + 1, 'h');
            if (r >= 8) { P(hx + 2, hy + 1, 'h'); P(hx + 1, hy + 1, 'h'); }
        }
        Outline(c, hairMask, Pal['J']);
        // Grey roots at the parting.
        foreach (var (x, y) in new[] { (30, 4), (31, 4), (32, 5), (33, 5), (31, 5) }) P(x, y, 'R');

        // ── Neck, shoulders, dress ──
        var bodyMask = new bool[W, H];
        for (int y = 52; y < H; y++)
            for (int x = 0; x < W; x++)
            {
                bool neck = y < 64 && x >= 25 && x <= 39;
                bool shoulders = y >= 62 && InEllipse(x, y, 32, 86, 31, 24);
                if (!neck && !shoulders) continue;
                bodyMask[x, y] = true;
                P(x, y, y < 56 || (x < 28 && y < 64) ? 'z' : 'S');
            }
        for (int y = 62; y < H; y++)
            for (int x = 0; x < W; x++)
            {
                if (!bodyMask[x, y]) continue;
                double scoop = 66 + 5 * (1 - Math.Pow((x - 32) / 11.0, 2));
                if (Math.Abs(x - 32) < 11 && y < scoop) continue;
                if (Math.Abs(x - 32) >= 11 && y < 65) continue;
                P(x, y, x < 20 || (x < 26 && y > 72) ? 'e' : 'D');
            }
        foreach (var (x, y) in new[] { (47, 66), (48, 66), (49, 67), (46, 67), (50, 68) }) P(x, y, 'd');
        Outline(c, bodyMask, Pal['E']);
        // Necklace and a little gold heart.
        foreach (var (x, y) in new[] { (26, 59), (27, 61), (28, 62), (29, 63), (30, 64), (34, 64), (35, 63), (36, 62), (37, 61), (38, 59) }) P(x, y, 'G');
        Sprite(31, 64, new[] { "G.G", "GgG", ".G." });

        // ── Face: a soft pear ──
        var faceMask = new bool[W, H];
        for (int y = 22; y < 60; y++)
            for (int x = 14; x < 50; x++)
                if (InEllipse(x, y, 32, 39, 14, 17) || InEllipse(x, y, 32, 47, 15.5, 10)) faceMask[x, y] = true;
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
            {
                if (!faceMask[x, y]) continue;
                bool farEdge = !faceMask[x - 2, y] || !faceMask[x - 3, y];   // shadow band down the far side
                bool underChin = y >= 55;
                P(x, y, farEdge || underChin ? 'z' : 'S');
            }
        Outline(c, faceMask, Pal['Z'], onlyOver: hairMask, alsoBottom: true);
        // Double chin.
        foreach (int x in Enumerable.Range(27, 11)) P(x, 59, 'z');
        P(26, 58, 'z'); P(38, 58, 'z');

        // ── Fringe: smaller lobes over the forehead, swept to her right ──
        var fringe = new (int cx, int cy, int r)[] { (20, 25, 3), (26, 22, 4), (33, 21, 4), (40, 22, 4), (45, 25, 3) };
        foreach (var (cx, cy, r) in fringe)
            for (int y = cy - r; y <= cy + r; y++)
                for (int x = cx - r; x <= cx + r; x++)
                    if (InEllipse(x, y, cx + 0.5, cy + 0.5, r, r)) P(x, y, y >= cy + r - 1 ? 'j' : 'H');
        foreach (var (cx, cy, _) in fringe) { P(cx + 1, cy - 2, 'h'); P(cx + 2, cy - 2, 'h'); }
        foreach (var (cx, cy, r) in fringe)
            for (int x = cx - r + 1; x <= cx + r - 1; x++)
                if (faceMask[x, cy + r + 1]) P(x, cy + r + 1, 'z'); // shadow on the forehead

        // ── Hoops ──
        Sprite(46, 48, new[] { ".GG.", "G..G", "G..g", "G..G", ".GG." });
        Sprite(15, 49, new[] { "G..", "G..", ".GG" });

        // ── Features, per mood: hand-typed sprites ──
        DrawFace(mood, Sprite, P);
        return c;
    }

    private static void DrawFace(Mood mood, Action<int, int, string[], bool> sprite, Action<int, int, char> p)
    {
        // Blue eyeshadow sits above every version of the eye.
        string[] shadow = { "..bBBB...", ".BBBBBB.." };
        sprite(35, 33, shadow, false);
        sprite(20, 33, shadow, true);

        string[] eye = mood switch
        {
            Mood.Happy => new[]
            {
                "..KKKK...",
                ".K....KK.",
                "K.......K",
                "..zzzz...",
            },
            Mood.Irritated => new[]
            {
                "KKKKKKKKK",
                ".KKKKKKK.",
                ".WWWIKIW.",
                "..zzzzz..",
            },
            _ => new[]
            {
                ".KKKKKKKK",
                ".WWWWWWW.",
                ".WWIKIWW.",
                "..tWIWt..",
                "...zzz...",
            },
        };
        sprite(35, 35, eye, false);
        sprite(20, 35, eye, true);

        string[] brow = mood switch
        {
            Mood.Happy => new[] { "...ZZZ...", "..Z...Z..", "........." },
            Mood.Irritated => new[] { ".......ZZ", "....ZZZ..", "..ZZ....." },
            _ => new[] { "ZZ.......", "..ZZZ....", ".....ZZ..", "........." },
        };
        // The near brow is the cross one when she's irritated; the far one goes up, sceptical.
        sprite(35, mood == Mood.Sad ? 29 : 30, brow, false);
        if (mood == Mood.Irritated) sprite(20, 29, new[] { "...ZZZ...", "..Z...ZZ." }, true);
        else sprite(20, mood == Mood.Sad ? 29 : 30, brow, true);

        // Nose: a few deliberate pixels.
        p(31, 42, 'z'); p(31, 43, 'z'); p(30, 44, 'z'); p(31, 45, 'Z'); p(32, 45, 'z'); p(33, 44, 's');

        // Cheeks.
        int cheek = mood == Mood.Happy ? 44 : 45;
        foreach (int cx in new[] { 20, 41 })
        {
            p(cx, cheek, 'P'); p(cx + 1, cheek, 'P'); p(cx + 2, cheek, 'P'); p(cx + 1, cheek + 1, 'P');
        }

        string[] mouth = mood switch
        {
            Mood.Happy => new[]
            {
                "u.......u",
                ".uuuuuuu.",
                ".kTTTTTk.",
                "..kkkkk..",
                "...lll...",
            },
            Mood.Irritated => new[]
            {
                ".........",
                "....uuu.z",
                "....kkkkz",
                ".....ll..",
            },
            _ => new[]
            {
                ".........",
                "...uuu...",
                "..kkkkk..",
                ".k.lll.k.",
                "...lll...",
            },
        };
        sprite(28, 48, mouth, false);

        if (mood == Mood.Sad)
        {
            // A tear and the mascara it carries.
            foreach (var (x, y) in new[] { (41, 40), (41, 41), (42, 42), (41, 43) }) p(x, y, 'm');
            p(41, 44, 't');
        }
    }

    /// <summary>Outlines a mask from outside. Optionally only where it borders other regions, and along its bottom.</summary>
    private static void Outline(Canvas c, bool[,] mask, Rgb color, bool[,]? onlyOver = null, bool alsoBottom = false)
    {
        var edge = new List<(int, int)>();
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
            {
                if (mask[x, y]) continue;
                bool next = (x > 0 && mask[x - 1, y]) || (x < W - 1 && mask[x + 1, y]) || (y > 0 && mask[x, y - 1]) || (y < H - 1 && mask[x, y + 1]);
                if (!next) continue;
                bool below = y > 0 && mask[x, y - 1];
                if (onlyOver is not null && !onlyOver[x, y] && !(alsoBottom && below)) continue;
                edge.Add((x, y));
            }
        foreach (var (x, y) in edge) c[x, y] = color;
    }
}
