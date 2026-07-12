namespace MyStackBlazor.DataGrid.Models;

public record GridLabels(
    string SearchPlaceholder = "Search…",
    string AddRowText = "Add Row",
    string ColumnsText = "Columns",
    string ExportText = "Export",
    string NoResultsText = "No results found",
    string ShowRowsWhereText = "Show rows where:")
{
    public static readonly GridLabels Default = new();
}
