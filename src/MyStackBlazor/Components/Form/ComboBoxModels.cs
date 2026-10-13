namespace MyStackBlazor.Components.Form;

/// <summary>A simple value/label pair for <c>StackCombobox</c> (earlier API: <c>Items</c>).</summary>
public record ComboboxItem(string Value, string Label);

/// <summary>Arguments for <c>StackCombobox.OnRead</c>. Fill <see cref="Data"/> for <see cref="Request"/>.</summary>
public class ComboBoxReadEventArgs<TItem>
{
    public required DropDownListReadRequest Request { get; init; }
    public CancellationToken CancellationToken { get; init; }
    public IEnumerable<TItem>? Data { get; set; }
    public int Total { get; set; }
}

/// <summary>Arguments for <c>StackCombobox.OnItemRender</c>. Set <see cref="Class"/> to style the item.</summary>
public class ComboBoxItemRenderEventArgs<TItem>
{
    public required TItem Item { get; init; }
    public string? Class { get; set; }
}

/// <summary>Popup size settings for <c>StackCombobox</c>. Place it inside <c>&lt;ComboBoxSettings&gt;</c>.</summary>
public class StackComboBoxPopupSettings : StackDropDownListPopupSettings
{
}
