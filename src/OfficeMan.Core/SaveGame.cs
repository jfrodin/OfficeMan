using System.Text.Json;
using OfficeMan.Core.World;

namespace OfficeMan.Core;

/// <summary>A complete snapshot of one life, including the luck still to come (the seed).</summary>
public sealed class SaveGame
{
    public const int CurrentVersion = 1;

    public int Version { get; set; } = CurrentVersion;
    public WorldState State { get; set; } = new();
    public string InkState { get; set; } = "";
    public string? ActiveStorylet { get; set; }
    public List<StoryLine> LastLines { get; set; } = new();

    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public string ToJson() => JsonSerializer.Serialize(this, Options);

    public static SaveGame FromJson(string json) =>
        JsonSerializer.Deserialize<SaveGame>(json, Options) ?? throw new FormatException("Empty save file");
}
