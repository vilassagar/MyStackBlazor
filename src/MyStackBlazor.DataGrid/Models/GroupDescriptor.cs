namespace MyStackBlazor.DataGrid.Models;

public record GroupDescriptor(string Field, string? Title = null, bool Descending = false);

public class GroupHeader
{
    public string Field { get; init; } = "";
    public string ColumnTitle { get; init; } = "";
    public object? Value { get; init; }
    public int Count { get; init; }
    public bool Collapsed { get; set; } = false;
}
