namespace MyStackBlazor.Components.Form;

/// <summary>Calendar levels, from days up to decades.</summary>
public enum CalendarView
{
    /// <summary>Days of a month.</summary>
    Month,
    /// <summary>Months of a year.</summary>
    Year,
    /// <summary>Years of a decade.</summary>
    Decade,
    /// <summary>Decades of a century.</summary>
    Century,
}

/// <summary>Arguments for <c>OnChange</c>: the confirmed range.</summary>
public class DateRangePickerChangeEventArgs
{
    public object? StartValue { get; init; }
    public object? EndValue { get; init; }
}

/// <summary>Arguments for <c>OnOpen</c>. Set <see cref="IsCancelled"/> to keep the popup closed.</summary>
public class DateRangePickerOpenEventArgs
{
    public bool IsCancelled { get; set; }
}

/// <summary>Arguments for <c>OnClose</c>. Set <see cref="IsCancelled"/> to keep the popup open.</summary>
public class DateRangePickerCloseEventArgs
{
    public bool IsCancelled { get; set; }
}

/// <summary>Arguments for <c>OnCalendarCellRender</c>. Set <see cref="Class"/> to style the cell.</summary>
public class DateRangePickerCalendarCellRenderEventArgs
{
    /// <summary>First day of the period the cell represents.</summary>
    public DateTime Date { get; init; }
    public CalendarView View { get; init; }
    public string? Class { get; set; }
}
