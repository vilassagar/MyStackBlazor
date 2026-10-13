namespace MyStackBlazor.Components.Form;

/// <summary>How dates are selected in a <see cref="StackDateCalendar"/>.</summary>
public enum CalendarSelectionMode
{
    /// <summary>One date (<c>Value</c>).</summary>
    Single,
    /// <summary>Any set of dates (<c>SelectedDates</c>); Ctrl+click toggles, Shift+click selects a span.</summary>
    Multiple,
    /// <summary>A start and end date (<c>RangeStart</c> / <c>RangeEnd</c>).</summary>
    Range,
}

/// <summary>How several calendar views (<c>Views</c> &gt; 1) are arranged.</summary>
public enum CalendarOrientation
{
    Horizontal,
    Vertical,
}

/// <summary>Arguments for <c>OnCellRender</c>. Set <see cref="Class"/> to style the cell.</summary>
public class CalendarCellRenderEventArgs
{
    /// <summary>First day of the period the cell represents.</summary>
    public DateTime Date { get; init; }
    public CalendarView View { get; init; }
    public string? Class { get; set; }
}

/// <summary>Calendar arithmetic shared by the calendar-based pickers.</summary>
internal static class CalendarMath
{
    public static DateTime PeriodStart(DateTime date, CalendarView view) => view switch
    {
        CalendarView.Year    => new DateTime(date.Year, 1, 1),
        CalendarView.Decade  => new DateTime(date.Year - date.Year % 10, 1, 1),
        CalendarView.Century => new DateTime(Math.Max(1, date.Year - date.Year % 100), 1, 1),
        _                    => new DateTime(date.Year, date.Month, 1),
    };

    /// <summary>Last day of the period one cell of <paramref name="view"/> covers.</summary>
    public static DateTime CellEnd(DateTime cellStart, CalendarView view) => view switch
    {
        CalendarView.Year    => cellStart.AddMonths(1).AddDays(-1),
        CalendarView.Decade  => cellStart.AddYears(1).AddDays(-1),
        CalendarView.Century => cellStart.AddYears(10).AddDays(-1),
        _                    => cellStart,
    };

    /// <summary>Moves by whole panels (one month in Month view, one year in Year view, …).</summary>
    public static DateTime Step(DateTime date, CalendarView view, int count) => view switch
    {
        CalendarView.Year    => date.AddYears(count),
        CalendarView.Decade  => date.AddYears(10 * count),
        CalendarView.Century => date.AddYears(100 * count),
        _                    => date.AddMonths(count),
    };

    public static DateTime Clamp(DateTime date, DateTime min, DateTime max) =>
        date < min.Date ? min.Date : date > max.Date ? max.Date : date;
}
