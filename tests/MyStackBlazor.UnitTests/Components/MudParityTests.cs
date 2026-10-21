using System.Globalization;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.Extensions.DependencyInjection;
using MyStackBlazor.Components.Charts;
using MyStackBlazor.Components.Common;
using MyStackBlazor.Components.Data;
using MyStackBlazor.Components.Layout;
using MyStackBlazor.Components.Media;
using MyStackBlazor.Components.Navigation;
using MyStackBlazor.Components.Overlays;

namespace MyStackBlazor.UnitTests.Components;

// ═══════════════════════════════════════════════════════════════════════════════
// Partly covered → completed
// ═══════════════════════════════════════════════════════════════════════════════

public class MessageBoxTests : TestContext
{
    public MessageBoxTests() => Services.AddScoped<DialogService>();

    [Fact]
    public async Task Service_MessageBox_Returns_False_For_No_Button()
    {
        var svc = Services.GetRequiredService<DialogService>();
        var cut = RenderComponent<StackConfirmDialogHost>();

        Task<bool?> answer = null!;
        await cut.InvokeAsync(() => { answer = svc.ShowMessageBoxAsync("Unsaved changes", "Save before closing?", "Save", "Don't save", "Cancel"); });

        cut.FindAll("button").Single(b => b.TextContent == "Don't save").Click();
        (await answer).Should().BeFalse();
        cut.FindAll("[role=alertdialog]").Should().BeEmpty();
    }

    [Fact]
    public async Task Service_MessageBox_Escape_Returns_Null()
    {
        var svc = Services.GetRequiredService<DialogService>();
        var cut = RenderComponent<StackConfirmDialogHost>();
        Task<bool?> answer = null!;
        await cut.InvokeAsync(() => { answer = svc.ShowMessageBoxAsync(new MessageBoxOptions { Title = "Hi", YesText = "OK" }); });

        cut.Find("div.fixed.inset-0.z-50").KeyDown("Escape");
        (await answer).Should().BeNull();
    }

    [Fact]
    public async Task Confirm_Still_Works_Alongside()
    {
        var svc = Services.GetRequiredService<DialogService>();
        var cut = RenderComponent<StackConfirmDialogHost>();
        Task<bool> answer = null!;
        await cut.InvokeAsync(() => { answer = svc.ConfirmAsync("Delete?", "Sure?", "Delete"); });
        cut.FindAll("button").Single(b => b.TextContent.Trim() == "Delete").Click();
        (await answer).Should().BeTrue();
    }

    [Fact]
    public async Task Declarative_ShowAsync_Renders_Custom_Content_And_Resolves_True()
    {
        var cut = RenderComponent<StackMessageBox>(p => p
            .Add(x => x.Title, "Delete 3 files?")
            .Add(x => x.YesText, "Delete")
            .Add(x => x.Destructive, true)
            .AddChildContent("<ul><li>a.txt</li></ul>"));
        Task<bool?> answer = null!;
        await cut.InvokeAsync(() => { answer = cut.Instance.ShowAsync(); });

        cut.Markup.Should().Contain("a.txt");
        cut.FindAll("button").Single(b => b.TextContent == "Delete").ClassList.Should().Contain("bg-destructive");
        cut.FindAll("button").Single(b => b.TextContent == "Delete").Click();
        (await answer).Should().BeTrue();
    }
}

public class ListViewGroupingTests : TestContext
{
    private sealed record Person(string Name, int Team);

    [Fact]
    public void Groups_Render_Subheaders_In_Sorted_Order()
    {
        var people = new[] { new Person("Zed", 2), new Person("Amy", 10), new Person("Bob", 2) };
        var cut = RenderComponent<StackListView<Person>>(p => p
            .Add(x => x.Items, people)
            .Add(x => x.GroupBy, x => x.Team)
            .Add(x => x.SortGroups, true)
            .Add(x => x.ShowGroupCount, true)
            .Add(x => x.ItemTemplate, x => $"<span class='n'>{x.Name}</span>"));

        var groups = cut.FindAll("[role=group]");
        groups.Should().HaveCount(2);
        groups[0].GetAttribute("aria-label").Should().Be("2");   // numeric, not text order ("10" < "2")
        groups[0].TextContent.Should().Contain("(2)");
        cut.FindAll("[role=listitem]").Should().HaveCount(3);
    }

    [Fact]
    public void Empty_List_Shows_EmptyText() =>
        RenderComponent<StackListView<string>>(p => p.Add(x => x.Items, Array.Empty<string>()).Add(x => x.EmptyText, "Nothing"))
            .Markup.Should().Contain("Nothing");
}

public class ImageTests : TestContext
{
    [Fact]
    public void Error_Switches_To_Fallback_Then_Icon()
    {
        var failed = false;
        var cut = RenderComponent<StackImage>(p => p
            .Add(x => x.Src, "broken.png")
            .Add(x => x.Alt, "Product")
            .Add(x => x.Fallback, "placeholder.png")
            .Add(x => x.OnFailed, () => { failed = true; }));

        cut.Find("img").GetAttribute("loading").Should().Be("lazy");
        cut.Find("img").TriggerEvent("onerror", EventArgs.Empty);
        cut.Find("img").GetAttribute("src").Should().Be("placeholder.png");
        cut.Find("img").TriggerEvent("onerror", EventArgs.Empty);
        cut.FindAll("img").Should().BeEmpty();
        cut.Find("[role=img]").GetAttribute("aria-label").Should().Be("Product");
        failed.Should().BeTrue();
    }

    [Fact]
    public void AspectRatio_And_Fit_Applied()
    {
        var cut = RenderComponent<StackImage>(p => p.Add(x => x.Src, "a.png").Add(x => x.Alt, "").Add(x => x.AspectRatio, "16/9").Add(x => x.Fit, "contain"));
        cut.Find("div").GetAttribute("style").Should().Contain("aspect-ratio:16/9");
        cut.Find("img").ClassList.Should().Contain("object-contain");
    }
}

public class LayoutPrimitiveTests : TestContext
{
    [Fact]
    public void Paper_Elevation_And_Outline()
    {
        var cut = RenderComponent<StackPaper>(p => p.Add(x => x.Elevation, 3).Add(x => x.Outlined, true));
        cut.Find("div").ClassList.Should().Contain(["shadow-md", "border", "rounded-lg", "p-4"]);
    }

    [Fact]
    public void Container_MaxWidth() =>
        RenderComponent<StackContainer>(p => p.Add(x => x.MaxWidth, "md"))
            .Find("div").ClassList.Should().Contain(["mx-auto", "max-w-screen-md", "px-4"]);

    [Fact]
    public void Spacer_Is_Flex1() => RenderComponent<StackSpacer>().Find("div").ClassList.Should().Contain("flex-1");

    [Fact]
    public void External_Link_Is_Safe_And_Announced()
    {
        var a = RenderComponent<StackLink>(p => p.Add(x => x.Href, "https://x.dev").Add(x => x.External, true).AddChildContent("Docs")).Find("a");
        a.GetAttribute("target").Should().Be("_blank");
        a.GetAttribute("rel").Should().Be("noopener noreferrer");
        a.TextContent.Should().Contain("(opens in a new tab)");
    }

    [Fact]
    public void ToggleIconButton_Swaps_Icon_And_Label()
    {
        bool? state = null;
        var cut = RenderComponent<StackToggleIconButton>(p => p
            .Add(x => x.Icon, "bell")
            .Add(x => x.ToggledIcon, "bell-off")
            .Add(x => x.Label, "Mute")
            .Add(x => x.ToggledLabel, "Unmute")
            .Add(x => x.ToggledChanged, v => state = v));

        cut.Find("button").GetAttribute("aria-pressed").Should().Be("false");
        cut.Find("button").Click();
        state.Should().BeTrue();
        cut.Find("button").GetAttribute("aria-label").Should().Be("Unmute");
        cut.Find("use").GetAttribute("href").Should().EndWith("#bell-off");
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Missing → added
// ═══════════════════════════════════════════════════════════════════════════════

public class OverlayAndProgressTests : TestContext
{
    [Fact]
    public void Overlay_AutoClose_Hides_On_Click()
    {
        bool? visible = null;
        var cut = RenderComponent<StackOverlay>(p => p
            .Add(x => x.Visible, true)
            .Add(x => x.Absolute, true)
            .Add(x => x.ShowLoader, true)
            .Add(x => x.AutoClose, true)
            .Add(x => x.VisibleChanged, v => visible = v));
        cut.Find("div").ClassList.Should().Contain("absolute");
        cut.Find("[role=status]").Should().NotBeNull();
        cut.Find("div").Click();
        visible.Should().BeFalse();
        cut.Markup.Trim().Should().BeEmpty();
    }

    [Fact]
    public void Circular_Progress_Reports_Value()
    {
        var cut = RenderComponent<StackProgress>(p => p.Add(x => x.Variant, "circular").Add(x => x.Value, 25).Add(x => x.ShowLabel, true));
        cut.Find("[role=progressbar]").GetAttribute("aria-valuenow").Should().Be("25");
        cut.Markup.Should().Contain("25%");
        cut.FindAll("circle").Should().HaveCount(2);
    }

    [Fact]
    public void Indeterminate_Has_No_Value()
    {
        var cut = RenderComponent<StackProgress>(p => p.Add(x => x.Indeterminate, true));
        cut.Find("[role=progressbar]").HasAttribute("aria-valuenow").Should().BeFalse();
        cut.Markup.Should().Contain("msb-progress-indeterminate");
    }

    [Fact]
    public void Linear_Default_Unchanged() =>
        RenderComponent<StackProgress>(p => p.Add(x => x.Value, 50))
            .Find("[role=progressbar] > div").GetAttribute("style").Should().Contain("width: 50%");
}

public class NavGroupAndExitPromptTests : TestContext
{
    [Fact]
    public void NavGroup_Opens_When_Navigating_Into_Section()
    {
        var nav = Services.GetRequiredService<NavigationManager>();
        var cut = RenderComponent<StackNavGroup>(p => p
            .Add(x => x.Title, "Settings")
            .Add(x => x.Href, "settings")
            .AddChildContent("<a>Profile</a>"));

        cut.Find("button").GetAttribute("aria-expanded").Should().Be("false");
        nav.NavigateTo("settings/profile");
        cut.WaitForAssertion(() => cut.Find("button").GetAttribute("aria-expanded").Should().Be("true"));
        cut.Markup.Should().Contain("Profile");

        cut.Find("button").Click();
        cut.Find("button").GetAttribute("aria-expanded").Should().Be("false");
    }

    [Fact]
    public void ExitPrompt_Blocks_Navigation_When_Not_Confirmed()
    {
        var nav = Services.GetRequiredService<NavigationManager>();
        string? asked = null;
        RenderComponent<StackExitPrompt>(p => p
            .Add(x => x.Enabled, true)
            .Add(x => x.ConfirmAsync, url => { asked = url; return Task.FromResult(false); }));

        nav.NavigateTo("elsewhere");
        asked.Should().EndWith("elsewhere");
        var fake = (Bunit.TestDoubles.FakeNavigationManager)nav;
        fake.History.First().State.Should().Be(Bunit.TestDoubles.NavigationState.Prevented);
    }

    [Fact]
    public void ExitPrompt_Disabled_Allows_Navigation()
    {
        var nav = Services.GetRequiredService<NavigationManager>();
        RenderComponent<StackExitPrompt>(p => p.Add(x => x.Enabled, false));
        nav.NavigateTo("elsewhere");
        ((Bunit.TestDoubles.FakeNavigationManager)nav).History.First().State.Should().Be(Bunit.TestDoubles.NavigationState.Succeeded);
    }
}

public class DropContainerTests : TestContext
{
    private sealed record Card(string Title, string Status);

    private IRenderedComponent<StackDropContainer<Card>> Render(List<DropItemEventArgs<Card>> drops, Func<Card, string, bool>? canDrop = null)
    {
        var cards = new List<Card> { new("A", "todo"), new("B", "todo"), new("C", "done") };
        return RenderComponent<StackDropContainer<Card>>(p =>
        {
            p.Add(x => x.Items, cards)
             .Add(x => x.ItemsSelector, (c, z) => c.Status == z)
             .Add(x => x.ItemDropped, a => drops.Add(a))
             .Add(x => x.ItemTemplate, c => $"<span>{c.Title}</span>")
             .Add(x => x.ChildContent, b =>
             {
                 b.OpenComponent<StackDropZone<Card>>(0); b.AddAttribute(1, "Identifier", "todo"); b.AddAttribute(2, "Title", "To do"); b.CloseComponent();
                 b.OpenComponent<StackDropZone<Card>>(3); b.AddAttribute(4, "Identifier", "done"); b.AddAttribute(5, "Title", "Done"); b.CloseComponent();
             });
            if (canDrop is not null) p.Add(x => x.CanDrop, canDrop);
        });
    }

    [Fact]
    public void Drag_Item_To_Other_Zone_Raises_ItemDropped()
    {
        var drops = new List<DropItemEventArgs<Card>>();
        var cut = Render(drops);
        cut.FindAll("section")[0].QuerySelectorAll("[role=listitem]").Should().HaveCount(2);

        cut.FindAll("[role=listitem]")[0].TriggerEvent("ondragstart", new DragEventArgs());
        cut.FindAll("section")[1].TriggerEvent("ondrop", new DragEventArgs());

        drops.Should().ContainSingle();
        drops[0].Item.Title.Should().Be("A");
        drops[0].ZoneIdentifier.Should().Be("done");
        drops[0].Index.Should().Be(1); // appended after "C"
    }

    [Fact]
    public void CanDrop_False_Refuses()
    {
        var drops = new List<DropItemEventArgs<Card>>();
        var cut = Render(drops, (_, zone) => zone != "done");
        cut.FindAll("[role=listitem]")[0].TriggerEvent("ondragstart", new DragEventArgs());
        cut.FindAll("section")[1].TriggerEvent("ondrop", new DragEventArgs());
        drops.Should().BeEmpty();
    }

    [Fact]
    public void Alt_Arrow_Moves_To_Next_Zone_And_Announces()
    {
        var drops = new List<DropItemEventArgs<Card>>();
        var cut = Render(drops);
        cut.FindAll("[role=listitem]")[1].KeyDown(new KeyboardEventArgs { Key = "ArrowRight", AltKey = true });
        drops.Single().Should().Be(new DropItemEventArgs<Card>(new Card("B", "todo"), "done", 1));
        cut.Find("[role=status]").TextContent.Should().Contain("moved to Done");
    }
}

public class BreakpointHiddenTests : TestContext
{
    public BreakpointHiddenTests()
    {
        Services.AddScoped<BreakpointService>();
        var module = JSInterop.SetupModule("./_content/MyStackBlazor/js/interactions.js");
        module.Setup<BreakpointService.WatchResult>("watchBreakpoint", _ => true).SetResult(new(1, "lg"));
    }

    [Theory]
    [InlineData(Breakpoint.Sm, true)]
    [InlineData(Breakpoint.Md, false)]
    [InlineData(Breakpoint.Xxl, false)]
    public void Below_Hides_Smaller_Screens(Breakpoint bp, bool hidden)
    {
        var cut = RenderComponent<StackHidden>(p => p.Add(x => x.Below, Breakpoint.Md).AddChildContent("desktop"));
        cut.Instance.Compute(bp).Should().Be(hidden);
    }

    [Fact]
    public void Breakpoint_Change_Hides_Content()
    {
        var svc = Services.GetRequiredService<BreakpointService>();
        var cut = RenderComponent<StackHidden>(p => p.Add(x => x.Below, Breakpoint.Md).AddChildContent("desktop"));
        cut.Markup.Should().Contain("desktop"); // lg

        svc.OnBreakpointChanged("sm");
        cut.WaitForAssertion(() => cut.Markup.Should().NotContain("desktop"));
        svc.OnBreakpointChanged("lg");
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("desktop"));
    }

    [Fact]
    public void Parse_Maps_Tailwind_Names() =>
        new[] { "xs", "sm", "md", "lg", "xl", "2xl" }.Select(BreakpointService.Parse)
            .Should().Equal(Breakpoint.Xs, Breakpoint.Sm, Breakpoint.Md, Breakpoint.Lg, Breakpoint.Xl, Breakpoint.Xxl);
}

public class TimelineAvatarHighlighterTests : TestContext
{
    [Fact]
    public void Timeline_Renders_Items_With_Time_And_Icon()
    {
        var cut = RenderComponent<StackTimeline>(p => p.Add(x => x.ChildContent, b =>
        {
            b.OpenComponent<StackTimelineItem>(0); b.AddAttribute(1, "Title", "Ordered"); b.AddAttribute(2, "Time", "09:00"); b.AddAttribute(3, "Icon", "check"); b.CloseComponent();
            b.OpenComponent<StackTimelineItem>(4); b.AddAttribute(5, "Title", "Shipped"); b.CloseComponent();
        }));
        cut.FindAll("ol > li").Should().HaveCount(2);
        cut.Find("time").TextContent.Should().Be("09:00");
        cut.FindAll("use").Should().ContainSingle();
    }

    [Fact]
    public void Timeline_Alternate_Uses_Three_Columns()
    {
        var cut = RenderComponent<StackTimeline>(p => p.Add(x => x.Alternate, true).Add(x => x.ChildContent, b =>
        {
            b.OpenComponent<StackTimelineItem>(0); b.AddAttribute(1, "Title", "One"); b.CloseComponent();
        }));
        cut.Find("li").ClassList.Should().Contain("md:grid-cols-[1fr_auto_1fr]");
    }

    [Fact]
    public void AvatarGroup_Shows_Max_And_Overflow()
    {
        var people = Enumerable.Range(1, 6).Select(i => new AvatarItem($"User {i}")).ToList();
        var cut = RenderComponent<StackAvatarGroup>(p => p.Add(x => x.Avatars, people).Add(x => x.Max, 3).Add(x => x.Total, 40));
        cut.FindAll("[title]").Should().HaveCount(3);
        cut.Markup.Should().Contain("+37");
    }

    [Fact]
    public void Highlighter_Marks_All_Terms_Case_Insensitive()
    {
        var cut = RenderComponent<StackHighlighter>(p => p
            .Add(x => x.Text, "New York and new jersey")
            .Add(x => x.HighlightedTexts, new[] { "new", "york" }));
        cut.FindAll("mark").Select(m => m.TextContent).Should().Equal("New", "York", "new");
        cut.Find("span").TextContent.Should().Be("New York and new jersey");
    }

    [Fact]
    public void Highlighter_UntilNextBoundary_And_Regex_Safe()
    {
        var cut = RenderComponent<StackHighlighter>(p => p
            .Add(x => x.Text, "c++ products (pro)")
            .Add(x => x.HighlightedText, "pro")
            .Add(x => x.UntilNextBoundary, true));
        cut.FindAll("mark").Select(m => m.TextContent).Should().Equal("products", "pro");

        RenderComponent<StackHighlighter>(p => p.Add(x => x.Text, "c++ rocks").Add(x => x.HighlightedText, "c++"))
            .Find("mark").TextContent.Should().Be("c++");
    }
}

public class JsUtilityComponentTests : TestContext
{
    private readonly BunitJSModuleInterop _module;

    public JsUtilityComponentTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        _module = JSInterop.SetupModule("./_content/MyStackBlazor/js/interactions.js");
        _module.Mode = JSRuntimeMode.Loose;
    }

    [Fact]
    public async Task Hotkey_Registers_And_Fires()
    {
        var pressed = 0;
        var cut = RenderComponent<StackHotkey>(p => p.Add(x => x.Keys, "mod+k").Add(x => x.OnPressed, () => { pressed++; }));
        _module.VerifyInvoke("registerHotkey").Arguments[1].Should().Be("mod+k");
        await cut.InvokeAsync(() => cut.Instance.OnHotkey());
        pressed.Should().Be(1);
    }

    [Fact]
    public void ScrollToTop_Appears_When_Scrolled()
    {
        var cut = RenderComponent<StackScrollToTop>();
        cut.FindAll("button").Should().BeEmpty();
        cut.InvokeAsync(() => cut.Instance.OnVisibilityChanged(true));
        cut.Find("button").GetAttribute("aria-label").Should().Be("Back to top");
        cut.Find("button").Click();
        _module.VerifyInvoke("scrollToTop");
    }

    [Fact]
    public async Task SwipeArea_Raises_Direction_Callbacks()
    {
        SwipeEventArgs? any = null;
        var left = 0;
        var cut = RenderComponent<StackSwipeArea>(p => p
            .Add(x => x.OnSwipe, a => any = a)
            .Add(x => x.OnSwipeLeft, () => { left++; }));
        await cut.InvokeAsync(() => cut.Instance.HandleSwipe("Left", -120));
        any.Should().Be(new SwipeEventArgs(SwipeDirection.Left, -120));
        left.Should().Be(1);
    }

    [Fact]
    public async Task TableOfContents_Highlights_Active_Section()
    {
        var cut = RenderComponent<StackTableOfContents>(p => p.Add(x => x.Sections, new[]
        {
            new TocSection("intro", "Intro"), new TocSection("setup", "Setup", 3), new TocSection("deep", "Deep", 4),
        }));
        cut.FindAll("a").Should().HaveCount(2); // level 4 hidden by MaxLevel 3
        await cut.InvokeAsync(() => cut.Instance.OnActiveSection("setup"));
        cut.FindAll("a")[1].GetAttribute("aria-current").Should().Be("location");
    }

    [Fact]
    public void FocusTrap_Traps_When_Enabled()
    {
        var fm = JSInterop.SetupModule("./_content/MyStackBlazor.Accessibility/js/focus-manager.js");
        fm.Mode = JSRuntimeMode.Loose; // loose void calls complete immediately
        var cut = RenderComponent<StackFocusTrap>(p => p.AddChildContent("<button>x</button>"));
        fm.VerifyInvoke("trapFocus");
        cut.SetParametersAndRender(p => p.Add(x => x.Enabled, false));
        fm.VerifyInvoke("releaseFocus");
        fm.VerifyInvoke("returnFocus");
    }

    [Fact]
    public void FabMenu_Opens_Runs_Action_And_Closes()
    {
        var shared = 0;
        var cut = RenderComponent<StackFabMenu>(p => p.Add(x => x.ChildContent, b =>
        {
            b.OpenComponent<StackFabMenuItem>(0);
            b.AddAttribute(1, "Icon", "share");
            b.AddAttribute(2, "Label", "Share");
            b.AddAttribute(3, "OnClick", EventCallback.Factory.Create(this, () => shared++));
            b.CloseComponent();
        }));
        cut.FindAll("[role=menu]").Should().BeEmpty();
        cut.Find("button[aria-haspopup=menu]").Click();
        cut.Find("button[aria-haspopup=menu]").GetAttribute("aria-expanded").Should().Be("true");

        cut.Find("[role=menuitem]").Click();
        shared.Should().Be(1);
        cut.FindAll("[role=menu]").Should().BeEmpty();
    }
}

public class NewChartTests : TestContext
{
    [Fact]
    public void HeatMap_Interpolates_Colors_And_Renders_Cells()
    {
        var cut = RenderComponent<StackHeatMapChart>(p => p
            .Add(x => x.Values, new List<IReadOnlyList<double?>> { new double?[] { 0, 5, 10 }, new double?[] { null, 2, 8 } })
            .Add(x => x.LowColor, "#000000")
            .Add(x => x.HighColor, "#ffffff"));
        cut.Instance.ColorFor(5, 0, 10).Should().Be("#808080");
        cut.FindAll("rect").Count.Should().Be(6 + 1); // 6 cells + legend bar
        cut.Markup.Should().Contain("no data");
    }

    [Fact]
    public void TimeSeries_Null_Values_Make_Gaps()
    {
        var d = new DateTime(2026, 1, 1);
        TimePoint[] pts = [new(d, 1), new(d.AddDays(1), 2), new(d.AddDays(2), null), new(d.AddDays(3), 4)];
        StackTimeSeriesChart.Segments(pts).Select(s => s.Count).Should().Equal(2, 1);

        var cut = RenderComponent<StackTimeSeriesChart>(p => p.Add(x => x.Series, new[] { new TimeSeries("Sales", pts) }));
        cut.FindAll("path[stroke-width='2']").Should().HaveCount(2);
    }

    [Fact]
    public void TimeSeries_Ticks_Use_Calendar_Units()
    {
        using var _ = new CultureSwap("en-US");
        var cut = RenderComponent<StackTimeSeriesChart>(p => p.Add(x => x.Series, Array.Empty<TimeSeries>()));
        var ticks = cut.Instance.TimeTicks(new DateTime(2026, 1, 15), new DateTime(2026, 12, 20)).ToList();
        ticks.Should().OnlyContain(t => t.Tick.Day == 1);
        ticks.First().Label.Should().Be("Feb 2026");
    }

    [Fact]
    public void Sankey_Layers_Nodes_And_Ignores_Cycles()
    {
        SankeyLink[] links =
        [
            new("Budget", "Eng", 60), new("Budget", "Sales", 40),
            new("Eng", "Salaries", 50), new("Eng", "Cloud", 10), new("Sales", "Salaries", 40),
            new("Salaries", "Budget", 5), // cycle → ignored
        ];
        var cut = RenderComponent<StackSankeyChart>(p => p.Add(x => x.Links, links));
        var layout = cut.Instance.Layout();
        layout.LayerCount.Should().Be(3);
        layout.Nodes.Single(n => n.Name == "Budget").Layer.Should().Be(0);
        layout.Nodes.Single(n => n.Name == "Salaries").Value.Should().Be(90);
        layout.Links.Should().HaveCount(5);
        cut.FindAll("rect").Should().HaveCount(5);
    }

    private sealed class CultureSwap : IDisposable
    {
        private readonly CultureInfo _old = CultureInfo.CurrentCulture;
        public CultureSwap(string n) => CultureInfo.CurrentCulture = new CultureInfo(n);
        public void Dispose() => CultureInfo.CurrentCulture = _old;
    }
}
