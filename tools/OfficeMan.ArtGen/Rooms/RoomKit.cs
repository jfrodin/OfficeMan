using System.Text.Json;
using OfficeMan.ArtGen.Raster;
using OfficeMan.ArtGen.Scenes;

namespace OfficeMan.ArtGen.Rooms;

/// <summary>Layers every room produces for the room shader. See design/visual_standard.md.</summary>
/// <remarks>Mask: which pixels belong to which interactable thing (content/rooms/*.json "mask"), for lighting it up.</remarks>
public sealed record RoomLayers(Canvas Albedo, Canvas Emissive, Canvas Glow, Canvas Front, Canvas? Mask = null);

/// <summary>Shared tools for drawing rooms to the visual standard.</summary>
public static class RoomKit
{
    public const int W = 480, H = 270;

    /// <summary>Bloom: the emissive layer blurred at two widths, so glows have a tight core and a wide halo.</summary>
    public static Canvas Glow(Canvas e)
    {
        double[] Channel(Func<Rgb, byte> pick) =>
            Enumerable.Range(0, W * H).Select(i => e.IsSet(i % W, i / W) ? pick(e[i % W, i / W]) / 255.0 : 0).ToArray();
        var rgb = new[] { Channel(c => c.R), Channel(c => c.G), Channel(c => c.B) };
        var tight = rgb.Select(c => Blur(c, 3, 3)).ToArray();
        var wide = rgb.Select(c => Blur(c, 12, 3)).ToArray();
        var g = new Canvas(W, H);
        for (int i = 0; i < W * H; i++)
        {
            byte Ch(int k) => (byte)Math.Clamp((tight[k][i] * 0.5 + wide[k][i] * 0.7) * 255, 0, 255);
            g[i % W, i / W] = new Rgb(Ch(0), Ch(1), Ch(2));
        }
        return g;
    }

    private static double[] Blur(double[] src, int r, int passes)
    {
        var a = (double[])src.Clone();
        var b = new double[a.Length];
        for (int p = 0; p < passes; p++)
        {
            for (int y = 0; y < H; y++)
            {
                double acc = 0;
                for (int x = -r; x <= r; x++) acc += a[y * W + Math.Clamp(x, 0, W - 1)];
                for (int x = 0; x < W; x++)
                {
                    b[y * W + x] = acc / (2 * r + 1);
                    acc += a[y * W + Math.Clamp(x + r + 1, 0, W - 1)] - a[y * W + Math.Clamp(x - r, 0, W - 1)];
                }
            }
            for (int x = 0; x < W; x++)
            {
                double acc = 0;
                for (int y = -r; y <= r; y++) acc += b[Math.Clamp(y, 0, H - 1) * W + x];
                for (int y = 0; y < H; y++)
                {
                    a[y * W + x] = acc / (2 * r + 1);
                    acc += b[Math.Clamp(y + r + 1, 0, H - 1) * W + x] - b[Math.Clamp(y - r, 0, H - 1) * W + x];
                }
            }
        }
        return a;
    }

    public static void ClearEmissive(Canvas e, int x0, int y0, int x1, int y1)
    {
        for (int y = y0; y < y1; y++)
            for (int x = x0; x < x1; x++) e.Clear(x, y);
    }

    public static void Emit(Canvas e, int x0, int y0, int x1, int y1, Rgb c)
    {
        for (int y = y0; y < y1; y++)
            for (int x = x0; x < x1; x++) e[x, y] = c;
    }

    /// <summary>Deterministic, small-amplitude variation in [-0.5, 0.5] for material texture, not decoration.</summary>
    public static double Noise(int x, int y, int seed)
    {
        uint h = (uint)(x * 374761393 + y * 668265263 + seed * 1442695041);
        h = (h ^ (h >> 13)) * 1274126177;
        return ((h ^ (h >> 16)) & 0xFFFF) / 65535.0 - 0.5;
    }

    public static IEnumerable<(int x, int y)> Circle(int cx, int cy, int r)
    {
        for (int y = cy - r; y <= cy + r; y++)
            for (int x = cx - r; x <= cx + r; x++)
                if ((x - cx) * (x - cx) + (y - cy) * (y - cy) <= r * r) yield return (x, y);
    }

    public static IEnumerable<(int x, int y)> Ring(int cx, int cy, int r)
    {
        for (int y = cy - r; y <= cy + r; y++)
            for (int x = cx - r; x <= cx + r; x++)
            {
                int d = (x - cx) * (x - cx) + (y - cy) * (y - cy);
                if (d <= r * r && d > (r - 1) * (r - 1)) yield return (x, y);
            }
    }

    /// <summary>A piece of furniture as content/rooms/&lt;id&gt;.json places it: its sprite, front foot and footprint.</summary>
    public sealed record Placed(string Sprite, int X, int Y, int HalfW, int Depth);

    /// <summary>The furniture (props with a footprint) in a room's layout, so the pictures agree with the json.</summary>
    public static List<Placed> Props(string roomId)
    {
        string root = Directory.GetCurrentDirectory();
        for (var d = new DirectoryInfo(root); d is not null; d = d.Parent)
            if (File.Exists(Path.Combine(d.FullName, "OfficeMan.sln"))) { root = d.FullName; break; }
        var options = new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true };
        using var doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(root, "content", "rooms", roomId + ".json")), options);
        var list = new List<Placed>();
        foreach (var t in doc.RootElement.GetProperty("things").EnumerateArray())
        {
            if (!t.TryGetProperty("prop", out var prop) || !prop.GetBoolean()) continue;
            if (!t.TryGetProperty("block", out var b)) continue;
            list.Add(new Placed(t.GetProperty("sprite").GetString()!, t.GetProperty("x").GetInt32(), t.GetProperty("y").GetInt32(),
                b[0].GetInt32(), b[1].GetInt32()));
        }
        return list;
    }

    /// <summary>A soft contact shadow on the floor under each piece of furniture: darker in the middle, a little wider than it.</summary>
    public static void PropShadows(SceneCanvas a, string roomId)
    {
        foreach (var p in Props(roomId))
        {
            double cx = p.X, cy = p.Y - p.Depth / 2.0, rx = p.HalfW + 3, ry = p.Depth / 2.0 + 2;
            for (int y = (int)(cy - ry); y <= (int)(cy + ry); y++)
                for (int x = (int)(cx - rx); x <= (int)(cx + rx); x++)
                {
                    double d = Math.Pow((x - cx) / rx, 2) + Math.Pow((y - cy) / ry, 2);
                    if (d < 1) a.AddLight(x, y, d < 0.55 ? -0.2 : -0.1);
                }
        }
    }

    /// <summary>Digits in a 3×5 pixel font, for clocks and door numbers.</summary>
    public static void Digits(Action<int, int> plot, int x, int y, string text)
    {
        var glyphs = new Dictionary<char, string[]>
        {
            ['0'] = new[] { "###", "#.#", "#.#", "#.#", "###" }, ['1'] = new[] { ".#.", "##.", ".#.", ".#.", "###" },
            ['2'] = new[] { "###", "..#", "###", "#..", "###" }, ['3'] = new[] { "###", "..#", ".##", "..#", "###" },
            ['4'] = new[] { "#.#", "#.#", "###", "..#", "..#" }, ['5'] = new[] { "###", "#..", "###", "..#", "###" },
            ['6'] = new[] { "###", "#..", "###", "#.#", "###" }, ['7'] = new[] { "###", "..#", "..#", "..#", "..#" },
            ['8'] = new[] { "###", "#.#", "###", "#.#", "###" }, ['9'] = new[] { "###", "#.#", "###", "..#", "###" },
            [':'] = new[] { "...", ".#.", "...", ".#.", "..." },
        };
        foreach (char ch in text)
        {
            if (!glyphs.TryGetValue(ch, out var g)) { x += 2; continue; }
            int w = ch == ':' ? 1 : 3, off = ch == ':' ? 1 : 0;
            for (int j = 0; j < 5; j++)
                for (int i = 0; i < w; i++)
                    if (g[j][i + off] == '#') plot(x + i, y + j);
            x += w + 1;
        }
    }
}
