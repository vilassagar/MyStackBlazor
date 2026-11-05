using System.Globalization;
using Microsoft.AspNetCore.Components.Forms;
using MyStackBlazor.Components.Form;

namespace MyStackBlazor.UnitTests.Components;

public class ExistingInputImprovementTests : TestContext
{
    private sealed class AmountModel
    {
        public decimal? Amount { get; set; }
        public double? Quantity { get; set; }
    }

    [Theory]
    [InlineData("en-IN", "1,23,456.78", "123456.78")]
    [InlineData("de-DE", "1.234,56", "1234.56")]
    [InlineData("en-US", "$1,234.56", "1234.56")]
    public void Currency_Parses_Using_The_Selected_Culture(string culture, string text, string expected)
    {
        decimal? result = null;
        var cut = RenderComponent<StackCurrencyInput>(p => p
            .Add(c => c.Culture, CultureInfo.GetCultureInfo(culture))
            .Add(c => c.ValueChanged, value => result = value));
        cut.Find("input").Input(text);
        result.Should().Be(decimal.Parse(expected, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void Currency_Formats_Indian_Grouping_And_Preserves_Partial_Input()
    {
        var cut = RenderComponent<StackCurrencyInput>(p => p
            .Add(c => c.Culture, CultureInfo.GetCultureInfo("en-IN"))
            .Add(c => c.Value, 123456.78m));
        cut.Find("input").GetAttribute("value").Should().Be("1,23,456.78");
        cut.Find("input").Focus();
        cut.Find("input").Input("12.");
        cut.Find("input").GetAttribute("value").Should().Be("12.");
        cut.Find("input").Blur();
        cut.Find("input").GetAttribute("value").Should().Be("12.00");
    }

    [Theory]
    [InlineData("invalid")]
    [InlineData("12.345")]
    [InlineData("101")]
    public void Currency_Invalid_Text_Preserves_Model_And_Blocks_Form_Validation(string text)
    {
        var model = new AmountModel { Amount = 10m };
        var context = new EditContext(model);
        var cut = RenderComponent<CascadingValue<EditContext>>(p => p
            .Add(c => c.Value, context)
            .AddChildContent<StackCurrencyInput>(p => p
                .Add(c => c.Value, model.Amount)
                .Add(c => c.Max, 100m)
                .Add(c => c.Culture, CultureInfo.InvariantCulture)
                .Add(c => c.ValueExpression, () => model.Amount)
                .Add(c => c.ValueChanged, value => model.Amount = value)));
        cut.Find("input").Input(text);
        cut.Find("input").Blur();
        cut.Find("input").GetAttribute("value").Should().Be(text);
        model.Amount.Should().Be(10m);
        context.Validate().Should().BeFalse();
        context.GetValidationMessages(new FieldIdentifier(model, nameof(model.Amount))).Should().ContainSingle();
        cut.Find("input").GetAttribute("aria-invalid").Should().Be("true");
        cut.Find("input").Input("25");
        model.Amount.Should().Be(25m);
        context.Validate().Should().BeTrue();
    }

    [Fact]
    public void Numeric_Uses_Invariant_Html_Number_Parsing()
    {
        var previous = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");
            double? result = null;
            var cut = RenderComponent<StackNumericInput>(p => p
                .Add(c => c.Value, 1.5)
                .Add(c => c.Step, 0.5)
                .Add(c => c.ValueChanged, value => result = value));
            cut.Find("input").GetAttribute("value").Should().Be("1.5");
            cut.Find("input").GetAttribute("inputmode").Should().Be("decimal");
            cut.Find("input").Input("2.75");
            result.Should().Be(2.75);
        }
        finally { CultureInfo.CurrentCulture = previous; }
    }

    [Fact]
    public void Numeric_Rejects_Nonfinite_Values_And_Clears_Validation_On_Recovery()
    {
        var model = new AmountModel { Quantity = 5 };
        var context = new EditContext(model);
        var cut = RenderComponent<CascadingValue<EditContext>>(p => p
            .Add(c => c.Value, context)
            .AddChildContent<StackNumericInput>(p => p
                .Add(c => c.Value, model.Quantity)
                .Add(c => c.ValueExpression, () => model.Quantity)
                .Add(c => c.ValueChanged, value => model.Quantity = value)));
        cut.Find("input").Input("1e-");
        model.Quantity.Should().Be(5);
        context.Validate().Should().BeFalse();
        cut.Find("input").Input("");
        model.Quantity.Should().BeNull();
        context.Validate().Should().BeTrue();
    }

    [Fact]
    public void Currency_Stepper_Clamp_And_ReadOnly_Are_Enforced_By_Event_Handlers()
    {
        decimal? result = null;
        var cut = RenderComponent<StackCurrencyInput>(p => p
            .Add(c => c.Min, 0m).Add(c => c.Max, 10m).Add(c => c.Value, 9m).Add(c => c.Step, 2m)
            .Add(c => c.ValueChanged, value => result = value));
        cut.Find("button[aria-label='Increase value']").Click();
        result.Should().Be(10m);
        cut.SetParametersAndRender(p => p.Add(c => c.ReadOnly, true));
        cut.Find("input").Input("2");
        cut.Find("button[aria-label='Decrease value']").Click();
        result.Should().Be(10m);
    }

    [Fact]
    public void Numeric_Stepper_Notifies_EditContext_And_Clamps()
    {
        var model = new AmountModel { Quantity = 9 };
        var context = new EditContext(model);
        var notified = false;
        context.OnFieldChanged += (_, args) => notified = args.FieldIdentifier.FieldName == nameof(model.Quantity);
        var cut = RenderComponent<CascadingValue<EditContext>>(p => p
            .Add(c => c.Value, context)
            .AddChildContent<StackNumericInput>(p => p
                .Add(c => c.Value, model.Quantity).Add(c => c.Step, 2).Add(c => c.Max, 10)
                .Add(c => c.ValueExpression, () => model.Quantity)
                .Add(c => c.ValueChanged, value => model.Quantity = value)));
        cut.Find("button[aria-label='Increase value']").Click();
        model.Quantity.Should().Be(10);
        notified.Should().BeTrue();
    }

    [Fact]
    public void Currency_External_Value_Replacement_Clears_Stale_Parsing_Error()
    {
        var cut = RenderComponent<StackCurrencyInput>(p => p.Add(c => c.Value, 10m));
        cut.Find("input").Input("invalid");
        cut.SetParametersAndRender(p => p.Add(c => c.Value, 20m));
        cut.Find("input").HasAttribute("aria-invalid").Should().BeFalse();
    }
}
