namespace OfficeMan.ArtGen.Portraits;

/// <summary>
/// The parameters a face is drawn from. Every expression in the game is a point in this space,
/// so new emotions are data, not new drawings.
/// </summary>
public sealed record Expression
{
    public string Name { get; init; } = "neutral";

    /// <summary>-1 frown … 0 flat … 1 broad smile.</summary>
    public double Smile { get; init; }

    /// <summary>Extra lift on the viewer's right mouth corner (lopsided smile).</summary>
    public double Lopsided { get; init; }

    /// <summary>0 closed … 1 open wide.</summary>
    public double MouthOpen { get; init; }

    /// <summary>0 eyes shut … 1 normally open.</summary>
    public double LidOpen { get; init; } = 1;

    /// <summary>0 … 1: lower lids push up (laughing, squinting).</summary>
    public double Squint { get; init; }

    /// <summary>Whole-brow lift in pixels (surprise, hope).</summary>
    public double BrowRaise { get; init; }

    /// <summary>Inner-brow lift: positive = sad/worried, negative = angry.</summary>
    public double BrowInner { get; init; }

    public double LookX { get; init; }
    public double LookY { get; init; }

    /// <summary>0 … 1 flush in the cheeks.</summary>
    public double Blush { get; init; }

    /// <summary>Mascara running.</summary>
    public bool Tears { get; init; }
}
