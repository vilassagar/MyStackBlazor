using System.Text;
using System.Text.RegularExpressions;

namespace MyStackBlazor.Components.Form;

/// <summary>
/// Parses a Telerik-style mask and fills its slots. Mask characters:
/// 0 digit · 9 digit or space · # digit, space, + or - · L letter · ? letter or space ·
/// &amp; any non-space · C any or space · A letter or digit · a letter, digit or space ·
/// * any (earlier API). Other characters are literals; \ escapes. Custom rules override/add characters.
/// </summary>
internal sealed class MaskEngine
{
    private readonly List<(bool IsSlot, char Rule, char Literal)> _positions = [];
    private readonly Dictionary<char, Regex> _custom = [];

    public MaskEngine(string? mask, IReadOnlyDictionary<char, string>? rules)
    {
        if (rules is not null)
            foreach (var (key, pattern) in rules)
                _custom[key] = new Regex($"^(?:{pattern})$", RegexOptions.CultureInvariant);

        var m = mask ?? "";
        for (var i = 0; i < m.Length; i++)
        {
            var c = m[i];
            if (c == '\\' && i + 1 < m.Length) { _positions.Add((false, '\0', m[++i])); continue; }
            if (IsRule(c)) _positions.Add((true, c, '\0'));
            else _positions.Add((false, '\0', c));
        }
    }

    public int Length => _positions.Count;
    public int SlotCount => _positions.Count(p => p.IsSlot);
    public bool IsEmptyMask => _positions.Count == 0;

    private bool IsRule(char c) => _custom.ContainsKey(c) || "09#L?&CAa*".IndexOf(c) >= 0;

    public bool Accepts(char rule, char c)
    {
        if (_custom.TryGetValue(rule, out var regex)) return regex.IsMatch(c.ToString());
        return rule switch
        {
            '0' => char.IsDigit(c),
            '9' => char.IsDigit(c) || c == ' ',
            '#' => char.IsDigit(c) || c is ' ' or '+' or '-',
            'L' => char.IsLetter(c),
            '?' => char.IsLetter(c) || c == ' ',
            '&' => !char.IsWhiteSpace(c),
            'C' => true,
            'A' => char.IsLetterOrDigit(c),
            'a' => char.IsLetterOrDigit(c) || c == ' ',
            '*' => true,
            _   => false,
        };
    }

    /// <summary>
    /// Fills slots in order from <paramref name="input"/>, skipping characters that match no slot.
    /// Literal characters and the prompt character in the input are ignored.
    /// </summary>
    public char?[] Fill(string input, char prompt, bool inputHasLiterals)
    {
        var slots = new char?[SlotCount];
        if (inputHasLiterals && input.Length == Length)
        {
            // Value with literals: read by position.
            for (int i = 0, s = 0; i < Length; i++)
            {
                if (!_positions[i].IsSlot) continue;
                var c = input[i];
                if (c != prompt && Accepts(_positions[i].Rule, c)) slots[s] = c;
                s++;
            }
            return slots;
        }

        var literals = _positions.Where(p => !p.IsSlot).Select(p => p.Literal).ToHashSet();
        var slot = 0;
        var slotRules = _positions.Where(p => p.IsSlot).Select(p => p.Rule).ToList();
        foreach (var c in input)
        {
            if (slot >= slots.Length) break;
            if (c == prompt) continue;
            if (Accepts(slotRules[slot], c)) { slots[slot++] = c; continue; }
            if (literals.Contains(c)) continue;
            // A character that fits a later slot (e.g. typing "a" into "000-LL") skips ahead.
            for (var next = slot + 1; next < slots.Length; next++)
            {
                if (!Accepts(slotRules[next], c)) continue;
                slots[next] = c;
                slot = next + 1;
                break;
            }
        }
        return slots;
    }

    /// <summary>The text with literals; empty slots show <paramref name="prompt"/>.</summary>
    public string Format(char?[] slots, char prompt)
    {
        var sb = new StringBuilder(Length);
        var s = 0;
        foreach (var p in _positions) sb.Append(p.IsSlot ? slots[s++] ?? prompt : p.Literal);
        return sb.ToString();
    }

    /// <summary>The value: typed characters only, or the full text with literals.</summary>
    public string Value(char?[] slots, bool includeLiterals, char prompt) =>
        includeLiterals ? Format(slots, prompt) : string.Concat(slots.Where(c => c is not null).Select(c => c!.Value));

    /// <summary>Position in the formatted text just after the <paramref name="filled"/>-th slot.</summary>
    public int CaretAfterSlot(int filled)
    {
        if (filled <= 0)
        {
            var first = _positions.FindIndex(p => p.IsSlot);
            return first < 0 ? 0 : first;
        }
        for (int i = 0, s = 0; i < Length; i++)
        {
            if (!_positions[i].IsSlot) continue;
            s++;
            if (s != filled) continue;
            // Skip literals that follow, so the caret lands on the next slot.
            var pos = i + 1;
            while (pos < Length && !_positions[pos].IsSlot) pos++;
            return pos;
        }
        return Length;
    }

    /// <summary>How many slot characters in <paramref name="text"/> lie before <paramref name="caret"/>.</summary>
    public int SlotsBefore(string text, int caret, char prompt)
    {
        var literals = _positions.Where(p => !p.IsSlot).Select(p => p.Literal).ToHashSet();
        var count = 0;
        for (var i = 0; i < Math.Min(caret, text.Length); i++)
            if (text[i] != prompt && !literals.Contains(text[i])) count++;
        return count;
    }
}
