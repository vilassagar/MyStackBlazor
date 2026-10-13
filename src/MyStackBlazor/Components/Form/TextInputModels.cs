namespace MyStackBlazor.Components.Form;

/// <summary>How the user can resize a <see cref="StackTextarea"/>.</summary>
public enum TextAreaResizeMode
{
    None,
    Both,
    Horizontal,
    Vertical,
    /// <summary>Same as <c>AutoSize</c>: the height follows the content.</summary>
    Auto,
}
