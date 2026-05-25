using Microsoft.AspNetCore.Components;

namespace MyStackBlazor.Components.Data;

public class AdvancedTableColumn<TItem>
{
    public required string Key { get; set; }
    public required string Header { get; set; }
    public Func<TItem, object?>? Value { get; set; }
    public RenderFragment<TItem>? Template { get; set; }
    public bool Sortable { get; set; } = true;
    public bool Filterable { get; set; } = true;
    public ColumnDataType DataType { get; set; } = ColumnDataType.String;
    public string? Class { get; set; }
}

public enum ColumnDataType
{
    String,
    Number,
    Date
}

public enum FilterOperator
{
    Contains,
    StartsWith,
    EndsWith,
    Equals,
    NotEquals,
    LessThan,
    LessThanOrEqualTo,
    GreaterThan,
    GreaterThanOrEqualTo,
    Between
}

public class AdvancedFilterRule
{
    public string ColumnKey { get; set; } = "";
    public FilterOperator Operator { get; set; } = FilterOperator.Contains;
    public string Value { get; set; } = "";
    public string Value2 { get; set; } = "";
}
