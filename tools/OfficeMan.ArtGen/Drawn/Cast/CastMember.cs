using OfficeMan.ArtGen.Raster;

namespace OfficeMan.ArtGen.Drawn.Cast;

/// <summary>
/// A character in the mature animation style. Each one is constructed from scratch: its own skull,
/// jaw, nose, ears, hair, body and marks. Only the expression rig and feature kit are shared.
/// </summary>
public abstract class CastMember
{
    public const int W = 600, H = 760;

    protected static readonly Rgb Ink = Rgb.Hex("33252a");

    public abstract string Name { get; }
    public abstract Rig Neutral { get; }
    public abstract Rig Distinct { get; }

    public Canvas Draw(Rig r)
    {
        var s = new Sketch(W, H);
        Paint(s, r);
        return s.ToCanvas(grain: 0.015);
    }

    protected abstract void Paint(Sketch s, Rig r);

    protected static void Backdrop(Sketch s, string light, string dark) =>
        s.Clear((x, y) =>
        {
            double d = Math.Sqrt(Math.Pow((x - 290) / 330.0, 2) + Math.Pow((y - 330) / 360.0, 2));
            return Rgb.Hex(light).Mix(Rgb.Hex(dark), Math.Clamp(d - 0.15, 0, 1));
        });

    protected static List<Pt> P(params double[] xy) => PathTools.Pts(xy);
}
