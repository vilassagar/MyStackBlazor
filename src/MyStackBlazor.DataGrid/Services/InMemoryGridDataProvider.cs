using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using MyStackBlazor.DataGrid.Models;

namespace MyStackBlazor.DataGrid.Services;

/// <summary>
/// <see cref="IGridDataProvider{TItem}"/> over an in-memory collection.
/// Sorting and filtering compare the real typed values (numbers, dates, enums, booleans),
/// not their display strings — so 10 sorts after 9 and 02/01/2025 after 12/31/2024.
/// </summary>
public sealed class InMemoryGridDataProvider<TItem> : IGridDataProvider<TItem>
{
    private readonly Func<IEnumerable<TItem>> _source;
    private readonly Func<string, Func<TItem, object?>?>? _selectorResolver;
    private readonly Dictionary<string, Func<TItem, object?>> _selectorCache = new(StringComparer.Ordinal);

    /// <param name="items">The rows. The collection is re-read on every request, so later additions/removals are picked up on refresh.</param>
    /// <param name="selectors">Optional value selectors keyed by column field. Fields without one fall back to a (dotted) property path on <typeparamref name="TItem"/>.</param>
    public InMemoryGridDataProvider(IEnumerable<TItem> items, IReadOnlyDictionary<string, Func<TItem, object?>>? selectors = null)
        : this(() => items, selectors is null ? null : f => selectors.TryGetValue(f, out var s) ? s : null) { }

    /// <param name="source">Called on every request to get the current rows.</param>
    /// <param name="selectorResolver">Returns the value selector for a field, or null to fall back to a property path.</param>
    public InMemoryGridDataProvider(Func<IEnumerable<TItem>> source, Func<string, Func<TItem, object?>?>? selectorResolver = null)
    {
        _source = source ?? throw new ArgumentNullException(nameof(source));
        _selectorResolver = selectorResolver;
    }

    /// <summary>Fields searched by the toolbar search box. Default: every field that is sorted, filtered, or any public property.</summary>
    public IReadOnlyList<string>? SearchFields { get; init; }

    /// <summary>Culture used to parse filter values typed by the user. Default: <see cref="CultureInfo.CurrentCulture"/>.</summary>
    public CultureInfo? Culture { get; init; }

    public ValueTask<GridDataResult<TItem>> GetDataAsync(GridRequest request, CancellationToken ct = default)
    {
        IEnumerable<TItem> query = _source() ?? [];

        foreach (var f in request.Filters)
        {
            var sel = GetSelector(f.Field);
            var filter = f;
            query = query.Where(item => Matches(sel(item), filter));
        }

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var term = request.SearchTerm.Trim();
            var fields = SearchFields ?? DefaultSearchFields;
            var sels = fields.Select(GetSelector).ToArray();
            query = query.Where(item => sels.Any(s => Format(s(item))?.Contains(term, StringComparison.CurrentCultureIgnoreCase) == true));
        }

        var list = query.ToList();

        if (request.Sort.Count > 0)
        {
            IOrderedEnumerable<TItem>? ordered = null;
            foreach (var s in request.Sort)
            {
                var sel = GetSelector(s.Field);
                var desc = s.Direction == SortDirection.Descending;
                ordered = ordered is null
                    ? (desc ? list.OrderByDescending(sel, ValueComparer.Instance) : list.OrderBy(sel, ValueComparer.Instance))
                    : (desc ? ordered.ThenByDescending(sel, ValueComparer.Instance) : ordered.ThenBy(sel, ValueComparer.Instance));
            }
            list = ordered!.ToList();
        }

        var total = list.Count;
        var start = Math.Clamp(request.Page.StartIndex, 0, total);
        var count = Math.Clamp(request.Page.Count, 0, total - start);
        return ValueTask.FromResult(new GridDataResult<TItem>(list.GetRange(start, count), total));
    }

    // ── Value access ──────────────────────────────────────────────────────

    private static readonly string[] DefaultSearchFields =
        typeof(TItem).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetIndexParameters().Length == 0)
            .Select(p => p.Name)
            .ToArray();

    private Func<TItem, object?> GetSelector(string field)
    {
        if (_selectorCache.TryGetValue(field, out var cached)) return cached;
        var resolved = _selectorResolver?.Invoke(field);
        if (resolved is not null) return _selectorCache[field] = resolved;
        // Not cached when a resolver exists: it may supply a selector later (e.g. a grid column that registers after the first request).
        var path = BuildPropertyPath(field);
        if (_selectorResolver is null) _selectorCache[field] = path;
        return path;
    }

    private static Func<TItem, object?> BuildPropertyPath(string field)
    {
        var props = new List<PropertyInfo>();
        var type = typeof(TItem);
        foreach (var part in field.Split('.'))
        {
            var p = type.GetProperty(part, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p is null) return _ => null;
            props.Add(p);
            type = p.PropertyType;
        }
        return item =>
        {
            object? cur = item;
            foreach (var p in props)
            {
                if (cur is null) return null;
                cur = p.GetValue(cur);
            }
            return cur;
        };
    }

    private CultureInfo Cult => Culture ?? CultureInfo.CurrentCulture;

    private string? Format(object? v) => v switch
    {
        null => null,
        IFormattable f => f.ToString(null, Cult),
        _ => v.ToString(),
    };

    // ── Filtering ─────────────────────────────────────────────────────────

    private bool Matches(object? value, FilterDescriptor f)
    {
        switch (f.Operator)
        {
            case FilterOperator.IsNull:    return value is null || value is string { Length: 0 };
            case FilterOperator.IsNotNull: return !(value is null || value is string { Length: 0 });
        }

        if (f.Value is null) return true;

        switch (f.Operator)
        {
            case FilterOperator.Contains:
                return Format(value)?.Contains(Format(f.Value) ?? "", StringComparison.CurrentCultureIgnoreCase) == true;
            case FilterOperator.StartsWith:
                return Format(value)?.StartsWith(Format(f.Value) ?? "", StringComparison.CurrentCultureIgnoreCase) == true;
            case FilterOperator.EndsWith:
                return Format(value)?.EndsWith(Format(f.Value) ?? "", StringComparison.CurrentCultureIgnoreCase) == true;
        }

        if (value is null) return f.Operator == FilterOperator.NotEquals;

        var target = Coerce(f.Value, value.GetType());
        int cmp;
        if (target is null)
        {
            // Couldn't convert (e.g. "abc" against an int column) → fall back to text comparison.
            cmp = string.Compare(Format(value), Format(f.Value), StringComparison.CurrentCultureIgnoreCase);
        }
        else
        {
            cmp = ValueComparer.Instance.Compare(value, target);
        }

        return f.Operator switch
        {
            FilterOperator.Equals             => cmp == 0,
            FilterOperator.NotEquals          => cmp != 0,
            FilterOperator.GreaterThan        => cmp > 0,
            FilterOperator.LessThan           => cmp < 0,
            FilterOperator.GreaterThanOrEqual => cmp >= 0,
            FilterOperator.LessThanOrEqual    => cmp <= 0,
            _ => true,
        };
    }

    /// <summary>Converts a filter value (usually the string the user typed) to <paramref name="type"/>. Returns null when it can't.</summary>
    internal object? Coerce(object raw, Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;
        if (type.IsInstanceOfType(raw)) return raw;
        var s = raw as string ?? Format(raw);
        if (s is null) return null;
        s = s.Trim();

        try
        {
            if (type == typeof(string)) return s;
            if (type.IsEnum)
                return Enum.TryParse(type, s, ignoreCase: true, out var e) ? e : null;
            if (type == typeof(DateTime))
                return DateTime.TryParse(s, Cult, DateTimeStyles.None, out var d) ||
                       DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out d) ? d : null;
            if (type == typeof(DateTimeOffset))
                return DateTimeOffset.TryParse(s, Cult, DateTimeStyles.None, out var d) ||
                       DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out d) ? d : null;
            if (type == typeof(DateOnly))
                return DateOnly.TryParse(s, Cult, DateTimeStyles.None, out var d) ||
                       DateOnly.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out d) ? d : null;
            if (type == typeof(TimeOnly))
                return TimeOnly.TryParse(s, Cult, DateTimeStyles.None, out var t) ? t : null;
            if (type == typeof(bool))
                return s.ToLowerInvariant() switch
                {
                    "true" or "yes" or "1" or "y" => true,
                    "false" or "no" or "0" or "n" => false,
                    _ => null,
                };
            if (type == typeof(Guid))
                return Guid.TryParse(s, out var g) ? g : null;
            if (IsNumeric(type))
            {
                if (decimal.TryParse(s, NumberStyles.Any, Cult, out var dec) ||
                    decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out dec))
                    return Convert.ChangeType(dec, type, CultureInfo.InvariantCulture);
                return null;
            }
            var conv = TypeDescriptor.GetConverter(type);
            return conv.CanConvertFrom(typeof(string)) ? conv.ConvertFromString(null, Cult, s) : null;
        }
        catch (Exception ex) when (ex is FormatException or OverflowException or InvalidCastException or NotSupportedException or ArgumentException)
        {
            return null;
        }
    }

    private static bool IsNumeric(Type t) => Type.GetTypeCode(t) is
        TypeCode.Byte or TypeCode.SByte or TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32 or
        TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64 or TypeCode.Single or TypeCode.Double or TypeCode.Decimal;

    /// <summary>
    /// Orders values by their real type: nulls first, numbers numerically (across numeric types),
    /// strings case-insensitively in the current culture, everything else via <see cref="IComparable"/>.
    /// </summary>
    internal sealed class ValueComparer : IComparer<object?>
    {
        public static readonly ValueComparer Instance = new();

        public int Compare(object? x, object? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return -1;
            if (y is null) return 1;

            if (x is string sx && y is string sy)
                return string.Compare(sx, sy, CultureInfo.CurrentCulture, CompareOptions.IgnoreCase);

            var tx = x.GetType();
            var ty = y.GetType();
            if (tx != ty && IsNumeric(tx) && IsNumeric(ty))
            {
                try { return Convert.ToDecimal(x, CultureInfo.InvariantCulture).CompareTo(Convert.ToDecimal(y, CultureInfo.InvariantCulture)); }
                catch (OverflowException) { return Convert.ToDouble(x, CultureInfo.InvariantCulture).CompareTo(Convert.ToDouble(y, CultureInfo.InvariantCulture)); }
            }

            if (tx == ty && x is IComparable c) return c.CompareTo(y);
            return Comparer.Default.Compare(x.ToString(), y.ToString());
        }
    }
}
