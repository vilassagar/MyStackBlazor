namespace MyStackBlazor.Components.Form;

/// <summary>Operator used when <c>Filterable</c> is enabled on a dropdown.</summary>
public enum StringFilterOperator
{
    StartsWith,
    Contains,
    DoesNotContain,
    EndsWith,
    IsEqualTo,
    IsNotEqualTo,
}

/// <summary>How the dropdown popup renders long lists.</summary>
public enum DropDownScrollMode
{
    /// <summary>All items are rendered and the popup scrolls.</summary>
    Scrollable,
    /// <summary>Only visible items are rendered (requires <c>ItemHeight</c> and <c>PageSize</c>).</summary>
    Virtual,
}

/// <summary>Whether the popup switches to a full-width action sheet on small screens.</summary>
public enum AdaptiveMode
{
    None,
    Auto,
}

/// <summary>Theme constants for the dropdown appearance parameters (<c>Size</c>, <c>Rounded</c>, <c>FillMode</c>).</summary>
public static class DropDownListTheme
{
    public static class Size
    {
        public const string Small  = "sm";
        public const string Medium = "md";
        public const string Large  = "lg";
    }

    public static class Rounded
    {
        public const string None   = "none";
        public const string Small  = "sm";
        public const string Medium = "md";
        public const string Large  = "lg";
        public const string Full   = "full";
    }

    public static class FillMode
    {
        public const string Solid   = "solid";
        public const string Flat    = "flat";
        public const string Outline = "outline";
    }
}

/// <summary>Arguments for <c>OnOpen</c>. Set <see cref="IsCancelled"/> to keep the popup closed.</summary>
public class DropDownListOpenEventArgs
{
    public bool IsCancelled { get; set; }
}

/// <summary>Arguments for <c>OnClose</c>. Set <see cref="IsCancelled"/> to keep the popup open.</summary>
public class DropDownListCloseEventArgs
{
    public bool IsCancelled { get; set; }
}

/// <summary>Arguments for <c>OnItemRender</c>. Set <see cref="Class"/> to add CSS classes to the item.</summary>
public class DropDownListItemRenderEventArgs<TItem>
{
    public required TItem Item { get; init; }
    public string? Class { get; set; }
}

/// <summary>Describes what the dropdown needs when it loads data through <c>OnRead</c>.</summary>
public class DropDownListReadRequest
{
    public string? FilterText { get; init; }
    public StringFilterOperator FilterOperator { get; init; }

    /// <summary>The field the filter applies to (<c>FilterField</c>, or <c>TextField</c> when not set).</summary>
    public string? FilterField { get; init; }
    public bool CaseSensitive { get; init; }
    public int Skip { get; init; }

    /// <summary>Number of items to return. 0 means "all".</summary>
    public int Take { get; init; }
}

/// <summary>
/// Arguments for <c>OnRead</c>. Fill <see cref="Data"/> (and <see cref="Total"/> when virtual
/// scrolling) with the items for <see cref="Request"/>.
/// </summary>
public class DropDownListReadEventArgs<TItem>
{
    public required DropDownListReadRequest Request { get; init; }
    public CancellationToken CancellationToken { get; init; }
    public IEnumerable<TItem>? Data { get; set; }
    public int Total { get; set; }
}
