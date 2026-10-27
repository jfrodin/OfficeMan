namespace OfficeMan.Script;

/// <summary>One line of ink, with its indentation (tabs count as four spaces) and where it came from.</summary>
public sealed record Line(int Indent, string Text, string File, int Number);

/// <summary>A stitch (<c>= name</c>) or the top of a knot: its lines, and the comments written just above it.</summary>
public sealed class Section
{
    public required string Id { get; init; }
    public List<string> Notes { get; } = new();
    public List<Line> Body { get; } = new();
}

/// <summary>A knot (<c>=== name</c>): its storylet tags, its own lines and its stitches.</summary>
public sealed class Knot
{
    public required string Id { get; init; }
    public required string File { get; init; }
    public bool IsFunction { get; init; }
    public Dictionary<string, string> Tags { get; } = new();
    public Section Top { get; init; } = null!;
    public List<Section> Stitches { get; } = new();

    public string? Tag(string name) => Tags.TryGetValue(name, out var v) ? v : null;
}

/// <summary>
/// Splits the ink files into knots and stitches. It doesn't understand ink; it relies on the house style
/// (one construct per line, nested blocks indented), which every file in content/ink follows.
/// </summary>
public static class InkFile
{
    public static List<Knot> Read(IEnumerable<string> paths)
    {
        var knots = new List<Knot>();
        foreach (string path in paths)
        {
            string file = Path.GetFileName(path);
            var raw = File.ReadAllLines(path);
            Knot? knot = null;
            Section? section = null;
            bool inHeader = false;
            for (int n = 0; n < raw.Length; n++)
            {
                string expanded = raw[n].Replace("\t", "    ").TrimStart('﻿');
                string text = expanded.Trim();
                int indent = expanded.Length - expanded.TrimStart().Length;

                if (text.StartsWith("==="))
                {
                    string head = text.Trim('=', ' ');
                    bool function = head.StartsWith("function ");
                    string id = (function ? head["function ".Length..] : head).Split('(')[0].Trim();
                    var top = new Section { Id = id };
                    top.Notes.AddRange(TakeTrailingNotes(section));
                    knot = new Knot { Id = id, File = file, IsFunction = function, Top = top };
                    knots.Add(knot);
                    section = top;
                    inHeader = true;
                    continue;
                }
                if (knot is null || section is null) continue;
                if (text.StartsWith("= ") || (text.StartsWith('=') && !text.StartsWith("==") && text.Length > 1 && char.IsLetter(text[1])))
                {
                    var stitch = new Section { Id = text.TrimStart('=', ' ').Trim() };
                    stitch.Notes.AddRange(TakeTrailingNotes(section));
                    knot.Stitches.Add(stitch);
                    section = stitch;
                    inHeader = false;
                    continue;
                }
                if (inHeader && text.StartsWith('#'))
                {
                    string tag = text.TrimStart('#', ' ');
                    int colon = tag.IndexOf(':');
                    if (colon < 0) knot.Tags[tag.Trim()] = "";
                    else knot.Tags[tag[..colon].Trim()] = tag[(colon + 1)..].Trim();
                    continue;
                }
                inHeader = false;
                if (text.Length == 0) continue;
                section.Body.Add(new Line(indent, text, file, n + 1));
            }
        }
        return knots;
    }

    /// <summary>Comments at the very end of a section belong to whatever comes next, as its introduction.</summary>
    private static List<string> TakeTrailingNotes(Section? previous)
    {
        var notes = new List<string>();
        if (previous is null) return notes;
        while (previous.Body.Count > 0 && previous.Body[^1].Text.StartsWith("//") && previous.Body[^1].Indent == 0)
        {
            notes.Insert(0, previous.Body[^1].Text);
            previous.Body.RemoveAt(previous.Body.Count - 1);
        }
        return notes;
    }
}
