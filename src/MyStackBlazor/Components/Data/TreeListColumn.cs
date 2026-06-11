using Microsoft.AspNetCore.Components;

namespace MyStackBlazor.Components.Data;

public class TreeListColumn<TItem>
{
    public required string Header { get; set; }
    public Func<TItem, string?>? Field { get; set; }
    public RenderFragment<TItem>? Template { get; set; }
    public string? Width { get; set; }
    public string? Class { get; set; }
}
