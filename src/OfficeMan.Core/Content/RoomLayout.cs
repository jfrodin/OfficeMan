using System.Text.Json;
using System.Text.Json.Nodes;

namespace OfficeMan.Core.Content;

/// <summary>
/// A walkable room: its picture, where Teo can walk, and the people and things in it. Things carry an
/// anchor that storylets point at with <c># where:</c>; a <c>when</c> condition decides if they're present.
/// Coordinates are in the room picture's native pixels; a thing's (X, Y) is where it stands on the floor.
/// </summary>
public sealed class RoomLayout
{
    public string Id { get; set; } = "";
    /// <summary>The unlit picture (albedo). Lights from <see cref="Lights"/> light it, and the people in it.</summary>
    public string Image { get; set; } = "";

    /// <summary>What glows by itself (windows, lamps, screens); black elsewhere.</summary>
    public string? Emissive { get; set; }

    /// <summary>A blurred copy of the emissive layer: the bloom around glowing things.</summary>
    public string? Glow { get; set; }

    /// <summary>Things in front of the people (transparent elsewhere).</summary>
    public string? Front { get; set; }

    /// <summary>False where Teo is part of the picture (asleep in bed) rather than a walking sprite.</summary>
    public bool ShowTeo { get; set; } = true;

    public float[] Ambient { get; set; } = { 0.45f, 0.45f, 0.45f };

    /// <summary>A wall clock's centre (x, y): its hands are drawn live and show the game's time.</summary>
    public int[]? Clock { get; set; }
    public List<RoomLight> Lights { get; set; } = new();

    /// <summary>
    /// Light that changes over the time spent here (an evening outdoors). Each step applies when its condition
    /// holds; the last one that holds wins. Ambient replaces <see cref="Ambient"/>; Emissive scales everything
    /// that glows (windows, bulbs), so lights can come on at dusk.
    /// </summary>
    public List<TimeStep> Timeline { get; set; } = new();

    public sealed class TimeStep
    {
        public string When { get; set; } = "";
        public float[]? Ambient { get; set; }
        public float Emissive { get; set; } = 1;
    }
    public Mood Look { get; set; } = new();

    /// <summary>A light: point, or a cone if <see cref="Dir"/> is set. Angles in degrees from the direction.</summary>
    public sealed class RoomLight
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Radius { get; set; } = 80;
        public float Intensity { get; set; } = 1;
        public string Color { get; set; } = "ffffff";
        public float[]? Dir { get; set; }
        public float Inner { get; set; } = 20;
        public float Outer { get; set; } = 40;

        /// <summary>Presence condition, e.g. "time &gt; 16:00".</summary>
        public string? When { get; set; }

        /// <summary>A failing bulb: dips and stutters now and then.</summary>
        public bool Flicker { get; set; }
    }

    /// <summary>The finishing pass: bloom, fog, exposure, saturation, vignette, grain.</summary>
    public sealed class Mood
    {
        public float Bloom { get; set; } = 0.8f;
        public float Fog { get; set; } = 0.1f;
        public string FogColor { get; set; } = "9a8c7a";
        public float Exposure { get; set; } = 1.15f;
        public float Saturation { get; set; } = 1f;
        public float Vignette { get; set; } = 0.42f;
        public float Grain { get; set; } = 0.035f;
    }
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

        /// <summary>
        /// Furniture drawn as a sprite so people can stand behind it and in front of it (a bar counter, a desk).
        /// A single frame, anchored at its front foot. Never interactable: put a separate thing next to it for that.
        /// </summary>
        public bool Prop { get; set; }

        /// <summary>
        /// The floor this thing takes up: half its width and its depth, from (X, Y) back. Teo walks round it.
        /// The art generator also reads it to put a shadow on the floor under the thing.
        /// </summary>
        public int[]? Block { get; set; }

        /// <summary>
        /// Where the mouse finds it: half its width and its height above (X, Y), for things bigger than a person
        /// (a door, a window, a wardrobe). Without it, about a person's width and height.
        /// </summary>
        public int[]? Hit { get; set; }

        /// <summary>A lit cigarette: offset (x, y) from the feet, facing right. Smoke rises from it.</summary>
        public float[]? Smoke { get; set; }

        /// <summary>Presence condition, e.g. "time &lt; 11:15".</summary>
        public string? When { get; set; }

        /// <summary>
        /// Something this person calls out when Teo walks near, once a visit, while they have something for him.
        /// A line in the air above them, not a conversation: Teo can go over or walk on.
        /// </summary>
        public string? Hail { get; set; }

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

    public static RoomLayout Load(string path) => Load(path, Path.GetDirectoryName(path)!);

    private static RoomLayout Load(string path, string dir) =>
        Read(path, dir).Deserialize<RoomLayout>(Options) ?? throw new FormatException($"Empty room: {path}");

    private static readonly JsonDocumentOptions DocOptions = new() { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true };

    /// <summary>
    /// A room's json. One with <c>"base": "&lt;room id&gt;"</c> is that room with changes: its own fields replace the
    /// base's, its things are added to the base's, and <c>"hide"</c> lists base things it leaves out. The same place
    /// in two states (Teo asleep in bed, Teo up and about) is then one set of furniture.
    /// </summary>
    private static JsonObject Read(string path, string dir)
    {
        var own = JsonNode.Parse(File.ReadAllText(path), documentOptions: DocOptions)?.AsObject()
            ?? throw new FormatException($"Empty room: {path}");
        if (own["base"]?.GetValue<string>() is not { } baseId) return own;

        var merged = Read(Path.Combine(dir, baseId + ".json"), dir);
        var hide = own["hide"]?.AsArray().Select(n => n!.GetValue<string>()).ToHashSet() ?? new HashSet<string>();
        var things = new JsonArray();
        foreach (var t in merged["things"]?.AsArray() ?? new JsonArray())
            if (!hide.Contains(t!["id"]!.GetValue<string>())) things.Add(t.DeepClone());
        foreach (var t in own["things"]?.AsArray() ?? new JsonArray()) things.Add(t!.DeepClone());
        foreach (var (key, value) in own)
            if (key is not ("base" or "hide" or "things")) merged[key] = value?.DeepClone();
        merged["things"] = things;
        return merged;
    }

    /// <summary>Loads every room in content/rooms, by id.</summary>
    public static Dictionary<string, RoomLayout> LoadAll(string contentDir)
    {
        string dir = Path.Combine(contentDir, "rooms");
        if (!Directory.Exists(dir)) return new();
        return Directory.GetFiles(dir, "*.json").Select(p => Load(p, dir)).ToDictionary(r => r.Id);
    }
}
