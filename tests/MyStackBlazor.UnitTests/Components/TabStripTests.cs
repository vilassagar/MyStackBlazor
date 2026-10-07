using Microsoft.AspNetCore.Components.Rendering;
using MyStackBlazor.Components.Navigation;

namespace MyStackBlazor.UnitTests.Components;

public class TabStripTests : TestContext
{
    public TabStripTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private record TabDef(string Id, string Title, bool Disabled = false, bool Visible = true, bool Closeable = false);

    private static readonly TabDef[] ThreeTabs =
    [
        new("one", "One"),
        new("two", "Two"),
        new("three", "Three"),
    ];

    private static RenderFragment Tabs(params TabDef[] tabs) => builder =>
    {
        foreach (var t in tabs)
        {
            builder.OpenComponent<StackTabStripTab>(0);
            builder.SetKey(t.Id);
            builder.AddAttribute(1, nameof(StackTabStripTab.Id), t.Id);
            builder.AddAttribute(2, nameof(StackTabStripTab.Title), t.Title);
            builder.AddAttribute(3, nameof(StackTabStripTab.Disabled), t.Disabled);
            builder.AddAttribute(4, nameof(StackTabStripTab.Visible), t.Visible);
            builder.AddAttribute(5, nameof(StackTabStripTab.Closeable), t.Closeable);
            builder.AddAttribute(6, nameof(StackTabStripTab.ChildContent),
                (RenderFragment)(b => b.AddMarkupContent(0, $"<p class=\"content\">{t.Title} content</p>")));
            builder.CloseComponent();
        }
    };

    private IRenderedComponent<StackTabStrip> Render(
        Action<ComponentParameterCollectionBuilder<StackTabStrip>>? configure = null, params TabDef[] tabs) =>
        RenderComponent<StackTabStrip>(p =>
        {
            p.Add(s => s.ChildContent, Tabs(tabs.Length == 0 ? ThreeTabs : tabs));
            configure?.Invoke(p);
        });

    private static string Content(IRenderedComponent<StackTabStrip> cut) =>
        cut.FindAll("[role=tabpanel]:not([hidden]) .content").Single().TextContent;

    [Fact]
    public void Renders_Headers_And_First_Tab_Content()
    {
        var cut = Render();
        cut.FindAll("[role=tab]").Select(t => t.TextContent.Trim()).Should().Equal("One", "Two", "Three");
        Content(cut).Should().Be("One content");
        cut.FindAll("[role=tabpanel]").Should().HaveCount(1);
    }

    [Fact]
    public void ActiveTabIndex_Selects_Tab()
    {
        var cut = Render(p => p.Add(s => s.ActiveTabIndex, 2));
        Content(cut).Should().Be("Three content");
        cut.FindAll("[role=tab]")[2].GetAttribute("aria-selected").Should().Be("true");
    }

    [Fact]
    public void ActiveTabId_Selects_Tab_And_Wins_Over_Index()
    {
        var cut = Render(p => p.Add(s => s.ActiveTabId, "two").Add(s => s.ActiveTabIndex, 2));
        Content(cut).Should().Be("Two content");
    }

    [Fact]
    public void Click_Fires_ActiveTabIdChanged_And_IndexChanged()
    {
        string? id = null;
        int? index = null;
        var cut = Render(p => p
            .Add(s => s.ActiveTabIdChanged, v => id = v)
            .Add(s => s.ActiveTabIndexChanged, v => index = v));

        cut.FindAll("[role=tab]")[1].Click();

        id.Should().Be("two");
        index.Should().Be(1);
        Content(cut).Should().Be("Two content");
    }

    [Fact]
    public void Disabled_Tab_Cannot_Be_Selected()
    {
        var cut = Render(null, new("a", "A"), new("b", "B", Disabled: true));
        var disabled = cut.FindAll("[role=tab]")[1];
        disabled.GetAttribute("aria-disabled").Should().Be("true");
        disabled.Click();
        Content(cut).Should().Be("A content");
    }

    [Fact]
    public void Hidden_Tab_Not_Rendered()
    {
        var cut = Render(null, new("a", "A"), new("b", "B", Visible: false), new("c", "C"));
        cut.FindAll("[role=tab]").Select(t => t.TextContent.Trim()).Should().Equal("A", "C");
    }

    [Fact]
    public void PersistTabContent_Keeps_Inactive_Panels_Hidden()
    {
        var cut = Render(p => p.Add(s => s.PersistTabContent, true));
        cut.FindAll("[role=tabpanel]").Should().HaveCount(3);
        cut.FindAll("[role=tabpanel][hidden]").Should().HaveCount(2);
        Content(cut).Should().Be("One content");
    }

    [Fact]
    public void Arrow_Keys_Move_Selection_Skipping_Disabled()
    {
        var cut = Render(null, new("a", "A"), new("b", "B", Disabled: true), new("c", "C"));
        cut.Find("[role=tablist]").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        Content(cut).Should().Be("C content");
        cut.Find("[role=tablist]").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        Content(cut).Should().Be("A content");
        cut.Find("[role=tablist]").KeyDown(new KeyboardEventArgs { Key = "End" });
        Content(cut).Should().Be("C content");
        cut.Find("[role=tablist]").KeyDown(new KeyboardEventArgs { Key = "Home" });
        Content(cut).Should().Be("A content");
    }

    [Fact]
    public void Vertical_Position_Uses_Up_Down_Keys()
    {
        var cut = Render(p => p.Add(s => s.TabPosition, TabPosition.Left));
        cut.Find("[role=tablist]").GetAttribute("aria-orientation").Should().Be("vertical");
        cut.Find("[role=tablist]").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        Content(cut).Should().Be("One content");
        cut.Find("[role=tablist]").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        Content(cut).Should().Be("Two content");
    }

    [Theory]
    [InlineData(TabPosition.Top, "flex-col")]
    [InlineData(TabPosition.Bottom, "flex-col-reverse")]
    [InlineData(TabPosition.Left, "flex-row")]
    [InlineData(TabPosition.Right, "flex-row-reverse")]
    public void TabPosition_Sets_Layout(TabPosition position, string expected)
    {
        var cut = Render(p => p.Add(s => s.TabPosition, position).Add(s => s.Id, "ts"));
        cut.Find("#ts").ClassList.Should().Contain(expected);
    }

    [Fact]
    public void TabAlignment_Stretch_Makes_Headers_Grow()
    {
        var cut = Render(p => p.Add(s => s.TabAlignment, TabStripTabAlignment.Stretch));
        cut.FindAll("[role=presentation]").Should().OnlyContain(h => h.ClassList.Contains("flex-1"));
    }

    [Fact]
    public void Scrollable_Shows_Buttons_By_Position()
    {
        var split = Render(p => p.Add(s => s.Scrollable, true));
        split.FindAll("button[aria-label^=Scroll]").Select(b => b.GetAttribute("aria-label"))
             .Should().Equal("Scroll left", "Scroll right");

        var hidden = Render(p => p
            .Add(s => s.Scrollable, true)
            .Add(s => s.ScrollButtonsVisibility, TabStripScrollButtonsVisibility.Hidden));
        hidden.FindAll("button[aria-label^=Scroll]").Should().BeEmpty();
        hidden.Find("[role=tablist]").ClassList.Should().Contain("overflow-x-auto");
    }

    [Fact]
    public void Closeable_Tab_Fires_OnTabClose_And_Activates_Neighbour()
    {
        TabStripTabCloseEventArgs? closed = null;
        var cut = Render(p => p.Add(s => s.OnTabClose, (TabStripTabCloseEventArgs a) => closed = a),
            new("a", "A", Closeable: true), new("b", "B"));

        cut.Find("button[aria-label='Close A']").Click();

        closed!.TabId.Should().Be("a");
        closed.TabIndex.Should().Be(0);
        cut.FindAll("[role=tab]").Select(t => t.TextContent.Trim()).Should().Equal("B");
        Content(cut).Should().Be("B content");
    }

    [Fact]
    public void OnTabClose_Can_Cancel()
    {
        var cut = Render(p => p.Add(s => s.OnTabClose, (TabStripTabCloseEventArgs a) => a.IsCancelled = true),
            new("a", "A", Closeable: true), new("b", "B"));
        cut.Find("button[aria-label='Close A']").Click();
        cut.FindAll("[role=tab]").Should().HaveCount(2);
    }

    [Fact]
    public void HeaderTemplate_And_Content_Are_Used()
    {
        var cut = RenderComponent<StackTabStrip>(p => p.Add(s => s.ChildContent, (RenderFragment)(b =>
        {
            b.OpenComponent<StackTabStripTab>(0);
            b.AddAttribute(1, nameof(StackTabStripTab.HeaderTemplate), (RenderFragment)(h => h.AddMarkupContent(0, "<b>Custom</b>")));
            b.AddAttribute(2, nameof(StackTabStripTab.Content), (RenderFragment)(c => c.AddMarkupContent(0, "<i>Body</i>")));
            b.CloseComponent();
        })));

        cut.Find("[role=tab] b").TextContent.Should().Be("Custom");
        cut.Find("[role=tabpanel] i").TextContent.Should().Be("Body");
    }

    [Fact]
    public void Aria_Links_Header_And_Panel()
    {
        var cut = Render(p => p.Add(s => s.Id, "ts"));
        var tab = cut.FindAll("[role=tab]")[0];
        var panel = cut.Find("[role=tabpanel]");
        tab.GetAttribute("aria-controls").Should().Be(panel.Id);
        panel.GetAttribute("aria-labelledby").Should().Be(tab.Id);
        tab.GetAttribute("tabindex").Should().Be("0");
        cut.FindAll("[role=tab]")[1].GetAttribute("tabindex").Should().Be("-1");
    }

    [Fact]
    public void Size_Width_Height_Applied()
    {
        var cut = Render(p => p
            .Add(s => s.Id, "ts")
            .Add(s => s.Size, TabStripTheme.Size.Large)
            .Add(s => s.Width, "500px")
            .Add(s => s.Height, "300px"));
        cut.Find("#ts").GetAttribute("style").Should().Be("width:500px;height:300px");
        cut.FindAll("[role=tab]")[0].ClassList.Should().Contain("py-3");
    }
}
