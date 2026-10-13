using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using MyStackBlazor.Components.Form;

namespace MyStackBlazor.UnitTests.Components;

public class FormComponentTests : TestContext
{
    public enum Plan { Free, Pro }

    public class Person
    {
        [Required, Display(Name = "Full name")]
        public string? Name { get; set; }
        public int Age { get; set; } = 30;
        public bool Subscribed { get; set; }
        public DateTime? BirthDate { get; set; }
        public Plan Plan { get; set; } = Plan.Pro;
        [ScaffoldColumn(false)]
        public string Hidden { get; set; } = "";
        public List<string> Tags { get; set; } = [];
    }

    public FormComponentTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = new CultureInfo("en-US");
    }

    private static RenderFragment Validator => b =>
    {
        b.OpenComponent<DataAnnotationsValidator>(0);
        b.CloseComponent();
    };

    private static RenderFragment Item(string field, Action<RenderTreeBuilder, int>? extra = null) => b =>
    {
        b.OpenComponent<StackFormItem>(0);
        b.AddAttribute(1, nameof(StackFormItem.Field), field);
        extra?.Invoke(b, 2);
        b.CloseComponent();
    };

    [Fact]
    public void Generates_An_Editor_Per_Simple_Property()
    {
        var cut = RenderComponent<StackForm>(p => p.Add(f => f.Model, new Person()));

        cut.FindAll("[data-field]").Select(e => e.GetAttribute("data-field"))
           .Should().Equal("Name", "Age", "Subscribed", "BirthDate", "Plan");
        cut.Find("[data-field=Name] label").TextContent.Should().Contain("Full name").And.Contain("*");
        cut.Find("[data-field=Age] input[role=spinbutton]").GetAttribute("value").Should().Be("30");
        cut.Find("[data-field=Subscribed] [role=checkbox], [data-field=Subscribed] input[type=checkbox], [data-field=Subscribed] button").Should().NotBeNull();
        cut.Find("[data-field=BirthDate] [role=group]").Should().NotBeNull("a DateInput inside the DatePicker");
        cut.Find("[data-field=Plan] [role=combobox]").TextContent.Should().Contain("Pro");
        cut.Find("button[type=submit]").TextContent.Trim().Should().Be("Submit");
    }

    [Fact]
    public async Task Editing_Updates_The_Model_And_Fires_OnUpdate()
    {
        var person = new Person();
        var updates = 0;
        var cut = RenderComponent<StackForm>(p => p
            .Add(f => f.Model, person)
            .Add(f => f.OnUpdate, () => updates++));

        await cut.Find("[data-field=Name] input").InputAsync(new ChangeEventArgs { Value = "Ada" });
        person.Name.Should().Be("Ada");
        updates.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Invalid_Submit_Shows_Inline_Message()
    {
        EditContext? invalid = null;
        var cut = RenderComponent<StackForm>(p => p
            .Add(f => f.Model, new Person())
            .Add(f => f.FormValidation, Validator)
            .Add(f => f.OnInvalidSubmit, c => invalid = c));

        cut.Find("form").Submit();
        invalid.Should().NotBeNull();
        cut.Find("[data-field=Name] [role=alert]").TextContent.Should().Contain("required");
    }

    [Fact]
    public void Valid_Submit_And_Tooltip_Messages()
    {
        EditContext? valid = null;
        var ok = RenderComponent<StackForm>(p => p
            .Add(f => f.Model, new Person { Name = "Ada" })
            .Add(f => f.FormValidation, Validator)
            .Add(f => f.OnValidSubmit, c => valid = c));
        ok.Find("form").Submit();
        valid.Should().NotBeNull();

        var tooltip = RenderComponent<StackForm>(p => p
            .Add(f => f.Model, new Person())
            .Add(f => f.FormValidation, Validator)
            .Add(f => f.ValidationMessageType, FormValidationMessageType.Tooltip));
        tooltip.Find("form").Submit();
        tooltip.Find("[data-field=Name] [role=alert]").ClassList.Should().Contain("absolute");
    }

    [Fact]
    public void Declared_Items_Columns_ColSpan_Hint_And_EditorType()
    {
        var cut = RenderComponent<StackForm>(p => p
            .Add(f => f.Model, new Person())
            .Add(f => f.Columns, 2)
            .Add(f => f.Orientation, FormOrientation.Horizontal)
            .Add(f => f.ButtonsLayout, FormButtonsLayout.End)
            .Add(f => f.FormItems, (RenderFragment)(b =>
            {
                Item("Name", (x, s) => { x.AddAttribute(s, nameof(StackFormItem.Hint), "As on your passport"); x.AddAttribute(s + 1, nameof(StackFormItem.ColSpan), 2); })(b);
                Item("Subscribed", (x, s) => x.AddAttribute(s, nameof(StackFormItem.EditorType), FormEditorType.Switch))(b);
            })));

        cut.Find(".grid").GetAttribute("style").Should().StartWith("grid-template-columns:repeat(2");
        cut.Find("[data-field=Name]").GetAttribute("style").Should().Be("grid-column:span 2");
        cut.Find("[data-field=Name]").ClassList.Should().Contain("grid-cols-[minmax(0,1fr)_minmax(0,2fr)]");
        cut.Find("[data-field=Name] p.text-muted-foreground").TextContent.Should().Be("As on your passport");
        cut.Find("[data-field=Subscribed] [role=switch]").Should().NotBeNull();
        cut.Find("button[type=submit]").ParentElement!.ClassList.Should().Contain("justify-end");
    }

    [Fact]
    public void Group_Template_And_Custom_Buttons()
    {
        var cut = RenderComponent<StackForm>(p => p
            .Add(f => f.Model, new Person())
            .Add(f => f.FormItems, (RenderFragment)(b =>
            {
                b.OpenComponent<StackFormGroup>(0);
                b.AddAttribute(1, nameof(StackFormGroup.LabelText), "Account");
                b.AddAttribute(2, nameof(StackFormGroup.ChildContent), Item("Name", (x, s) =>
                    x.AddAttribute(s, nameof(StackFormItem.Template), (RenderFragment)(t => t.AddMarkupContent(0, "<i class='custom'>custom editor</i>")))));
                b.CloseComponent();
            }))
            .Add(f => f.FormButtons, "<button class='save'>Save</button>"));

        cut.Find("fieldset legend").TextContent.Should().Be("Account");
        cut.Find("[data-field=Name] .custom").Should().NotBeNull();
        cut.Find(".save").Should().NotBeNull();
        cut.FindAll("button[type=submit]").Should().BeEmpty();
    }

    [Fact]
    public void Enabled_False_Disables_Generated_Editors()
    {
        var cut = RenderComponent<StackForm>(p => p.Add(f => f.Model, new Person()).Add(f => f.Enabled, false));
        cut.Find("[data-field=Name] input").HasAttribute("disabled").Should().BeTrue();
        cut.Find("button[type=submit]").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void ValidationTooltip_Shows_When_Invalid()
    {
        var person = new Person();
        var context = new EditContext(person);
        var messages = new ValidationMessageStore(context);
        var cut = RenderComponent<CascadingValue<EditContext>>(p => p
            .Add(c => c.Value, context)
            .Add(c => c.ChildContent, (RenderFragment)(b =>
            {
                b.OpenComponent<StackValidationTooltip<string?>>(0);
                b.AddAttribute(1, "For", (System.Linq.Expressions.Expression<Func<string?>>)(() => person.Name));
                b.AddAttribute(2, "TargetSelector", "#name");
                b.CloseComponent();
            })));

        cut.FindAll("[role=alert]").Should().BeEmpty();
        messages.Add(context.Field(nameof(Person.Name)), "Name is required.");
        cut.InvokeAsync(context.NotifyValidationStateChanged);
        cut.Find("[role=alert]").TextContent.Should().Be("Name is required.");
        JSInterop.VerifyInvoke("MyStackBlazor.anchorTo");
    }
}
