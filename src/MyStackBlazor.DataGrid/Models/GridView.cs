namespace MyStackBlazor.DataGrid.Models;

/// <summary>
/// A named snapshot of a grid's layout and query: sort, filters, search, hidden columns, grouping and page size.
/// Plain data so it round-trips through System.Text.Json — persist it wherever you like (localStorage, user profile, DB).
/// </summary>
public sealed record GridView
{
    public string Name { get; init; } = "";
    public IReadOnlyList<SortDescriptor> Sort { get; init; } = [];
    /// <summary>Text typed in the filter row, keyed by column field.</summary>
    public IReadOnlyDictionary<string, string> FilterRow { get; init; } = new Dictionary<string, string>();
    /// <summary>Header filter-menu conditions, keyed by column field.</summary>
    public IReadOnlyDictionary<string, GridViewFilter> MenuFilters { get; init; } = new Dictionary<string, GridViewFilter>();
    public string? SearchTerm { get; init; }
    public IReadOnlyList<string> HiddenColumns { get; init; } = [];
    public IReadOnlyList<string> Groups { get; init; } = [];
    /// <summary>Page size when the grid is pageable; null keeps the grid's current size.</summary>
    public int? PageSize { get; init; }
}

/// <summary>One header filter-menu condition. <see cref="Operator"/> is the menu key, e.g. "contains", "gt", "isnull".</summary>
public sealed record GridViewFilter(string Operator, string? Value);
