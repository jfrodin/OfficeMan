using System.Text.Json;

namespace OfficeMan.Core.Content;

/// <summary>
/// Maps what the story talks about (speakers, moods, scenes) to image files. The only place that
/// knows about artwork: replacing art never touches dialogue or engine code.
/// </summary>
public sealed class ArtManifest
{
    public string Silhouette { get; set; } = "";
    public Dictionary<string, PortraitSet> Portraits { get; set; } = new();
    public Dictionary<string, string> Scenes { get; set; } = new();
    public Dictionary<string, string> PhaseScenes { get; set; } = new();
    public string DefaultScene { get; set; } = "";

    /// <summary>Repository root that relative paths resolve against.</summary>
    public string Root { get; private set; } = "";

    public sealed class PortraitSet
    {
        public Dictionary<string, string> Images { get; set; } = new();
        public Dictionary<string, string> Aliases { get; set; } = new();
    }

    public static ArtManifest Load(string contentDir)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true,
        };
        var manifest = JsonSerializer.Deserialize<ArtManifest>(File.ReadAllText(Path.Combine(contentDir, "art.json")), options)
                       ?? throw new FormatException("Empty art manifest");
        manifest.Root = Path.GetFullPath(Path.Combine(contentDir, ".."));
        return manifest;
    }

    /// <summary>True if this speaker has their own portraits (otherwise they get the silhouette).</summary>
    public bool HasPortraits(string speaker) => Portraits.ContainsKey(speaker);

    /// <summary>Absolute path of the best portrait: mood → alias → neutral → first image → silhouette.</summary>
    public string Portrait(string speaker, string? mood)
    {
        if (!Portraits.TryGetValue(speaker, out var set) || set.Images.Count == 0) return Abs(Silhouette);
        string m = mood ?? "neutral";
        if (set.Images.TryGetValue(m, out var file)) return Abs(file);
        if (set.Aliases.TryGetValue(m, out var alias) && set.Images.TryGetValue(alias, out file)) return Abs(file);
        if (set.Images.TryGetValue("neutral", out file)) return Abs(file);
        if (set.Aliases.TryGetValue("neutral", out alias) && set.Images.TryGetValue(alias, out file)) return Abs(file);
        return Abs(set.Images.Values.First());
    }

    /// <summary>Absolute path of the backdrop: an explicit scene tag wins, then the phase's scene, then the default.</summary>
    public string Scene(string phaseId, string? sceneTag = null)
    {
        string id = sceneTag ?? (PhaseScenes.TryGetValue(phaseId, out var s) ? s : DefaultScene);
        return Abs(Scenes.TryGetValue(id, out var file) ? file : Scenes[DefaultScene]);
    }

    /// <summary>Every image file the manifest refers to.</summary>
    public IEnumerable<string> AllFiles() =>
        Portraits.Values.SelectMany(p => p.Images.Values).Concat(Scenes.Values).Append(Silhouette).Select(Abs);

    private string Abs(string relative) => Path.GetFullPath(Path.Combine(Root, relative));
}
