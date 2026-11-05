using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace MyStackBlazor.E2ETests;

public class EditorTests : PageTest
{
    [Test]
    public async Task Sanitizer_Rejects_Obfuscated_Urls_And_Keeps_Safe_Markup()
    {
        var demoUrl = Environment.GetEnvironmentVariable("DEMO_BASE_URL") ?? "http://127.0.0.1:5080";
        await Page.GotoAsync(new Uri(new Uri(demoUrl), "/forms").AbsoluteUri);

        var result = await Page.EvaluateAsync<string>(
            "html => window.MyStackBlazor.editorSanitize(html)",
            "<a href='java&#x09;script:alert(1)' onclick='alert(1)'>unsafe</a>" +
            "<svg><a href='https://example.com'>foreign</a></svg>" +
            "<a href='/docs' target='_blank'><strong>safe</strong></a>");

        Assert.That(result, Is.EqualTo(
            "<a>unsafe</a><a href=\"/docs\" target=\"_blank\" rel=\"noopener noreferrer\"><strong>safe</strong></a>"));
    }
}
