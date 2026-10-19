namespace OfficeMan.Core.World;

/// <summary>
/// Who Teo is (four hidden traits) and what his body and mood do to them right now.
/// </summary>
/// <remarks>
/// Agreed with the creator 2026-10-19 (GAME_VISION.md §7, design/traits_proposal.md). Each trait is a line
/// between two poles, -2 … +2, stored as the quality <c>trait_&lt;name&gt;</c>; both ends help and both hurt.
/// <list type="bullet">
/// <item><b>nerve</b>: + bold, says it and does it; - careful, watches and waits.</item>
/// <item><b>warmth</b>: + open, gets involved; - closed, keeps his distance.</item>
/// <item><b>appetite</b>: + hungry, drink, sex, now; - held back, steady.</item>
/// <item><b>honesty</b>: + blunt, says what he thinks; - smooth, says what people want to hear.</item>
/// </list>
/// Every life starts from the creator's baseline Teo with one or two traits nudged a step. Conditions (drink,
/// tiredness, mood, shame, a hangover) push the traits for the moment; lasting change is rare and comes only from
/// big moments the script marks with <c>shift</c>. Content reads the effective values by name ("nerve", "warmth", …)
/// and the skills the checks use ("tact", "judgment", "restraint").
/// </remarks>
public static class Teo
{
    public static readonly string[] Traits = { "nerve", "warmth", "appetite", "honesty" };

    /// <summary>The creator's Teo: careful, open, held back, smooth. A man who lets life happen to him.</summary>
    public static readonly IReadOnlyDictionary<string, int> Baseline = new Dictionary<string, int>
    {
        ["nerve"] = -1, ["warmth"] = 1, ["appetite"] = -1, ["honesty"] = -1,
    };

    public const int TraitMin = -2, TraitMax = 2;

    public static readonly string[] DerivedNames =
    {
        "drunk", "tired", "hungover", "nerve", "warmth", "appetite", "honesty",
        "tact", "judgment", "restraint", "shifts", "time", "day", "hour",
    };

    /// <summary>Sets the baseline and nudges one or two traits a step, seeded, so a reload gives the same man.</summary>
    public static void StartingTraits(WorldState s)
    {
        foreach (var (name, value) in Baseline) s.Set("trait_" + name, value);
        int count = Rng.Range(s.Seed, "traits:count", 1, 3);
        var pool = Traits.ToList();
        for (int i = 0; i < count; i++)
        {
            string name = pool[Rng.Range(s.Seed, $"traits:pick:{i}", 0, pool.Count)];
            pool.Remove(name);
            int delta = Rng.Range(s.Seed, $"traits:dir:{i}", 0, 2) == 0 ? -1 : 1;
            s.Set("trait_" + name, Math.Clamp(s.Get("trait_" + name) + delta, TraitMin, TraitMax));
        }
    }

    /// <summary>A lasting change after a big moment: one step, once per reason. Returns false if already applied.</summary>
    public static bool Shift(WorldState s, string trait, int delta, string reason)
    {
        if (!Traits.Contains(trait)) throw new ArgumentException($"unknown trait '{trait}'");
        if (s.Get("shifted:" + reason) != 0) return false;
        s.Set("shifted:" + reason, 1);
        int before = s.Get("trait_" + trait);
        int after = Math.Clamp(before + Math.Sign(delta), TraitMin, TraitMax);
        if (after == before) return false;   // already as far as he goes that way
        s.Set("trait_" + trait, after);
        s.TraitShifts.Add(new TraitShift(trait, after - before, reason, s.Day, s.Minute));
        return true;
    }

    /// <summary>0 sober, 1 tipsy, 2 drunk, 3 very drunk, 4 wrecked.</summary>
    public static int Drunk(WorldState s) => s.Get("alcohol") switch
    {
        <= 1 => 0,
        <= 3 => 1,
        <= 6 => 2,
        <= 9 => 3,
        _ => 4,
    };

    /// <summary>0 fresh, 1 tired, 2 exhausted, 3 running on nothing.</summary>
    public static int Tired(WorldState s)
    {
        int hoursAwake = (s.AbsoluteMinute - s.AwakeSince) / 60;
        int level = hoursAwake switch
        {
            < 10 => 0,
            < 15 => 1,
            < 19 => 2,
            _ => 3,
        };
        if (Drunk(s) >= 3) level++;
        return Math.Min(3, level);
    }

    /// <summary>The morning after a heavy night: until lunch on the next day.</summary>
    public static bool Hungover(WorldState s) => s.Day >= 1 && s.Get("peak_drunk") >= 3 && s.Minute < 13 * 60;

    private static int MoodPush(WorldState s) => s.Get("mood") switch { >= 1 => 1, <= -2 => -1, _ => 0 };

    // ── The traits as they are right now ──

    public static int Nerve(WorldState s) =>
        s.Get("trait_nerve") + MoodPush(s) + new[] { 0, 1, 1, 2, 2 }[Drunk(s)] - s.Get("shame") - (Hungover(s) ? 1 : 0);

    public static int Warmth(WorldState s) =>
        s.Get("trait_warmth") + MoodPush(s) + new[] { 0, 1, 1, 0, -1 }[Drunk(s)]
        - (Tired(s) >= 2 ? 1 : 0) - (Hungover(s) ? 1 : 0);

    public static int Appetite(WorldState s) => s.Get("trait_appetite") + new[] { 0, 1, 1, 2, 2 }[Drunk(s)];

    public static int Honesty(WorldState s) => s.Get("trait_honesty") + new[] { 0, 0, 1, 1, 2 }[Drunk(s)];

    // ── What the checks use ──

    /// <summary>Saying the right thing: warm and smooth helps; drink helps a little, then a lot less.</summary>
    public static int Tact(WorldState s) =>
        (int)Math.Floor((Warmth(s) - Honesty(s)) / 2.0) + new[] { 0, 1, 0, -1, -2 }[Drunk(s)] - (Tired(s) >= 2 ? 1 : 0);

    /// <summary>Seeing the danger: a hungry man sees less of it, a drunk one much less.</summary>
    public static int Judgment(WorldState s) =>
        1 - Math.Max(0, s.Get("trait_appetite")) - new[] { 0, 0, 1, 2, 4 }[Drunk(s)] - (Tired(s) >= 2 ? 1 : 0);

    /// <summary>Stopping himself.</summary>
    public static int Restraint(WorldState s) => -Appetite(s) - (s.Get("mood") < 0 ? 1 : 0);

    /// <summary>Resolves a derived name, or returns null if the name is a plain quality.</summary>
    public static int? Derived(WorldState s, string name) => name switch
    {
        "drunk" => Drunk(s),
        "tired" => Tired(s),
        "hungover" => Hungover(s) ? 1 : 0,
        "nerve" => Nerve(s),
        "warmth" => Warmth(s),
        "appetite" => Appetite(s),
        "honesty" => Honesty(s),
        "tact" => Tact(s),
        "judgment" => Judgment(s),
        "restraint" => Restraint(s),
        "shifts" => s.TraitShifts.Count,
        "time" => s.Minute,
        "day" => s.Day,
        "hour" => s.Minute / 60,
        _ => null,
    };

    public static int Value(WorldState s, string name) => Derived(s, name) ?? s.Get(name);

    /// <summary>
    /// Advances the clock, letting alcohol wear off at one unit per hour.
    /// </summary>
    public static void PassTime(WorldState s, int minutes)
    {
        if (minutes <= 0) return;
        s.Minute += minutes;

        s.MetabolismMinutes += minutes;
        while (s.MetabolismMinutes >= 60)
        {
            s.MetabolismMinutes -= 60;
            if (s.Get("alcohol") > 0) s.Add("alcohol", -1);
        }
    }

    /// <summary>Moves to a new day at the given minute, as after sleep. Sleep burns off alcohol.</summary>
    public static void WakeUp(WorldState s, int day, int minute)
    {
        int asleep = Math.Max(0, (day * 1440 + minute) - s.AbsoluteMinute);
        s.Day = day;
        s.Minute = minute;
        s.MetabolismMinutes = 0;
        s.Set("alcohol", Math.Max(0, s.Get("alcohol") - asleep / 90));
        s.AwakeSince = s.AbsoluteMinute;
    }

    /// <summary>How the time of day feels, for arrival cards: no exact clock is ever shown to the player.</summary>
    public static string DayPart(WorldState s)
    {
        int m = s.Minute % 1440;
        string part = m switch
        {
            < 5 * 60 => "the small hours",
            < 12 * 60 => "morning",
            < 14 * 60 => "lunchtime",
            < 17 * 60 => "afternoon",
            < 21 * 60 => "evening",
            _ => "night",
        };
        string[] days = { "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday", "Monday" };
        int dayIndex = s.Day + s.Minute / 1440;
        return $"{days[dayIndex % 7]} · {part}";
    }

    public static string Clock(WorldState s)
    {
        int m = s.Minute % 1440;
        string[] days = { "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday", "Monday" };
        int dayIndex = s.Day + s.Minute / 1440;
        return $"{days[dayIndex % 7]} {m / 60:00}:{m % 60:00}";
    }
}

/// <summary>A lasting change to one of Teo's traits, and what caused it.</summary>
public sealed record TraitShift(string Trait, int Delta, string Reason, int Day, int Minute);
