namespace MyStackBlazor.DataGrid.Models;

public record SortDescriptor(string Field, SortDirection Direction);

public enum SortDirection { Ascending, Descending }
