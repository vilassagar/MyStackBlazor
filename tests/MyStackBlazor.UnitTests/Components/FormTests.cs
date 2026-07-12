using MyStackBlazor.Components.Form;

namespace MyStackBlazor.UnitTests.Components;

// ═══════════════════════════════════════════════════════════════════════════════
// Switch
// ═══════════════════════════════════════════════════════════════════════════════

public class SwitchTests : TestContext
{
    [Fact]
    public void Switch_Renders_RoleSwitch()
    {
        var cut = RenderComponent<Switch>();
        cut.Find("[role=switch]").Should().NotBeNull();
    }

    [Fact]
    public void Switch_Unchecked_AriaCheckedFalse()
    {
        var cut = RenderComponent<Switch>(p => p.Add(s => s.Checked, false));
        cut.Find("[role=switch]").GetAttribute("aria-checked").Should().Be("false");
    }

    [Fact]
    public void Switch_Checked_AriaCheckedTrue()
    {
        var cut = RenderComponent<Switch>(p => p.Add(s => s.Checked, true));
        cut.Find("[role=switch]").GetAttribute("aria-checked").Should().Be("true");
    }

    [Fact]
    public void Switch_Checked_AppliesPrimaryClass()
    {
        var cut = RenderComponent<Switch>(p => p.Add(s => s.Checked, true));
        cut.Find("[role=switch]").ClassName.Should().Contain("bg-primary");
    }

    [Fact]
    public void Switch_Unchecked_AppliesTintClass()
    {
        var cut = RenderComponent<Switch>(p => p.Add(s => s.Checked, false));
        cut.Find("[role=switch]").ClassName.Should().Contain("bg-muted");
    }

    [Fact]
    public void Switch_Disabled_SetOnButton()
    {
        var cut = RenderComponent<Switch>(p => p.Add(s => s.Disabled, true));
        cut.Find("[role=switch]").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void Switch_Label_Rendered_WhenProvided()
    {
        var cut = RenderComponent<Switch>(p => p.Add(s => s.Label, "Enable notifications"));
        cut.Find("label").TextContent.Should().Contain("Enable notifications");
    }

    [Fact]
    public void Switch_NoLabel_WhenNotProvided()
    {
        var cut = RenderComponent<Switch>();
        cut.FindAll("label").Count.Should().Be(0);
    }

    [Fact]
    public async Task Switch_Click_TogglesCheckedState()
    {
        bool? newValue = null;
        var cut = RenderComponent<Switch>(p => p
            .Add(s => s.Checked, false)
            .Add(s => s.CheckedChanged,
                EventCallback.Factory.Create<bool>(this, v => newValue = v)));

        await cut.Find("[role=switch]").ClickAsync(new MouseEventArgs());

        newValue.Should().BeTrue();
    }

    [Fact]
    public async Task Switch_Disabled_Click_DoesNotFireCallback()
    {
        bool fired = false;
        var cut = RenderComponent<Switch>(p => p
            .Add(s => s.Disabled, true)
            .Add(s => s.CheckedChanged,
                EventCallback.Factory.Create<bool>(this, _ => fired = true)));

        await cut.Find("[role=switch]").ClickAsync(new MouseEventArgs());

        fired.Should().BeFalse();
    }

    [Fact]
    public void Switch_Id_SetOnButton()
    {
        var cut = RenderComponent<Switch>(p => p.Add(s => s.Id, "my-switch"));
        cut.Find("[role=switch]").GetAttribute("id").Should().Be("my-switch");
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Checkbox
// ═══════════════════════════════════════════════════════════════════════════════

public class CheckboxTests : TestContext
{
    [Fact]
    public void Checkbox_Renders_RoleCheckbox()
    {
        var cut = RenderComponent<Checkbox>();
        cut.Find("[role=checkbox]").Should().NotBeNull();
    }

    [Fact]
    public void Checkbox_Unchecked_AriaCheckedFalse()
    {
        var cut = RenderComponent<Checkbox>(p => p.Add(c => c.Checked, false));
        cut.Find("[role=checkbox]").GetAttribute("aria-checked").Should().Be("false");
    }

    [Fact]
    public void Checkbox_Checked_AriaCheckedTrue()
    {
        var cut = RenderComponent<Checkbox>(p => p.Add(c => c.Checked, true));
        cut.Find("[role=checkbox]").GetAttribute("aria-checked").Should().Be("true");
    }

    [Fact]
    public void Checkbox_Checked_RendersCheckmarkSvg()
    {
        var cut = RenderComponent<Checkbox>(p => p.Add(c => c.Checked, true));
        cut.FindAll("svg").Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Checkbox_Unchecked_HidesCheckmarkSvg()
    {
        var cut = RenderComponent<Checkbox>(p => p.Add(c => c.Checked, false));
        cut.FindAll("svg").Count.Should().Be(0);
    }

    [Fact]
    public void Checkbox_Disabled_SetOnButton()
    {
        var cut = RenderComponent<Checkbox>(p => p.Add(c => c.Disabled, true));
        cut.Find("[role=checkbox]").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void Checkbox_Label_Rendered()
    {
        var cut = RenderComponent<Checkbox>(p => p.Add(c => c.Label, "Accept terms"));
        cut.Find("label").TextContent.Should().Contain("Accept terms");
    }

    [Fact]
    public async Task Checkbox_Click_TogglesCheckedChanged()
    {
        bool? newValue = null;
        var cut = RenderComponent<Checkbox>(p => p
            .Add(c => c.Checked, false)
            .Add(c => c.CheckedChanged,
                EventCallback.Factory.Create<bool>(this, v => newValue = v)));

        await cut.Find("[role=checkbox]").ClickAsync(new MouseEventArgs());

        newValue.Should().BeTrue();
    }

    [Fact]
    public async Task Checkbox_Disabled_Click_DoesNotFireCallback()
    {
        bool fired = false;
        var cut = RenderComponent<Checkbox>(p => p
            .Add(c => c.Disabled, true)
            .Add(c => c.CheckedChanged,
                EventCallback.Factory.Create<bool>(this, _ => fired = true)));

        await cut.Find("[role=checkbox]").ClickAsync(new MouseEventArgs());

        fired.Should().BeFalse();
    }

    [Fact]
    public void Checkbox_Id_SetOnButton()
    {
        var cut = RenderComponent<Checkbox>(p => p.Add(c => c.Id, "agree"));
        cut.Find("[role=checkbox]").GetAttribute("id").Should().Be("agree");
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Input
// ═══════════════════════════════════════════════════════════════════════════════

public class InputTests : TestContext
{
    [Fact]
    public void Input_Renders_InputElement()
    {
        var cut = RenderComponent<Input>();
        cut.Find("input").Should().NotBeNull();
    }

    [Theory]
    [InlineData("text")]
    [InlineData("email")]
    [InlineData("password")]
    [InlineData("number")]
    [InlineData("search")]
    public void Input_Type_SetOnElement(string type)
    {
        var cut = RenderComponent<Input>(p => p.Add(i => i.Type, type));
        cut.Find("input").GetAttribute("type").Should().Be(type);
    }

    [Fact]
    public void Input_Placeholder_SetOnElement()
    {
        var cut = RenderComponent<Input>(p => p.Add(i => i.Placeholder, "Enter your name…"));
        cut.Find("input").GetAttribute("placeholder").Should().Be("Enter your name…");
    }

    [Fact]
    public void Input_Value_SetOnElement()
    {
        var cut = RenderComponent<Input>(p => p.Add(i => i.Value, "hello"));
        cut.Find("input").GetAttribute("value").Should().Be("hello");
    }

    [Fact]
    public void Input_Disabled_SetOnElement()
    {
        var cut = RenderComponent<Input>(p => p.Add(i => i.Disabled, true));
        cut.Find("input").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void Input_ReadOnly_SetOnElement()
    {
        var cut = RenderComponent<Input>(p => p.Add(i => i.ReadOnly, true));
        cut.Find("input").HasAttribute("readonly").Should().BeTrue();
    }

    [Fact]
    public void Input_Id_SetOnElement()
    {
        var cut = RenderComponent<Input>(p => p.Add(i => i.Id, "email-field"));
        cut.Find("input").GetAttribute("id").Should().Be("email-field");
    }

    [Fact]
    public async Task Input_Change_FiresValueChanged()
    {
        string? result = null;
        var cut = RenderComponent<Input>(p => p
            .Add(i => i.ValueChanged,
                EventCallback.Factory.Create<string?>(this, v => result = v)));

        // Component uses @oninput for ValueChanged, so InputAsync triggers the callback
        await cut.Find("input").InputAsync(new ChangeEventArgs { Value = "typed text" });

        result.Should().Be("typed text");
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Textarea
// ═══════════════════════════════════════════════════════════════════════════════

public class TextareaTests : TestContext
{
    [Fact]
    public void Textarea_Renders_TextareaElement()
    {
        var cut = RenderComponent<Textarea>();
        cut.Find("textarea").Should().NotBeNull();
    }

    [Fact]
    public void Textarea_Placeholder_SetOnElement()
    {
        var cut = RenderComponent<Textarea>(p => p.Add(t => t.Placeholder, "Write something…"));
        cut.Find("textarea").GetAttribute("placeholder").Should().Be("Write something…");
    }

    [Fact]
    public void Textarea_Rows_SetOnElement()
    {
        var cut = RenderComponent<Textarea>(p => p.Add(t => t.Rows, 6));
        cut.Find("textarea").GetAttribute("rows").Should().Be("6");
    }

    [Fact]
    public void Textarea_Disabled_SetOnElement()
    {
        var cut = RenderComponent<Textarea>(p => p.Add(t => t.Disabled, true));
        cut.Find("textarea").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void Textarea_ReadOnly_SetOnElement()
    {
        var cut = RenderComponent<Textarea>(p => p.Add(t => t.ReadOnly, true));
        cut.Find("textarea").HasAttribute("readonly").Should().BeTrue();
    }

    [Fact]
    public async Task Textarea_Change_FiresValueChanged()
    {
        string? result = null;
        var cut = RenderComponent<Textarea>(p => p
            .Add(t => t.ValueChanged,
                EventCallback.Factory.Create<string?>(this, v => result = v)));

        // Component uses @oninput, so InputAsync triggers ValueChanged
        await cut.Find("textarea").InputAsync(new ChangeEventArgs { Value = "some text" });

        result.Should().Be("some text");
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Select
// ═══════════════════════════════════════════════════════════════════════════════

public class SelectTests : TestContext
{
    [Fact]
    public void Select_Renders_SelectElement()
    {
        var cut = RenderComponent<Select>();
        cut.Find("select").Should().NotBeNull();
    }

    [Fact]
    public void Select_Disabled_SetOnElement()
    {
        var cut = RenderComponent<Select>(p => p.Add(s => s.Disabled, true));
        cut.Find("select").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void Select_Id_SetOnElement()
    {
        var cut = RenderComponent<Select>(p => p.Add(s => s.Id, "country-select"));
        cut.Find("select").GetAttribute("id").Should().Be("country-select");
    }

    [Fact]
    public void Select_ChildContent_RenderedAsOptions()
    {
        var cut = RenderComponent<Select>(p => p
            .AddChildContent("<option value=\"a\">Apple</option><option value=\"b\">Banana</option>"));

        cut.FindAll("option").Count.Should().Be(2);
    }

    [Fact]
    public async Task Select_Change_FiresValueChanged()
    {
        string? result = null;
        var cut = RenderComponent<Select>(p => p
            .Add(s => s.ValueChanged,
                EventCallback.Factory.Create<string?>(this, v => result = v))
            .AddChildContent("<option value=\"x\">X</option>"));

        await cut.Find("select").ChangeAsync(new ChangeEventArgs { Value = "x" });

        result.Should().Be("x");
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Slider
// ═══════════════════════════════════════════════════════════════════════════════

public class SliderTests : TestContext
{
    [Fact]
    public void Slider_Renders_RangeInput()
    {
        var cut = RenderComponent<Slider>();
        cut.Find("input[type=range]").Should().NotBeNull();
    }

    [Fact]
    public void Slider_Min_SetOnInput()
    {
        var cut = RenderComponent<Slider>(p => p.Add(s => s.Min, 10.0));
        cut.Find("input[type=range]").GetAttribute("min").Should().Be("10");
    }

    [Fact]
    public void Slider_Max_SetOnInput()
    {
        var cut = RenderComponent<Slider>(p => p.Add(s => s.Max, 200.0));
        cut.Find("input[type=range]").GetAttribute("max").Should().Be("200");
    }

    [Fact]
    public void Slider_Step_SetOnInput()
    {
        var cut = RenderComponent<Slider>(p => p.Add(s => s.Step, 5.0));
        cut.Find("input[type=range]").GetAttribute("step").Should().Be("5");
    }

    [Fact]
    public void Slider_Value_SetOnInput()
    {
        var cut = RenderComponent<Slider>(p => p.Add(s => s.Value, 42.0));
        cut.Find("input[type=range]").GetAttribute("value").Should().Be("42");
    }

    [Fact]
    public void Slider_Disabled_SetOnInput()
    {
        var cut = RenderComponent<Slider>(p => p.Add(s => s.Disabled, true));
        cut.Find("input[type=range]").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public async Task Slider_Change_FiresValueChanged()
    {
        double? result = null;
        var cut = RenderComponent<Slider>(p => p
            .Add(s => s.Min, 0.0)
            .Add(s => s.Max, 100.0)
            .Add(s => s.ValueChanged,
                EventCallback.Factory.Create<double>(this, v => result = v)));

        // Component uses @oninput, so InputAsync triggers the callback
        await cut.Find("input[type=range]").InputAsync(new ChangeEventArgs { Value = "75" });

        result.Should().Be(75.0);
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Rating
// ═══════════════════════════════════════════════════════════════════════════════

public class RatingTests : TestContext
{
    [Fact]
    public void Rating_Renders_RoleGroup()
    {
        var cut = RenderComponent<Rating>();
        cut.Find("[role=group]").Should().NotBeNull();
    }

    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(7)]
    public void Rating_Max_RendersCorrectNumberOfStars(int max)
    {
        // Step=1 renders one button per star; default Step=0.5 renders two (half + full) per star
        var cut = RenderComponent<Rating>(p => p
            .Add(r => r.Max, max)
            .Add(r => r.Step, 1m));
        cut.FindAll("button").Count.Should().Be(max);
    }

    [Fact]
    public void Rating_Disabled_RendersNoInteractiveButtons()
    {
        var cut = RenderComponent<Rating>(p => p
            .Add(r => r.Max, 5)
            .Add(r => r.Disabled, true));

        // Disabled rating renders no interactive buttons — the hotspot buttons are not emitted
        cut.FindAll("button").Count.Should().Be(0);
    }

    [Fact]
    public async Task Rating_StarClick_FiresValueChanged()
    {
        decimal? result = null;
        var cut = RenderComponent<Rating>(p => p
            .Add(r => r.Max, 5)
            .Add(r => r.ValueChanged,
                EventCallback.Factory.Create<decimal>(this, v => result = v)));

        await cut.FindAll("button")[2].ClickAsync(new MouseEventArgs());

        result.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Rating_ReadOnly_DoesNotThrow()
    {
        var act = () => RenderComponent<Rating>(p => p
            .Add(r => r.ReadOnly, true)
            .Add(r => r.Value, 3.5m));

        act.Should().NotThrow();
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// NumericInput
// ═══════════════════════════════════════════════════════════════════════════════

public class NumericInputTests : TestContext
{
    [Fact]
    public void NumericInput_Renders_NumberInput()
    {
        var cut = RenderComponent<NumericInput>();
        cut.Find("input[type=number]").Should().NotBeNull();
    }

    [Fact]
    public void NumericInput_Placeholder_SetOnInput()
    {
        var cut = RenderComponent<NumericInput>(p => p.Add(n => n.Placeholder, "0.00"));
        cut.Find("input[type=number]").GetAttribute("placeholder").Should().Be("0.00");
    }

    [Fact]
    public void NumericInput_Disabled_SetOnInput()
    {
        var cut = RenderComponent<NumericInput>(p => p.Add(n => n.Disabled, true));
        cut.Find("input[type=number]").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void NumericInput_Min_SetOnInput()
    {
        var cut = RenderComponent<NumericInput>(p => p.Add(n => n.Min, 0.0));
        cut.Find("input[type=number]").GetAttribute("min").Should().Be("0");
    }

    [Fact]
    public void NumericInput_Max_SetOnInput()
    {
        var cut = RenderComponent<NumericInput>(p => p.Add(n => n.Max, 999.0));
        cut.Find("input[type=number]").GetAttribute("max").Should().Be("999");
    }

    [Fact]
    public void NumericInput_ShowStepper_RendersStepperButtons()
    {
        var cut = RenderComponent<NumericInput>(p => p.Add(n => n.ShowStepper, true));
        cut.FindAll("button").Count.Should().BeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task NumericInput_Change_FiresValueChanged()
    {
        double? result = null;
        var cut = RenderComponent<NumericInput>(p => p
            .Add(n => n.ValueChanged,
                EventCallback.Factory.Create<double?>(this, v => result = v)));

        // Component uses @oninput, so InputAsync triggers ValueChanged
        await cut.Find("input[type=number]").InputAsync(new ChangeEventArgs { Value = "42" });

        result.Should().Be(42.0);
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// RadioGroup
// ═══════════════════════════════════════════════════════════════════════════════

public class RadioGroupTests : TestContext
{
    [Fact]
    public void RadioGroup_Renders_RoleRadiogroup()
    {
        var cut = RenderComponent<RadioGroup>();
        cut.Find("[role=radiogroup]").Should().NotBeNull();
    }

    [Fact]
    public void RadioGroup_ChildContent_Rendered()
    {
        var cut = RenderComponent<RadioGroup>(p => p
            .AddChildContent("<div data-item>Option A</div>"));

        cut.Find("[data-item]").Should().NotBeNull();
    }

    [Fact]
    public void RadioGroup_Class_AppliedToRoot()
    {
        var cut = RenderComponent<RadioGroup>(p => p.Add(r => r.Class, "flex gap-4"));
        cut.Find("[role=radiogroup]").ClassName.Should().Contain("flex");
    }
}
