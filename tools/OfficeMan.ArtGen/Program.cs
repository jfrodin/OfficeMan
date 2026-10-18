using OfficeMan.ArtGen.Drawn;
using OfficeMan.ArtGen.Drawn.Cast;
using OfficeMan.ArtGen.Portraits;
using OfficeMan.ArtGen.Raster;
using OfficeMan.ArtGen.Scenes;

// Generates art into art/out/. Every image is produced from code; nothing is loaded from disk.
//   dotnet run --project tools/OfficeMan.ArtGen              everything
//   dotnet run --project tools/OfficeMan.ArtGen -- cast      only the named set(s): pixel, styles, drawn, construct, cast, game
// The "game" set writes the temporary in-game art to art/game/ (committed; referenced by content/art.json).

string outDir = Path.Combine(FindRepoRoot(), "art", "out");
Directory.CreateDirectory(outDir);
var sets = args.Length == 0 ? new HashSet<string> { "pixel", "styles", "drawn", "construct", "cast", "game" } : args.ToHashSet();
var dark = Rgb.Hex("101418");

var expressions = new[]
{
    new Expression { Name = "neutral", Smile = 0.3 },
    new Expression { Name = "laughing", Smile = 1.0, MouthOpen = 0.75, Squint = 0.75, LidOpen = 0.55, BrowRaise = 1, Blush = 0.5 },
    new Expression { Name = "hopeful", Smile = 0.35, BrowInner = 0.6, BrowRaise = 1, LookX = -0.8, LookY = -0.6, Blush = 0.7 },
    new Expression { Name = "crushed", Smile = -0.7, BrowInner = 1.0, LidOpen = 0.55, LookY = 0.7, Tears = true },
    new Expression { Name = "tipsy", Smile = 0.6, Lopsided = 0.8, MouthOpen = 0.2, LidOpen = 0.45, BrowRaise = 0.5, Blush = 1.0, LookX = 0.5 },
};
var crushed = expressions.First(x => x.Name == "crushed");

// Art test v1: soft-lit pixel portrait and Teo's flat.
if (sets.Contains("pixel"))
{
    var sheet = new Canvas(MagdaPortrait.W * expressions.Length, MagdaPortrait.H);
    for (int i = 0; i < expressions.Length; i++)
    {
        var portrait = MagdaPortrait.Draw(expressions[i]);
        Png.Save(portrait.Scaled(4), Path.Combine(outDir, $"magda_{expressions[i].Name}.png"));
        sheet.Draw(portrait, i * MagdaPortrait.W, 0);
    }
    Png.Save(sheet.Scaled(3), Path.Combine(outDir, "magda_sheet.png"));
    Png.Save(TeoFlat.Draw().Scaled(4), Path.Combine(outDir, "teo_flat.png"));
}

// Finishes applied to the pixel portrait (superseded; kept for reference).
if (sets.Contains("styles"))
{
    var cel = MagdaPortrait.Draw(crushed, PortraitStyle.Cel);
    Row("style_comparison", new[]
    {
        MagdaPortrait.Draw(crushed).Scaled(4),
        cel.Scaled(4),
        StyleFilters.Chunky(cel).Scaled(8),
        StyleFilters.Riso(MagdaPortrait.Draw(crushed, PortraitStyle.Soft with { Background = false }), 4),
        StyleFilters.Smooth(cel),
    }, 448, 544);
}

// One authored drawing rendered three ways (superseded; kept for reference).
if (sets.Contains("drawn"))
{
    var drawing = new MagdaDrawing(crushed);
    Row("drawn_comparison", new[]
    {
        MagdaPortrait.Draw(crushed).Scaled(5), DrawnStyles.Flat(drawing), DrawnStyles.Ink(drawing), DrawnStyles.Pencil(drawing),
    }, 600, 760);
}

// Three ways of constructing Magda, three moods each.
if (sets.Contains("construct"))
{
    var moods = new[] { Mood.Happy, Mood.Irritated, Mood.Sad };
    Row("construct_1_animation", moods.Select(CartoonMagda.Draw).ToArray(), 600, 760);
    Row("construct_2_realism", moods.Select(RealistMagda.Draw).ToArray(), 600, 760);
    Row("construct_3_pixel", moods.Select(m => PixelMagda.Draw(m).Scaled(8)).ToArray(), 600, 760);
}

// The cast in the mature animation style: neutral above, one distinctive expression below.
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
    foreach (var member in new CastMember[] { new Viktor(), new Zdenka(), new Kornel() })
        foreach (var rig in new[] { member.Neutral, member.Distinct })
            Png.Save(member.Draw(rig), Path.Combine(portraits, $"{member.Name}_{rig.Name}.png"));
    Png.Save(Placeholders.Silhouette(), Path.Combine(portraits, "silhouette.png"));

    Png.Save(TeoFlat.Draw().Scaled(6), Path.Combine(scenes, "teo_flat.png"));
    foreach (var (name, image) in Placeholders.Scenes())
        Png.Save(image.Scaled(6), Path.Combine(scenes, $"{name}.png"));
    Console.WriteLine($"Wrote game art to {gameDir}");
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
