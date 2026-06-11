namespace MyStackBlazor.DataGrid.Models;

public record PageDescriptor(int StartIndex, int Count)
{
    public int PageNumber => Count > 0 ? StartIndex / Count : 0;
}
