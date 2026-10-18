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

/// <summary>Rooms must contain every anchor the story places in them.</summary>
[Collection("content")]
public class RoomTests(ContentFixture fixture)
{
    private static readonly string ContentDir = ContentLocator.Find();

    [Fact]
    public void EveryAnchorInARoomPhaseExistsInThatRoom()
    {
        var art = ArtManifest.Load(ContentDir);
        var rooms = RoomLayout.LoadAll(ContentDir);
        foreach (var s in fixture.Content.Storylets.Where(s => s.Where is not null))
            foreach (string phase in s.Phases)
            {
                string? roomId = art.RoomFor(phase);
                if (roomId is null) continue;
                Assert.True(rooms.ContainsKey(roomId), $"room '{roomId}' missing");
                Assert.True(rooms[roomId].Things.Any(t => t.AnchorId == s.Where), $"storylet '{s.Id}': no '{s.Where}' in room '{roomId}'");
            }
    }

    [Fact]
    public void HubStoryletsInRoomPhasesAllHaveAnAnchor()
    {
        var art = ArtManifest.Load(ContentDir);
        foreach (var s in fixture.Content.Storylets.Where(s => s.Kind is StoryletKind.Hub or StoryletKind.Exit))
            foreach (string phase in s.Phases.Where(p => art.RoomFor(p) is not null))
                Assert.True(s.Where is not null, $"storylet '{s.Id}' in walkable phase '{phase}' has no '# where:'");
    }

    [Fact]
    public void RoomImagesAndSpritesExist()
    {
        var art = ArtManifest.Load(ContentDir);
        foreach (var room in RoomLayout.LoadAll(ContentDir).Values)
        {
            foreach (string? layer in new[] { room.Image, room.Emissive, room.Glow, room.Front })
                if (layer is not null) Assert.True(File.Exists(art.Resolve(layer)), $"missing {layer}");
            foreach (var t in room.Things.Where(t => t.Sprite is not null))
                Assert.True(art.Sprites.ContainsKey(t.Sprite!), $"room '{room.Id}': unknown sprite '{t.Sprite}'");
        }
    }
}
