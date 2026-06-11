using MyStackBlazor.Components.Common;

namespace MyStackBlazor.UnitTests.Components;

public class ButtonTests : TestContext
{
    [Fact]
    public void Button_Renders_ChildContent()
    {
        var cut = RenderComponent<Button>(p => p
            .AddChildContent("Click me"));

        cut.Find("button").TextContent.Should().Contain("Click me");
    }

    [Fact]
    public void Button_Renders_AriaLabel_WhenProvided()
    {
        var cut = RenderComponent<Button>(p => p
            .Add(b => b.AriaLabel, "Save document")
            .AddChildContent("Save"));

        cut.Find("button").GetAttribute("aria-label").Should().Be("Save document");
    }

    [Fact]
    public void Button_Disabled_SetsAriaDisabled()
    {
        var cut = RenderComponent<Button>(p => p
            .Add(b => b.Disabled, true)
            .AddChildContent("Save"));

        cut.Find("button").GetAttribute("aria-disabled").Should().Be("true");
    }

    [Fact]
    public void Button_Disabled_IsAlsoHtmlDisabled()
    {
        var cut = RenderComponent<Button>(p => p
            .Add(b => b.Disabled, true)
            .AddChildContent("Save"));

        cut.Find("button").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void Button_Loading_SetsAriaBusy()
    {
        var cut = RenderComponent<Button>(p => p
            .Add(b => b.IsLoading, true)
            .AddChildContent("Save"));

        cut.Find("button").GetAttribute("aria-busy").Should().Be("true");
    }

    [Fact]
    public async Task Button_Click_InvokesOnClick()
    {
        bool clicked = false;
        var cut = RenderComponent<Button>(p => p
            .Add(b => b.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => clicked = true))
            .AddChildContent("Click"));

        await cut.Find("button").ClickAsync(new MouseEventArgs());

        clicked.Should().BeTrue();
    }

    [Fact]
    public async Task Button_EnterKey_InvokesOnClick()
    {
        bool clicked = false;
        var cut = RenderComponent<Button>(p => p
            .Add(b => b.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => clicked = true))
            .AddChildContent("Click"));

        await cut.Find("button").KeyDownAsync(new KeyboardEventArgs { Key = "Enter" });

        clicked.Should().BeTrue();
    }

    [Fact]
    public async Task Button_SpaceKey_InvokesOnClick()
    {
        bool clicked = false;
        var cut = RenderComponent<Button>(p => p
            .Add(b => b.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => clicked = true))
            .AddChildContent("Click"));

        await cut.Find("button").KeyDownAsync(new KeyboardEventArgs { Key = " " });

        clicked.Should().BeTrue();
    }

    [Fact]
    public async Task Button_Disabled_DoesNotFireOnClick()
    {
        bool clicked = false;
        var cut = RenderComponent<Button>(p => p
            .Add(b => b.Disabled, true)
            .Add(b => b.OnClick, EventCallback.Factory.Create<MouseEventArgs>(this, _ => clicked = true))
            .AddChildContent("Click"));

        await cut.Find("button").ClickAsync(new MouseEventArgs());

        clicked.Should().BeFalse();
    }

    [Theory]
    [InlineData("default", "bg-primary")]
    [InlineData("destructive", "bg-destructive")]
    [InlineData("outline", "border-input")]
    [InlineData("secondary", "bg-secondary")]
    [InlineData("ghost", "hover:bg-accent")]
    [InlineData("link", "underline-offset-4")]
    public void Button_Variant_AppliesCorrectClass(string variant, string expectedClass)
    {
        var cut = RenderComponent<Button>(p => p
            .Add(b => b.Variant, variant)
            .AddChildContent("X"));

        cut.Find("button").ClassName.Should().Contain(expectedClass);
    }

    [Theory]
    [InlineData("sm", "h-9")]
    [InlineData("lg", "h-11")]
    [InlineData("icon", "h-10 w-10")]
    [InlineData("default", "h-10 px-4")]
    public void Button_Size_AppliesCorrectClass(string size, string expectedClass)
    {
        var cut = RenderComponent<Button>(p => p
            .Add(b => b.Size, size)
            .AddChildContent("X"));

        cut.Find("button").ClassName.Should().Contain(expectedClass.Split(' ')[0]);
    }
}
