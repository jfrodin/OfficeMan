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
    public List<RoomLight> Lights { get; set; } = new();
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

        /// <summary>A lit cigarette: offset (x, y) from the feet, facing right. Smoke rises from it.</summary>
        public float[]? Smoke { get; set; }

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
