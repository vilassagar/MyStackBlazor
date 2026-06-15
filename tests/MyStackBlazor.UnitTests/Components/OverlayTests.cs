using MyStackBlazor.Components.Overlays;

namespace MyStackBlazor.UnitTests.Components;

// ═══════════════════════════════════════════════════════════════════════════════
// Dialog
// ═══════════════════════════════════════════════════════════════════════════════

public class DialogTests : TestContext
{
    [Fact]
    public void Dialog_IsOpen_False_DialogNotRendered()
    {
        var cut = RenderComponent<Dialog>(p => p
            .Add(d => d.IsOpen, false)
            .AddChildContent("Dialog body"));

        cut.FindAll("[role=dialog]").Count.Should().Be(0);
    }

    [Fact]
    public void Dialog_IsOpen_True_DialogRendered()
    {
        var cut = RenderComponent<Dialog>(p => p
            .Add(d => d.IsOpen, true)
            .AddChildContent("Dialog body"));

        cut.Find("[role=dialog]").Should().NotBeNull();
    }

    [Fact]
    public void Dialog_IsOpen_True_ChildContentVisible()
    {
        var cut = RenderComponent<Dialog>(p => p
            .Add(d => d.IsOpen, true)
            .AddChildContent("Hello from dialog"));

        cut.Find("[role=dialog]").TextContent.Should().Contain("Hello from dialog");
    }

    [Fact]
    public async Task Dialog_Backdrop_Click_FiresIsOpenChanged_WhenEnabled()
    {
        bool? result = null;
        var cut = RenderComponent<Dialog>(p => p
            .Add(d => d.IsOpen, true)
            .Add(d => d.CloseOnBackdropClick, true)
            .Add(d => d.IsOpenChanged,
                EventCallback.Factory.Create<bool>(this, v => result = v))
            .AddChildContent("Content"));

        // The outer wrapper is the first .fixed; the backdrop is the second .fixed
        var backdrop = cut.FindAll(".fixed")[1];
        await backdrop.ClickAsync(new MouseEventArgs());

        result.Should().BeFalse();
    }

    [Fact]
    public void Dialog_UpdatedIsOpen_True_Then_False_HidesDialog()
    {
        var cut = RenderComponent<Dialog>(p => p
            .Add(d => d.IsOpen, true)
            .AddChildContent("x"));

        cut.Find("[role=dialog]").Should().NotBeNull();

        cut.SetParametersAndRender(p => p.Add(d => d.IsOpen, false));

        cut.FindAll("[role=dialog]").Count.Should().Be(0);
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// AlertDialog
// ═══════════════════════════════════════════════════════════════════════════════

public class AlertDialogTests : TestContext
{
    [Fact]
    public void AlertDialog_IsOpen_False_NotRendered()
    {
        var cut = RenderComponent<AlertDialog>(p => p.Add(a => a.IsOpen, false));
        cut.FindAll("[role=alertdialog]").Count.Should().Be(0);
    }

    [Fact]
    public void AlertDialog_IsOpen_True_Rendered()
    {
        var cut = RenderComponent<AlertDialog>(p => p
            .Add(a => a.IsOpen, true)
            .Add(a => a.Title, "Confirm?")
            .Add(a => a.Description, "This cannot be undone."));

        cut.Find("[role=alertdialog]").Should().NotBeNull();
    }

    [Fact]
    public void AlertDialog_Title_Displayed()
    {
        var cut = RenderComponent<AlertDialog>(p => p
            .Add(a => a.IsOpen, true)
            .Add(a => a.Title, "Delete item")
            .Add(a => a.Description, "Are you sure?"));

        cut.Markup.Should().Contain("Delete item");
    }

    [Fact]
    public void AlertDialog_Description_Displayed()
    {
        var cut = RenderComponent<AlertDialog>(p => p
            .Add(a => a.IsOpen, true)
            .Add(a => a.Title, "Warning")
            .Add(a => a.Description, "This action is irreversible."));

        cut.Markup.Should().Contain("This action is irreversible.");
    }

    [Fact]
    public void AlertDialog_ConfirmText_AppearedOnButton()
    {
        var cut = RenderComponent<AlertDialog>(p => p
            .Add(a => a.IsOpen, true)
            .Add(a => a.Title, "Delete")
            .Add(a => a.ConfirmText, "Yes, delete"));

        cut.Markup.Should().Contain("Yes, delete");
    }

    [Fact]
    public void AlertDialog_CancelText_AppearedOnButton()
    {
        var cut = RenderComponent<AlertDialog>(p => p
            .Add(a => a.IsOpen, true)
            .Add(a => a.Title, "Delete")
            .Add(a => a.CancelText, "No, keep it"));

        cut.Markup.Should().Contain("No, keep it");
    }

    [Fact]
    public async Task AlertDialog_ConfirmButton_FiresOnConfirm()
    {
        bool confirmed = false;
        var cut = RenderComponent<AlertDialog>(p => p
            .Add(a => a.IsOpen, true)
            .Add(a => a.Title, "Confirm")
            .Add(a => a.ConfirmText, "Yes")
            .Add(a => a.OnConfirm,
                EventCallback.Factory.Create(this, () => confirmed = true)));

        var confirmBtn = cut.FindAll("button")
            .First(b => b.TextContent.Trim() == "Yes");
        await confirmBtn.ClickAsync(new MouseEventArgs());

        confirmed.Should().BeTrue();
    }

    [Fact]
    public async Task AlertDialog_CancelButton_FiresOnCancel()
    {
        bool cancelled = false;
        var cut = RenderComponent<AlertDialog>(p => p
            .Add(a => a.IsOpen, true)
            .Add(a => a.Title, "Confirm")
            .Add(a => a.CancelText, "Cancel")
            .Add(a => a.OnCancel,
                EventCallback.Factory.Create(this, () => cancelled = true)));

        var cancelBtn = cut.FindAll("button")
            .First(b => b.TextContent.Trim() == "Cancel");
        await cancelBtn.ClickAsync(new MouseEventArgs());

        cancelled.Should().BeTrue();
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Tooltip
// ═══════════════════════════════════════════════════════════════════════════════

public class TooltipTests : TestContext
{
    [Fact]
    public void Tooltip_Renders_Container()
    {
        var cut = RenderComponent<Tooltip>(p => p
            .Add(t => t.Text, "Helpful hint")
            .AddChildContent("<button>Hover me</button>"));

        cut.Find("div").Should().NotBeNull();
    }

    [Fact]
    public async Task Tooltip_Text_PresentInMarkup()
    {
        var cut = RenderComponent<Tooltip>(p => p
            .Add(t => t.Text, "Save document")
            .AddChildContent("<button>Save</button>"));

        // Tooltip text is only rendered after mouseenter sets _visible = true
        await cut.Find("div").TriggerEventAsync("onmouseenter", new MouseEventArgs());

        cut.Markup.Should().Contain("Save document");
    }

    [Fact]
    public void Tooltip_ChildContent_Rendered()
    {
        var cut = RenderComponent<Tooltip>(p => p
            .Add(t => t.Text, "hint")
            .AddChildContent("<span id='trigger'>Hover</span>"));

        cut.Find("#trigger").Should().NotBeNull();
    }

    [Theory]
    [InlineData("top")]
    [InlineData("bottom")]
    [InlineData("left")]
    [InlineData("right")]
    public void Tooltip_Side_DoesNotThrow(string side)
    {
        var act = () => RenderComponent<Tooltip>(p => p
            .Add(t => t.Side, side)
            .Add(t => t.Text, "hint")
            .AddChildContent("x"));

        act.Should().NotThrow();
    }
}
