using System.Text.Json;

namespace OfficeMan.Core.Content;

/// <summary>
/// A walkable room: its picture, where Teo can walk, and the people and things in it. Things carry an
/// anchor that storylets point at with <c># where:</c>; a <c>when</c> condition decides if they're present.
/// Coordinates are in the room picture's native pixels; a thing's (X, Y) is where it stands on the floor.
/// </summary>
public sealed class RoomLayout
{
    public string Id { get; set; } = "";
    public string Image { get; set; } = "";
    public int Width { get; set; } = 480;
    public int Height { get; set; } = 270;
    public WalkArea Walk { get; set; } = new();
    public int[] Spawn { get; set; } = { 40, 220 };
    public List<RoomThing> Things { get; set; } = new();

    public sealed class WalkArea
    {
        public int MinX { get; set; }
        public int MaxX { get; set; }
        public int MinY { get; set; }
        public int MaxY { get; set; }
    }

    public sealed class RoomThing
    {
        public string Id { get; set; } = "";

        /// <summary>The storylet anchor this thing offers; defaults to its id.</summary>
        public string? Anchor { get; set; }

        /// <summary>Shown in the interaction prompt ("Magda", "The file trolley").</summary>
        public string Name { get; set; } = "";

        /// <summary>A sprite sheet name for people; null for things that are part of the picture.</summary>
        public string? Sprite { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        /// <summary>On the wall: reachable from anywhere along the floor below it.</summary>
        public bool Wall { get; set; }

        /// <summary>Facing left instead of right.</summary>
        public bool FaceLeft { get; set; }

        /// <summary>Presence condition, e.g. "time &lt; 11:15".</summary>
        public string? When { get; set; }

        /// <summary>What Teo thinks when he looks at it and there's nothing else to do. {clock} is replaced by the time.</summary>
        public string? Look { get; set; }

        public string AnchorId => Anchor ?? Id;
    }

    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    public static RoomLayout Load(string path) =>
        JsonSerializer.Deserialize<RoomLayout>(File.ReadAllText(path), Options) ?? throw new FormatException($"Empty room: {path}");

    /// <summary>Loads every room in content/rooms, by id.</summary>
    public static Dictionary<string, RoomLayout> LoadAll(string contentDir)
    {
        string dir = Path.Combine(contentDir, "rooms");
        if (!Directory.Exists(dir)) return new();
        return Directory.GetFiles(dir, "*.json").Select(Load).ToDictionary(r => r.Id);
    }
}
