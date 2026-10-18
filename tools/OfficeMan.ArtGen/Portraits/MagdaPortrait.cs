using OfficeMan.ArtGen.Raster;

namespace OfficeMan.ArtGen.Portraits;

/// <summary>
/// Magda Pálffy, 52, the Secretary. Bust portrait in the red dress from the summer party.
/// Volumes are smooth height fields lit and quantised onto hand-picked ramps;
/// facial features are drawn from an <see cref="Expression"/>.
/// </summary>
public static class MagdaPortrait
{
    public const int W = 112, H = 136;

    // ── Palette ────────────────────────────────────────────────
    private static readonly Ramp Background = Ramp.Of("1a2429", "1f2c32", "26363d", "2e4048");
    private static readonly Ramp Skin = Ramp.Of("5a2a26", "8a4436", "b2604c", "cf8064", "e5a184", "f3c2a0", "fbdcc0");
    private static readonly Ramp Hair = Ramp.Of("1c0a12", "35101e", "531a2c", "742638", "963646", "b65254", "d27c6c");
    private static readonly Ramp Dress = Ramp.Of("360810", "5c0d1a", "861424", "b01e2c", "d43838", "ec6656");
    private static readonly Ramp Gold = Ramp.Of("5a3a12", "8c6420", "c49a3a", "ecd27a");
    private static readonly Ramp Lips = Ramp.Of("4e1216", "8e2228", "b8363a", "d85a54", "ec8a7a");
    private static readonly Ramp Shadow = Ramp.Of("34447c", "5068a4", "7894c6");
    private static readonly Ramp Iris = Ramp.Of("2a1a10", "5a3a20", "8a6034");
    private static readonly Rgb Ink = Rgb.Hex("170a0e");
    private static readonly Rgb Lash = Rgb.Hex("1e1016");
    private static readonly Rgb Brow = Rgb.Hex("4a2024");
    private static readonly Rgb EyeWhite = Rgb.Hex("ebe0d2");
    private static readonly Rgb EyeWhiteShade = Rgb.Hex("b9a79a");
    private static readonly Rgb Teeth = Rgb.Hex("efe5d3");
    private static readonly Rgb MouthDark = Rgb.Hex("3a0e16");
    private static readonly Rgb Mascara = Rgb.Hex("383048");
    private static readonly Rgb Tear = Rgb.Hex("d4e4ee");
    private static readonly Rgb BlushPink = Rgb.Hex("e4807c");

    private static readonly Light Key = new(-0.5, -0.55, 0.8, ambient: 0.22, rim: 0.25);

    // ── Proportions ────────────────────────────────────────────
    private const double Cx = 56;
    private const double HeadY = 60, HeadRx = 25, HeadRy = 28;
    private const double EyeY = 63, EyeDx = 11, EyeHalfW = 5.6;
    private const double MouthY = 84, MouthHalfW = 7.5;
    private const double FringeBottom = 46;

    private static readonly Cells Curls = new(seed: 52, cellSize: 4.2);

    public static Canvas Draw(Expression e)
    {
        var canvas = new Canvas(W, H);
        DrawBackground(canvas);

        var hairBackField = new HeightField(W, H, HairBackBlobs(), smoothness: 0.25);
        var hairBack = new Canvas(W, H);
        Volume.Render(hairBack, hairBackField, Hair, Key, HairTexture, clip: (x, y) => CurlyEdge(hairBackField, x, y));

        var headField = new HeightField(W, H, HeadBlobs(e), smoothness: 0.3);
        var body = new Canvas(W, H);
        DrawBody(body, headField);

        var head = new Canvas(W, H);
        Volume.Render(head, headField, Skin, Key, FaceShading, dither: 0.35);

        var hairFrontField = new HeightField(W, H, HairFrontBlobs(), smoothness: 0.25);
        var hairFront = new Canvas(W, H);
        Volume.Render(hairFront, hairFrontField, Hair, Key, HairTexture, clip: (x, y) => CurlyEdge(hairFrontField, x, y));

        var silhouette = new Canvas(W, H);
        silhouette.Draw(hairBack);
        silhouette.Draw(body);
        silhouette.Draw(head);
        silhouette.Draw(hairFront);

        canvas.Draw(hairBack);
        canvas.Draw(body);
        canvas.Draw(head);
        DrawFace(canvas, head, e);
        CastHairShadow(canvas, head, hairFront);
        canvas.Draw(hairFront);
        DrawEarrings(canvas);
        canvas.Outline(silhouette, (_, _) => Ink);
        return canvas;
    }

    // ── Background ─────────────────────────────────────────────

    private static void DrawBackground(Canvas c)
    {
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
            {
                double dx = (x - 38) / 95.0, dy = (y - 36) / 115.0;
                double t = 0.95 - Math.Sqrt(dx * dx + dy * dy) * 1.5;
                c[x, y] = Background.At(t, x, y, 0.5);
            }
    }

    // ── Hair ───────────────────────────────────────────────────

    private static List<Blob> HairBackBlobs() => new()
    {
        new(Cx, 50, 35, 33, Z: -26, Depth: 0.9),
        new(Cx - 4, 34, 28, 18, Z: -18),
        new(Cx - 28, 74, 13, 21, Z: -28),
        new(Cx + 28, 74, 13, 21, Z: -28),
        new(Cx - 30, 92, 9, 9, Z: -32),
        new(Cx + 30, 91, 9, 9, Z: -32),
    };

    private static List<Blob> HairFrontBlobs() => new()
    {
        new(Cx - 3, 37, 26, 11, Z: 18, Depth: 0.8),   // fringe volume
        new(Cx - 14, 41, 13, 7.5, Z: 20),            // swept to her right
        new(Cx + 10, 40, 12, 7, Z: 19),
        new(Cx - 25, 52, 6.5, 13, Z: 8),             // locks framing the face
        new(Cx + 25, 52, 6.5, 13, Z: 8),
        new(Cx - 26.5, 66, 5.5, 8, Z: 4),
        new(Cx + 26.5, 66, 5.5, 8, Z: 4),
    };

    /// <summary>Tight permed curls: darken the creases between clumps, catch light on their tops.</summary>
    private static double HairTexture(int x, int y, double t)
    {
        var (f1, f2) = Curls.At(x, y * 1.1);
        double crease = Math.Clamp((f2 - f1) * 2.2, 0, 1);   // 0 at cell borders
        double dome = 1 - Math.Clamp(f1 * 1.4, 0, 1);         // 1 at clump centres
        return t * (0.72 + 0.28 * crease) + dome * 0.12 - 0.05;
    }

    /// <summary>Breaks up the hair silhouette into curl bumps.</summary>
    private static bool CurlyEdge(HeightField f, int x, int y)
    {
        int outside = 0;
        for (int j = -2; j <= 2; j++)
            for (int i = -2; i <= 2; i++)
                if (!f.Contains(x + i, y + j)) outside++;
        if (outside == 0) return true;
        var (f1, _) = Curls.At(x + 0.5, y * 1.1 + 0.5);
        return f1 < 0.62 - outside * 0.025;
    }

    private static void CastHairShadow(Canvas c, Canvas head, Canvas hairFront)
    {
        // Hair throws a soft shadow down-right onto the face.
        for (int y = 0; y < H; y++)
            for (int x = 0; x < W; x++)
            {
                if (!head.IsSet(x, y) || hairFront.IsSet(x, y)) continue;
                if (hairFront.IsSet(x - 1, y - 2) || hairFront.IsSet(x, y - 2) || hairFront.IsSet(x - 2, y - 1))
                    c[x, y] = Darker(c[x, y]);
            }
    }

    private static Rgb Darker(Rgb skin)
    {
        for (int i = 1; i < Skin.Colors.Count; i++)
            if (Skin.Colors[i] == skin) return Skin.Colors[i - 1];
        return skin.Mix(Skin.Step(1), 0.5);
    }

    // ── Body ───────────────────────────────────────────────────

    private static void DrawBody(Canvas body, HeightField headField)
    {
        // Shadow the head casts onto neck and chest.
        double JawShadow(int x, int y, double t)
        {
            // The neck sits in the head's shadow; darkest right under the jaw.
            if (y < 108) t -= 0.12;
            for (int k = 1; k <= 4; k++)
                if (headField.Contains(x + 1, y - k)) return t - 0.25 * (1 - (k - 1) / 4.0);
            return t;
        }

        var skin = new List<Blob>
        {
            new(Cx, 100, 16, 12, Z: -6),
            new(Cx, 134, 52, 30, Z: -12, Depth: 0.45),
        };
        Volume.Render(body, skin, Skin, Key, JawShadow, dither: 0.35, smoothness: 0.2);

        var dress = new List<Blob>
        {
            new(Cx, 152, 64, 44, Z: -8, Depth: 0.4),
            new(Cx - 13, 127, 15, 10, Z: -3, Depth: 0.35),
            new(Cx + 13, 127, 15, 10, Z: -3, Depth: 0.35),
        };
        var dressLayer = new Canvas(W, H);
        Volume.Render(dressLayer, dress, Dress, Key, (x, y, t) =>
        {
            // A couple of soft folds from the bust down.
            double fold = Math.Sin((x - Cx) * 0.28 + y * 0.05);
            return t + fold * 0.05 - (y > 128 && Math.Abs(x - Cx) < 3 ? 0.18 : 0);
        }, dither: 0.25, clip: (x, y) => y >= Neckline(x), smoothness: 0.12);
        body.Draw(dressLayer);

        // Neckline edge and the shadow line at the centre of the bust.
        for (int x = 0; x < W; x++)
        {
            int y = (int)Math.Floor(Neckline(x)) - 1;
            if (body.IsSet(x, y) && !dressLayer.IsSet(x, y)) body[x, y] = Skin.Step(2);
        }
        for (int y = 119; y < 125; y++) body[(int)Cx, y] = Skin.Step(2);

        // Necklace: a thin gold chain with a small pendant.
        for (int x = (int)Cx - 15; x <= Cx + 15; x++)
        {
            double y = 103 + 0.03 * Math.Pow(x - Cx, 2);
            body[x, (int)Math.Round(y)] = Gold.Step((x + 1) % 4 == 0 ? 3 : x < Cx ? 2 : 1);
        }
        body[(int)Cx, 104] = Gold.Step(2);
        body[(int)Cx - 1, 105] = Gold.Step(3);
        body[(int)Cx, 105] = Gold.Step(2);
        body[(int)Cx + 1, 105] = Gold.Step(1);
        body[(int)Cx, 106] = Gold.Step(1);
    }

    private static double Neckline(double x)
    {
        double d = (x - Cx) / 27;
        return Math.Abs(d) >= 1 ? 107 : 107 + 12 * (1 - d * d);
    }

    // ── Head ───────────────────────────────────────────────────

    private static List<Blob> HeadBlobs(Expression e)
    {
        double cheekLift = Math.Max(0, e.Smile) * 2 + e.Squint;
        return new List<Blob>
        {
            new(Cx, HeadY, HeadRx, HeadRy, Z: 0),
            new(Cx - 12.5, 75 - cheekLift, 11, 9.5, Z: 6),
            new(Cx + 12.5, 75 - cheekLift, 11, 9.5, Z: 6),
            new(Cx, 80, 22.5, 15, Z: 2),                   // full lower face
            new(Cx, 89, 9, 5, Z: 8),                       // chin
            new(Cx, 93, 15, 6, Z: -3, Depth: 0.6),         // double chin
            new(Cx + 0.5, 70, 3.4, 8, Z: 19, Depth: 0.6),  // bridge
            new(Cx + 0.5, 76, 3.9, 3.2, Z: 21),            // tip
            new(Cx - 2.8, 77, 2.4, 2.0, Z: 18),            // wings
            new(Cx + 3.8, 77, 2.4, 2.0, Z: 18),
        };
    }

    private static double FaceShading(int x, int y, double t)
    {
        // Eye sockets sit back.
        foreach (double ex in new[] { Cx - EyeDx, Cx + EyeDx })
        {
            double dx = (x - ex) / 8, dy = (y - EyeY + 1) / 5;
            double d = dx * dx + dy * dy;
            if (d < 1) t -= 0.14 * (1 - d);
        }
        return t;
    }

    // ── Face ───────────────────────────────────────────────────

    private static void DrawFace(Canvas c, Canvas head, Expression e)
    {
        DrawBlush(c, head, e);
        foreach (int side in new[] { -1, 1 })
        {
            double ex = Cx + side * EyeDx;
            DrawEyeshadow(c, ex, e);
            DrawEye(c, ex, side, e);
            DrawBrow(c, ex, side, e);
            if (e.Tears) DrawMascaraRun(c, ex, side, e);
        }
        DrawNoseDetail(c);
        DrawMouth(c, e);
        if (e.Smile > 0.45) DrawSmileLines(c, e);
    }

    private static double Upper(double dx, Expression e) =>
        EyeY - (2.9 * e.LidOpen + 0.5) * (1 - Math.Pow(dx / EyeHalfW, 2)) + 0.3;

    private static double Lower(double dx, Expression e) =>
        EyeY + (1.9 - 1.4 * e.Squint) * (1 - Math.Pow(dx / EyeHalfW, 2)) + 0.6;

    private static void DrawEye(Canvas c, double ex, int side, Expression e)
    {
        int x0 = (int)Math.Floor(ex - EyeHalfW), x1 = (int)Math.Ceiling(ex + EyeHalfW);
        double ix = ex + e.LookX * 1.8, iy = EyeY + e.LookY * 1.1 + 0.4;

        for (int x = x0; x <= x1; x++)
        {
            double dx = x + 0.5 - ex;
            if (Math.Abs(dx) > EyeHalfW) continue;
            double top = Upper(dx, e), bottom = Lower(dx, e);
            for (int y = (int)Math.Floor(top); y <= (int)Math.Ceiling(bottom); y++)
            {
                double py = y + 0.5;
                if (py < top || py > bottom) continue;
                double di = Math.Sqrt(Math.Pow(x + 0.5 - ix, 2) + Math.Pow(py - iy, 2));
                Rgb col;
                if (di < 2.6)
                {
                    col = di < 1.1 ? Ink : Iris.At(0.95 - (py - top < 1.2 ? 0.6 : 0) - di * 0.18, x, y);
                    if (Math.Abs(x + 0.5 - (ix - 0.9)) < 0.6 && Math.Abs(py - (iy - 1.0)) < 0.6) col = Rgb.Hex("f6f2ec");
                }
                else
                {
                    col = py - top < 1.0 ? EyeWhiteShade : EyeWhite;
                }
                c[x, y] = col;
            }
        }

        // Upper lid, heavy with mascara, thicker towards the outer corner, with a flick.
        for (int x = x0; x <= x1; x++)
        {
            double dx = x + 0.5 - ex;
            if (Math.Abs(dx) > EyeHalfW + 0.5) continue;
            int y = (int)Math.Floor(Upper(Math.Clamp(dx, -EyeHalfW, EyeHalfW), e));
            c[x, y] = Lash;
            if (dx * side > 0 || e.LidOpen < 0.5) c[x, y - 1] = Lash;
        }
        int fx = (int)Math.Round(ex + side * (EyeHalfW + 1));
        c[fx, (int)Math.Floor(EyeY - 0.3)] = Lash;
        c[fx + side, (int)Math.Floor(EyeY - 1.3)] = Lash;

        // Lower lid and the tired bag beneath it.
        for (int x = x0 + 1; x <= x1 - 1; x++)
        {
            double dx = x + 0.5 - ex;
            int y = (int)Math.Ceiling(Lower(dx, e));
            c[x, y] = Skin.Step(2);
            if (Math.Abs(dx) < EyeHalfW - 1.5 && e.Squint < 0.6) c[x, y + 2] = Skin.Step(3);
        }
    }

    private static void DrawEyeshadow(Canvas c, double ex, Expression e)
    {
        for (int y = (int)(EyeY - 8); y < EyeY; y++)
            for (int x = (int)(ex - EyeHalfW - 1); x <= ex + EyeHalfW + 1; x++)
            {
                double dx = x + 0.5 - ex;
                double lid = Upper(Math.Clamp(dx, -EyeHalfW, EyeHalfW), e);
                double above = lid - (y + 0.5);
                if (above < 0.5 || above > 4.5) continue;
                double strength = (1 - above / 4.5) * (1 - Math.Abs(dx) / (EyeHalfW + 2));
                if (strength > Bayer.At(x, y) * 0.8)
                    c[x, y] = Shadow.At(0.15 + strength * 0.6, x, y);
            }
    }

    private static void DrawBrow(Canvas c, double ex, int side, Expression e)
    {
        for (int i = 0; i <= 14; i++)
        {
            double u = i / 14.0; // 0 inner end, 1 outer end
            double x = ex - side * 5.5 + side * u * 12.5;
            double arch = -2.4 * Math.Sin(u * Math.PI * 0.85);
            double inner = e.BrowInner * 2.6 * Math.Pow(1 - u, 1.5);
            double y = EyeY - 8 - e.BrowRaise + arch - inner + u * 0.8;
            int px = (int)Math.Round(x), py = (int)Math.Round(y);
            c[px, py] = Brow;
            if (u < 0.4) c[px, py + 1] = Brow;
        }
    }

    private static void DrawMascaraRun(Canvas c, double ex, int side, Expression e)
    {
        var rng = new Random(side > 0 ? 7 : 11);
        for (int streak = 0; streak < 2; streak++)
        {
            double x = ex + (streak == 0 ? -1.5 : 1.8) * side + rng.NextDouble() * 0.5;
            double y = Lower(x - ex, e) + 1.5;
            int length = 8 + rng.Next(6);
            for (int i = 0; i < length; i++)
            {
                x += (rng.NextDouble() - 0.5) * 0.6;
                y += 1;
                if (i % 5 == 3) continue;
                c[(int)Math.Round(x), (int)y] = i < 3 ? Lash : Mascara;
            }
        }
        c[(int)Math.Round(ex + side * 1.5), (int)Math.Ceiling(Lower(1.5, e)) + 1] = Tear;
    }

    private static void DrawBlush(Canvas c, Canvas head, Expression e)
    {
        double amount = 0.2 + e.Blush * 0.6;
        foreach (double bx in new[] { Cx - 13, Cx + 13 })
            for (int y = 68; y < 84; y++)
                for (int x = (int)bx - 8; x <= bx + 8; x++)
                {
                    if (!head.IsSet(x, y)) continue;
                    double d = Math.Sqrt(Math.Pow((x - bx) / 7.5, 2) + Math.Pow((y - 75) / 5, 2));
                    if (d >= 1) continue;
                    if ((1 - d) * amount > Bayer.At(x, y)) c[x, y] = c[x, y].Mix(BlushPink, 0.4);
                }
    }

    private static void DrawNoseDetail(Canvas c)
    {
        c[(int)Cx - 2, 78] = Skin.Step(1);
        c[(int)Cx + 3, 78] = Skin.Step(1);
        c[(int)Cx + 4, 77] = Skin.Step(2);
        c[(int)Cx, 76] = Skin.Step(6);
    }

    // ── Mouth ──────────────────────────────────────────────────

    private static void DrawMouth(Canvas c, Expression e)
    {
        double w = MouthHalfW + Math.Max(0, e.Smile) * 1.5 + e.MouthOpen * 0.5;

        double Seam(double dx)
        {
            double u = dx / w;
            double lift = e.Smile * 2.8 * u * u + (dx > 0 ? e.Lopsided * 1.6 * u * u : 0);
            return MouthY - lift;
        }
        double Open(double dx) => e.MouthOpen * (1 - Math.Pow(dx / w, 2));

        for (int x = (int)Math.Floor(Cx - w); x <= (int)Math.Ceiling(Cx + w); x++)
        {
            double dx = x + 0.5 - Cx;
            if (Math.Abs(dx) > w) continue;
            double u = Math.Abs(dx) / w;
            double seam = Seam(dx);
            double gapTop = seam - Open(dx) * 2.6;
            double gapBottom = seam + Open(dx) * 4.8;
            double upperThick = 2.1 * Math.Sqrt(1 - u * u) + 0.3 - (Math.Abs(dx) < 1.2 ? 0.6 : 0);
            double lowerThick = 2.7 * Math.Sqrt(1 - u * u) + 0.2;

            for (int y = (int)Math.Floor(gapTop - upperThick); y <= (int)Math.Ceiling(gapBottom + lowerThick); y++)
            {
                double py = y + 0.5;
                if (py >= gapTop - upperThick && py < gapTop)
                    c[x, y] = Lips.At(0.38 - u * 0.15, x, y);
                else if (py >= gapTop && py <= gapBottom && e.MouthOpen > 0.05)
                {
                    bool teeth = py < gapTop + 1.9 && u < 0.75 && e.MouthOpen > 0.25;
                    c[x, y] = teeth ? Teeth : MouthDark;
                }
                else if (py > gapBottom && py <= gapBottom + lowerThick)
                {
                    double t = 0.78 - (py - gapBottom) / lowerThick * 0.35 - u * 0.2;
                    bool shine = Math.Abs(dx + 1.5) < 1 && py - gapBottom < 1.2;
                    c[x, y] = shine ? Lips.Step(4) : Lips.At(t, x, y);
                }
            }
            if (e.MouthOpen <= 0.05) c[x, (int)Math.Floor(seam)] = u > 0.85 ? Skin.Step(1) : Lips.Step(0);
        }
        for (int x = (int)(Cx - 3); x <= Cx + 3; x++)
            c[x, (int)Math.Ceiling(Seam(0) + Open(0) * 4.8 + 3.6)] = Skin.Step(3);
    }

    private static void DrawSmileLines(Canvas c, Expression e)
    {
        foreach (int side in new[] { -1, 1 })
        {
            double strength = (e.Smile - 0.45) / 0.55;
            int steps = 3 + (int)(strength * 3);
            for (int i = 0; i < steps; i++)
            {
                double x = Cx + side * (7 + i * 0.9 + strength);
                double y = 78 + i * 1.3;
                c[(int)Math.Round(x), (int)Math.Round(y)] = Skin.Step(2);
            }
        }
    }

    // ── Earrings ───────────────────────────────────────────────

    private static void DrawEarrings(Canvas c)
    {
        foreach (int side in new[] { -1, 1 })
        {
            double ex = Cx + side * 25.5, ey = 85;
            for (int a = 0; a < 48; a++)
            {
                double ang = a / 48.0 * Math.PI * 2;
                int x = (int)Math.Round(ex + Math.Cos(ang) * 3.6);
                int y = (int)Math.Round(ey + Math.Sin(ang) * 4.4);
                double lit = 0.5 - Math.Cos(ang + 0.8) * 0.45;
                c[x, y] = Gold.At(lit, x, y);
            }
        }
    }
}
