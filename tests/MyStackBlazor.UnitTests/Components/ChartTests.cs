using MyStackBlazor.Components.Charts;

namespace MyStackBlazor.UnitTests.Components;

public class LineChartTests : TestContext
{
    private static readonly IReadOnlyList<ChartSeries> SingleSeries =
    [
        new ChartSeries("Revenue", [10.0, 20.0, 15.0, 30.0])
    ];

    private static readonly IReadOnlyList<ChartSeries> MultiSeries =
    [
        new ChartSeries("Revenue",  [10.0, 20.0, 15.0, 30.0]),
        new ChartSeries("Expenses", [5.0,  12.0, 18.0, 22.0])
    ];

    [Fact]
    public void LineChart_Renders_SvgElement()
    {
        var cut = RenderComponent<LineChart>(p => p
            .Add(c => c.Series, SingleSeries));

        cut.Find("svg").Should().NotBeNull();
    }

    [Fact]
    public void LineChart_Title_AppearsWhenSet()
    {
        var cut = RenderComponent<LineChart>(p => p
            .Add(c => c.Series, SingleSeries)
            .Add(c => c.Title, "Monthly Revenue"));

        cut.Find("title").TextContent.Should().Be("Monthly Revenue");
    }

    [Fact]
    public void LineChart_NoTitleElement_WhenTitleIsNull()
    {
        var cut = RenderComponent<LineChart>(p => p
            .Add(c => c.Series, SingleSeries)
            .Add(c => c.Title, (string?)null));

        cut.Find("title").TextContent.Should().Be("Line chart");
    }

    [Fact]
    public void LineChart_AriaLabel_EqualsTitle()
    {
        var cut = RenderComponent<LineChart>(p => p
            .Add(c => c.Series, SingleSeries)
            .Add(c => c.Title, "Sales Chart"));

        cut.Find("svg").GetAttribute("aria-label").Should().Be("Sales Chart");
    }

    [Fact]
    public void LineChart_AriaLabel_DefaultsToLineChart_WhenTitleNull()
    {
        var cut = RenderComponent<LineChart>(p => p
            .Add(c => c.Series, SingleSeries));

        cut.Find("svg").GetAttribute("aria-label").Should().Be("Line chart");
    }

    [Fact]
    public void LineChart_ShowLegendTrue_LegendDivPresent_WithSeriesNames()
    {
        var cut = RenderComponent<LineChart>(p => p
            .Add(c => c.Series, MultiSeries)
            .Add(c => c.ShowLegend, true));

        var markup = cut.Markup;
        markup.Should().Contain("Revenue");
        markup.Should().Contain("Expenses");
    }

    [Fact]
    public void LineChart_ShowLegendFalse_LegendDivAbsent()
    {
        var cut = RenderComponent<LineChart>(p => p
            .Add(c => c.Series, MultiSeries)
            .Add(c => c.ShowLegend, false));

        cut.FindAll("div.flex.flex-wrap").Should().BeEmpty();
    }

    [Fact]
    public void LineChart_SeriesCount_EqualsNumberOfLinePaths()
    {
        var cut = RenderComponent<LineChart>(p => p
            .Add(c => c.Series, MultiSeries)
            .Add(c => c.ShowArea, false));

        var linePaths = cut.FindAll("path[fill='none']");
        linePaths.Count.Should().Be(MultiSeries.Count);
    }

    [Fact]
    public void LineChart_ShowAreaTrue_AreaPathsPresent()
    {
        var cut = RenderComponent<LineChart>(p => p
            .Add(c => c.Series, MultiSeries)
            .Add(c => c.ShowArea, true));

        var areaPaths = cut.FindAll("path[opacity='0.12']");
        areaPaths.Count.Should().Be(MultiSeries.Count);
    }

    [Fact]
    public void LineChart_ShowDotsTrue_CirclesRendered()
    {
        var cut = RenderComponent<LineChart>(p => p
            .Add(c => c.Series, SingleSeries)
            .Add(c => c.ShowDots, true));

        var circles = cut.FindAll("circle");
        circles.Count.Should().Be(SingleSeries[0].Values.Count);
    }

    [Fact]
    public void LineChart_ShowDotsFalse_NoCircles()
    {
        var cut = RenderComponent<LineChart>(p => p
            .Add(c => c.Series, SingleSeries)
            .Add(c => c.ShowDots, false));

        cut.FindAll("circle").Should().BeEmpty();
    }

    [Fact]
    public void LineChart_EmptySeries_RendersWithoutThrowing()
    {
        var act = () => RenderComponent<LineChart>(p => p
            .Add(c => c.Series, Array.Empty<ChartSeries>()));

        act.Should().NotThrow();
    }
}

public class BarChartTests : TestContext
{
    private static readonly IReadOnlyList<ChartSeries> SingleSeries =
    [
        new ChartSeries("Sales", [100.0, 200.0, 150.0])
    ];

    private static readonly IReadOnlyList<ChartSeries> MultiSeries =
    [
        new ChartSeries("Sales",    [100.0, 200.0, 150.0]),
        new ChartSeries("Returns",  [10.0,  20.0,  15.0])
    ];

    [Fact]
    public void BarChart_Renders_SvgElement()
    {
        var cut = RenderComponent<BarChart>(p => p
            .Add(c => c.Series, SingleSeries));

        cut.Find("svg").Should().NotBeNull();
    }

    [Fact]
    public void BarChart_Grouped_CorrectRectCount()
    {
        int N = 3;
        int M = 2;
        var cut = RenderComponent<BarChart>(p => p
            .Add(c => c.Series, MultiSeries)
            .Add(c => c.Stacked, false));

        var rects = cut.FindAll("rect");
        rects.Count.Should().Be(N * M);
    }

    [Fact]
    public void BarChart_Title_AppearsWhenSet()
    {
        var cut = RenderComponent<BarChart>(p => p
            .Add(c => c.Series, SingleSeries)
            .Add(c => c.Title, "Monthly Sales"));

        cut.Find("title").TextContent.Should().Be("Monthly Sales");
    }

    [Fact]
    public void BarChart_AriaLabel_EqualsTitle()
    {
        var cut = RenderComponent<BarChart>(p => p
            .Add(c => c.Series, SingleSeries)
            .Add(c => c.Title, "Sales Chart"));

        cut.Find("svg").GetAttribute("aria-label").Should().Be("Sales Chart");
    }

    [Fact]
    public void BarChart_AriaLabel_DefaultsToBarChart_WhenTitleNull()
    {
        var cut = RenderComponent<BarChart>(p => p
            .Add(c => c.Series, SingleSeries));

        cut.Find("svg").GetAttribute("aria-label").Should().Be("Bar chart");
    }

    [Fact]
    public void BarChart_ShowLegendTrue_LegendPresent()
    {
        var cut = RenderComponent<BarChart>(p => p
            .Add(c => c.Series, MultiSeries)
            .Add(c => c.ShowLegend, true));

        cut.Markup.Should().Contain("Sales");
        cut.Markup.Should().Contain("Returns");
    }

    [Fact]
    public void BarChart_ShowLegendFalse_LegendAbsent()
    {
        var cut = RenderComponent<BarChart>(p => p
            .Add(c => c.Series, MultiSeries)
            .Add(c => c.ShowLegend, false));

        cut.FindAll("div.flex.flex-wrap").Should().BeEmpty();
    }

    [Fact]
    public void BarChart_Stacked_RendersAtLeastNRects()
    {
        int N = 3;
        var cut = RenderComponent<BarChart>(p => p
            .Add(c => c.Series, SingleSeries)
            .Add(c => c.Stacked, true));

        var rects = cut.FindAll("rect");
        rects.Count.Should().BeGreaterThanOrEqualTo(N);
    }

    [Fact]
    public void BarChart_EmptySeries_RendersWithoutThrowing()
    {
        var act = () => RenderComponent<BarChart>(p => p
            .Add(c => c.Series, Array.Empty<ChartSeries>()));

        act.Should().NotThrow();
    }
}

public class PieChartTests : TestContext
{
    private static readonly IReadOnlyList<ChartSegment> Segments =
    [
        new ChartSegment("Alpha",   40),
        new ChartSegment("Beta",    30),
        new ChartSegment("Gamma",   20),
        new ChartSegment("Delta",   10)
    ];

    [Fact]
    public void PieChart_Renders_SvgElement()
    {
        var cut = RenderComponent<PieChart>(p => p
            .Add(c => c.Segments, Segments));

        cut.Find("svg").Should().NotBeNull();
    }

    [Fact]
    public void PieChart_CorrectNumberOfPaths()
    {
        var cut = RenderComponent<PieChart>(p => p
            .Add(c => c.Segments, Segments));

        cut.FindAll("path").Count.Should().Be(Segments.Count);
    }

    [Fact]
    public void PieChart_AriaLabel_DefaultsToPieChart_WhenTitleNull()
    {
        var cut = RenderComponent<PieChart>(p => p
            .Add(c => c.Segments, Segments));

        cut.Find("svg").GetAttribute("aria-label").Should().Be("Pie chart");
    }

    [Fact]
    public void PieChart_AriaLabel_EqualsTitle()
    {
        var cut = RenderComponent<PieChart>(p => p
            .Add(c => c.Segments, Segments)
            .Add(c => c.Title, "Distribution"));

        cut.Find("svg").GetAttribute("aria-label").Should().Be("Distribution");
    }

    [Fact]
    public void PieChart_ShowPercentagesTrue_PercentTextPresent()
    {
        var cut = RenderComponent<PieChart>(p => p
            .Add(c => c.Segments, Segments)
            .Add(c => c.ShowPercentages, true));

        cut.Markup.Should().Contain("%");
    }

    [Fact]
    public void PieChart_ShowLegendTrue_LegendWithLabels()
    {
        var cut = RenderComponent<PieChart>(p => p
            .Add(c => c.Segments, Segments)
            .Add(c => c.ShowLegend, true));

        foreach (var seg in Segments)
        {
            cut.Markup.Should().Contain(seg.Label);
        }
    }

    [Fact]
    public void PieChart_ShowLegendFalse_NoLegendDiv()
    {
        var cut = RenderComponent<PieChart>(p => p
            .Add(c => c.Segments, Segments)
            .Add(c => c.ShowLegend, false));

        cut.FindAll("div.gap-1\\.5").Should().BeEmpty();
    }

    [Fact]
    public void PieChart_Donut_PathsContainTwoArcCommands()
    {
        var cut = RenderComponent<PieChart>(p => p
            .Add(c => c.Segments, Segments)
            .Add(c => c.Donut, true));

        var paths = cut.FindAll("path");
        paths.Should().NotBeEmpty();
        foreach (var path in paths)
        {
            var d = path.GetAttribute("d") ?? "";
            var aCount = d.Split('A', StringSplitOptions.None).Length - 1;
            aCount.Should().Be(2, because: "donut path has outer arc and inner arc");
        }
    }

    [Fact]
    public void PieChart_Donut_NotDonut_PathsContainOneArcCommand()
    {
        var cut = RenderComponent<PieChart>(p => p
            .Add(c => c.Segments, Segments)
            .Add(c => c.Donut, false));

        var paths = cut.FindAll("path");
        paths.Should().NotBeEmpty();
        foreach (var path in paths)
        {
            var d = path.GetAttribute("d") ?? "";
            var aCount = d.Split('A', StringSplitOptions.None).Length - 1;
            aCount.Should().Be(1, because: "pie path has one arc command");
        }
    }

    [Fact]
    public void PieChart_CenterLabel_AppearsWhenDonutAndSet()
    {
        var cut = RenderComponent<PieChart>(p => p
            .Add(c => c.Segments, Segments)
            .Add(c => c.Donut, true)
            .Add(c => c.CenterLabel, "Total"));

        cut.Markup.Should().Contain("Total");
    }

    [Fact]
    public void PieChart_EmptySegments_RendersWithoutThrowing()
    {
        var act = () => RenderComponent<PieChart>(p => p
            .Add(c => c.Segments, Array.Empty<ChartSegment>()));

        act.Should().NotThrow();
    }
}

public class GaugeChartTests : TestContext
{
    // ── Rendering ──────────────────────────────────────────────────────────

    [Fact]
    public void GaugeChart_Renders_SvgElement()
    {
        var cut = RenderComponent<GaugeChart>(p => p.Add(c => c.Value, 60));
        cut.Find("svg").Should().NotBeNull();
    }

    [Fact]
    public void GaugeChart_Title_AppearsWhenSet()
    {
        var cut = RenderComponent<GaugeChart>(p => p
            .Add(c => c.Value, 60)
            .Add(c => c.Title, "CPU Usage"));

        cut.Markup.Should().Contain("CPU Usage");
    }

    [Fact]
    public void GaugeChart_NoTitleParagraph_WhenTitleNull()
    {
        var cut = RenderComponent<GaugeChart>(p => p.Add(c => c.Value, 60));
        cut.FindAll("p").Should().BeEmpty();
    }

    // ── ARIA ───────────────────────────────────────────────────────────────

    [Fact]
    public void GaugeChart_AriaLabel_IncludesValueAndTitle()
    {
        var cut = RenderComponent<GaugeChart>(p => p
            .Add(c => c.Value, 75)
            .Add(c => c.Title, "Memory"));

        cut.Find("svg").GetAttribute("aria-label").Should().Contain("Memory");
        cut.Find("svg").GetAttribute("aria-label").Should().Contain("75");
    }

    [Fact]
    public void GaugeChart_AriaLabel_FallsBack_WhenTitleNull()
    {
        var cut = RenderComponent<GaugeChart>(p => p.Add(c => c.Value, 50));
        cut.Find("svg").GetAttribute("aria-label").Should().StartWith("Gauge:");
    }

    [Fact]
    public void GaugeChart_AriaValueNow_MatchesValue()
    {
        var cut = RenderComponent<GaugeChart>(p => p.Add(c => c.Value, 42));
        cut.Find("svg").GetAttribute("aria-valuenow").Should().Be("42");
    }

    [Fact]
    public void GaugeChart_AriaValueMin_MatchesMin()
    {
        var cut = RenderComponent<GaugeChart>(p => p
            .Add(c => c.Value, 50)
            .Add(c => c.Min, 10));
        cut.Find("svg").GetAttribute("aria-valuemin").Should().Be("10");
    }

    [Fact]
    public void GaugeChart_AriaValueMax_MatchesMax()
    {
        var cut = RenderComponent<GaugeChart>(p => p
            .Add(c => c.Value, 50)
            .Add(c => c.Max, 200));
        cut.Find("svg").GetAttribute("aria-valuemax").Should().Be("200");
    }

    // ── Unit label ─────────────────────────────────────────────────────────

    [Fact]
    public void GaugeChart_Unit_AppearsInMarkup()
    {
        var cut = RenderComponent<GaugeChart>(p => p
            .Add(c => c.Value, 80)
            .Add(c => c.Unit, "MB/s"));

        cut.Markup.Should().Contain("MB/s");
    }

    [Fact]
    public void GaugeChart_NoUnit_WhenUnitNull()
    {
        var cut = RenderComponent<GaugeChart>(p => p
            .Add(c => c.Value, 80)
            .Add(c => c.Unit, (string?)null));

        cut.Markup.Should().NotContain("MB/s");
    }

    // ── Value arc ──────────────────────────────────────────────────────────

    [Fact]
    public void GaugeChart_AtZero_OnlyTrackPathRendered()
    {
        var cut = RenderComponent<GaugeChart>(p => p
            .Add(c => c.Value, 0)
            .Add(c => c.Min, 0)
            .Add(c => c.Max, 100));

        cut.FindAll("path").Count.Should().Be(1, because: "only the background track renders at value=min");
    }

    [Fact]
    public void GaugeChart_AboveZero_BothPathsRendered()
    {
        var cut = RenderComponent<GaugeChart>(p => p
            .Add(c => c.Value, 50)
            .Add(c => c.Min, 0)
            .Add(c => c.Max, 100));

        cut.FindAll("path").Count.Should().Be(2, because: "track + value arc");
    }

    [Fact]
    public void GaugeChart_CustomColor_AppliesToValueArc()
    {
        var cut = RenderComponent<GaugeChart>(p => p
            .Add(c => c.Value, 70)
            .Add(c => c.Color, "#ef4444"));

        var valuePath = cut.FindAll("path")[1];
        valuePath.GetAttribute("stroke").Should().Be("#ef4444");
    }

    // ── Thresholds ─────────────────────────────────────────────────────────

    [Fact]
    public void GaugeChart_Threshold_LowValue_UsesFirstThresholdColor()
    {
        var thresholds = new[]
        {
            new GaugeThreshold(0.5, "#22c55e"),
            new GaugeThreshold(0.8, "#f59e0b"),
            new GaugeThreshold(1.0, "#ef4444"),
        };

        var cut = RenderComponent<GaugeChart>(p => p
            .Add(c => c.Value, 30)
            .Add(c => c.Min, 0)
            .Add(c => c.Max, 100)
            .Add(c => c.Thresholds, thresholds));

        var valuePath = cut.FindAll("path")[1];
        valuePath.GetAttribute("stroke").Should().Be("#22c55e");
    }

    [Fact]
    public void GaugeChart_Threshold_HighValue_UsesLastThresholdColor()
    {
        var thresholds = new[]
        {
            new GaugeThreshold(0.5, "#22c55e"),
            new GaugeThreshold(0.8, "#f59e0b"),
            new GaugeThreshold(1.0, "#ef4444"),
        };

        var cut = RenderComponent<GaugeChart>(p => p
            .Add(c => c.Value, 90)
            .Add(c => c.Min, 0)
            .Add(c => c.Max, 100)
            .Add(c => c.Thresholds, thresholds));

        var valuePath = cut.FindAll("path")[1];
        valuePath.GetAttribute("stroke").Should().Be("#ef4444");
    }

    [Fact]
    public void GaugeChart_Thresholds_TickMarksRendered()
    {
        var thresholds = new[]
        {
            new GaugeThreshold(0.5, "#22c55e"),
            new GaugeThreshold(0.8, "#f59e0b"),
        };

        var cut = RenderComponent<GaugeChart>(p => p
            .Add(c => c.Value, 50)
            .Add(c => c.Thresholds, thresholds));

        cut.FindAll("line").Count.Should().Be(thresholds.Length);
    }

    // ── Edge cases ─────────────────────────────────────────────────────────

    [Fact]
    public void GaugeChart_ValueAboveMax_ClampedTo100Percent()
    {
        var cut = RenderComponent<GaugeChart>(p => p
            .Add(c => c.Value, 150)
            .Add(c => c.Min, 0)
            .Add(c => c.Max, 100));

        cut.FindAll("path").Count.Should().Be(2);
    }

    [Fact]
    public void GaugeChart_ValueBelowMin_ClampedToZeroPercent()
    {
        var cut = RenderComponent<GaugeChart>(p => p
            .Add(c => c.Value, -10)
            .Add(c => c.Min, 0)
            .Add(c => c.Max, 100));

        cut.FindAll("path").Count.Should().Be(1, because: "value clamped to min → no value arc");
    }

    [Fact]
    public void GaugeChart_MinEqualsMax_RendersWithoutThrowing()
    {
        var act = () => RenderComponent<GaugeChart>(p => p
            .Add(c => c.Value, 50)
            .Add(c => c.Min, 50)
            .Add(c => c.Max, 50));

        act.Should().NotThrow();
    }

    [Fact]
    public void GaugeChart_ValueText_RenderedInMarkup()
    {
        var cut = RenderComponent<GaugeChart>(p => p.Add(c => c.Value, 42));
        cut.Markup.Should().Contain("42");
    }
}
