using MyStackBlazor.Components.Common;

namespace MyStackBlazor.UnitTests.Components;

public class ChipListTests : TestContext
{
    // ── Rendering ──────────────────────────────────────────────────────────

    [Fact]
    public void ChipList_Renders_OneChipPerItem()
    {
        var items = new[] { "Alpha", "Beta", "Gamma" };

        var cut = RenderComponent<ChipList<string>>(p => p
            .Add(c => c.Items, items));

        cut.FindAll("[role=listitem]").Count.Should().Be(3);
    }

    [Fact]
    public void ChipList_Renders_ItemLabels_FromStrings()
    {
        var items = new[] { "React", "Blazor" };

        var cut = RenderComponent<ChipList<string>>(p => p
            .Add(c => c.Items, items));

        var text = cut.Find("[role=list]").TextContent;
        text.Should().Contain("React").And.Contain("Blazor");
    }

    [Fact]
    public void ChipList_Uses_ItemLabel_Func()
    {
        var items = new[] { 1, 2, 3 };

        var cut = RenderComponent<ChipList<int>>(p => p
            .Add(c => c.Items, items)
            .Add(c => c.ItemLabel, n => $"Item {n}"));

        var text = cut.Find("[role=list]").TextContent;
        text.Should().Contain("Item 1").And.Contain("Item 2").And.Contain("Item 3");
    }

    [Fact]
    public void ChipList_Renders_With_Role_List()
    {
        var cut = RenderComponent<ChipList<string>>(p => p
            .Add(c => c.Items, new[] { "x" }));

        cut.Find("[role=list]").Should().NotBeNull();
    }

    [Fact]
    public void ChipList_Each_Chip_Has_Role_Listitem()
    {
        var cut = RenderComponent<ChipList<string>>(p => p
            .Add(c => c.Items, new[] { "a", "b" }));

        cut.FindAll("[role=listitem]").Count.Should().Be(2);
    }

    // ── Accessibility ───────────────────────────────────────────────────────

    [Fact]
    public void ChipList_Has_Default_AriaLabel()
    {
        var cut = RenderComponent<ChipList<string>>(p => p
            .Add(c => c.Items, new[] { "x" }));

        cut.Find("[role=list]").GetAttribute("aria-label").Should().Be("Chip list");
    }

    [Fact]
    public void ChipList_Uses_Custom_Label_For_AriaLabel()
    {
        var cut = RenderComponent<ChipList<string>>(p => p
            .Add(c => c.Items, new[] { "x" })
            .Add(c => c.Label, "Selected tags"));

        cut.Find("[role=list]").GetAttribute("aria-label").Should().Be("Selected tags");
    }

    [Fact]
    public void ChipList_RemoveButton_Has_Accessible_Label()
    {
        var cut = RenderComponent<ChipList<string>>(p => p
            .Add(c => c.Items, new[] { "JavaScript" })
            .Add(c => c.Removable, true));

        cut.Find("button[aria-label]")
           .GetAttribute("aria-label")
           .Should().Be("Remove JavaScript");
    }

    // ── Remove behaviour ───────────────────────────────────────────────────

    [Fact]
    public async Task ChipList_OnRemove_Fires_With_Correct_Item()
    {
        string? removed = null;
        var items = new[] { "React", "Vue", "Svelte" };

        var cut = RenderComponent<ChipList<string>>(p => p
            .Add(c => c.Items, items)
            .Add(c => c.OnRemove, EventCallback.Factory.Create<string>(this, s => removed = s)));

        await cut.FindAll("button[aria-label]")[1].ClickAsync(new MouseEventArgs());

        removed.Should().Be("Vue");
    }

    [Fact]
    public async Task ChipList_OnRemove_Fires_For_ObjectItem()
    {
        var item1 = new TagModel(1, "C#");
        var item2 = new TagModel(2, ".NET");
        TagModel? removed = null;

        var cut = RenderComponent<ChipList<TagModel>>(p => p
            .Add(c => c.Items, new[] { item1, item2 })
            .Add(c => c.ItemLabel, t => t.Name)
            .Add(c => c.OnRemove, EventCallback.Factory.Create<TagModel>(this, t => removed = t)));

        await cut.FindAll("button[aria-label]")[0].ClickAsync(new MouseEventArgs());

        removed.Should().Be(item1);
    }

    // ── Removable flag ─────────────────────────────────────────────────────

    [Fact]
    public void ChipList_Removable_False_Hides_RemoveButtons()
    {
        var cut = RenderComponent<ChipList<string>>(p => p
            .Add(c => c.Items, new[] { "A", "B" })
            .Add(c => c.Removable, false));

        cut.FindAll("button").Count.Should().Be(0);
    }

    [Fact]
    public void ChipList_Removable_True_Shows_RemoveButtons()
    {
        var cut = RenderComponent<ChipList<string>>(p => p
            .Add(c => c.Items, new[] { "A", "B" })
            .Add(c => c.Removable, true));

        cut.FindAll("button").Count.Should().Be(2);
    }

    // ── Variant & Size ─────────────────────────────────────────────────────

    [Theory]
    [InlineData("default",     "bg-primary")]
    [InlineData("secondary",   "bg-secondary")]
    [InlineData("destructive", "bg-destructive")]
    [InlineData("outline",     "border-border")]
    public void ChipList_Variant_AppliesCorrectClass(string variant, string expectedClass)
    {
        var cut = RenderComponent<ChipList<string>>(p => p
            .Add(c => c.Items, new[] { "tag" })
            .Add(c => c.Variant, variant)
            .Add(c => c.Removable, false));

        cut.Find("[role=listitem] span").ClassName.Should().Contain(expectedClass);
    }

    [Theory]
    [InlineData("sm",      "px-2 py-0")]
    [InlineData("default", "px-2.5")]
    [InlineData("lg",      "px-3 py-1")]
    public void ChipList_Size_AppliesCorrectClass(string size, string expectedClass)
    {
        var cut = RenderComponent<ChipList<string>>(p => p
            .Add(c => c.Items, new[] { "tag" })
            .Add(c => c.Size, size)
            .Add(c => c.Removable, false));

        cut.Find("[role=listitem] span").ClassName.Should().Contain(expectedClass.Split(' ')[0]);
    }

    // ── Empty state ────────────────────────────────────────────────────────

    [Fact]
    public void ChipList_EmptyText_Shown_When_Items_Empty()
    {
        var cut = RenderComponent<ChipList<string>>(p => p
            .Add(c => c.Items, Array.Empty<string>())
            .Add(c => c.EmptyText, "No tags selected"));

        cut.Find("[role=list]").TextContent.Should().Contain("No tags selected");
    }

    [Fact]
    public void ChipList_EmptyText_NotShown_When_Items_Present()
    {
        var cut = RenderComponent<ChipList<string>>(p => p
            .Add(c => c.Items, new[] { "tag" })
            .Add(c => c.EmptyText, "No tags selected"));

        cut.Find("[role=list]").TextContent.Should().NotContain("No tags selected");
    }

    [Fact]
    public void ChipList_EmptyContent_Rendered_When_Items_Empty()
    {
        var cut = RenderComponent<ChipList<string>>(p => p
            .Add(c => c.Items, Array.Empty<string>())
            .Add(c => c.EmptyContent, b => b.AddMarkupContent(0, "<em>empty</em>")));

        cut.Find("em").TextContent.Should().Be("empty");
    }

    [Fact]
    public void ChipList_NoEmptyState_When_Items_Present_And_EmptyContent_Set()
    {
        var cut = RenderComponent<ChipList<string>>(p => p
            .Add(c => c.Items, new[] { "x" })
            .Add(c => c.EmptyContent, b => b.AddMarkupContent(0, "<em>empty</em>")));

        cut.FindAll("em").Count.Should().Be(0);
    }

    // ── Custom template ────────────────────────────────────────────────────

    [Fact]
    public void ChipList_ItemTemplate_Overrides_Default_Label()
    {
        var cut = RenderComponent<ChipList<string>>(p => p
            .Add(c => c.Items, new[] { "hello" })
            .Add(c => c.ItemTemplate, item => b =>
                b.AddMarkupContent(0, $"<strong>{item.ToUpper()}</strong>")));

        cut.Find("strong").TextContent.Should().Be("HELLO");
    }

    // ── Null / edge cases ──────────────────────────────────────────────────

    [Fact]
    public void ChipList_Null_Items_Renders_Empty_Container()
    {
        var cut = RenderComponent<ChipList<string>>(p => p
            .Add(c => c.Items, (IEnumerable<string>?)null));

        cut.FindAll("[role=listitem]").Count.Should().Be(0);
    }

    [Fact]
    public void ChipList_ChipClass_Applied_To_Each_Chip()
    {
        var cut = RenderComponent<ChipList<string>>(p => p
            .Add(c => c.Items, new[] { "a", "b" })
            .Add(c => c.ChipClass, "custom-chip")
            .Add(c => c.Removable, false));

        cut.FindAll("[role=listitem] span")
           .All(el => el.ClassName.Contains("custom-chip"))
           .Should().BeTrue();
    }

    private sealed record TagModel(int Id, string Name);
}
