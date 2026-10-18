using System.Text.Json;
using System.Text.Json.Serialization;

namespace OfficeMan.Core.Content;

public enum PhaseMode
{
    /// <summary>The director plays exactly one storylet, then the phase ends.</summary>
    Scene,
    /// <summary>The player picks from a menu of situations until time runs out or they leave.</summary>
    Hub,
}

/// <summary>A stretch of the day: where Teo is and how play works there.</summary>
public sealed class Phase
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public PhaseMode Mode { get; set; }

    /// <summary>Day the phase belongs to. If later than the current day, Teo sleeps until <see cref="Start"/>.</summary>
    public int Day { get; set; }

    /// <summary>Clock time the phase starts ("HH:MM"). Time jumps forward to it; never backward. Null keeps the current time.</summary>
    public string? Start { get; set; }

    /// <summary>Hub only: clock time after which the timeout storylet fires and the phase ends.</summary>
    public string? End { get; set; }

    /// <summary>Phase that follows unless content chooses another. "end" finishes the game.</summary>
    public string Next { get; set; } = "end";

    [JsonIgnore] public int? StartMinute => Start is null ? null : Storylet.ParseTime(Id, Start);
    [JsonIgnore] public int? EndMinute => End is null ? null : Storylet.ParseTime(Id, End);
}

public sealed class PhaseSchedule
{
    public const string EndPhase = "end";

    public string FirstPhase { get; set; } = "";
    public List<Phase> Phases { get; set; } = new();

    public Phase Get(string id) =>
        Phases.Find(p => p.Id == id) ?? throw new KeyNotFoundException($"Unknown phase '{id}'");

    public bool Contains(string id) => Phases.Exists(p => p.Id == id);

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
    };

    public static PhaseSchedule FromJson(string json) =>
        JsonSerializer.Deserialize<PhaseSchedule>(json, JsonOptions)
        ?? throw new FormatException("Empty phase schedule");
}
