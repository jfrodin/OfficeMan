using OfficeMan.ArtGen.Portraits;
using OfficeMan.ArtGen.Raster;
using OfficeMan.ArtGen.Scenes;

// Generates the art-direction test images into art/out/.
//   dotnet run --project tools/OfficeMan.ArtGen
// Every image is produced from code; nothing is loaded from disk.

string outDir = Path.Combine(FindRepoRoot(), "art", "out");
Directory.CreateDirectory(outDir);

var expressions = new[]
{
    new Expression { Name = "neutral", Smile = 0.3 },
    new Expression { Name = "laughing", Smile = 1.0, MouthOpen = 0.75, Squint = 0.75, LidOpen = 0.55, BrowRaise = 1, Blush = 0.5 },
    new Expression { Name = "hopeful", Smile = 0.35, BrowInner = 0.6, BrowRaise = 1, LookX = -0.8, LookY = -0.6, Blush = 0.7 },
    new Expression { Name = "crushed", Smile = -0.7, BrowInner = 1.0, LidOpen = 0.55, LookY = 0.7, Tears = true },
    new Expression { Name = "tipsy", Smile = 0.6, Lopsided = 0.8, MouthOpen = 0.2, LidOpen = 0.45, BrowRaise = 0.5, Blush = 1.0, LookX = 0.5 },
};

var sheet = new Canvas(MagdaPortrait.W * expressions.Length, MagdaPortrait.H);
for (int i = 0; i < expressions.Length; i++)
{
    var portrait = MagdaPortrait.Draw(expressions[i]);
    Png.Save(portrait.Scaled(4), Path.Combine(outDir, $"magda_{expressions[i].Name}.png"));
    sheet.Draw(portrait, i * MagdaPortrait.W, 0);
}
Png.Save(sheet.Scaled(3), Path.Combine(outDir, "magda_sheet.png"));
Png.Save(TeoFlat.Draw().Scaled(4), Path.Combine(outDir, "teo_flat.png"));

// Style comparison: the same expression in five styles, each shown at the same on-screen size (448×544).
var crushed = expressions.First(x => x.Name == "crushed");
var cel = MagdaPortrait.Draw(crushed, PortraitStyle.Cel);
var styles = new (string name, Canvas image)[]
{
    ("A_soft", MagdaPortrait.Draw(crushed).Scaled(4)),
    ("B_cel", cel.Scaled(4)),
    ("C_chunky", StyleFilters.Chunky(cel).Scaled(8)),
    ("D_riso", StyleFilters.Riso(MagdaPortrait.Draw(crushed, PortraitStyle.Soft with { Background = false }), 4)),
    ("E_smooth", StyleFilters.Smooth(cel)),
};
var compare = new Canvas(448 * styles.Length + 16 * (styles.Length - 1), 544);
compare.Fill(Rgb.Hex("101418"));
for (int i = 0; i < styles.Length; i++)
{
    Png.Save(styles[i].image, Path.Combine(outDir, $"style_{styles[i].name}.png"));
    compare.Draw(styles[i].image, i * (448 + 16), 0);
}
Png.Save(compare, Path.Combine(outDir, "style_comparison.png"));
Console.WriteLine($"Wrote art to {outDir}");

static string FindRepoRoot()
{
    for (var d = new DirectoryInfo(Directory.GetCurrentDirectory()); d is not null; d = d.Parent)
        if (File.Exists(Path.Combine(d.FullName, "OfficeMan.sln"))) return d.FullName;
    return Directory.GetCurrentDirectory();
}
