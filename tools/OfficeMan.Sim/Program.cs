using System.Collections.Concurrent;
using System.Diagnostics;
using OfficeMan.Core;
using OfficeMan.Core.Content;

// Plays many lives automatically and reports on the content.
//   dotnet run --project tools/OfficeMan.Sim -- [--runs 1000] [--seed 1] [--persona random|drinker|cautious|kind|bold|late|all]
// Exit code 1 if any run crashed or hit a dead end.

int runs = 1000;
int firstSeed = 1;
string personaArg = "all";
for (int i = 0; i < args.Length; i++)
{
    if (args[i] == "--runs") runs = int.Parse(args[++i]);
    else if (args[i] == "--seed") firstSeed = int.Parse(args[++i]);
    else if (args[i] == "--persona") personaArg = args[++i];
}

ContentBundle content;
try { content = ContentBundle.Load(ContentLocator.Find()); }
catch (ContentException e) { Console.Error.WriteLine(e.Message); return 1; }

foreach (string w in content.Warnings) Console.WriteLine($"warning: {w}");

var personas = personaArg == "all" ? Persona.All : Persona.All.Where(p => p.Name == personaArg).ToArray();
bool failed = false;
var allPlayed = new ConcurrentDictionary<string, int>();

foreach (var persona in personas)
{
    var report = Simulator.Run(content, persona, runs, firstSeed);
    report.Print();
    failed |= report.Failures.Count > 0 || report.DeadEnds > 0;
    foreach (var (id, n) in report.StoryletPlays) allPlayed.AddOrUpdate(id, n, (_, old) => old + n);
}

var never = content.Storylets.Where(s => !allPlayed.ContainsKey(s.Id)).Select(s => s.Id).ToList();
Console.WriteLine();
Console.WriteLine(never.Count == 0
    ? "Coverage: every storylet was reached at least once."
    : $"Coverage: NEVER REACHED ({never.Count}): {string.Join(", ", never)}");

return failed ? 1 : 0;

sealed record Persona(string Name, IReadOnlyDictionary<string, double> TagWeights, double ExitWeight)
{
    public static readonly Persona[] All =
    {
        new("random", new Dictionary<string, double>(), 0.6),
        new("drinker", new Dictionary<string, double> { ["drink"] = 5, ["cautious"] = 0.3 }, 0.3),
        new("cautious", new Dictionary<string, double> { ["cautious"] = 5, ["drink"] = 0.2, ["bold"] = 0.3 }, 1.0),
        new("kind", new Dictionary<string, double> { ["kind"] = 5, ["bold"] = 0.5 }, 0.5),
        new("bold", new Dictionary<string, double> { ["bold"] = 5, ["drink"] = 2, ["cautious"] = 0.2 }, 0.4),
        // Stays to the end of everything: the player who wants to see what happens next.
        new("late", new Dictionary<string, double>(), 0.05),
    };

    public double Weight(ChoiceView c)
    {
        double w = 1;
        foreach (string tag in c.Tags)
        {
            if (tag == "exit") w *= ExitWeight;
            else if (TagWeights.TryGetValue(tag.Trim(), out double m)) w *= m;
        }
        return w;
    }
}

sealed class Report
{
    public required string Persona { get; init; }
    public int Runs;
    public long Decisions;
    public int DeadEnds;
    public TimeSpan Elapsed;
    public readonly ConcurrentBag<string> Failures = new();
    public readonly ConcurrentDictionary<string, int> StoryletPlays = new();
    public readonly ConcurrentDictionary<string, int> WokeAt = new();
    public readonly ConcurrentDictionary<string, int> Endings = new();
    public readonly ConcurrentDictionary<string, int> Nights = new();
    public readonly ConcurrentDictionary<string, int> Headlines = new();
    public readonly ConcurrentDictionary<string, int> PeakDrunk = new();
    public readonly ConcurrentDictionary<string, int> StartingTeo = new();
    public readonly ConcurrentDictionary<string, int> Shifts = new();
    public readonly ConcurrentDictionary<string, int> People = new();
    public readonly ConcurrentDictionary<string, int> LateNights = new();
    public int StayedLate;
    public readonly ConcurrentDictionary<string, int> NightByNerve = new();
    public readonly ConcurrentDictionary<string, int> NerveCount = new();
    public readonly ConcurrentDictionary<string, byte> UniqueLives = new();
    public readonly ConcurrentDictionary<string, byte> UniqueStories = new();

    public void Print()
    {
        Console.WriteLine();
        Console.WriteLine($"═══ persona: {Persona} · {Runs} lives · {Elapsed.TotalSeconds:0.0}s ═══");
        Console.WriteLine($"  decisions per life: {(double)Decisions / Math.Max(1, Runs):0.0}");
        Console.WriteLine($"  distinct storylet sequences: {UniqueLives.Count} ({100.0 * UniqueLives.Count / Math.Max(1, Runs):0}%)");
        Console.WriteLine($"  distinct headline sets (what the player would retell): {UniqueStories.Count}");
        Console.WriteLine($"  crashes: {Failures.Count}   dead ends: {DeadEnds}");
        foreach (string f in Failures.Take(5)) Console.WriteLine($"    ! {f}");

        Section("how Tuesday night went", Nights);
        Console.WriteLine($"  still at the party at 22:00: {100.0 * StayedLate / Math.Max(1, Runs):0.0}%; their nights (share of them):");
        foreach (var (key, n) in LateNights.OrderByDescending(kv => kv.Value).Take(8))
            Console.WriteLine($"    {100.0 * n / Math.Max(1, StayedLate),5:0.0}%  {key}");
        Section("woke up on Wednesday", WokeAt);
        Section("closing hook", Endings);
        Section("peak drunkenness (0 sober … 4 wrecked)", PeakDrunk);
        Section("most common headlines", Headlines, 12);
        Section("starting Teo (traits nudged from the baseline)", StartingTeo, 12);
        Section("lasting trait shifts", Shifts, 12);
        if (!People.IsEmpty) Section("where other people's day ended up (their own plans)", People, 30);
        Console.WriteLine("  how the night went, by starting nerve (share within each nerve level):");
        foreach (var (key, n) in NightByNerve.OrderBy(kv => kv.Key).ThenByDescending(kv => kv.Value))
        {
            string nerve = key.Split('|')[0];
            int total = NerveCount.TryGetValue(nerve, out int t) ? t : 1;
            if (100.0 * n / total >= 8) Console.WriteLine($"    nerve {nerve,2}: {100.0 * n / total,5:0.0}%  {key.Split('|')[1]}");
        }
    }

    private void Section(string title, ConcurrentDictionary<string, int> data, int take = 20)
    {
        Console.WriteLine($"  {title}:");
        foreach (var (key, n) in data.OrderByDescending(kv => kv.Value).Take(take))
            Console.WriteLine($"    {100.0 * n / Math.Max(1, Runs),5:0.0}%  {key}");
    }
}

static class Simulator
{
    private static readonly HashSet<string> NightStorylets =
        new() { "club_schengen", "magda_walk", "ramona_car", "river_night", "kornel_night", "home_stairwell" };

    private static readonly string[] WakePlaces = { "home (no record)", "home", "Magda's", "Kornel's sofa", "sobering-up station" };

    public static Report Run(ContentBundle content, Persona persona, int runs, int firstSeed)
    {
        var report = new Report { Persona = persona.Name, Runs = runs };
        var clock = Stopwatch.StartNew();

        Parallel.For(0, runs, i =>
        {
            int seed = firstSeed + i;
            try
            {
                var game = Game.New(content, seed);
                var start = OfficeMan.Core.World.Teo.Traits
                    .Select(t => (t, d: game.State.Get("trait_" + t) - OfficeMan.Core.World.Teo.Baseline[t]))
                    .Where(x => x.d != 0).Select(x => $"{x.t} {x.d:+0;-0}");
                Count(report.StartingTeo, string.Join(", ", start));
                string startNerve = game.State.Get("trait_nerve").ToString();
                int decisions = 0;
                bool late = false;
                while (!game.IsOver)
                {
                    late |= game.State.PhaseId == "party" && game.State.Minute >= 22 * 60;
                    if (++decisions > 2000) throw new InvalidOperationException("over 2000 decisions; probable loop");
                    var choices = game.Current.Choices;
                    var weights = choices.Select(c => (int)Math.Round(persona.Weight(c) * 100)).ToList();
                    int pick = Rng.Weighted(seed, $"sim:{persona.Name}:{decisions}", weights);
                    game.Choose(Math.Max(0, pick));
                }

                Interlocked.Add(ref report.Decisions, decisions);
                Interlocked.Add(ref report.DeadEnds, game.DeadEnds);
                var s = game.State;
                foreach (string id in s.History) report.StoryletPlays.AddOrUpdate(id, 1, (_, n) => n + 1);
                Count(report.WokeAt, WakePlaces[Math.Clamp(s.Get("slept_at"), 0, WakePlaces.Length - 1)]);
                Count(report.Nights, string.Join(" → ", s.History.Where(NightStorylets.Contains)));
                if (late)
                {
                    Interlocked.Increment(ref report.StayedLate);
                    Count(report.LateNights, string.Join(" → ", s.History.Where(NightStorylets.Contains)));
                }
                Count(report.Endings, s.History.LastOrDefault(h => h.StartsWith("end_")) ?? "(none)");
                Count(report.PeakDrunk, s.Get("peak_drunk").ToString());
                foreach (string h in s.Headlines.Distinct()) Count(report.Headlines, h);
                foreach (var (who, c) in s.Characters.Where(kv => kv.Value.Doing.Length > 0))
                    Count(report.People, $"{who}: {c.Doing}");
                foreach (var shift in s.TraitShifts) Count(report.Shifts, $"{shift.Trait} {shift.Delta:+0;-0} ({shift.Reason})");
                Count(report.NerveCount, startNerve);
                string night = string.Join(" → ", s.History.Where(NightStorylets.Contains));
                Count(report.NightByNerve, $"{startNerve}|{(night.Length == 0 ? "(home)" : night)}");
                report.UniqueLives.TryAdd(string.Join(">", s.History), 0);
                report.UniqueStories.TryAdd(string.Join("|", s.Headlines.OrderBy(h => h)), 0);
            }
            catch (Exception e)
            {
                report.Failures.Add($"seed {seed}: {e.GetType().Name}: {e.Message.Split('\n')[0]}");
            }
        });

        report.Elapsed = clock.Elapsed;
        return report;
    }

    private static void Count(ConcurrentDictionary<string, int> d, string key) => d.AddOrUpdate(key, 1, (_, n) => n + 1);
}
