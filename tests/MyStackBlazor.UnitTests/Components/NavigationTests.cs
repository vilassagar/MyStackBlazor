using MyStackBlazor.Components.Navigation;

namespace MyStackBlazor.UnitTests.Components;

// ═══════════════════════════════════════════════════════════════════════════════
// Pagination
// ═══════════════════════════════════════════════════════════════════════════════

public class PaginationTests : TestContext
{
    [Fact]
    public void Pagination_Renders_NavWithRole()
    {
        var cut = RenderComponent<Pagination>(p => p
            .Add(p2 => p2.CurrentPage, 1)
            .Add(p2 => p2.TotalPages, 5));

        cut.Find("[role=navigation]").Should().NotBeNull();
    }

    [Fact]
    public void Pagination_AriaLabel_IsPagination()
    {
        var cut = RenderComponent<Pagination>(p => p
            .Add(p2 => p2.TotalPages, 3));

        cut.Find("nav").GetAttribute("aria-label").Should().Be("pagination");
    }

    [Fact]
    public void Pagination_RendersPageNumbers()
    {
        var cut = RenderComponent<Pagination>(p => p
            .Add(p2 => p2.CurrentPage, 1)
            .Add(p2 => p2.TotalPages, 5));

        cut.FindAll("button[aria-label^='Go to page']").Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Pagination_PreviousButton_DisabledOnFirstPage()
    {
        var cut = RenderComponent<Pagination>(p => p
            .Add(p2 => p2.CurrentPage, 1)
            .Add(p2 => p2.TotalPages, 5));

        cut.Find("button[aria-label='Go to previous page']")
           .HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void Pagination_NextButton_DisabledOnLastPage()
    {
        var cut = RenderComponent<Pagination>(p => p
            .Add(p2 => p2.CurrentPage, 5)
            .Add(p2 => p2.TotalPages, 5));

        cut.Find("button[aria-label='Go to next page']")
           .HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void Pagination_PreviousButton_Enabled_WhenNotFirstPage()
    {
        var cut = RenderComponent<Pagination>(p => p
            .Add(p2 => p2.CurrentPage, 3)
            .Add(p2 => p2.TotalPages, 5));

        cut.Find("button[aria-label='Go to previous page']")
           .HasAttribute("disabled").Should().BeFalse();
    }

    [Fact]
    public void Pagination_NextButton_Enabled_WhenNotLastPage()
    {
        var cut = RenderComponent<Pagination>(p => p
            .Add(p2 => p2.CurrentPage, 3)
            .Add(p2 => p2.TotalPages, 5));

        cut.Find("button[aria-label='Go to next page']")
           .HasAttribute("disabled").Should().BeFalse();
    }

    [Fact]
    public async Task Pagination_NextClick_FiresCurrentPageChanged()
    {
        int? result = null;
        var cut = RenderComponent<Pagination>(p => p
            .Add(p2 => p2.CurrentPage, 2)
            .Add(p2 => p2.TotalPages, 5)
            .Add(p2 => p2.CurrentPageChanged,
                EventCallback.Factory.Create<int>(this, v => result = v)));

        await cut.Find("button[aria-label='Go to next page']")
                 .ClickAsync(new MouseEventArgs());

        result.Should().Be(3);
    }

    [Fact]
    public async Task Pagination_PreviousClick_FiresCurrentPageChanged()
    {
        int? result = null;
        var cut = RenderComponent<Pagination>(p => p
            .Add(p2 => p2.CurrentPage, 3)
            .Add(p2 => p2.TotalPages, 5)
            .Add(p2 => p2.CurrentPageChanged,
                EventCallback.Factory.Create<int>(this, v => result = v)));

        await cut.Find("button[aria-label='Go to previous page']")
                 .ClickAsync(new MouseEventArgs());

        result.Should().Be(2);
    }

    [Fact]
    public async Task Pagination_PageButton_FiresCurrentPageChanged_WithCorrectPage()
    {
        int? result = null;
        var cut = RenderComponent<Pagination>(p => p
            .Add(p2 => p2.CurrentPage, 1)
            .Add(p2 => p2.TotalPages, 5)
            .Add(p2 => p2.CurrentPageChanged,
                EventCallback.Factory.Create<int>(this, v => result = v)));

        // With CurrentPage=1 and SiblingCount=1 the visible pages are 1, 2, …, 5 — page 2 is visible
        var pageBtn = cut.Find("button[aria-label='Go to page 2']");
        await pageBtn.ClickAsync(new MouseEventArgs());

        result.Should().Be(2);
    }

    [Fact]
    public void Pagination_CurrentPage_IsHighlighted()
    {
        var cut = RenderComponent<Pagination>(p => p
            .Add(p2 => p2.CurrentPage, 2)
            .Add(p2 => p2.TotalPages, 5));

        var activePage = cut.Find("button[aria-label='Go to page 2']");
        activePage.ClassName.Should().Contain("bg-primary");
    }

    [Fact]
    public void Pagination_SinglePage_BothButtonsDisabled()
    {
        var cut = RenderComponent<Pagination>(p => p
            .Add(p2 => p2.CurrentPage, 1)
            .Add(p2 => p2.TotalPages, 1));

        cut.Find("button[aria-label='Go to previous page']").HasAttribute("disabled").Should().BeTrue();
        cut.Find("button[aria-label='Go to next page']").HasAttribute("disabled").Should().BeTrue();
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Tabs + TabsList + TabsTrigger + TabsContent
// ═══════════════════════════════════════════════════════════════════════════════

public class TabsTests : TestContext
{
    private static IRenderedComponent<StackTabs> RenderTabs(TestContext ctx, string active = "tab1") =>
        ctx.RenderComponent<StackTabs>(p => p
            .Add(t => t.ActiveTab, active)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<StackTabsList>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b =>
                {
                    b.OpenComponent<StackTabsTrigger>(0);
                    b.AddAttribute(1, "Value", "tab1");
                    b.AddAttribute(2, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, "Tab 1")));
                    b.CloseComponent();

                    b.OpenComponent<StackTabsTrigger>(3);
                    b.AddAttribute(4, "Value", "tab2");
                    b.AddAttribute(5, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, "Tab 2")));
                    b.CloseComponent();
                }));
                builder.CloseComponent();

                builder.OpenComponent<StackTabsContent>(6);
                builder.AddAttribute(7, "Value", "tab1");
                builder.AddAttribute(8, "ChildContent", (RenderFragment)(b => b.AddContent(0, "Panel 1 content")));
                builder.CloseComponent();

                builder.OpenComponent<StackTabsContent>(9);
                builder.AddAttribute(10, "Value", "tab2");
                builder.AddAttribute(11, "ChildContent", (RenderFragment)(b => b.AddContent(0, "Panel 2 content")));
                builder.CloseComponent();
            }));

    [Fact]
    public void Tabs_Renders_TablistRole()
    {
        var cut = RenderTabs(this);
        cut.Find("[role=tablist]").Should().NotBeNull();
    }

    [Fact]
    public void Tabs_Renders_TabButtons()
    {
        var cut = RenderTabs(this);
        cut.FindAll("[role=tab]").Count.Should().Be(2);
    }

    [Fact]
    public void Tabs_Renders_TabPanels()
    {
        var cut = RenderTabs(this);
        cut.FindAll("[role=tabpanel]").Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Tabs_ActiveTab_IsAriaSelectedTrue()
    {
        var cut = RenderTabs(this, "tab1");
        var tabs = cut.FindAll("[role=tab]");
        tabs[0].GetAttribute("aria-selected").Should().Be("true");
    }

    [Fact]
    public void Tabs_InactiveTab_IsAriaSelectedFalse()
    {
        var cut = RenderTabs(this, "tab1");
        var tabs = cut.FindAll("[role=tab]");
        tabs[1].GetAttribute("aria-selected").Should().Be("false");
    }

    [Fact]
    public void Tabs_ActiveTabContent_Visible()
    {
        var cut = RenderTabs(this, "tab1");
        cut.Find("[role=tabpanel]").TextContent.Should().Contain("Panel 1 content");
    }

    [Fact]
    public async Task Tabs_ClickTab2_ChangesActiveTab()
    {
        string? result = null;
        var cut = RenderComponent<Tabs>(p => p
            .Add(t => t.ActiveTab, "tab1")
            .Add(t => t.ActiveTabChanged,
                EventCallback.Factory.Create<string?>(this, v => result = v))
            .AddChildContent(builder =>
            {
                builder.OpenComponent<TabsList>(0);
                builder.AddAttribute(1, "ChildContent", (RenderFragment)(b =>
                {
                    b.OpenComponent<TabsTrigger>(0);
                    b.AddAttribute(1, "Value", "tab1");
                    b.AddAttribute(2, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, "T1")));
                    b.CloseComponent();
                    b.OpenComponent<TabsTrigger>(3);
                    b.AddAttribute(4, "Value", "tab2");
                    b.AddAttribute(5, "ChildContent", (RenderFragment)(b2 => b2.AddContent(0, "T2")));
                    b.CloseComponent();
                }));
                builder.CloseComponent();
            }));

        await cut.FindAll("[role=tab]")[1].ClickAsync(new MouseEventArgs());

        result.Should().Be("tab2");
    }

    [Fact]
    public void Tabs_ActiveTab_AppliesActiveClass()
    {
        var cut = RenderTabs(this, "tab1");
        var tab1 = cut.FindAll("[role=tab]")[0];
        tab1.ClassName.Should().Contain("bg-background");
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Breadcrumb + BreadcrumbItem
// ═══════════════════════════════════════════════════════════════════════════════

public class BreadcrumbTests : TestContext
{
    [Fact]
    public void Breadcrumb_Renders_NavElement()
    {
        var cut = RenderComponent<Breadcrumb>();
        cut.Find("nav").Should().NotBeNull();
    }

    [Fact]
    public void BreadcrumbItem_Link_RenderedWhenHrefProvided()
    {
        var cut = RenderComponent<Breadcrumb>(p => p
            .AddChildContent<BreadcrumbItem>(i => i
                .Add(b => b.Href, "/home")
                .AddChildContent("Home")));

        cut.Find("a").GetAttribute("href").Should().Be("/home");
    }

    [Fact]
    public void BreadcrumbItem_NoLink_WhenNoHref()
    {
        var cut = RenderComponent<Breadcrumb>(p => p
            .AddChildContent<BreadcrumbItem>(i => i
                .Add(b => b.IsCurrentPage, true)
                .AddChildContent("Current Page")));

        cut.FindAll("a").Count.Should().Be(0);
    }

    [Fact]
    public void BreadcrumbItem_CurrentPage_SetsAriaCurrent()
    {
        var cut = RenderComponent<Breadcrumb>(p => p
            .AddChildContent<BreadcrumbItem>(i => i
                .Add(b => b.IsCurrentPage, true)
                .AddChildContent("Current")));

        cut.Find("[aria-current=page]").Should().NotBeNull();
    }

    [Fact]
    public void BreadcrumbItem_Separator_Shown_WhenShowSeparatorTrue()
    {
        var cut = RenderComponent<Breadcrumb>(p => p
            .AddChildContent(builder =>
            {
                builder.OpenComponent<BreadcrumbItem>(0);
                builder.AddAttribute(1, "Href", "/home");
                builder.AddAttribute(2, "ShowSeparator", true);
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(b => b.AddContent(0, "Home")));
                builder.CloseComponent();

                builder.OpenComponent<BreadcrumbItem>(4);
                builder.AddAttribute(5, "IsCurrentPage", true);
                builder.AddAttribute(6, "ShowSeparator", false);
                builder.AddAttribute(7, "ChildContent", (RenderFragment)(b => b.AddContent(0, "Page")));
                builder.CloseComponent();
            }));

        // Each BreadcrumbItem with ShowSeparator=true renders 2 <li>: one for content, one for separator
        cut.FindAll("li").Count.Should().Be(3);
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// ToggleButton
// ═══════════════════════════════════════════════════════════════════════════════

public class ToggleButtonTests : TestContext
{
    [Fact]
    public void ToggleButton_Renders_Button()
    {
        var cut = RenderComponent<ToggleButton>(p => p.AddChildContent("Bold"));
        cut.Find("button").Should().NotBeNull();
    }

    [Fact]
    public void ToggleButton_IsPressed_False_AriaPressed_False()
    {
        var cut = RenderComponent<ToggleButton>(p => p
            .Add(t => t.IsPressed, false)
            .AddChildContent("B"));

        cut.Find("button").GetAttribute("aria-pressed").Should().Be("false");
    }

    [Fact]
    public void ToggleButton_IsPressed_True_AriaPressed_True()
    {
        var cut = RenderComponent<ToggleButton>(p => p
            .Add(t => t.IsPressed, true)
            .AddChildContent("B"));

        cut.Find("button").GetAttribute("aria-pressed").Should().Be("true");
    }

    [Fact]
    public async Task ToggleButton_Click_FiresIsPressedChanged()
    {
        bool? result = null;
        var cut = RenderComponent<ToggleButton>(p => p
            .Add(t => t.IsPressed, false)
            .Add(t => t.IsPressedChanged,
                EventCallback.Factory.Create<bool>(this, v => result = v))
            .AddChildContent("Bold"));

        await cut.Find("button").ClickAsync(new MouseEventArgs());

        result.Should().BeTrue();
    }

    [Fact]
    public async Task ToggleButton_Click_WhenPressed_FiresFalse()
    {
        bool? result = null;
        var cut = RenderComponent<ToggleButton>(p => p
            .Add(t => t.IsPressed, true)
            .Add(t => t.IsPressedChanged,
                EventCallback.Factory.Create<bool>(this, v => result = v))
            .AddChildContent("Bold"));

        await cut.Find("button").ClickAsync(new MouseEventArgs());

        result.Should().BeFalse();
    }

    [Fact]
    public void ToggleButton_ChildContent_Rendered()
    {
        var cut = RenderComponent<ToggleButton>(p => p.AddChildContent("Italic"));
        cut.Find("button").TextContent.Should().Contain("Italic");
    }
}
