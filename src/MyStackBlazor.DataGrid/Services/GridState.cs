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

    public GridState ToggleSort(string field)
    {
        var existing = Sort.FirstOrDefault(s => s.Field == field);
        List<SortDescriptor> updated;

        if (existing is null)
            updated = [new SortDescriptor(field, SortDirection.Ascending)];
        else if (existing.Direction == SortDirection.Ascending)
            updated = [new SortDescriptor(field, SortDirection.Descending)];
        else
            updated = Sort.Where(s => s.Field != field).ToList();

        return WithSort(updated);
    }
}
