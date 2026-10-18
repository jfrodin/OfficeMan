namespace OfficeMan.Core.World;

/// <summary>
/// Everything that is true about one life at one moment. Plain data, serialised as-is into saves.
/// </summary>
public sealed class WorldState
{
    public int Seed { get; set; }

    /// <summary>Day index. 0 is the first Tuesday.</summary>
    public int Day { get; set; }

    /// <summary>Minutes since midnight of <see cref="Day"/>. May exceed 1440 for the small hours.</summary>
    public int Minute { get; set; }

    public string PhaseId { get; set; } = "";

    /// <summary>True once the current phase's entry storylets have run.</summary>
    public bool PhaseEntered { get; set; }

    /// <summary>Set by content to leave the current phase for a specific one.</summary>
    public string? NextPhase { get; set; }

    /// <summary>Counts hub decisions; used to key per-step randomness.</summary>
    public int Steps { get; set; }

    /// <summary>Named numeric facts: traits, conditions, flags, money. Absent means 0.</summary>
    public Dictionary<string, int> Qualities { get; set; } = new();

    public Dictionary<string, CharacterState> Characters { get; set; } = new();

    public Dictionary<string, int> PlayCounts { get; set; } = new();

    /// <summary>Storylet ids in the order they were played.</summary>
    public List<string> History { get; set; } = new();

    /// <summary>One-line summaries of things that mattered, for endings and the run log.</summary>
    public List<string> Headlines { get; set; } = new();

    /// <summary>How many times each check id has been rolled, so repeated checks get fresh rolls.</summary>
    public Dictionary<string, int> CheckCounts { get; set; } = new();

    /// <summary>Minutes accumulated towards the next unit of alcohol wearing off.</summary>
    public int MetabolismMinutes { get; set; }

    /// <summary>Absolute minute (day * 1440 + minute) when Teo last woke up.</summary>
    public int AwakeSince { get; set; }

    public int AbsoluteMinute => Day * 1440 + Minute;

    public int Get(string quality) => Qualities.TryGetValue(quality, out int v) ? v : 0;

    public void Set(string quality, int value)
    {
        if (value == 0) Qualities.Remove(quality);
        else Qualities[quality] = value;
    }

    public void Add(string quality, int delta) => Set(quality, Get(quality) + delta);

    public CharacterState Character(string id)
    {
        if (!Characters.TryGetValue(id, out var c))
        {
            c = new CharacterState();
            Characters[id] = c;
        }
        return c;
    }

    public int Plays(string storyletId) => PlayCounts.TryGetValue(storyletId, out int n) ? n : 0;
}

/// <summary>What one person thinks of Teo, and what they remember.</summary>
public sealed class CharacterState
{
    /// <summary>How they feel about Teo. Roughly -10 (hatred) to 10 (love).</summary>
    public int Regard { get; set; }

    public List<Memory> Memories { get; set; } = new();

    public bool Remembers(string tag) => Memories.Exists(m => m.Tag == tag);
}

public sealed record Memory(string Tag, int Day, int Minute);
