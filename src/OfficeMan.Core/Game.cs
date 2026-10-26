using Ink.Runtime;
using OfficeMan.Core.Conditions;
using OfficeMan.Core.Content;
using OfficeMan.Core.World;

namespace OfficeMan.Core;

/// <summary>
/// Runs one life: walks the phase schedule, lets the director pick storylets, plays them through
/// ink, and exposes each decision point as a <see cref="Presentation"/>.
/// </summary>
/// <remarks>
/// All world state lives in <see cref="WorldState"/>. Ink holds only the flow of the current
/// storylet and its own visit counts; it reads and writes the world through external functions.
/// </remarks>
public sealed class Game : IConditionContext
{
    private const int MaxStepsPerAdvance = 10_000;
    private const int DeadEndMinutes = 30;

    private static readonly HashSet<string> MetadataTagKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "phase", "kind", "choice", "when", "window", "at", "chance", "priority", "weight", "duration", "repeat", "max", "where",
    };

    private readonly Story _story;
    private readonly List<StoryLine> _pending = new();
    private List<Storylet> _hubOptions = new();
    private string? _activeStorylet;

    public ContentBundle Content { get; }
    public WorldState State { get; }
    public Presentation Current { get; private set; } = null!;
    public bool IsOver => Current.Kind == PresentationKind.End;

    /// <summary>Times a hub had nothing to offer and the engine had to skip time. Should stay 0.</summary>
    public int DeadEnds { get; private set; }

    /// <summary>Most recent hidden checks, newest last. For debugging and the simulator only.</summary>
    public List<string> CheckLog { get; } = new();

    private Game(ContentBundle content, WorldState state)
    {
        Content = content;
        State = state;
        _story = new Story(content.StoryJson);
        _story.onError += (message, type) =>
        {
            if (type == Ink.ErrorType.Error) throw new StoryException(message);
        };
        BindExternals();
    }

    public static Game New(ContentBundle content, int seed)
    {
        var first = content.Schedule.Get(content.Schedule.FirstPhase);
        var state = new WorldState
        {
            Seed = seed,
            Day = first.Day,
            Minute = first.StartMinute ?? 0,
            PhaseId = first.Id,
        };
        state.AwakeSince = state.AbsoluteMinute;

        OfficeMan.Core.World.Teo.StartingTraits(state);
        var game = new Game(content, state);
        game.RunSilently("init");
        game.Current = game.Advance();
        return game;
    }

    public static Game Load(ContentBundle content, SaveGame save)
    {
        if (save.Version != SaveGame.CurrentVersion)
            throw new InvalidOperationException($"Unsupported save version {save.Version}");

        var game = new Game(content, save.State);
        game._story.state.LoadJson(save.InkState);
        game._activeStorylet = save.ActiveStorylet;
        game._pending.AddRange(save.LastLines);
        game.Current = game._activeStorylet is not null ? game.DialoguePresentation() : game.Advance();
        return game;
    }

    public SaveGame Save() => new()
    {
        State = State,
        InkState = _story.state.ToJson(),
        ActiveStorylet = _activeStorylet,
        LastLines = Current.Lines.ToList(),
    };

    public Presentation Choose(int index)
    {
        if (IsOver) throw new InvalidOperationException("The game is over.");
        if (index < 0 || index >= Current.Choices.Count) throw new ArgumentOutOfRangeException(nameof(index));

        foreach (string tag in Current.Choices[index].Tags)
            if (tag.Trim() is "bold" or "kind" or "cautious" or "drink") State.Add("chose_" + tag.Trim(), 1);

        if (Current.Kind == PresentationKind.Dialogue)
        {
            _story.ChooseChoiceIndex(index);
        }
        else
        {
            State.Steps++;
            StartStorylet(_hubOptions[index]);
        }
        Current = Advance();
        return Current;
    }

    // ───────────────────────── director ─────────────────────────

    private Presentation Advance()
    {
        for (int guard = 0; guard < MaxStepsPerAdvance; guard++)
        {
            if (_activeStorylet is not null)
            {
                CollectLines();
                if (_story.currentChoices.Count > 0)
                {
                    if (Content.Get(_activeStorylet).Kind == StoryletKind.Offstage)
                        throw new ContentException($"offstage storylet '{_activeStorylet}' must not offer choices");
                    return DialoguePresentation();
                }
                FinishStorylet();
                continue;
            }

            if (State.PhaseId == PhaseSchedule.EndPhase) return EndPresentation();

            var phase = Content.Schedule.Get(State.PhaseId);
            if (!State.PhaseEntered) EnterPhase(phase);

            if (State.NextPhase is not null || State.Get("_phase_complete") != 0)
            {
                LeavePhase(phase);
                continue;
            }

            if (phase.Mode == PhaseMode.Scene)
            {
                var pick = PickScene(phase);
                if (pick is null)
                {
                    State.Set("_phase_complete", 1);
                    continue;
                }
                StartStorylet(pick);
                continue;
            }

            // Hub: entry storylets, then what other people do on their own, then interrupts, then timeout, then the menu.
            var auto = Eligible(phase, StoryletKind.Enter).FirstOrDefault()
                       ?? Eligible(phase, StoryletKind.Offstage)
                           .FirstOrDefault(s => Rng.Percent(State.Seed, $"offstage:{s.Id}:{State.Steps}", s.Chance))
                       ?? Eligible(phase, StoryletKind.Interrupt)
                           .FirstOrDefault(s => Rng.Percent(State.Seed, $"interrupt:{s.Id}:{State.Steps}", s.Chance));
            if (auto is not null)
            {
                StartStorylet(auto);
                continue;
            }

            if (State.Minute >= phase.EndMinute)
            {
                var timeout = Eligible(phase, StoryletKind.Timeout).FirstOrDefault();
                if (timeout is not null) StartStorylet(timeout);
                else State.Set("_phase_complete", 1);
                continue;
            }

            _hubOptions = Eligible(phase, StoryletKind.Hub)
                .Concat(Eligible(phase, StoryletKind.Exit))
                .ToList();
            if (_hubOptions.Count == 0)
            {
                DeadEnds++;
                Teo.PassTime(State, DeadEndMinutes);
                continue;
            }
            return HubPresentation(phase);
        }
        throw new InvalidOperationException($"Director made no progress in phase '{State.PhaseId}'.");
    }

    private void EnterPhase(Phase phase)
    {
        if (phase.Day > State.Day)
        {
            Teo.WakeUp(State, phase.Day, phase.StartMinute ?? 0);
        }
        else if (phase.StartMinute is int start && start > State.Minute)
        {
            Teo.PassTime(State, start - State.Minute);
        }

        State.PhaseEntered = true;
        State.Set("_phase_complete", 0);
        // Every arrival is marked in the text, so front ends switch place exactly where the story does.
        _pending.Add(new StoryLine(
            string.IsNullOrEmpty(phase.Title) ? Teo.DayPart(State) : $"{phase.Title} · {Teo.Clock(State)}",
            new[] { "enter:" + phase.Id, "title:" + phase.Title, "daypart:" + Teo.DayPart(State) },
            LineKind.Header));
    }

    private void LeavePhase(Phase phase)
    {
        string next = State.NextPhase ?? phase.Next;
        State.NextPhase = null;
        State.PhaseEntered = false;
        State.Set("_phase_complete", 0);
        State.PhaseId = next;
    }

    private Storylet? PickScene(Phase phase)
    {
        var candidates = Eligible(phase, StoryletKind.Scene).ToList();
        if (candidates.Count == 0) return null;

        int top = candidates.Max(s => s.Priority);
        var best = candidates.Where(s => s.Priority == top).ToList();
        int i = Rng.Weighted(State.Seed, $"scene:{phase.Id}:{State.Day}:{State.Steps}", best.Select(s => s.Weight).ToList());
        return best[Math.Max(0, i)];
    }

    /// <summary>Storylets of a kind that can play right now, best first.</summary>
    private IEnumerable<Storylet> Eligible(Phase phase, StoryletKind kind) =>
        Content.Storylets
            .Where(s => s.Kind == kind && s.InPhase(phase.Id) && IsEligible(s))
            .OrderByDescending(s => s.Priority);

    public bool IsEligible(Storylet s)
    {
        if (s.MaxPlays > 0 && State.Plays(s.Id) >= s.MaxPlays) return false;
        if (s.WindowStart is int ws && State.Minute < ws) return false;
        if (s.WindowEnd is int we && State.Minute >= we) return false;
        if (s.At is int at && State.Minute < at) return false;
        return s.When.IsTrue(this);
    }

    // ───────────────────────── storylets ─────────────────────────

    private bool _beginning;

    private void StartStorylet(Storylet s)
    {
        _activeStorylet = s.Id;
        _beginning = true;
        _story.ChoosePathString(s.Id);
    }

    private void FinishStorylet()
    {
        var s = Content.Get(_activeStorylet!);
        _activeStorylet = null;

        State.PlayCounts[s.Id] = State.Plays(s.Id) + 1;
        State.History.Add(s.Id);

        // An exit where someone talked him into staying: he doesn't leave, and the walk home doesn't happen.
        if (s.Kind == StoryletKind.Exit && State.Get("_stay") != 0)
        {
            State.Set("_stay", 0);
            return;
        }
        Teo.PassTime(State, s.Duration);

        var phase = Content.Schedule.Get(State.PhaseId);
        if (phase.Mode == PhaseMode.Scene || s.Kind is StoryletKind.Exit or StoryletKind.Timeout)
            State.Set("_phase_complete", 1);
    }

    private void CollectLines()
    {
        while (_story.canContinue)
        {
            string text;
            try { text = _story.Continue().Trim(); }
            catch (StoryException e)
            {
                throw new StoryException($"in storylet '{_activeStorylet}' at {_story.state.currentPathString}: {e.Message}");
            }
            var tags = _story.currentTags.Where(t => !IsMetadataTag(t)).ToList();
            if (text.Length == 0) continue;
            if (Content.Get(_activeStorylet!).Kind == StoryletKind.Offstage && !tags.Exists(IsHeard))
                throw new ContentException($"offstage storylet '{_activeStorylet}': only overheard lines (#heard) are allowed: \"{text}\"");
            // The first line of each storylet says so: front ends end the previous conversation there.
            if (_beginning) tags.Add("begin:" + _activeStorylet);
            _beginning = false;
            _pending.Add(new StoryLine(text, tags));
        }
    }

    private static bool IsHeard(string tag)
    {
        string t = tag.Trim();
        return t.Equals("heard", StringComparison.OrdinalIgnoreCase) || t.StartsWith("heard:", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsMetadataTag(string tag)
    {
        string t = tag.Trim();
        int colon = t.IndexOf(':');
        return MetadataTagKeys.Contains(colon < 0 ? t : t[..colon].Trim());
    }

    /// <summary>Runs a knot for its side effects only (e.g. <c>init</c>), discarding text.</summary>
    private void RunSilently(string knot)
    {
        if (!Content.KnotNames.Contains(knot)) return;
        _story.ChoosePathString(knot);
        while (_story.canContinue) _story.Continue();
        if (_story.currentChoices.Count > 0)
            throw new ContentException($"Knot '{knot}' must not offer choices.");
    }

    // ───────────────────────── presentations ─────────────────────────

    private Presentation DialoguePresentation() => new()
    {
        Kind = PresentationKind.Dialogue,
        Lines = TakePending(),
        Choices = _story.currentChoices
            .Select((c, i) => new ChoiceView(i, c.text.Trim(), c.tags ?? new List<string>()))
            .ToList(),
        Clock = Teo.Clock(State),
        When = Teo.DayPart(State),
        PhaseTitle = Content.Schedule.Get(State.PhaseId).Title,
        PhaseId = State.PhaseId,
    };

    private Presentation HubPresentation(Phase phase) => new()
    {
        Kind = PresentationKind.Hub,
        Lines = TakePending(),
        Choices = _hubOptions
            .Select((s, i) => new ChoiceView(i, s.ChoiceText, HubTags(s)))
            .ToList(),
        Clock = Teo.Clock(State),
        When = Teo.DayPart(State),
        PhaseTitle = phase.Title,
        PhaseId = phase.Id,
    };

    private static IReadOnlyList<string> HubTags(Storylet s)
    {
        var tags = new List<string>();
        if (s.Kind == StoryletKind.Exit) tags.Add("exit");
        if (s.Where is not null) tags.Add("where:" + s.Where);
        return tags;
    }

    private readonly Dictionary<string, Condition> _conditionCache = new();

    /// <summary>Evaluates a condition expression against the current world (e.g. who is in a room).</summary>
    public bool Evaluate(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression)) return true;
        if (!_conditionCache.TryGetValue(expression, out var c))
            _conditionCache[expression] = c = Condition.Parse(expression);
        return c.IsTrue(this);
    }

    private Presentation EndPresentation() => new()
    {
        Kind = PresentationKind.End,
        Lines = TakePending(),
        Choices = Array.Empty<ChoiceView>(),
        Clock = Teo.Clock(State),
        When = Teo.DayPart(State),
        Headlines = State.Headlines.ToList(),
        Today = Collect("today"),
    };

    /// <summary>Runs a knot that only prints (no choices) and returns its lines.</summary>
    private List<string> Collect(string knot)
    {
        var lines = new List<string>();
        if (!Content.KnotNames.Contains(knot)) return lines;
        _story.ChoosePathString(knot);
        while (_story.canContinue)
        {
            string text = _story.Continue().Trim();
            if (text.Length > 0) lines.Add(text);
        }
        return lines;
    }

    private List<StoryLine> TakePending()
    {
        var lines = _pending.ToList();
        _pending.Clear();
        return lines;
    }

    // ───────────────────────── world access ─────────────────────────

    public int Value(string name) => Teo.Value(State, name);

    public int Call(string function, IReadOnlyList<string> args)
    {
        string Arg(int i) => i < args.Count ? args[i] : throw new FormatException($"{function}() needs {i + 1} argument(s)");
        return function switch
        {
            "played" => State.Plays(Arg(0)),
            "remembers" => State.Character(Arg(0)).Remembers(Arg(1)) ? 1 : 0,
            "regard" => State.Character(Arg(0)).Regard,
            "doing" => State.Character(Arg(0)).Doing == Arg(1) ? 1 : 0,
            "feels" => State.Character(Arg(0)).Feels,
            "q" => Value(Arg(0)),
            _ => throw new FormatException($"Unknown condition function '{function}'"),
        };
    }

    /// <summary>
    /// A hidden check. Returns 0 (badly), 1 (mixed) or 2 (well). Never pass/fail:
    /// content writes a different outcome for each tier.
    /// </summary>
    public int Check(string id, string stat)
    {
        int n = State.CheckCounts.TryGetValue(id, out int c) ? c : 0;
        State.CheckCounts[id] = n + 1;

        int a = Rng.Range(State.Seed, $"check:{id}:{n}:a", -1, 2);
        int b = Rng.Range(State.Seed, $"check:{id}:{n}:b", -1, 2);
        int value = Value(stat);
        int total = value + a + b;
        int tier = total < 0 ? 0 : total >= 2 ? 2 : 1;

        CheckLog.Add($"{id}: {stat}={value} roll={a + b:+0;-0;0} → {new[] { "bad", "mixed", "good" }[tier]}");
        _lastCause = Cause(stat, value, a + b, tier);
        return tier;
    }

    private string _lastCause = "";

    /// <summary>
    /// What decided a check, for the hindsight line: luck when the dice went against the odds, otherwise the drink,
    /// tiredness or (for nerve) his nature. Empty when nothing stands out.
    /// </summary>
    private string Cause(string stat, int value, int roll, int tier)
    {
        if (tier == 0 && value >= 1 && roll <= -2) return "luck_bad";
        if (tier == 2 && value <= 0 && roll >= 2) return "luck_good";
        if (Teo.Drunk(State) >= 2) return "drink";
        if (tier == 0 && Teo.Tired(State) >= 2) return "tired";
        if (stat == "nerve" && State.Get("trait_nerve") >= 0) return "bold";
        if (stat == "nerve" && State.Get("trait_nerve") <= -2 && tier == 0) return "careful";
        return "";
    }

    private void BindExternals()
    {
        // Pure reads are lookahead-safe, so content can use them inside choice text and inline logic.
        _story.BindExternalFunction("q", (string name) => Value(name), lookaheadSafe: true);
        _story.BindExternalFunction("qset", (string name, int value) => State.Set(name, value));
        _story.BindExternalFunction("qadd", (string name, int delta) => State.Add(name, delta));
        _story.BindExternalFunction("remember", (string who, string tag) =>
        {
            var c = State.Character(who);
            if (!c.Remembers(tag)) c.Memories.Add(new Memory(tag, State.Day, State.Minute));
        });
        _story.BindExternalFunction("remembers", (string who, string tag) => State.Character(who).Remembers(tag), lookaheadSafe: true);
        _story.BindExternalFunction("regard", (string who) => State.Character(who).Regard, lookaheadSafe: true);
        _story.BindExternalFunction("like", (string who, int delta) =>
        {
            var c = State.Character(who);
            c.Regard = Math.Clamp(c.Regard + delta, -10, 10);
        });
        _story.BindExternalFunction("doing", (string who, string what) => State.Character(who).Doing == what, lookaheadSafe: true);
        _story.BindExternalFunction("set_doing", (string who, string what) => { State.Character(who).Doing = what; });
        _story.BindExternalFunction("feels", (string who) => State.Character(who).Feels, lookaheadSafe: true);
        _story.BindExternalFunction("feel", (string who, int delta) =>
        {
            var c = State.Character(who);
            c.Feels = Math.Clamp(c.Feels + delta, -5, 5);
        });
        _story.BindExternalFunction("check", (string id, string stat) => Check(id, stat));
        _story.BindExternalFunction("chance", (string id, int percent) =>
            Rng.Percent(State.Seed, $"chance:{id}:{State.Day}", percent));
        _story.BindExternalFunction("drink", (int units) => State.Add("alcohol", units));
        _story.BindExternalFunction("pass_time", (int minutes) => Teo.PassTime(State, minutes));
        _story.BindExternalFunction("stay", () => State.Set("_stay", 1));
        _story.BindExternalFunction("goto_phase", (string id) =>
        {
            if (!Content.Schedule.Contains(id) && id != PhaseSchedule.EndPhase)
                throw new ContentException($"goto_phase: unknown phase '{id}'");
            State.NextPhase = id;
        });
        _story.BindExternalFunction("headline", (string text) => State.Headlines.Add(text));
        _story.BindExternalFunction("played", (string id) => State.Plays(id), lookaheadSafe: true);
        _story.BindExternalFunction("why", () => { string c = _lastCause; _lastCause = ""; return c; });
        _story.BindExternalFunction("shift", (string trait, int delta, string reason) => OfficeMan.Core.World.Teo.Shift(State, trait, delta, reason));
    }
}
