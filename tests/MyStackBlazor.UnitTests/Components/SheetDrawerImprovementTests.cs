using MyStackBlazor.Components.Overlays;

namespace MyStackBlazor.UnitTests.Components;

public class SheetDrawerImprovementTests : TestContext
{
    public SheetDrawerImprovementTests()
    {
        var module = JSInterop.SetupModule("./_content/MyStackBlazor/js/dialog.js");
        module.Mode = JSRuntimeMode.Loose;
        module.Setup<bool>("isTop", _ => true).SetResult(true);
    }

    [Fact]
    public void Sheet_Escape_Closes_And_Releases_Focus()
    {
        bool? result = null;
        var cut = RenderComponent<StackSheet>(p => p
            .Add(c => c.IsOpen, true).Add(c => c.AriaLabel, "Settings")
            .Add(c => c.IsOpenChanged, value => result = value));
        cut.Find("[role=dialog]").GetAttribute("aria-label").Should().Be("Settings");
        cut.Find("[role=dialog]").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        result.Should().BeFalse();
        JSInterop.VerifyInvoke("deactivate");
    }

    [Fact]
    public void Drawer_Escape_Closes_And_Releases_Focus()
    {
        bool? result = null;
        var cut = RenderComponent<StackDrawer>(p => p
            .Add(c => c.IsOpen, true).Add(c => c.IsOpenChanged, value => result = value)
            .AddChildContent("<button>Action</button>"));
        cut.Find("[role=dialog]").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        result.Should().BeFalse();
        JSInterop.VerifyInvoke("deactivate");
    }

    [Fact]
    public void Sheet_Escape_Can_Be_Disabled_And_Uses_Visible_Title()
    {
        var cut = RenderComponent<StackSheet>(p => p
            .Add(c => c.IsOpen, true).Add(c => c.CloseOnEscape, false)
            .Add(c => c.AriaLabelledBy, "settings-title")
            .AddChildContent("<h2 id='settings-title'>Settings</h2>"));
        cut.Find("[role=dialog]").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        cut.Find("[role=dialog]").GetAttribute("aria-labelledby").Should().Be("settings-title");
        cut.Find("[role=dialog]").HasAttribute("aria-label").Should().BeFalse();
    }

    [Fact]
    public void Drawer_Escape_Can_Be_Disabled()
    {
        var cut = RenderComponent<StackDrawer>(p => p
            .Add(c => c.IsOpen, true).Add(c => c.CloseOnEscape, false));
        cut.Find("[role=dialog]").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        cut.Find("[role=dialog]").Should().NotBeNull();
    }

    [Fact]
    public void Closed_Sheet_Does_Not_Initialize_Focus()
    {
        var cut = RenderComponent<StackSheet>();
        cut.FindAll("[role=dialog]").Should().BeEmpty();
        JSInterop.Invocations.Should().BeEmpty();
    }
}
