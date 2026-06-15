using MyStackBlazor.DataGrid.Models;

namespace MyStackBlazor.DataGrid.Services;

/// <summary>
/// Server-side data contract for MsDataGrid.
/// Implement this to wire up your own data source (EF Core, REST API, etc.).
/// </summary>
public interface IGridDataProvider<TItem>
{
    ValueTask<GridDataResult<TItem>> GetDataAsync(GridRequest request, CancellationToken ct = default);
}

public record GridRequest(
    PageDescriptor Page,
    IReadOnlyList<SortDescriptor> Sort,
    IReadOnlyList<FilterDescriptor> Filters,
    string? SearchTerm = null);

public record GridDataResult<TItem>(IEnumerable<TItem> Items, int TotalCount);
