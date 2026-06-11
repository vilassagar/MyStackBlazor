namespace MyStackBlazor.DataGrid.Models;

public record FilterDescriptor(string Field, FilterOperator Operator, object? Value);

public enum FilterOperator
{
    Equals,
    NotEquals,
    Contains,
    StartsWith,
    EndsWith,
    GreaterThan,
    LessThan,
    GreaterThanOrEqual,
    LessThanOrEqual,
    IsNull,
    IsNotNull
}
