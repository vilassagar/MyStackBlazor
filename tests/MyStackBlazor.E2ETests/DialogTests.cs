using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace MyStackBlazor.E2ETests;

public class DialogTests : PageTest
{
    [Test]
    public async Task Keyboard_Focus_Wraps_And_Escape_Restores_Trigger()
    {
        await Page.GotoAsync(Environment.GetEnvironmentVariable("DEMO_BASE_URL") ?? "http://127.0.0.1:5080/overlays");
        var trigger = Page.Locator("#dialog-trigger");
        await trigger.ClickAsync();
        var dialog = Page.GetByRole(AriaRole.Dialog, new() { Name = "Edit details", Exact = true });
        var input = Page.Locator("#dialog-name");
        var close = dialog.GetByRole(AriaRole.Button, new() { Name = "Close", Exact = true });
        await Expect(input).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync("Shift+Tab");
        await Expect(close).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync("Tab");
        await Expect(input).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync("Escape");
        await Expect(dialog).ToHaveCountAsync(0);
        await Expect(trigger).ToBeFocusedAsync();
        // Repeat to catch leaked listeners after close/reopen.
        await trigger.ClickAsync();
        await Expect(input).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync("Escape");
        await Expect(trigger).ToBeFocusedAsync();
    }

    [Test]
    public async Task Nested_Escape_Closes_Only_The_Top_Dialog()
    {
        await Page.GotoAsync(Environment.GetEnvironmentVariable("DEMO_BASE_URL") ?? "http://127.0.0.1:5080/overlays");
        await Page.Locator("#dialog-trigger").ClickAsync();
        var nestedTrigger = Page.Locator("#nested-trigger");
        await nestedTrigger.ClickAsync();
        var nested = Page.GetByRole(AriaRole.Dialog, new() { Name = "Nested confirmation", Exact = true });
        await Expect(nested.GetByRole(AriaRole.Button, new() { Name = "Close", Exact = true })).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync("Escape");
        await Expect(nested).ToHaveCountAsync(0);
        await Expect(Page.GetByRole(AriaRole.Dialog, new() { Name = "Edit details", Exact = true })).ToBeVisibleAsync();
        await Expect(nestedTrigger).ToBeFocusedAsync();
    }

    [TestCase("Open right sheet", "Edit settings")]
    [TestCase("Open drawer", "Quick actions")]
    public async Task Sheet_And_Drawer_Trap_Focus_And_Restore_On_Escape(string triggerName, string title)
    {
        await Page.GotoAsync(Environment.GetEnvironmentVariable("DEMO_BASE_URL") ?? "http://127.0.0.1:5080/overlays");
        var trigger = Page.GetByRole(AriaRole.Button, new() { Name = triggerName, Exact = true });
        await trigger.ClickAsync();
        var modal = Page.GetByRole(AriaRole.Dialog, new() { Name = title, Exact = true });
        await Expect(modal).ToBeVisibleAsync();
        var buttons = modal.GetByRole(AriaRole.Button);
        await Expect(buttons.First).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync("Shift+Tab");
        await Expect(buttons.Last).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync("Tab");
        await Expect(buttons.First).ToBeFocusedAsync();
        await Page.Keyboard.PressAsync("Escape");
        await Expect(modal).ToHaveCountAsync(0);
        await Expect(trigger).ToBeFocusedAsync();
    }
}
