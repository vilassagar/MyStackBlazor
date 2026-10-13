namespace MyStackBlazor.Components.Form;

/// <summary>
/// Converts the generic value type of the date components (DateTime, DateOnly, TimeOnly and their
/// nullable forms) to and from DateTime.
/// </summary>
internal static class DateValue<T>
{
    private static readonly Type Underlying = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

    public static bool IsNullable => !typeof(T).IsValueType || Nullable.GetUnderlyingType(typeof(T)) is not null;
    /// <summary>TimeOnly and TimeSpan carry no date.</summary>
    public static bool IsTimeOnly => Underlying == typeof(TimeOnly) || Underlying == typeof(TimeSpan);
    public static bool IsDateOnly => Underlying == typeof(DateOnly);

    /// <summary>Null for null and for the default DateTime/DateOnly (treated as "no value").</summary>
    public static DateTime? ToDateTime(T? value) => (object?)value switch
    {
        DateTime d when d == default => null,
        DateTime d                   => d,
        DateOnly d when d == default => null,
        DateOnly d                   => d.ToDateTime(TimeOnly.MinValue),
        TimeOnly t                   => DateTime.MinValue.Date + t.ToTimeSpan(),
        TimeSpan t                   => DateTime.MinValue.Date + t,
        DateTimeOffset o             => o.DateTime,
        _                            => null,
    };

    public static T? FromDateTime(DateTime? date)
    {
        if (date is null) return default;
        // Each branch is boxed explicitly: in a ?: chain DateTime would silently convert to DateTimeOffset.
        object value;
        if (Underlying == typeof(DateOnly)) value = DateOnly.FromDateTime(date.Value);
        else if (Underlying == typeof(TimeOnly)) value = TimeOnly.FromDateTime(date.Value);
        else if (Underlying == typeof(TimeSpan)) value = date.Value.TimeOfDay;
        else if (Underlying == typeof(DateTimeOffset)) value = new DateTimeOffset(date.Value);
        else value = date.Value;
        return (T)value;
    }
}
