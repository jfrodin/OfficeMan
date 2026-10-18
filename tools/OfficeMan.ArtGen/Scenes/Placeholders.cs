using OfficeMan.ArtGen.Drawn;
using OfficeMan.ArtGen.Raster;

namespace OfficeMan.ArtGen.Scenes;

/// <summary>
/// Temporary location backdrops (320×180) and a generic portrait silhouette. Deliberately simple:
/// they set a mood and a place until real environment art exists.
/// </summary>
public static class Placeholders
{
    public const int W = 320, H = 180;

    public static IEnumerable<(string name, Canvas image)> Scenes()
    {
        yield return ("office", Office());
        yield return ("terrace", Terrace());
        yield return ("club", Club());
        yield return ("river", River());
        yield return ("stairwell", Stairwell());
        yield return ("estate_night", Estate());
        yield return ("car_night", Car());
        yield return ("station", Station());
        yield return ("magda_flat", SmallFlat("f2d8c0", "b0907c", "f6c6d0"));
        yield return ("kornel_flat", SmallFlat("e4e2d0", "9c9a88", "b8cde0"));
    }

    private static Canvas Gradient(string top, string bottom, double dither = 0.8)
    {
        var ramp = new Ramp(Enumerable.Range(0, 8).Select(i => Rgb.Hex(top).Mix(Rgb.Hex(bottom), i / 7.0)).ToArray());
        var c = new Canvas(W, H);
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
                c[x, y] = ramp.At(y / (double)H, x, y, dither);
        return c;
    }

    private static void Rect(Canvas c, int x0, int y0, int x1, int y1, Rgb col)
    {
        for (int y = Math.Max(0, y0); y < Math.Min(H, y1); y++)
            for (int x = Math.Max(0, x0); x < Math.Min(W, x1); x++)
                c[x, y] = col;
    }

    private static void Glow(Canvas c, double cx, double cy, double r, Rgb col, double strength)
    {
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
            {
                double d = Math.Sqrt((x - cx) * (x - cx) + (y - cy) * (y - cy)) / r;
                if (d >= 1) continue;
                double a = (1 - d) * strength;
                if (a > Bayer.At(x, y) * 0.9) c[x, y] = c[x, y].Mix(col, Math.Min(0.6, a));
            }
    }

    private static Canvas Office()
    {
        var c = Gradient("6d7a72", "3b4440");
        for (int i = 0; i < 4; i++) Rect(c, 20 + i * 76, 8, 70 + i * 76, 11, Rgb.Hex("e8f0e0"));   // fluorescent strips
        for (int i = 0; i < 3; i++) Rect(c, 34 + i * 100, 36, 104 + i * 100, 92, Rgb.Hex("9fb4c0")); // windows
        for (int i = 0; i < 3; i++) Rect(c, 34 + i * 100, 62, 104 + i * 100, 64, Rgb.Hex("6d7a72"));
        Rect(c, 0, 120, W, H, Rgb.Hex("4a4238"));                                                    // floor
        for (int i = 0; i < 5; i++) Rect(c, 10 + i * 64, 112, 58 + i * 64, 124, Rgb.Hex("7a6a52"));  // desks
        return c;
    }

    private static Canvas Terrace()
    {
        var c = Gradient("f2b07a", "4a6a74");
        Glow(c, 230, 92, 60, Rgb.Hex("ffd9a0"), 0.9);                                               // low sun
        Rect(c, 0, 104, W, 132, Rgb.Hex("3e5e66"));                                                  // river
        for (int i = 0; i < 40; i++) c[(i * 37) % W, 108 + (i * 13) % 20] = Rgb.Hex("f6d0a0");      // glints
        Rect(c, 0, 132, W, H, Rgb.Hex("6a4a36"));                                                    // decking
        for (int x = 0; x < W; x += 9) c[x, 30 + (int)(Math.Sin(x * 0.05) * 6)] = Rgb.Hex("fff0b0"); // string lights
        return c;
    }

    private static Canvas Club()
    {
        var c = Gradient("120c22", "05040a");
        Glow(c, 70, 60, 70, Rgb.Hex("3a5aff"), 0.8);
        Glow(c, 250, 50, 80, Rgb.Hex("ff3aa8"), 0.7);
        Glow(c, 160, 140, 90, Rgb.Hex("6a2aff"), 0.5);
        return c;
    }

    private static Canvas River()
    {
        var c = Gradient("0e1626", "050a12");
        Rect(c, 0, 96, W, H, Rgb.Hex("0a1420"));
        for (int i = 0; i < 9; i++)
        {
            int x = 20 + i * 34;
            Rect(c, x, 70 - i % 3 * 8, x + 2, 96, Rgb.Hex("2a3040"));
            for (int y = 100; y < 170; y += 6) c[x + (y / 6) % 3, y] = Rgb.Hex("e8b860");            // lamp reflections
            c[x, 68 - i % 3 * 8] = Rgb.Hex("ffd890");
        }
        return c;
    }

    private static Canvas Stairwell()
    {
        var c = Gradient("6a6450", "2a2620");
        Glow(c, 160, 20, 90, Rgb.Hex("f0e0a0"), 0.6);
        for (int i = 0; i < 8; i++) Rect(c, 60 + i * 14, 120 - i * 10, 280, 122 - i * 10, Rgb.Hex("3a3428"));
        Rect(c, 20, 40, 50, 120, Rgb.Hex("5a4030"));                                                 // a door
        return c;
    }

    private static Canvas Estate()
    {
        var c = Gradient("1a1a2a", "0a0a12");
        for (int b = 0; b < 4; b++)
        {
            int x0 = 10 + b * 80, top = 20 + b % 2 * 24;
            Rect(c, x0, top, x0 + 60, H, Rgb.Hex("23232e"));
            for (int y = top + 6; y < H - 10; y += 10)
                for (int x = x0 + 6; x < x0 + 56; x += 10)
                    if ((x * 7 + y * 13) % 5 == 0) Rect(c, x, y, x + 5, y + 5, Rgb.Hex("e8b860"));
        }
        Glow(c, 160, 170, 90, Rgb.Hex("ff9a40"), 0.5);                                              // sodium lamp
        return c;
    }

    private static Canvas Car()
    {
        var c = Gradient("10141c", "05070a");
        Rect(c, 40, 30, 280, 100, Rgb.Hex("1a2230"));                                                // windscreen
        Glow(c, 230, 60, 50, Rgb.Hex("ffb060"), 0.6);
        Rect(c, 0, 120, W, H, Rgb.Hex("2a1e18"));                                                    // leather
        return c;
    }

    private static Canvas Station()
    {
        var c = Gradient("c8d4c8", "8a9a90");
        for (int y = 0; y < H; y += 12) Rect(c, 0, y, W, y + 1, Rgb.Hex("a8b8ac"));                 // tiles
        for (int x = 0; x < W; x += 12) Rect(c, x, 0, x + 1, H, Rgb.Hex("a8b8ac"));
        for (int i = 0; i < 4; i++) Rect(c, 20 + i * 76, 110, 80 + i * 76, 140, Rgb.Hex("5a6a74"));  // beds
        return c;
    }

    private static Canvas SmallFlat(string wall, string floor, string accent)
    {
        var c = Gradient(wall, floor);
        Glow(c, 80, 40, 80, Rgb.Hex("fff4dc"), 0.7);                                               // morning window light
        Rect(c, 40, 30, 110, 90, Rgb.Hex("dcecf4"));                                                 // window
        Rect(c, 74, 30, 76, 90, Rgb.Hex(wall));
        Rect(c, 160, 100, 290, 140, Rgb.Hex(accent));                                                // sofa
        Rect(c, 160, 92, 290, 102, Rgb.Hex(floor));
        return c;
    }

    /// <summary>A neutral portrait card for characters without art yet.</summary>
    public static Canvas Silhouette()
    {
        var s = new Sketch(600, 760, scale: 2);
        s.Clear((x, y) => Rgb.Hex("5a5e66").Mix(Rgb.Hex("2e3036"), Math.Clamp(y / 760.0, 0, 1)));
        s.Fill(Shapes.Ellipse(300, 330, 110, 140), Rgb.Hex("23252a"));
        s.Fill(PathBuilder.From(240, 440).L(360, 440).L(372, 560).C(460, 590, 540, 620, 580, 700).L(580, 760).L(20, 760).L(20, 700)
            .C(60, 620, 140, 590, 228, 560).Points, Rgb.Hex("23252a"));
        return s.ToCanvas();
    }
}
