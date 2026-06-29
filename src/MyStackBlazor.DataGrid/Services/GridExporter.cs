using System.Text;
using System.Text.Json;
using ClosedXML.Excel;

namespace MyStackBlazor.DataGrid.Services;

public record ColumnDefinition(string Title, Func<object, object?> ValueSelector);

public static class GridExporter
{
    public static byte[] ExportToExcel<TItem>(
        IEnumerable<TItem> data,
        IEnumerable<ColumnDefinition> columns)
    {
        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Export");
        var colList = columns.ToList();

        for (int i = 0; i < colList.Count; i++)
        {
            var cell = ws.Cell(1, i + 1);
            cell.Value = colList[i].Title;
            cell.Style.Font.Bold = true;
        }

        int row = 2;
        foreach (var item in data)
        {
            for (int c = 0; c < colList.Count; c++)
            {
                var raw = colList[c].ValueSelector(item!);
                ws.Cell(row, c + 1).Value = raw switch
                {
                    null        => XLCellValue.FromObject(""),
                    bool b      => (XLCellValue)b,
                    double d    => (XLCellValue)d,
                    int i       => (XLCellValue)(double)i,
                    long l      => (XLCellValue)(double)l,
                    decimal m   => (XLCellValue)(double)m,
                    DateTime dt => (XLCellValue)dt,
                    DateTimeOffset dto => (XLCellValue)dto.DateTime,
                    _           => (XLCellValue)(raw.ToString() ?? "")
                };
            }
            row++;
        }

        ws.Columns().AdjustToContents();
        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    public static byte[] ExportToCsv<TItem>(
        IEnumerable<TItem> data,
        IEnumerable<ColumnDefinition> columns)
    {
        var colList = columns.ToList();
        var sb = new StringBuilder();

        sb.AppendLine(string.Join(",", colList.Select(c => CsvEscape(c.Title))));

        foreach (var item in data)
            sb.AppendLine(string.Join(",",
                colList.Select(c => CsvEscape(c.ValueSelector(item!)?.ToString() ?? ""))));

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public static byte[] ExportToJson<TItem>(
        IEnumerable<TItem> data,
        IEnumerable<ColumnDefinition> columns)
    {
        var colList = columns.ToList();
        var rows = data.Select(item =>
            colList.ToDictionary(
                c => c.Title,
                c => (object?)(c.ValueSelector(item!)?.ToString() ?? "")));

        var json = JsonSerializer.Serialize(rows,
            new JsonSerializerOptions { WriteIndented = true });
        return Encoding.UTF8.GetBytes(json);
    }

    private static string CsvEscape(string value)
    {
        if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
            return $"\"{value.Replace("\"", "\"\"")}\"";
        return value;
    }
}
