using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace MyStackBlazor.E2ETests;

public class ThemeTests : PageTest
{
    private static string DemoUrl =>
        new Uri(new Uri(Environment.GetEnvironmentVariable("DEMO_BASE_URL") ??
            "http://127.0.0.1:5080"), "/").AbsoluteUri;

    [Test]
    public async Task Explicit_Theme_Survives_Reload_And_Themes_The_Document()
    {
        await Page.EmulateMediaAsync(new() { ColorScheme = ColorScheme.Light });
        await Page.GotoAsync(DemoUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Switch to dark theme", Exact = true }).ClickAsync();
        await Expect(Page.Locator("html")).ToHaveCSSAsync("color-scheme", "dark");

        await Page.ReloadAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Switch to light theme", Exact = true }))
            .ToBeVisibleAsync();
        await Expect(Page.Locator("html")).ToHaveCSSAsync("color-scheme", "dark");
    }

    [Test]
    public async Task System_Mode_Tracks_Os_Without_Persisting_An_Explicit_Theme()
    {
        await Page.EmulateMediaAsync(new() { ColorScheme = ColorScheme.Dark });
        await Page.GotoAsync(DemoUrl);
        await Expect(Page.Locator("html")).ToHaveCSSAsync("color-scheme", "dark");

        await Page.EmulateMediaAsync(new() { ColorScheme = ColorScheme.Light });
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Switch to dark theme", Exact = true }))
            .ToBeVisibleAsync();
        Assert.That(await Page.EvaluateAsync<string?>("localStorage.getItem('msb-theme')"), Is.Null);
    }
}
