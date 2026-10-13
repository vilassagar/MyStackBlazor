using System.Globalization;
using MyStackBlazor.Components.Form;

namespace MyStackBlazor.UnitTests.Components;

public class TextBoxTests : TestContext
{
    [Fact]
    public async Task Debounce_Delays_ValueChanged_And_Change_Flushes()
    {
        var values = new List<string?>();
        object? changed = null;
        var cut = RenderComponent<StackTextBox>(p => p
            .Add(t => t.DebounceDelay, 50)
            .Add(t => t.ValueChanged, v => values.Add(v))
            .Add(t => t.OnChange, v => changed = v));

        var first = cut.Find("input").InputAsync(new ChangeEventArgs { Value = "he" });
        await cut.Find("input").InputAsync(new ChangeEventArgs { Value = "hello" });
        await first;
        values.Should().Equal("hello");

        await cut.Find("input").ChangeAsync(new ChangeEventArgs { Value = "hello!" });
        values.Should().Equal("hello", "hello!");
        changed.Should().Be("hello!");
    }

    [Fact]
    public void Telerik_Attributes_Render_On_Input()
    {
        var cut = RenderComponent<StackTextBox>(p => p
            .Add(t => t.MaxLength, 10)
            .Add(t => t.InputMode, "email")
            .Add(t => t.AutoComplete, "email")
            .Add(t => t.SpellCheck, false)
            .Add(t => t.Name, "mail")
            .Add(t => t.Password, true)
            .Add(t => t.Title, "Your e-mail"));
        var input = cut.Find("input");
        input.GetAttribute("maxlength").Should().Be("10");
        input.GetAttribute("inputmode").Should().Be("email");
        input.GetAttribute("autocomplete").Should().Be("email");
        input.GetAttribute("spellcheck").Should().Be("false");
        input.GetAttribute("name").Should().Be("mail");
        input.GetAttribute("type").Should().Be("password");
        cut.Find("span[title]").GetAttribute("title").Should().Be("Your e-mail");
    }

    [Fact]
    public void Adornments_And_Clear_Button()
    {
        string? value = "abc";
        var cut = RenderComponent<StackTextBox>(p => p
            .Add(t => t.Value, "abc")
            .Add(t => t.ShowClearButton, true)
            .Add(t => t.TextBoxPrefixTemplate, "<b class='pre'>@</b>")
            .Add(t => t.TextBoxSuffixTemplate, "<b class='suf'>.com</b>")
            .Add(t => t.ValueChanged, v => value = v));
        cut.Find(".pre").Should().NotBeNull();
        cut.Find(".suf").Should().NotBeNull();
        cut.Find("button[aria-label=Clear]").Click();
        value.Should().BeEmpty();
    }

    [Fact]
    public void Enabled_False_And_Appearance()
    {
        var cut = RenderComponent<StackTextBox>(p => p
            .Add(t => t.Enabled, false)
            .Add(t => t.Size, "sm")
            .Add(t => t.Rounded, "full"));
        cut.Find("input").HasAttribute("disabled").Should().BeTrue();
        cut.Find("span").ClassList.Should().Contain(["h-8", "rounded-full", "opacity-50"]);
    }
}

public class TextAreaTests : TestContext
{
    public TextAreaTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Counter_And_MaxLength()
    {
        var cut = RenderComponent<StackTextarea>(p => p
            .Add(t => t.Value, "hello")
            .Add(t => t.MaxLength, 20)
            .Add(t => t.ShowCounter, true));
        cut.Find("textarea").GetAttribute("maxlength").Should().Be("20");
        cut.Find("[aria-live]").TextContent.Should().Be("5 / 20");
    }

    [Theory]
    [InlineData(TextAreaResizeMode.None, "resize-none")]
    [InlineData(TextAreaResizeMode.Both, "resize")]
    [InlineData(TextAreaResizeMode.Horizontal, "resize-x")]
    [InlineData(TextAreaResizeMode.Vertical, "resize-y")]
    public void ResizeMode_Classes(TextAreaResizeMode mode, string expected)
    {
        var cut = RenderComponent<StackTextarea>(p => p.Add(t => t.ResizeMode, mode));
        cut.Find("textarea").ClassList.Should().Contain(expected);
    }

    [Fact]
    public void AutoSize_Calls_Js_And_Disables_Resize()
    {
        var cut = RenderComponent<StackTextarea>(p => p.Add(t => t.AutoSize, true));
        cut.Find("textarea").ClassList.Should().Contain("resize-none");
        JSInterop.VerifyInvoke("MyStackBlazor.textAreaAutoSize");
    }

    [Fact]
    public async Task OnChange_Receives_Value()
    {
        object? changed = null;
        var cut = RenderComponent<StackTextarea>(p => p.Add(t => t.OnChange, v => changed = v));
        await cut.Find("textarea").ChangeAsync(new ChangeEventArgs { Value = "notes" });
        changed.Should().Be("notes");
    }
}

public class NumericTextBoxTests : TestContext
{
    public NumericTextBoxTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = new CultureInfo("en-US");
    }

    [Fact]
    public void Shows_Format_When_Not_Focused_And_Plain_Number_When_Focused()
    {
        var cut = RenderComponent<StackNumericTextBox<decimal>>(p => p
            .Add(n => n.Value, 1234.5m)
            .Add(n => n.Format, "C2"));
        cut.Find("input").GetAttribute("value").Should().Be("$1,234.50");
        cut.Find("input").Focus();
        cut.Find("input").GetAttribute("value").Should().Be("1234.5");
    }

    [Fact]
    public async Task Typing_Parses_With_Culture()
    {
        decimal? value = null;
        var cut = RenderComponent<StackNumericTextBox<decimal?>>(p => p
            .Add(n => n.DebounceDelay, 0)
            .Add(n => n.ValueChanged, v => value = v));
        await cut.Find("input").InputAsync(new ChangeEventArgs { Value = "42.75" });
        value.Should().Be(42.75m);
        await cut.Find("input").InputAsync(new ChangeEventArgs { Value = "" });
        value.Should().BeNull();
    }

    [Fact]
    public void Arrows_And_Keys_Step_Within_Min_Max()
    {
        int value = 9;
        var cut = RenderComponent<StackNumericTextBox<int>>(p => p
            .Add(n => n.Value, 9)
            .Add(n => n.Min, 0)
            .Add(n => n.Max, 10)
            .Add(n => n.Step, 2)
            .Add(n => n.ValueChanged, v => value = v));

        cut.Find("button[aria-label='Increase value']").Click();
        value.Should().Be(10, "clamped to Max");
        cut.Find("button[aria-label='Increase value']").HasAttribute("disabled").Should().BeTrue();

        cut.Find("input").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        value.Should().Be(8);
    }

    [Fact]
    public void Zero_Is_A_Real_Minimum_For_Int()
    {
        var cut = RenderComponent<StackNumericTextBox<int>>(p => p.Add(n => n.Value, 0).Add(n => n.Min, 0));
        cut.Find("button[aria-label='Decrease value']").HasAttribute("disabled").Should().BeTrue();
        cut.Find("input").GetAttribute("aria-valuemin").Should().Be("0");

        var unbounded = RenderComponent<StackNumericTextBox<int>>(p => p.Add(n => n.Value, 0));
        unbounded.Find("input").HasAttribute("aria-valuemin").Should().BeFalse();
    }

    [Fact]
    public async Task Out_Of_Range_Is_Clamped_On_Blur_And_Decimals_Round()
    {
        double? value = null;
        object? changed = null;
        var cut = RenderComponent<StackNumericTextBox<double?>>(p => p
            .Add(n => n.Max, 100)
            .Add(n => n.Decimals, 1)
            .Add(n => n.DebounceDelay, 0)
            .Add(n => n.ValueChanged, v => value = v)
            .Add(n => n.OnChange, v => changed = v));

        cut.Find("input").Focus();
        await cut.Find("input").InputAsync(new ChangeEventArgs { Value = "150" });
        value.Should().BeNull("out of range while typing");
        cut.Find("input").Blur();
        value.Should().Be(100);
        changed.Should().Be(100.0);

        cut.Find("input").Focus();
        await cut.Find("input").InputAsync(new ChangeEventArgs { Value = "3.14159" });
        value.Should().Be(3.1);
    }

    [Fact]
    public void Arrows_Can_Be_Hidden()
    {
        var cut = RenderComponent<StackNumericTextBox<int>>(p => p.Add(n => n.Arrows, false));
        cut.FindAll("button").Should().BeEmpty();
    }
}

public class MaskEngineTests
{
    [Theory]
    [InlineData("(999) 000-0000", "5551234567", "(555) 123-4567")]
    [InlineData("LL-0000", "ab1234", "ab-1234")]
    [InlineData("AAA", "a1b", "a1b")]
    public void Fills_Slots_In_Order(string mask, string input, string expected)
    {
        var engine = new MaskEngine(mask, null);
        engine.Format(engine.Fill(input, '_', false), '_').Should().Be(expected);
    }

    [Fact]
    public void Rejects_Characters_That_Fit_No_Slot()
    {
        var engine = new MaskEngine("000-000", null);
        engine.Format(engine.Fill("12x3", '_', false), '_').Should().Be("123-___");
    }

    [Fact]
    public void Value_With_And_Without_Literals()
    {
        var engine = new MaskEngine("00/00", null);
        var slots = engine.Fill("1225", '_', false);
        engine.Value(slots, includeLiterals: false, '_').Should().Be("1225");
        engine.Value(slots, includeLiterals: true, '_').Should().Be("12/25");
        engine.Fill("12/25", '_', inputHasLiterals: true).Should().Equal('1', '2', '2', '5');
    }

    [Fact]
    public void Custom_Rules_And_Escapes()
    {
        var engine = new MaskEngine(@"\0x HH", new Dictionary<char, string> { ['H'] = "[0-9A-Fa-f]" });
        engine.Format(engine.Fill("1fz", '_', false), '_').Should().Be("0x 1f");
    }

    [Fact]
    public void Caret_Skips_Literals()
    {
        var engine = new MaskEngine("(000) 000", null);
        engine.CaretAfterSlot(0).Should().Be(1);
        engine.CaretAfterSlot(3).Should().Be(6, "after ') '");
    }
}

public class MaskedTextBoxTests : TestContext
{
    public MaskedTextBoxTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Displays_Value_In_Mask()
    {
        var cut = RenderComponent<StackMaskedTextBox>(p => p
            .Add(m => m.Mask, "(999) 000-0000")
            .Add(m => m.Value, "5551234567"));
        cut.Find("input").GetAttribute("value").Should().Be("(555) 123-4567");
    }

    [Fact]
    public async Task Typing_Publishes_Value_Without_Literals()
    {
        string? value = null;
        var cut = RenderComponent<StackMaskedTextBox>(p => p
            .Add(m => m.Mask, "000-000")
            .Add(m => m.ValueChanged, v => value = v));
        cut.Find("input").Focus();
        await cut.Find("input").InputAsync(new ChangeEventArgs { Value = "123456" });
        value.Should().Be("123456");
        cut.Find("input").GetAttribute("value").Should().Be("123-456");
    }

    [Fact]
    public async Task IncludeLiterals_Keeps_Them()
    {
        string? value = null;
        var cut = RenderComponent<StackMaskedTextBox>(p => p
            .Add(m => m.Mask, "00/00")
            .Add(m => m.IncludeLiterals, true)
            .Add(m => m.ValueChanged, v => value = v));
        await cut.Find("input").InputAsync(new ChangeEventArgs { Value = "1225" });
        value.Should().Be("12/25");
    }

    [Fact]
    public void MaskOnFocus_Shows_Placeholder_Until_Focused()
    {
        var cut = RenderComponent<StackMaskedTextBox>(p => p
            .Add(m => m.Mask, "000")
            .Add(m => m.MaskOnFocus, true)
            .Add(m => m.Placeholder, "Code"));
        cut.Find("input").GetAttribute("value").Should().BeEmpty();
        cut.Find("input").Focus();
        cut.Find("input").GetAttribute("value").Should().Be("___");
    }

    [Fact]
    public void Earlier_Api_Still_Works()
    {
        var cut = RenderComponent<StackMaskedTextBox>(p => p
            .Add(m => m.Mask, "000-000-0000")
            .Add(m => m.Value, "5551234567")
            .AddUnmatched("data-test", "x"));
        cut.Find("input").GetAttribute("value").Should().Be("555-123-4567");
        cut.Find("div[data-test=x]").Should().NotBeNull("extra attributes stay on the wrapper");
    }
}
