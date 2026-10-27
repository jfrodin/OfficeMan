using System.Diagnostics;
using System.Text.Json;
using OfficeMan.Core.Content;
using OfficeMan.Script;

// Writes the whole game's text as one script to read, edit or hand to someone: design/script.md.
//   dotnet run --project tools/OfficeMan.Script [-- --out path] [--force]
// The ink in content/ink stays the source. Edits made in the script are carried back into the ink by hand, so this
// refuses to overwrite a script with uncommitted changes unless --force is given.

string content = ContentLocator.Find();
string repo = Path.GetDirectoryName(content)!;
string outPath = Path.Combine(repo, "design", "script.md");
bool force = false;
for (int i = 0; i < args.Length; i++)
{
    if (args[i] == "--out") outPath = Path.GetFullPath(args[++i]);
    else if (args[i] == "--force") force = true;
}

if (!force && File.Exists(outPath) && HasUncommittedChanges(repo, outPath))
{
    Console.Error.WriteLine($"{outPath} has changes that aren't committed (edits to carry into the ink?). Use --force to overwrite.");
    return 1;
}

using var phasesJson = JsonDocument.Parse(File.ReadAllText(Path.Combine(content, "phases.json")),
    new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true });
var phases = phasesJson.RootElement.GetProperty("phases").EnumerateArray().Select(p => new
{
    Id = p.GetProperty("id").GetString()!,
    Title = p.GetProperty("title").GetString()!,
    Day = p.GetProperty("day").GetInt32(),
    Start = p.TryGetProperty("start", out var s) ? s.GetString() : null,
}).ToList();
var titles = phases.ToDictionary(p => p.Id, p => p.Title.Length > 0 ? p.Title : "The end");

string ink = Path.Combine(content, "ink");
var files = new[] { "tuesday_home", "tuesday_office", "tuesday_party", "tuesday_people", "tuesday_night", "wednesday", "lib" }
    .Select(f => Path.Combine(ink, f + ".ink"));
var knots = InkFile.Read(files);
var unknown = Directory.GetFiles(ink, "*.ink").Select(Path.GetFileNameWithoutExtension)
    .Except(new[] { "main", "tuesday_home", "tuesday_office", "tuesday_party", "tuesday_people", "tuesday_night", "wednesday", "lib" }).ToList();
if (unknown.Count > 0) Console.Error.WriteLine($"warning: not in the script: {string.Join(", ", unknown)} (add them to the list in Program.cs)");

var w = new ScriptWriter(titles);
w.Raw("# The Misadventures of Office Man: the script");
w.Raw("");
w.Raw("Every line in the game, in the order a day can run. Written out from `content/ink` (`dotnet run --project tools/OfficeMan.Script`); the ink is still the source, so changes made here are carried back into it by hand.");
w.Raw("");
w.Raw("How to read it:");
w.Raw("");
w.Raw("- Plain paragraphs are Teo's own voice: what he sees and thinks. **NAME:** is someone speaking.");
w.Raw("- **▸ Bold with an arrow** is a choice the player makes. Tags after a choice say what kind it is: *[bold]*, *[cautious]*, *[kind]*, *[drink]*.");
w.Raw("- *If ...:* and *Otherwise:* are variations. Only one of them plays. What follows each is indented under it.");
w.Raw("- Teo's state: drunk runs sober, tipsy, drunk, very drunk, wrecked. Mood runs from low (-2) to good (+2). His traits are lines with two ends: nerve (bold or careful), warmth (open or closed), appetite (hungry or held back), honesty (blunt or smooth). Regard is what someone thinks of Teo, -10 to 10.");
w.Raw("- A *hidden check* uses one of Teo's traits plus luck, and it goes badly, half well or well. The player never sees a number.");
w.Raw("- Names in `backticks` are things the game remembers (things Teo did or found out). The italic line under each heading says when a scene can happen, and its name in the files.");
w.Raw("");

foreach (var phase in phases)
{
    var here = knots.Where(k => !k.IsFunction && k.Tag("phase") == phase.Id).ToList();
    if (here.Count == 0) continue;
    string day = phase.Day == 0 ? "TUESDAY" : "WEDNESDAY";
    string when = phase.Start ?? (phase.Day == 0 ? "NIGHT" : "");
    w.Raw($"## {day}{(when.Length > 0 ? " " + when : "")}. {titles[phase.Id].ToUpperInvariant()}");
    w.Raw("");
    foreach (var knot in Ordered(here))
        w.Knot(knot, Heading(knot));
}

w.Raw("## SHARED LINES");
w.Raw("");
w.Raw("Passages and lines used in several places.");
w.Raw("");
foreach (var knot in knots.Where(k => !k.IsFunction && k.Tag("phase") is null && k.Id is not "init"))
    w.Knot(knot, knot.Id == "today" ? "The end screen: who Teo was today" : Heading(knot));
foreach (var knot in knots.Where(k => k.IsFunction))
    w.FunctionLines(knot);

string text = w.ToString().TrimEnd() + Environment.NewLine;
Directory.CreateDirectory(Path.GetDirectoryName(outPath)!);
File.WriteAllText(outPath, text);
Console.WriteLine($"Wrote {outPath} ({text.Split('\n').Length} lines, {knots.Count(k => !k.IsFunction)} scenes).");
return 0;

static IEnumerable<Knot> Ordered(List<Knot> here)
{
    int Rank(Knot k) => k.Tag("kind") switch
    {
        "enter" => 0, "interrupt" or "offstage" => 1, "hub" => 2, "scene" => 3, "exit" => 4, "timeout" => 5, _ => 6,
    };
    // Things that happen on their own in time order; the rest as the writer ordered them.
    return here.Select((k, i) => (k, i))
        .OrderBy(x => Rank(x.k))
        .ThenBy(x => Rank(x.k) == 1 ? (x.k.Tag("at") ?? "99:99").PadLeft(5, '0') : "")
        .ThenBy(x => x.i)
        .Select(x => x.k);
}

static string Heading(Knot k)
{
    if (k.Tag("kind") is "hub" or "exit" && k.Tag("choice") is { } choice) return choice;
    string words = k.Id.Replace('_', ' ');
    return char.ToUpperInvariant(words[0]) + words[1..];
}

static bool HasUncommittedChanges(string repo, string path)
{
    try
    {
        var git = Process.Start(new ProcessStartInfo("git", $"status --porcelain -- \"{path}\"")
        {
            WorkingDirectory = repo, RedirectStandardOutput = true, UseShellExecute = false,
        })!;
        string output = git.StandardOutput.ReadToEnd();
        git.WaitForExit();
        return output.Trim().Length > 0 && !output.StartsWith("??");
    }
    catch { return false; }
}
