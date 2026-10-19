using System.Text;
using System.Text.RegularExpressions;
using OfficeMan.Core;
using OfficeMan.Core.Content;

// Text-only player for the prototype.
//   dotnet run --project src/OfficeMan.Cli -- [--seed N] [--debug] [--fast]
// During play: a number picks a choice; s = save, l = load, d = toggle debug, q = quit.
// Text types out at reading pace; any key shows the rest of the passage at once. --fast turns that off.

Console.OutputEncoding = Encoding.UTF8;

int seed = Environment.TickCount & 0x7fffffff;
bool debug = false;
bool fast = false;
for (int i = 0; i < args.Length; i++)
{
    if (args[i] == "--seed" && i + 1 < args.Length) seed = int.Parse(args[++i]);
    else if (args[i] == "--debug") debug = true;
    else if (args[i] == "--fast") fast = true;
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
var ui = new TextUi { Animate = !fast && !Console.IsInputRedirected && !Console.IsOutputRedirected };

Console.WriteLine();
ui.Dim($"THE MISADVENTURES OF OFFICE MAN, prototype · life #{seed}");
if (ui.Animate) ui.Dim("(Press any key to show text instantly.)");
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
        ui.Dim($"Pick 1-{view.Choices.Count}. (s save · l load · d debug · q quit)");
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

    // Reading pace: per visible character, plus a breath between lines.
    private const int CharDelayMs = 12;
    private const int LinePauseMs = 280;
    private const int HeaderPauseMs = 500;

    /// <summary>Type text out at reading pace. Off when input or output is redirected.</summary>
    public bool Animate { get; init; }

    private bool _skipping;

    public void Show(Presentation view)
    {
        _skipping = false;
        foreach (var line in view.Lines)
        {
            if (line.Kind == LineKind.Header)
            {
                if (line.Tags.Contains("title:")) continue; // untitled phase: nothing to announce
                Pause(HeaderPauseMs);
                Console.WriteLine();
                Console.WriteLine($"{Bold}{Yellow}── {line.Text} ──{Reset}");
                Console.WriteLine();
                Pause(HeaderPauseMs);
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
        if (view.Kind == PresentationKind.Hub) Dim($"{view.Clock}. What now?");
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
        if (view.Today.Count > 0)
        {
            Console.WriteLine();
            foreach (string t in view.Today) Console.WriteLine($"  {Markup(t)}");
        }
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

    private void Write(string text, int indent)
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
                TypeOut(line.ToString());
                Console.WriteLine();
                int hang = indent == 0 ? 0 : indent + 4;
                line.Clear().Append(new string(' ', hang));
                visible = hang;
            }
            if (visible > indent && line.Length > 0 && line[^1] != ' ') { line.Append(' '); visible++; }
            line.Append(word);
            visible += len;
        }
        TypeOut(line.ToString());
        Console.WriteLine();
        Console.WriteLine();
        Pause(LinePauseMs);
    }

    /// <summary>Writes text a character at a time; escape codes are written whole.</summary>
    private void TypeOut(string text)
    {
        if (!Animate || _skipping)
        {
            Console.Write(text);
            return;
        }
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '\u001b')
            {
                int end = text.IndexOf('m', i);
                Console.Write(text[i..(end + 1)]);
                i = end;
                continue;
            }
            Console.Write(text[i]);
            if (SkipRequested())
            {
                Console.Write(text[(i + 1)..]);
                return;
            }
            if (text[i] != ' ') Thread.Sleep(CharDelayMs);
        }
    }

    private void Pause(int ms)
    {
        if (!Animate || _skipping) return;
        var until = DateTime.UtcNow.AddMilliseconds(ms);
        while (DateTime.UtcNow < until)
        {
            if (SkipRequested()) return;
            Thread.Sleep(15);
        }
    }

    /// <summary>Any key press skips to the end of the passage. The key is swallowed.</summary>
    private bool SkipRequested()
    {
        while (Console.KeyAvailable)
        {
            Console.ReadKey(intercept: true);
            _skipping = true;
        }
        return _skipping;
    }

    [GeneratedRegex("<i>(.*?)</i>")]
    private static partial Regex ItalicTag();

    [GeneratedRegex("\u001b\\[[0-9;]*m")]
    private static partial Regex AnsiCode();
}
