using MyStackBlazor.DataGrid.Models;

namespace MyStackBlazor.DataGrid.Services;

public sealed class GridState
{
    public static GridState Default => new();

    public IReadOnlyList<SortDescriptor> Sort { get; private set; } = [];
    public IReadOnlyList<FilterDescriptor> Filters { get; private set; } = [];
    public string? SearchTerm { get; private set; }

    public GridState WithSort(IReadOnlyList<SortDescriptor> sort)
        => new() { Sort = sort, Filters = Filters, SearchTerm = SearchTerm };

    public GridState WithFilters(IReadOnlyList<FilterDescriptor> filters)
        => new() { Sort = Sort, Filters = filters, SearchTerm = SearchTerm };

    public GridState WithSearch(string? term)
        => new() { Sort = Sort, Filters = Filters, SearchTerm = term };

    /// <summary>
    /// Toggle sort on <paramref name="field"/>.
    /// When <paramref name="addToExisting"/> is true (Shift+click), the sort is appended to the
    /// existing multi-sort list instead of replacing it.
    /// </summary>
    public GridState ToggleSort(string field, bool addToExisting = false)
    {
        var existing = Sort.FirstOrDefault(s => s.Field == field);
        List<SortDescriptor> updated;

        if (existing is null)
        {
            updated = addToExisting
                ? [.. Sort, new SortDescriptor(field, SortDirection.Ascending)]
                : [new SortDescriptor(field, SortDirection.Ascending)];
        }
        else if (existing.Direction == SortDirection.Ascending)
        {
            var desc = new SortDescriptor(field, SortDirection.Descending);
            updated = addToExisting
                ? Sort.Select(s => s.Field == field ? desc : s).ToList()
                : [desc];
        }
        else
        {
            updated = addToExisting
                ? Sort.Where(s => s.Field != field).ToList()
                : [];
        }

        return WithSort(updated);
    }
}
