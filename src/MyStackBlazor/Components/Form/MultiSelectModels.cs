namespace MyStackBlazor.Components.Form;

/// <summary>How selected items are shown in a <c>StackMultiSelect</c>.</summary>
public enum MultiSelectTagMode
{
    /// <summary>One tag per selected item.</summary>
    Multiple,
    /// <summary>A single summary tag ("3 items selected").</summary>
    Single,
}

/// <summary>Arguments for <c>StackMultiSelect.OnRead</c>. Fill <see cref="Data"/> for <see cref="Request"/>.</summary>
public class MultiSelectReadEventArgs<TItem>
{
    public required DropDownListReadRequest Request { get; init; }
    public CancellationToken CancellationToken { get; init; }
    public IEnumerable<TItem>? Data { get; set; }
    public int Total { get; set; }
}

/// <summary>Arguments for <c>StackMultiSelect.OnItemRender</c>. Set <see cref="Class"/> to style the item.</summary>
public class MultiSelectItemRenderEventArgs<TItem>
{
    public required TItem Item { get; init; }
    public string? Class { get; set; }
}

/// <summary>Popup size settings for <c>StackMultiSelect</c>. Place it inside <c>&lt;MultiSelectSettings&gt;</c>.</summary>
public class StackMultiSelectPopupSettings : StackDropDownListPopupSettings
{
}
