namespace MyStackBlazor.Components.Form;

public class MultiColumnComboBoxColumn<TItem>
{
    public required string Header { get; set; }
    public required Func<TItem, string?> Value { get; set; }
    public string? Width { get; set; }
}
