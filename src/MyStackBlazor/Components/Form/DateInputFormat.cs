using System.Globalization;
using System.Text;

namespace MyStackBlazor.Components.Form;

/// <summary>What the empty segments of a date input show (Telerik <c>FormatPlaceholder</c>).</summary>
public enum DateInputFormatPlaceholder
{
    /// <summary>The format letters, e.g. "MM/dd/yyyy".</summary>
    Formatted,
    /// <summary>Single letters, e.g. "m/d/y".</summary>
    Short,
    /// <summary>Words, e.g. "month/day/year".</summary>
    Wide,
    /// <summary>Nothing.</summary>
    None,
}

/// <summary>An editable (or display-only) piece of a date format.</summary>
internal enum DatePartKind
{
    Literal,
    Day,
    Month,
    MonthName,
    Year,
    Hour24,
    Hour12,
    Minute,
    Second,
    AmPm,
    Weekday,
    Fraction,
}

internal sealed record DateFormatToken(DatePartKind Kind, string Text)
{
    public bool IsEditable => Kind is not (DatePartKind.Literal or DatePartKind.Weekday or DatePartKind.Fraction);

    public (int Min, int Max) Range => Kind switch
    {
        DatePartKind.Day                         => (1, 31),
        DatePartKind.Month or DatePartKind.MonthName => (1, 12),
        DatePartKind.Year                        => (1, 9999),
        DatePartKind.Hour24                      => (0, 23),
        DatePartKind.Hour12                      => (1, 12),
        DatePartKind.Minute or DatePartKind.Second => (0, 59),
        DatePartKind.AmPm                        => (0, 1),
        _                                        => (0, 0),
    };

    /// <summary>How many digits complete the part ("yy" = 2, "yyyy" = 4, others 2).</summary>
    public int MaxDigits => Kind == DatePartKind.Year ? (Text.Length <= 2 ? 2 : 4) : 2;
}

/// <summary>
/// Splits .NET date/time format strings into segments and holds the editable part values.
/// Kept free of rendering so the editing rules can be unit-tested.
/// </summary>
internal sealed class DateInputFormat
{
    public IReadOnlyList<DateFormatToken> Tokens { get; }
    public CultureInfo Culture { get; }

    public DateInputFormat(string? format, CultureInfo culture)
    {
        Culture = culture;
        Tokens = Tokenize(Expand(string.IsNullOrEmpty(format) ? "d" : format, culture.DateTimeFormat));
    }

    public bool HasDate => Tokens.Any(t => t.Kind is DatePartKind.Day or DatePartKind.Month or DatePartKind.MonthName or DatePartKind.Year);
    public bool HasTime => Tokens.Any(t => t.Kind is DatePartKind.Hour12 or DatePartKind.Hour24 or DatePartKind.Minute or DatePartKind.Second);

    /// <summary>Turns standard one-letter formats ("d", "g", "t"…) into their culture pattern.</summary>
    private static string Expand(string format, DateTimeFormatInfo dtf) => format switch
    {
        "d"        => dtf.ShortDatePattern,
        "D"        => dtf.LongDatePattern,
        "t"        => dtf.ShortTimePattern,
        "T"        => dtf.LongTimePattern,
        "g"        => dtf.ShortDatePattern + " " + dtf.ShortTimePattern,
        "G"        => dtf.ShortDatePattern + " " + dtf.LongTimePattern,
        "f"        => dtf.LongDatePattern + " " + dtf.ShortTimePattern,
        "F"        => dtf.FullDateTimePattern,
        "M" or "m" => dtf.MonthDayPattern,
        "Y" or "y" => dtf.YearMonthPattern,
        "s"        => dtf.SortableDateTimePattern,
        _          => format,
    };

    private static List<DateFormatToken> Tokenize(string pattern)
    {
        var tokens = new List<DateFormatToken>();
        var literal = new StringBuilder();

        void FlushLiteral()
        {
            if (literal.Length == 0) return;
            tokens.Add(new DateFormatToken(DatePartKind.Literal, literal.ToString()));
            literal.Clear();
        }

        for (var i = 0; i < pattern.Length; i++)
        {
            var c = pattern[i];
            if (c is '\'' or '"')
            {
                var end = pattern.IndexOf(c, i + 1);
                if (end < 0) end = pattern.Length;
                literal.Append(pattern, i + 1, Math.Max(0, end - i - 1));
                i = end;
                continue;
            }
            if (c == '\\' && i + 1 < pattern.Length)
            {
                literal.Append(pattern[++i]);
                continue;
            }
            if ("dMyHhmstf".IndexOf(c) < 0)
            {
                literal.Append(c);
                continue;
            }

            var run = 1;
            while (i + run < pattern.Length && pattern[i + run] == c) run++;
            var text = pattern.Substring(i, run);
            i += run - 1;

            var kind = c switch
            {
                'd' => run >= 3 ? DatePartKind.Weekday : DatePartKind.Day,
                'M' => run >= 3 ? DatePartKind.MonthName : DatePartKind.Month,
                'y' => DatePartKind.Year,
                'H' => DatePartKind.Hour24,
                'h' => DatePartKind.Hour12,
                'm' => DatePartKind.Minute,
                's' => DatePartKind.Second,
                't' => DatePartKind.AmPm,
                _   => DatePartKind.Fraction,
            };
            FlushLiteral();
            tokens.Add(new DateFormatToken(kind, text));
        }
        FlushLiteral();
        return tokens;
    }

    // ── Part values ──────────────────────────────────────────────────────────
    /// <summary>Current value of each editable token (by token index); null = empty.</summary>
    public Dictionary<int, int?> Values { get; } = [];

    public void Load(DateTime? date)
    {
        Values.Clear();
        for (var i = 0; i < Tokens.Count; i++)
        {
            if (!Tokens[i].IsEditable) continue;
            Values[i] = date is DateTime d ? Read(Tokens[i], d) : null;
        }
    }

    private static int Read(DateFormatToken token, DateTime d) => token.Kind switch
    {
        DatePartKind.Day                             => d.Day,
        DatePartKind.Month or DatePartKind.MonthName => d.Month,
        DatePartKind.Year                            => d.Year,
        DatePartKind.Hour24                          => d.Hour,
        DatePartKind.Hour12                          => d.Hour % 12 == 0 ? 12 : d.Hour % 12,
        DatePartKind.Minute                          => d.Minute,
        DatePartKind.Second                          => d.Second,
        DatePartKind.AmPm                            => d.Hour >= 12 ? 1 : 0,
        _                                            => 0,
    };

    public bool IsComplete => Values.Values.All(v => v is not null);
    public bool IsEmpty => Values.Values.All(v => v is null);

    private int? ValueOf(params DatePartKind[] kinds)
    {
        foreach (var (index, value) in Values)
            if (kinds.Contains(Tokens[index].Kind)) return value;
        return null;
    }

    /// <summary>
    /// Builds the date from the parts. Parts missing from the format come from <paramref name="fallback"/>.
    /// Returns null when a part is empty or the combination is not a real date (and auto-correct is off).
    /// </summary>
    public DateTime? Build(DateTime fallback, int twoDigitYearMax, bool autoCorrect)
    {
        if (!IsComplete) return null;

        var year = ValueOf(DatePartKind.Year) ?? fallback.Year;
        var yearToken = Tokens.FirstOrDefault(t => t.Kind == DatePartKind.Year);
        if (yearToken is { MaxDigits: 2 } && year < 100)
            year += year <= twoDigitYearMax % 100 ? 2000 : 1900;
        var month = ValueOf(DatePartKind.Month, DatePartKind.MonthName) ?? fallback.Month;
        var day = ValueOf(DatePartKind.Day) ?? (HasDate ? 1 : fallback.Day);

        int hour;
        if (ValueOf(DatePartKind.Hour24) is int h24) hour = h24;
        else if (ValueOf(DatePartKind.Hour12) is int h12) hour = h12 % 12 + (ValueOf(DatePartKind.AmPm) == 1 ? 12 : 0);
        else hour = HasTime ? 0 : fallback.Hour;
        var minute = ValueOf(DatePartKind.Minute) ?? (HasTime ? 0 : fallback.Minute);
        var second = ValueOf(DatePartKind.Second) ?? (HasTime ? 0 : fallback.Second);

        year = Math.Clamp(year, 1, 9999);
        var daysInMonth = DateTime.DaysInMonth(year, month);
        if (day > daysInMonth)
        {
            if (!autoCorrect) return null;
            day = daysInMonth;
        }
        return new DateTime(year, month, day, hour, minute, second);
    }

    // ── Display ──────────────────────────────────────────────────────────────
    public string Display(int index, DateInputFormatPlaceholder placeholder, DateTime? built)
    {
        var token = Tokens[index];
        if (token.Kind == DatePartKind.Literal) return token.Text;
        if (token.Kind == DatePartKind.Weekday) return built?.ToString(token.Text.Length == 3 ? "ddd" : "dddd", Culture) ?? "";
        if (token.Kind == DatePartKind.Fraction) return built?.ToString(token.Text.Length == 1 ? "%f" : token.Text, Culture) ?? new string('0', token.Text.Length);

        var value = Values.GetValueOrDefault(index);
        if (value is null) return Placeholder(token, placeholder);

        var v = value.Value;
        return token.Kind switch
        {
            DatePartKind.MonthName => token.Text.Length == 3
                ? Culture.DateTimeFormat.AbbreviatedMonthNames[v - 1]
                : Culture.DateTimeFormat.MonthNames[v - 1],
            DatePartKind.AmPm => v == 1 ? Culture.DateTimeFormat.PMDesignator : Culture.DateTimeFormat.AMDesignator,
            DatePartKind.Year => token.Text.Length == 2 ? (v % 100).ToString("00", Culture) : v.ToString(new string('0', Math.Min(token.Text.Length, 4)), Culture),
            _ => token.Text.Length >= 2 ? v.ToString("00", Culture) : v.ToString(Culture),
        };
    }

    private static string Placeholder(DateFormatToken token, DateInputFormatPlaceholder placeholder) => placeholder switch
    {
        DateInputFormatPlaceholder.None => " ",
        DateInputFormatPlaceholder.Short => token.Kind switch
        {
            DatePartKind.Day                             => "d",
            DatePartKind.Month or DatePartKind.MonthName => "m",
            DatePartKind.Year                            => "y",
            DatePartKind.Hour12 or DatePartKind.Hour24   => "h",
            DatePartKind.Minute                          => "m",
            DatePartKind.Second                          => "s",
            _                                            => "--",
        },
        DateInputFormatPlaceholder.Wide => token.Kind switch
        {
            DatePartKind.Day                             => "day",
            DatePartKind.Month or DatePartKind.MonthName => "month",
            DatePartKind.Year                            => "year",
            DatePartKind.Hour12 or DatePartKind.Hour24   => "hour",
            DatePartKind.Minute                          => "minute",
            DatePartKind.Second                          => "second",
            _                                            => "AM/PM",
        },
        _ => token.Text,
    };

    public static string PartName(DatePartKind kind) => kind switch
    {
        DatePartKind.Day                             => "day",
        DatePartKind.Month or DatePartKind.MonthName => "month",
        DatePartKind.Year                            => "year",
        DatePartKind.Hour12 or DatePartKind.Hour24   => "hour",
        DatePartKind.Minute                          => "minute",
        DatePartKind.Second                          => "second",
        DatePartKind.AmPm                            => "AM/PM",
        _                                            => "",
    };

    // ── Editing ──────────────────────────────────────────────────────────────
    /// <summary>
    /// Applies one typed character to the part at <paramref name="index"/>.
    /// <paramref name="buffer"/> holds what was typed into this part so far.
    /// Returns true when the part is complete (no more characters fit).
    /// </summary>
    public bool Type(int index, char key, ref string buffer)
    {
        var token = Tokens[index];
        var (min, max) = token.Range;

        if (token.Kind == DatePartKind.AmPm)
        {
            var am = Culture.DateTimeFormat.AMDesignator;
            var pm = Culture.DateTimeFormat.PMDesignator;
            if (am.Length > 0 && char.ToLowerInvariant(am[0]) == char.ToLowerInvariant(key)) Values[index] = 0;
            else if (pm.Length > 0 && char.ToLowerInvariant(pm[0]) == char.ToLowerInvariant(key)) Values[index] = 1;
            else return false;
            buffer = "";
            return true;
        }

        if (token.Kind == DatePartKind.MonthName && char.IsLetter(key))
        {
            var candidate = buffer + key;
            var match = FindMonth(candidate) ?? FindMonth(key.ToString());
            if (match is null) return false;
            var typedLetters = FindMonth(candidate) is not null ? candidate : key.ToString();
            buffer = typedLetters;
            Values[index] = match;
            // Complete once only one month starts with the typed letters.
            return Enumerable.Range(1, 12).Count(m => MonthStarts(m, typedLetters)) == 1;
        }

        if (!char.IsAsciiDigit(key)) return false;

        var typed = buffer + key;
        var value = int.Parse(typed, CultureInfo.InvariantCulture);
        if (value > max || typed.Length > token.MaxDigits)
        {
            typed = key.ToString();
            value = key - '0';
        }
        buffer = typed;
        var complete = typed.Length >= token.MaxDigits || value * 10 > max;

        // A leading "0" for a part that starts at 1 (month, day) waits for the next digit;
        // a finished part below its minimum ("00" month) becomes the minimum.
        Values[index] = value >= min ? value : complete ? min : null;
        if (complete) buffer = "";
        return complete;
    }

    private bool MonthStarts(int month, string text) =>
        Culture.DateTimeFormat.MonthNames[month - 1].StartsWith(text, StringComparison.CurrentCultureIgnoreCase)
        || Culture.DateTimeFormat.AbbreviatedMonthNames[month - 1].StartsWith(text, StringComparison.CurrentCultureIgnoreCase);

    private int? FindMonth(string text)
    {
        for (var m = 1; m <= 12; m++) if (MonthStarts(m, text)) return m;
        return null;
    }

    /// <summary>Arrow up/down: steps the part, wrapping (except years). Empty parts start from <paramref name="reference"/>.</summary>
    public void Step(int index, int delta, DateTime reference)
    {
        var token = Tokens[index];
        var (min, max) = token.Range;
        var current = Values.GetValueOrDefault(index);
        if (current is null)
        {
            Values[index] = Read(token, reference);
            return;
        }
        var next = current.Value + delta;
        if (token.Kind == DatePartKind.Year) next = Math.Clamp(next, min, max);
        else if (next > max) next = min;
        else if (next < min) next = max;
        Values[index] = next;
    }

    /// <summary>Clamps the day to the month length after month/year edits (Telerik AutoCorrectParts).</summary>
    public void CorrectDay(DateTime fallback)
    {
        var dayIndex = Values.Keys.FirstOrDefault(i => Tokens[i].Kind == DatePartKind.Day, -1);
        if (dayIndex < 0 || Values[dayIndex] is not int day) return;
        var month = ValueOf(DatePartKind.Month, DatePartKind.MonthName) ?? fallback.Month;
        var year = ValueOf(DatePartKind.Year) ?? fallback.Year;
        if (year < 1) return;
        var days = DateTime.DaysInMonth(Math.Clamp(year, 1, 9999), month);
        if (day > days) Values[dayIndex] = days;
    }
}
