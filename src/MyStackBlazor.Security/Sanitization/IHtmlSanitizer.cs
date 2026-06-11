namespace MyStackBlazor.Security;

public interface IHtmlSanitizer
{
    /// <summary>
    /// Strips unsafe tags/attributes from raw HTML. Safe to render with @((MarkupString)result).
    /// </summary>
    string Sanitize(string rawHtml);
}
