using MyStackBlazor.Components.Data;

namespace MyStackBlazor.UnitTests.Components;

// ═══════════════════════════════════════════════════════════════════════════════
// Alert
// ═══════════════════════════════════════════════════════════════════════════════

public class AlertTests : TestContext
{
    [Fact]
    public void Alert_Renders_RoleAlert()
    {
        var cut = RenderComponent<Alert>(p => p
            .AddChildContent("Something went wrong."));

        cut.Find("[role=alert]").Should().NotBeNull();
    }

    [Fact]
    public void Alert_ChildContent_Rendered()
    {
        var cut = RenderComponent<Alert>(p => p
            .AddChildContent("Connection lost."));

        cut.Find("[role=alert]").TextContent.Should().Contain("Connection lost.");
    }

    [Theory]
    [InlineData("default",     "bg-background")]
    [InlineData("destructive", "border-destructive")]
    public void Alert_Variant_AppliesClass(string variant, string expectedClass)
    {
        var cut = RenderComponent<Alert>(p => p
            .Add(a => a.Variant, variant)
            .AddChildContent("msg"));

        cut.Find("[role=alert]").ClassName.Should().Contain(expectedClass);
    }

    [Fact]
    public void Alert_Class_AppendedToRoot()
    {
        var cut = RenderComponent<Alert>(p => p
            .Add(a => a.Class, "my-4")
            .AddChildContent("msg"));

        cut.Find("[role=alert]").ClassName.Should().Contain("my-4");
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// AlertTitle
// ═══════════════════════════════════════════════════════════════════════════════

public class AlertTitleTests : TestContext
{
    [Fact]
    public void AlertTitle_Renders_HeadingElement()
    {
        var cut = RenderComponent<AlertTitle>(p => p.AddChildContent("Error"));
        cut.Find("h5").Should().NotBeNull();
    }

    [Fact]
    public void AlertTitle_ChildContent_Rendered()
    {
        var cut = RenderComponent<AlertTitle>(p => p.AddChildContent("Warning!"));
        cut.Find("h5").TextContent.Should().Contain("Warning!");
    }

    [Fact]
    public void AlertTitle_Class_AppendedToElement()
    {
        var cut = RenderComponent<AlertTitle>(p => p
            .Add(t => t.Class, "text-red-500")
            .AddChildContent("Error"));

        cut.Find("h5").ClassName.Should().Contain("text-red-500");
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// AlertDescription
// ═══════════════════════════════════════════════════════════════════════════════

public class AlertDescriptionTests : TestContext
{
    [Fact]
    public void AlertDescription_ChildContent_Rendered()
    {
        var cut = RenderComponent<AlertDescription>(p => p
            .AddChildContent("Please check your internet connection."));

        cut.Markup.Should().Contain("Please check your internet connection.");
    }

    [Fact]
    public void AlertDescription_Class_AppendedToElement()
    {
        var cut = RenderComponent<AlertDescription>(p => p
            .Add(d => d.Class, "text-sm")
            .AddChildContent("desc"));

        cut.Find("div").ClassName.Should().Contain("text-sm");
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Progress
// ═══════════════════════════════════════════════════════════════════════════════

public class ProgressTests : TestContext
{
    [Fact]
    public void Progress_Renders_RoleProgressbar()
    {
        var cut = RenderComponent<Progress>(p => p.Add(p2 => p2.Value, 50));
        cut.Find("[role=progressbar]").Should().NotBeNull();
    }

    [Fact]
    public void Progress_AriaValueMin_IsZero()
    {
        var cut = RenderComponent<Progress>(p => p.Add(p2 => p2.Value, 50));
        cut.Find("[role=progressbar]").GetAttribute("aria-valuemin").Should().Be("0");
    }

    [Fact]
    public void Progress_AriaValueMax_MatchesMax()
    {
        var cut = RenderComponent<Progress>(p => p
            .Add(p2 => p2.Value, 25)
            .Add(p2 => p2.Max, 200));

        cut.Find("[role=progressbar]").GetAttribute("aria-valuemax").Should().Be("200");
    }

    [Fact]
    public void Progress_AriaValueNow_MatchesValue()
    {
        var cut = RenderComponent<Progress>(p => p
            .Add(p2 => p2.Value, 75)
            .Add(p2 => p2.Max, 100));

        cut.Find("[role=progressbar]").GetAttribute("aria-valuenow").Should().Be("75");
    }

    [Fact]
    public void Progress_FullValue_ShowsFullWidth()
    {
        var cut = RenderComponent<Progress>(p => p
            .Add(p2 => p2.Value, 100)
            .Add(p2 => p2.Max, 100));

        var bar = cut.Find("[role=progressbar] div");
        bar.GetAttribute("style").Should().Contain("width: 100%");
    }

    [Fact]
    public void Progress_ZeroValue_ShowsZeroWidth()
    {
        var cut = RenderComponent<Progress>(p => p
            .Add(p2 => p2.Value, 0)
            .Add(p2 => p2.Max, 100));

        var bar = cut.Find("[role=progressbar] div");
        bar.GetAttribute("style").Should().Contain("width: 0%");
    }

    [Fact]
    public void Progress_50Percent_ShowsHalfWidth()
    {
        var cut = RenderComponent<Progress>(p => p
            .Add(p2 => p2.Value, 50)
            .Add(p2 => p2.Max, 100));

        var bar = cut.Find("[role=progressbar] div");
        bar.GetAttribute("style").Should().Contain("width: 50%");
    }

    [Fact]
    public void Progress_ValueAboveMax_ClampedTo100Percent()
    {
        var cut = RenderComponent<Progress>(p => p
            .Add(p2 => p2.Value, 150)
            .Add(p2 => p2.Max, 100));

        var bar = cut.Find("[role=progressbar] div");
        bar.GetAttribute("style").Should().Contain("width: 100%");
    }

    [Fact]
    public void Progress_Class_AppendedToRoot()
    {
        var cut = RenderComponent<Progress>(p => p
            .Add(p2 => p2.Value, 30)
            .Add(p2 => p2.Class, "h-2 my-4"));

        cut.Find("[role=progressbar]").ClassName.Should().Contain("h-2");
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// TreeView
// ═══════════════════════════════════════════════════════════════════════════════

internal sealed record TreeNodeModel(string Name, List<TreeNodeModel>? Children = null);

public class TreeViewTests : TestContext
{
    private static readonly List<TreeNodeModel> FlatItems =
    [
        new("Alice"),
        new("Bob"),
        new("Charlie"),
    ];

    private static readonly List<TreeNodeModel> HierarchyItems =
    [
        new("Engineering", Children: [
            new("Alice"),
            new("Bob"),
        ]),
        new("Marketing", Children: [
            new("Charlie"),
        ]),
    ];

    [Fact]
    public void TreeView_Renders_RoleTree()
    {
        var cut = RenderComponent<TreeView<TreeNodeModel>>(p => p
            .Add(t => t.Items, FlatItems)
            .Add(t => t.GetLabel, n => n.Name));

        cut.Find("[role=tree]").Should().NotBeNull();
    }

    [Fact]
    public void TreeView_RendersAllRootItems()
    {
        var cut = RenderComponent<TreeView<TreeNodeModel>>(p => p
            .Add(t => t.Items, FlatItems)
            .Add(t => t.GetLabel, n => n.Name));

        cut.FindAll("[role=treeitem]").Count.Should().BeGreaterThanOrEqualTo(FlatItems.Count);
    }

    [Fact]
    public void TreeView_GetLabel_DisplaysNodeName()
    {
        var cut = RenderComponent<TreeView<TreeNodeModel>>(p => p
            .Add(t => t.Items, FlatItems)
            .Add(t => t.GetLabel, n => n.Name));

        cut.Markup.Should().Contain("Alice").And.Contain("Bob").And.Contain("Charlie");
    }

    [Fact]
    public void TreeView_EmptyItems_RendersEmptyTree()
    {
        var cut = RenderComponent<TreeView<TreeNodeModel>>(p => p
            .Add(t => t.Items, [])
            .Add(t => t.GetLabel, n => n.Name));

        cut.FindAll("[role=treeitem]").Count.Should().Be(0);
    }

    [Fact]
    public void TreeView_HierarchyItems_RendersRootNodes()
    {
        var cut = RenderComponent<TreeView<TreeNodeModel>>(p => p
            .Add(t => t.Items, HierarchyItems)
            .Add(t => t.GetLabel, n => n.Name)
            .Add(t => t.GetChildren, n => n.Children));

        cut.Markup.Should().Contain("Engineering");
        cut.Markup.Should().Contain("Marketing");
    }

    [Fact]
    public async Task TreeView_ClickItem_FiresSelectedItemChanged()
    {
        TreeNodeModel? selected = null;
        var cut = RenderComponent<TreeView<TreeNodeModel>>(p => p
            .Add(t => t.Items, FlatItems)
            .Add(t => t.GetLabel, n => n.Name)
            .Add(t => t.SelectedItemChanged,
                EventCallback.Factory.Create<TreeNodeModel?>(this, v => selected = v)));

        // The click handler is on the <button> inside [role=treeitem], not on the <li> itself
        await cut.Find("[role=treeitem] button").ClickAsync(new MouseEventArgs());

        selected.Should().NotBeNull();
        selected!.Name.Should().Be("Alice");
    }

    [Fact]
    public void TreeView_ItemTemplate_OverridesDefaultLabel()
    {
        var cut = RenderComponent<TreeView<TreeNodeModel>>(p => p
            .Add(t => t.Items, FlatItems)
            .Add(t => t.GetLabel, n => n.Name)
            .Add(t => t.ItemTemplate, item => b =>
                b.AddMarkupContent(0, $"<em>{item.Name.ToUpper()}</em>")));

        cut.FindAll("em").Count.Should().Be(FlatItems.Count);
        cut.Find("em").TextContent.Should().Be("ALICE");
    }
}
