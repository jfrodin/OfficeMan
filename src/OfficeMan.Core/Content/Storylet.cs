using OfficeMan.Core.Conditions;

namespace OfficeMan.Core.Content;

public enum StoryletKind
{
    /// <summary>Picked by the director in a scene phase. Exactly one plays per scene phase.</summary>
    Scene,
    /// <summary>Plays automatically when a hub phase begins.</summary>
    Enter,
    /// <summary>Offered on the hub menu.</summary>
    Hub,
    /// <summary>Offered on the hub menu; playing it ends the phase.</summary>
    Exit,
    /// <summary>Fires on its own between hub decisions (at a time, by chance, or both).</summary>
    Interrupt,
    /// <summary>Plays when a hub phase runs out of time, then ends the phase.</summary>
    Timeout,
}

/// <summary>
/// A self-contained situation: an ink knot plus the metadata in the tags at its top.
/// </summary>
/// <remarks>
/// Recognised tags (one per line, directly under the knot header):
/// <code>
/// # phase: office            phase id(s), comma-separated
/// # kind: hub                scene | enter | hub | exit | interrupt | timeout
/// # choice: Talk to Magda    hub menu text
/// # when: drunk >= 2         eligibility condition (see Condition)
/// # window: 08:30-12:00      time of day it is available
/// # at: 18:00                interrupt: earliest time it may fire
/// # chance: 30               interrupt: percent chance per hub step once eligible
/// # priority: 10             higher wins (scene choice, interrupt order, menu order)
/// # weight: 3                relative odds among equal priority (scene)
/// # duration: 20             minutes that pass when it finishes
/// # repeat                   may play any number of times
/// # max: 3                   may play up to N times (default 1)
/// </code>
/// </remarks>
public sealed class Storylet
{
    public required string Id { get; init; }
    public required IReadOnlyList<string> Phases { get; init; }
    public StoryletKind Kind { get; init; } = StoryletKind.Hub;
    public string ChoiceText { get; init; } = "";
    public Condition When { get; init; } = Condition.Always;
    public int? WindowStart { get; init; }
    public int? WindowEnd { get; init; }
    public int? At { get; init; }
    public int Chance { get; init; } = 100;
    public int Priority { get; init; }
    public int Weight { get; init; } = 1;
    public int Duration { get; init; }

    /// <summary>Maximum plays; 0 means unlimited.</summary>
    public int MaxPlays { get; init; } = 1;

    public IReadOnlyDictionary<string, string> Tags { get; init; } = new Dictionary<string, string>();

    public bool InPhase(string phaseId) => Phases.Contains(phaseId);

    public static Storylet FromTags(string id, IEnumerable<string> rawTags)
    {
        var tags = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (string raw in rawTags)
        {
            string t = raw.Trim();
            int colon = t.IndexOf(':');
            if (colon < 0) tags[t] = "";
            else tags[t[..colon].Trim()] = t[(colon + 1)..].Trim();
        }

        string Get(string key, string fallback = "") => tags.TryGetValue(key, out var v) ? v : fallback;

        var kind = Enum.TryParse<StoryletKind>(Get("kind", "hub"), ignoreCase: true, out var k)
            ? k
            : throw new FormatException($"Storylet '{id}': unknown kind '{Get("kind")}'");

        int defaultDuration = kind is StoryletKind.Hub or StoryletKind.Exit ? 20 : 0;

        int? windowStart = null, windowEnd = null;
        if (tags.TryGetValue("window", out var window))
        {
            var parts = window.Split('-', 2);
            if (parts.Length != 2) throw new FormatException($"Storylet '{id}': window must be HH:MM-HH:MM");
            windowStart = ParseTime(id, parts[0]);
            windowEnd = ParseTime(id, parts[1]);
        }

        int maxPlays = tags.ContainsKey("repeat") ? 0 : int.Parse(Get("max", "1"));

        Condition when;
        try { when = Condition.Parse(Get("when")); }
        catch (FormatException e) { throw new FormatException($"Storylet '{id}': {e.Message}", e); }

        if (!tags.TryGetValue("phase", out var phase))
            throw new FormatException($"Storylet '{id}' has no '# phase:' tag");

        return new Storylet
        {
            Id = id,
            Phases = phase.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
            Kind = kind,
            ChoiceText = Get("choice"),
            When = when,
            WindowStart = windowStart,
            WindowEnd = windowEnd,
            At = tags.TryGetValue("at", out var at) ? ParseTime(id, at) : null,
            Chance = int.Parse(Get("chance", "100")),
            Priority = int.Parse(Get("priority", "0")),
            Weight = int.Parse(Get("weight", "1")),
            Duration = int.Parse(Get("duration", defaultDuration.ToString())),
            MaxPlays = maxPlays,
            Tags = tags,
        };
    }

    /// <summary>Parses HH:MM. Hours past 24 are allowed for the small hours (e.g. 25:30).</summary>
    public static int ParseTime(string context, string text)
    {
        var parts = text.Trim().Split(':');
        if (parts.Length != 2 || !int.TryParse(parts[0], out int h) || !int.TryParse(parts[1], out int m))
            throw new FormatException($"{context}: bad time '{text}'");
        return h * 60 + m;
    }
}
