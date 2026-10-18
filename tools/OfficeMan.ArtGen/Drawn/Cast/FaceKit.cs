using OfficeMan.ArtGen.Raster;

namespace OfficeMan.ArtGen.Drawn.Cast;

/// <summary>
/// The expression rig shared by every character. Values are per side so faces can be asymmetric:
/// L and R mean the viewer's left and right. Distances are in drawing units (a 600×760 page).
/// </summary>
public sealed record Rig
{
    public string Name { get; init; } = "neutral";

    /// <summary>Brow ends, raised (+) or lowered (−).</summary>
    public double BrowInL { get; init; }
    public double BrowInR { get; init; }
    public double BrowOutL { get; init; }
    public double BrowOutR { get; init; }

    /// <summary>Upper lid openness: 0 closed, 1 relaxed, 1.3 wide.</summary>
    public double OpenL { get; init; } = 1;
    public double OpenR { get; init; } = 1;

    /// <summary>Lower lid lift, 0..1 (squint, smile).</summary>
    public double LiftL { get; init; }
    public double LiftR { get; init; }

    public double GazeX { get; init; }
    public double GazeY { get; init; }

    /// <summary>Mouth corners, raised (+) or lowered (−).</summary>
    public double CornerL { get; init; }
    public double CornerR { get; init; }

    /// <summary>Mouth open 0..1; lips pressed 0..1; extra width; upper lip raised on the left (sneer).</summary>
    public double Open { get; init; }
    public double Press { get; init; }
    public double Stretch { get; init; }
    public double Sneer { get; init; }

    /// <summary>Cheeks pushed up (smiling), per side.</summary>
    public double CheekL { get; init; }
    public double CheekR { get; init; }
}

/// <summary>An eye's construction. InnerDir is +1 if the inner corner (towards the nose) is to the right.</summary>
public sealed record EyeSpec(Pt C, double W, double H, int InnerDir, double Tilt = 0, double Hood = 0.3,
    string Iris = "5a3a26", double LidWeight = 2.6, bool Lashes = false, string? Shadow = null, double Bags = 0.3);

/// <summary>A brow's construction. Hairs: 0 draws a solid pencilled stroke, otherwise that many hair strokes.</summary>
public sealed record BrowSpec(Pt Inner, Pt Outer, double Arch, double Thick, string Color, int Hairs = 0);

/// <summary>A mouth's construction. Asym raises the right corner at rest; Sag drops the seam's middle.</summary>
public sealed record MouthSpec(Pt C, double HalfW, double UpperT, double LowerT, string Upper, string Lower,
    double Asym = 0, double Sag = 0, double Overbite = 0);

/// <summary>Shared feature drawing. Characters supply specs; the rig supplies the emotion.</summary>
public sealed class FaceKit(Sketch s, Rgb line, Rgb skinShade, Rgb skinDeep)
{
    public Rgb Line { get; } = line;

    public void Eye(EyeSpec e, double open, double lift, double gx, double gy, IReadOnlyList<Pt> clip, bool wet = false)
    {
        var inner = new Pt(e.C.X + e.InnerDir * e.W, e.C.Y);
        var outer = new Pt(e.C.X - e.InnerDir * e.W, e.C.Y - e.Tilt);
        int d = e.InnerDir;

        if (open < 0.15)
        {
            // Closed: a single lid line, curving up when the cheek pushes it (laughing).
            double bow = 3 + lift * 6;
            var closed = PathBuilder.From(inner.X, inner.Y).C(e.C.X + d * e.W * 0.4, e.C.Y - bow, e.C.X - d * e.W * 0.4, e.C.Y - bow, outer.X, outer.Y + 1);
            s.Stroke(closed, e.LidWeight * 1.1, Line, taper: true);
            if (e.Lashes) Lashes(outer, d, e.LidWeight);
            s.Stroke(PathBuilder.From(e.C.X + d * e.W * 0.5, e.C.Y + e.H * 0.9).C(e.C.X + d * e.W * 0.2, e.C.Y + e.H * 1.2, e.C.X - d * e.W * 0.3, e.C.Y + e.H * 1.2, e.C.X - d * e.W * 0.6, e.C.Y + e.H * 0.8), 1.4, skinDeep, 0.6, taper: true);
            return;
        }

        double top = e.C.Y - e.H * open;
        var upper = PathBuilder.From(inner.X, inner.Y).C(e.C.X + d * e.W * 0.55, top, e.C.X - d * e.W * 0.45, top + e.Tilt * 0.2, outer.X, outer.Y).Points;
        double lo = e.H * (0.55 - lift * 0.45);
        var lower = PathBuilder.From(outer.X, outer.Y).C(e.C.X - d * e.W * 0.4, e.C.Y + lo, e.C.X + d * e.W * 0.45, e.C.Y + lo * 0.9, inner.X, inner.Y).Points;
        var white = upper.Concat(lower.Skip(1)).ToList();

        if (e.Shadow is not null)
        {
            var lidArea = PathBuilder.From(inner.X, inner.Y).C(e.C.X + d * e.W * 0.55, top - e.H * 1.6, e.C.X - d * e.W * 0.5, top - e.H * 1.5, outer.X - d * 4, outer.Y - 2).Points;
            s.SoftFill(lidArea, Rgb.Hex(e.Shadow), 0.75, 3, clip);
        }

        s.Fill(white, Rgb.Hex("efe6dc"));
        double ir = e.H * 0.68;
        var iris = new Pt(e.C.X + gx * e.W * 0.32, e.C.Y + gy * e.H * 0.3 + e.H * 0.05);
        s.Fill(Shapes.Ellipse(iris.X, iris.Y, ir, ir), Rgb.Hex(e.Iris), clip: white);
        s.Fill(Shapes.Ellipse(iris.X, iris.Y, ir * 0.45, ir * 0.45), Rgb.Hex("1c1214"), clip: white);
        s.SoftFill(Shapes.Ellipse(e.C.X, top, e.W * 1.1, e.H * 0.55), skinDeep, 0.55, 2, white);
        s.Fill(Shapes.Ellipse(iris.X - ir * 0.3, iris.Y - ir * 0.35, Math.Max(1.4, ir * 0.18), Math.Max(1.4, ir * 0.18)), Rgb.Hex("fbf7f2"), clip: white);
        if (wet)
        {
            s.Stroke(lower.Skip(3).Take(lower.Count - 6).ToList(), 1.6, Rgb.Hex("e4f2fa"), 0.9);
            s.Fill(Shapes.Ellipse(iris.X + ir * 0.35, iris.Y + ir * 0.3, 1.3, 1.3), Rgb.Hex("fbf7f2"), clip: white);
        }

        // The crease above: low and heavy on hooded eyes.
        double crease = top - e.H * (0.9 - e.Hood * 0.5);
        s.Stroke(PathBuilder.From(inner.X - d * 2, inner.Y - e.H * 0.5).C(e.C.X + d * e.W * 0.4, crease - 2, e.C.X - d * e.W * 0.4, crease - 1, outer.X + d * 2, outer.Y - e.H * 0.4), 1.5, skinDeep, 0.55 + e.Hood * 0.3, taper: true);
        s.Stroke(upper, e.LidWeight, Line);
        if (e.Lashes) Lashes(outer, d, e.LidWeight);
        s.Stroke(lower.Take(lower.Count * 2 / 3).ToList(), 1.1, skinDeep, 0.75, taper: true);
        if (e.Bags > 0)
            s.Stroke(PathBuilder.From(e.C.X + d * e.W * 0.55, e.C.Y + e.H * 1.15 + 2).C(e.C.X + d * e.W * 0.2, e.C.Y + e.H * 1.6 + 2, e.C.X - d * e.W * 0.3, e.C.Y + e.H * 1.6 + 2, e.C.X - d * e.W * 0.7, e.C.Y + e.H * 1.1 + 2), 1.3, skinDeep, e.Bags, taper: true);
    }

    private void Lashes(Pt outer, int d, double weight)
    {
        for (int i = 0; i < 3; i++)
        {
            var a = new Pt(outer.X + d * (2 + i * 3), outer.Y - i * 1.5);
            s.Stroke(PathBuilder.From(a.X, a.Y).C(a.X - d * 3, a.Y - 3, a.X - d * 6, a.Y - 4 - i, a.X - d * 8, a.Y - 7 - i * 1.5), weight * 0.7, Line, taper: true);
        }
    }

    public void Brow(BrowSpec b, double inRaise, double outRaise, int seed)
    {
        var inner = new Pt(b.Inner.X, b.Inner.Y - inRaise);
        var outer = new Pt(b.Outer.X, b.Outer.Y - outRaise);
        var mid = (inner + outer) * 0.5;
        var path = PathBuilder.From(inner.X, inner.Y)
            .C(inner.X + (outer.X - inner.X) * 0.35, mid.Y - b.Arch, inner.X + (outer.X - inner.X) * 0.7, mid.Y - b.Arch, outer.X, outer.Y).Points;
        var col = Rgb.Hex(b.Color);
        if (b.Hairs == 0)
        {
            s.Stroke(path, b.Thick, col, 0.9, taper: true);
            return;
        }
        var rng = new Random(seed);
        for (int i = 0; i < b.Hairs; i++)
        {
            double t = i / (double)(b.Hairs - 1);
            var p = path[(int)(t * (path.Count - 1))];
            double thick = b.Thick * (1 - t * 0.6);
            var dir = outer - inner;
            dir = dir * (1 / dir.Length);
            var a = new Pt(p.X - dir.X * 3, p.Y + thick * 0.4 + rng.NextDouble() * 2 - 1);
            var bb = new Pt(p.X + dir.X * 5, p.Y - thick * 0.5 + rng.NextDouble() * 2 - 1);
            s.Stroke(new List<Pt> { a, bb }, 1.6, col, 0.85, taper: true);
        }
    }

    public void Mouth(MouthSpec m, Rig r, IReadOnlyList<Pt> clip)
    {
        double hw = m.HalfW + r.Stretch;
        var L = new Pt(m.C.X - hw, m.C.Y - r.CornerL);
        var R = new Pt(m.C.X + hw, m.C.Y - r.CornerR - m.Asym);
        double seamY = m.C.Y + m.Sag - (r.CornerL + r.CornerR) * 0.12;
        double gap = r.Open * m.HalfW * 0.65;
        double ut = m.UpperT * (1 - r.Press * 0.7), lt = m.LowerT * (1 - r.Press * 0.6);
        double cx = m.C.X;

        var seamTop = PathBuilder.From(L.X, L.Y).C(cx - hw * 0.45, seamY - r.Sneer, cx + hw * 0.45, seamY, R.X, R.Y).Points;
        var seamBottom = PathBuilder.From(L.X, L.Y).C(cx - hw * 0.45, seamY + gap, cx + hw * 0.45, seamY + gap, R.X, R.Y).Points;
        var top = PathBuilder.From(L.X, L.Y).C(cx - hw * 0.55, seamY - ut - r.Sneer, cx - 4, seamY - ut - 1.5 - r.Sneer * 0.5, cx, seamY - ut + 1)
            .C(cx + 4, seamY - ut - 1.5, cx + hw * 0.55, seamY - ut, R.X, R.Y).Points;
        var bottom = PathBuilder.From(R.X, R.Y).C(cx + hw * 0.45, seamY + gap + lt, cx - hw * 0.45, seamY + gap + lt, L.X, L.Y).Points;

        var upperLip = top.Concat(Enumerable.Reverse(seamTop).Skip(1)).ToList();
        var lowerLip = seamBottom.Concat(bottom.Skip(1)).ToList();

        if (gap > 1)
        {
            var opening = seamTop.Concat(Enumerable.Reverse(seamBottom).Skip(1)).ToList();
            s.Fill(opening, Rgb.Hex("3c1418"));
            var teeth = PathBuilder.From(L.X + 3, L.Y + 1).C(cx - hw * 0.45, seamY + 1, cx + hw * 0.45, seamY + 1, R.X - 3, R.Y + 1)
                .L(R.X - 6, R.Y + 5 + m.Overbite).C(cx + hw * 0.4, seamY + 6 + m.Overbite, cx - hw * 0.4, seamY + 6 + m.Overbite, L.X + 6, L.Y + 5).Points;
            s.Fill(teeth, Rgb.Hex("efe5d4"), clip: opening);
            if (m.Overbite > 0)
                foreach (double dx in new[] { -4.0, 4.0 })
                    s.Fill(PathBuilder.From(cx + dx - 4, seamY).L(cx + dx + 4, seamY).L(cx + dx + 4, seamY + 8 + m.Overbite).L(cx + dx - 4, seamY + 8 + m.Overbite).Points, Rgb.Hex("f4ecdd"), clip: opening);
            s.Stroke(Enumerable.Reverse(seamBottom).ToList(), 1.2, Line, 0.6);
        }
        s.Fill(lowerLip, Rgb.Hex(m.Lower));
        s.Fill(upperLip, Rgb.Hex(m.Upper));
        s.SoftFill(Shapes.Ellipse(cx + 3, seamY + gap + lt * 0.45, hw * 0.3, lt * 0.25 + 0.5), Rgb.Hex("ffffff"), 0.18, 2);
        s.Stroke(seamTop, 2, Line, 0.9);
        // Corner dimples follow the corners.
        foreach (var (p, dir, c) in new[] { (L, -1, r.CornerL), (R, 1, r.CornerR) })
            s.Stroke(PathBuilder.From(p.X + dir, p.Y - 2).C(p.X + dir * 3, p.Y, p.X + dir * 3, p.Y + 2, p.X + dir * 1.5, p.Y + 4 - Math.Max(0, c) * 0.3), 1.3, skinDeep, 0.6, taper: true);
        s.SoftFill(Shapes.Ellipse(cx, seamY + gap + lt + 6, hw * 0.45, 3.5), skinShade, 0.6, 3, clip);
    }

    /// <summary>A crease or wrinkle: a tapered line that shows more as the face moves.</summary>
    public void Crease(IReadOnlyList<Pt> pts, double strength, double width = 1.8) =>
        s.Stroke(pts, width, skinDeep, Math.Clamp(strength, 0, 1), taper: true);
}
