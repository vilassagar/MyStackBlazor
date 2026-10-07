using System.Text;

namespace MyStackBlazor.Components.Data;

/// <summary>Symbologies supported by <see cref="StackBarcode"/> (same names as the Telerik BarcodeType).</summary>
public enum BarcodeType
{
    Code11,
    Code39,
    Code39Extended,
    Code93,
    Code93Extended,
    /// <summary>Code 128 with automatic switching between subsets A, B and C.</summary>
    Code128,
    Code128A,
    Code128B,
    Code128C,
    /// <summary>GS1-128. Write application identifiers in parentheses: "(01)09501101530003(10)ABC123".</summary>
    GS1128,
    EAN8,
    EAN13,
    UPCA,
    UPCE,
    MSImod10,
    MSImod11,
    MSImod1010,
    MSImod1110,
    POSTNET,
}

/// <summary>One bar, in modules. <see cref="Top"/> is the fraction of the bar height left empty above it (POSTNET half bars).</summary>
public sealed record BarcodeBar(int Start, int Width, double Top = 0, bool Guard = false);

/// <summary>Human-readable text centred on <see cref="Center"/> (in modules).</summary>
public sealed record BarcodeTextSegment(string Text, double Center);

/// <summary>The result of encoding a value: bars and text laid out on a grid of <see cref="Modules"/> units.</summary>
public sealed class BarcodeEncoding
{
    public required int Modules { get; init; }
    public required IReadOnlyList<BarcodeBar> Bars { get; init; }
    public required IReadOnlyList<BarcodeTextSegment> Text { get; init; }
    /// <summary>The data actually encoded, including any check digits.</summary>
    public required string EncodedValue { get; init; }
}

/// <summary>Turns values into bar patterns. Throws <see cref="ArgumentException"/> for values the symbology cannot encode.</summary>
public static class BarcodeEncoder
{
    public static BarcodeEncoding Encode(BarcodeType type, string? value, bool checksum = false)
    {
        value ??= "";
        if (value.Length == 0) throw new ArgumentException("A value is required.", nameof(value));

        return type switch
        {
            BarcodeType.Code11         => Code11(value, checksum),
            BarcodeType.Code39         => Code39(value.ToUpperInvariant(), value, checksum),
            BarcodeType.Code39Extended => Code39(Code39FullAscii(value), value, checksum),
            BarcodeType.Code93         => Code93(value, extended: false),
            BarcodeType.Code93Extended => Code93(value, extended: true),
            BarcodeType.Code128        => Code128(value, Code128Set.Auto),
            BarcodeType.Code128A       => Code128(value, Code128Set.A),
            BarcodeType.Code128B       => Code128(value, Code128Set.B),
            BarcodeType.Code128C       => Code128(value, Code128Set.C),
            BarcodeType.GS1128         => Gs1128(value),
            BarcodeType.EAN8           => Ean8(value),
            BarcodeType.EAN13          => Ean13(value, upcA: false),
            BarcodeType.UPCA           => Ean13(value, upcA: true),
            BarcodeType.UPCE           => UpcE(value),
            BarcodeType.MSImod10       => Msi(value, "10"),
            BarcodeType.MSImod11       => Msi(value, "11"),
            BarcodeType.MSImod1010     => Msi(value, "1010"),
            BarcodeType.MSImod1110     => Msi(value, "1110"),
            BarcodeType.POSTNET        => Postnet(value),
            _                          => throw new ArgumentOutOfRangeException(nameof(type)),
        };
    }

    // ── Pattern builder ──────────────────────────────────────────────────────
    /// <summary>Collects modules ('1' = bar, '0' = space) and guard ranges, then turns them into bars.</summary>
    private sealed class Pattern
    {
        private readonly StringBuilder _modules = new();
        private readonly List<(int Start, int End)> _guards = [];

        public int Length => _modules.Length;

        public Pattern Modules(string bits, bool guard = false)
        {
            if (guard) _guards.Add((_modules.Length, _modules.Length + bits.Length));
            _modules.Append(bits);
            return this;
        }

        /// <summary>Appends alternating bar/space widths, starting with a bar (e.g. "212222").</summary>
        public Pattern Widths(string widths)
        {
            var bar = true;
            foreach (var w in widths)
            {
                _modules.Append(bar ? '1' : '0', w - '0');
                bar = !bar;
            }
            return this;
        }

        /// <summary>Appends narrow/wide elements starting with a bar (n = 1 module, w = <paramref name="wide"/> modules).</summary>
        public Pattern NarrowWide(string elements, int wide = 3)
        {
            var bar = true;
            foreach (var e in elements)
            {
                _modules.Append(bar ? '1' : '0', e == 'w' ? wide : 1);
                bar = !bar;
            }
            return this;
        }

        public BarcodeEncoding Build(string encoded, IReadOnlyList<BarcodeTextSegment> text)
        {
            var bits = _modules.ToString();
            var bars = new List<BarcodeBar>();
            for (var i = 0; i < bits.Length; i++)
            {
                if (bits[i] != '1') continue;
                var start = i;
                while (i + 1 < bits.Length && bits[i + 1] == '1') i++;
                var guard = _guards.Any(g => start >= g.Start && start < g.End);
                bars.Add(new BarcodeBar(start, i - start + 1, 0, guard));
            }
            return new BarcodeEncoding { Modules = bits.Length, Bars = bars, Text = text, EncodedValue = encoded };
        }
    }

    private static IReadOnlyList<BarcodeTextSegment> Centered(string text, int modules) => [new(text, modules / 2.0)];

    private static bool AllDigits(string s) => s.Length > 0 && s.All(char.IsAsciiDigit);

    // ── Code 11 ──────────────────────────────────────────────────────────────
    private const string Code11Chars = "0123456789-";
    private static readonly string[] Code11Patterns =
        ["nnnnw", "wnnnw", "nwnnw", "wwnnn", "nnwnw", "wnwnn", "nwwnn", "nnnww", "wnnwn", "wnnnn", "nnwnn"];
    private const string Code11StartStop = "nnwwn";

    private static BarcodeEncoding Code11(string value, bool checksum)
    {
        if (value.Any(c => !Code11Chars.Contains(c))) throw new ArgumentException("Code 11 accepts digits and '-' only.");

        var data = value;
        if (checksum)
        {
            data += Code11Check(data, 10);
            if (value.Length >= 10) data += Code11Check(data, 9);
        }

        var p = new Pattern().NarrowWide(Code11StartStop, 2).Modules("0");
        foreach (var c in data) p.NarrowWide(Code11Patterns[Code11Chars.IndexOf(c)], 2).Modules("0");
        p.NarrowWide(Code11StartStop, 2);
        return p.Build(data, Centered(value, p.Length));
    }

    private static char Code11Check(string data, int maxWeight)
    {
        var sum = 0;
        for (int i = data.Length - 1, weight = 1; i >= 0; i--, weight = weight % maxWeight + 1)
            sum += Code11Chars.IndexOf(data[i]) * weight;
        return Code11Chars[sum % 11];
    }

    // ── Code 39 ──────────────────────────────────────────────────────────────
    private const string Code39Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%";
    private static readonly string[] Code39Patterns =
    [
        "nnnwwnwnn", "wnnwnnnnw", "nnwwnnnnw", "wnwwnnnnn", "nnnwwnnnw", "wnnwwnnnn", "nnwwwnnnn", "nnnwnnwnw", "wnnwnnwnn", "nnwwnnwnn",
        "wnnnnwnnw", "nnwnnwnnw", "wnwnnwnnn", "nnnnwwnnw", "wnnnwwnnn", "nnwnwwnnn", "nnnnnwwnw", "wnnnnwwnn", "nnwnnwwnn", "nnnnwwwnn",
        "wnnnnnnww", "nnwnnnnww", "wnwnnnnwn", "nnnnwnnww", "wnnnwnnwn", "nnwnwnnwn", "nnnnnnwww", "wnnnnnwwn", "nnwnnnwwn", "nnnnwnwwn",
        "wwnnnnnnw", "nwwnnnnnw", "wwwnnnnnn", "nwnnwnnnw", "wwnnwnnnn", "nwwnwnnnn", "nwnnnnwnw", "wwnnnnwnn", "nwwnnnwnn",
        "nwnwnwnnn", "nwnwnnnwn", "nwnnnwnwn", "nnnwnwnwn",
    ];
    private const string Code39StartStop = "nwnnwnwnn";

    private static BarcodeEncoding Code39(string data, string display, bool checksum)
    {
        if (data.Any(c => !Code39Chars.Contains(c)))
            throw new ArgumentException("Code 39 accepts 0-9, A-Z, space and - . $ / + % (use Code39Extended for other characters).");

        if (checksum) data += Code39Chars[data.Sum(c => Code39Chars.IndexOf(c)) % 43];

        var p = new Pattern().NarrowWide(Code39StartStop).Modules("0");
        foreach (var c in data) p.NarrowWide(Code39Patterns[Code39Chars.IndexOf(c)]).Modules("0");
        p.NarrowWide(Code39StartStop);
        return p.Build(data, Centered(display, p.Length));
    }

    /// <summary>Full-ASCII mapping shared by Code 39 Extended and Code 93 Extended.</summary>
    private static string FullAscii(char c) => c switch
    {
        '\0'                       => "%U",
        >= '\u0001' and <= '\u001A' => "$" + (char)('A' + c - 1),
        >= '\u001B' and <= '\u001F' => "%" + (char)('A' + c - 0x1B),
        ' ' or '-' or '.'          => c.ToString(),
        >= '!' and <= ','          => "/" + (char)('A' + c - '!'),
        '/'                        => "/O",
        >= '0' and <= '9'          => c.ToString(),
        ':'                        => "/Z",
        >= ';' and <= '?'          => "%" + (char)('F' + c - ';'),
        '@'                        => "%V",
        >= 'A' and <= 'Z'          => c.ToString(),
        >= '[' and <= '_'          => "%" + (char)('K' + c - '['),
        '`'                        => "%W",
        >= 'a' and <= 'z'          => "+" + (char)('A' + c - 'a'),
        >= '{' and <= '\u007F'     => "%" + (char)('P' + c - '{'),
        _ => throw new ArgumentException($"Character '{c}' is not ASCII and cannot be encoded."),
    };

    private static string Code39FullAscii(string value) => string.Concat(value.Select(FullAscii));

    // ── Code 93 ──────────────────────────────────────────────────────────────
    private const string Code93Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ-. $/+%";
    private static readonly string[] Code93Patterns =
    [
        "131112", "111213", "111312", "111411", "121113", "121212", "121311", "111114", "131211", "141111",
        "211113", "211212", "211311", "221112", "221211", "231111", "112113", "112212", "112311", "122112",
        "132111", "111123", "111222", "111321", "121122", "131121", "212112", "212211", "211122", "211221",
        "221121", "222111", "112122", "112221", "122121", "123111", "121131", "311112", "311211", "321111",
        "112131", "113121", "211131",
        "121221", "312111", "311121", "122211", // shift symbols ($) (%) (/) (+) = values 43-46
    ];
    private const string Code93StartStop = "111141";

    private static BarcodeEncoding Code93(string value, bool extended)
    {
        var values = new List<int>();
        if (extended)
        {
            foreach (var c in value)
            {
                var mapped = FullAscii(c);
                if (mapped.Length == 2)
                {
                    values.Add(mapped[0] switch { '$' => 43, '%' => 44, '/' => 45, _ => 46 });
                    values.Add(Code93Chars.IndexOf(mapped[1]));
                }
                else values.Add(Code93Chars.IndexOf(mapped[0]));
            }
        }
        else
        {
            var upper = value.ToUpperInvariant();
            if (upper.Any(c => !Code93Chars.Contains(c)))
                throw new ArgumentException("Code 93 accepts 0-9, A-Z, space and - . $ / + % (use Code93Extended for other characters).");
            values.AddRange(upper.Select(c => Code93Chars.IndexOf(c)));
        }

        values.Add(Code93Check(values, 20));
        values.Add(Code93Check(values, 15));

        var p = new Pattern().Widths(Code93StartStop);
        foreach (var v in values) p.Widths(Code93Patterns[v]);
        p.Widths(Code93StartStop).Modules("1");
        return p.Build(value, Centered(value, p.Length));
    }

    private static int Code93Check(List<int> values, int maxWeight)
    {
        var sum = 0;
        for (int i = values.Count - 1, weight = 1; i >= 0; i--, weight = weight % maxWeight + 1)
            sum += values[i] * weight;
        return sum % 47;
    }

    // ── Code 128 / GS1-128 ───────────────────────────────────────────────────
    private enum Code128Set { Auto, A, B, C }

    private static readonly string[] Code128Patterns =
    [
        "212222", "222122", "222221", "121223", "121322", "131222", "122213", "122312", "132212", "221213",
        "221312", "231212", "112232", "122132", "122231", "113222", "123122", "123221", "223211", "221132",
        "221231", "213212", "223112", "312131", "311222", "321122", "321221", "312212", "322112", "322211",
        "212123", "212321", "232121", "111323", "131123", "131321", "112313", "132113", "132311", "211313",
        "231113", "231311", "112133", "112331", "132131", "113123", "113321", "133121", "313121", "211331",
        "231131", "213113", "213311", "213131", "311123", "311321", "331121", "312113", "312311", "332111",
        "314111", "221411", "431111", "111224", "111422", "121124", "121421", "141122", "141221", "112214",
        "112412", "122114", "122411", "142112", "142211", "241211", "221114", "413111", "241112", "134111",
        "111242", "121142", "121241", "114212", "124112", "124211", "411212", "421112", "421211", "212141",
        "214121", "412121", "111143", "111341", "131141", "114113", "114311", "411113", "411311", "113141",
        "114131", "311141", "411131", "211412", "211214", "211232",
    ];
    private const string Code128Stop = "2331112";
    private const int StartA = 103, StartB = 104, StartC = 105, CodeA = 101, CodeB = 100, CodeC = 99, Fnc1 = 102;
    private const char Fnc1Marker = 'ñ';

    private static BarcodeEncoding Code128(string value, Code128Set set) =>
        Code128Symbol(Code128Codes(value, set), value, value);

    private static BarcodeEncoding Code128Symbol(List<int> codes, string encoded, string display)
    {
        var p = new Pattern();
        foreach (var code in codes) p.Widths(Code128Patterns[code]);
        p.Widths(Code128Stop);
        return p.Build(encoded, Centered(display, p.Length));
    }

    private static List<int> Code128Codes(string data, Code128Set forced)
    {
        if (data.Any(c => c > 127 && c != Fnc1Marker)) throw new ArgumentException("Code 128 accepts ASCII characters only.");

        var codes = new List<int>();
        switch (forced)
        {
            case Code128Set.A:
                if (data.Any(c => c is > '_' and not Fnc1Marker)) throw new ArgumentException("Code 128A accepts control characters, digits, upper case and punctuation only.");
                codes.Add(StartA);
                codes.AddRange(data.Select(c => ValueInA(c)));
                break;
            case Code128Set.B:
                if (data.Any(c => c < ' ')) throw new ArgumentException("Code 128B does not accept control characters.");
                codes.Add(StartB);
                codes.AddRange(data.Select(c => ValueInB(c)));
                break;
            case Code128Set.C:
                if (!AllDigits(data) || data.Length % 2 != 0) throw new ArgumentException("Code 128C accepts an even number of digits only.");
                codes.Add(StartC);
                for (var i = 0; i < data.Length; i += 2) codes.Add(int.Parse(data.AsSpan(i, 2)));
                break;
            default:
                codes.AddRange(Code128Auto(data));
                break;
        }

        var checksum = codes[0];
        for (var i = 1; i < codes.Count; i++) checksum += codes[i] * i;
        codes.Add(checksum % 103);
        return codes;
    }

    private static int ValueInA(char c) => c == Fnc1Marker ? Fnc1 : c < ' ' ? c + 64 : c - ' ';
    private static int ValueInB(char c) => c == Fnc1Marker ? Fnc1 : c - ' ';

    private static int DigitRun(string data, int start)
    {
        var i = start;
        while (i < data.Length && char.IsAsciiDigit(data[i])) i++;
        return i - start;
    }

    /// <summary>Uses subset C for runs of 4+ digits (or an all-digit value), A for control characters, otherwise B.</summary>
    private static IEnumerable<int> Code128Auto(string data)
    {
        var codes = new List<int>();
        var set = Code128Set.Auto;
        var i = 0;

        var leadingFnc1 = data.Length > 0 && data[0] == Fnc1Marker;
        var firstData = leadingFnc1 ? 1 : 0;
        var run = DigitRun(data, firstData);
        if (run >= 4 || (run >= 2 && run == data.Length - firstData && run % 2 == 0)) { codes.Add(StartC); set = Code128Set.C; }
        else if (firstData < data.Length && data[firstData] < ' ') { codes.Add(StartA); set = Code128Set.A; }
        else { codes.Add(StartB); set = Code128Set.B; }

        while (i < data.Length)
        {
            var c = data[i];
            if (c == Fnc1Marker) { codes.Add(Fnc1); i++; continue; }

            if (set == Code128Set.C)
            {
                if (DigitRun(data, i) >= 2) { codes.Add(int.Parse(data.AsSpan(i, 2))); i += 2; continue; }
                set = c < ' ' ? Code128Set.A : Code128Set.B;
                codes.Add(set == Code128Set.A ? CodeA : CodeB);
                continue;
            }

            run = DigitRun(data, i);
            if (run >= 4)
            {
                // An odd run keeps its first digit in the current subset.
                if (run % 2 == 1) { codes.Add(set == Code128Set.A ? ValueInA(c) : ValueInB(c)); i++; }
                codes.Add(CodeC);
                set = Code128Set.C;
                continue;
            }

            if (set == Code128Set.B && c < ' ') { codes.Add(CodeA); set = Code128Set.A; }
            else if (set == Code128Set.A && c >= '`') { codes.Add(CodeB); set = Code128Set.B; }
            codes.Add(set == Code128Set.A ? ValueInA(c) : ValueInB(c));
            i++;
        }
        return codes;
    }

    /// <summary>
    /// GS1-128: application identifiers in parentheses, e.g. "(01)09501101530003(17)250101(10)ABC".
    /// FNC1 follows the start code and every variable-length field that is not last.
    /// </summary>
    private static BarcodeEncoding Gs1128(string value)
    {
        var data = new StringBuilder().Append(Fnc1Marker);
        var encoded = new StringBuilder();

        if (!value.StartsWith('('))
        {
            data.Append(value);
            encoded.Append(value);
        }
        else
        {
            var fields = new List<(string Ai, string Data)>();
            var i = 0;
            while (i < value.Length)
            {
                var close = value.IndexOf(')', i);
                if (value[i] != '(' || close < 0) throw new ArgumentException("GS1-128 values look like \"(01)09501101530003(10)ABC\".");
                var ai = value[(i + 1)..close];
                var next = value.IndexOf('(', close);
                var fieldData = next < 0 ? value[(close + 1)..] : value[(close + 1)..next];
                if (!AllDigits(ai) || ai.Length is < 2 or > 4) throw new ArgumentException($"\"{ai}\" is not a valid application identifier.");
                fields.Add((ai, fieldData));
                i = next < 0 ? value.Length : next;
            }

            for (var f = 0; f < fields.Count; f++)
            {
                var (ai, fieldData) = fields[f];
                data.Append(ai).Append(fieldData);
                encoded.Append(ai).Append(fieldData);
                if (f < fields.Count - 1 && !IsFixedLengthAi(ai)) data.Append(Fnc1Marker);
            }
        }

        return Code128Symbol(Code128Codes(data.ToString(), Code128Set.Auto), encoded.ToString(), value);
    }

    private static bool IsFixedLengthAi(string ai) =>
        int.Parse(ai.AsSpan(0, 2)) is (>= 0 and <= 4) or (>= 11 and <= 20) or 23 or (>= 31 and <= 36) or 41;

    // ── EAN / UPC ────────────────────────────────────────────────────────────
    private static readonly string[] EanL = ["0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011"];
    private static readonly string[] EanG = ["0100111", "0110011", "0011011", "0100001", "0011101", "0111001", "0000101", "0010001", "0001001", "0010111"];
    private static readonly string[] EanR = ["1110010", "1100110", "1101100", "1000010", "1011100", "1001110", "1010000", "1000100", "1001000", "1110100"];
    private static readonly string[] Ean13Parity = ["LLLLLL", "LLGLGG", "LLGGLG", "LLGGGL", "LGLLGG", "LGGLLG", "LGGGLL", "LGLGLG", "LGLGGL", "LGGLGL"];
    private static readonly string[] UpcEParity = ["GGGLLL", "GGLGLL", "GGLLGL", "GGLLLG", "GLGGLL", "GLLGGL", "GLLLGG", "GLGLGL", "GLGLLG", "GLLGLG"];
    private const int Quiet = 9;

    /// <summary>The GS1 mod-10 check digit (weights 3,1 from the right).</summary>
    public static int Mod10CheckDigit(string digits)
    {
        var sum = 0;
        for (var i = 0; i < digits.Length; i++)
            sum += (digits[digits.Length - 1 - i] - '0') * (i % 2 == 0 ? 3 : 1);
        return (10 - sum % 10) % 10;
    }

    /// <summary>Accepts the data digits alone (the check digit is added) or with a correct check digit.</summary>
    private static string WithCheckDigit(string value, int dataLength, string name)
    {
        if (!AllDigits(value) || (value.Length != dataLength && value.Length != dataLength + 1))
            throw new ArgumentException($"{name} needs {dataLength} digits (or {dataLength + 1} including the check digit).");
        var check = Mod10CheckDigit(value[..dataLength]);
        if (value.Length == dataLength + 1 && value[^1] - '0' != check)
            throw new ArgumentException($"Invalid {name} check digit; expected {check}.");
        return value[..dataLength] + check;
    }

    private static BarcodeEncoding Ean13(string value, bool upcA)
    {
        var digits = upcA ? "0" + WithCheckDigit(value, 11, "UPC-A") : WithCheckDigit(value, 12, "EAN-13");
        var parity = Ean13Parity[digits[0] - '0'];

        var p = new Pattern().Modules(new string('0', Quiet)).Modules("101", guard: true);
        for (var i = 1; i <= 6; i++) p.Modules((parity[i - 1] == 'L' ? EanL : EanG)[digits[i] - '0'], guard: upcA && i == 1);
        p.Modules("01010", guard: true);
        for (var i = 7; i <= 12; i++) p.Modules(EanR[digits[i] - '0'], guard: upcA && i == 12);
        p.Modules("101", guard: true).Modules(new string('0', Quiet));

        IReadOnlyList<BarcodeTextSegment> text = upcA
            ?
            [
                new(digits[1].ToString(), Quiet / 2.0),
                new(digits[2..7], Quiet + 3 + 7 + 17.5),
                new(digits[7..12], Quiet + 50 + 17.5),
                new(digits[12].ToString(), Quiet + 95 + Quiet / 2.0),
            ]
            :
            [
                new(digits[0].ToString(), Quiet / 2.0),
                new(digits[1..7], Quiet + 3 + 21),
                new(digits[7..13], Quiet + 50 + 21),
            ];
        return p.Build(upcA ? digits[1..] : digits, text);
    }

    private static BarcodeEncoding Ean8(string value)
    {
        var digits = WithCheckDigit(value, 7, "EAN-8");
        var p = new Pattern().Modules(new string('0', Quiet)).Modules("101", guard: true);
        for (var i = 0; i < 4; i++) p.Modules(EanL[digits[i] - '0']);
        p.Modules("01010", guard: true);
        for (var i = 4; i < 8; i++) p.Modules(EanR[digits[i] - '0']);
        p.Modules("101", guard: true).Modules(new string('0', Quiet));
        return p.Build(digits, [new(digits[..4], Quiet + 3 + 14), new(digits[4..], Quiet + 36 + 14)]);
    }

    /// <summary>UPC-E: 6 digits (number system 0), or number system + 6 digits, optionally + check digit.</summary>
    private static BarcodeEncoding UpcE(string value)
    {
        if (!AllDigits(value) || value.Length is < 6 or > 8) throw new ArgumentException("UPC-E needs 6 digits, or number system + 6 digits (+ check digit).");
        var numberSystem = value.Length >= 7 ? value[0] : '0';
        if (numberSystem is not ('0' or '1')) throw new ArgumentException("UPC-E number system must be 0 or 1.");
        var six = value.Length >= 7 ? value.Substring(1, 6) : value;

        var check = Mod10CheckDigit(ExpandUpcE(numberSystem, six));
        if (value.Length == 8 && value[7] - '0' != check) throw new ArgumentException($"Invalid UPC-E check digit; expected {check}.");

        var parity = UpcEParity[check];
        var p = new Pattern().Modules(new string('0', Quiet)).Modules("101", guard: true);
        for (var i = 0; i < 6; i++)
        {
            // Number system 1 uses the mirrored parity pattern.
            var even = (parity[i] == 'G') == (numberSystem == '0');
            p.Modules((even ? EanG : EanL)[six[i] - '0']);
        }
        p.Modules("010101", guard: true).Modules(new string('0', Quiet));

        return p.Build($"{numberSystem}{six}{check}",
        [
            new(numberSystem.ToString(), Quiet / 2.0),
            new(six, Quiet + 3 + 21),
            new(check.ToString(), Quiet + 51 + Quiet / 2.0),
        ]);
    }

    private static string ExpandUpcE(char ns, string d) => d[5] switch
    {
        '0' or '1' or '2' => $"{ns}{d[0]}{d[1]}{d[5]}0000{d[2]}{d[3]}{d[4]}",
        '3'               => $"{ns}{d[0]}{d[1]}{d[2]}00000{d[3]}{d[4]}",
        '4'               => $"{ns}{d[0]}{d[1]}{d[2]}{d[3]}00000{d[4]}",
        _                 => $"{ns}{d[0]}{d[1]}{d[2]}{d[3]}{d[4]}0000{d[5]}",
    };

    // ── MSI ──────────────────────────────────────────────────────────────────
    private static BarcodeEncoding Msi(string value, string scheme)
    {
        if (!AllDigits(value)) throw new ArgumentException("MSI accepts digits only.");

        var data = scheme switch
        {
            "10"   => value + MsiMod10(value),
            "11"   => value + MsiMod11(value),
            "1010" => AppendMod10(AppendMod10(value)),
            _      => AppendMod10(value + MsiMod11(value)),
        };

        var p = new Pattern().Modules("110");
        foreach (var c in data)
        {
            var bits = Convert.ToString(c - '0', 2).PadLeft(4, '0');
            foreach (var bit in bits) p.Modules(bit == '1' ? "110" : "100");
        }
        p.Modules("1001");
        return p.Build(data, Centered(data, p.Length));

        static string AppendMod10(string s) => s + MsiMod10(s);
    }

    /// <summary>Luhn check digit.</summary>
    private static int MsiMod10(string digits)
    {
        var sum = 0;
        for (var i = 0; i < digits.Length; i++)
        {
            var d = digits[digits.Length - 1 - i] - '0';
            if (i % 2 == 0) { d *= 2; if (d > 9) d -= 9; }
            sum += d;
        }
        return (10 - sum % 10) % 10;
    }

    /// <summary>IBM mod 11 (weights 2–7 from the right). A result of 10 is written as "10".</summary>
    private static string MsiMod11(string digits)
    {
        var sum = 0;
        for (var i = 0; i < digits.Length; i++) sum += (digits[digits.Length - 1 - i] - '0') * (i % 6 + 2);
        return ((11 - sum % 11) % 11).ToString();
    }

    // ── POSTNET ──────────────────────────────────────────────────────────────
    private static readonly string[] PostnetPatterns =
        ["11000", "00011", "00101", "00110", "01001", "01010", "01100", "10001", "10010", "10100"];

    private static BarcodeEncoding Postnet(string value)
    {
        var digits = new string(value.Where(c => c != '-' && c != ' ').ToArray());
        if (!AllDigits(digits) || digits.Length is not (5 or 9 or 11))
            throw new ArgumentException("POSTNET needs 5, 9 or 11 digits (ZIP, ZIP+4 or delivery point).");

        var check = (10 - digits.Sum(c => c - '0') % 10) % 10;
        var heights = "1" + string.Concat((digits + check).Select(c => PostnetPatterns[c - '0'])) + "1";

        const int barWidth = 2, step = 5;
        var bars = heights.Select((h, i) => new BarcodeBar(i * step, barWidth, h == '1' ? 0 : 0.6)).ToList();
        var modules = (heights.Length - 1) * step + barWidth;
        return new BarcodeEncoding
        {
            Modules = modules,
            Bars = bars,
            Text = Centered(value, modules),
            EncodedValue = digits + check,
        };
    }
}
