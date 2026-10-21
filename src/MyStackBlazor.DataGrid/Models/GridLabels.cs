namespace MyStackBlazor.DataGrid.Models;

public record GridLabels(
    string SearchPlaceholder = "Search…",
    string AddRowText = "Add Row",
    string ColumnsText = "Columns",
    string ExportText = "Export",
    string NoResultsText = "No results found",
    string ShowRowsWhereText = "Show rows where:",
    string RowsPerPageText = "Rows per page",
    string ViewsText = "Views",
    string SaveViewText = "Save view",
    string ViewNamePlaceholder = "View name…",
    string NoSavedViewsText = "No saved views")
{
    public static readonly GridLabels Default = new();
}
