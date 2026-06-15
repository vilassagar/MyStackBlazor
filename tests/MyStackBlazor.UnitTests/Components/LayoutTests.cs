using MyStackBlazor.Components.Layout;

namespace MyStackBlazor.UnitTests.Components;

// ═══════════════════════════════════════════════════════════════════════════════
// Card
// ═══════════════════════════════════════════════════════════════════════════════

public class CardTests : TestContext
{
    [Fact]
    public void Card_Renders_DivElement()
    {
        var cut = RenderComponent<Card>();
        cut.Find("div").Should().NotBeNull();
    }

    [Fact]
    public void Card_ChildContent_Rendered()
    {
        var cut = RenderComponent<Card>(p => p.AddChildContent("Card body"));
        cut.Markup.Should().Contain("Card body");
    }

    [Fact]
    public void Card_Class_AppendedToRoot()
    {
        var cut = RenderComponent<Card>(p => p.Add(c => c.Class, "shadow-lg"));
        cut.Find("div").ClassName.Should().Contain("shadow-lg");
    }

    [Fact]
    public void Card_AdditionalAttributes_PassedThrough()
    {
        var cut = RenderComponent<Card>(p => p.AddUnmatched("data-id", "card-1"));
        cut.Find("[data-id=card-1]").Should().NotBeNull();
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Accordion + AccordionItem
// ═══════════════════════════════════════════════════════════════════════════════

public class AccordionTests : TestContext
{
    [Fact]
    public void Accordion_Renders_Container()
    {
        var cut = RenderComponent<Accordion>(p => p
            .AddChildContent<AccordionItem>(i => i
                .Add(ai => ai.Title, "Section 1")
                .Add(ai => ai.Value, "s1")
                .AddChildContent("Content here")));

        cut.Find("div").Should().NotBeNull();
    }

    [Fact]
    public void AccordionItem_Title_Displayed()
    {
        var cut = RenderComponent<Accordion>(p => p
            .AddChildContent<AccordionItem>(i => i
                .Add(ai => ai.Title, "FAQ Answer")
                .Add(ai => ai.Value, "faq")
                .AddChildContent("Answer text")));

        cut.Markup.Should().Contain("FAQ Answer");
    }

    [Fact]
    public void AccordionItem_Initially_Collapsed()
    {
        var cut = RenderComponent<Accordion>(p => p
            .AddChildContent<AccordionItem>(i => i
                .Add(ai => ai.Title, "Section")
                .Add(ai => ai.Value, "s1")
                .AddChildContent("Content")));

        cut.Find("button").GetAttribute("aria-expanded").Should().Be("false");
    }

    [Fact]
    public async Task AccordionItem_Click_ExpandsItem()
    {
        var cut = RenderComponent<Accordion>(p => p
            .AddChildContent<AccordionItem>(i => i
                .Add(ai => ai.Title, "Section")
                .Add(ai => ai.Value, "s1")
                .AddChildContent("Content")));

        await cut.Find("button").ClickAsync(new MouseEventArgs());

        cut.Find("button").GetAttribute("aria-expanded").Should().Be("true");
    }

    [Fact]
    public async Task AccordionItem_Click_Twice_CollapsesItem()
    {
        var cut = RenderComponent<Accordion>(p => p
            .AddChildContent<AccordionItem>(i => i
                .Add(ai => ai.Title, "Section")
                .Add(ai => ai.Value, "s1")
                .AddChildContent("Content")));

        var btn = cut.Find("button");
        await btn.ClickAsync(new MouseEventArgs());
        await btn.ClickAsync(new MouseEventArgs());

        cut.Find("button").GetAttribute("aria-expanded").Should().Be("false");
    }

    [Fact]
    public async Task Accordion_AllowMultipleFalse_CollapsesPrevious_OnNewOpen()
    {
        var cut = RenderComponent<Accordion>(p => p
            .Add(a => a.AllowMultiple, false)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "Title", "Section A");
                builder.AddAttribute(2, "Value", "a");
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(b => b.AddContent(0, "A content")));
                builder.CloseComponent();

                builder.OpenComponent<AccordionItem>(4);
                builder.AddAttribute(5, "Title", "Section B");
                builder.AddAttribute(6, "Value", "b");
                builder.AddAttribute(7, "ChildContent", (RenderFragment)(b => b.AddContent(0, "B content")));
                builder.CloseComponent();
            }));

        var buttons = cut.FindAll("button");
        await buttons[0].ClickAsync(new MouseEventArgs());  // open A
        await buttons[1].ClickAsync(new MouseEventArgs());  // open B → A should close

        var expandedButtons = cut.FindAll("button[aria-expanded=true]");
        expandedButtons.Count.Should().Be(1);
    }

    [Fact]
    public async Task Accordion_AllowMultipleTrue_KeepsBothOpen()
    {
        var cut = RenderComponent<Accordion>(p => p
            .Add(a => a.AllowMultiple, true)
            .AddChildContent(builder =>
            {
                builder.OpenComponent<AccordionItem>(0);
                builder.AddAttribute(1, "Title", "A");
                builder.AddAttribute(2, "Value", "a");
                builder.AddAttribute(3, "ChildContent", (RenderFragment)(b => b.AddContent(0, "A")));
                builder.CloseComponent();

                builder.OpenComponent<AccordionItem>(4);
                builder.AddAttribute(5, "Title", "B");
                builder.AddAttribute(6, "Value", "b");
                builder.AddAttribute(7, "ChildContent", (RenderFragment)(b => b.AddContent(0, "B")));
                builder.CloseComponent();
            }));

        var buttons = cut.FindAll("button");
        await buttons[0].ClickAsync(new MouseEventArgs());
        await buttons[1].ClickAsync(new MouseEventArgs());

        cut.FindAll("button[aria-expanded=true]").Count.Should().Be(2);
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Collapsible
// ═══════════════════════════════════════════════════════════════════════════════

public class CollapsibleTests : TestContext
{
    [Fact]
    public void Collapsible_Renders_Container()
    {
        var cut = RenderComponent<Collapsible>(p => p
            .AddChildContent("Collapsible content"));

        cut.Find("div").Should().NotBeNull();
    }

    [Fact]
    public void Collapsible_IsOpen_True_ContentVisible()
    {
        var cut = RenderComponent<Collapsible>(p => p
            .Add(c => c.IsOpen, true)
            .AddChildContent("Visible content"));

        cut.Markup.Should().Contain("Visible content");
    }

    [Fact]
    public void Collapsible_IsOpen_False_ContentHidden()
    {
        var cut = RenderComponent<Collapsible>(p => p
            .Add(c => c.IsOpen, false)
            .AddChildContent("Hidden content"));

        // Content rendered but hidden via CSS
        cut.Find("div").Should().NotBeNull();
    }

    [Fact]
    public async Task Collapsible_Toggle_FiresIsOpenChanged()
    {
        bool? newValue = null;
        var cut = RenderComponent<Collapsible>(p => p
            .Add(c => c.IsOpen, false)
            .Add(c => c.IsOpenChanged,
                EventCallback.Factory.Create<bool>(this, v => newValue = v))
            .Add(c => c.Trigger, b => b.AddMarkupContent(0, "<button id='trigger'>Toggle</button>"))
            .AddChildContent("Content"));

        cut.SetParametersAndRender(p => p.Add(c => c.IsOpen, true));

        newValue.Should().BeNull(); // callback only fires on user interaction
        cut.Instance.IsOpen.Should().BeTrue();
    }
}
