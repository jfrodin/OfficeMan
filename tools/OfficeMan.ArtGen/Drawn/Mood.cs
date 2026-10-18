namespace OfficeMan.ArtGen.Drawn;

/// <summary>
/// The emotions used to compare character construction methods. Each method decides for itself
/// how a mood changes the face; a cartoon pushes it, realism holds it back.
/// </summary>
public enum Mood
{
    Happy,
    Irritated,
    Sad,
}

public static class PathTools
{
    /// <summary>
    /// A silhouette of rounded lobes through hand-placed anchor points: each span between two
    /// anchors bulges away from <paramref name="centre"/>. Used for curly hair and soft shapes.
    /// </summary>
    public static List<Pt> Lobes(IReadOnlyList<Pt> anchors, Pt centre, double bulge, bool close = true)
    {
        var path = PathBuilder.From(anchors[0].X, anchors[0].Y);
        int n = close ? anchors.Count : anchors.Count - 1;
        for (int i = 0; i < n; i++)
        {
            Pt a = anchors[i], b = anchors[(i + 1) % anchors.Count];
            var mid = (a + b) * 0.5;
            var dir = mid - centre;
            dir = dir * (1 / Math.Max(1e-6, dir.Length));
            double span = (b - a).Length;
            var c1 = a + (b - a) * 0.1 + dir * (span * bulge);
            var c2 = a + (b - a) * 0.9 + dir * (span * bulge);
            path.C(c1.X, c1.Y, c2.X, c2.Y, b.X, b.Y, 16);
        }
        return path;
    }

    public static List<Pt> Shift(IReadOnlyList<Pt> pts, double dx, double dy) =>
        pts.Select(p => new Pt(p.X + dx, p.Y + dy)).ToList();

    public static List<Pt> ScaleAbout(IReadOnlyList<Pt> pts, Pt c, double k) =>
        pts.Select(p => c + (p - c) * k).ToList();

    public static List<Pt> Close(IReadOnlyList<Pt> pts) => pts.Append(pts[0]).ToList();

    public static List<Pt> Pts(params double[] xy) =>
        Enumerable.Range(0, xy.Length / 2).Select(i => new Pt(xy[i * 2], xy[i * 2 + 1])).ToList();
}
