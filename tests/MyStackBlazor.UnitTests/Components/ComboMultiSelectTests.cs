using MyStackBlazor.Components.Form;

namespace MyStackBlazor.UnitTests.Components;

public class ComboBoxTests : TestContext
{
    public record City(int Id, string Name, string Country);

    private static readonly List<City> Cities =
    [
        new(1, "Berlin", "DE"), new(2, "Bern", "CH"), new(3, "Boston", "US"),
        new(4, "Paris", "FR"), new(5, "Porto", "PT"),
    ];

    public ComboBoxTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<StackCombobox<City, int>> Render(
        Action<ComponentParameterCollectionBuilder<StackCombobox<City, int>>>? configure = null) =>
        RenderComponent<StackCombobox<City, int>>(p =>
        {
            p.Add(c => c.Data, Cities).Add(c => c.TextField, "Name").Add(c => c.ValueField, "Id")
             .Add(c => c.DebounceDelay, 0).Add(c => c.Id, "cb");
            configure?.Invoke(p);
        });

    private static List<string> Options(IRenderedFragment cut) =>
        cut.FindAll("[role=option]").Select(o => o.TextContent.Trim()).ToList();

    [Fact]
    public void Shows_Text_Of_Value()
    {
        var cut = Render(p => p.Add(c => c.Value, 4));
        cut.Find("#cb").GetAttribute("value").Should().Be("Paris");
    }

    [Fact]
    public void Filterable_Typing_Narrows_And_Enter_Selects_First()
    {
        int value = 0;
        var cut = Render(p => p.Add(c => c.Filterable, true).Add(c => c.ValueChanged, v => value = v));
        cut.Find("#cb").Input("ber");
        Options(cut).Should().Equal("Berlin", "Bern");
        cut.Find("#cb").KeyDown(new KeyboardEventArgs { Key = "Enter" });
        value.Should().Be(1);
        cut.Find("#cb").GetAttribute("value").Should().Be("Berlin");
    }

    [Fact]
    public void Not_Filterable_Highlights_First_Match()
    {
        var cut = Render();
        cut.Find("#cb").Input("po");
        Options(cut).Should().HaveCount(5);
        cut.Find("#cb").GetAttribute("aria-activedescendant").Should().Be("cb-listbox-4", "Porto");
    }

    [Fact]
    public void Exact_Text_Match_On_Blur_Selects()
    {
        int value = 0;
        var cut = Render(p => p.Add(c => c.ValueChanged, v => value = v));
        cut.Find("#cb").Input("boston");
        cut.Find("#cb").Blur();
        value.Should().Be(3);
    }

    [Fact]
    public void Unmatched_Text_Reverts_Without_AllowCustom()
    {
        var cut = Render(p => p.Add(c => c.Value, 2));
        cut.Find("#cb").Input("Atlantis");
        cut.Find("#cb").Blur();
        cut.Find("#cb").GetAttribute("value").Should().Be("Bern");
    }

    [Fact]
    public void AllowCustom_Uses_Typed_String()
    {
        string? value = null;
        var cut = RenderComponent<StackCombobox<string, string>>(p => p
            .Add(c => c.Data, new[] { "Red", "Green" })
            .Add(c => c.AllowCustom, true)
            .Add(c => c.DebounceDelay, 0)
            .Add(c => c.ValueChanged, v => value = v));
        cut.Find("input").Input("Purple");
        cut.Find("input").KeyDown(new KeyboardEventArgs { Key = "Enter" });
        value.Should().Be("Purple");
    }

    [Fact]
    public void Clicking_An_Option_And_OnChange()
    {
        object? changed = null;
        var cut = Render(p => p.Add(c => c.OnChange, v => changed = v));
        cut.Find("button[aria-label='Toggle list']").Click();
        cut.FindAll("[role=option]")[3].Click();
        changed.Should().Be(4);
        cut.FindAll("[role=listbox]").Should().BeEmpty();
    }

    [Fact]
    public void Grouping_And_Templates()
    {
        var cut = Render(p => p
            .Add(c => c.GroupField, "Country")
            .Add(c => c.ItemTemplate, c => $"<b>{c.Name}</b>")
            .Add(c => c.FooterTemplate, "<i class='ftr'>5 cities</i>"));
        cut.Find("button[aria-label='Toggle list']").Click();
        cut.FindAll("[role=presentation]").Select(h => h.TextContent.Trim()).Should().Equal("CH", "DE", "FR", "PT", "US");
        cut.Find("[role=option] b").Should().NotBeNull();
        cut.Find(".ftr").Should().NotBeNull();
    }

    [Fact]
    public void Earlier_Items_Api()
    {
        string? value = null;
        var cut = RenderComponent<StackCombobox<ComboboxItem, string>>(p => p
            .Add(c => c.Items, new List<ComboboxItem> { new("ny", "New York"), new("la", "Los Angeles") })
            .Add(c => c.Value, "la")
            .Add(c => c.ValueChanged, v => value = v));
        cut.Find("input").GetAttribute("value").Should().Be("Los Angeles");
        cut.Find("button[aria-label='Toggle list']").Click();
        cut.FindAll("[role=option]")[0].Click();
        value.Should().Be("ny");
    }
}

public class MultiSelectTests : TestContext
{
    public record Skill(int Id, string Name, string Area);

    private static readonly List<Skill> Skills =
    [
        new(1, "C#", "Backend"), new(2, "Go", "Backend"), new(3, "CSS", "Frontend"), new(4, "React", "Frontend"),
    ];

    public MultiSelectTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<StackMultiSelect<Skill, int>> Render(
        Action<ComponentParameterCollectionBuilder<StackMultiSelect<Skill, int>>>? configure = null) =>
        RenderComponent<StackMultiSelect<Skill, int>>(p =>
        {
            p.Add(m => m.Data, Skills).Add(m => m.TextField, "Name").Add(m => m.ValueField, "Id")
             .Add(m => m.DebounceDelay, 0).Add(m => m.Id, "ms");
            configure?.Invoke(p);
        });

    [Fact]
    public void Tags_Show_Selected_Values()
    {
        var cut = Render(p => p.Add(m => m.Value, [1, 4]));
        cut.FindAll("[data-tag]").Select(t => t.GetAttribute("data-tag")).Should().Equal("C#", "React");
    }

    [Fact]
    public void Selecting_Adds_Value_And_AutoClose_Closes()
    {
        List<int>? value = null;
        var cut = Render(p => p.Add(m => m.ValueChanged, v => value = v));
        cut.Find("#ms").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        cut.FindAll("[role=option]")[2].Click();
        value.Should().Equal(3);
        cut.FindAll("[role=listbox]").Should().BeEmpty("AutoClose defaults to true");
    }

    [Fact]
    public void AutoClose_False_Keeps_Popup_Open_And_Toggles()
    {
        List<int>? value = null;
        var cut = Render(p => p.Add(m => m.AutoClose, false).Add(m => m.ValueChanged, v => value = v));
        cut.Find("#ms").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        cut.FindAll("[role=option]")[0].Click();
        cut.FindAll("[role=option]")[1].Click();
        cut.FindAll("[role=option]")[0].Click();
        value.Should().Equal(2);
    }

    [Fact]
    public void Filterable_Typing_Filters()
    {
        var cut = Render(p => p.Add(m => m.Filterable, true));
        cut.Find("#ms").Input("c");
        cut.FindAll("[role=option]").Select(o => o.TextContent.Trim()).Should().Equal("C#", "CSS");
    }

    [Fact]
    public void Backspace_And_Tag_Remove_And_Clear()
    {
        List<int>? value = null;
        var cut = Render(p => p.Add(m => m.Value, [1, 2, 3]).Add(m => m.ValueChanged, v => value = v));
        cut.Find("#ms").KeyDown(new KeyboardEventArgs { Key = "Backspace" });
        value.Should().Equal(1, 2);
        cut.Find("button[aria-label='Remove C#']").Click();
        value.Should().Equal(2);
        cut.Find("button[aria-label='Clear all']").Click();
        value.Should().BeEmpty();
    }

    [Fact]
    public void Single_TagMode_Shows_Summary()
    {
        var cut = Render(p => p.Add(m => m.Value, [1, 2, 3]).Add(m => m.TagMode, MultiSelectTagMode.Single));
        cut.Markup.Should().Contain("3 items selected");
        cut.FindAll("[data-tag]").Should().BeEmpty();
    }

    [Fact]
    public void MaxSelection_Disables_Remaining_Items()
    {
        var cut = Render(p => p.Add(m => m.Value, [1]).Add(m => m.MaxSelection, 1));
        cut.Find("#ms").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        cut.FindAll("[role=option]")[1].GetAttribute("aria-disabled").Should().Be("true");
        cut.FindAll("[role=option]")[0].HasAttribute("aria-disabled").Should().BeFalse("selected items can be removed");
    }

    [Fact]
    public void Select_All_And_Group_Select_All()
    {
        List<int>? value = null;
        var cut = Render(p => p.Add(m => m.ValueChanged, v => value = v).Add(m => m.AutoClose, false));
        cut.Find("#ms").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        cut.FindAll("button").First(b => b.TextContent.Contains("Select All")).Click();
        value.Should().Equal(1, 2, 3, 4);

        var grouped = Render(p => p.Add(m => m.GroupField, "Area").Add(m => m.ValueChanged, v => value = v).Add(m => m.AutoClose, false));
        grouped.Find("#ms").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        grouped.Find("button[aria-label='Select all Frontend']").Click();
        value.Should().Equal(3, 4);
    }

    [Fact]
    public void Earlier_Api_With_Items_As_Values()
    {
        IList<string>? selected = null;
        var cut = RenderComponent<StackMultiSelect<string, string>>(p => p
            .Add(m => m.Options, new[] { "India", "Germany", "Japan" })
            .Add(m => m.SelectedItems, new List<string> { "India" })
            .Add(m => m.ItemLabel, x => x.ToUpperInvariant())
            .Add(m => m.SelectedItemsChanged, v => selected = v));

        cut.FindAll("[data-tag]").Select(t => t.GetAttribute("data-tag")).Should().Equal("INDIA");
        cut.Find("input").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        cut.FindAll("[role=option]")[2].Click();
        selected.Should().Equal("India", "Japan");
        cut.FindAll("[role=listbox]").Should().NotBeEmpty("the earlier API keeps the popup open");
        cut.Find("input").HasAttribute("readonly").Should().BeFalse("the earlier API always filters");
    }
}
