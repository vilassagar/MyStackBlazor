namespace MyStackBlazor.Security;

public sealed class SecurityOptions
{
    /// <summary>HTML tags permitted by the sanitizer.</summary>
    public ISet<string> AllowedTags { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "strong", "em", "b", "i", "u", "s", "p", "br",
        "ul", "ol", "li", "a", "span", "div", "blockquote",
        "h1", "h2", "h3", "h4", "h5", "h6", "code", "pre"
    };

    /// <summary>HTML attributes permitted by the sanitizer.</summary>
    public ISet<string> AllowedAttributes { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "href", "title", "class", "id", "target", "rel"
    };

    /// <summary>URI schemes permitted in href and src attributes. No javascript: URIs.</summary>
    public ISet<string> AllowedUriSchemes { get; set; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "https", "mailto"
    };
}
