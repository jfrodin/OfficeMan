using System.Text;
using System.Text.RegularExpressions;
using OfficeMan.Core;
using OfficeMan.Core.Content;

// Text-only player for the prototype.
//   dotnet run --project src/OfficeMan.Cli -- [--seed N] [--debug]
// During play: a number picks a choice; s = save, l = load, d = toggle debug, q = quit.

Console.OutputEncoding = Encoding.UTF8;

int seed = Environment.TickCount & 0x7fffffff;
bool debug = false;
for (int i = 0; i < args.Length; i++)
{
    if (args[i] == "--seed" && i + 1 < args.Length) seed = int.Parse(args[++i]);
    else if (args[i] == "--debug") debug = true;
}

ContentBundle content;
try
{
    content = ContentBundle.Load(ContentLocator.Find());
}
catch (ContentException e)
{
    Console.Error.WriteLine(e.Message);
    return 1;
}

string savePath = Path.Combine(AppContext.BaseDirectory, "saves", "slot1.json");
var game = Game.New(content, seed);
var ui = new TextUi();

Console.WriteLine();
ui.Dim($"THE MISADVENTURES OF OFFICE MAN — prototype · life #{seed}");
Console.WriteLine();

while (true)
{
    var view = game.Current;
    ui.Show(view);
    if (debug) ui.Debug(game);

    if (view.Kind == PresentationKind.End)
    {
        ui.Ending(view);
        return 0;
    }

    while (true)
    {
        Console.Write("\n> ");
        string? input = Console.ReadLine()?.Trim().ToLowerInvariant();
        if (input is null || input == "q") return 0;

        if (input == "d")
        {
            debug = !debug;
            if (debug) ui.Debug(game);
            continue;
        }
        if (input == "s")
        {
            Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);
            File.WriteAllText(savePath, game.Save().ToJson());
            ui.Dim($"Saved to {savePath}");
            continue;
        }
        if (input == "l")
        {
            if (!File.Exists(savePath)) { ui.Dim("No save yet."); continue; }
            game = Game.Load(content, SaveGame.FromJson(File.ReadAllText(savePath)));
            ui.Dim("Loaded.");
            break;
        }
        if (int.TryParse(input, out int n) && n >= 1 && n <= view.Choices.Count)
        {
            Console.WriteLine();
            game.Choose(n - 1);
            break;
        }
        ui.Dim($"Pick 1–{view.Choices.Count}. (s save · l load · d debug · q quit)");
    }
}

sealed partial class TextUi
{
    private const int Width = 78;
    private const string Reset = "\u001b[0m";
    private const string Bold = "\u001b[1m";
    private const string DimCode = "\u001b[2m";
    private const string Italic = "\u001b[3m";
    private const string Yellow = "\u001b[33m";
    private const string Cyan = "\u001b[36m";

    public void Show(Presentation view)
    {
        foreach (var line in view.Lines)
        {
            if (line.Kind == LineKind.Header)
            {
                Console.WriteLine();
                Console.WriteLine($"{Bold}{Yellow}── {line.Text} ──{Reset}");
                Console.WriteLine();
                continue;
            }

            if (line.Speaker is { } speaker)
            {
                string body = line.Body.StartsWith('"') ? Markup(line.Body) : $"“{Markup(line.Body)}”";
                Write($"{Bold}{speaker}{Reset}  {body}", indent: 2);
            }
            else
            {
                Write(Markup(line.Text), indent: 0);
            }
        }

        if (view.Choices.Count == 0) return;

        Console.WriteLine();
        if (view.Kind == PresentationKind.Hub) Dim($"{view.Clock} — what now?");
        for (int i = 0; i < view.Choices.Count; i++)
        {
            var c = view.Choices[i];
            string mark = c.Tags.Contains("exit") ? " →" : "";
            Console.WriteLine($"  {Cyan}{i + 1}.{Reset} {Markup(c.Text)}{mark}");
        }
    }

    public void Ending(Presentation view)
    {
        Console.WriteLine();
        Console.WriteLine($"{Bold}{Yellow}── To be continued ──{Reset}");
        if (view.Headlines.Count > 0)
        {
            Console.WriteLine();
            Dim("What happened:");
            foreach (string h in view.Headlines) Console.WriteLine($"  · {h}");
        }
        Console.WriteLine();
    }

    public void Debug(Game game)
    {
        var s = game.State;
        string derived = string.Join("  ", OfficeMan.Core.World.Teo.DerivedNames.Take(6)
            .Select(n => $"{n}={OfficeMan.Core.World.Teo.Value(s, n)}"));
        Dim($"[debug] {OfficeMan.Core.World.Teo.Clock(s)} · phase={s.PhaseId} · {derived}");
        Dim("[debug] " + string.Join(" ", s.Qualities.Where(q => !q.Key.StartsWith('_')).Select(q => $"{q.Key}={q.Value}")));
        Dim("[debug] regard: " + string.Join(" ", s.Characters.Select(c => $"{c.Key}={c.Value.Regard}")));
        foreach (string check in game.CheckLog.TakeLast(3)) Dim($"[debug] check {check}");
    }

    public void Dim(string text) => Console.WriteLine($"{DimCode}{text}{Reset}");

    private static string Markup(string text) =>
        ItalicTag().Replace(text, m => $"{Italic}{m.Groups[1].Value}{Reset}");

    private static void Write(string text, int indent)
    {
        // Word-wrap by visible length (ANSI codes don't count).
        var words = text.Split(' ');
        var line = new StringBuilder(new string(' ', indent));
        int visible = indent;
        foreach (string word in words)
        {
            int len = AnsiCode().Replace(word, "").Length;
            if (visible + len + 1 > Width && visible > indent)
            {
                Console.WriteLine(line.ToString());
                int hang = indent == 0 ? 0 : indent + 4;
                line.Clear().Append(new string(' ', hang));
                visible = hang;
            }
            if (visible > indent && line.Length > 0 && line[^1] != ' ') { line.Append(' '); visible++; }
            line.Append(word);
            visible += len;
        }
        Console.WriteLine(line.ToString());
        Console.WriteLine();
    }

    [GeneratedRegex("<i>(.*?)</i>")]
    private static partial Regex ItalicTag();

    [GeneratedRegex("\u001b\\[[0-9;]*m")]
    private static partial Regex AnsiCode();
}
