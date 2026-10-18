namespace OfficeMan.Core.World;

/// <summary>
/// Derived state of the protagonist: what his body and mood do to his underlying traits.
/// </summary>
/// <remarks>
/// PROVISIONAL. The trait list and every formula here are placeholders until the trait system is
/// designed with the creator (GAME_VISION.md §7). Content should only rely on the names below.
/// Base traits are stored as qualities <c>base_nerve</c>, <c>base_tact</c>, <c>base_judgment</c>, <c>base_restraint</c>.
/// </remarks>
public static class Teo
{
    public static readonly string[] DerivedNames =
        { "drunk", "tired", "nerve", "tact", "judgment", "restraint", "time", "day", "hour" };

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

    public static int Nerve(WorldState s) =>
        s.Get("base_nerve") + s.Get("mood") + new[] { 0, 1, 2, 3, 3 }[Drunk(s)] - s.Get("shame");

    public static int Tact(WorldState s) =>
        s.Get("base_tact") + (s.Get("mood") > 0 ? 1 : 0) + new[] { 0, 1, 0, -2, -3 }[Drunk(s)]
        - (Tired(s) >= 2 ? 1 : 0);

    public static int Judgment(WorldState s) =>
        s.Get("base_judgment") - new[] { 0, 0, 1, 2, 4 }[Drunk(s)] - (Tired(s) >= 2 ? 1 : 0);

    public static int Restraint(WorldState s) =>
        s.Get("base_restraint") - new[] { 0, 1, 2, 3, 4 }[Drunk(s)] - (s.Get("mood") < 0 ? 1 : 0);

    /// <summary>Resolves a derived name, or returns null if the name is a plain quality.</summary>
    public static int? Derived(WorldState s, string name) => name switch
    {
        "drunk" => Drunk(s),
        "tired" => Tired(s),
        "nerve" => Nerve(s),
        "tact" => Tact(s),
        "judgment" => Judgment(s),
        "restraint" => Restraint(s),
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

    public static string Clock(WorldState s)
    {
        int m = s.Minute % 1440;
        string[] days = { "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday", "Monday" };
        int dayIndex = s.Day + s.Minute / 1440;
        return $"{days[dayIndex % 7]} {m / 60:00}:{m % 60:00}";
    }
}
