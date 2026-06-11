using ClosedXML.Excel;

namespace MyStackBlazor.DataGrid.Services;

public record ColumnDefinition(string Title, Func<object, object?> ValueSelector);

public static class GridExporter
{
    /// <summary>
    /// Exports data to an Excel workbook and returns the raw bytes for download.
    /// </summary>
    public static byte[] ExportToExcel<TItem>(
        IEnumerable<TItem> data,
        IEnumerable<ColumnDefinition> columns)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Export");

        var colList = columns.ToList();

        // Header row
        for (int i = 0; i < colList.Count; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = colList[i].Title;
            cell.Style.Font.Bold = true;
        }

        // Data rows
        int row = 2;
        foreach (var item in data)
        {
            for (int c = 0; c < colList.Count; c++)
            {
                var raw = colList[c].ValueSelector(item!);
                ws.Cell(row, c + 1).Value = raw switch
                {
                    null                        => XLCellValue.FromObject(""),
                    bool b                      => (XLCellValue)b,
                    double d                    => (XLCellValue)d,
                    int i                       => (XLCellValue)(double)i,
                    long l                      => (XLCellValue)(double)l,
                    decimal m                   => (XLCellValue)(double)m,
                    DateTime dt                 => (XLCellValue)dt,
                    DateTimeOffset dto          => (XLCellValue)dto.DateTime,
                    _                           => (XLCellValue)(raw.ToString() ?? "")
                };
            }
            row++;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }
}
