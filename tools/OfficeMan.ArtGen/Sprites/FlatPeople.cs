using OfficeMan.ArtGen.Drawn;
using OfficeMan.ArtGen.Raster;

namespace OfficeMan.ArtGen.Sprites;

/// <summary>
/// Proposal (2026-10-27): the walking people drawn again at high resolution, as smooth flat shapes, so they sit in a
/// sharp 3D room (game/demo/StyleDemo.cs, style h) instead of being blown-up pixels. Built from the same
/// <see cref="PersonSpec"/> as <see cref="PixelPeople"/>, so everyone is redrawn at once.
/// One standing frame, facing the viewer and turned a little to the right. 160×360, feet at y 350; a person is
/// 300-325 units tall, about 175 units per metre.
/// </summary>
public static class FlatPeople
{
    public const int FrameW = 160, FrameH = 360, Foot = 350;
    public const double UnitsPerMetre = 310 / 1.75;
    private static readonly Rgb Ink = Rgb.Hex("231a1e");

    public static Canvas Frame(PersonSpec p) => new Figure(p).Draw();

    private sealed class Figure(PersonSpec p)
    {
        // The colour is painted over the outline colour, so soft edges darken into a line; the mask says what's figure.
        private readonly Sketch _paint = new(FrameW, FrameH, 3), _mask = new(FrameW, FrameH, 3);
        private static readonly Rgb White = new(255, 255, 255), Black = new(0, 0, 0);

        private const double Cx = 80;
        private readonly double _h = 300 + p.Height * 5;
        private double Top => Foot - _h;
        private double ChinY => Top + 48;
        private double ShoulderY => ChinY + 10;
        private double HipY => Foot - _h * 0.47;
        private double WaistY => HipY - 22;
        private double Sw => 24 + p.Build * 2.6 - (p.Curvy ? 5 : 0);   // half the shoulders
        private double Hw => 18 + p.Build * 1.8 + (p.Curvy ? 7 : 0);   // half the hips
        private double Waist => p.Curvy ? Hw - 9 : Hw + (p.Belly ? 5 : 0) - 2;
        private double FaceX => Cx + 4;                                 // features shifted: turned a little right

        private bool SkinArms => p.Top is TopStyle.Vest or TopStyle.Dress;

        private void Fill(IReadOnlyList<Pt> poly, string hex, IReadOnlyList<Pt>? clip = null)
        {
            _paint.Fill(poly, Rgb.Hex(hex), 1, clip);
            _mask.Fill(poly, White, 1, clip);
        }

        private void Detail(IReadOnlyList<Pt> poly, string hex, IReadOnlyList<Pt>? clip = null, double alpha = 1) =>
            _paint.Fill(poly, Rgb.Hex(hex), alpha, clip);

        private void Line(IReadOnlyList<Pt> pts, double width, string hex, double alpha = 1) =>
            _paint.Stroke(pts, width, Rgb.Hex(hex), alpha);

        private static List<Pt> Poly(params double[] xy) =>
            Enumerable.Range(0, xy.Length / 2).Select(i => new Pt(xy[i * 2], xy[i * 2 + 1])).ToList();

        private static List<Pt> Mirror(List<Pt> pts) => pts.Select(q => new Pt(2 * Cx - q.X, q.Y)).ToList();

        public Canvas Draw()
        {
            _paint.Clear((_, _) => Ink);
            _mask.Clear((_, _) => Black);

            HairBehind();
            Legs();
            Arms();
            Torso();
            Neck();
            Head();
            Face();
            Hair();
            if (p.Cigarette) Cigarette();

            var colour = _paint.ToCanvas();
            var cover = _mask.ToCanvas();
            var c = new Canvas(FrameW, FrameH);
            for (int y = 0; y < FrameH; y++)
                for (int x = 0; x < FrameW; x++)
                    if (cover[x, y].R > 90) c[x, y] = colour[x, y];
            // A thin dark line round the outside, as in the pixel people.
            var outlined = new Canvas(FrameW, FrameH);
            outlined.Outline(c, (_, _) => Ink);
            outlined.Draw(c);
            return outlined;
        }

        // ── Body ──────────────────────────────────────────

        private void Legs()
        {
            double knee = (HipY + Foot) / 2, ankle = Foot - 9;
            bool bare = p.Legs is not null;
            string near = p.Legs ?? p.Bottom, far = bare ? p.SkinShade : p.BottomShade;
            double outer = Hw - 3, inner = 3;
            var left = Poly(Cx - outer, HipY - 4, Cx - inner, HipY - 4, Cx - inner - 1, knee, Cx - 6, ankle, Cx - 17, ankle, Cx - outer + 4, knee);
            var right = Mirror(left);
            Fill(left, far);
            Fill(right, near);
            if (!bare)
            {
                Detail(Poly(Cx - outer, HipY - 4, Cx - outer + 7, HipY - 4, Cx - 14, ankle, Cx - 17, ankle, Cx - outer + 4, knee), p.BottomShade);
                Line(Poly(Cx + inner + 1, HipY + 6, Cx + inner + 2, ankle - 4), 1.4, p.BottomShade, 0.7);
            }
            // Shoes, pointing a little to the right.
            Fill(Shapes.Ellipse(Cx - 10, Foot - 5, 10, 6), p.Shoes);
            Fill(Shapes.Ellipse(Cx + 14, Foot - 5, 11, 6), p.Shoes);
            Detail(Shapes.Ellipse(Cx + 17, Foot - 8, 5, 1.6), "ffffff", alpha: 0.18);
        }

        private void Arms()
        {
            double top = ShoulderY + 6, wrist = HipY + 16;
            var arm = Poly(Cx + Sw - 9, top - 2, Cx + Sw + 3, top + 2, Cx + Sw + 7, (top + wrist) / 2, Cx + Sw + 8, wrist,
                Cx + Sw - 1, wrist, Cx + Sw - 5, (top + wrist) / 2);
            string sleeve = SkinArms ? p.Skin : p.TopColor, sleeveFar = SkinArms ? p.SkinShade : p.TopShade;
            Fill(Mirror(arm), sleeveFar);
            Fill(arm, sleeve);
            if (p.Top is TopStyle.ShirtTie or TopStyle.LowTop)
            {
                // Short sleeves: the forearm is bare.
                double elbow = (top + wrist) / 2 + 6;
                var fore = Poly(Cx + Sw - 5, elbow, Cx + Sw + 7, elbow, Cx + Sw + 8, wrist, Cx + Sw - 1, wrist);
                Fill(fore, p.Skin);
                Fill(Mirror(fore), p.SkinShade);
            }
            Fill(Shapes.Ellipse(Cx + Sw + 4, wrist + 7, 6, 8.5), p.Skin);
            Fill(Shapes.Ellipse(Cx - Sw - 4, wrist + 7, 6, 8.5), p.SkinShade);
        }

        private List<Pt> TorsoShape()
        {
            double y0 = ShoulderY, bust = ShoulderY + 30;
            var right = PathBuilder.From(Cx, y0 - 2)
                .C(Cx + Sw * 0.5, y0 - 3, Cx + Sw - 2, y0, Cx + Sw, y0 + 10)
                .C(Cx + Sw + 1, y0 + 20, Cx + Sw - (p.Curvy ? 0 : 2), bust - 6, Cx + Sw - (p.Curvy ? -1 : 3), bust)
                .C(Cx + Sw - 6, bust + 16, Cx + Waist + 1, WaistY - 8, Cx + Waist, WaistY)
                .C(Cx + Waist + (p.Belly ? 3 : 0), WaistY + 10, Cx + Hw, HipY - 8, Cx + Hw, HipY + 2)
                .L(Cx, HipY + 2).Points;
            var left = Mirror(right);
            left.Reverse();
            return right.Concat(left).ToList();
        }

        private void Torso()
        {
            var torso = TorsoShape();
            var shadeSide = Poly(0, 0, Cx - Sw * 0.72, 0, Cx - Sw * 0.66, WaistY, Cx - Hw * 0.7, FrameH, 0, FrameH);

            // Skirts and dresses hang below the hips; drawn first so the top overlaps their waistband.
            if (p.Top is TopStyle.LowTop or TopStyle.Cardigan || (p.Top == TopStyle.Dress))
            {
                bool dress = p.Top == TopStyle.Dress;
                string cloth = dress ? p.TopColor : p.Bottom, shade = dress ? p.TopShade : p.BottomShade;
                double hem = HipY + (Foot - HipY) * (dress ? (p.Curvy ? 0.42 : 0.6) : 0.36);
                double flare = dress && !p.Curvy ? 14 : 3;
                var skirt = Poly(Cx - Hw, HipY - 6, Cx + Hw, HipY - 6, Cx + Hw + flare, hem, Cx - Hw - flare, hem);
                Fill(skirt, cloth);
                Detail(shadeSide, shade, skirt);
            }

            string body = p.Top == TopStyle.Vest ? p.Skin : p.TopColor;
            Fill(torso, body);
            Detail(shadeSide, p.Top == TopStyle.Vest ? p.SkinShade : p.TopShade, torso);

            switch (p.Top)
            {
                case TopStyle.Shirt:
                    Collar(p.TopColor, p.TopShade);
                    Line(Poly(FaceX - 2, ShoulderY + 8, FaceX - 1, HipY - 2), 1.3, p.TopShade, 0.8);
                    break;
                case TopStyle.ShirtTie:
                    Collar(p.TopColor, p.TopShade);
                    Fill(Poly(FaceX - 5, ShoulderY - 1, FaceX + 3, ShoulderY - 1, FaceX + 1, ShoulderY + 6, FaceX - 3, ShoulderY + 6), p.Accent ?? "8a6a3a");
                    Fill(Poly(FaceX - 3, ShoulderY + 6, FaceX + 1, ShoulderY + 6, FaceX + 4, ShoulderY + 58, FaceX - 1, ShoulderY + 64, FaceX - 6, ShoulderY + 58), p.Accent ?? "8a6a3a");
                    break;
                case TopStyle.SuitOpen:
                case TopStyle.Suit:
                {
                    bool open = p.Top == TopStyle.SuitOpen;
                    string shirt = p.Accent ?? "ece9e4";
                    double v = open ? 52 : 34;
                    var front = Poly(FaceX - 13, ShoulderY - 1, FaceX + 9, ShoulderY - 1, FaceX - 1, ShoulderY + v);
                    Fill(front, shirt);
                    if (open) Detail(Poly(FaceX - 4, ShoulderY - 1, FaceX + 2, ShoulderY - 1, FaceX - 1, ShoulderY + 7), p.SkinShade);
                    // Lapels.
                    Line(Poly(FaceX - 13, ShoulderY, FaceX - 4, ShoulderY + 20, FaceX - 1, ShoulderY + v), 2.2, p.TopShade);
                    Line(Poly(FaceX + 9, ShoulderY, FaceX + 4, ShoulderY + 20, FaceX - 1, ShoulderY + v), 2.2, p.TopShade);
                    foreach (double b in new[] { v + 10, v + 30 }) Fill(Shapes.Ellipse(FaceX - 1, ShoulderY + b, 1.8, 1.8), p.TopShade);
                    if (!open) Fill(Shapes.Ellipse(FaceX - 10, ShoulderY + 24, 3, 3), "4a7a5a");   // a brooch
                    break;
                }
                case TopStyle.Vest:
                {
                    // The white vest over a bare chest and shoulders.
                    var cut = Poly(Cx - Sw + 14, ShoulderY - 4, Cx - Sw + 22, ShoulderY - 4, FaceX - 8, ShoulderY + 18, FaceX + 6, ShoulderY + 18,
                        Cx + Sw - 22, ShoulderY - 4, Cx + Sw - 14, ShoulderY - 4, Cx + Sw - 6, ShoulderY + 30, Cx + Hw + 4, HipY + 4, Cx - Hw - 4, HipY + 4, Cx - Sw + 6, ShoulderY + 30);
                    Fill(cut, p.TopColor, torso);
                    Detail(shadeSide, p.TopShade, cut);
                    break;
                }
                case TopStyle.LowTop:
                case TopStyle.Dress when p.LowNeck:
                {
                    var neckline = PathBuilder.From(Cx - Sw + 10, ShoulderY - 2).C(Cx - 10, ShoulderY + 4, FaceX - 6, ShoulderY + 22, FaceX, ShoulderY + 26)
                        .C(FaceX + 6, ShoulderY + 22, Cx + 10, ShoulderY + 4, Cx + Sw - 10, ShoulderY - 2).Points;
                    Fill(neckline, p.Skin, torso);
                    Line(Poly(FaceX, ShoulderY + 16, FaceX, ShoulderY + 25), 1.4, p.SkinShade, 0.7);
                    if (p.Curvy) Line(PathBuilder.From(Cx - Sw + 6, ShoulderY + 36).C(Cx - 10, ShoulderY + 44, Cx + 10, ShoulderY + 44, Cx + Sw - 6, ShoulderY + 36).Points, 1.6, p.TopShade, 0.8);
                    break;
                }
                case TopStyle.Dress:
                    Fill(PathBuilder.From(Cx - 14, ShoulderY - 2).C(Cx - 8, ShoulderY + 8, FaceX + 4, ShoulderY + 8, Cx + 14, ShoulderY - 2).Points, p.Skin, torso);
                    break;
            }
            if (p.Top is TopStyle.Shirt or TopStyle.ShirtTie or TopStyle.SuitOpen)
                Fill(Poly(Cx - Hw, HipY - 4, Cx + Hw, HipY - 4, Cx + Hw, HipY + 1, Cx - Hw, HipY + 1), p.BottomShade, torso);   // belt
            if (p.Sash)
                Fill(Poly(Cx + Sw - 8, ShoulderY, Cx + Sw, ShoulderY + 6, Cx - Hw + 6, HipY, Cx - Hw - 2, HipY - 8), p.Accent ?? "f8f0f8", torso);
        }

        private void Collar(string cloth, string shade)
        {
            Fill(Poly(FaceX - 14, ShoulderY - 4, FaceX - 3, ShoulderY - 1, FaceX - 4, ShoulderY + 9), cloth);
            Fill(Poly(FaceX + 10, ShoulderY - 4, FaceX - 1, ShoulderY - 1, FaceX, ShoulderY + 9), cloth);
            Line(Poly(FaceX - 14, ShoulderY - 4, FaceX - 4, ShoulderY + 9, FaceX - 2, ShoulderY + 2, FaceX, ShoulderY + 9, FaceX + 10, ShoulderY - 4), 1.3, shade);
        }

        private void Neck()
        {
            double w = 7 + p.Build * 0.6;
            Fill(Poly(Cx - w + 2, ChinY - 10, Cx + w + 2, ChinY - 10, Cx + w + 3, ShoulderY + 1, Cx - w + 1, ShoulderY + 1), p.Skin);
            Detail(Poly(Cx - w, ChinY - 10, Cx + w + 4, ChinY - 10, Cx + w + 4, ChinY + 1, Cx - w, ChinY + 4), p.SkinShade);
        }

        // ── Head ──────────────────────────────────────────

        private double Hx => Cx + 2;
        private double Hy => Top + 25;
        private double Rx => 18 + (p.Build - 6) * 0.8;
        private const double Ry = 24;

        private List<Pt> HeadShape()
        {
            // Wider at the temples than the jaw; a heavier jaw on bigger people.
            double jaw = Rx * (p.Build >= 8 ? 0.9 : 0.78);
            return PathBuilder.From(Hx, Hy - Ry)
                .C(Hx + Rx * 0.75, Hy - Ry, Hx + Rx, Hy - Ry * 0.5, Hx + Rx, Hy)
                .C(Hx + Rx, Hy + Ry * 0.5, Hx + jaw, Hy + Ry * 0.85, Hx + 2, Hy + Ry)
                .C(Hx - jaw + 4, Hy + Ry * 0.85, Hx - Rx, Hy + Ry * 0.5, Hx - Rx, Hy)
                .C(Hx - Rx, Hy - Ry * 0.5, Hx - Rx * 0.75, Hy - Ry, Hx, Hy - Ry).Points;
        }

        private void Head()
        {
            Fill(Shapes.Ellipse(Hx - Rx + 1, Hy + 3, 4, 6), p.SkinShade);
            Fill(Shapes.Ellipse(Hx + Rx - 1, Hy + 3, 4, 6), p.Skin);
            var head = HeadShape();
            Fill(head, p.Skin);
            // The far side of the face in shade, a crescent.
            Detail(Shapes.Ellipse(Hx - Rx * 1.55, Hy + 2, Rx * 0.95, Ry * 1.3), p.SkinShade, head);
            if (p.Earrings)
                foreach (double s in new[] { -1.0, 1.0 })
                    Fill(Shapes.Ellipse(Hx + s * (Rx - 1), Hy + 11, 2.2, 2.2), "e0b850");
        }

        private void Face()
        {
            double ey = Hy + 1, gap = 8.5;
            foreach (double s in new[] { -1.0, 1.0 })
            {
                double ex = FaceX + s * gap;
                Detail(Shapes.Ellipse(ex, ey, 2.4, 3), "2a1c1c");
                Line(Poly(ex - 4.5, ey - 7 - (s < 0 ? 0 : 0.6), ex + 4.5, ey - 7.5), 2.2, p.HairShade);
            }
            if (p.Glasses)
            {
                foreach (double s in new[] { -1.0, 1.0 })
                    _paint.Stroke(Shapes.Ellipse(FaceX + s * gap, ey, 6, 5).Append(Shapes.Ellipse(FaceX + s * gap, ey, 6, 5)[0]).ToList(), 1.6, Rgb.Hex(p.GlassesColor));
                Line(Poly(FaceX - gap + 6, ey - 1, FaceX + gap - 6, ey - 1), 1.4, p.GlassesColor);
            }
            // Nose and mouth.
            Line(Poly(FaceX + 1, ey + 3, FaceX + 3, ey + 10, FaceX, ey + 11), 1.6, p.SkinShade);
            if (p.Moustache is not null)
                Fill(PathBuilder.From(FaceX - 8, ey + 17).C(FaceX - 6, ey + 11, FaceX + 6, ey + 11, FaceX + 8, ey + 17).L(FaceX, ey + 15).Points, p.Moustache);
            Line(PathBuilder.From(FaceX - 5, ey + 17).C(FaceX - 2, ey + 18.5, FaceX + 2, ey + 18.5, FaceX + 5, ey + 16.5).Points, 1.7, "8a4a44");
            if (p.Curvy) Line(PathBuilder.From(FaceX - 4, ey + 17).C(FaceX - 1, ey + 19.5, FaceX + 2, ey + 19.5, FaceX + 4, ey + 17).Points, 2.4, "b8405a", 0.9);
        }

        // ── Hair ──────────────────────────────────────────

        private List<Pt> Cap(double forehead, double puff, double sides)
        {
            // Over the crown from ear to ear, down to the forehead line.
            return PathBuilder.From(Hx - Rx - 2, Hy + sides)
                .C(Hx - Rx - 3 - puff * 0.3, Hy - Ry * 0.6, Hx - Rx * 0.7, Hy - Ry - puff, Hx, Hy - Ry - puff)
                .C(Hx + Rx * 0.7, Hy - Ry - puff, Hx + Rx + 3 + puff * 0.3, Hy - Ry * 0.6, Hx + Rx + 2, Hy + sides)
                .L(Hx + Rx - 2, Hy - 4)
                .C(Hx + Rx * 0.5, Top + forehead - 2, Hx - Rx * 0.3, Top + forehead, Hx - Rx + 2, Hy - 4).Points;
        }

        private void HairBehind()
        {
            switch (p.Hair)
            {
                case HairStyle.Perm:
                {
                    var mass = Shapes.Scallop(Shapes.Ellipse(Hx, Hy - 2, Rx + 13, Ry + 10, 96), new Pt(Hx, Hy), 4, 11, seed: 3);
                    Fill(mass, p.HairColor);
                    Detail(Shapes.Ellipse(Hx - Rx - 6, Hy + 4, Rx * 0.8, Ry + 6), p.HairShade, mass);
                    break;
                }
                case HairStyle.Bun:
                    Fill(Shapes.Ellipse(Hx - 3, Top - 4, 11, 10), p.HairColor);
                    Detail(Shapes.Ellipse(Hx - 7, Top - 2, 6, 7), p.HairShade);
                    break;
            }
        }

        private void Hair()
        {
            switch (p.Hair)
            {
                case HairStyle.Messy:
                {
                    var cap = Shapes.Scallop(Cap(forehead: 14, puff: 5, sides: -2), new Pt(Hx, Hy + 4), 3.5, 9, seed: 5);
                    Fill(cap, p.HairColor);
                    Detail(Shapes.Ellipse(Hx - Rx - 4, Hy - 4, Rx * 0.7, Ry), p.HairShade, cap);
                    break;
                }
                case HairStyle.Perm:
                {
                    var fringe = Shapes.Scallop(Cap(forehead: 15, puff: 9, sides: 6), new Pt(Hx, Hy + 6), 4, 9, seed: 7);
                    Fill(fringe, p.HairColor);
                    Detail(Shapes.Ellipse(Hx - Rx - 4, Hy - 2, Rx * 0.7, Ry + 4), p.HairShade, fringe);
                    break;
                }
                case HairStyle.SweptBack:
                {
                    var cap = Cap(forehead: 9, puff: 4, sides: -4);
                    Fill(cap, p.HairColor);
                    Detail(Shapes.Ellipse(Hx - Rx - 4, Hy - 4, Rx * 0.7, Ry), p.HairShade, cap);
                    for (int i = 0; i < 4; i++)
                        Line(Poly(Hx - 10 + i * 6, Top + 7, Hx - 14 + i * 5, Top - 1), 1.2, p.HairShade, 0.7);
                    if (p.GreyAt is not null)
                        foreach (double s in new[] { -1.0, 1.0 })
                            Detail(Shapes.Ellipse(Hx + s * (Rx - 1), Hy - 6, 4, 5), p.GreyAt, cap);
                    break;
                }
                case HairStyle.Bun:
                {
                    var cap = Cap(forehead: 11, puff: 1, sides: -2);
                    Fill(cap, p.HairColor);
                    Detail(Shapes.Ellipse(Hx - Rx - 4, Hy - 4, Rx * 0.7, Ry), p.HairShade, cap);
                    Line(Poly(Hx - 6, Top + 3, Hx - 3, Top - 2), 1.2, p.HairShade, 0.8);
                    break;
                }
                case HairStyle.Combover:
                {
                    var cap = Cap(forehead: 7, puff: 1, sides: -5);
                    Fill(cap, p.HairColor);
                    Detail(Shapes.Ellipse(Hx - Rx - 4, Hy - 4, Rx * 0.7, Ry), p.HairShade, cap);
                    for (int i = 0; i < 5; i++)
                        Line(Poly(Hx - 12, Top + 4 + i * 2.2, Hx + 14, Top + 2 + i * 2.6), 1, p.HairShade, 0.6);
                    break;
                }
                case HairStyle.Bald:
                    foreach (double s in new[] { -1.0, 1.0 })
                    {
                        var fringe = Poly(Hx + s * (Rx + 2), Hy - 8, Hx + s * (Rx - 6), Hy - 6, Hx + s * (Rx - 4), Hy + 6, Hx + s * (Rx + 2), Hy + 8);
                        Fill(fringe, s < 0 ? p.HairShade : p.HairColor);
                    }
                    Detail(Shapes.Ellipse(Hx + 6, Top + 8, 6, 3), "ffffff", alpha: 0.22);
                    break;
            }
        }

        private void Cigarette()
        {
            double hx = Cx + Sw + 4, hy = HipY + 23;
            Fill(Poly(hx + 1, hy - 1.3, hx + 14, hy - 5.3, hx + 14.6, hy - 2.9, hx + 1.6, hy + 1.3), "f0ece4");
            Fill(Shapes.Ellipse(hx + 14.5, hy - 4.2, 1.6, 1.6), "e85a2a");
        }
    }
}
