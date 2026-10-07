using MyStackBlazor.Components.Form;

namespace MyStackBlazor.UnitTests.Components;

public class AutoCompleteTests : TestContext
{
    public record Country(string Name, string Region);

    private static readonly List<Country> Countries =
    [
        new("Canada", "Americas"), new("Chile", "Americas"), new("China", "Asia"),
        new("France", "Europe"), new("Germany", "Europe"), new("Japan", "Asia"),
    ];

    public AutoCompleteTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private IRenderedComponent<StackAutoComplete<Country>> Render(
        Action<ComponentParameterCollectionBuilder<StackAutoComplete<Country>>>? configure = null) =>
        RenderComponent<StackAutoComplete<Country>>(p =>
        {
            p.Add(a => a.Data, Countries)
             .Add(a => a.ValueField, nameof(Country.Name))
             .Add(a => a.DebounceDelay, 0)
             .Add(a => a.Id, "ac");
            configure?.Invoke(p);
        });

    private static List<string> Options(IRenderedFragment cut) =>
        cut.FindAll("[role=option]").Select(o => o.TextContent.Trim()).ToList();

    [Fact]
    public void Filterable_StartsWith_Suggests_Matches()
    {
        var cut = Render(p => p.Add(a => a.Filterable, true));
        cut.Find("#ac").Input("ch");
        Options(cut).Should().Equal("Chile", "China");
        cut.Find("#ac").GetAttribute("aria-expanded").Should().Be("true");
    }

    [Fact]
    public void Contains_Operator()
    {
        var cut = Render(p => p.Add(a => a.Filterable, true).Add(a => a.FilterOperator, StringFilterOperator.Contains));
        cut.Find("#ac").Input("an");
        Options(cut).Should().Equal("Canada", "France", "Germany", "Japan");
    }

    [Fact]
    public void Not_Filterable_Suggests_All_Items()
    {
        var cut = Render();
        cut.Find("#ac").Input("zz");
        Options(cut).Should().HaveCount(6);
    }

    [Fact]
    public void Typing_Updates_Value()
    {
        string? value = null;
        var cut = Render(p => p.Add(a => a.ValueChanged, v => value = v));
        cut.Find("#ac").Input("Fra");
        value.Should().Be("Fra");
    }

    [Fact]
    public void Clicking_A_Suggestion_Sets_Value_And_Fires_OnChange()
    {
        string? value = null;
        object? changed = null;
        var cut = Render(p => p
            .Add(a => a.Filterable, true)
            .Add(a => a.ValueChanged, v => value = v)
            .Add(a => a.OnChange, v => changed = v));

        cut.Find("#ac").Input("ja");
        cut.Find("[role=option]").Click();

        value.Should().Be("Japan");
        changed.Should().Be("Japan");
        cut.FindAll("[role=listbox]").Should().BeEmpty();
        cut.Find("#ac").GetAttribute("value").Should().Be("Japan");
    }

    [Fact]
    public void Keyboard_ArrowDown_And_Enter_Select()
    {
        string? value = null;
        var cut = Render(p => p.Add(a => a.Filterable, true).Add(a => a.ValueChanged, v => value = v));
        cut.Find("#ac").Input("c");
        cut.Find("#ac").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        cut.Find("#ac").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        cut.Find("#ac").GetAttribute("aria-activedescendant").Should().Be("ac-listbox-1");
        cut.Find("#ac").KeyDown(new KeyboardEventArgs { Key = "Enter" });
        value.Should().Be("Chile");
    }

    [Fact]
    public void Enter_Without_Suggestion_Commits_Typed_Text()
    {
        object? changed = null;
        var cut = Render(p => p.Add(a => a.Filterable, true).Add(a => a.OnChange, v => changed = v));
        cut.Find("#ac").Input("Atlantis");
        cut.Find("#ac").KeyDown(new KeyboardEventArgs { Key = "Enter" });
        changed.Should().Be("Atlantis");
    }

    [Fact]
    public void MinLength_Delays_Suggestions()
    {
        var cut = Render(p => p.Add(a => a.Filterable, true).Add(a => a.MinLength, 3));
        cut.Find("#ac").Input("ch");
        cut.FindAll("[role=listbox]").Should().BeEmpty();
        cut.Find("#ac").Input("chi");
        Options(cut).Should().Equal("Chile", "China");
    }

    [Fact]
    public void NoDataTemplate_Shown_When_Nothing_Matches()
    {
        var cut = Render(p => p.Add(a => a.Filterable, true).Add(a => a.NoDataTemplate, "<em>Nope</em>"));
        cut.Find("#ac").Input("xyz");
        cut.Find("em").TextContent.Should().Be("Nope");
    }

    [Fact]
    public void GroupField_Renders_Headers()
    {
        var cut = Render(p => p.Add(a => a.GroupField, nameof(Country.Region)));
        cut.Find("#ac").Input("a");
        cut.FindAll("[role=presentation]").Select(h => h.TextContent.Trim()).Should().Equal("Americas", "Asia", "Europe");
    }

    [Fact]
    public void Templates_And_ItemRender()
    {
        var cut = Render(p => p
            .Add(a => a.ItemTemplate, c => $"<b>{c.Name} ({c.Region})</b>")
            .Add(a => a.HeaderTemplate, "<span class='hdr'>Countries</span>")
            .Add(a => a.FooterTemplate, "<span class='ftr'>6 total</span>")
            .Add(a => a.OnItemRender, (AutoCompleteItemRenderEventArgs<Country> e) => { if (e.Item.Region == "Asia") e.Class = "asia"; }));
        cut.Find("#ac").Input("x");
        cut.Find("[role=option] b").TextContent.Should().Be("Canada (Americas)");
        cut.Find(".hdr").Should().NotBeNull();
        cut.Find(".ftr").Should().NotBeNull();
        cut.FindAll("[role=option].asia").Should().HaveCount(2);
    }

    [Fact]
    public void OnRead_Supplies_Suggestions()
    {
        string? requested = null;
        var cut = Render(p => p.Add(a => a.OnRead, (AutoCompleteReadEventArgs<Country> e) =>
        {
            requested = e.Request.FilterText;
            e.Data = Countries.Where(c => c.Name.Contains(e.Request.FilterText!, StringComparison.OrdinalIgnoreCase));
        }));
        cut.Find("#ac").Input("many");
        requested.Should().Be("many");
        Options(cut).Should().Equal("Germany");
    }

    [Fact]
    public void Open_And_Close_Events_Can_Cancel()
    {
        var cut = Render(p => p.Add(a => a.OnOpen, (AutoCompleteOpenEventArgs e) => e.IsCancelled = true));
        cut.Find("#ac").Input("c");
        cut.FindAll("[role=listbox]").Should().BeEmpty();
    }

    [Fact]
    public void Clear_Button_Resets_Value()
    {
        string? value = "Japan";
        var cut = Render(p => p.Add(a => a.Value, "Japan").Add(a => a.ValueChanged, v => value = v));
        cut.Find("button[aria-label=Clear]").Click();
        value.Should().BeNull();
        cut.Find("#ac").GetAttribute("value").Should().BeEmpty();
    }

    [Fact]
    public void Disabled_ReadOnly_And_Appearance()
    {
        Render(p => p.Add(a => a.Enabled, false)).Find("#ac").HasAttribute("disabled").Should().BeTrue();
        Render(p => p.Add(a => a.ReadOnly, true)).Find("#ac").HasAttribute("readonly").Should().BeTrue();

        var styled = Render(p => p.Add(a => a.Size, "lg").Add(a => a.Rounded, "full").Add(a => a.InputMode, "search"));
        styled.Find("#ac").ParentElement!.ClassList.Should().Contain(["h-12", "rounded-full"]);
        styled.Find("#ac").GetAttribute("inputmode").Should().Be("search");
    }

    [Fact]
    public void PopupSettings_Child_Sets_Size()
    {
        var cut = Render(p => p.Add(a => a.AutoCompleteSettings, b =>
        {
            b.OpenComponent<StackAutoCompletePopupSettings>(0);
            b.AddAttribute(1, nameof(StackAutoCompletePopupSettings.Height), "123px");
            b.CloseComponent();
        }));
        cut.Find("#ac").Input("a");
        cut.Find("[role=listbox]").GetAttribute("style").Should().Contain("height:123px");
    }

    [Fact]
    public void Earlier_Api_Still_Works()
    {
        Country? selected = null;
        var cut = RenderComponent<StackAutoComplete<Country>>(p => p
            .Add(a => a.Search, text => Countries.Where(c => c.Name.StartsWith(text, StringComparison.OrdinalIgnoreCase)))
            .Add(a => a.ItemText, c => c.Name.ToUpperInvariant())
            .Add(a => a.DebounceMs, 0)
            .Add(a => a.MinChars, 2)
            .Add(a => a.NoResultsText, "Nothing")
            .Add(a => a.SelectedItemChanged, c => selected = c));

        cut.Find("input").Input("g");
        cut.FindAll("[role=listbox]").Should().BeEmpty("MinChars is 2");

        cut.Find("input").Input("ge");
        Options(cut).Should().Equal("GERMANY");
        cut.Find("[role=option]").Click();
        selected!.Name.Should().Be("Germany");

        cut.Find("input").Input("zz");
        cut.Find("[role=listbox]").TextContent.Should().Contain("Nothing");
    }
}
