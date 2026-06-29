using Microsoft.AspNetCore.Components;
using MyStackBlazor.DataGrid.Models;

namespace MyStackBlazor.DataGrid;

public class MsDataGridColumn<TItem> : ComponentBase
{
    [CascadingParameter] private MsDataGrid<TItem>? Parent { get; set; }

    [Parameter, EditorRequired] public string Field { get; set; } = "";
    [Parameter, EditorRequired] public string Title { get; set; } = "";
    [Parameter] public bool Sortable { get; set; } = true;
    [Parameter] public bool Filterable { get; set; } = false;
    [Parameter] public bool Visible { get; set; } = true;
    [Parameter] public bool Groupable { get; set; } = false;
    [Parameter] public string? Width { get; set; }
    [Parameter] public string? Tooltip { get; set; }
    [Parameter] public Func<TItem, object?>? ValueSelector { get; set; }
    [Parameter] public RenderFragment<TItem>? CellTemplate { get; set; }
    [Parameter] public Func<TItem, string?>? CellStyleFunc { get; set; }
    [Parameter] public Func<TItem, string?>? CellClassFunc { get; set; }
    // Editing
    [Parameter] public bool Editable { get; set; } = false;
    [Parameter] public EditorType EditorType { get; set; } = EditorType.Text;
    /// <summary>Apply an edit string value back to the item. Required when Editable=true.</summary>
    [Parameter] public Action<TItem, string>? ValueSetter { get; set; }

    protected override void OnInitialized() => Parent?.RegisterColumn(this);

    public object? GetValue(TItem item) =>
        ValueSelector?.Invoke(item) ?? typeof(TItem).GetProperty(Field)?.GetValue(item);

    public void SetValue(TItem item, string value) =>
        ValueSetter?.Invoke(item, value);
}
