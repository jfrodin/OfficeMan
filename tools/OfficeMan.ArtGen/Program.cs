using OfficeMan.ArtGen.Drawn;
using OfficeMan.ArtGen.Drawn.Cast;
using OfficeMan.ArtGen.Portraits;
using OfficeMan.ArtGen.Raster;
using OfficeMan.ArtGen.Rooms;
using OfficeMan.ArtGen.Scenes;
using OfficeMan.ArtGen.Sprites;

// Generates the project's art from code; nothing is loaded from disk.
//   dotnet run --project tools/OfficeMan.ArtGen              the in-game art (same as "game")
//   dotnet run --project tools/OfficeMan.ArtGen -- <sets>    any of: game, sprites, cast, flat, reference
//     game       rooms, sprites, temporary portraits and scene pictures → art/game/ (committed; see content/art.json)
//     sprites    preview sheets of the pixel people → art/out/
//     logos      proposal: the game's logo, three designs → art/out/logos/
//     icons      proposal: the Windows icon, three designs → art/out/icons/
//     people     proposal: the people redrawn at high resolution → art/out/people/
//     cast       the temporary cast portraits on one sheet → art/out/
//     flat       the portrait proposal in the drawn_flat style → art/out/flat/
//     reference  re-renders the two protected references (art/reference/) for comparison → art/out/reference/

string outDir = Path.Combine(FindRepoRoot(), "art", "out");
Directory.CreateDirectory(outDir);
var sets = args.Length == 0 ? new HashSet<string> { "game" } : args.ToHashSet();
var dark = Rgb.Hex("101418");

// The two references the creator chose (art/reference/README.md). Their sources must not change; this only checks them.
if (sets.Contains("reference"))
{
    string dir = Path.Combine(outDir, "reference");
    Directory.CreateDirectory(dir);
    var crushed = new Expression { Name = "crushed", Smile = -0.7, BrowInner = 1.0, LidOpen = 0.55, LookY = 0.7, Tears = true };
    Png.Save(DrawnStyles.Flat(new MagdaDrawing(crushed)), Path.Combine(dir, "drawn_flat.png"));
    var moods = new[] { Mood.Happy, Mood.Irritated, Mood.Sad };
    Row(Path.Combine("reference", "construct_1_animation"), moods.Select(CartoonMagda.Draw).ToArray(), 600, 760);
}

// The cast in the flat style of art/reference/drawn_flat.png: a sheet, plus each portrait on its own.
if (sets.Contains("flat"))
{
    var portraits = OfficeMan.ArtGen.Drawn.Flat.FlatCast.All().ToList();
    string dir = Path.Combine(outDir, "flat");
    Directory.CreateDirectory(dir);
    foreach (var (name, mood, image) in portraits) Png.Save(image, Path.Combine(dir, $"{name}_{mood}.png"));
    const int cols = 4;
    int rows = (portraits.Count + cols - 1) / cols;
    var sheet = new Canvas(616 * cols - 16, 776 * rows - 16);
    sheet.Fill(dark);
    for (int i = 0; i < portraits.Count; i++) sheet.Draw(portraits[i].image, i % cols * 616, i / cols * 776);
    Png.Save(sheet.Scaled(1), Path.Combine(outDir, "flat_cast.png"));
}

if (sets.Contains("cast"))
{
    var cast = new CastMember[] { new Magda(), new Viktor(), new Zdenka(), new Kornel() };
    var sheet = new Canvas(616 * cast.Length - 16, 760 * 2 + 16);
    sheet.Fill(dark);
    for (int i = 0; i < cast.Length; i++)
    {
        var neutral = cast[i].Draw(cast[i].Neutral);
        var distinct = cast[i].Draw(cast[i].Distinct);
        Png.Save(neutral, Path.Combine(outDir, $"cast_{cast[i].Name}_{cast[i].Neutral.Name}.png"));
        Png.Save(distinct, Path.Combine(outDir, $"cast_{cast[i].Name}_{cast[i].Distinct.Name}.png"));
        sheet.Draw(neutral, i * 616, 0);
        sheet.Draw(distinct, i * 616, 776);
    }
    Png.Save(sheet, Path.Combine(outDir, "cast_sheet.png"));
}

// Temporary in-game art: portraits as cards, location backdrops at 1920×1080.
if (sets.Contains("game"))
{
    string gameDir = Path.Combine(FindRepoRoot(), "art", "game");
    string portraits = Path.Combine(gameDir, "portraits"), scenes = Path.Combine(gameDir, "scenes");
    Directory.CreateDirectory(portraits);
    Directory.CreateDirectory(scenes);

    foreach (var mood in new[] { Mood.Happy, Mood.Irritated, Mood.Sad })
        Png.Save(CartoonMagda.Draw(mood), Path.Combine(portraits, $"magda_{mood.ToString().ToLowerInvariant()}.png"));
    foreach (var member in new CastMember[] { new Viktor(), new Zdenka(), new Kornel(), new Stojan() })
        foreach (var rig in new[] { member.Neutral, member.Distinct })
            Png.Save(member.Draw(rig), Path.Combine(portraits, $"{member.Name}_{rig.Name}.png"));
    Png.Save(Placeholders.Silhouette(), Path.Combine(portraits, "silhouette.png"));

    // Walkable rooms (native pixel size; the game scales them) and sprite sheets.
    string rooms = Path.Combine(gameDir, "rooms"), sprites = Path.Combine(gameDir, "sprites");
    Directory.CreateDirectory(rooms);
    Directory.CreateDirectory(sprites);
    var office = OfficeRoom.Draw();
    Png.Save(office.Albedo, Path.Combine(rooms, "office.png"));
    Png.Save(office.Emissive, Path.Combine(rooms, "office_emissive.png"));
    Png.Save(office.Glow, Path.Combine(rooms, "office_glow.png"));
    Png.Save(office.Front, Path.Combine(rooms, "office_front.png"));
    foreach (var (name, layers) in new[] { ("flat", FlatRoom.Draw()), ("stairwell", StairwellRoom.Draw(night: false)), ("stairwell_night", StairwellRoom.Draw(night: true)), ("terrace", TerraceRoom.Draw()) })
    {
        Png.Save(layers.Albedo, Path.Combine(rooms, $"{name}.png"));
        Png.Save(layers.Emissive, Path.Combine(rooms, $"{name}_emissive.png"));
        Png.Save(layers.Glow, Path.Combine(rooms, $"{name}_glow.png"));
        Png.Save(layers.Front, Path.Combine(rooms, $"{name}_front.png"));
    }
    foreach (var person in PixelPeople.All)
        Png.Save(PixelPeople.Sheet(person), Path.Combine(sprites, $"{person.Name}.png"));
    Png.Save(LyingPerson.Sheet(PixelPeople.Teo), Path.Combine(sprites, "teo_bed.png"));
    // Furniture out on the floor: one frame each, placed by content/rooms/*.json.
    Png.Save(TerraceRoom.BarCounter(), Path.Combine(sprites, "terrace_bar.png"));
    Png.Save(TerraceRoom.TicketTable(), Path.Combine(sprites, "terrace_tickets.png"));
    foreach (var kind in new[] { "cloth", "drinks", "empty" })
        Png.Save(TerraceRoom.BistroTable(kind), Path.Combine(sprites, $"terrace_table_{kind}.png"));
    Png.Save(TerraceRoom.StandingTable(), Path.Combine(sprites, "terrace_standing.png"));
    foreach (var who in new[] { "magda", "teo", "kornel", "zdenka" })
        Png.Save(OfficeRoom.DeskSprite($"office_desk_{who}"), Path.Combine(sprites, $"office_desk_{who}.png"));
    Png.Save(OfficeRoom.Trolley(), Path.Combine(sprites, "office_trolley.png"));
    Png.Save(FlatRoom.Table(), Path.Combine(sprites, "flat_table.png"));
    Png.Save(FlatRoom.Armchair(), Path.Combine(sprites, "flat_armchair.png"));
    Png.Save(FlatRoom.DryingRack(), Path.Combine(sprites, "flat_rack.png"));
    foreach (var person in PixelPeople.Sitters)
        Png.Save(PixelPeople.SittingSheet(person), Path.Combine(sprites, $"{person.Name}_sit.png"));
    foreach (var (name, image) in Placeholders.Scenes())
        Png.Save(image.Scaled(6), Path.Combine(scenes, $"{name}.png"));
    Console.WriteLine($"Wrote game art to {gameDir}");
}

// Proposal: the game's logo, each on its own (transparent) and all on one sheet on the background each is meant for.
if (sets.Contains("logos"))
{
    string dir = Path.Combine(outDir, "logos");
    Directory.CreateDirectory(dir);
    var names = OfficeMan.ArtGen.Icons.Logos.Names;
    var sheet = new Canvas(OfficeMan.ArtGen.Icons.Logos.W, OfficeMan.ArtGen.Icons.Logos.H * names.Length);
    for (int i = 0; i < names.Length; i++)
    {
        var logo = OfficeMan.ArtGen.Icons.Logos.Draw(names[i]);
        Png.Save(logo, Path.Combine(dir, $"{names[i]}.png"));
        var bg = Rgb.Hex(OfficeMan.ArtGen.Icons.Logos.Background(names[i]));
        for (int y = 0; y < OfficeMan.ArtGen.Icons.Logos.H; y++)
            for (int x = 0; x < OfficeMan.ArtGen.Icons.Logos.W; x++) sheet[x, i * OfficeMan.ArtGen.Icons.Logos.H + y] = bg;
        sheet.Draw(logo, 0, i * OfficeMan.ArtGen.Icons.Logos.H);
    }
    Png.Save(sheet, Path.Combine(outDir, "logos_sheet.png"));
}

// Proposal: the Windows icon. One row per design: on a light and a dark background, at 256, 48, 32 and 16 px,
// then 32 and 16 blown up ×6 to judge the pixels.
if (sets.Contains("icons"))
{
    string dir = Path.Combine(outDir, "icons");
    Directory.CreateDirectory(dir);
    const int gap = 24, rowH = 256 + 2 * gap;
    int halfW = gap + (256 + 48 + 32 + 16 + 32 * 6 + 16 * 6) + 6 * gap;
    var sheet = new Canvas(halfW * 2, rowH * OfficeMan.ArtGen.Icons.AppIcons.Names.Length);
    for (int r = 0; r < OfficeMan.ArtGen.Icons.AppIcons.Names.Length; r++)
    {
        string name = OfficeMan.ArtGen.Icons.AppIcons.Names[r];
        var icons = OfficeMan.ArtGen.Icons.AppIcons.Sizes.ToDictionary(z => z, z => OfficeMan.ArtGen.Icons.AppIcons.Draw(name, z));
        foreach (var (z, icon) in icons) Png.Save(icon, Path.Combine(dir, $"{name}_{z}.png"));
        for (int half = 0; half < 2; half++)
        {
            var bg = Rgb.Hex(half == 0 ? "eef0f2" : "1f2226");
            for (int y = r * rowH; y < (r + 1) * rowH; y++)
                for (int x = half * halfW; x < (half + 1) * halfW; x++) sheet[x, y] = bg;
            int x0 = half * halfW + gap, mid = r * rowH + rowH / 2;
            foreach (var z in OfficeMan.ArtGen.Icons.AppIcons.Sizes)
            {
                sheet.Draw(icons[z], x0, mid - z / 2);
                x0 += z + gap;
            }
            foreach (var z in new[] { 32, 16 })
            {
                sheet.Draw(icons[z].Scaled(6), x0, mid - z * 3);
                x0 += z * 6 + gap;
            }
        }
    }
    Png.Save(sheet, Path.Combine(outDir, "icons_sheet.png"));
}

// Proposal: the people redrawn at high resolution as flat shapes, for the hybrid look (design/art_direction.md).
if (sets.Contains("people"))
{
    string dir = Path.Combine(outDir, "people");
    Directory.CreateDirectory(dir);
    var people = PixelPeople.All.ToList();
    var sheet = new Canvas(FlatPeople.FrameW * people.Count, FlatPeople.FrameH);
    sheet.Fill(Rgb.Hex("8a8478"));
    for (int i = 0; i < people.Count; i++)
    {
        var frame = FlatPeople.Frame(people[i]);
        Png.Save(frame, Path.Combine(dir, $"{people[i].Name}.png"));
        sheet.Draw(frame, i * FlatPeople.FrameW, 0);
    }
    Png.Save(sheet, Path.Combine(outDir, "people_sheet.png"));
}

// Pixel people: a preview sheet (all frames, ×8) for checking the sprites.
if (sets.Contains("sprites"))
{
    var people = PixelPeople.All.ToList();
    var preview = new Canvas(PixelPeople.FrameW * PixelPeople.Frames, PixelPeople.FrameH * people.Count);
    preview.Fill(Rgb.Hex("8a8478"));
    for (int i = 0; i < people.Count; i++) preview.Draw(PixelPeople.Sheet(people[i]), 0, i * PixelPeople.FrameH);
    Png.Save(preview.Scaled(6), Path.Combine(outDir, "sprites_preview.png"));

    // One person up close: idle and walk frames side by side, big enough to judge.
    foreach (var person in new[] { PixelPeople.Magda, PixelPeople.MagdaParty })
    {
        var close = new Canvas(PixelPeople.FrameW * 3, PixelPeople.FrameH);
        close.Fill(Rgb.Hex("8a8478"));
        for (int f = 0; f < 3; f++) close.Draw(PixelPeople.Frame(person, f * 2), f * PixelPeople.FrameW, 0);
        Png.Save(close.Scaled(10), Path.Combine(outDir, $"sprite_{person.Name}.png"));
    }

    // The office with everyone in it, to judge scale.
    var room = OfficeRoom.Draw().Albedo;
    foreach (var (person, x, y) in new[] { (PixelPeople.Magda, 96, 200), (PixelPeople.Teo, 200, 236), (PixelPeople.Kornel, 284, 204), (PixelPeople.Zdenka, 360, 202), (PixelPeople.Viktor, 440, 160) })
        room.Draw(PixelPeople.Frame(person, 0), x - PixelPeople.FrameW / 2, y - PixelPeople.FrameH);
    Png.Save(room.Scaled(4), Path.Combine(outDir, "office_preview.png"));
}

Console.WriteLine($"Wrote art to {outDir}");

void Row(string name, Canvas[] images, int cellW, int cellH)
{
    var row = new Canvas(cellW * images.Length + 16 * (images.Length - 1), cellH);
    row.Fill(dark);
    for (int i = 0; i < images.Length; i++)
        row.Draw(images[i], i * (cellW + 16) + (cellW - images[i].Width) / 2, (cellH - images[i].Height) / 2);
    Png.Save(row, Path.Combine(outDir, $"{name}.png"));
}

static string FindRepoRoot()
{
    for (var d = new DirectoryInfo(Directory.GetCurrentDirectory()); d is not null; d = d.Parent)
        if (File.Exists(Path.Combine(d.FullName, "OfficeMan.sln"))) return d.FullName;
    return Directory.GetCurrentDirectory();
}
