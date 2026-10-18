using System.Text.RegularExpressions;
using OfficeMan.Core.Content;

namespace OfficeMan.Tests;

/// <summary>The art manifest must cover everything the story refers to.</summary>
public class ArtManifestTests
{
    private static readonly string ContentDir = ContentLocator.Find();
    private static readonly ArtManifest Art = ArtManifest.Load(ContentDir);
    private static readonly string Ink = string.Join("\n",
        Directory.GetFiles(Path.Combine(ContentDir, "ink"), "*.ink").Select(File.ReadAllText));

    [Fact]
    public void EveryReferencedImageExists()
    {
        foreach (string file in Art.AllFiles())
            Assert.True(File.Exists(file), $"missing image: {file}");
    }

    [Fact]
    public void EveryPhaseHasABackdrop()
    {
        var schedule = PhaseSchedule.FromJson(File.ReadAllText(Path.Combine(ContentDir, "phases.json")));
        foreach (var phase in schedule.Phases)
            Assert.True(Art.PhaseScenes.ContainsKey(phase.Id), $"phase '{phase.Id}' has no scene in art.json");
    }

    [Fact]
    public void EverySceneTagIsKnown()
    {
        foreach (Match m in Regex.Matches(Ink, @"#\s*scene:\s*(\w+)"))
            Assert.True(Art.Scenes.ContainsKey(m.Groups[1].Value), $"unknown scene '{m.Groups[1].Value}'");
    }

    [Fact]
    public void MoodsUsedForPortraitedCharactersAreDefined()
    {
        // NAME: words ... #mood:x  (a typo here would silently fall back to neutral)
        foreach (Match m in Regex.Matches(Ink, @"^\s*([A-Z][A-Z' ]+):.*#mood:(\w+)", RegexOptions.Multiline))
        {
            string speaker = m.Groups[1].Value, mood = m.Groups[2].Value;
            if (!Art.Portraits.TryGetValue(speaker, out var set)) continue;
            Assert.True(set.Images.ContainsKey(mood) || set.Aliases.ContainsKey(mood), $"{speaker} has no '{mood}' portrait or alias");
        }
    }

    [Fact]
    public void FallbackChainEndsInSilhouette()
    {
        Assert.EndsWith("silhouette.png", Art.Portrait("NOBODY", "sad"));
        Assert.EndsWith("magda_sad.png", Art.Portrait("MAGDA", "crying"));      // alias
        Assert.EndsWith("viktor_neutral.png", Art.Portrait("VIKTOR", "bored")); // unknown mood → neutral
    }
}
