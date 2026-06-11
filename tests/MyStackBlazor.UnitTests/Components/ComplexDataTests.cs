using MyStackBlazor.Components.Data;

namespace MyStackBlazor.UnitTests.Components;

// ── Shared test models ────────────────────────────────────────────────────────

internal sealed record TreeNode(string Name, string? Dept = null, List<TreeNode>? Children = null);
internal sealed record SaleRecord(string Region, string Quarter, double Amount);

// ═════════════════════════════════════════════════════════════════════════════
// TreeList tests
// ═════════════════════════════════════════════════════════════════════════════

public class TreeListTests : TestContext
{
    private static IReadOnlyList<TreeListColumn<TreeNode>> TwoColumns =>
    [
        new TreeListColumn<TreeNode> { Header = "Name",       Field = n => n.Name },
        new TreeListColumn<TreeNode> { Header = "Department", Field = n => n.Dept },
    ];

    private static List<TreeNode> FlatItems =>
    [
        new TreeNode("Alice", "Engineering"),
        new TreeNode("Bob",   "Marketing"),
        new TreeNode("Carol", "Sales"),
    ];

    private static List<TreeNode> HierarchyItems =>
    [
        new TreeNode("Engineering", Children:
        [
            new TreeNode("Alice", "Backend"),
            new TreeNode("Bob",   "Frontend"),
        ]),
        new TreeNode("Marketing", Children:
        [
            new TreeNode("Carol", "Content"),
        ]),
    ];

    // ── Structure ─────────────────────────────────────────────────────────────

    [Fact]
    public void TreeList_Renders_TableWithTreegridRole()
    {
        var cut = RenderComponent<TreeList<TreeNode>>(p => p
            .Add(c => c.Items, FlatItems)
            .Add(c => c.Columns, TwoColumns));

        cut.Find("[role=treegrid]").Should().NotBeNull();
    }

    [Fact]
    public void TreeList_Renders_OneRowPerRootItem()
    {
        var cut = RenderComponent<TreeList<TreeNode>>(p => p
            .Add(c => c.Items, FlatItems)
            .Add(c => c.Columns, TwoColumns));

        cut.FindAll("[role=row]").Count.Should().BeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public void TreeList_RenderColumns_ColumnHeaders()
    {
        var cut = RenderComponent<TreeList<TreeNode>>(p => p
            .Add(c => c.Items, FlatItems)
            .Add(c => c.Columns, TwoColumns));

        cut.Markup.Should().Contain("Name").And.Contain("Department");
    }

    // ── Expand / Collapse ──────────────────────────────────────────────────────

    [Fact]
    public void TreeList_ChildrenHidden_BeforeExpand()
    {
        IReadOnlyList<TreeListColumn<TreeNode>> cols =
        [
            new TreeListColumn<TreeNode> { Header = "Name", Field = n => n.Name },
        ];

        var cut = RenderComponent<TreeList<TreeNode>>(p => p
            .Add(c => c.Items, HierarchyItems)
            .Add(c => c.Columns, cols)
            .Add(c => c.ChildrenSelector, n => n.Children));

        cut.Markup.Should().NotContain("Alice");
        cut.Markup.Should().NotContain("Bob");
    }

    [Fact]
    public void TreeList_ChildrenShown_AfterExpand()
    {
        IReadOnlyList<TreeListColumn<TreeNode>> cols =
        [
            new TreeListColumn<TreeNode> { Header = "Name", Field = n => n.Name },
        ];

        var cut = RenderComponent<TreeList<TreeNode>>(p => p
            .Add(c => c.Items, HierarchyItems)
            .Add(c => c.Columns, cols)
            .Add(c => c.ChildrenSelector, n => n.Children));

        cut.Find("button[aria-label='Expand row']").Click();

        cut.Markup.Should().Contain("Alice");
    }

    [Fact]
    public void TreeList_ChildrenHidden_AfterCollapseToggle()
    {
        IReadOnlyList<TreeListColumn<TreeNode>> cols =
        [
            new TreeListColumn<TreeNode> { Header = "Name", Field = n => n.Name },
        ];

        var cut = RenderComponent<TreeList<TreeNode>>(p => p
            .Add(c => c.Items, HierarchyItems)
            .Add(c => c.Columns, cols)
            .Add(c => c.ChildrenSelector, n => n.Children));

        var expand = cut.Find("button[aria-label='Expand row']");
        expand.Click();

        cut.Find("button[aria-label='Collapse row']").Click();

        cut.Markup.Should().NotContain("Alice");
    }

    [Fact]
    public void TreeList_DefaultExpanded_ShowsChildren()
    {
        IReadOnlyList<TreeListColumn<TreeNode>> cols =
        [
            new TreeListColumn<TreeNode> { Header = "Name", Field = n => n.Name },
        ];

        var cut = RenderComponent<TreeList<TreeNode>>(p => p
            .Add(c => c.Items, HierarchyItems)
            .Add(c => c.Columns, cols)
            .Add(c => c.ChildrenSelector, n => n.Children)
            .Add(c => c.DefaultExpanded, true));

        cut.Markup.Should().Contain("Alice").And.Contain("Carol");
    }

    // ── ARIA ───────────────────────────────────────────────────────────────────

    [Fact]
    public void TreeList_ExpandableRow_HasAriaExpandedFalse()
    {
        IReadOnlyList<TreeListColumn<TreeNode>> cols =
        [
            new TreeListColumn<TreeNode> { Header = "Name", Field = n => n.Name },
        ];

        var cut = RenderComponent<TreeList<TreeNode>>(p => p
            .Add(c => c.Items, HierarchyItems)
            .Add(c => c.Columns, cols)
            .Add(c => c.ChildrenSelector, n => n.Children));

        var rows = cut.FindAll("[role=row][aria-expanded='false']");
        rows.Should().NotBeEmpty();
    }

    [Fact]
    public void TreeList_ExpandedRow_HasAriaExpandedTrue()
    {
        IReadOnlyList<TreeListColumn<TreeNode>> cols =
        [
            new TreeListColumn<TreeNode> { Header = "Name", Field = n => n.Name },
        ];

        var cut = RenderComponent<TreeList<TreeNode>>(p => p
            .Add(c => c.Items, HierarchyItems)
            .Add(c => c.Columns, cols)
            .Add(c => c.ChildrenSelector, n => n.Children));

        cut.Find("button[aria-label='Expand row']").Click();

        var rows = cut.FindAll("[role=row][aria-expanded='true']");
        rows.Should().NotBeEmpty();
    }

    [Fact]
    public void TreeList_ChildRows_HaveAriaLevelTwo()
    {
        IReadOnlyList<TreeListColumn<TreeNode>> cols =
        [
            new TreeListColumn<TreeNode> { Header = "Name", Field = n => n.Name },
        ];

        var cut = RenderComponent<TreeList<TreeNode>>(p => p
            .Add(c => c.Items, HierarchyItems)
            .Add(c => c.Columns, cols)
            .Add(c => c.ChildrenSelector, n => n.Children)
            .Add(c => c.DefaultExpanded, true));

        var level2Rows = cut.FindAll("[aria-level='2']");
        level2Rows.Count.Should().Be(3);
    }

    [Fact]
    public void TreeList_RootRows_HaveAriaLevelOne()
    {
        IReadOnlyList<TreeListColumn<TreeNode>> cols =
        [
            new TreeListColumn<TreeNode> { Header = "Name", Field = n => n.Name },
        ];

        var cut = RenderComponent<TreeList<TreeNode>>(p => p
            .Add(c => c.Items, HierarchyItems)
            .Add(c => c.Columns, cols)
            .Add(c => c.ChildrenSelector, n => n.Children));

        cut.FindAll("[aria-level='1']").Count.Should().Be(2);
    }

    [Fact]
    public void TreeList_CustomAriaLabel_AppliedToTable()
    {
        IReadOnlyList<TreeListColumn<TreeNode>> cols =
        [
            new TreeListColumn<TreeNode> { Header = "Name", Field = n => n.Name },
        ];

        var cut = RenderComponent<TreeList<TreeNode>>(p => p
            .Add(c => c.Items, FlatItems)
            .Add(c => c.Columns, cols)
            .Add(c => c.Label, "Employee tree"));

        cut.Find("[role=treegrid]").GetAttribute("aria-label").Should().Be("Employee tree");
    }

    // ── Empty state ────────────────────────────────────────────────────────────

    [Fact]
    public void TreeList_EmptyItems_ShowsEmptyText()
    {
        IReadOnlyList<TreeListColumn<TreeNode>> cols =
        [
            new TreeListColumn<TreeNode> { Header = "Name", Field = n => n.Name },
        ];

        var cut = RenderComponent<TreeList<TreeNode>>(p => p
            .Add(c => c.Items, Array.Empty<TreeNode>())
            .Add(c => c.Columns, cols)
            .Add(c => c.EmptyText, "Nothing here"));

        cut.Markup.Should().Contain("Nothing here");
    }

    [Fact]
    public void TreeList_EmptyItems_DefaultEmptyMessage_Shown()
    {
        IReadOnlyList<TreeListColumn<TreeNode>> cols =
        [
            new TreeListColumn<TreeNode> { Header = "Name", Field = n => n.Name },
        ];

        var cut = RenderComponent<TreeList<TreeNode>>(p => p
            .Add(c => c.Items, Array.Empty<TreeNode>())
            .Add(c => c.Columns, cols));

        cut.Markup.Should().Contain("No data");
    }

    // ── Leaf node has no expand button ────────────────────────────────────────

    [Fact]
    public void TreeList_FlatItem_NoExpandButton()
    {
        IReadOnlyList<TreeListColumn<TreeNode>> cols =
        [
            new TreeListColumn<TreeNode> { Header = "Name", Field = n => n.Name },
        ];

        var cut = RenderComponent<TreeList<TreeNode>>(p => p
            .Add(c => c.Items, FlatItems)
            .Add(c => c.Columns, cols)
            .Add(c => c.ChildrenSelector, n => n.Children));

        cut.FindAll("button[aria-label='Expand row']").Should().BeEmpty();
    }
}

// ═════════════════════════════════════════════════════════════════════════════
// PivotGrid tests
// ═════════════════════════════════════════════════════════════════════════════

public class PivotGridTests : TestContext
{
    private static readonly IReadOnlyList<SaleRecord> SalesData =
    [
        new SaleRecord("North", "Q1", 100),
        new SaleRecord("North", "Q2", 150),
        new SaleRecord("South", "Q1",  80),
        new SaleRecord("South", "Q2", 120),
        new SaleRecord("North", "Q1",  50), // duplicate cell — should sum to 150
    ];

    // ── Structure ─────────────────────────────────────────────────────────────

    [Fact]
    public void PivotGrid_Renders_Table()
    {
        var cut = RenderComponent<PivotGrid<SaleRecord>>(p => p
            .Add(c => c.Data,      SalesData)
            .Add(c => c.RowKey,    r => r.Region)
            .Add(c => c.ColumnKey, r => r.Quarter)
            .Add(c => c.Value,     r => r.Amount));

        cut.Find("table").Should().NotBeNull();
    }

    [Fact]
    public void PivotGrid_Renders_UniqueColumnHeaders()
    {
        var cut = RenderComponent<PivotGrid<SaleRecord>>(p => p
            .Add(c => c.Data,      SalesData)
            .Add(c => c.RowKey,    r => r.Region)
            .Add(c => c.ColumnKey, r => r.Quarter)
            .Add(c => c.Value,     r => r.Amount));

        var markup = cut.Markup;
        var q1Count = System.Text.RegularExpressions.Regex.Matches(markup, ">Q1<").Count;
        var q2Count = System.Text.RegularExpressions.Regex.Matches(markup, ">Q2<").Count;
        q1Count.Should().BeGreaterThanOrEqualTo(1);
        q2Count.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public void PivotGrid_Renders_UniqueRowLabels()
    {
        var cut = RenderComponent<PivotGrid<SaleRecord>>(p => p
            .Add(c => c.Data,      SalesData)
            .Add(c => c.RowKey,    r => r.Region)
            .Add(c => c.ColumnKey, r => r.Quarter)
            .Add(c => c.Value,     r => r.Amount));

        cut.Markup.Should().Contain("North").And.Contain("South");
    }

    // ── Aggregation ───────────────────────────────────────────────────────────

    [Fact]
    public void PivotGrid_CellValues_SummedCorrectly()
    {
        var cut = RenderComponent<PivotGrid<SaleRecord>>(p => p
            .Add(c => c.Data,       SalesData)
            .Add(c => c.RowKey,     r => r.Region)
            .Add(c => c.ColumnKey,  r => r.Quarter)
            .Add(c => c.Value,      r => r.Amount)
            .Add(c => c.ShowTotals, false));

        // North Q1 = 100 + 50 = 150
        cut.Markup.Should().Contain(">150<");
        // South Q2 = 120
        cut.Markup.Should().Contain(">120<");
    }

    [Fact]
    public void PivotGrid_RowTotals_ComputedCorrectly()
    {
        var cut = RenderComponent<PivotGrid<SaleRecord>>(p => p
            .Add(c => c.Data,       SalesData)
            .Add(c => c.RowKey,     r => r.Region)
            .Add(c => c.ColumnKey,  r => r.Quarter)
            .Add(c => c.Value,      r => r.Amount)
            .Add(c => c.ShowTotals, true));

        // North row total = 150 + 150 = 300
        cut.Markup.Should().Contain(">300<");
        // South row total = 80 + 120 = 200
        cut.Markup.Should().Contain(">200<");
    }

    [Fact]
    public void PivotGrid_ColumnTotals_ComputedCorrectly()
    {
        var cut = RenderComponent<PivotGrid<SaleRecord>>(p => p
            .Add(c => c.Data,       SalesData)
            .Add(c => c.RowKey,     r => r.Region)
            .Add(c => c.ColumnKey,  r => r.Quarter)
            .Add(c => c.Value,      r => r.Amount)
            .Add(c => c.ShowTotals, true));

        // Q1 total = 150 + 80 = 230
        cut.Markup.Should().Contain(">230<");
        // Q2 total = 150 + 120 = 270
        cut.Markup.Should().Contain(">270<");
    }

    [Fact]
    public void PivotGrid_GrandTotal_ComputedCorrectly()
    {
        var cut = RenderComponent<PivotGrid<SaleRecord>>(p => p
            .Add(c => c.Data,       SalesData)
            .Add(c => c.RowKey,     r => r.Region)
            .Add(c => c.ColumnKey,  r => r.Quarter)
            .Add(c => c.Value,      r => r.Amount)
            .Add(c => c.ShowTotals, true));

        // Grand total = 150 + 150 + 80 + 120 = 500
        cut.Markup.Should().Contain(">500<");
    }

    [Fact]
    public void PivotGrid_ShowTotalsFalse_HidesTotalsColumn()
    {
        var cut = RenderComponent<PivotGrid<SaleRecord>>(p => p
            .Add(c => c.Data,       SalesData)
            .Add(c => c.RowKey,     r => r.Region)
            .Add(c => c.ColumnKey,  r => r.Quarter)
            .Add(c => c.Value,      r => r.Amount)
            .Add(c => c.ShowTotals, false));

        cut.Markup.Should().NotContain(">Total<");
        cut.FindAll("tfoot").Should().BeEmpty();
    }

    // ── Custom aggregation ────────────────────────────────────────────────────

    [Fact]
    public void PivotGrid_CustomAggregate_Count()
    {
        var cut = RenderComponent<PivotGrid<SaleRecord>>(p => p
            .Add(c => c.Data,       SalesData)
            .Add(c => c.RowKey,     r => r.Region)
            .Add(c => c.ColumnKey,  r => r.Quarter)
            .Add(c => c.Value,      _ => 1.0)            // value = 1 so count = sum
            .Add(c => c.Aggregate,  vals => vals.Count())
            .Add(c => c.ShowTotals, false));

        // North has Q1 × 2 entries
        cut.Markup.Should().Contain(">2<");
    }

    // ── Title ─────────────────────────────────────────────────────────────────

    [Fact]
    public void PivotGrid_Title_RenderedWhenSet()
    {
        var cut = RenderComponent<PivotGrid<SaleRecord>>(p => p
            .Add(c => c.Data,      SalesData)
            .Add(c => c.RowKey,    r => r.Region)
            .Add(c => c.ColumnKey, r => r.Quarter)
            .Add(c => c.Value,     r => r.Amount)
            .Add(c => c.Title,     "Sales by Region"));

        cut.Markup.Should().Contain("Sales by Region");
    }

    [Fact]
    public void PivotGrid_Title_NotRenderedWhenNull()
    {
        var cut = RenderComponent<PivotGrid<SaleRecord>>(p => p
            .Add(c => c.Data,      SalesData)
            .Add(c => c.RowKey,    r => r.Region)
            .Add(c => c.ColumnKey, r => r.Quarter)
            .Add(c => c.Value,     r => r.Amount));

        cut.FindAll("div.font-semibold").Should().BeEmpty();
    }

    [Fact]
    public void PivotGrid_RowLabel_RenderedInHeader()
    {
        var cut = RenderComponent<PivotGrid<SaleRecord>>(p => p
            .Add(c => c.Data,      SalesData)
            .Add(c => c.RowKey,    r => r.Region)
            .Add(c => c.ColumnKey, r => r.Quarter)
            .Add(c => c.Value,     r => r.Amount)
            .Add(c => c.RowLabel,  "Region"));

        cut.Markup.Should().Contain("Region");
    }

    // ── Empty data ────────────────────────────────────────────────────────────

    [Fact]
    public void PivotGrid_EmptyData_RendersWithoutException()
    {
        var ex = Record.Exception(() =>
            RenderComponent<PivotGrid<SaleRecord>>(p => p
                .Add(c => c.Data,      Array.Empty<SaleRecord>())
                .Add(c => c.RowKey,    r => r.Region)
                .Add(c => c.ColumnKey, r => r.Quarter)
                .Add(c => c.Value,     r => r.Amount)));

        ex.Should().BeNull();
    }

    [Fact]
    public void PivotGrid_EmptyData_ShowsNoDataMessage()
    {
        var cut = RenderComponent<PivotGrid<SaleRecord>>(p => p
            .Add(c => c.Data,      Array.Empty<SaleRecord>())
            .Add(c => c.RowKey,    r => r.Region)
            .Add(c => c.ColumnKey, r => r.Quarter)
            .Add(c => c.Value,     r => r.Amount));

        cut.Markup.Should().Contain("No data");
    }
}
