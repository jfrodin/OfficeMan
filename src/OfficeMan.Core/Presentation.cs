using System.Text.Json.Serialization;

namespace OfficeMan.Core;

public enum LineKind
{
    /// <summary>Narration, inner monologue or dialogue from ink.</summary>
    Text,
    /// <summary>A scene heading inserted by the engine (place and time).</summary>
    Header,
}

public sealed record StoryLine(string Text, IReadOnlyList<string> Tags, LineKind Kind = LineKind.Text)
{
    /// <summary>
    /// The speaker for dialogue lines written as <c>NAME: words</c>, otherwise null (narration).
    /// Only ALL-CAPS names before the colon count, so monologue containing a colon is never misread.
    /// </summary>
    [JsonIgnore]
    public string? Speaker
    {
        get
        {
            int colon = Text.IndexOf(": ", StringComparison.Ordinal);
            if (colon <= 1 || colon > 24) return null;
            string name = Text[..colon];
            return name.All(c => char.IsUpper(c) || c == ' ' || c == '\'') ? name : null;
        }
    }

    /// <summary>The words without the speaker prefix.</summary>
    [JsonIgnore]
    public string Body => Speaker is { } s ? Text[(s.Length + 2)..] : Text;
}

public sealed record ChoiceView(int Index, string Text, IReadOnlyList<string> Tags);

public enum PresentationKind
{
    /// <summary>Choices inside a scene.</summary>
    Dialogue,
    /// <summary>The menu of situations available in a hub phase.</summary>
    Hub,
    /// <summary>The prototype is over.</summary>
    End,
}

/// <summary>Everything a front end needs to show at one decision point.</summary>
public sealed class Presentation
{
    public required PresentationKind Kind { get; init; }
    public required IReadOnlyList<StoryLine> Lines { get; init; }
    public required IReadOnlyList<ChoiceView> Choices { get; init; }
    public required string Clock { get; init; }
    public string PhaseTitle { get; init; } = "";

    /// <summary>The phase this decision belongs to; front ends map it to a location backdrop.</summary>
    public string PhaseId { get; init; } = "";
    public IReadOnlyList<string> Headlines { get; init; } = Array.Empty<string>();
}
