using Microsoft.AspNetCore.Components;

namespace MyStackBlazor.DataGrid;

/// <summary>
/// Column definition for MsDataGrid. Place inside MsDataGrid as child content.
/// This is a non-rendering component — it only declares metadata.
/// </summary>
public class MsDataGridColumn<TItem> : ComponentBase
{
    [CascadingParameter] private MsDataGrid<TItem>? Parent { get; set; }

    [Parameter, EditorRequired] public string Field { get; set; } = "";
    [Parameter, EditorRequired] public string Title { get; set; } = "";
    [Parameter] public bool Sortable { get; set; } = true;
    [Parameter] public bool Filterable { get; set; } = false;
    [Parameter] public string? Width { get; set; }
    [Parameter] public Func<TItem, object?>? ValueSelector { get; set; }
    [Parameter] public RenderFragment<TItem>? CellTemplate { get; set; }

    protected override void OnInitialized() => Parent?.RegisterColumn(this);

    public object? GetValue(TItem item) =>
        ValueSelector?.Invoke(item) ?? typeof(TItem).GetProperty(Field)?.GetValue(item);
}
