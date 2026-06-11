using MyStackBlazor.Security;

namespace MyStackBlazor.UnitTests.Components;

public class HtmlSanitizerTests
{
    private readonly IHtmlSanitizer _sanitizer = new DefaultHtmlSanitizer();

    [Fact]
    public void Sanitize_AllowsSafeHtml()
    {
        var result = _sanitizer.Sanitize("<strong>Hello</strong> <em>World</em>");
        result.Should().Contain("<strong>Hello</strong>");
        result.Should().Contain("<em>World</em>");
    }

    [Fact]
    public void Sanitize_RemovesScriptTags()
    {
        var result = _sanitizer.Sanitize("<script>alert('xss')</script>Safe text");
        result.Should().NotContain("<script>");
        result.Should().Contain("Safe text");
    }

    [Fact]
    public void Sanitize_RemovesJavascriptHref()
    {
        var result = _sanitizer.Sanitize("<a href=\"javascript:alert('xss')\">Click</a>");
        result.Should().NotContain("javascript:");
    }

    [Fact]
    public void Sanitize_AllowsHttpsHref()
    {
        var result = _sanitizer.Sanitize("<a href=\"https://example.com\">Link</a>");
        result.Should().Contain("href=\"https://example.com\"");
    }

    [Fact]
    public void Sanitize_RemovesOnclickAttributes()
    {
        var result = _sanitizer.Sanitize("<button onclick=\"alert('xss')\">Click</button>");
        result.Should().NotContain("onclick");
    }

    [Fact]
    public void Sanitize_RemovesIframeTags()
    {
        var result = _sanitizer.Sanitize("<iframe src=\"https://evil.com\"></iframe>");
        result.Should().NotContain("<iframe");
    }

    [Fact]
    public void Sanitize_EmptyString_ReturnsEmpty()
    {
        _sanitizer.Sanitize("").Should().BeEmpty();
    }

    [Fact]
    public void Sanitize_PlainText_PassesThrough()
    {
        var result = _sanitizer.Sanitize("Hello, world!");
        result.Should().Be("Hello, world!");
    }
}
