using MyStackBlazor.Components.Form;

namespace MyStackBlazor.UnitTests.Components;

public class DropDownListTests : TestContext
{
    private record Product(int Id, string Name, string Category);

    private static readonly List<Product> Products =
    [
        new(1, "Apple",  "Fruit"),
        new(2, "Banana", "Fruit"),
        new(3, "Carrot", "Vegetable"),
        new(4, "Cherry", "Fruit"),
    ];

    public DropDownListTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<StackDropDownList<Product, int>> Render(
        Action<ComponentParameterCollectionBuilder<StackDropDownList<Product, int>>>? configure = null) =>
        RenderComponent<StackDropDownList<Product, int>>(p =>
        {
            p.Add(d => d.Data, Products)
             .Add(d => d.TextField, nameof(Product.Name))
             .Add(d => d.ValueField, nameof(Product.Id));
            configure?.Invoke(p);
        });

    [Fact]
    public void Shows_DefaultText_When_No_Value()
    {
        var cut = Render(p => p.Add(d => d.DefaultText, "Pick one"));
        cut.Find("[role=combobox]").TextContent.Should().Contain("Pick one");
    }

    [Fact]
    public void Shows_Text_Of_Selected_Value()
    {
        var cut = Render(p => p.Add(d => d.Value, 3));
        cut.Find("[role=combobox]").TextContent.Should().Contain("Carrot");
    }

    [Fact]
    public void Without_DefaultText_Shows_First_Item()
    {
        var cut = Render();
        cut.Find("[role=combobox]").TextContent.Should().Contain("Apple");
    }

    [Fact]
    public void Click_Opens_Popup_With_All_Items()
    {
        var cut = Render();
        cut.Find("[role=combobox]").Click();
        cut.FindAll("[role=option]").Count.Should().Be(4);
        cut.Find("[role=combobox]").GetAttribute("aria-expanded").Should().Be("true");
    }

    [Fact]
    public void DefaultText_Is_Rendered_As_First_Option()
    {
        var cut = Render(p => p.Add(d => d.DefaultText, "None"));
        cut.Find("[role=combobox]").Click();
        var options = cut.FindAll("[role=option]");
        options.Count.Should().Be(5);
        options[0].TextContent.Trim().Should().Be("None");
    }

    [Fact]
    public void Selecting_Item_Fires_ValueChanged_And_OnChange_And_Closes()
    {
        int? value = null;
        object? changed = null;
        var cut = Render(p => p
            .Add(d => d.ValueChanged, v => value = v)
            .Add(d => d.OnChange, v => changed = v));

        cut.Find("[role=combobox]").Click();
        cut.FindAll("[role=option]")[1].Click();

        value.Should().Be(2);
        changed.Should().Be(2);
        cut.FindAll("[role=listbox]").Should().BeEmpty();
    }

    [Fact]
    public void Disabled_Does_Not_Open()
    {
        var cut = Render(p => p.Add(d => d.Enabled, false));
        var trigger = cut.Find("[role=combobox]");
        trigger.Click();
        cut.FindAll("[role=listbox]").Should().BeEmpty();
        trigger.GetAttribute("aria-disabled").Should().Be("true");
        trigger.GetAttribute("tabindex").Should().Be("-1");
    }

    [Fact]
    public void ReadOnly_Does_Not_Open()
    {
        var cut = Render(p => p.Add(d => d.ReadOnly, true));
        cut.Find("[role=combobox]").Click();
        cut.FindAll("[role=listbox]").Should().BeEmpty();
    }

    [Fact]
    public void OnOpen_Can_Cancel()
    {
        var cut = Render(p => p.Add(d => d.OnOpen, (DropDownListOpenEventArgs a) => a.IsCancelled = true));
        cut.Find("[role=combobox]").Click();
        cut.FindAll("[role=listbox]").Should().BeEmpty();
    }

    [Fact]
    public void OnClose_Can_Cancel()
    {
        var cut = Render(p => p.Add(d => d.OnClose, (DropDownListCloseEventArgs a) => a.IsCancelled = true));
        cut.Find("[role=combobox]").Click();
        cut.FindAll("[role=option]")[0].Click();
        cut.FindAll("[role=listbox]").Should().NotBeEmpty();
    }

    [Fact]
    public void Filter_StartsWith_Is_Default()
    {
        var cut = Render(p => p.Add(d => d.Filterable, true));
        cut.Find("[role=combobox]").Click();
        cut.Find("input[role=searchbox]").Input("c");
        cut.FindAll("[role=option]").Select(o => o.TextContent.Trim())
           .Should().BeEquivalentTo("Carrot", "Cherry");
    }

    [Fact]
    public void Filter_Contains_Operator()
    {
        var cut = Render(p => p
            .Add(d => d.Filterable, true)
            .Add(d => d.FilterOperator, StringFilterOperator.Contains));
        cut.Find("[role=combobox]").Click();
        cut.Find("input[role=searchbox]").Input("an");
        cut.FindAll("[role=option]").Select(o => o.TextContent.Trim())
           .Should().BeEquivalentTo("Banana");
    }

    private List<string> OpenAndFilter(IRenderedComponent<StackDropDownList<Product, int>> cut, string text)
    {
        cut.Find("[role=combobox]").Click();
        cut.Find("input[role=searchbox]").Input(text);
        return cut.FindAll("[role=option]").Select(o => o.TextContent.Trim()).ToList();
    }

    [Fact]
    public void MinLength_Delays_Filtering()
    {
        var cut = Render(p => p.Add(d => d.Filterable, true).Add(d => d.MinLength, 2));
        OpenAndFilter(cut, "c").Should().HaveCount(4);
        cut.Find("input[role=searchbox]").Input("ca");
        cut.FindAll("[role=option]").Select(o => o.TextContent.Trim()).Should().Equal("Carrot");
    }

    [Fact]
    public void FilterField_Filters_On_Another_Property()
    {
        var cut = Render(p => p.Add(d => d.Filterable, true).Add(d => d.FilterField, nameof(Product.Category)));
        OpenAndFilter(cut, "veg").Should().Equal("Carrot");
    }

    [Fact]
    public void FilterCaseSensitive_Respects_Case()
    {
        var cut = Render(p => p.Add(d => d.Filterable, true).Add(d => d.FilterCaseSensitive, true));
        OpenAndFilter(cut, "c").Should().BeEmpty();
        cut.Find("input[role=searchbox]").Input("C");
        cut.FindAll("[role=option]").Should().HaveCount(2);
    }

    [Fact]
    public void FilterIgnoreDiacritics_Matches_Accented_Text()
    {
        var cut = RenderComponent<StackDropDownList<string, string>>(p => p
            .Add(d => d.Data, new[] { "Café", "Crème", "Tea" })
            .Add(d => d.Filterable, true)
            .Add(d => d.FilterOperator, StringFilterOperator.Contains)
            .Add(d => d.FilterIgnoreDiacritics, true));
        cut.Find("[role=combobox]").Click();
        cut.Find("input[role=searchbox]").Input("creme");
        cut.FindAll("[role=option]").Select(o => o.TextContent.Trim()).Should().Equal("Crème");
    }

    [Fact]
    public void CustomFilter_Replaces_Operator()
    {
        var cut = Render(p => p
            .Add(d => d.Filterable, true)
            .Add(d => d.CustomFilter, (item, text) => item.Id.ToString() == text));
        OpenAndFilter(cut, "4").Should().Equal("Cherry");
    }

    [Fact]
    public void FilterValue_TwoWay_Binding()
    {
        string? filter = null;
        var cut = Render(p => p
            .Add(d => d.Filterable, true)
            .Add(d => d.FilterValue, "b")
            .Add(d => d.PersistFilter, true)
            .Add(d => d.FilterValueChanged, v => filter = v));
        cut.Find("[role=combobox]").Click();
        cut.FindAll("[role=option]").Select(o => o.TextContent.Trim()).Should().Equal("Banana");

        cut.Find("input[role=searchbox]").Input("ch");
        filter.Should().Be("ch");
    }

    [Fact]
    public void Filter_Cleared_On_Close_Unless_Persisted()
    {
        var cut = Render(p => p.Add(d => d.Filterable, true));
        OpenAndFilter(cut, "ch");
        cut.Find("input[role=searchbox]").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        cut.Find("[role=combobox]").Click();
        cut.Find("input[role=searchbox]").GetAttribute("value").Should().BeEmpty();

        var persisted = Render(p => p.Add(d => d.Filterable, true).Add(d => d.PersistFilter, true));
        OpenAndFilter(persisted, "ch");
        persisted.Find("input[role=searchbox]").KeyDown(new KeyboardEventArgs { Key = "Escape" });
        persisted.Find("[role=combobox]").Click();
        persisted.FindAll("[role=option]").Select(o => o.TextContent.Trim()).Should().Equal("Cherry");
    }

    [Fact]
    public void Filter_Clear_Button_Resets_Filter()
    {
        var cut = Render(p => p.Add(d => d.Filterable, true));
        OpenAndFilter(cut, "ch").Should().HaveCount(1);
        cut.Find("button[aria-label='Clear filter']").Click();
        cut.FindAll("[role=option]").Should().HaveCount(4);
    }

    [Fact]
    public void FilterMaxLength_Sets_Attribute()
    {
        var cut = Render(p => p.Add(d => d.Filterable, true).Add(d => d.FilterMaxLength, 10));
        cut.Find("[role=combobox]").Click();
        cut.Find("input[role=searchbox]").GetAttribute("maxlength").Should().Be("10");
    }

    [Fact]
    public void HighlightFilterMatch_Bolds_Match()
    {
        var cut = Render(p => p
            .Add(d => d.Filterable, true)
            .Add(d => d.FilterOperator, StringFilterOperator.Contains)
            .Add(d => d.HighlightFilterMatch, true));
        OpenAndFilter(cut, "nan");
        cut.Find("[role=option] strong").TextContent.Should().Be("nan");
    }

    [Fact]
    public void OnRead_Receives_FilterField_And_CaseSensitive()
    {
        var requests = new List<DropDownListReadRequest>();
        var cut = RenderComponent<StackDropDownList<Product, int>>(p => p
            .Add(d => d.TextField, nameof(Product.Name))
            .Add(d => d.ValueField, nameof(Product.Id))
            .Add(d => d.Filterable, true)
            .Add(d => d.FilterDebounceDelay, 0)
            .Add(d => d.FilterField, nameof(Product.Category))
            .Add(d => d.FilterCaseSensitive, true)
            .Add(d => d.OnRead, (DropDownListReadEventArgs<Product> a) => { requests.Add(a.Request); a.Data = Products; }));

        cut.Find("[role=combobox]").Click();
        cut.Find("input[role=searchbox]").Input("Fr");
        cut.WaitForAssertion(() => requests.Should().HaveCount(2));
        requests[1].FilterText.Should().Be("Fr");
        requests[1].FilterField.Should().Be(nameof(Product.Category));
        requests[1].CaseSensitive.Should().BeTrue();
    }

    [Fact]
    public void NoDataTemplate_Shown_When_Filter_Matches_Nothing()
    {
        var cut = Render(p => p
            .Add(d => d.Filterable, true)
            .Add(d => d.NoDataTemplate, "<em>Nothing here</em>"));
        cut.Find("[role=combobox]").Click();
        cut.Find("input[role=searchbox]").Input("zzz");
        cut.Find("em").TextContent.Should().Be("Nothing here");
    }

    [Fact]
    public void GroupField_Renders_Group_Headers()
    {
        var cut = Render(p => p.Add(d => d.GroupField, nameof(Product.Category)));
        cut.Find("[role=combobox]").Click();
        var headers = cut.FindAll("[role=presentation]").Select(h => h.TextContent.Trim()).ToList();
        headers.Should().Equal("Fruit", "Vegetable");
        // Items are ordered by group.
        cut.FindAll("[role=option]").Last().TextContent.Trim().Should().Be("Carrot");
    }

    [Fact]
    public void ItemTemplate_And_ValueTemplate_Are_Used()
    {
        var cut = Render(p => p
            .Add(d => d.Value, 1)
            .Add(d => d.ValueTemplate, item => $"<b>V:{item.Name}</b>")
            .Add(d => d.ItemTemplate, item => $"<i>I:{item.Name}</i>"));

        cut.Find("[role=combobox] b").TextContent.Should().Be("V:Apple");
        cut.Find("[role=combobox]").Click();
        cut.FindAll("[role=option] i")[0].TextContent.Should().Be("I:Apple");
    }

    [Fact]
    public void Header_And_Footer_Templates_Render()
    {
        var cut = Render(p => p
            .Add(d => d.HeaderTemplate, "<span class='hdr'>Header</span>")
            .Add(d => d.FooterTemplate, "<span class='ftr'>Footer</span>"));
        cut.Find("[role=combobox]").Click();
        cut.Find(".hdr").Should().NotBeNull();
        cut.Find(".ftr").Should().NotBeNull();
    }

    [Fact]
    public void OnItemRender_Adds_Class()
    {
        var cut = Render(p => p.Add(d => d.OnItemRender,
            (DropDownListItemRenderEventArgs<Product> a) => { if (a.Item.Id == 2) a.Class = "special"; }));
        cut.Find("[role=combobox]").Click();
        cut.FindAll("[role=option].special").Should().HaveCount(1);
    }

    [Fact]
    public void ArrowDown_When_Closed_Selects_Next_Item()
    {
        int? value = null;
        var cut = Render(p => p
            .Add(d => d.Value, 1)
            .Add(d => d.ValueChanged, v => value = v));
        cut.Find("[role=combobox]").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        value.Should().Be(2);
    }

    [Fact]
    public void Keyboard_Open_Navigate_And_Select()
    {
        int? value = null;
        var cut = Render(p => p
            .Add(d => d.Value, 1)
            .Add(d => d.ValueChanged, v => value = v));
        var trigger = cut.Find("[role=combobox]");

        trigger.KeyDown(new KeyboardEventArgs { Key = "Enter" });
        cut.Find("[role=combobox]").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        cut.Find("[role=combobox]").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        cut.Find("[role=combobox]").KeyDown(new KeyboardEventArgs { Key = "Enter" });

        value.Should().Be(3);
        cut.FindAll("[role=listbox]").Should().BeEmpty();
    }

    [Fact]
    public void TypeAhead_Selects_Matching_Item()
    {
        int? value = null;
        var cut = Render(p => p.Add(d => d.ValueChanged, v => value = v));
        cut.Find("[role=combobox]").KeyDown(new KeyboardEventArgs { Key = "c" });
        value.Should().Be(3);
    }

    [Fact]
    public void Primitive_Data_Binds_Item_As_Value()
    {
        string? value = null;
        var cut = RenderComponent<StackDropDownList<string, string>>(p => p
            .Add(d => d.Data, new[] { "Red", "Green", "Blue" })
            .Add(d => d.ValueChanged, v => value = v));
        cut.Find("[role=combobox]").Click();
        cut.FindAll("[role=option]")[2].Click();
        value.Should().Be("Blue");
    }

    [Fact]
    public void OnRead_Supplies_Data_And_Filter_Text()
    {
        var requests = new List<DropDownListReadRequest>();
        var cut = RenderComponent<StackDropDownList<Product, int>>(p => p
            .Add(d => d.TextField, nameof(Product.Name))
            .Add(d => d.ValueField, nameof(Product.Id))
            .Add(d => d.Value, 4)
            .Add(d => d.OnRead, (DropDownListReadEventArgs<Product> a) =>
            {
                requests.Add(a.Request);
                a.Data = Products.Where(x => x.Name.StartsWith(a.Request.FilterText ?? "", StringComparison.OrdinalIgnoreCase));
                a.Total = a.Data.Count();
            }));

        requests.Should().HaveCount(1);
        cut.Find("[role=combobox]").TextContent.Should().Contain("Cherry");
    }

    [Fact]
    public void ShowClearButton_Resets_Value()
    {
        int? value = 2;
        var cut = Render(p => p
            .Add(d => d.Value, 2)
            .Add(d => d.ShowClearButton, true)
            .Add(d => d.ValueChanged, v => value = v));
        cut.Find("button[aria-label=Clear]").Click();
        value.Should().Be(0);
    }

    [Fact]
    public void Appearance_Parameters_Apply_Classes()
    {
        var cut = Render(p => p
            .Add(d => d.Size, DropDownListTheme.Size.Large)
            .Add(d => d.Rounded, DropDownListTheme.Rounded.Full)
            .Add(d => d.FillMode, DropDownListTheme.FillMode.Outline));
        var cls = cut.Find("[role=combobox]").ClassList;
        cls.Should().Contain(["h-12", "rounded-full", "border-2"]);
    }

    [Fact]
    public void PopupSettings_Child_Applies_Size()
    {
        var cut = Render(p => p.Add(d => d.DropDownListSettings, b =>
        {
            b.OpenComponent<StackDropDownListPopupSettings>(0);
            b.AddAttribute(1, nameof(StackDropDownListPopupSettings.Height), "321px");
            b.AddAttribute(2, nameof(StackDropDownListPopupSettings.Width), "400px");
            b.CloseComponent();
        }));
        cut.Find("[role=combobox]").Click();
        cut.Find("[role=listbox]").GetAttribute("style").Should().Contain("height:321px");
        cut.Find("[role=listbox]").ParentElement!.GetAttribute("style").Should().Contain("width:400px");
    }

    [Fact]
    public void Accessibility_Attributes_Are_Set()
    {
        var cut = Render(p => p
            .Add(d => d.Id, "ddl")
            .Add(d => d.AriaLabel, "Product")
            .Add(d => d.TabIndex, 5));
        var trigger = cut.Find("[role=combobox]");
        trigger.Id.Should().Be("ddl");
        trigger.GetAttribute("aria-label").Should().Be("Product");
        trigger.GetAttribute("tabindex").Should().Be("5");
    }

    [Fact]
    public void Title_Is_ActionSheet_Header_In_AdaptiveMode()
    {
        var cut = Render(p => p
            .Add(d => d.AdaptiveMode, AdaptiveMode.Auto)
            .Add(d => d.Title, "Choose a product"));
        cut.Find("[role=combobox]").HasAttribute("title").Should().BeFalse();
        cut.Find("[role=combobox]").Click();
        cut.Markup.Should().Contain("Choose a product");
    }

    [Fact]
    public void Title_Not_Rendered_Without_AdaptiveMode()
    {
        var cut = Render(p => p.Add(d => d.Title, "Choose a product"));
        cut.Find("[role=combobox]").Click();
        cut.Markup.Should().NotContain("Choose a product");
    }

    [Fact]
    public void InputMode_Set_On_Filter_Input()
    {
        var cut = Render(p => p.Add(d => d.Filterable, true).Add(d => d.InputMode, "numeric"));
        cut.Find("[role=combobox]").Click();
        cut.Find("input[role=searchbox]").GetAttribute("inputmode").Should().Be("numeric");
    }

    private enum Status { Draft, Active, Archived }
    private record StatusItem(Status Value, string Text);
    private record GuidItem(Guid Value, string Text);

    [Fact]
    public void Enum_Value_Field_Binds()
    {
        Status? value = null;
        var cut = RenderComponent<StackDropDownList<StatusItem, Status>>(p => p
            .Add(d => d.Data, new[] { new StatusItem(Status.Draft, "Draft"), new StatusItem(Status.Active, "Active"), new StatusItem(Status.Archived, "Archived") })
            .Add(d => d.Value, Status.Active)
            .Add(d => d.ValueChanged, v => value = v));
        cut.Find("[role=combobox]").TextContent.Should().Contain("Active");
        cut.Find("[role=combobox]").Click();
        cut.FindAll("[role=option]")[2].Click();
        value.Should().Be(Status.Archived);
    }

    [Fact]
    public void Guid_Value_Field_Binds()
    {
        var a = Guid.NewGuid();
        var b = Guid.NewGuid();
        Guid? value = null;
        var cut = RenderComponent<StackDropDownList<GuidItem, Guid>>(p => p
            .Add(d => d.Data, new[] { new GuidItem(a, "First"), new GuidItem(b, "Second") })
            .Add(d => d.Value, b)
            .Add(d => d.ValueChanged, v => value = v));
        cut.Find("[role=combobox]").TextContent.Should().Contain("Second");
        cut.Find("[role=combobox]").Click();
        cut.FindAll("[role=option]")[0].Click();
        value.Should().Be(a);
    }

    [Fact]
    public void Nullable_Value_With_DefaultText_Shows_Hint()
    {
        var cut = RenderComponent<StackDropDownList<Product, int?>>(p => p
            .Add(d => d.Data, Products)
            .Add(d => d.TextField, nameof(Product.Name))
            .Add(d => d.ValueField, nameof(Product.Id))
            .Add(d => d.DefaultText, "Select…")
            .Add(d => d.Value, null));
        cut.Find("[role=combobox]").TextContent.Should().Contain("Select…");
    }
}
