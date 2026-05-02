using Microsoft.AspNetCore.Components;

namespace MyStackBlazor.Components.Data;

public class DataTableColumn<TItem>
{
    public required string Key { get; set; }
    public required string Header { get; set; }
    public Func<TItem, object?>? Value { get; set; }
    public RenderFragment<TItem>? Template { get; set; }
    public bool Sortable { get; set; } = true;
    public string? Class { get; set; }
}
