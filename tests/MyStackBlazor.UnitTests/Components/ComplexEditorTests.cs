using MyStackBlazor.Components.Form;
using Bunit;

namespace MyStackBlazor.UnitTests.Components;

// ── Shared models ─────────────────────────────────────────────────────────────

internal sealed class Category
{
    public required string Name { get; init; }
    public List<Category> Children { get; init; } = [];
}

internal sealed record Employee(string Name, string Department, string Grade);

// ═════════════════════════════════════════════════════════════════════════════
// DropDownTree tests
// ═════════════════════════════════════════════════════════════════════════════

public class DropDownTreeTests : TestContext
{
    private static List<Category> Tree =>
    [
        new Category { Name = "Electronics", Children =
        [
            new Category { Name = "Phones" },
            new Category { Name = "Laptops" },
        ]},
        new Category { Name = "Clothing", Children =
        [
            new Category { Name = "Shirts" },
        ]},
        new Category { Name = "Books" },
    ];

    // ── Structure ─────────────────────────────────────────────────────────

    [Fact]
    public void DropDownTree_Renders_Trigger()
    {
        var cut = RenderComponent<DropDownTree<Category>>(p => p
            .Add(c => c.Items, Tree)
            .Add(c => c.GetLabel, cat => cat.Name)
            .Add(c => c.Placeholder, "Choose category"));

        cut.Find("[role=combobox]").Should().NotBeNull();
        cut.Markup.Should().Contain("Choose category");
    }

    [Fact]
    public void DropDownTree_Trigger_AriaHaspopup_IsTree()
    {
        var cut = RenderComponent<DropDownTree<Category>>(p => p
            .Add(c => c.Items, Tree)
            .Add(c => c.GetLabel, cat => cat.Name));

        cut.Find("[role=combobox]").GetAttribute("aria-haspopup").Should().Be("tree");
    }

    [Fact]
    public void DropDownTree_ClosedInitially_AriaExpandedFalse()
    {
        var cut = RenderComponent<DropDownTree<Category>>(p => p
            .Add(c => c.Items, Tree)
            .Add(c => c.GetLabel, cat => cat.Name));

        cut.Find("[role=combobox]").GetAttribute("aria-expanded").Should().Be("false");
    }

    [Fact]
    public void DropDownTree_Opens_OnTriggerClick()
    {
        var cut = RenderComponent<DropDownTree<Category>>(p => p
            .Add(c => c.Items, Tree)
            .Add(c => c.GetLabel, cat => cat.Name));

        cut.Find("[role=combobox]").Click();

        cut.Find("[role=tree]").Should().NotBeNull();
    }

    [Fact]
    public void DropDownTree_Shows_RootItems()
    {
        var cut = RenderComponent<DropDownTree<Category>>(p => p
            .Add(c => c.Items, Tree)
            .Add(c => c.GetLabel, cat => cat.Name));

        cut.Find("[role=combobox]").Click();

        var markup = cut.Markup;
        markup.Should().Contain("Electronics");
        markup.Should().Contain("Clothing");
        markup.Should().Contain("Books");
    }

    [Fact]
    public void DropDownTree_ChildrenHidden_BeforeExpand()
    {
        var cut = RenderComponent<DropDownTree<Category>>(p => p
            .Add(c => c.Items, Tree)
            .Add(c => c.GetLabel, cat => cat.Name)
            .Add(c => c.GetChildren, cat => cat.Children));

        cut.Find("[role=combobox]").Click();

        cut.Markup.Should().NotContain("Phones");
    }

    [Fact]
    public void DropDownTree_ChildrenVisible_AfterExpand()
    {
        var cut = RenderComponent<DropDownTree<Category>>(p => p
            .Add(c => c.Items, Tree)
            .Add(c => c.GetLabel, cat => cat.Name)
            .Add(c => c.GetChildren, cat => cat.Children));

        cut.Find("[role=combobox]").Click();

        // Click "Electronics" to expand
        cut.FindAll("[role=treeitem]")
           .First(e => e.TextContent.Contains("Electronics"))
           .Click();

        cut.Markup.Should().Contain("Phones").And.Contain("Laptops");
    }

    [Fact]
    public void DropDownTree_LeafSelection_ClosesDropdown()
    {
        var cut = RenderComponent<DropDownTree<Category>>(p => p
            .Add(c => c.Items, Tree)
            .Add(c => c.GetLabel, cat => cat.Name)
            .Add(c => c.GetChildren, cat => cat.Children));

        cut.Find("[role=combobox]").Click();

        // Expand Electronics
        cut.FindAll("[role=treeitem]")
           .First(e => e.TextContent.Contains("Electronics"))
           .Click();

        // Select leaf "Phones"
        cut.FindAll("[role=treeitem]")
           .First(e => e.TextContent.Contains("Phones"))
           .Click();

        cut.FindAll("[role=tree]").Should().BeEmpty();
    }

    [Fact]
    public void DropDownTree_LeafSelection_FiresValueChanged()
    {
        Category? selected = null;

        var cut = RenderComponent<DropDownTree<Category>>(p => p
            .Add(c => c.Items, Tree)
            .Add(c => c.GetLabel, cat => cat.Name)
            .Add(c => c.GetChildren, cat => cat.Children)
            .Add(c => c.ValueChanged, cat => selected = cat));

        cut.Find("[role=combobox]").Click();
        cut.FindAll("[role=treeitem]").First(e => e.TextContent.Contains("Books")).Click();

        selected?.Name.Should().Be("Books");
    }

    [Fact]
    public void DropDownTree_SelectedLabel_ShownInTrigger()
    {
        var books = Tree.Single(t => t.Name == "Books");

        var cut = RenderComponent<DropDownTree<Category>>(p => p
            .Add(c => c.Items, Tree)
            .Add(c => c.GetLabel, cat => cat.Name)
            .Add(c => c.Value, books));

        cut.Find("[role=combobox]").TextContent.Should().Contain("Books");
    }

    [Fact]
    public void DropDownTree_SearchFilter_FiltersItems()
    {
        var cut = RenderComponent<DropDownTree<Category>>(p => p
            .Add(c => c.Items, Tree)
            .Add(c => c.GetLabel, cat => cat.Name)
            .Add(c => c.GetChildren, cat => cat.Children)
            .Add(c => c.Searchable, true));

        cut.Find("[role=combobox]").Click();
        cut.Find("input[type=text]").Input("Book");

        cut.FindAll("[role=treeitem]").Count.Should().Be(1);
        cut.Markup.Should().Contain("Books");
        cut.Markup.Should().NotContain("Electronics");
    }

    [Fact]
    public void DropDownTree_BranchSelect_SelectsParentNode()
    {
        Category? selected = null;

        var cut = RenderComponent<DropDownTree<Category>>(p => p
            .Add(c => c.Items, Tree)
            .Add(c => c.GetLabel, cat => cat.Name)
            .Add(c => c.GetChildren, cat => cat.Children)
            .Add(c => c.AllowBranchSelect, true)
            .Add(c => c.ValueChanged, cat => selected = cat));

        cut.Find("[role=combobox]").Click();
        cut.FindAll("[role=treeitem]").First(e => e.TextContent.Contains("Electronics")).Click();

        selected?.Name.Should().Be("Electronics");
    }
}

// ═════════════════════════════════════════════════════════════════════════════
// MultiColumnComboBox tests
// ═════════════════════════════════════════════════════════════════════════════

public class MultiColumnComboBoxTests : TestContext
{
    private static readonly IReadOnlyList<Employee> Employees =
    [
        new Employee("Alice Wong",  "Engineering", "L4"),
        new Employee("Bob Smith",   "Marketing",   "L3"),
        new Employee("Carol Lee",   "Engineering", "L5"),
        new Employee("Dave Miller", "Sales",       "L2"),
    ];

    private static IReadOnlyList<MultiColumnComboBoxColumn<Employee>> Columns =>
    [
        new MultiColumnComboBoxColumn<Employee> { Header = "Name",       Value = e => e.Name },
        new MultiColumnComboBoxColumn<Employee> { Header = "Department", Value = e => e.Department },
        new MultiColumnComboBoxColumn<Employee> { Header = "Grade",      Value = e => e.Grade },
    ];

    // ── Structure ─────────────────────────────────────────────────────────

    [Fact]
    public void MultiColumnComboBox_Renders_Trigger()
    {
        var cut = RenderComponent<MultiColumnComboBox<Employee>>(p => p
            .Add(c => c.Items, Employees)
            .Add(c => c.Columns, Columns)
            .Add(c => c.GetLabel, e => e.Name));

        cut.Find("[role=combobox]").Should().NotBeNull();
    }

    [Fact]
    public void MultiColumnComboBox_AriaHaspopup_IsGrid()
    {
        var cut = RenderComponent<MultiColumnComboBox<Employee>>(p => p
            .Add(c => c.Items, Employees)
            .Add(c => c.Columns, Columns));

        cut.Find("[role=combobox]").GetAttribute("aria-haspopup").Should().Be("grid");
    }

    [Fact]
    public void MultiColumnComboBox_Opens_OnClick()
    {
        var cut = RenderComponent<MultiColumnComboBox<Employee>>(p => p
            .Add(c => c.Items, Employees)
            .Add(c => c.Columns, Columns));

        cut.Find("[role=combobox]").Click();

        cut.Find("[role=grid]").Should().NotBeNull();
    }

    [Fact]
    public void MultiColumnComboBox_ShowsColumnHeaders()
    {
        var cut = RenderComponent<MultiColumnComboBox<Employee>>(p => p
            .Add(c => c.Items, Employees)
            .Add(c => c.Columns, Columns)
            .Add(c => c.Searchable, false));

        cut.Find("[role=combobox]").Click();

        var markup = cut.Markup;
        markup.Should().Contain("Name").And.Contain("Department").And.Contain("Grade");
    }

    [Fact]
    public void MultiColumnComboBox_ShowsAllRowsWhenClosed()
    {
        var cut = RenderComponent<MultiColumnComboBox<Employee>>(p => p
            .Add(c => c.Items, Employees)
            .Add(c => c.Columns, Columns)
            .Add(c => c.Searchable, false));

        cut.Find("[role=combobox]").Click();

        cut.FindAll("[role=row]").Count.Should().BeGreaterThanOrEqualTo(4);
    }

    [Fact]
    public void MultiColumnComboBox_ShowsAllColumnValues()
    {
        var cut = RenderComponent<MultiColumnComboBox<Employee>>(p => p
            .Add(c => c.Items, Employees)
            .Add(c => c.Columns, Columns)
            .Add(c => c.Searchable, false));

        cut.Find("[role=combobox]").Click();

        cut.Markup.Should().Contain("Alice Wong")
                           .And.Contain("Engineering")
                           .And.Contain("L4");
    }

    [Fact]
    public void MultiColumnComboBox_Search_FiltersRows()
    {
        var cut = RenderComponent<MultiColumnComboBox<Employee>>(p => p
            .Add(c => c.Items, Employees)
            .Add(c => c.Columns, Columns)
            .Add(c => c.Searchable, true));

        cut.Find("[role=combobox]").Click();
        cut.Find("input[type=text]").Input("alice");

        cut.FindAll("[role=row]").Count(r => r.GetAttribute("aria-selected") is not null).Should().Be(1);
    }

    [Fact]
    public void MultiColumnComboBox_Search_FindsByAnyColumn()
    {
        var cut = RenderComponent<MultiColumnComboBox<Employee>>(p => p
            .Add(c => c.Items, Employees)
            .Add(c => c.Columns, Columns)
            .Add(c => c.Searchable, true));

        cut.Find("[role=combobox]").Click();
        cut.Find("input[type=text]").Input("Marketing");

        // Only Bob Smith is in Marketing
        cut.FindAll("[role=row]").Count(r => r.GetAttribute("aria-selected") is not null).Should().Be(1);
        cut.Markup.Should().Contain("Bob Smith");
    }

    [Fact]
    public void MultiColumnComboBox_Selection_ClosesDropdown()
    {
        var cut = RenderComponent<MultiColumnComboBox<Employee>>(p => p
            .Add(c => c.Items, Employees)
            .Add(c => c.Columns, Columns)
            .Add(c => c.Searchable, false));

        cut.Find("[role=combobox]").Click();
        cut.FindAll("[role=row]").First(r => r.GetAttribute("aria-selected") is not null).Click();

        cut.FindAll("[role=grid]").Should().BeEmpty();
    }

    [Fact]
    public void MultiColumnComboBox_Selection_FiresValueChanged()
    {
        Employee? selected = null;

        var cut = RenderComponent<MultiColumnComboBox<Employee>>(p => p
            .Add(c => c.Items, Employees)
            .Add(c => c.Columns, Columns)
            .Add(c => c.Searchable, false)
            .Add(c => c.ValueChanged, e => selected = e));

        cut.Find("[role=combobox]").Click();
        cut.FindAll("[role=row]").First(r => r.TextContent.Contains("Alice")).Click();

        selected?.Name.Should().Be("Alice Wong");
    }

    [Fact]
    public void MultiColumnComboBox_SelectedLabel_ShownInTrigger()
    {
        var alice = Employees[0];

        var cut = RenderComponent<MultiColumnComboBox<Employee>>(p => p
            .Add(c => c.Items, Employees)
            .Add(c => c.Columns, Columns)
            .Add(c => c.GetLabel, e => e.Name)
            .Add(c => c.Value, alice));

        cut.Find("[role=combobox]").TextContent.Should().Contain("Alice Wong");
    }

    [Fact]
    public void MultiColumnComboBox_NoResults_ShowsMessage()
    {
        var cut = RenderComponent<MultiColumnComboBox<Employee>>(p => p
            .Add(c => c.Items, Employees)
            .Add(c => c.Columns, Columns)
            .Add(c => c.Searchable, true));

        cut.Find("[role=combobox]").Click();
        cut.Find("input[type=text]").Input("zzz_no_match");

        cut.Markup.Should().Contain("No results");
    }
}

// ═════════════════════════════════════════════════════════════════════════════
// MarkdownEditor tests
// ═════════════════════════════════════════════════════════════════════════════

public class MarkdownEditorTests : TestContext
{
    public MarkdownEditorTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
    }

    // ── Structure ─────────────────────────────────────────────────────────

    [Fact]
    public void MarkdownEditor_Renders_Textarea()
    {
        var cut = RenderComponent<MarkdownEditor>();

        cut.Find("textarea").Should().NotBeNull();
    }

    [Fact]
    public void MarkdownEditor_ShowToolbarTrue_ShowsToolbar()
    {
        var cut = RenderComponent<MarkdownEditor>(p => p
            .Add(c => c.ShowToolbar, true));

        cut.Find("[role=toolbar]").Should().NotBeNull();
    }

    [Fact]
    public void MarkdownEditor_ShowToolbarFalse_HidesToolbar()
    {
        var cut = RenderComponent<MarkdownEditor>(p => p
            .Add(c => c.ShowToolbar, false));

        cut.FindAll("[role=toolbar]").Should().BeEmpty();
    }

    [Fact]
    public void MarkdownEditor_Input_UpdatesValue()
    {
        string? received = null;

        var cut = RenderComponent<MarkdownEditor>(p => p
            .Add(c => c.ValueChanged, v => received = v));

        cut.Find("textarea").Input("Hello world");

        received.Should().Be("Hello world");
    }

    [Fact]
    public void MarkdownEditor_PreviewPanel_RenderedInSplitMode()
    {
        var cut = RenderComponent<MarkdownEditor>(p => p
            .Add(c => c.Value, "# Title"));

        cut.Find("[aria-label='Markdown preview']").Should().NotBeNull();
    }

    [Fact]
    public void MarkdownEditor_Preview_RendersH1()
    {
        var cut = RenderComponent<MarkdownEditor>(p => p
            .Add(c => c.Value, "# Hello"));

        var preview = cut.Find("[aria-label='Markdown preview']");
        preview.InnerHtml.Should().Contain("<h1");
        preview.InnerHtml.Should().Contain("Hello");
    }

    [Fact]
    public void MarkdownEditor_Preview_RendersBold()
    {
        var cut = RenderComponent<MarkdownEditor>(p => p
            .Add(c => c.Value, "**bold text**"));

        cut.Find("[aria-label='Markdown preview']").InnerHtml.Should().Contain("<strong>bold text</strong>");
    }

    [Fact]
    public void MarkdownEditor_Preview_RendersItalic()
    {
        var cut = RenderComponent<MarkdownEditor>(p => p
            .Add(c => c.Value, "*italic*"));

        cut.Find("[aria-label='Markdown preview']").InnerHtml.Should().Contain("<em>italic</em>");
    }

    [Fact]
    public void MarkdownEditor_Preview_RendersUnorderedList()
    {
        var cut = RenderComponent<MarkdownEditor>(p => p
            .Add(c => c.Value, "- item one\n- item two"));

        var preview = cut.Find("[aria-label='Markdown preview']").InnerHtml;
        preview.Should().Contain("<ul").And.Contain("<li");
    }

    [Fact]
    public void MarkdownEditor_Preview_RendersOrderedList()
    {
        var cut = RenderComponent<MarkdownEditor>(p => p
            .Add(c => c.Value, "1. first\n2. second"));

        cut.Find("[aria-label='Markdown preview']").InnerHtml.Should().Contain("<ol");
    }

    [Fact]
    public void MarkdownEditor_Preview_RendersBlockquote()
    {
        var cut = RenderComponent<MarkdownEditor>(p => p
            .Add(c => c.Value, "> A quote"));

        cut.Find("[aria-label='Markdown preview']").InnerHtml.Should().Contain("<blockquote");
    }

    [Fact]
    public void MarkdownEditor_Preview_RendersInlineCode()
    {
        var cut = RenderComponent<MarkdownEditor>(p => p
            .Add(c => c.Value, "Use `code` here"));

        cut.Find("[aria-label='Markdown preview']").InnerHtml.Should().Contain("<code");
    }

    [Fact]
    public void MarkdownEditor_Preview_RendersLink()
    {
        var cut = RenderComponent<MarkdownEditor>(p => p
            .Add(c => c.Value, "[Click me](https://example.com)"));

        cut.Find("[aria-label='Markdown preview']").InnerHtml.Should().Contain("<a ");
    }

    [Fact]
    public void MarkdownEditor_Preview_EncodesHtmlInContent()
    {
        var cut = RenderComponent<MarkdownEditor>(p => p
            .Add(c => c.Value, "<script>alert(1)</script>"));

        var preview = cut.Find("[aria-label='Markdown preview']").InnerHtml;
        preview.Should().NotContain("<script>");
    }

    [Fact]
    public void MarkdownEditor_EditMode_HidesPreview()
    {
        var cut = RenderComponent<MarkdownEditor>();

        cut.Find("button[aria-label='Edit']").Click();

        cut.Find("button[aria-label='Edit']").GetAttribute("aria-pressed").Should().Be("true");
        cut.FindAll("[aria-label='Markdown preview']").Should().BeEmpty();
    }

    [Fact]
    public void MarkdownEditor_PreviewMode_HidesTextarea()
    {
        var cut = RenderComponent<MarkdownEditor>(p => p
            .Add(c => c.Value, "hello"));

        cut.Find("button[aria-label='Preview']").Click();

        cut.FindAll("textarea").Should().BeEmpty();
    }

    [Fact]
    public void MarkdownEditor_StatusBar_ShowsWordCount()
    {
        var cut = RenderComponent<MarkdownEditor>(p => p
            .Add(c => c.Value, "Hello world foo"));

        cut.Markup.Should().Contain("3 words");
    }

    [Fact]
    public void MarkdownEditor_StatusBar_ShowsCharCount()
    {
        var cut = RenderComponent<MarkdownEditor>(p => p
            .Add(c => c.Value, "Hello"));

        cut.Markup.Should().Contain("5 chars");
    }

    [Fact]
    public void MarkdownEditor_Toolbar_HasBoldButton()
    {
        var cut = RenderComponent<MarkdownEditor>();

        cut.Find("button[aria-label='Bold']").Should().NotBeNull();
    }

    [Fact]
    public void MarkdownEditor_Toolbar_HasHeadingButtons()
    {
        var cut = RenderComponent<MarkdownEditor>();

        cut.Find("button[aria-label='Heading 1']").Should().NotBeNull();
        cut.Find("button[aria-label='Heading 2']").Should().NotBeNull();
        cut.Find("button[aria-label='Heading 3']").Should().NotBeNull();
    }
}
