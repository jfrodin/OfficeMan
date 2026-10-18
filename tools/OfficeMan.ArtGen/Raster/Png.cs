using System.IO.Compression;
using System.Text;

namespace OfficeMan.ArtGen.Raster;

/// <summary>Minimal PNG writer (8-bit RGBA). Unset canvas pixels are transparent.</summary>
public static class Png
{
    public static void Save(Canvas canvas, string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(path))!);
        using var file = File.Create(path);
        file.Write(new byte[] { 137, 80, 78, 71, 13, 10, 26, 10 });

        var header = new byte[13];
        WriteBigEndian(header, 0, canvas.Width);
        WriteBigEndian(header, 4, canvas.Height);
        header[8] = 8; // bit depth
        header[9] = 6; // RGBA
        WriteChunk(file, "IHDR", header);

        using var raw = new MemoryStream();
        for (int y = 0; y < canvas.Height; y++)
        {
            raw.WriteByte(0); // no filter
            for (int x = 0; x < canvas.Width; x++)
            {
                var c = canvas[x, y];
                raw.WriteByte(c.R);
                raw.WriteByte(c.G);
                raw.WriteByte(c.B);
                raw.WriteByte(canvas.IsSet(x, y) ? (byte)255 : (byte)0);
            }
        }

        using var compressed = new MemoryStream();
        using (var z = new ZLibStream(compressed, CompressionLevel.SmallestSize, leaveOpen: true))
            raw.WriteTo(z);
        WriteChunk(file, "IDAT", compressed.ToArray());
        WriteChunk(file, "IEND", Array.Empty<byte>());
    }

    private static void WriteChunk(Stream s, string type, byte[] data)
    {
        var len = new byte[4];
        WriteBigEndian(len, 0, data.Length);
        s.Write(len);
        var typeBytes = Encoding.ASCII.GetBytes(type);
        s.Write(typeBytes);
        s.Write(data);
        var crc = new byte[4];
        WriteBigEndian(crc, 0, (int)Crc32(typeBytes, data));
        s.Write(crc);
    }

    private static void WriteBigEndian(byte[] b, int offset, int v)
    {
        b[offset] = (byte)(v >> 24);
        b[offset + 1] = (byte)(v >> 16);
        b[offset + 2] = (byte)(v >> 8);
        b[offset + 3] = (byte)v;
    }

    private static readonly uint[] CrcTable = BuildCrcTable();

    private static uint[] BuildCrcTable()
    {
        var t = new uint[256];
        for (uint n = 0; n < 256; n++)
        {
            uint c = n;
            for (int k = 0; k < 8; k++) c = (c & 1) != 0 ? 0xEDB88320u ^ (c >> 1) : c >> 1;
            t[n] = c;
        }
        return t;
    }

    private static uint Crc32(byte[] a, byte[] b)
    {
        uint c = 0xFFFFFFFFu;
        foreach (byte x in a) c = CrcTable[(c ^ x) & 0xFF] ^ (c >> 8);
        foreach (byte x in b) c = CrcTable[(c ^ x) & 0xFF] ^ (c >> 8);
        return c ^ 0xFFFFFFFFu;
    }
}
