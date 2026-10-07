namespace MyStackBlazor.Components.Navigation;

/// <summary>Where the tab headers render relative to the content.</summary>
public enum TabPosition
{
    Top,
    Bottom,
    Left,
    Right,
}

/// <summary>How the tab headers are aligned along the tab list.</summary>
public enum TabStripTabAlignment
{
    Start,
    Center,
    End,
    /// <summary>Headers are spread out with equal space between them.</summary>
    Justify,
    /// <summary>Headers grow to fill the tab list equally.</summary>
    Stretch,
}

/// <summary>Where the scroll buttons render when <c>Scrollable</c> is true.</summary>
public enum TabStripScrollButtonsPosition
{
    /// <summary>One button on each side of the tab list.</summary>
    Split,
    /// <summary>Both buttons before the tab list.</summary>
    Start,
    /// <summary>Both buttons after the tab list.</summary>
    End,
}

/// <summary>When the scroll buttons are shown when <c>Scrollable</c> is true.</summary>
public enum TabStripScrollButtonsVisibility
{
    Visible,
    /// <summary>Only when the tabs overflow the tab list.</summary>
    Auto,
    Hidden,
}

/// <summary>Theme constants for <c>StackTabStrip.Size</c>.</summary>
public static class TabStripTheme
{
    public static class Size
    {
        public const string Small  = "sm";
        public const string Medium = "md";
        public const string Large  = "lg";
    }
}

/// <summary>Arguments for <c>OnTabClose</c>. Set <see cref="IsCancelled"/> to keep the tab.</summary>
public class TabStripTabCloseEventArgs
{
    public required string TabId { get; init; }
    public int TabIndex { get; init; }
    public bool IsCancelled { get; set; }
}
