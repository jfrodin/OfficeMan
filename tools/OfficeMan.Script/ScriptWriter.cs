using System.Text;
using System.Text.RegularExpressions;

namespace OfficeMan.Script;

/// <summary>
/// Writes knots as script. Teo's thoughts are plain paragraphs, speech is <c>**NAME:** words</c>, choices and
/// conditions are list items with what follows indented under them. Engine bookkeeping (memories, regard, flags)
/// is left out; what changes the story for a reader (headlines, money, hidden checks, where it goes next) stays.
/// </summary>
public sealed class ScriptWriter
{
    private readonly StringBuilder _out = new();
    private readonly Dictionary<string, string> _phaseTitles;
    private Knot _knot = null!;

    public ScriptWriter(Dictionary<string, string> phaseTitles) => _phaseTitles = phaseTitles;

    public override string ToString() => _out.ToString();

    public void Raw(string text) => _out.AppendLine(text);

    public void Knot(Knot knot, string heading)
    {
        _knot = knot;
        Raw($"### {heading}");
        Raw("");
        string meta = Meta(knot);
        if (meta.Length > 0) { Raw($"*{meta}*"); Raw(""); }
        Notes(knot.Top.Notes, 0);
        Block(knot.Top.Body, 0, knot.Top.Body.Count, 0);
        foreach (var stitch in knot.Stitches)
        {
            Raw($"#### {Conditions.Words(stitch.Id)}");
            Raw("");
            Notes(stitch.Notes, 0);
            Block(stitch.Body, 0, stitch.Body.Count, 0);
        }
    }

    /// <summary>Text lines and returned strings from a helper function (hindsight, Viktor's target, tickets).</summary>
    public bool FunctionLines(Knot knot)
    {
        var lines = knot.Top.Body
            .Select(l => l.Text)
            .Select(t => t.StartsWith("~ return \"") ? t["~ return \"".Length..].TrimEnd('"') : IsText(t) ? t : null)
            .Where(t => !string.IsNullOrEmpty(t))
            .ToList();
        if (lines.Count == 0) return false;
        string name = Conditions.Words(knot.Id);
        Raw($"### {char.ToUpperInvariant(name[0])}{name[1..]}");
        Raw("");
        Notes(knot.Top.Notes, 0);
        foreach (string t in lines) Raw($"- {Inline(t!)}");
        Raw("");
        return true;
    }

    private static bool IsText(string t) =>
        !(t.StartsWith('~') || t.StartsWith('{') || t.StartsWith('}') || t.StartsWith("//") || t.StartsWith("->") || t.StartsWith('-') || t.StartsWith('#'));

    // ── Storylet header ───────────────────────────────

    private string Meta(Knot k)
    {
        var parts = new List<string>();
        switch (k.Tag("kind"))
        {
            case "enter": parts.Add("On arrival"); break;
            case "interrupt": parts.Add(k.Tag("at") is { } at ? $"{at}, happens by itself" : "Happens by itself"); break;
            case "offstage": parts.Add(k.Tag("at") is { } at2 ? $"{at2}, offstage (Teo may only hear it)" : "Offstage (Teo may only hear it)"); break;
            case "exit": parts.Add("A way out"); break;
            case "timeout": parts.Add("When time runs out"); break;
            case "hub": parts.Add(k.Tag("where") is { } where ? $"Teo's choice, at: {Conditions.Words(where)}" : "Teo's choice"); break;
            case "scene": break;
        }
        if (k.Tag("window") is { } window) parts.Add($"open {window.Replace("-", " to ")}");
        if (k.Tag("when") is { } when) parts.Add($"only if {Conditions.Translate(when, k.Id)}");
        if (k.Tag("chance") is { } chance) parts.Add($"{chance}% chance");
        if (k.Tag("max") is { } max) parts.Add($"at most {max} times");
        if (k.Tags.ContainsKey("repeat")) parts.Add("can be done again");
        if (k.Tag("duration") is { } d && d != "0") parts.Add($"takes {d} min");
        parts.Add($"`{k.Id}`");
        return string.Join(" · ", parts);
    }

    // ── Body ──────────────────────────────────────────

    private void Block(List<Line> ls, int from, int to, int depth)
    {
        int i = from;
        while (i < to)
        {
            string t = ls[i].Text;
            if (t.StartsWith('{') && !t.Contains('}')) { i = Conditional(ls, i, to, depth); continue; }
            if (t.StartsWith('*') || t.StartsWith('+')) { i = Choices(ls, i, to, depth); continue; }
            if (t.StartsWith('-') && !t.StartsWith("->"))
            {
                // A gather that opens a block: the block follows the choices, at this level.
                string rest = t.TrimStart('-').Trim();
                if (rest.StartsWith('{') && !rest.Contains('}')) { ls[i] = ls[i] with { Text = rest }; continue; }
                Gather(t, depth);
                i++;
                continue;
            }
            if (t.StartsWith("//"))
            {
                // A comment over several lines is one note.
                var note = new List<string>();
                int noteIndent = ls[i].Indent;
                while (i < to && ls[i].Text.StartsWith("//") && ls[i].Indent == noteIndent)
                    note.Add(ls[i++].Text.TrimStart('/', ' '));
                Note(string.Join(" ", note), depth);
                continue;
            }
            Single(t, depth);
            i++;
        }
    }

    /// <summary>The end of the block that starts at <paramref name="i"/>: the first later line at the same indent or less.</summary>
    private static int End(List<Line> ls, int i, int to)
    {
        int j = i + 1;
        while (j < to && ls[j].Indent > ls[i].Indent) j++;
        return j;
    }

    private int Conditional(List<Line> ls, int i, int to, int depth)
    {
        int indent = ls[i].Indent;
        int close = i + 1;
        while (close < to && !(ls[close].Indent == indent && ls[close].Text == "}")) close++;

        string head = ls[i].Text.TrimStart('{').Trim();
        if (head.EndsWith(':')) head = head[..^1].Trim();
        var markers = Enumerable.Range(i + 1, close - i - 1)
            .Where(j => ls[j].Indent == indent && ls[j].Text.StartsWith('-') && !ls[j].Text.StartsWith("->"))
            .ToList();

        bool shuffle = head.StartsWith("shuffle") || head is "cycle" or "stopping" or "once";
        bool firstIsMarker = markers.Count > 0 && markers[0] == i + 1;

        if (shuffle)
        {
            Item(depth, head.Contains("once") ? "*One of these, at random, each only once:*" : "*One of these, at random:*", bullet: true);
            for (int m = 0; m < markers.Count; m++)
            {
                int end = m + 1 < markers.Count ? markers[m + 1] : close;
                string first = ls[markers[m]].Text.TrimStart('-').Trim();
                Item(depth + 1, Format(first), bullet: true);
                Block(ls, markers[m] + 1, end, depth + 2);
            }
        }
        else if (head.Length > 0 && !firstIsMarker)
        {
            // { cond: ... - else: ... }
            int end = markers.Count > 0 ? markers[0] : close;
            Item(depth, $"*If {Conditions.Translate(head, _knot.Id)}:*", bullet: true);
            Block(ls, i + 1, end, depth + 1);
            for (int m = 0; m < markers.Count; m++)
            {
                int stop = m + 1 < markers.Count ? markers[m + 1] : close;
                Item(depth, "*Otherwise:*", bullet: true);
                Block(ls, markers[m] + 1, stop, depth + 1);
            }
        }
        else
        {
            // { - cond: ... - cond: ... - else: ... }, or a switch: { r: - 0: ... - 1: ... }
            string subject = Regex.Replace(head, @"^q\(""(\w+)""\)$", "$1");
            bool check = Conditions.CheckVars.Contains(subject);
            for (int m = 0; m < markers.Count; m++)
            {
                int end = m + 1 < markers.Count ? markers[m + 1] : close;
                string marker = ls[markers[m]].Text.TrimStart('-').Trim();
                int colon = ColonOutside(marker);
                string cond = colon < 0 ? marker : marker[..colon].Trim();
                string rest = colon < 0 ? "" : marker[(colon + 1)..].Trim();
                string label = cond == "else" ? "Otherwise"
                    : check && int.TryParse(cond, out int r) ? $"If it {Conditions.CheckResult(r)}"
                    : head.Length > 0 ? $"If `{subject}` is {cond}"
                    : $"{(m == 0 ? "If" : "Or if")} {Conditions.Translate(cond, _knot.Id)}";
                Item(depth, $"*{label}:*", bullet: true);
                if (rest.Length > 0) Single(rest, depth + 1);
                Block(ls, markers[m] + 1, end, depth + 1);
            }
        }
        return Math.Min(close + 1, to);
    }

    private int Choices(List<Line> ls, int i, int to, int depth)
    {
        int indent = ls[i].Indent;
        while (i < to && ls[i].Indent == indent && (ls[i].Text.StartsWith('*') || ls[i].Text.StartsWith('+')))
        {
            int end = End(ls, i, to);
            string t = ls[i].Text;
            bool sticky = t.StartsWith('+');
            t = t.TrimStart('*', '+', ' ');

            var conds = new List<string>();
            while (t.StartsWith('{'))
            {
                int close = Matching(t, 0);
                conds.Add(Conditions.Translate(t[1..close], _knot.Id));
                t = t[(close + 1)..].TrimStart();
            }
            string label = t, after = "";
            if (t.StartsWith('['))
            {
                int close = t.IndexOf(']');
                label = t[1..close];
                after = t[(close + 1)..].Trim();
            }
            var (words, tags) = SplitTags(label);
            var notes = new List<string>();
            notes.AddRange(tags.Where(tag => !tag.Contains(':')).Select(tag => $"[{tag}]"));
            if (conds.Count > 0) notes.Add($"(only if {string.Join(" and ", conds)})");
            if (sticky) notes.Add("(can be picked again)");

            // A choice that only leads somewhere says where on the same line.
            string jump = "";
            if (after.Length == 0 && end == i + 2 && ls[i + 1].Text.StartsWith("->") && !ls[i + 1].Text.StartsWith("->->"))
            {
                jump = DivertText(ls[i + 1].Text);
                if (jump.Length > 0) end = i + 1;
            }
            Item(depth, $"▸ **{Inline(words)}**{(notes.Count > 0 ? " *" + string.Join(" ", notes) + "*" : "")}{(jump.Length > 0 ? $" *{jump}*" : "")}", bullet: true);
            if (after.Length > 0) Single(after, depth + 1);
            Block(ls, i + 1, end, depth + 1);
            if (jump.Length > 0) end = i + 2;
            i = end;
        }
        return i;
    }

    private void Gather(string t, int depth)
    {
        string rest = t.TrimStart('-').Trim();
        var label = Regex.Match(rest, @"^\((\w+)\)\s*");
        if (label.Success)
        {
            rest = rest[label.Length..];
        }
        if (rest.Length > 0) Single(rest, depth);
    }

    private void Single(string t, int depth)
    {
        if (t == "}" || t.StartsWith('#')) return;
        if (t.StartsWith("//")) { Note(t.TrimStart('/', ' '), depth); return; }
        if (t.StartsWith('~')) { Logic(t, depth); return; }
        if (t.StartsWith("->")) { Divert(t, depth); return; }

        string divert = "";
        int arrow = t.IndexOf(" -> ", StringComparison.Ordinal);
        if (arrow > 0) { divert = t[(arrow + 1)..]; t = t[..arrow]; }

        Item(depth, Format(t));
        if (divert.Length > 0) Divert(divert, depth);
    }

    /// <summary>A line of text: speech as <c>**NAME:** words</c>, with how it's said; anything else as it is.</summary>
    private static string Format(string t)
    {
        var (words, tags) = SplitTags(t);
        var speech = Regex.Match(words, @"^([A-Z][A-Z' ]+):\s+(.*)$");
        if (!speech.Success) return Inline(words);
        var how = tags.Where(x => x.StartsWith("mood:") || x.StartsWith("heard:"))
            .Select(x => x[(x.IndexOf(':') + 1)..].Trim()).ToList();
        string paren = how.Count > 0 ? $" *({string.Join(", ", how)})*" : "";
        return $"**{speech.Groups[1].Value}:**{paren} {Inline(speech.Groups[2].Value)}";
    }

    /// <summary>A writer's comment, kept for what it says about intent; references to files are left out.</summary>
    private void Note(string t, int depth)
    {
        if (t.Contains('═') || t.Contains("──")) return;
        t = Regex.Replace(t, @"\s*\([^()]*(\.json|\.ink|\.cs|\.md|/)[^()]*\)", "");
        if (t.Trim().Length > 0) Item(depth, $"*(Note: {t.Trim()})*");
    }

    private void Logic(string t, int depth)
    {
        string code = t.TrimStart('~').Trim();
        Match m;
        if ((m = Regex.Match(code, @"temp (\w+) = check\(""(\w+)"",\s*""(\w+)""\)")).Success)
        {
            Conditions.CheckVars.Add(m.Groups[1].Value);
            Item(depth, m.Groups[3].Value == "luck" ? "*(Left to luck.)*" : $"*(A hidden check on Teo's {m.Groups[3].Value}.)*");
        }
        else if ((m = Regex.Match(code, @"headline\(""(.*)""\)")).Success)
            Item(depth, $"*[Headline: {Inline(m.Groups[1].Value)}]*");
        else if ((m = Regex.Match(code, @"goto_phase\(""(\w+)""\)")).Success)
            Item(depth, $"*[Goes to: {_phaseTitles.GetValueOrDefault(m.Groups[1].Value, m.Groups[1].Value)}]*");
        else if ((m = Regex.Match(code, @"q(set|add)\(""money"",\s*(-?\d+)\)")).Success)
            Item(depth, m.Groups[1].Value == "set" ? $"*[Teo has {m.Groups[2].Value} euro.]*"
                : $"*[Money {(m.Groups[2].Value.StartsWith('-') ? "" : "+")}{m.Groups[2].Value} euro]*");
        else if (code.StartsWith("stay()"))
            Item(depth, "*[He doesn't leave after all.]*");
        else if (code.StartsWith("use_ticket()"))
            Item(depth, "*[Uses a drink ticket, unless the bar is open.]*");
        else if ((m = Regex.Match(code, @"^set_doing\(""(\w+)"",\s*""(\w+)""\)")).Success)
            Item(depth, $"*[{Conditions.Who(m.Groups[1].Value)} is now at/doing: {Conditions.Words(m.Groups[2].Value)}]*");
    }

    private void Divert(string t, int depth)
    {
        string text = DivertText(t);
        if (text.Length > 0) Item(depth, $"*{text}*");
    }

    private static string DivertText(string t)
    {
        string target = t[2..].Trim();
        if (target is "DONE" or "END" or "" || target.StartsWith("->")) return "";
        bool tunnel = target.EndsWith("->");
        target = target.TrimEnd('-', '>', ' ');
        string label = target == "opts" ? "back to the choices" : Conditions.Words(target.Split('.')[^1]);
        return tunnel ? $"→ {label} (then back here)" : $"→ {label}";
    }

    // ── Text ──────────────────────────────────────────

    private static (string Words, List<string> Tags) SplitTags(string t)
    {
        int hash = t.IndexOf('#');
        if (hash < 0) return (t.Trim(), new());
        var tags = t[(hash + 1)..].Split('#').Select(x => x.Trim()).Where(x => x.Length > 0).ToList();
        return (t[..hash].Trim(), tags);
    }

    private static string Inline(string t)
    {
        t = t.Replace("<i>", "*").Replace("</i>", "*");
        t = t.Replace("{viktor_target()}", "[Felicity / the maid of honour / the new waitress]");
        t = t.Replace("{hindsight()}", "*[a hindsight line, see Shared lines]*");
        t = Regex.Replace(t, @"\{(\w+)\(\)\}", m => $"[{Conditions.Words(m.Groups[1].Value)}]");
        return t;
    }

    private void Notes(List<string> notes, int depth)
    {
        if (notes.Count > 0) Note(string.Join(" ", notes.Select(n => n.TrimStart('/', ' '))), depth);
    }

    private static int Matching(string t, int open)
    {
        int level = 0;
        for (int k = open; k < t.Length; k++)
        {
            if (t[k] == '{') level++;
            else if (t[k] == '}' && --level == 0) return k;
        }
        return t.Length - 1;
    }

    private static int ColonOutside(string t)
    {
        int level = 0;
        bool quoted = false;
        for (int k = 0; k < t.Length; k++)
        {
            char c = t[k];
            if (c == '"') quoted = !quoted;
            else if (!quoted && c == '(') level++;
            else if (!quoted && c == ')') level--;
            else if (!quoted && level == 0 && c == ':') return k;
        }
        return -1;
    }

    /// <summary>One paragraph at a nesting depth; each depth is a list level (two spaces).</summary>
    private void Item(int depth, string text, bool bullet = false)
    {
        string pad = new(' ', depth * 2);
        _out.AppendLine(bullet ? $"{pad}- {text}" : $"{pad}{text}");
        _out.AppendLine();
    }
}
