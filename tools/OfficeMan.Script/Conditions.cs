using System.Text.RegularExpressions;

namespace OfficeMan.Script;

/// <summary>
/// Turns conditions into plain words, both ink's (<c>drunk() >= 2 &amp;&amp; is("open_bar")</c>) and the storylet
/// <c># when:</c> kind (<c>drunk >= 2 and open_bar</c>). Anything it doesn't know stays readable as a name in
/// backticks, so nothing is hidden.
/// </summary>
public static class Conditions
{
    private static readonly string[] DrunkLevels = { "sober", "tipsy", "drunk", "very drunk", "wrecked" };

    // Each line: the stat, what it's called when high, what it's called when low.
    private static readonly Dictionary<string, (string High, string Low)> Traits = new()
    {
        ["mood"] = ("in a good mood", "in a low mood"),
        ["nerve"] = ("bold", "careful"),
        ["warmth"] = ("open", "closed"),
        ["appetite"] = ("hungry", "held back"),
        ["honesty"] = ("blunt", "smooth"),
    };

    private static readonly Dictionary<string, string> SleptAt = new()
    {
        ["0"] = "at home", ["1"] = "at home", ["2"] = "at Magda's", ["3"] = "at Kornel's", ["4"] = "at the sobering-up station",
    };

    private const string Value = @"(?<rhs>\d+:\d\d|-?\d+(?:\s*[*+]\s*\d+)*|[a-z_]+(?:\s*\+\s*\d+)?)";
    private static readonly Regex Comparison = new(
        @"(?<lhs>[a-z_]+\([^()]*\)|[a-z_]+)\s*(?<op>>=|<=|==|!=|<|>)\s*" + Value);
    private static readonly Regex Call = new(@"(?<name>[a-z_]+)\((?<args>[^()]*)\)");
    private static readonly Regex Name = new(@"\b(?<name>[a-z_][a-z0-9_]*)\b");

    /// <summary>Temporary variables that hold the result of a hidden check (r, r2, roll); the writer adds them.</summary>
    public static readonly HashSet<string> CheckVars = new() { "r" };

    /// <summary>The current knot, so <c>played("this knot")</c> can read as "the first time".</summary>
    public static string Translate(string expr, string? here = null)
    {
        var parts = new List<string>();
        string Hold(string words) { parts.Add(words); return $"\u0001{parts.Count - 1}\u0002"; }

        string s = expr.Trim();
        s = s.Replace("&&", " and ").Replace("||", " or ");
        s = Regex.Replace(s, @"\bq\(""(\w+)""\)", "$1");
        s = Regex.Replace(s, @"(\w+\([^()]*\)|\w+)\s*==\s*false\b", "not $1");
        s = Regex.Replace(s, @"(\w+\([^()]*\)|\w+)\s*==\s*true\b", "$1");
        s = Regex.Replace(s, @"!(?!=)", "not ");
        s = Comparison.Replace(s, m => Hold(Compare(m.Groups["lhs"].Value, m.Groups["op"].Value, m.Groups["rhs"].Value.Trim(), here)));
        s = Call.Replace(s, m => Hold(Truth(m.Groups["name"].Value, Args(m.Groups["args"].Value))));
        s = Name.Replace(s, m => m.Value is "and" or "or" or "not" ? m.Value : Hold(Flag(m.Value)));
        s = Regex.Replace(s, @"\s+", " ").Trim();
        s = Regex.Replace(s, "\u0001(\\d+)\u0002", m => parts[int.Parse(m.Groups[1].Value)]);
        return s;
    }

    private static string[] Args(string args) =>
        args.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(a => a.Trim('"')).ToArray();

    private static string Compare(string lhs, string op, string rhs, string? here)
    {
        string name = lhs, arg = "";
        var call = Call.Match(lhs);
        if (call.Success)
        {
            var args = Args(call.Groups["args"].Value);
            name = call.Groups["name"].Value;
            if (name == "q" && args.Length == 1) name = args[0];
            else if (args.Length > 0) arg = args[0];
        }

        if (Traits.TryGetValue(name, out var pole) && int.TryParse(rhs, out int t))
        {
            bool up = op is ">=" or ">", down = op is "<=" or "<";
            if (up && (t > 0 || (t == 0 && op == ">"))) return $"Teo is {pole.High} ({name} {Range(op, rhs)})";
            if (down && (t < 0 || (t == 0 && op == "<"))) return $"Teo is {pole.Low} ({name} {Range(op, rhs)})";
            return $"{name} {Range(op, rhs)}";
        }

        switch (name)
        {
            case "drunk" when int.TryParse(rhs, out int d) && d >= 0 && d < DrunkLevels.Length:
                return op switch
                {
                    ">=" => d == 0 ? "always" : $"Teo is {DrunkLevels[d]} or worse",
                    ">" => $"Teo is {DrunkLevels[Math.Min(d + 1, 4)]} or worse",
                    "<=" => d == 0 ? "Teo is sober" : $"Teo is {DrunkLevels[d]} at most",
                    "<" => d == 1 ? "Teo is sober" : $"Teo is {DrunkLevels[d - 1]} at most",
                    "==" => $"Teo is {DrunkLevels[d]}",
                    _ => $"Teo isn't {DrunkLevels[d]}",
                };
            case "money":
                return op switch
                {
                    ">=" => $"Teo has {rhs} euro or more",
                    "<" => $"Teo has less than {rhs} euro",
                    _ => $"Teo's money {Range(op, rhs)} euro",
                };
            case "time":
                return op switch
                {
                    ">=" or ">" => $"it's {Clock(rhs)} or later",
                    "<" or "<=" => $"it's before {Clock(rhs)}",
                    _ => $"it's {Clock(rhs)}",
                };
            case "slept_at" when op == "==" && SleptAt.TryGetValue(rhs, out var where):
                return $"Teo slept {where}";
            case "regard":
                return $"{Who(arg)}'s regard for Teo is {Range(op, rhs)} (-10 to 10)";
            case "feels":
                return $"{Who(arg)}'s own evening is {Range(op, rhs)} (-5 bad to 5 good)";
            case "tickets_left":
                return $"Teo has {Range(op, rhs)} drink tickets left";
            case "played":
                return Played(arg, op, rhs, here);
            case var v when CheckVars.Contains(v) && int.TryParse(rhs, out int r) && op == "==":
                return $"the check {CheckResult(r)}";
            case "hungover" or "hangover":
                return $"Teo's hangover is {Range(op, rhs)} (0 to 3)";
            case "n" when op == "<":
                return $"fewer than {rhs} lines so far";
        }
        if (Regex.IsMatch(rhs, "^[a-z_]+$")) rhs = $"`{rhs}`";
        string value = op switch { "==" => $"is {rhs}", "!=" => $"isn't {rhs}", _ => Range(op, rhs) };
        return $"`{name}`{(arg == "" ? "" : $" ({arg})")} {value}";
    }

    private static string Played(string knot, string op, string rhs, string? here)
    {
        bool self = knot == here;
        if (rhs == "0" && op == "==") return self ? "the first time" : $"`{knot}` hasn't happened";
        if ((rhs == "0" && op == ">") || (rhs == "1" && op == ">=")) return self ? "not the first time" : $"`{knot}` has happened";
        return self ? $"done {Range(op, rhs)} times before" : $"`{knot}` has happened {Range(op, rhs)} times";
    }

    public static string CheckResult(int r) => r switch { 0 => "goes badly", 1 => "goes half well", _ => "goes well" };

    private static string Truth(string name, string[] args) => name switch
    {
        "is" when args.Length == 1 => Flag(args[0]),
        "hungover" => "Teo is hungover",
        "chance" when args.Length == 2 => $"a {args[1]}% chance",
        "remembers" when args.Length == 2 => $"{Who(args[0])} remembers: {Words(args[1])}",
        "doing" when args.Length == 2 => $"{Who(args[0])} is at/doing: {Words(args[1])}",
        _ => $"`{name}({string.Join(", ", args)})`",
    };

    private static string Flag(string name) => $"`{name}`";

    private static string Range(string op, string rhs) => op switch
    {
        ">=" => $"{rhs} or more",
        ">" => $"more than {rhs}",
        "<=" => $"{rhs} or less",
        "<" => $"less than {rhs}",
        "==" => rhs,
        _ => $"not {rhs}",
    };

    /// <summary>"22:30", "28 * 60" (minutes since Tuesday midnight) or "magda_went_down + 60".</summary>
    private static string Clock(string rhs)
    {
        if (rhs.Contains(':')) return rhs;
        if (Regex.IsMatch(rhs, @"^[\d\s*+]+$"))
        {
            int minutes = rhs.Split('+').Sum(term => term.Split('*').Select(f => int.Parse(f.Trim())).Aggregate(1, (a, b) => a * b));
            return $"{minutes / 60 % 24:00}:{minutes % 60:00}";
        }
        var m = Regex.Match(rhs, @"^([a-z_]+)\s*\+\s*(\d+)$");
        return m.Success ? $"{m.Groups[2].Value} minutes after `{m.Groups[1].Value}`" : rhs;
    }

    public static string Who(string id) => id.Length == 0 ? id : char.ToUpperInvariant(id[0]) + id[1..];

    public static string Words(string id) => id.Replace('_', ' ');
}
