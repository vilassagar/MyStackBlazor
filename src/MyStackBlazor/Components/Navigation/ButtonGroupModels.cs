namespace MyStackBlazor.Components.Navigation;

/// <summary>How many toggle buttons in a <see cref="StackButtonGroup"/> can be selected at once.</summary>
public enum ButtonGroupSelectionMode
{
    /// <summary>Selecting a toggle button deselects the others (radio-like).</summary>
    Single,
    /// <summary>Each toggle button is selected independently.</summary>
    Multiple,
}

/// <summary>Theme constants for the <see cref="StackButtonGroup"/> appearance parameters.</summary>
public static class ButtonGroupTheme
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
        public const string Outline = "outline";
        public const string Flat    = "flat";
        public const string Clear   = "clear";
        public const string Link    = "link";
    }

    public static class ThemeColor
    {
        public const string Base      = "base";
        public const string Primary   = "primary";
        public const string Secondary = "secondary";
        public const string Tertiary  = "tertiary";
        public const string Info      = "info";
        public const string Success   = "success";
        public const string Warning   = "warning";
        public const string Error     = "error";
        public const string Dark      = "dark";
        public const string Light     = "light";
        public const string Inverse   = "inverse";
    }
}
