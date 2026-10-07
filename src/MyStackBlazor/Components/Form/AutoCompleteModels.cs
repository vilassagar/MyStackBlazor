namespace MyStackBlazor.Components.Form;

/// <summary>Arguments for <c>StackAutoComplete.OnOpen</c>. Set <see cref="IsCancelled"/> to keep the popup closed.</summary>
public class AutoCompleteOpenEventArgs
{
    public bool IsCancelled { get; set; }
}

/// <summary>Arguments for <c>StackAutoComplete.OnClose</c>. Set <see cref="IsCancelled"/> to keep the popup open.</summary>
public class AutoCompleteCloseEventArgs
{
    public bool IsCancelled { get; set; }
}

/// <summary>Arguments for <c>StackAutoComplete.OnItemRender</c>. Set <see cref="Class"/> to style the item.</summary>
public class AutoCompleteItemRenderEventArgs<TItem>
{
    public required TItem Item { get; init; }
    public string? Class { get; set; }
}

/// <summary>Arguments for <c>StackAutoComplete.OnRead</c>. Fill <see cref="Data"/> with the suggestions for <see cref="Request"/>.</summary>
public class AutoCompleteReadEventArgs<TItem>
{
    public required DropDownListReadRequest Request { get; init; }
    public CancellationToken CancellationToken { get; init; }
    public IEnumerable<TItem>? Data { get; set; }
    public int Total { get; set; }
}

/// <summary>Popup size settings for <c>StackAutoComplete</c>. Place it inside <c>&lt;AutoCompleteSettings&gt;</c>.</summary>
public class StackAutoCompletePopupSettings : StackDropDownListPopupSettings
{
}
