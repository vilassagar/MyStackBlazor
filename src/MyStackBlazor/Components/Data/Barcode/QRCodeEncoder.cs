using System.Text;

namespace MyStackBlazor.Components.Data;

/// <summary>QR error-correction level (Telerik names). Higher levels survive more damage but hold less data.</summary>
public enum QRCodeErrorCorrection
{
    /// <summary>~7% recovery.</summary>
    Low,
    /// <summary>~15% recovery.</summary>
    Medium,
    /// <summary>~25% recovery.</summary>
    Quartile,
    /// <summary>~30% recovery; use it when an overlay covers the centre.</summary>
    High,
}

/// <summary>How text is turned into bytes (Telerik names).</summary>
public enum QRCodeEncoding
{
    /// <summary>Latin-1; characters above U+00FF cannot be encoded.</summary>
    ISO_8859_1,
    /// <summary>UTF-8, announced with an ECI header when the text is not plain ASCII.</summary>
    UTF_8,
}

/// <summary>An encoded QR symbol: a square grid of dark/light modules.</summary>
public sealed class QRCodeMatrix
{
    private readonly bool[,] _modules;

    internal QRCodeMatrix(int version, QRCodeErrorCorrection errorCorrection, int mask, bool[,] modules)
    {
        Version = version;
        ErrorCorrection = errorCorrection;
        Mask = mask;
        _modules = modules;
    }

    /// <summary>1–40; the symbol is 17 + 4 × Version modules wide.</summary>
    public int Version { get; }
    public QRCodeErrorCorrection ErrorCorrection { get; }
    public int Mask { get; }
    public int Size => _modules.GetLength(0);

    /// <summary>True when the module at column <paramref name="x"/>, row <paramref name="y"/> is dark.</summary>
    public bool this[int x, int y] => _modules[y, x];
}

/// <summary>
/// QR Code model 2 encoder (ISO/IEC 18004): numeric, alphanumeric and byte modes, versions 1–40,
/// all four error-correction levels and automatic mask selection.
/// </summary>
public static class QRCodeEncoder
{
    // ── Tables (index 0 unused; indexed by error-correction level then version) ──
    private static readonly int[][] EccCodewordsPerBlock =
    [
        [-1, 7, 10, 15, 20, 26, 18, 20, 24, 30, 18, 20, 24, 26, 30, 22, 24, 28, 30, 28, 28, 28, 28, 30, 30, 26, 28, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30],
        [-1, 10, 16, 26, 18, 24, 16, 18, 22, 22, 26, 30, 22, 22, 24, 24, 28, 28, 26, 26, 26, 26, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28, 28],
        [-1, 13, 22, 18, 26, 18, 24, 18, 22, 20, 24, 28, 26, 24, 20, 30, 24, 28, 28, 26, 30, 28, 30, 30, 30, 30, 28, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30],
        [-1, 17, 28, 22, 16, 22, 28, 26, 26, 24, 28, 24, 28, 22, 24, 24, 30, 28, 28, 26, 28, 30, 24, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30],
    ];

    private static readonly int[][] ErrorCorrectionBlocks =
    [
        [-1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 4, 4, 4, 4, 4, 6, 6, 6, 6, 7, 8, 8, 9, 9, 10, 12, 12, 12, 13, 14, 15, 16, 17, 18, 19, 19, 20, 21, 22, 24, 25],
        [-1, 1, 1, 1, 2, 2, 4, 4, 4, 5, 5, 5, 8, 9, 9, 10, 10, 11, 13, 14, 16, 17, 17, 18, 20, 21, 23, 25, 26, 28, 29, 31, 33, 35, 37, 38, 40, 43, 45, 47, 49],
        [-1, 1, 1, 2, 2, 4, 4, 6, 6, 8, 8, 8, 10, 12, 16, 12, 17, 16, 18, 21, 20, 23, 23, 25, 27, 29, 34, 34, 35, 38, 40, 43, 45, 48, 51, 53, 56, 59, 62, 65, 68],
        [-1, 1, 1, 2, 4, 4, 4, 5, 6, 8, 8, 11, 11, 16, 16, 18, 16, 19, 21, 25, 25, 25, 34, 30, 32, 35, 37, 40, 42, 45, 48, 51, 54, 57, 60, 63, 66, 70, 74, 77, 81],
    ];

    /// <summary>The 2-bit level code written into the format information.</summary>
    private static int FormatBits(QRCodeErrorCorrection ecl) => ecl switch
    {
        QRCodeErrorCorrection.Low      => 1,
        QRCodeErrorCorrection.Medium   => 0,
        QRCodeErrorCorrection.Quartile => 3,
        _                              => 2,
    };

    private const string AlphanumericChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ $%*+-./:";

    // ── Public API ───────────────────────────────────────────────────────────
    /// <summary>Encodes <paramref name="text"/> in the smallest version that fits. Throws <see cref="ArgumentException"/> if it is too long or not encodable.</summary>
    public static QRCodeMatrix Encode(string text, QRCodeErrorCorrection ecl = QRCodeErrorCorrection.Low, QRCodeEncoding encoding = QRCodeEncoding.ISO_8859_1)
    {
        ArgumentNullException.ThrowIfNull(text);
        var segment = Segment.For(text, encoding);

        int version;
        int dataBits = 0;
        for (version = 1; ; version++)
        {
            var capacity = DataCodewords(version, ecl) * 8;
            dataBits = segment.TotalBits(version);
            if (dataBits <= capacity) break;
            if (version == 40) throw new ArgumentException($"The value is too long for a QR code at {ecl} error correction.");
        }

        var codewords = BuildDataCodewords(segment, version, ecl);
        var allCodewords = AddErrorCorrectionAndInterleave(codewords, version, ecl);
        return Draw(version, ecl, allCodewords);
    }

    // ── Segments ─────────────────────────────────────────────────────────────
    private sealed class Segment
    {
        public required int Mode { get; init; }          // 1 numeric, 2 alphanumeric, 4 byte
        public required int CharCount { get; init; }
        public required List<bool> Data { get; init; }
        public int? Eci { get; init; }

        public int CountBits(int version) => (Mode, version) switch
        {
            (1, < 10) => 10, (1, < 27) => 12, (1, _) => 14,
            (2, < 10) => 9,  (2, < 27) => 11, (2, _) => 13,
            (_, < 10) => 8,  _ => 16,
        };

        public int TotalBits(int version)
        {
            var bits = 4 + CountBits(version) + Data.Count;
            if (CharCount >= 1 << CountBits(version)) return int.MaxValue;
            return Eci is null ? bits : bits + 12;
        }

        public static Segment For(string text, QRCodeEncoding encoding)
        {
            if (text.Length > 0 && text.All(char.IsAsciiDigit))
            {
                var bits = new List<bool>();
                for (var i = 0; i < text.Length; i += 3)
                {
                    var chunk = text.Substring(i, Math.Min(3, text.Length - i));
                    AppendBits(bits, int.Parse(chunk), chunk.Length * 3 + 1);
                }
                return new Segment { Mode = 1, CharCount = text.Length, Data = bits };
            }

            if (text.Length > 0 && text.All(c => AlphanumericChars.Contains(c)))
            {
                var bits = new List<bool>();
                for (var i = 0; i + 1 < text.Length; i += 2)
                    AppendBits(bits, AlphanumericChars.IndexOf(text[i]) * 45 + AlphanumericChars.IndexOf(text[i + 1]), 11);
                if (text.Length % 2 == 1) AppendBits(bits, AlphanumericChars.IndexOf(text[^1]), 6);
                return new Segment { Mode = 2, CharCount = text.Length, Data = bits };
            }

            byte[] bytes;
            int? eci = null;
            if (encoding == QRCodeEncoding.UTF_8)
            {
                bytes = Encoding.UTF8.GetBytes(text);
                if (text.Any(c => c > 127)) eci = 26; // ECI 26 = UTF-8
            }
            else
            {
                if (text.Any(c => c > 255)) throw new ArgumentException("The value has characters outside ISO-8859-1; use Encoding UTF_8.");
                bytes = text.Select(c => (byte)c).ToArray();
            }

            var data = new List<bool>(bytes.Length * 8);
            foreach (var b in bytes) AppendBits(data, b, 8);
            return new Segment { Mode = 4, CharCount = bytes.Length, Data = data, Eci = eci };
        }
    }

    private static void AppendBits(List<bool> bits, int value, int length)
    {
        for (var i = length - 1; i >= 0; i--) bits.Add(((value >> i) & 1) != 0);
    }

    // ── Capacity ─────────────────────────────────────────────────────────────
    internal static int RawDataModules(int version)
    {
        var result = (16 * version + 128) * version + 64;
        if (version >= 2)
        {
            var alignments = version / 7 + 2;
            result -= (25 * alignments - 10) * alignments - 55;
            if (version >= 7) result -= 36;
        }
        return result;
    }

    internal static int DataCodewords(int version, QRCodeErrorCorrection ecl) =>
        RawDataModules(version) / 8 - EccCodewordsPerBlock[(int)ecl][version] * ErrorCorrectionBlocks[(int)ecl][version];

    // ── Codewords ────────────────────────────────────────────────────────────
    internal static byte[] BuildDataCodewords(string text, QRCodeErrorCorrection ecl, QRCodeEncoding encoding, out int version)
    {
        var segment = Segment.For(text, encoding);
        for (version = 1; version <= 40; version++)
            if (segment.TotalBits(version) <= DataCodewords(version, ecl) * 8) break;
        return BuildDataCodewords(segment, version, ecl);
    }

    private static byte[] BuildDataCodewords(Segment segment, int version, QRCodeErrorCorrection ecl)
    {
        var bits = new List<bool>();
        if (segment.Eci is int eci)
        {
            AppendBits(bits, 0b0111, 4);
            AppendBits(bits, eci, 8);
        }
        AppendBits(bits, segment.Mode, 4);
        AppendBits(bits, segment.CharCount, segment.CountBits(version));
        bits.AddRange(segment.Data);

        var capacityBits = DataCodewords(version, ecl) * 8;
        AppendBits(bits, 0, Math.Min(4, capacityBits - bits.Count));        // terminator
        AppendBits(bits, 0, (8 - bits.Count % 8) % 8);                       // byte align
        for (var pad = 0xEC; bits.Count < capacityBits; pad ^= 0xEC ^ 0x11)  // 0xEC, 0x11, …
            AppendBits(bits, pad, 8);

        var bytes = new byte[bits.Count / 8];
        for (var i = 0; i < bits.Count; i++)
            if (bits[i]) bytes[i >> 3] |= (byte)(1 << (7 - (i & 7)));
        return bytes;
    }

    // ── Reed-Solomon over GF(256), polynomial 0x11D ──────────────────────────
    private static int GfMultiply(int x, int y)
    {
        var z = 0;
        for (var i = 7; i >= 0; i--)
        {
            z = (z << 1) ^ ((z >> 7) * 0x11D);
            z ^= ((y >> i) & 1) * x;
        }
        return z & 0xFF;
    }

    internal static byte[] ReedSolomonDivisor(int degree)
    {
        var result = new byte[degree];
        result[degree - 1] = 1;
        var root = 1;
        for (var i = 0; i < degree; i++)
        {
            for (var j = 0; j < result.Length; j++)
            {
                result[j] = (byte)GfMultiply(result[j], root);
                if (j + 1 < result.Length) result[j] ^= result[j + 1];
            }
            root = GfMultiply(root, 0x02);
        }
        return result;
    }

    internal static byte[] ReedSolomonRemainder(ReadOnlySpan<byte> data, byte[] divisor)
    {
        var result = new byte[divisor.Length];
        foreach (var b in data)
        {
            var factor = b ^ result[0];
            Array.Copy(result, 1, result, 0, result.Length - 1);
            result[^1] = 0;
            for (var i = 0; i < result.Length; i++) result[i] ^= (byte)GfMultiply(divisor[i], factor);
        }
        return result;
    }

    private static byte[] AddErrorCorrectionAndInterleave(byte[] data, int version, QRCodeErrorCorrection ecl)
    {
        var numBlocks = ErrorCorrectionBlocks[(int)ecl][version];
        var blockEccLength = EccCodewordsPerBlock[(int)ecl][version];
        var rawCodewords = RawDataModules(version) / 8;
        var numShortBlocks = numBlocks - rawCodewords % numBlocks;
        var shortBlockLength = rawCodewords / numBlocks;

        var blocks = new List<byte[]>();
        var divisor = ReedSolomonDivisor(blockEccLength);
        for (int i = 0, k = 0; i < numBlocks; i++)
        {
            var dataLength = shortBlockLength - blockEccLength + (i < numShortBlocks ? 0 : 1);
            var blockData = data.AsSpan(k, dataLength);
            k += dataLength;
            var ecc = ReedSolomonRemainder(blockData, divisor);

            // Short blocks get a placeholder byte so every block has the same length for interleaving.
            var block = new byte[shortBlockLength + 1];
            blockData.CopyTo(block);
            ecc.CopyTo(block, block.Length - blockEccLength);
            blocks.Add(block);
        }

        var result = new List<byte>(rawCodewords);
        for (var i = 0; i < blocks[0].Length; i++)
            for (var j = 0; j < blocks.Count; j++)
                if (i != shortBlockLength - blockEccLength || j >= numShortBlocks)
                    result.Add(blocks[j][i]);
        return result.ToArray();
    }

    // ── Drawing ──────────────────────────────────────────────────────────────
    private sealed class Grid(int size)
    {
        public readonly bool[,] Modules = new bool[size, size];
        public readonly bool[,] IsFunction = new bool[size, size];
        public int Size { get; } = size;

        public void SetFunction(int x, int y, bool dark)
        {
            Modules[y, x] = dark;
            IsFunction[y, x] = true;
        }
    }

    internal static int[] AlignmentPositions(int version)
    {
        if (version == 1) return [];
        var count = version / 7 + 2;
        var size = version * 4 + 17;
        var step = version == 32 ? 26 : (version * 4 + count * 2 + 1) / (count * 2 - 2) * 2;
        var result = new int[count];
        result[0] = 6;
        for (int i = count - 1, pos = size - 7; i >= 1; i--, pos -= step) result[i] = pos;
        return result;
    }

    /// <summary>The 15 format bits for a level and mask (BCH-encoded and XOR-masked).</summary>
    internal static int FormatInformation(QRCodeErrorCorrection ecl, int mask)
    {
        var data = FormatBits(ecl) << 3 | mask;
        var rem = data;
        for (var i = 0; i < 10; i++) rem = (rem << 1) ^ ((rem >> 9) * 0x537);
        return ((data << 10) | rem) ^ 0x5412;
    }

    private static QRCodeMatrix Draw(int version, QRCodeErrorCorrection ecl, byte[] codewords)
    {
        var grid = new Grid(version * 4 + 17);
        DrawFunctionPatterns(grid, version, ecl);
        DrawCodewords(grid, codewords);

        // Pick the mask with the lowest penalty.
        var bestMask = 0;
        var bestPenalty = int.MaxValue;
        for (var mask = 0; mask < 8; mask++)
        {
            ApplyMask(grid, mask);
            DrawFormatBits(grid, ecl, mask);
            var penalty = Penalty(grid);
            if (penalty < bestPenalty) { bestMask = mask; bestPenalty = penalty; }
            ApplyMask(grid, mask); // XOR again to undo
        }

        ApplyMask(grid, bestMask);
        DrawFormatBits(grid, ecl, bestMask);
        return new QRCodeMatrix(version, ecl, bestMask, grid.Modules);
    }

    private static void DrawFunctionPatterns(Grid g, int version, QRCodeErrorCorrection ecl)
    {
        var size = g.Size;
        for (var i = 0; i < size; i++)
        {
            g.SetFunction(6, i, i % 2 == 0);
            g.SetFunction(i, 6, i % 2 == 0);
        }

        DrawFinder(g, 3, 3);
        DrawFinder(g, size - 4, 3);
        DrawFinder(g, 3, size - 4);

        var positions = AlignmentPositions(version);
        for (var i = 0; i < positions.Length; i++)
            for (var j = 0; j < positions.Length; j++)
            {
                // Skip the three corners occupied by finder patterns.
                if ((i == 0 && j == 0) || (i == 0 && j == positions.Length - 1) || (i == positions.Length - 1 && j == 0)) continue;
                for (var dy = -2; dy <= 2; dy++)
                    for (var dx = -2; dx <= 2; dx++)
                        g.SetFunction(positions[i] + dx, positions[j] + dy, Math.Max(Math.Abs(dx), Math.Abs(dy)) != 1);
            }

        DrawFormatBits(g, ecl, 0); // reserve the area; redrawn after masking
        DrawVersion(g, version);
    }

    private static void DrawFinder(Grid g, int x, int y)
    {
        for (var dy = -4; dy <= 4; dy++)
            for (var dx = -4; dx <= 4; dx++)
            {
                int xx = x + dx, yy = y + dy;
                if (xx < 0 || xx >= g.Size || yy < 0 || yy >= g.Size) continue;
                var distance = Math.Max(Math.Abs(dx), Math.Abs(dy));
                g.SetFunction(xx, yy, distance != 2 && distance != 4);
            }
    }

    private static void DrawFormatBits(Grid g, QRCodeErrorCorrection ecl, int mask)
    {
        var bits = FormatInformation(ecl, mask);
        bool Bit(int i) => ((bits >> i) & 1) != 0;
        var size = g.Size;

        for (var i = 0; i <= 5; i++) g.SetFunction(8, i, Bit(i));
        g.SetFunction(8, 7, Bit(6));
        g.SetFunction(8, 8, Bit(7));
        g.SetFunction(7, 8, Bit(8));
        for (var i = 9; i < 15; i++) g.SetFunction(14 - i, 8, Bit(i));

        for (var i = 0; i < 8; i++) g.SetFunction(size - 1 - i, 8, Bit(i));
        for (var i = 8; i < 15; i++) g.SetFunction(8, size - 15 + i, Bit(i));
        g.SetFunction(8, size - 8, true); // always-dark module
    }

    private static void DrawVersion(Grid g, int version)
    {
        if (version < 7) return;
        var rem = version;
        for (var i = 0; i < 12; i++) rem = (rem << 1) ^ ((rem >> 11) * 0x1F25);
        var bits = (version << 12) | rem;
        for (var i = 0; i < 18; i++)
        {
            var dark = ((bits >> i) & 1) != 0;
            int a = g.Size - 11 + i % 3, b = i / 3;
            g.SetFunction(a, b, dark);
            g.SetFunction(b, a, dark);
        }
    }

    private static void DrawCodewords(Grid g, byte[] data)
    {
        var size = g.Size;
        var bit = 0;
        for (var right = size - 1; right >= 1; right -= 2)
        {
            if (right == 6) right = 5; // skip the vertical timing column
            for (var vert = 0; vert < size; vert++)
                for (var j = 0; j < 2; j++)
                {
                    var x = right - j;
                    var upward = ((right + 1) & 2) == 0;
                    var y = upward ? size - 1 - vert : vert;
                    if (g.IsFunction[y, x] || bit >= data.Length * 8) continue;
                    g.Modules[y, x] = ((data[bit >> 3] >> (7 - (bit & 7))) & 1) != 0;
                    bit++;
                }
        }
    }

    private static bool MaskBit(int mask, int x, int y) => mask switch
    {
        0 => (x + y) % 2 == 0,
        1 => y % 2 == 0,
        2 => x % 3 == 0,
        3 => (x + y) % 3 == 0,
        4 => (x / 3 + y / 2) % 2 == 0,
        5 => x * y % 2 + x * y % 3 == 0,
        6 => (x * y % 2 + x * y % 3) % 2 == 0,
        _ => ((x + y) % 2 + x * y % 3) % 2 == 0,
    };

    private static void ApplyMask(Grid g, int mask)
    {
        for (var y = 0; y < g.Size; y++)
            for (var x = 0; x < g.Size; x++)
                if (!g.IsFunction[y, x] && MaskBit(mask, x, y))
                    g.Modules[y, x] = !g.Modules[y, x];
    }

    /// <summary>The four ISO penalty rules: runs, 2×2 blocks, finder-like patterns, dark balance.</summary>
    private static int Penalty(Grid g)
    {
        var size = g.Size;
        var m = g.Modules;
        var penalty = 0;

        for (var pass = 0; pass < 2; pass++)
        {
            for (var a = 0; a < size; a++)
            {
                var run = 1;
                var line = new StringBuilder(size + 8).Append("0000");
                for (var b = 0; b < size; b++)
                {
                    var current = pass == 0 ? m[a, b] : m[b, a];
                    line.Append(current ? '1' : '0');
                    if (b > 0 && current == (pass == 0 ? m[a, b - 1] : m[b - 1, a]))
                    {
                        run++;
                        if (run == 5) penalty += 3;
                        else if (run > 5) penalty++;
                    }
                    else run = 1;
                }
                line.Append("0000");
                var s = line.ToString();
                for (var i = s.IndexOf("10111010000", StringComparison.Ordinal); i >= 0; i = s.IndexOf("10111010000", i + 1, StringComparison.Ordinal)) penalty += 40;
                for (var i = s.IndexOf("00001011101", StringComparison.Ordinal); i >= 0; i = s.IndexOf("00001011101", i + 1, StringComparison.Ordinal)) penalty += 40;
            }
        }

        for (var y = 0; y < size - 1; y++)
            for (var x = 0; x < size - 1; x++)
                if (m[y, x] == m[y, x + 1] && m[y, x] == m[y + 1, x] && m[y, x] == m[y + 1, x + 1]) penalty += 3;

        var dark = 0;
        foreach (var module in m) if (module) dark++;
        var total = size * size;
        var k = (Math.Abs(dark * 20 - total * 10) + total - 1) / total - 1;
        penalty += k * 10;
        return penalty;
    }
}
