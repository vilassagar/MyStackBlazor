using Microsoft.AspNetCore.Components;

namespace MyStackBlazor.DataGrid;

/// <summary>
/// Appends a command cell to every row in MsDataGrid with Edit and/or Delete buttons.
/// Place inside MsDataGrid as child content alongside MsDataGridColumn elements.
/// </summary>
public class MsDataGridCommandColumn<TItem> : ComponentBase
{
    [CascadingParameter] private MsDataGrid<TItem>? Parent { get; set; }

    [Parameter] public string Title { get; set; } = "";
    [Parameter] public string? Width { get; set; } = "140px";
    [Parameter] public string? EditLabel { get; set; } = "Edit";
    [Parameter] public string? DeleteLabel { get; set; } = "Delete";
    [Parameter] public EventCallback<TItem> OnEdit { get; set; }
    [Parameter] public EventCallback<TItem> OnDelete { get; set; }
    /// <summary>Override default Edit/Delete buttons with a fully custom RenderFragment.</summary>
    [Parameter] public RenderFragment<TItem>? CustomButtons { get; set; }

    protected override void OnInitialized() => Parent?.RegisterCommandColumn(this);
}
