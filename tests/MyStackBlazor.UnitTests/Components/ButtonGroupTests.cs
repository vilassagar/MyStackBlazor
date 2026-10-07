using Microsoft.AspNetCore.Components.Rendering;
using MyStackBlazor.Components.Common;
using MyStackBlazor.Components.Navigation;

namespace MyStackBlazor.UnitTests.Components;

public class ButtonGroupTests : TestContext
{
    private static RenderFragment Toggles(params (string Text, bool Selected, Action<bool>? Changed)[] toggles) => b =>
    {
        var seq = 0;
        foreach (var (text, selected, changed) in toggles)
        {
            b.OpenComponent<StackButtonGroupToggleButton>(seq++);
            b.AddAttribute(seq++, nameof(StackButtonGroupToggleButton.Selected), selected);
            if (changed is not null)
                b.AddAttribute(seq++, nameof(StackButtonGroupToggleButton.SelectedChanged), EventCallback.Factory.Create(new object(), changed));
            b.AddAttribute(seq++, nameof(StackButtonGroupToggleButton.ChildContent), (RenderFragment)(c => c.AddContent(0, text)));
            b.CloseComponent();
        }
    };

    private static string[] Pressed(IRenderedFragment cut) =>
        cut.FindAll("button[aria-pressed=true]").Select(x => x.TextContent.Trim()).ToArray();

    [Fact]
    public void Legacy_StackButton_Children_Still_Render()
    {
        var cut = RenderComponent<StackButtonGroup>(p => p.AddChildContent<StackButton>(b => b.AddChildContent("Left")));
        cut.Find("[role=group]").ClassList.Should().Contain("inline-flex");
        cut.Find("button").TextContent.Should().Contain("Left");
    }

    [Fact]
    public void Single_Mode_Selecting_Deselects_Others()
    {
        bool? firstChanged = null;
        var cut = RenderComponent<StackButtonGroup>(p => p.Add(g => g.ChildContent,
            Toggles(("Day", true, v => firstChanged = v), ("Week", false, null))));

        cut.FindAll("button")[1].Click();

        Pressed(cut).Should().Equal("Week");
        firstChanged.Should().BeFalse();
    }

    [Fact]
    public void Single_Mode_Clicking_Selected_Keeps_It_Selected()
    {
        var cut = RenderComponent<StackButtonGroup>(p => p.Add(g => g.ChildContent, Toggles(("Day", true, null))));
        cut.Find("button").Click();
        Pressed(cut).Should().Equal("Day");
    }

    [Fact]
    public void Multiple_Mode_Toggles_Independently()
    {
        var cut = RenderComponent<StackButtonGroup>(p => p
            .Add(g => g.SelectionMode, ButtonGroupSelectionMode.Multiple)
            .Add(g => g.ChildContent, Toggles(("B", false, null), ("I", false, null), ("U", true, null))));

        cut.FindAll("button")[0].Click();
        cut.FindAll("button")[1].Click();
        cut.FindAll("button")[2].Click();

        Pressed(cut).Should().Equal("B", "I");
    }

    [Fact]
    public void SelectedChanged_Fires_On_Click()
    {
        bool? changed = null;
        var cut = RenderComponent<StackButtonGroupToggleButton>(p => p
            .Add(t => t.SelectedChanged, v => changed = v)
            .AddChildContent("Solo"));
        cut.Find("button").Click();
        changed.Should().BeTrue();
    }

    [Fact]
    public void Group_Enabled_False_Disables_All_Buttons()
    {
        var cut = RenderComponent<StackButtonGroup>(p => p
            .Add(g => g.Enabled, false)
            .Add(g => g.ChildContent, Toggles(("A", false, null), ("B", false, null))));
        cut.FindAll("button").Should().OnlyContain(x => x.HasAttribute("disabled"));
        cut.Find("[role=group]").GetAttribute("aria-disabled").Should().Be("true");
    }

    [Fact]
    public void Group_Enabled_Change_Refreshes_Buttons()
    {
        var cut = RenderComponent<StackButtonGroup>(p => p.Add(g => g.ChildContent, Toggles(("A", false, null))));
        cut.Find("button").HasAttribute("disabled").Should().BeFalse();
        cut.SetParametersAndRender(p => p.Add(g => g.Enabled, false));
        cut.Find("button").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void Button_OnClick_Fires_And_Respects_Enabled_And_Visible()
    {
        var clicks = 0;
        var cut = RenderComponent<StackButtonGroup>(p => p.Add(g => g.ChildContent, (RenderFragment)(b =>
        {
            b.OpenComponent<StackButtonGroupButton>(0);
            b.AddAttribute(1, nameof(StackButtonGroupButton.OnClick), EventCallback.Factory.Create<MouseEventArgs>(this, () => clicks++));
            b.AddAttribute(2, nameof(StackButtonGroupButton.ChildContent), (RenderFragment)(c => c.AddContent(0, "Go")));
            b.CloseComponent();
            b.OpenComponent<StackButtonGroupButton>(3);
            b.AddAttribute(4, nameof(StackButtonGroupButton.Enabled), false);
            b.CloseComponent();
            b.OpenComponent<StackButtonGroupButton>(5);
            b.AddAttribute(6, nameof(StackButtonGroupButton.Visible), false);
            b.CloseComponent();
        })));

        cut.FindAll("button").Should().HaveCount(2);
        cut.FindAll("button")[0].Click();
        clicks.Should().Be(1);
        cut.FindAll("button")[1].HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void Icon_Title_TabIndex_Rendered()
    {
        var cut = RenderComponent<StackButtonGroupButton>(p => p
            .Add(b => b.Icon, "plus")
            .Add(b => b.Title, "Add")
            .Add(b => b.TabIndex, 3));
        var button = cut.Find("button");
        button.QuerySelector("svg").Should().NotBeNull();
        button.GetAttribute("title").Should().Be("Add");
        button.GetAttribute("tabindex").Should().Be("3");
    }

    [Fact]
    public void Appearance_Parameters_Apply_Classes()
    {
        var cut = RenderComponent<StackButtonGroup>(p => p
            .Add(g => g.Size, ButtonGroupTheme.Size.Large)
            .Add(g => g.Rounded, ButtonGroupTheme.Rounded.Full)
            .Add(g => g.FillMode, ButtonGroupTheme.FillMode.Outline)
            .Add(g => g.ThemeColor, ButtonGroupTheme.ThemeColor.Primary)
            .Add(g => g.ChildContent, Toggles(("A", false, null), ("B", true, null))));

        var buttons = cut.FindAll("button");
        buttons[0].ClassList.Should().Contain(["h-12", "rounded-full", "border-primary", "text-primary"]);
        buttons[1].ClassList.Should().Contain(["bg-primary", "text-primary-foreground"]);
    }

    [Fact]
    public void Button_ThemeColor_Overrides_Group()
    {
        var cut = RenderComponent<StackButtonGroup>(p => p
            .Add(g => g.ThemeColor, ButtonGroupTheme.ThemeColor.Primary)
            .Add(g => g.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<StackButtonGroupButton>(0);
                b.AddAttribute(1, nameof(StackButtonGroupButton.ThemeColor), ButtonGroupTheme.ThemeColor.Error);
                b.CloseComponent();
            })));
        cut.Find("button").ClassList.Should().Contain("bg-destructive");
    }

    [Fact]
    public void Width_Stretches_Buttons()
    {
        var cut = RenderComponent<StackButtonGroup>(p => p.Add(g => g.Width, "100%"));
        var group = cut.Find("[role=group]");
        group.GetAttribute("style").Should().Be("width:100%");
        group.ClassList.Should().Contain("[&>*]:flex-1");
    }
}
