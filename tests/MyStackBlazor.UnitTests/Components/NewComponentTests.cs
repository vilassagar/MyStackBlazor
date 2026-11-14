using Microsoft.Extensions.DependencyInjection;
using Bunit.TestDoubles;
using MyStackBlazor.Components.Charts;
using MyStackBlazor.Components.Common;
using MyStackBlazor.Components.Data;
using MyStackBlazor.Components.Form;
using MyStackBlazor.Components.Layout;
using MyStackBlazor.Components.Navigation;

namespace MyStackBlazor.UnitTests.Components;

// ═══════════════════════════════════════════════════════════════════════════════
// TagInput
// ═══════════════════════════════════════════════════════════════════════════════

public class TagInputTests : TestContext
{
    [Fact]
    public void TagInput_RendersInitialValues_AsListItems()
    {
        var cut = RenderComponent<StackTagInput>(p => p
            .Add(t => t.Values, new[] { "blazor", "maui" }));

        cut.FindAll("li").Select(li => li.TextContent.Trim()).Should().Equal("blazor", "maui");
    }

    [Fact]
    public void TagInput_Enter_AddsTrimmedTag_AndRaisesValuesChanged()
    {
        IReadOnlyList<string>? latest = null;
        var cut = RenderComponent<StackTagInput>(p => p
            .Add(t => t.ValuesChanged, (IReadOnlyList<string> v) => latest = v));

        cut.Find("input").Input("  dotnet  ");
        cut.Find("input").KeyDown(new KeyboardEventArgs { Key = "Enter" });

        latest.Should().Equal("dotnet");
        cut.FindAll("li").Should().HaveCount(1);
    }

    [Fact]
    public void TagInput_PastingCommaSeparatedText_AddsEachTag()
    {
        IReadOnlyList<string>? latest = null;
        var cut = RenderComponent<StackTagInput>(p => p
            .Add(t => t.ValuesChanged, (IReadOnlyList<string> v) => latest = v));

        cut.Find("input").Input("a, b, c");

        // "c" stays in the box until Enter / separator / blur.
        latest.Should().Equal("a", "b");
    }

    [Fact]
    public void TagInput_RejectsCaseInsensitiveDuplicates_ByDefault()
    {
        var changes = 0;
        var cut = RenderComponent<StackTagInput>(p => p
            .Add(t => t.Values, new[] { "Blazor" })
            .Add(t => t.ValuesChanged, (IReadOnlyList<string> _) => changes++));

        cut.Find("input").Input("blazor");
        cut.Find("input").KeyDown(new KeyboardEventArgs { Key = "Enter" });

        changes.Should().Be(0);
    }

    [Fact]
    public void TagInput_Validate_RejectsInvalidTags()
    {
        var changes = 0;
        var cut = RenderComponent<StackTagInput>(p => p
            .Add(t => t.Validate, (Func<string, bool>)(s => s.Contains('@')))
            .Add(t => t.ValuesChanged, (IReadOnlyList<string> _) => changes++));

        cut.Find("input").Input("not-an-email");
        cut.Find("input").KeyDown(new KeyboardEventArgs { Key = "Enter" });

        changes.Should().Be(0);
    }

    [Fact]
    public void TagInput_BackspaceOnEmptyInput_RemovesLastTag()
    {
        string? removed = null;
        var cut = RenderComponent<StackTagInput>(p => p
            .Add(t => t.Values, new[] { "one", "two" })
            .Add(t => t.OnTagRemoved, (string s) => removed = s));

        cut.Find("input").KeyDown(new KeyboardEventArgs { Key = "Backspace" });

        removed.Should().Be("two");
    }

    [Fact]
    public void TagInput_RemoveButton_RemovesThatTag()
    {
        IReadOnlyList<string>? latest = null;
        var cut = RenderComponent<StackTagInput>(p => p
            .Add(t => t.Values, new[] { "one", "two" })
            .Add(t => t.ValuesChanged, (IReadOnlyList<string> v) => latest = v));

        cut.Find("button[aria-label='Remove one']").Click();

        latest.Should().Equal("two");
    }

    [Fact]
    public void TagInput_MaxTagsReached_HidesInput()
    {
        var cut = RenderComponent<StackTagInput>(p => p
            .Add(t => t.Values, new[] { "a", "b" })
            .Add(t => t.MaxTags, 2));

        cut.FindAll("input").Should().BeEmpty();
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// CopyButton
// ═══════════════════════════════════════════════════════════════════════════════

public class CopyButtonTests : TestContext
{
    [Fact]
    public void CopyButton_Click_CopiesText_AndShowsCopiedState()
    {
        JSInterop.SetupVoid("MyStackBlazor.copyToClipboard", "hello").SetVoidResult();
        string? copied = null;
        var cut = RenderComponent<StackCopyButton>(p => p
            .Add(c => c.Text, "hello")
            .Add(c => c.OnCopied, (string s) => copied = s));

        cut.Find("button").Click();

        copied.Should().Be("hello");
        cut.Find("button").GetAttribute("data-state").Should().Be("copied");
        cut.Find("button").GetAttribute("aria-label").Should().Be("Copied");
    }

    [Fact]
    public void CopyButton_EmptyText_DoesNotCallJs()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        var cut = RenderComponent<StackCopyButton>(p => p.Add(c => c.Text, ""));

        cut.Find("button").Click();

        JSInterop.Invocations.Should().BeEmpty();
    }

    [Fact]
    public void CopyButton_ShowText_RendersLabel()
    {
        var cut = RenderComponent<StackCopyButton>(p => p
            .Add(c => c.Text, "x")
            .Add(c => c.ShowText, true)
            .Add(c => c.Label, "Copy code"));

        cut.Find("button").TextContent.Should().Contain("Copy code");
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// StatCard
// ═══════════════════════════════════════════════════════════════════════════════

public class StatCardTests : TestContext
{
    [Fact]
    public void StatCard_RendersLabelAndValue()
    {
        var cut = RenderComponent<StackStatCard>(p => p
            .Add(s => s.Label, "Revenue")
            .Add(s => s.Value, "₹4.2L"));

        cut.Find("[role=group]").GetAttribute("aria-label").Should().Be("Revenue");
        cut.Markup.Should().Contain("₹4.2L");
    }

    [Theory]
    [InlineData(5, false, "up", "text-green-600")]
    [InlineData(-5, false, "down", "text-destructive")]
    [InlineData(-5, true, "down", "text-green-600")]
    [InlineData(0, false, "flat", "text-muted-foreground")]
    public void StatCard_TrendColour_FollowsInvertTrend(double change, bool invert, string trend, string expectedClass)
    {
        var cut = RenderComponent<StackStatCard>(p => p
            .Add(s => s.Label, "Metric")
            .Add(s => s.Change, (decimal)change)
            .Add(s => s.InvertTrend, invert));

        var badge = cut.Find("[data-trend]");
        badge.GetAttribute("data-trend").Should().Be(trend);
        badge.ClassList.Should().Contain(expectedClass);
    }

    [Fact]
    public void StatCard_Loading_HidesValue()
    {
        var cut = RenderComponent<StackStatCard>(p => p
            .Add(s => s.Label, "Users")
            .Add(s => s.Value, "1,024")
            .Add(s => s.Loading, true));

        cut.Markup.Should().NotContain("1,024");
        cut.Find(".animate-pulse").Should().NotBeNull();
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// DescriptionList
// ═══════════════════════════════════════════════════════════════════════════════

public class DescriptionListTests : TestContext
{
    [Fact]
    public void DescriptionList_Items_RenderAsTermsAndDescriptions()
    {
        var cut = RenderComponent<StackDescriptionList>(p => p
            .Add(d => d.Items, new Dictionary<string, string?> { ["Name"] = "Asha", ["Email"] = null }));

        cut.FindAll("dt").Select(e => e.TextContent).Should().Equal("Name", "Email");
        cut.FindAll("dd").Select(e => e.TextContent.Trim()).Should().Equal("Asha", "—");
    }

    [Fact]
    public void DescriptionList_ChildItems_RenderCustomContent()
    {
        var cut = RenderComponent<StackDescriptionList>(p => p
            .Add(d => d.Orientation, "vertical")
            .Add(d => d.Columns, 2)
            .AddChildContent<StackDescriptionItem>(i => i
                .Add(x => x.Term, "Status")
                .AddChildContent("<strong>Active</strong>")));

        cut.Find("dl").ClassList.Should().Contain("md:grid-cols-2");
        cut.Find("dd strong").TextContent.Should().Be("Active");
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// BottomNavigation
// ═══════════════════════════════════════════════════════════════════════════════

public class BottomNavigationTests : TestContext
{
    [Fact]
    public void BottomNavigation_ValueMode_ClickSelectsItem()
    {
        string? selected = null;
        var cut = RenderComponent<StackBottomNavigation>(p => p
            .Add(n => n.Value, "home")
            .Add(n => n.ValueChanged, (string? v) => selected = v)
            .AddChildContent<StackBottomNavigationItem>(i => i.Add(x => x.Value, "home").Add(x => x.Label, "Home").Add(x => x.Icon, "house"))
            .AddChildContent<StackBottomNavigationItem>(i => i.Add(x => x.Value, "search").Add(x => x.Label, "Search").Add(x => x.Icon, "search")));

        cut.FindAll("button")[1].Click();

        selected.Should().Be("search");
        cut.FindAll("button")[1].GetAttribute("aria-current").Should().Be("page");
        cut.FindAll("button")[0].HasAttribute("aria-current").Should().BeFalse();
    }

    [Fact]
    public void BottomNavigation_RouteMode_MarksMatchingHrefActive()
    {
        Services.GetRequiredService<FakeNavigationManager>().NavigateTo("/orders/42");

        var cut = RenderComponent<StackBottomNavigation>(p => p
            .AddChildContent<StackBottomNavigationItem>(i => i.Add(x => x.Href, "/").Add(x => x.Match, Microsoft.AspNetCore.Components.Routing.NavLinkMatch.All).Add(x => x.Label, "Home"))
            .AddChildContent<StackBottomNavigationItem>(i => i.Add(x => x.Href, "/orders").Add(x => x.Label, "Orders")));

        var links = cut.FindAll("a");
        links[0].HasAttribute("aria-current").Should().BeFalse();
        links[1].GetAttribute("aria-current").Should().Be("page");
    }

    [Fact]
    public void BottomNavigation_Badge_RendersCount()
    {
        var cut = RenderComponent<StackBottomNavigation>(p => p
            .AddChildContent<StackBottomNavigationItem>(i => i.Add(x => x.Value, "inbox").Add(x => x.Label, "Inbox").Add(x => x.Badge, "3")));

        cut.Find("button").TextContent.Should().Contain("3");
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Countdown
// ═══════════════════════════════════════════════════════════════════════════════

public class CountdownTests : TestContext
{
    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }

    private static readonly DateTimeOffset Now = new(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);

    [Fact]
    public void Countdown_Compact_ShowsMinutesAndSeconds()
    {
        var cut = RenderComponent<StackCountdown>(p => p
            .Add(c => c.TimeProvider, new FixedTimeProvider(Now))
            .Add(c => c.Target, Now.AddSeconds(90))
            .Add(c => c.Format, "compact"));

        cut.Find("[role=timer]").TextContent.Trim().Should().Be("01:30");
    }

    [Fact]
    public void Countdown_Blocks_ShowsDaysOnlyWhenNeeded()
    {
        var cut = RenderComponent<StackCountdown>(p => p
            .Add(c => c.TimeProvider, new FixedTimeProvider(Now))
            .Add(c => c.Target, Now.AddDays(2).AddHours(3)));

        cut.Markup.Should().Contain("days");
        cut.Instance.Remaining.Should().Be(TimeSpan.FromHours(51));
    }

    [Fact]
    public void Countdown_PastTarget_ShowsCompletedContent()
    {
        var cut = RenderComponent<StackCountdown>(p => p
            .Add(c => c.TimeProvider, new FixedTimeProvider(Now))
            .Add(c => c.Target, Now.AddMinutes(-1))
            .Add(c => c.CompletedContent, "<span class='done'>Expired</span>"));

        cut.Find(".done").TextContent.Should().Be("Expired");
        cut.Find("[role=timer]").GetAttribute("aria-label").Should().Be("Time is up");
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Sparkline
// ═══════════════════════════════════════════════════════════════════════════════

public class SparklineTests : TestContext
{
    [Fact]
    public void Sparkline_Line_RendersPathAndLastPoint()
    {
        var cut = RenderComponent<StackSparkline>(p => p.Add(s => s.Values, new double[] { 1, 3, 2 }));

        cut.Find("path").GetAttribute("d").Should().StartWith("M");
        cut.FindAll("circle").Should().HaveCount(1);
        cut.Find("svg").GetAttribute("aria-label").Should().Be("Trend from 1 to 2, low 1, high 3");
    }

    [Fact]
    public void Sparkline_Bar_RendersOneRectPerValue()
    {
        var cut = RenderComponent<StackSparkline>(p => p
            .Add(s => s.Values, new double[] { 4, 8, 6, 2 })
            .Add(s => s.Type, "bar"));

        cut.FindAll("rect").Should().HaveCount(4);
    }

    [Fact]
    public void Sparkline_FewerThanTwoValues_RendersNoShapes()
    {
        var cut = RenderComponent<StackSparkline>(p => p.Add(s => s.Values, new double[] { 5 }));

        cut.FindAll("path").Should().BeEmpty();
    }

    [Fact]
    public void Sparkline_IgnoresNaNAndInfinity()
    {
        var act = () => RenderComponent<StackSparkline>(p => p
            .Add(s => s.Values, new[] { 1, double.NaN, double.PositiveInfinity, 2 }));

        act.Should().NotThrow();
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// InfiniteScroll
// ═══════════════════════════════════════════════════════════════════════════════

public class InfiniteScrollTests : TestContext
{
    public InfiniteScrollTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void InfiniteScroll_HasMore_StartsObserving()
    {
        RenderComponent<StackInfiniteScroll>(p => p.Add(s => s.HasMore, true));

        JSInterop.VerifyInvoke("MyStackBlazor.observeInfiniteScroll");
    }

    [Fact]
    public void InfiniteScroll_NoMore_DoesNotObserve_AndShowsEndContent()
    {
        var cut = RenderComponent<StackInfiniteScroll>(p => p
            .Add(s => s.HasMore, false)
            .Add(s => s.EndContent, "<p class='end'>All caught up</p>"));

        JSInterop.VerifyNotInvoke("MyStackBlazor.observeInfiniteScroll");
        cut.Find(".end").TextContent.Should().Be("All caught up");
    }

    [Fact]
    public async Task InfiniteScroll_Trigger_CallsOnLoadMore()
    {
        var calls = 0;
        var cut = RenderComponent<StackInfiniteScroll>(p => p.Add(s => s.OnLoadMore, () => calls++));

        await cut.InvokeAsync(() => cut.Instance.TriggerLoadMore());

        calls.Should().Be(1);
    }

    [Fact]
    public async Task InfiniteScroll_LoadFailure_ShowsRetry()
    {
        var cut = RenderComponent<StackInfiniteScroll>(p => p
            .Add(s => s.OnLoadMore, () => Task.FromException(new InvalidOperationException("boom"))));

        await cut.InvokeAsync(() => cut.Instance.TriggerLoadMore());

        cut.Find("[role=alert]").TextContent.Should().Contain("Retry");
    }
}
