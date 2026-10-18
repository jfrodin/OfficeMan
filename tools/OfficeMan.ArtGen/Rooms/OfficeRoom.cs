using OfficeMan.ArtGen.Raster;
using OfficeMan.ArtGen.Scenes;

namespace OfficeMan.ArtGen.Rooms;

/// <summary>
/// The Permits Section, 480×270, side-on with a strip of floor to walk on. Morning light through
/// three windows; Viktor's glass office at the right; four desks against the back wall.
/// Positions here must match content/rooms/office.json.
/// </summary>
public static class OfficeRoom
{
    public const int W = 480, H = 270;
    private const int WallBottom = 168;

    private static readonly Ramp Wall = Ramp.Of("2e3632", "3c4640", "4c5850", "5e6a60", "727c70", "8a9282", "a6aa96");
    private static readonly Ramp Floor = Ramp.Of("2a2620", "36302a", "443c34", "52483e", "625648", "746654");
    private static readonly Ramp Wood = Ramp.Of("2a1e16", "3c2c20", "54402e", "6a5038", "806446", "9a7a56");
    private static readonly Ramp Metal = Ramp.Of("2a2c30", "3e4248", "585e66", "767c84", "9aa0a6", "c4c8cc");
    private static readonly Ramp Glass = Ramp.Of("3a4a52", "4e6670", "6a8890", "8eacb2", "b8d0d2");
    private static readonly Ramp Sky = Ramp.Of("8ea4b4", "a6bcc8", "c2d4dc", "dce8ec", "f2f6f4");
    private static readonly Ramp Paper = Ramp.Of("8a8478", "b4ae9e", "d8d2c0", "f0ecdc");
    private static readonly Ramp Balloon = Ramp.Of("7a1a30", "b02a48", "e04a64", "f48aa0");
    private static readonly Ramp BalloonB = Ramp.Of("1e3a7a", "2e56b0", "4a7ae0", "8aaaf0");
    private static readonly Ramp Plant = Ramp.Of("1a2a14", "2a401c", "3c5a26", "567a32");

    public static Canvas Draw()
    {
        var s = new SceneCanvas(W, H);

        // Back wall with a darker skirting and a chair rail.
        s.FillRect(0, 0, W, WallBottom, Wall, (x, y) => 0.5 - y / (double)WallBottom * 0.12, dither: 0.3);
        s.FillRect(0, 118, W, 121, Wall, (x, y) => 0.32);
        s.FillRect(0, WallBottom - 6, W, WallBottom, Wood, (x, y) => 0.3);

        // Fluorescent strips.
        for (int i = 0; i < 4; i++) s.FillRect(30 + i * 110, 4, 96 + i * 110, 8, Paper, (x, y) => 1.0, dither: 0);

        // Three windows: sky, the opposite building, the frames.
        foreach (int wx in new[] { 40, 128, 216 })
        {
            s.FillRect(wx, 26, wx + 64, 100, Sky, (x, y) => 0.9 - (y - 26) / 74.0 * 0.5, dither: 0.6);
            s.FillRect(wx, 70, wx + 64, 100, Wall, (x, y) => 0.25 + ((x / 6 + y / 8) % 2 == 0 ? 0.04 : 0), dither: 0.2);
            s.FillRect(wx - 2, 24, wx + 66, 26, Metal, (x, y) => 0.75, 0);
            s.FillRect(wx - 2, 100, wx + 66, 103, Metal, (x, y) => 0.65, 0);
            s.FillRect(wx + 31, 26, wx + 33, 100, Metal, (x, y) => 0.7, 0);
        }

        // The EU poster: a family on a bridge we haven't built.
        s.FillRect(300, 34, 336, 84, Paper, (x, y) => 0.85, 0);
        s.FillRect(302, 36, 334, 66, Sky, (x, y) => 0.7, 0.3);
        s.FillRect(302, 58, 334, 62, Metal, (x, y) => 0.5, 0);
        foreach (int fx in new[] { 310, 316, 322, 326 }) s.FillRect(fx, 50, fx + 2, 58, Balloon, (x, y) => 0.4, 0);
        s.FillRect(306, 70, 330, 72, BalloonB, (x, y) => 0.6, 0);
        s.FillRect(308, 75, 328, 76, Metal, (x, y) => 0.5, 0);

        // Wall clock.
        foreach (var (x, y) in Circle(356, 40, 7)) s.Paint(x, y, Paper, 0.95, 0);
        foreach (var (x, y) in Ring(356, 40, 8)) s.Paint(x, y, Metal, 0.3, 0);
        s.FillRect(356, 35, 357, 41, Metal, (x, y) => 0.1, 0);
        s.FillRect(356, 40, 360, 41, Metal, (x, y) => 0.1, 0);

        // Viktor's glass office, door open.
        s.FillRect(388, 14, W, WallBottom, Glass, (x, y) => 0.55 + (x + y) % 23 / 120.0, dither: 0.4);
        s.FillRect(388, 14, 391, WallBottom, Metal, (x, y) => 0.6, 0);
        s.FillRect(430, 14, 432, WallBottom, Metal, (x, y) => 0.6, 0);
        s.FillRect(400, 112, 470, 120, Wood, (x, y) => 0.55, 0);     // his desk
        s.FillRect(404, 120, 408, 150, Wood, (x, y) => 0.3, 0);
        s.FillRect(462, 120, 466, 150, Wood, (x, y) => 0.3, 0);
        s.FillRect(440, 40, 470, 64, Wood, (x, y) => 0.5, 0);        // the framed photo with the minister
        s.FillRect(443, 43, 467, 61, Paper, (x, y) => 0.55, 0);

        // Floor: linoleum, darker away from the windows.
        s.FillRect(0, WallBottom, W, H, Floor, (x, y) =>
        {
            double seam = (y - WallBottom) % 18 == 0 ? -0.1 : 0;
            return 0.45 - (y - WallBottom) / 102.0 * 0.12 + seam;
        }, dither: 0.3);

        // Desks against the wall: Magda, Teo, Kornel, Zdenka.
        Desk(s, 70, 132);
        Desk(s, 160, 222);
        Desk(s, 250, 312);
        Desk(s, 330, 384);

        // Magda's desk: balloons on strings, the GIFT tin.
        foreach (var (bx, by, ramp) in new[] { (82, 92, Balloon), (94, 84, BalloonB), (106, 94, Balloon), (100, 104, BalloonB) })
        {
            foreach (var (x, y) in Circle(bx, by, 5)) s.Paint(x, y, ramp, 0.45 + (x - bx + y - by) * -0.04, 0);
            for (int y = by + 5; y < 140; y++) s.Paint(bx + (y % 4 == 0 ? 1 : 0), y, Metal, 0.7, 0);
        }
        s.FillRect(112, 132, 120, 140, Metal, (x, y) => 0.8, 0);       // the tin

        // Teo's desk: a computer, a pile.
        Computer(s, 172);
        s.FillRect(200, 130, 214, 140, Paper, (x, y) => 0.6 + (y % 2) * 0.1, 0);
        // Kornel's desk: computer, everything at right angles.
        Computer(s, 262);
        s.FillRect(292, 136, 304, 140, Paper, (x, y) => 0.8, 0);
        // Zdenka's desk: one file, one ruler. Nothing else.
        s.FillRect(346, 136, 362, 140, Paper, (x, y) => 0.7, 0);
        s.FillRect(364, 138, 380, 139, Metal, (x, y) => 0.9, 0);

        // The file trolley.
        s.FillRect(234, 150, 246, 152, Metal, (x, y) => 0.6, 0);
        for (int y = 128; y < 150; y += 4) s.FillRect(232, y, 248, y + 3, Paper, (x, yy) => 0.5 + (y % 8) * 0.03, 0);
        s.FillRect(232, 152, 234, 166, Metal, (x, y) => 0.4, 0);
        s.FillRect(246, 152, 248, 166, Metal, (x, y) => 0.4, 0);

        // The door, the printer, a plant that someone else waters.
        s.FillRect(4, 58, 32, WallBottom, Wood, (x, y) => 0.4 + (x == 28 ? 0.2 : 0), 0.2);
        s.FillRect(2, 56, 34, 58, Wood, (x, y) => 0.25, 0);
        s.FillRect(26, 112, 28, 116, Metal, (x, y) => 0.9, 0);
        s.FillRect(42, 132, 62, 150, Metal, (x, y) => 0.55, 0.2);
        s.FillRect(44, 128, 60, 132, Metal, (x, y) => 0.75, 0);
        s.FillRect(140, 150, 152, 166, Wood, (x, y) => 0.45, 0);
        foreach (var (x, y) in Circle(146, 140, 8)) s.Paint(x, y, Plant, 0.35 + ((x + y) % 3) * 0.08, 0);

        // Morning light falling from the windows onto the floor.
        foreach (int wx in new[] { 40, 128, 216 })
        {
            var patch = new[] { (wx + 20.0, (double)WallBottom), (wx + 84.0, (double)WallBottom), (wx + 120.0, 236.0), (wx + 56.0, 236.0) };
            SceneCanvas.ForEachInPolygon(patch, (x, y) => s.AddLight(x, y, 0.14));
        }
        return s.Render();
    }

    private static void Desk(SceneCanvas s, int x0, int x1)
    {
        s.FillRect(x0, 140, x1, 146, Wood, (x, y) => y == 140 ? 0.7 : 0.5, 0.2);
        s.FillRect(x0 + 2, 146, x1 - 2, 166, Wood, (x, y) => 0.32 + (x == x0 + 2 ? -0.08 : 0), 0.2);
        s.FillRect(x0 + 6, 152, x1 - 6, 153, Wood, (x, y) => 0.2, 0);
    }

    private static void Computer(SceneCanvas s, int x0)
    {
        s.FillRect(x0, 116, x0 + 22, 134, Metal, (x, y) => 0.7, 0);
        s.FillRect(x0 + 2, 118, x0 + 20, 132, Glass, (x, y) => 0.5 + (y % 3 == 0 ? 0.08 : 0), 0);
        s.FillRect(x0 + 9, 134, x0 + 13, 140, Metal, (x, y) => 0.5, 0);
        s.FillRect(x0 - 2, 138, x0 + 24, 140, Metal, (x, y) => 0.8, 0);
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
