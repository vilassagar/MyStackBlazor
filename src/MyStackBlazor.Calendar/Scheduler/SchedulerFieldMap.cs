using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;

namespace MyStackBlazor.Calendar;

/// <summary>Reads and writes item properties by name, as configured by the scheduler's *Field parameters.</summary>
internal static class SchedulerFieldMap<TItem>
{
    private static readonly ConcurrentDictionary<string, PropertyInfo?> Cache = new();

    private static PropertyInfo? Property(string? name) =>
        string.IsNullOrEmpty(name)
            ? null
            : Cache.GetOrAdd(name, n => typeof(TItem).GetProperty(n, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));

    public static bool Has(string? field) => Property(field) is not null;

    public static object? Get(TItem item, string? field) => item is null ? null : Property(field)?.GetValue(item);

    public static DateTime GetDate(TItem item, string field) => Get(item, field) switch
    {
        DateTime d       => d,
        DateTimeOffset o => o.LocalDateTime,
        DateOnly d       => d.ToDateTime(TimeOnly.MinValue),
        _                => default,
    };

    public static void Set(TItem item, string? field, object? value)
    {
        var prop = Property(field);
        if (prop is null || !prop.CanWrite || item is null) return;
        prop.SetValue(item, Convert(value, prop.PropertyType));
    }

    private static object? Convert(object? value, Type target)
    {
        var underlying = Nullable.GetUnderlyingType(target) ?? target;
        if (value is null) return target.IsValueType && Nullable.GetUnderlyingType(target) is null ? Activator.CreateInstance(target) : null;
        if (underlying.IsInstanceOfType(value)) return value;

        if (value is DateTime date)
        {
            if (underlying == typeof(DateTimeOffset)) return new DateTimeOffset(date);
            if (underlying == typeof(DateOnly)) return DateOnly.FromDateTime(date);
        }
        if (value is IEnumerable<DateTime> dates && target.IsAssignableFrom(typeof(List<DateTime>)))
            return dates.ToList();
        if (underlying.IsEnum) return Enum.Parse(underlying, value.ToString()!, ignoreCase: true);
        if (underlying == typeof(Guid)) return Guid.Parse(value.ToString()!);
        return System.Convert.ChangeType(value, underlying, CultureInfo.InvariantCulture);
    }

    /// <summary>Shallow copy through public read/write properties.</summary>
    public static TItem Clone(TItem source, Func<TItem>? factory)
    {
        var copy = factory is not null ? factory() : Activator.CreateInstance<TItem>();
        foreach (var prop in typeof(TItem).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.CanRead && prop.CanWrite && prop.GetIndexParameters().Length == 0)
            {
                var value = prop.GetValue(source);
                // Copy lists so edits to the clone never touch the original.
                if (value is List<DateTime> list) value = new List<DateTime>(list);
                prop.SetValue(copy, value);
            }
        }
        return copy;
    }
}
