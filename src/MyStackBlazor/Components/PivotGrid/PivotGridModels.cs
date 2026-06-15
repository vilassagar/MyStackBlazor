using Microsoft.AspNetCore.Components;

namespace MyStackBlazor.Components.PivotGrid;

public enum PivotColumnType { Text, Number, Date, Boolean }

public class PivotColumn<TItem>
{
    public string Key { get; set; } = "";
    public string Title { get; set; } = "";
    public Func<TItem, object?> ValueSelector { get; set; } = _ => null;
    public RenderFragment<TItem>? Template { get; set; }
    public bool Sortable { get; set; } = true;
    public bool Filterable { get; set; } = true;
    public bool Hidden { get; set; } = false;
    public string? Width { get; set; }
    public string Align { get; set; } = "left";      // left | center | right
    public PivotColumnType Type { get; set; } = PivotColumnType.Text;
    public string? Format { get; set; }               // e.g. "N2", "yyyy-MM-dd"
    public Func<TItem, string?>? CellClass { get; set; }
    public Func<TItem, string?>? CellStyle { get; set; }
}

public class CellClickArgs<TItem>
{
    public string ColumnKey { get; set; } = "";
    public string ColumnTitle { get; set; } = "";
    public TItem? Row { get; set; }
    public object? Value { get; set; }
}
