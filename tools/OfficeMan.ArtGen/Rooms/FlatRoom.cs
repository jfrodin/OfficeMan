using OfficeMan.ArtGen.Raster;
using OfficeMan.ArtGen.Scenes;
using static OfficeMan.ArtGen.Rooms.RoomKit;

namespace OfficeMan.ArtGen.Rooms;

/// <summary>
/// Teo's flat at dawn: a 1979 panel-block studio, side-on, to the visual standard.
/// It should say: single, tired, underpaid, not a slob but not trying. Nothing here is random.
/// Left to right: the door (jacket, shoes, unopened post), the kitchenette (dishes, noodle cups, one empty
/// bottle, a crooked cupboard door, the fridge with last month's calendar), the window (dawn over the
/// blocks opposite, the dying ficus), the bed with Teo still in it, the nightstand (06:47), the wardrobe
/// (shirts, the diploma never hung). Above the bed, a pale patch where a picture used to be.
/// Out on the floor (prop sprites, placed by content/rooms/flat.json): the table he eats at alone, its second chair
/// piled with work he brought home, the armchair bought for reading, the drying rack. In front: the laundry pile.
/// </summary>
public static class FlatRoom
{
    private const int Ceil = 20, Base = 178;

    private static readonly Ramp Paper = Ramp.Of("262a25", "323730", "40463d", "50574b", "626a5c", "767e6e", "8c937f");
    private static readonly Ramp PaperPrint = Ramp.Of("2c2e26", "3a3d32", "4a4e40", "5c604f", "70745f");
    private static readonly Ramp CeilingR = Ramp.Of("2a2c32", "34373e", "40434b", "4c4f58");
    private static readonly Ramp Parquet = Ramp.Of("1e140e", "2a1c13", "382619", "47311f", "573d27", "684a30");
    private static readonly Ramp Rug = Ramp.Of("2a1216", "3a181c", "4c2026", "602a30", "763a3c");
    private static readonly Ramp Wood = Ramp.Of("1c140e", "281c14", "36261a", "463222", "58402b", "6c5036");
    private static readonly Ramp Veneer = Ramp.Of("1e1410", "2a1c16", "38261c", "483224", "5a3f2e");
    private static readonly Ramp Cream = Ramp.Of("4a4436", "625a48", "7c735c", "968c72", "b0a688", "c8bf9e");
    private static readonly Ramp Laminate = Ramp.Of("5a5446", "746c5a", "8e866e", "a89f84", "c0b89a");
    private static readonly Ramp Metal = Ramp.Of("26282c", "393d42", "52575d", "6e747a", "8e949a", "b4b8bc");
    private static readonly Ramp Fridge = Ramp.Of("6a6656", "848070", "9e9a88", "b6b29e", "ccc8b2");
    private static readonly Ramp Sheet = Ramp.Of("4a4c56", "62646e", "7c7e88", "9a9ba2", "b8b8b8");
    private static readonly Ramp Curtain = Ramp.Of("3a2a2e", "4e3a3e", "644c50", "7a6064");
    private static readonly Ramp Leaf = Ramp.Of("12180c", "1c2612", "283618", "38441e", "4c5226", "60602c");
    private static readonly Ramp Pot = Ramp.Of("34180e", "4e2414", "6a321c", "843f24");
    private static readonly Ramp Glass = Ramp.Of("1a2a20", "24382a", "304a38", "3e5e48");
    private static readonly Ramp Jacket = Ramp.Of("14182a", "1c2238", "262e48", "323c5a");
    private static readonly Ramp Shirt = Ramp.Of("56626e", "6e7c8a", "8a98a6", "a8b4c0");
    private static readonly Ramp Envelope = Ramp.Of("8a8474", "a8a290", "c6c0ac", "dcd6c2");
    private static readonly Ramp Oilcloth = Ramp.Of("4a1a1a", "6a2626", "8a3430", "a8443c");
    private static readonly Ramp Mustard = Ramp.Of("2e2410", "45361a", "5e4a24", "786030", "8e763e");
    private static readonly Ramp Towel = Ramp.Of("2a3a4a", "3a5064", "4c6880", "60809a");

    public static RoomLayers Draw()
    {
        var a = new SceneCanvas(W, H);
        var e = new Canvas(W, H);
        var f = new Canvas(W, H);

        Shell(a);
        PropShadows(a, "flat");
        Door(a, e);
        Kitchenette(a);
        Window(a, e);
        Bed(a);
        Nightstand(a, e);
        Wardrobe(a);
        Front(f);
        return new RoomLayers(a.Render(), e, Glow(e), f);
    }

    private static void Shell(SceneCanvas a)
    {
        // Ceiling, and the crack shaped like Italy, or a leg.
        a.FillRect(0, 0, W, Ceil, CeilingR, (x, y) => 0.55 - y / 120.0, 0);
        foreach (var (x, y) in new[] { (300, 6), (301, 7), (302, 8), (304, 9), (305, 10), (307, 11), (308, 12), (310, 13), (311, 14), (313, 15),
                                       (315, 16), (316, 17), (316, 18), (315, 19), (317, 19), (319, 18), (320, 19), (318, 21), (314, 15), (312, 16) })
            a.Paint(x, y, CeilingR, 0.05, 0);
        a.FillRect(0, Ceil - 2, W, Ceil, Wood, (x, y) => 0.25, 0);   // cornice
        a.FillRect(200, Ceil, 201, 58, Metal, (x, y) => 0.3, 0);      // the cord
        a.FillRect(197, 58, 204, 61, Metal, (x, y) => 0.45, 0);       // the socket
        foreach (var (x, y) in Circle(200, 65, 4)) a.Paint(x, y, Fridge, 0.45 + (x < 200 ? 0.1 : 0), 0); // the bulb, off

        // Wallpaper: a faded green with a narrow printed stripe every 16 px. Regular, quiet.
        a.FillRect(0, Ceil, W, Base, Paper, (x, y) => 0.55 - (y - Ceil) / 600.0, 0);
        for (int x = 6; x < W; x += 16)
            for (int y = Ceil; y < Base - 6; y++)
            {
                a.Paint(x, y, PaperPrint, 0.6, 0);
                if ((y - Ceil) % 12 is 4 or 5) { a.Paint(x - 1, y, PaperPrint, 0.55, 0); a.Paint(x + 1, y, PaperPrint, 0.55, 0); }
            }
        // A seam that's started to peel, above the bed.
        for (int y = Ceil; y < Ceil + 22; y++) a.Paint(342, y, Paper, 0.85, 0);
        for (int i = 0; i < 6; i++) a.Paint(343 + i / 2, Ceil + 4 + i, Paper, 0.95, 0);

        // Skirting and parquet.
        a.FillRect(0, Base - 6, W, Base, Wood, (x, y) => y == Base - 6 ? 0.5 : 0.3, 0);
        a.FillRect(0, Base, W, H, Parquet, (x, y) =>
        {
            int row = (y - Base) / 6, board = (x + row * 37) / 44;
            double seam = (y - Base) % 6 == 0 || (x + row * 37) % 44 == 0 ? -0.06 : 0;
            double tone = (board * 7 + row * 3) % 5 * 0.012;
            double worn = Math.Max(0, 1 - Math.Abs(x - 200) / 140.0) * 0.06;      // the path from door to window
            double contact = Math.Max(0, 1 - (y - Base) / 8.0) * 0.2;
            return 0.5 + tone + seam + worn - contact;
        }, 0);
        // A cheap rag rug under the table, stripes faded where the chair goes in and out.
        a.FillRect(96, 226, 186, 254, Rug, (x, y) =>
        {
            bool edge = y < 228 || y > 251;
            double stripe = (y - 226) / 4 % 3 == 0 ? 0.1 : (y - 226) / 4 % 3 == 1 ? -0.04 : 0.03;
            return (edge ? 0.3 : 0.42) + stripe - (Math.Abs(x - 140) < 14 ? 0.05 : 0);
        }, 0);
        // A worn rug in front of the bed.
        a.FillRect(300, 186, 402, 204, Rug, (x, y) =>
        {
            bool border = y < 188 || y > 201 || x < 303 || x > 398;
            return (border ? 0.35 : 0.55) + ((x / 4 + y / 3) % 3 == 0 && !border ? 0.08 : 0) - (x > 380 ? 0.05 : 0);
        }, 0);
    }

    private static void Door(SceneCanvas a, Canvas e)
    {
        a.FillRect(12, 110, 48, Base, Wood, (x, y) => 0.35, 0);                      // frame
        a.FillRect(15, 113, 45, Base, Cream, (x, y) => 0.5 + (x == 15 ? -0.1 : 0), 0); // the door
        a.FillRect(18, 116, 42, 142, Cream, (x, y) => 0.45, 0);                      // panels
        a.FillRect(18, 146, 42, 172, Cream, (x, y) => 0.45, 0);
        a.FillRect(40, 140, 43, 143, Metal, (x, y) => 0.8, 0);                       // handle
        a.Paint(30, 124, Metal, 0.1, 0);                                             // spyhole
        a.FillRect(38, 128, 44, 129, Metal, (x, y) => 0.7, 0);                       // chain
        Emit(e, 16, Base - 1, 44, Base, Rgb.Hex("6a5030"));                          // the stair light under the door

        // Coat hooks, his one jacket, a scarf.
        a.FillRect(52, 104, 76, 106, Wood, (x, y) => 0.5, 0);
        a.FillRect(54, 106, 70, 140, Jacket, (x, y) => 0.45 + (x > 66 ? -0.12 : 0) - (y - 106) / 200.0, 0);
        a.FillRect(70, 106, 74, 132, Rug, (x, y) => 0.5, 0);
        // Shoes on the mat; post nobody has opened.
        a.FillRect(14, Base - 2, 46, Base, Rug, (x, y) => 0.3, 0);
        a.FillRect(18, Base - 6, 28, Base - 2, Veneer, (x, y) => 0.45, 0);
        a.FillRect(30, Base - 5, 40, Base - 2, Metal, (x, y) => 0.75, 0);
        foreach (var (x0, y0) in new[] { (50, Base - 3), (54, Base - 5), (58, Base - 3) })
            a.FillRect(x0, y0, x0 + 9, y0 + 3, Envelope, (x, y) => 0.6 + (x0 % 3) * 0.08, 0);
        a.FillRect(78, 124, 81, 128, Cream, (x, y) => 0.6, 0);                       // light switch
    }

    private static void Kitchenette(SceneCanvas a)
    {
        // Base units and counter.
        a.FillRect(86, 150, 170, Base, Cream, (x, y) => 0.42 + ((x - 86) % 28 == 0 ? -0.15 : 0), 0);
        for (int x = 98; x < 170; x += 28) a.FillRect(x, 158, x + 6, 159, Metal, (xx, y) => 0.6, 0);
        a.FillRect(84, 147, 172, 151, Laminate, (x, y) => y == 147 ? 0.75 : 0.5, 0);
        // Sink full of dishes.
        a.FillRect(92, 145, 120, 148, Metal, (x, y) => 0.55, 0);
        foreach (var (x0, y0, w) in new[] { (95, 140, 12), (96, 137, 10), (108, 141, 9), (110, 138, 7) })
            a.FillRect(x0, y0, x0 + w, y0 + 2, Fridge, (x, y) => 0.6 + (y0 % 3) * 0.05, 0);
        a.FillRect(116, 134, 119, 145, Metal, (x, y) => 0.75, 0);                    // tap
        // Two-ring hob, the kettle.
        a.FillRect(128, 145, 152, 147, Metal, (x, y) => 0.3, 0);
        a.FillRect(132, 136, 144, 145, Metal, (x, y) => 0.6 - (x > 140 ? 0.15 : 0), 0);
        a.FillRect(144, 138, 147, 140, Metal, (x, y) => 0.5, 0);
        // On the counter: two noodle cups, one empty bottle.
        foreach (int x0 in new[] { 154, 161 }) a.FillRect(x0, 140, x0 + 6, 147, Fridge, (x, y) => 0.7, 0);
        a.FillRect(155, 142, 160, 144, Rug, (x, y) => 0.7, 0);
        a.FillRect(124, 128, 127, 147, Glass, (x, y) => 0.6 + (x == 124 ? 0.2 : 0), 0);
        a.FillRect(125, 124, 126, 128, Glass, (x, y) => 0.5, 0);
        // Wall cupboard, one door hanging crooked.
        a.FillRect(86, 90, 168, 118, Cream, (x, y) => 0.48, 0);
        a.FillRect(127, 90, 128, 118, Cream, (x, y) => 0.25, 0);
        a.FillRect(128, 92, 132, 120, Cream, (x, y) => 0.18, 0);                      // the gap behind the crooked door
        a.FillRect(132, 92, 166, 121, Cream, (x, y) => 0.5 - (x - 132) / 200.0, 0);
        // On top of the cupboards: a moving box never unpacked, the big pot, an unlabelled bottle.
        var card = Ramp.Of("3a2a1a", "54402a", "6e5638", "8a6e48", "a08458");
        a.FillRect(92, 70, 118, 89, card, (x, y) => 0.55 - (x > 114 ? 0.15 : 0) + (y == 70 ? 0.2 : 0), 0);
        a.FillRect(92, 76, 118, 77, card, (x, y) => 0.3, 0);                            // tape
        a.FillRect(98, 81, 110, 81, Metal, (x, y) => 0.25, 0);                          // a word in marker
        a.FillRect(100, 83, 106, 83, Metal, (x, y) => 0.25, 0);
        a.FillRect(130, 79, 152, 89, Metal, (x, y) => 0.55 - (x > 148 ? 0.15 : 0) + (y == 79 ? 0.2 : 0), 0);
        a.FillRect(127, 81, 129, 82, Metal, (x, y) => 0.4, 0);
        a.FillRect(153, 81, 155, 82, Metal, (x, y) => 0.4, 0);
        a.FillRect(158, 74, 162, 89, Glass, (x, y) => 0.55 + (x == 159 ? 0.25 : 0), 0);
        a.FillRect(159, 70, 161, 73, Glass, (x, y) => 0.5, 0);
        a.FillRect(159, 69, 161, 69, Rug, (x, y) => 0.6, 0);
        // The smoke alarm on the ceiling, its battery hanging out on two wires.
        a.FillRect(56, Ceil, 64, Ceil + 2, Fridge, (x, y) => 0.6, 0);
        a.Paint(58, Ceil + 3, Metal, 0.3, 0); a.Paint(59, Ceil + 4, Metal, 0.3, 0); a.Paint(59, Ceil + 5, Metal, 0.3, 0);
        a.FillRect(58, Ceil + 6, 61, Ceil + 9, Envelope, (x, y) => 0.4, 0);
        // The fridge, with last month's calendar.
        a.FillRect(174, 120, 198, Base, Fridge, (x, y) => 0.55 - (x > 194 ? 0.12 : 0), 0);
        a.FillRect(174, 140, 198, 141, Fridge, (x, y) => 0.3, 0);
        a.FillRect(195, 126, 197, 136, Metal, (x, y) => 0.6, 0);
        a.FillRect(178, 124, 190, 136, Envelope, (x, y) => 0.75, 0);
        a.FillRect(178, 124, 190, 127, Rug, (x, y) => 0.6, 0);
        for (int y = 129; y < 135; y += 2) a.FillRect(179, y, 189, y + 1, Envelope, (x, yy) => 0.5, 0);
    }

    private static void Window(SceneCanvas a, Canvas e)
    {
        const int x0 = 216, x1 = 292, y0 = 62, y1 = 138;
        for (int y = y0; y < y1; y++)
            for (int x = x0; x < x1; x++)
            {
                double t = (y - y0) / (double)(y1 - y0);
                var sky = Rgb.Hex("2e3a6e").Mix(Rgb.Hex("b8607a"), Math.Clamp(t * 2.0 - 0.25, 0, 1)).Mix(Rgb.Hex("f0a060"), Math.Clamp(t * 2.6 - 1.2, 0, 1));
                // The blocks opposite: three slabs, low enough to leave the sunrise, a few windows lit by people up as early as Teo.
                bool block = (x < 238 && y > 112) || (x >= 238 && x < 266 && y > 104) || (x >= 266 && y > 118);
                var c = block ? Rgb.Hex("16182a") : sky;
                if (block && x % 5 == 2 && y % 6 == 3 && (x * 13 + y * 7) % 9 == 0) c = Rgb.Hex("e8b060");
                e[x, y] = c.Mix(Rgb.Hex("000000"), block ? 0.4 : 0.3);
                a.Paint(x, y, Glass, 0.25, 0);
            }
        // Frame, transom, sill, radiator.
        void Frame(int ax, int ay, int bx, int by) => a.FillRect(ax, ay, bx, by, Cream, (x, y) => 0.55, 0);
        Frame(x0 - 3, y0 - 3, x1 + 3, y0);
        Frame(x0 - 3, y1, x1 + 3, y1 + 3);
        Frame(x0 - 3, y0, x0, y1);
        Frame(x1, y0, x1 + 3, y1);
        Frame((x0 + x1) / 2 - 1, y0, (x0 + x1) / 2 + 1, y1);
        Frame(x0, 84, x1, 86);
        ClearEmissive(e, (x0 + x1) / 2 - 1, y0, (x0 + x1) / 2 + 1, y1);
        ClearEmissive(e, x0, 84, x1, 86);
        a.FillRect(x0 - 6, y1 + 3, x1 + 6, y1 + 6, Cream, (x, y) => y == y1 + 3 ? 0.7 : 0.4, 0);
        for (int x = x0; x < x1 - 4; x++)
            if ((x - x0) % 4 != 3) a.FillRect(x, 150, x + 1, 170, Cream, (xx, y) => 0.42 + (y < 152 ? 0.1 : 0), 0);
        a.FillRect(x0, 170, x1 - 4, 172, Metal, (x, y) => 0.4, 0);
        // A thin curtain pushed to one side.
        a.FillRect(x0 - 8, 58, x1 + 10, 60, Metal, (x, y) => 0.5, 0);
        for (int y = 60; y < 150; y++)
            for (int x = x1 - 6; x < x1 + 10; x++)
            {
                double fold = Math.Sin((x - x1) * 1.1) * 0.08;
                if (x < x1 - 4 + Math.Sin(y * 0.07) * 1.5) continue;
                a.Paint(x, y, Curtain, 0.5 + fold, 0);
                e.Clear(x, y);
            }
        // The ficus: thin stems, drooping leaves, some on the floor already.
        a.FillRect(300, 160, 316, Base, Pot, (x, y) => 0.5 - (x - 300) / 40.0, 0);
        a.FillRect(298, 158, 318, 161, Pot, (x, y) => 0.65, 0);
        var rng = new Random(32);
        foreach (int stem in new[] { -2, 1, 3 })
        {
            double x = 308 + stem, y = 158;
            for (int i = 0; i < 40 + stem * 3; i++)
            {
                x += stem * 0.08 + (rng.NextDouble() - 0.5) * 0.3;
                y -= 1;
                a.Paint((int)x, (int)y, Wood, 0.4, 0);
                if (i % 7 == 6)
                    foreach (int dir in new[] { -1, 1 })
                        for (int k = 0; k < 6; k++)
                            a.Paint((int)x + dir * (k + 1), (int)y + k / 2 + 1, Leaf, 0.3 + (rng.NextDouble() < 0.4 ? 0.35 : 0.1), 0);
            }
        }
        foreach (var (lx, ly) in new[] { (292, Base + 3), (322, Base + 6), (330, Base + 2) })
            a.FillRect(lx, ly, lx + 3, ly + 1, Leaf, (x, y) => 0.75, 0);
    }

    private static void Bed(SceneCanvas a)
    {
        // A single bed along the wall: frame, mattress, pillow. Teo and his blanket are a sprite (teo_bed).
        a.FillRect(334, 156, 398, Base, Wood, (x, y) => 0.38 - (y > 172 ? 0.1 : 0), 0);
        a.FillRect(336, 176, 340, Base, Wood, (x, y) => 0.2, 0);
        a.FillRect(392, 176, 396, Base, Wood, (x, y) => 0.2, 0);
        a.FillRect(396, 126, 402, Base, Wood, (x, y) => 0.45 + (y == 126 ? 0.15 : 0), 0);   // headboard
        a.FillRect(330, 152, 330 + 2, 166, Wood, (x, y) => 0.3, 0);                          // footboard
        a.FillRect(334, 150, 396, 157, Sheet, (x, y) => 0.55, 0);
        a.FillRect(378, 141, 396, 151, Sheet, (x, y) => 0.75 - (y - 141) / 40.0, 0);          // pillow
        // The pale patch where a picture used to hang, and the nail.
        a.FillRect(346, 70, 382, 102, Paper, (x, y) => 0.68, 0);
        a.Paint(364, 66, Metal, 0.8, 0);
    }

    private static void Nightstand(SceneCanvas a, Canvas e)
    {
        a.FillRect(406, 152, 428, Base, Veneer, (x, y) => 0.42 - (x > 424 ? 0.12 : 0), 0);
        a.FillRect(404, 150, 430, 153, Veneer, (x, y) => 0.6, 0);
        a.FillRect(408, 160, 426, 161, Veneer, (x, y) => 0.2, 0);
        // Alarm clock: 06:47 in red.
        a.FillRect(408, 141, 426, 150, Metal, (x, y) => 0.12, 0);
        Digits((x, y) => e[x, y] = Rgb.Hex("ff3a22"), 409, 143, "06:47");
        // A glass of water, half.
        a.FillRect(428 - 2, 144, 430, 150, Glass, (x, y) => 0.55, 0);
    }

    private static void Wardrobe(SceneCanvas a)
    {
        a.FillRect(436, 92, 474, Base, Veneer, (x, y) => 0.4 + (x == 455 ? -0.2 : 0) - (x > 470 ? 0.1 : 0), 0);
        a.FillRect(434, 90, 476, 93, Veneer, (x, y) => 0.6, 0);
        // One door open a hand's width: shirts. The good one, the other one.
        a.FillRect(456, 96, 462, 170, Veneer, (x, y) => 0.08, 0);
        a.FillRect(457, 100, 461, 140, Shirt, (x, y) => 0.5, 0);
        a.FillRect(452, 128, 454, 132, Metal, (x, y) => 0.7, 0);
        // On top: his university diploma, in its frame, never hung.
        a.FillRect(440, 82, 466, 90, Wood, (x, y) => 0.5, 0);
        a.FillRect(442, 84, 464, 89, Envelope, (x, y) => 0.6, 0);
        a.FillRect(476, Ceil, W, Base, Paper, (x, y) => 0.3, 0);                      // the corner
    }

    private static void Front(Canvas f)
    {
        // The laundry pile at the right edge, waiting for the rack to be free.
        var cloth = new[] { "1e2236", "3a4256", "5a2a2a", "2a3a30" };
        for (int i = 0; i < 6; i++)
            for (int y = 252 - i * 3; y < H; y++)
                for (int x = 446 + i * 4; x < 480 - (5 - i) * 2; x++)
                    if ((x + y + i) % 9 != 0) f[x, y] = Rgb.Hex(cloth[i % 4]);
    }

    // ── Furniture on the floor (prop sprites) ──────────────────

    /// <summary>
    /// The table he eats at, alone: oilcloth, one plate, one mug, the jar of instant coffee. His chair pushed in
    /// behind it; the second chair pulled out at the end, with a stack of files from work on the seat.
    /// Anchored at the front foot.
    /// </summary>
    public static Canvas Table()
    {
        const int w = 58, h = 46, f = h;
        var a = new SceneCanvas(w, h);
        // His chair, behind: only the back shows.
        a.FillRect(10, f - 42, 22, f - 25, Wood, (x, y) => (x == 10 ? 0.6 : 0.42) + (y == f - 42 ? 0.15 : 0), 0);
        a.FillRect(12, f - 38, 20, f - 36, Wood, (x, y) => 0.25, 0);
        // Legs, then the oilcloth: red and white checks, hanging over the edge.
        foreach (int lx in new[] { 3, 38 }) a.FillRect(lx, f - 18, lx + 3, f, Wood, (x, y) => 0.32 - (x == lx + 2 ? 0.08 : 0), 0);
        for (int y = f - 26; y < f - 18; y++)
            for (int x = 0; x < 44; x++)
            {
                bool check = (x / 4 + (y - f + 26) / 3) % 2 == 0;
                double light = y < f - 23 ? (y == f - 26 ? 0.75 : 0.62) : 0.45 - (y - f + 23) * 0.03;
                a.Paint(x, y, check ? Oilcloth : Envelope, light, 0);
            }
        int top = f - 26;
        // A plate with crumbs, the mug, the coffee jar.
        a.FillRect(6, top - 2, 20, top, Fridge, (x, y) => y == top - 2 ? 0.8 : 0.6, 0);
        a.Paint(11, top - 2, Wood, 0.6, 0);
        a.Paint(15, top - 2, Wood, 0.6, 0);
        a.FillRect(24, top - 6, 29, top, Cream, (x, y) => 0.7 - (x == 28 ? 0.15 : 0), 0);
        a.FillRect(29, top - 5, 31, top - 2, Cream, (x, y) => 0.55, 0);
        a.FillRect(33, top - 8, 39, top, Glass, (x, y) => 0.45 + (x == 34 ? 0.25 : 0), 0);
        a.FillRect(33, top - 9, 39, top - 8, Mustard, (x, y) => 0.7, 0);
        // The second chair at the end, side on: back, seat, legs, and the files.
        a.FillRect(46, f - 36, 49, f, Wood, (x, y) => 0.4 + (x == 46 ? 0.15 : 0), 0);
        a.FillRect(46, f - 16, 58, f - 13, Wood, (x, y) => y == f - 16 ? 0.6 : 0.4, 0);
        a.FillRect(55, f - 13, 58, f, Wood, (x, y) => 0.3, 0);
        for (int i = 0; i < 4; i++)
        {
            int y0 = f - 19 - i * 3;
            a.FillRect(48 + i % 2, y0, 58 - i % 2, y0 + 3, Envelope, (x, y) => 0.55 + i * 0.06 + (y == y0 ? 0.12 : 0), 0);
        }
        return a.Render();
    }

    /// <summary>The armchair he bought to read in: worn mustard, a blanket over one arm, yesterday's paper on the seat.</summary>
    public static Canvas Armchair()
    {
        const int w = 36, h = 38, f = h;
        var a = new SceneCanvas(w, h);
        a.FillRect(5, f - 36, 31, f - 14, Mustard, (x, y) => 0.5 + (y == f - 36 ? 0.18 : 0) - (y - f + 36) / 120.0 + Noise(x, y, 3) * 0.04, 0);
        a.FillRect(8, f - 34, 28, f - 16, Mustard, (x, y) => 0.42, 0);                        // the hollow a back made
        a.FillRect(6, f - 16, 30, f - 9, Mustard, (x, y) => y == f - 16 ? 0.72 : 0.56, 0);     // seat cushion
        foreach (int ax in new[] { 0, 29 })
            a.FillRect(ax, f - 24, ax + 7, f - 4, Mustard, (x, y) => (y == f - 24 ? 0.7 : 0.48) - (ax == 29 ? 0.08 : 0), 0);
        a.FillRect(2, f - 9, 34, f - 3, Mustard, (x, y) => 0.36, 0);
        foreach (int lx in new[] { 3, 30 }) a.FillRect(lx, f - 3, lx + 3, f, Wood, (x, y) => 0.3, 0);
        // The blanket over the right arm, the newspaper folded on the seat.
        a.FillRect(28, f - 26, 36, f - 6, Towel, (x, y) => 0.5 + ((x + y) % 4 == 0 ? 0.1 : 0) - (x > 34 ? 0.1 : 0), 0);
        a.FillRect(10, f - 19, 22, f - 15, Envelope, (x, y) => 0.6 + (y == f - 19 ? 0.15 : 0), 0);
        for (int x = 11; x < 21; x += 2) a.Paint(x, f - 17, Metal, 0.35, 0);
        return a.Render();
    }

    /// <summary>The drying rack: two shirts, a towel that has been drying since Thursday, socks, one of them alone.</summary>
    public static Canvas DryingRack()
    {
        const int w = 48, h = 40, f = h;
        var a = new SceneCanvas(w, h);
        // The X legs and the rails.
        for (int i = 0; i < 26; i++)
        {
            a.Paint(4 + i / 2, f - 1 - i, Metal, 0.6, 0);
            a.Paint(17 - i / 2, f - 1 - i, Metal, 0.5, 0);
            a.Paint(30 + i / 2, f - 1 - i, Metal, 0.6, 0);
            a.Paint(43 - i / 2, f - 1 - i, Metal, 0.5, 0);
        }
        a.FillRect(0, f - 27, 48, f - 26, Metal, (x, y) => 0.75, 0);
        a.FillRect(0, f - 20, 48, f - 19, Metal, (x, y) => 0.6, 0);
        // Over the top rail: a shirt, the towel, the other shirt.
        a.FillRect(2, f - 27, 14, f - 12, Shirt, (x, y) => 0.6 - (y - f + 27) / 60.0 + (x == 2 ? 0.1 : 0), 0);
        a.FillRect(16, f - 27, 30, f - 8, Towel, (x, y) => 0.55 + ((y - f) % 5 == 0 ? -0.1 : 0), 0);
        a.FillRect(32, f - 27, 44, f - 14, Shirt, (x, y) => 0.45 - (y - f + 27) / 60.0, 0);
        // Socks on the lower rail.
        foreach (var (sx, len) in new[] { (6, 7), (10, 6), (37, 8) })
            a.FillRect(sx, f - 19, sx + 3, f - 19 + len, Jacket, (x, y) => 0.55, 0);
        return a.Render();
    }
}
