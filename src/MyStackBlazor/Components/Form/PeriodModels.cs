using System.Globalization;

namespace MyStackBlazor.Components.Form;

/// <summary>The granularity of a <see cref="DatePeriod"/>.</summary>
public enum PeriodType
{
    Day,
    Week,
    Month,
    Quarter,
    Year,
    /// <summary>Arbitrary start/end chosen by the user.</summary>
    Custom,
}

/// <summary>
/// A calendar period such as "week of 19 Oct 2026", "Q4 2026" or "FY2027".
/// <see cref="Start"/> and <see cref="End"/> are dates (midnight) and both inclusive;
/// use <see cref="EndExclusive"/> for <c>date &lt; end</c> queries.
/// </summary>
public readonly record struct DatePeriod(DateTime Start, DateTime End, PeriodType Type)
{
    /// <summary>The day after <see cref="End"/> — convenient for half-open range queries.</summary>
    public DateTime EndExclusive => End.AddDays(1);

    /// <summary>Number of days in the period, inclusive.</summary>
    public int Days => (End - Start).Days + 1;

    public bool Contains(DateTime date) => date.Date >= Start && date.Date <= End;

    /// <summary>
    /// The period of <paramref name="type"/> that contains <paramref name="anchor"/>.
    /// </summary>
    /// <param name="firstDayOfWeek">First day of a week period.</param>
    /// <param name="fiscalYearStartMonth">1–12. Quarters and years start from this month (e.g. 4 = April–March fiscal year).</param>
    public static DatePeriod Create(PeriodType type, DateTime anchor, DayOfWeek firstDayOfWeek = DayOfWeek.Monday, int fiscalYearStartMonth = 1)
    {
        if (fiscalYearStartMonth is < 1 or > 12) throw new ArgumentOutOfRangeException(nameof(fiscalYearStartMonth));
        var d = anchor.Date;
        switch (type)
        {
            case PeriodType.Day:
                return new(d, d, type);
            case PeriodType.Week:
            {
                var diff = ((int)d.DayOfWeek - (int)firstDayOfWeek + 7) % 7;
                var start = d.AddDays(-diff);
                return new(start, start.AddDays(6), type);
            }
            case PeriodType.Month:
            {
                var start = new DateTime(d.Year, d.Month, 1);
                return new(start, start.AddMonths(1).AddDays(-1), type);
            }
            case PeriodType.Quarter:
            {
                var monthsIntoYear = (d.Month - fiscalYearStartMonth + 12) % 12;
                var start = new DateTime(d.Year, d.Month, 1).AddMonths(-(monthsIntoYear % 3));
                return new(start, start.AddMonths(3).AddDays(-1), type);
            }
            case PeriodType.Year:
            {
                var monthsIntoYear = (d.Month - fiscalYearStartMonth + 12) % 12;
                var start = new DateTime(d.Year, d.Month, 1).AddMonths(-monthsIntoYear);
                return new(start, start.AddYears(1).AddDays(-1), type);
            }
            default:
                return new(d, d, PeriodType.Custom);
        }
    }

    /// <summary>A custom period; the dates are swapped if given in reverse.</summary>
    public static DatePeriod Custom(DateTime start, DateTime end) =>
        start.Date <= end.Date ? new(start.Date, end.Date, PeriodType.Custom) : new(end.Date, start.Date, PeriodType.Custom);

    /// <summary>The period <paramref name="count"/> steps later (negative = earlier). Custom periods move by their own length.</summary>
    public DatePeriod Shift(int count) => Type switch
    {
        PeriodType.Day     => new(Start.AddDays(count), End.AddDays(count), Type),
        PeriodType.Week    => new(Start.AddDays(7 * count), End.AddDays(7 * count), Type),
        PeriodType.Month   => FromStart(Start.AddMonths(count), 1),
        PeriodType.Quarter => FromStart(Start.AddMonths(3 * count), 3),
        PeriodType.Year    => FromStart(Start.AddYears(count), 12),
        _                  => new(Start.AddDays(Days * count), End.AddDays(Days * count), Type),
    };

    private DatePeriod FromStart(DateTime start, int months) => new(start, start.AddMonths(months).AddDays(-1), Type);

    /// <summary>
    /// Quarter number 1–4 relative to the fiscal year that starts in <paramref name="fiscalYearStartMonth"/>.
    /// </summary>
    public int GetQuarter(int fiscalYearStartMonth = 1) => (Start.Month - fiscalYearStartMonth + 12) % 12 / 3 + 1;

    /// <summary>
    /// Fiscal year number. With a January start it is the calendar year; otherwise the year in which the fiscal year ends
    /// (April 2026 – March 2027 → 2027).
    /// </summary>
    public int GetFiscalYear(int fiscalYearStartMonth = 1) =>
        fiscalYearStartMonth == 1 ? Start.Year : (Start.Month >= fiscalYearStartMonth ? Start.Year + 1 : Start.Year);

    /// <summary>Short display text, e.g. "October 19 – October 25, 2026" (week, en-US), "October 2026", "Q4 2026", "FY2027".</summary>
    public string ToDisplayString(CultureInfo? culture = null, int fiscalYearStartMonth = 1)
    {
        var c = culture ?? CultureInfo.CurrentCulture;
        var fiscal = fiscalYearStartMonth != 1;
        return Type switch
        {
            PeriodType.Day     => Start.ToString("D", c),
            PeriodType.Month   => Start.ToString(c.DateTimeFormat.YearMonthPattern, c),
            PeriodType.Quarter => fiscal
                ? $"Q{GetQuarter(fiscalYearStartMonth)} FY{GetFiscalYear(fiscalYearStartMonth)}"
                : $"Q{GetQuarter()} {Start.Year}",
            PeriodType.Year    => fiscal ? $"FY{GetFiscalYear(fiscalYearStartMonth)}" : Start.Year.ToString(c),
            _                  => FormatRange(c),
        };
    }

    private string FormatRange(CultureInfo c)
    {
        var md = c.DateTimeFormat.MonthDayPattern;
        return Start.Year == End.Year
            ? $"{Start.ToString(md, c)} – {End.ToString(md, c)}, {End.Year}"
            : $"{Start.ToString("d", c)} – {End.ToString("d", c)}";
    }

    public override string ToString() => ToDisplayString(CultureInfo.InvariantCulture);
}

/// <summary>A named quick-pick such as "Last month". <see cref="Resolve"/> receives today's date.</summary>
public sealed record PeriodPreset(string Label, Func<DateTime, DatePeriod> Resolve)
{
    /// <summary>Today, This/Last week, This/Last month, This/Last quarter, This/Last year, Year to date.</summary>
    public static IReadOnlyList<PeriodPreset> Defaults(DayOfWeek firstDayOfWeek = DayOfWeek.Monday, int fiscalYearStartMonth = 1)
    {
        DatePeriod P(PeriodType t, DateTime d) => DatePeriod.Create(t, d, firstDayOfWeek, fiscalYearStartMonth);
        return
        [
            new("Today",        t => P(PeriodType.Day, t)),
            new("This week",    t => P(PeriodType.Week, t)),
            new("Last week",    t => P(PeriodType.Week, t).Shift(-1)),
            new("This month",   t => P(PeriodType.Month, t)),
            new("Last month",   t => P(PeriodType.Month, t).Shift(-1)),
            new("This quarter", t => P(PeriodType.Quarter, t)),
            new("Last quarter", t => P(PeriodType.Quarter, t).Shift(-1)),
            new("This year",    t => P(PeriodType.Year, t)),
            new("Last year",    t => P(PeriodType.Year, t).Shift(-1)),
            new("Year to date", t => DatePeriod.Custom(P(PeriodType.Year, t).Start, t)),
        ];
    }
}
