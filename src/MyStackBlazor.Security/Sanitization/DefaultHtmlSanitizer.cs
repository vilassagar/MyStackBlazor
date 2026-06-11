using Ganss.Xss;

namespace MyStackBlazor.Security;

public sealed class DefaultHtmlSanitizer : IHtmlSanitizer
{
    private readonly HtmlSanitizer _sanitizer;

    public DefaultHtmlSanitizer() : this(new SecurityOptions()) { }

    public DefaultHtmlSanitizer(SecurityOptions options)
    {
        _sanitizer = new HtmlSanitizer();

        // Start from clean slate — deny by default
        _sanitizer.AllowedTags.Clear();
        foreach (var tag in options.AllowedTags)
            _sanitizer.AllowedTags.Add(tag);

        _sanitizer.AllowedAttributes.Clear();
        foreach (var attr in options.AllowedAttributes)
            _sanitizer.AllowedAttributes.Add(attr);

        _sanitizer.AllowedSchemes.Clear();
        foreach (var scheme in options.AllowedUriSchemes)
            _sanitizer.AllowedSchemes.Add(scheme);
    }

    public string Sanitize(string rawHtml) => _sanitizer.Sanitize(rawHtml);
}
