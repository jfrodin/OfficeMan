using System.Text.RegularExpressions;
using Ink;

namespace OfficeMan.Core.Content;

/// <summary>
/// Compiled, validated game content: the ink story, every storylet, and the phase schedule.
/// Immutable and shareable between many game instances (e.g. in the simulator).
/// </summary>
public sealed class ContentBundle
{
    public string StoryJson { get; }
    public IReadOnlyList<Storylet> Storylets { get; }
    public PhaseSchedule Schedule { get; }
    public IReadOnlyList<string> Warnings { get; }

    /// <summary>Every knot in the story, storylet or not.</summary>
    public IReadOnlySet<string> KnotNames { get; }

    private readonly Dictionary<string, Storylet> _byId;

    private ContentBundle(string storyJson, List<Storylet> storylets, PhaseSchedule schedule, List<string> warnings, HashSet<string> knots)
    {
        KnotNames = knots;
        StoryJson = storyJson;
        Storylets = storylets;
        Schedule = schedule;
        Warnings = warnings;
        _byId = storylets.ToDictionary(s => s.Id);
    }

    public Storylet Get(string id) => _byId[id];

    public bool TryGet(string id, out Storylet storylet) => _byId.TryGetValue(id, out storylet!);

    /// <summary>
    /// Compiles <c>main.ink</c> in <paramref name="contentDir"/> and reads <c>phases.json</c>.
    /// Every knot carrying a <c># phase:</c> tag becomes a storylet.
    /// </summary>
    public static ContentBundle Load(string contentDir)
    {
        string inkDir = Path.Combine(contentDir, "ink");
        string mainPath = Path.Combine(inkDir, "main.ink");

        var errors = new List<string>();
        var warnings = new List<string>();
        var compiler = new Compiler(File.ReadAllText(mainPath), new Compiler.Options
        {
            sourceFilename = mainPath,
            countAllVisits = true,
            fileHandler = new DirectoryFileHandler(inkDir),
            errorHandler = (message, type) =>
            {
                if (type == ErrorType.Error) errors.Add(message);
                else warnings.Add(message);
            },
        });

        var story = compiler.Compile();
        if (story is null || errors.Count > 0)
            throw new ContentException("ink compilation failed:\n" + string.Join("\n", errors));

        var knots = KnotNamesInSourceOrder(inkDir).ToList();
        var storylets = new List<Storylet>();
        foreach (string knot in knots)
        {
            var tags = story.TagsForContentAtPath(knot);
            if (tags is null || !tags.Exists(t => t.TrimStart().StartsWith("phase:", StringComparison.OrdinalIgnoreCase)))
                continue;
            try { storylets.Add(Storylet.FromTags(knot, tags)); }
            catch (FormatException e) { errors.Add(e.Message); }
        }

        var schedule = PhaseSchedule.FromJson(File.ReadAllText(Path.Combine(contentDir, "phases.json")));
        Validate(storylets, schedule, errors, warnings);

        if (errors.Count > 0)
            throw new ContentException("content validation failed:\n" + string.Join("\n", errors));

        return new ContentBundle(story.ToJson(), storylets, schedule, warnings, knots.ToHashSet());
    }

    private static void Validate(List<Storylet> storylets, PhaseSchedule schedule, List<string> errors, List<string> warnings)
    {
        if (!schedule.Contains(schedule.FirstPhase))
            errors.Add($"phases.json: first phase '{schedule.FirstPhase}' does not exist");

        foreach (var phase in schedule.Phases)
        {
            if (phase.Next != PhaseSchedule.EndPhase && !schedule.Contains(phase.Next))
                errors.Add($"phase '{phase.Id}': next phase '{phase.Next}' does not exist");

            var inPhase = storylets.Where(s => s.InPhase(phase.Id)).ToList();
            if (inPhase.Count == 0)
                warnings.Add($"phase '{phase.Id}' has no storylets");

            if (phase.Mode == PhaseMode.Hub)
            {
                if (phase.End is null) errors.Add($"hub phase '{phase.Id}' needs an end time");
                if (!inPhase.Exists(s => s.Kind == StoryletKind.Exit))
                    warnings.Add($"hub phase '{phase.Id}' has no exit storylet");
            }
        }

        foreach (var s in storylets)
        {
            foreach (string p in s.Phases)
                if (!schedule.Contains(p)) errors.Add($"storylet '{s.Id}': unknown phase '{p}'");
            if (s.Kind is StoryletKind.Hub or StoryletKind.Exit && string.IsNullOrEmpty(s.ChoiceText))
                errors.Add($"storylet '{s.Id}': hub storylets need a '# choice:' tag");
        }
    }

    private static readonly Regex KnotHeader = new(@"^\s*={2,}\s*(?!function\b)(\w+)", RegexOptions.Multiline);

    private static IEnumerable<string> KnotNamesInSourceOrder(string inkDir)
    {
        // main.ink first, then includes in file-name order; knot order within a file is preserved.
        var files = Directory.GetFiles(inkDir, "*.ink", SearchOption.AllDirectories)
            .OrderBy(f => Path.GetFileName(f) == "main.ink" ? 0 : 1)
            .ThenBy(f => f, StringComparer.OrdinalIgnoreCase);

        foreach (string file in files)
            foreach (Match m in KnotHeader.Matches(File.ReadAllText(file)))
                yield return m.Groups[1].Value;
    }

    private sealed class DirectoryFileHandler(string root) : IFileHandler
    {
        public string ResolveInkFilename(string includeName) => Path.Combine(root, includeName);
        public string LoadInkFileContents(string fullFilename) => File.ReadAllText(fullFilename);
    }
}

public sealed class ContentException(string message) : Exception(message);
