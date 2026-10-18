namespace OfficeMan.ArtGen.Raster;

/// <summary>
/// A soft 3D lump seen from the front: an ellipsoid with a centre, radii and a forward offset.
/// </summary>
public readonly record struct Blob(double X, double Y, double Rx, double Ry, double Z = 0, double Depth = 1)
{
    /// <summary>Surface height at (px,py), or null if outside.</summary>
    public double? Height(double px, double py)
    {
        double dx = (px - X) / Rx;
        double dy = (py - Y) / Ry;
        double r2 = dx * dx + dy * dy;
        if (r2 > 1) return null;
        return Z + Math.Sqrt(1 - r2) * Depth * Math.Min(Rx, Ry);
    }
}

/// <summary>A light from a direction, plus ambient and an optional cool rim from the opposite side.</summary>
public sealed class Light(double lx, double ly, double lz, double ambient = 0.25, double rim = 0.0)
{
    private readonly double _lx = lx / Len(lx, ly, lz), _ly = ly / Len(lx, ly, lz), _lz = lz / Len(lx, ly, lz);

    private static double Len(double x, double y, double z) => Math.Sqrt(x * x + y * y + z * z);

    public double Shade(double nx, double ny, double nz)
    {
        double lambert = Math.Max(0, nx * _lx + ny * _ly + nz * _lz);
        double rimLight = rim * Math.Pow(Math.Max(0, 1 - nz), 3) * Math.Max(0, nx);
        return ambient + (1 - ambient) * lambert + rimLight;
    }
}

/// <summary>
/// A height field built from blobs with a smooth union, so lumps melt into each other
/// (cheeks into jaw, bust into torso) instead of meeting at hard seams.
/// </summary>
public sealed class HeightField
{
    public int Width { get; }
    public int Height { get; }
    private readonly double[] _z;
    private readonly bool[] _in;

    public HeightField(int width, int height, IReadOnlyList<Blob> blobs, double smoothness = 0.35)
    {
        Width = width;
        Height = height;
        _z = new double[width * height];
        _in = new bool[width * height];
        Span<double> hs = blobs.Count <= 256 ? stackalloc double[blobs.Count] : new double[blobs.Count];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
            {
                // Smooth maximum (log-sum-exp) over every blob covering this pixel.
                double max = double.NegativeInfinity;
                int n = 0;
                foreach (var b in blobs)
                    if (b.Height(x + 0.5, y + 0.5) is double h)
                    {
                        hs[n++] = h;
                        if (h > max) max = h;
                    }
                if (n == 0) continue;
                double sum = 0;
                for (int i = 0; i < n; i++) sum += Math.Exp(smoothness * (hs[i] - max));
                _z[y * width + x] = max + Math.Log(sum) / smoothness;
                _in[y * width + x] = true;
            }
    }

    public bool Contains(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height && _in[y * Width + x];

    public double Z(int x, int y) => _z[y * Width + x];

    /// <summary>Surface normal from central differences; edges fall away from the viewer.</summary>
    public (double nx, double ny, double nz) Normal(int x, int y)
    {
        double z = Z(x, y);
        double Sample(int sx, int sy) => Contains(sx, sy) ? Z(sx, sy) : z - 2.5;
        double dzdx = (Sample(x + 1, y) - Sample(x - 1, y)) / 2;
        double dzdy = (Sample(x, y + 1) - Sample(x, y - 1)) / 2;
        double nx = -dzdx, ny = -dzdy, nz = 1;
        double len = Math.Sqrt(nx * nx + ny * ny + nz * nz);
        return (nx / len, ny / len, nz / len);
    }
}

public static class Volume
{
    /// <summary>
    /// Lights a height field and quantises it onto a ramp. <paramref name="shade"/> may adjust the
    /// light level per pixel (texture, occlusion); <paramref name="clip"/> limits where it draws.
    /// </summary>
    public static void Render(Canvas layer, HeightField field, Ramp ramp, Light light,
        Func<int, int, double, double>? shade = null, double dither = 0.0, Func<int, int, bool>? clip = null)
    {
        for (int y = 0; y < layer.Height; y++)
            for (int x = 0; x < layer.Width; x++)
            {
                if (!field.Contains(x, y)) continue;
                if (clip is not null && !clip(x, y)) continue;
                var (nx, ny, nz) = field.Normal(x, y);
                double t = light.Shade(nx, ny, nz);
                if (shade is not null) t = shade(x, y, t);
                layer[x, y] = ramp.At(t, x, y, dither);
            }
    }

    public static void Render(Canvas layer, IReadOnlyList<Blob> blobs, Ramp ramp, Light light,
        Func<int, int, double, double>? shade = null, double dither = 0.0, Func<int, int, bool>? clip = null,
        double smoothness = 0.35) =>
        Render(layer, new HeightField(layer.Width, layer.Height, blobs, smoothness), ramp, light, shade, dither, clip);
}

/// <summary>Cellular (Worley) noise: distance to the nearest and second-nearest feature point.</summary>
public sealed class Cells(int seed, double cellSize)
{
    public (double f1, double f2) At(double x, double y)
    {
        double sx = x / cellSize, sy = y / cellSize;
        int cx = (int)Math.Floor(sx), cy = (int)Math.Floor(sy);
        double f1 = 9, f2 = 9;
        for (int j = -1; j <= 1; j++)
            for (int i = -1; i <= 1; i++)
            {
                int gx = cx + i, gy = cy + j;
                uint h = Hash(gx, gy);
                double px = gx + (h & 0xFFFF) / 65535.0;
                double py = gy + (h >> 16) / 65535.0;
                double d = Math.Sqrt((px - sx) * (px - sx) + (py - sy) * (py - sy));
                if (d < f1) { f2 = f1; f1 = d; }
                else if (d < f2) f2 = d;
            }
        return (f1, f2);
    }

    private uint Hash(int x, int y)
    {
        uint h = (uint)(x * 374761393 + y * 668265263 + seed * 2246822519);
        h = (h ^ (h >> 13)) * 1274126177;
        return h ^ (h >> 16);
    }
}
