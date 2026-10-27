using OfficeMan.ArtGen.Raster;

namespace OfficeMan.ArtGen.Sprites;

/// <summary>
/// The mouse pointers, drawn as pixel art in the game's colours: paper filled, dark outline, stamp blue for detail.
/// The pointer never chooses an action; it shows what a click will do (the creator, 2026-10-27):
///   arrow  walk there      eye   look at it      hand  use it
///   talk   talk to them    exit  leave this way
/// Each is a 16×16 grid drawn at ×2. Hotspots are in content/art.json's "cursors" (game pixels, ×2 already).
/// </summary>
public static class Cursors
{
    public const int Scale = 2;

    // '#' outline, 'o' paper, 'b' stamp blue, ' ' transparent.
    private static readonly Dictionary<string, string[]> Grids = new()
    {
        ["arrow"] = new[]
        {
            "#               ",
            "##              ",
            "#o#             ",
            "#oo#            ",
            "#ooo#           ",
            "#oooo#          ",
            "#ooooo#         ",
            "#oooooo#        ",
            "#ooooooo#       ",
            "#oooooooo#      ",
            "#ooooo#####     ",
            "#oo#oo#         ",
            "#o# #oo#        ",
            "##  #oo#        ",
            "     #oo#       ",
            "     ###        ",
        },
        ["hand"] = new[]
        {
            "     ##         ",
            "    #oo#        ",
            "    #oo#        ",
            "    #oo#        ",
            "    #oo###      ",
            "    #oo#oo##    ",
            " ## #oo#oo#o#   ",
            "#oo##oo#oo#o##  ",
            "#ooo#oooooo#o#  ",
            " #oooooooooooo# ",
            "  #ooooooooooo# ",
            "  #oooooooooo#  ",
            "   #ooooooooo#  ",
            "    #ooooooo#   ",
            "    #ooooooo#   ",
            "    #########   ",
        },
        ["eye"] = new[]
        {
            "                ",
            "                ",
            "                ",
            "     ######     ",
            "   ##oooooo##   ",
            "  #ooobbbbooo#  ",
            " #ooobb##bbooo# ",
            "#ooooob##booooo#",
            " #ooobb##bbooo# ",
            "  #ooobbbbooo#  ",
            "   ##oooooo##   ",
            "     ######     ",
            "                ",
            "                ",
            "                ",
            "                ",
        },
        ["talk"] = new[]
        {
            "                ",
            "  ###########   ",
            " #ooooooooooo#  ",
            "#ooooooooooooo# ",
            "#ooooooooooooo# ",
            "#oobboobboobbo# ",
            "#oobboobboobbo# ",
            "#ooooooooooooo# ",
            "#ooooooooooooo# ",
            " #ooooooooooo#  ",
            "  ###oo#######  ",
            "    #oo#        ",
            "    #o#         ",
            "    ##          ",
            "                ",
            "                ",
        },
        ["exit"] = new[]
        {
            "                ",
            "       #        ",
            "       ##       ",
            "       #o#      ",
            "       #oo#     ",
            "########ooo#    ",
            "#ooooooooooo#   ",
            "#oooooooooooo#  ",
            "#ooooooooooo#   ",
            "########ooo#    ",
            "       #oo#     ",
            "       #o#      ",
            "       ##       ",
            "       #        ",
            "                ",
            "                ",
        },
    };

    public static IEnumerable<string> Names => Grids.Keys;

    public static Canvas Draw(string name)
    {
        var grid = Grids[name];
        var c = new Canvas(16 * Scale, 16 * Scale);
        var colours = new Dictionary<char, Rgb> { ['#'] = Rgb.Hex("1a1612"), ['o'] = Rgb.Hex("f2e6cc"), ['b'] = Rgb.Hex("2f56b0") };
        for (int y = 0; y < 16; y++)
            for (int x = 0; x < 16; x++)
                if (colours.TryGetValue(grid[y][x], out var col))
                    for (int dy = 0; dy < Scale; dy++)
                        for (int dx = 0; dx < Scale; dx++) c[x * Scale + dx, y * Scale + dy] = col;
        return c;
    }
}
