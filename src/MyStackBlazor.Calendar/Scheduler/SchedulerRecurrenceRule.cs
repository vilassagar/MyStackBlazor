using System.Globalization;
using System.Text;

namespace MyStackBlazor.Calendar;

/// <summary>
/// A subset of the iCalendar RRULE format used by the scheduler's RecurrenceRuleField:
/// FREQ (DAILY/WEEKLY/MONTHLY/YEARLY), INTERVAL, COUNT, UNTIL, BYDAY (weekly) and BYMONTHDAY (monthly).
/// Example: "FREQ=WEEKLY;INTERVAL=2;BYDAY=MO,WE;COUNT=10".
/// </summary>
public sealed class SchedulerRecurrenceRule
{
    private const int MaxIterations = 20_000;

    private static readonly (string Code, DayOfWeek Day)[] DayCodes =
    [
        ("MO", DayOfWeek.Monday), ("TU", DayOfWeek.Tuesday), ("WE", DayOfWeek.Wednesday),
        ("TH", DayOfWeek.Thursday), ("FR", DayOfWeek.Friday), ("SA", DayOfWeek.Saturday), ("SU", DayOfWeek.Sunday),
    ];

    public RecurrenceFrequency Frequency { get; set; } = RecurrenceFrequency.Daily;
    public int Interval { get; set; } = 1;
    public int? Count { get; set; }
    public DateTime? Until { get; set; }
    /// <summary>Days of the week for weekly rules. Empty means the series start's weekday.</summary>
    public List<DayOfWeek> ByDay { get; set; } = [];
    /// <summary>Day of the month for monthly rules. Null means the series start's day.</summary>
    public int? ByMonthDay { get; set; }

    /// <summary>Parses an RRULE string. Returns null for null/empty or unsupported rules.</summary>
    public static SchedulerRecurrenceRule? Parse(string? rule)
    {
        if (string.IsNullOrWhiteSpace(rule)) return null;
        var text = rule.Trim();
        if (text.StartsWith("RRULE:", StringComparison.OrdinalIgnoreCase)) text = text[6..];

        var result = new SchedulerRecurrenceRule();
        var hasFrequency = false;
        foreach (var part in text.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var pair = part.Split('=', 2);
            if (pair.Length != 2) continue;
            var value = pair[1].Trim();
            switch (pair[0].Trim().ToUpperInvariant())
            {
                case "FREQ":
                    if (!Enum.TryParse<RecurrenceFrequency>(value, ignoreCase: true, out var freq)) return null;
                    result.Frequency = freq;
                    hasFrequency = true;
                    break;
                case "INTERVAL":
                    if (int.TryParse(value, out var interval) && interval > 0) result.Interval = interval;
                    break;
                case "COUNT":
                    if (int.TryParse(value, out var count) && count > 0) result.Count = count;
                    break;
                case "UNTIL":
                    result.Until = ParseUntil(value);
                    break;
                case "BYDAY":
                    foreach (var code in value.Split(',', StringSplitOptions.RemoveEmptyEntries))
                    {
                        var trimmed = code.Trim().ToUpperInvariant();
                        var match = DayCodes.FirstOrDefault(d => trimmed.EndsWith(d.Code, StringComparison.Ordinal));
                        if (match.Code is not null && !result.ByDay.Contains(match.Day)) result.ByDay.Add(match.Day);
                    }
                    break;
                case "BYMONTHDAY":
                    if (int.TryParse(value.Split(',')[0], out var monthDay) && monthDay is >= 1 and <= 31) result.ByMonthDay = monthDay;
                    break;
            }
        }
        return hasFrequency ? result : null;
    }

    private static DateTime? ParseUntil(string value)
    {
        string[] formats = ["yyyyMMdd'T'HHmmss'Z'", "yyyyMMdd'T'HHmmss", "yyyyMMdd"];
        return DateTime.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var until)
            ? until
            : null;
    }

    public override string ToString()
    {
        var sb = new StringBuilder($"FREQ={Frequency.ToString().ToUpperInvariant()}");
        if (Interval > 1) sb.Append($";INTERVAL={Interval}");
        if (Frequency == RecurrenceFrequency.Weekly && ByDay.Count > 0)
            sb.Append(";BYDAY=").Append(string.Join(",", ByDay.Select(d => DayCodes.First(c => c.Day == d).Code)));
        if (Frequency == RecurrenceFrequency.Monthly && ByMonthDay is int day)
            sb.Append($";BYMONTHDAY={day}");
        if (Count is int count) sb.Append($";COUNT={count}");
        else if (Until is DateTime until) sb.Append($";UNTIL={until.ToString("yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture)}");
        return sb.ToString();
    }

    /// <summary>
    /// Start times of the occurrences that overlap [rangeStart, rangeEnd), for a series that starts at
    /// <paramref name="seriesStart"/> and whose occurrences last <paramref name="duration"/>.
    /// </summary>
    public IEnumerable<DateTime> GetOccurrences(DateTime seriesStart, TimeSpan duration, DateTime rangeStart, DateTime rangeEnd)
    {
        var produced = 0;
        foreach (var candidate in Candidates(seriesStart).Take(MaxIterations))
        {
            if (candidate < seriesStart) continue;
            produced++;
            if (Count is int count && produced > count) yield break;
            if (Until is DateTime until && candidate > until) yield break;
            if (candidate >= rangeEnd) yield break;
            if (candidate + duration > rangeStart || (duration == TimeSpan.Zero && candidate >= rangeStart))
                yield return candidate;
        }
    }

    private IEnumerable<DateTime> Candidates(DateTime start)
    {
        var time = start.TimeOfDay;
        switch (Frequency)
        {
            case RecurrenceFrequency.Daily:
                for (var d = start; ; d = d.AddDays(Interval)) yield return d;

            case RecurrenceFrequency.Weekly:
            {
                var days = (ByDay.Count > 0 ? ByDay : [start.DayOfWeek])
                    .Select(d => ((int)d + 6) % 7) // Monday-based offset (RRULE default WKST=MO)
                    .Distinct().Order().ToList();
                var weekStart = start.Date.AddDays(-(((int)start.DayOfWeek + 6) % 7));
                for (var week = weekStart; ; week = week.AddDays(7 * Interval))
                    foreach (var offset in days)
                        yield return week.AddDays(offset) + time;
            }

            case RecurrenceFrequency.Monthly:
            {
                var day = ByMonthDay ?? start.Day;
                for (var month = new DateTime(start.Year, start.Month, 1); ; month = month.AddMonths(Interval))
                    if (day <= DateTime.DaysInMonth(month.Year, month.Month))
                        yield return month.AddDays(day - 1) + time;
            }

            default:
                for (var year = start.Year; year <= 9998; year += Interval)
                    if (start.Day <= DateTime.DaysInMonth(year, start.Month))
                        yield return new DateTime(year, start.Month, start.Day) + time;
                yield break;
        }
    }
}
