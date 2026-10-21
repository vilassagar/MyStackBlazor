using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Routing;
using MyStackBlazor.Components.Common;
using MyStackBlazor.Components.Data;
using MyStackBlazor.Components.Form;
using MyStackBlazor.Components.Layout;
using MyStackBlazor.Components.Navigation;
using MyStackBlazor.Components.Overlays;
using MyStackBlazor.DataGrid;
using MyStackBlazor.DataGrid.Models;
using MyStackBlazor.DataGrid.Services;
using FilterOperator = MyStackBlazor.DataGrid.Models.FilterOperator;
using SortDirection = MyStackBlazor.DataGrid.Models.SortDirection;

namespace MyStackBlazor.UnitTests.Components;

// ═══════════════════════════════════════════════════════════════════════════════
// InMemoryGridDataProvider — typed sort / filter
// ═══════════════════════════════════════════════════════════════════════════════

public class InMemoryGridDataProviderTests
{
    public sealed record Row(string Name, int Qty, decimal Price, DateTime Date, Status Status, Inner? Inner = null);
    public sealed record Inner(string City);
    public enum Status { Open, Closed }

    private static readonly Row[] Rows =
    [
        new("b", 10, 9.5m,  new DateTime(2025, 2, 1),   Status.Open,   new("Pune")),
        new("a", 9,  100m,  new DateTime(2024, 12, 31), Status.Closed, new("Mumbai")),
        new("c", 100, 20m,  new DateTime(2025, 1, 15),  Status.Open,   null),
    ];

    private static async Task<List<Row>> Query(InMemoryGridDataProvider<Row> p, IReadOnlyList<SortDescriptor>? sort = null,
        IReadOnlyList<FilterDescriptor>? filters = null, string? search = null, int start = 0, int count = 100)
    {
        var r = await p.GetDataAsync(new GridRequest(new PageDescriptor(start, count), sort ?? [], filters ?? [], search));
        return r.Items.ToList();
    }

    [Fact]
    public async Task Sorts_Numbers_Numerically_Not_As_Text()
    {
        var rows = await Query(new(Rows), [new SortDescriptor("Qty", SortDirection.Ascending)]);
        rows.Select(r => r.Qty).Should().Equal(9, 10, 100);
    }

    [Fact]
    public async Task Sorts_Dates_Chronologically_Descending()
    {
        var rows = await Query(new(Rows), [new SortDescriptor("Date", SortDirection.Descending)]);
        rows.Select(r => r.Name).Should().Equal("b", "c", "a");
    }

    [Fact]
    public async Task MultiSort_Uses_ThenBy()
    {
        var rows = await Query(new(Rows),
            [new SortDescriptor("Status", SortDirection.Ascending), new SortDescriptor("Price", SortDirection.Descending)]);
        rows.Select(r => r.Name).Should().Equal("c", "b", "a");
    }

    [Fact]
    public async Task Filter_GreaterThan_Parses_Typed_Value()
    {
        var rows = await Query(new(Rows), filters: [new FilterDescriptor("Qty", FilterOperator.GreaterThan, "9")]);
        rows.Select(r => r.Name).Should().BeEquivalentTo("b", "c");
    }

    [Fact]
    public async Task Filter_Date_LessThan_Uses_Invariant_Iso_Input()
    {
        using var _ = new CultureScope("en-US");
        var rows = await Query(new(Rows), filters: [new FilterDescriptor("Date", FilterOperator.LessThan, "2025-01-01")]);
        rows.Select(r => r.Name).Should().Equal("a");
    }

    [Fact]
    public async Task Filter_Enum_Equals_By_Name()
    {
        var rows = await Query(new(Rows), filters: [new FilterDescriptor("Status", FilterOperator.Equals, "closed")]);
        rows.Select(r => r.Name).Should().Equal("a");
    }

    [Fact]
    public async Task Nested_Property_Path_And_Null_Filter()
    {
        var p = new InMemoryGridDataProvider<Row>(Rows);
        (await Query(p, filters: [new FilterDescriptor("Inner.City", FilterOperator.Contains, "pu")])).Select(r => r.Name).Should().Equal("b");
        (await Query(p, filters: [new FilterDescriptor("Inner.City", FilterOperator.IsNull, null)])).Select(r => r.Name).Should().Equal("c");
    }

    [Fact]
    public async Task Search_Matches_Any_Field_And_Pages()
    {
        var p = new InMemoryGridDataProvider<Row>(Rows);
        var r = await p.GetDataAsync(new GridRequest(new PageDescriptor(1, 1), [new SortDescriptor("Name", SortDirection.Ascending)], [], null));
        r.TotalCount.Should().Be(3);
        r.Items.Single().Name.Should().Be("b");

        (await Query(p, search: "Mumbai")).Select(x => x.Name).Should().Equal("a");
    }

    [Fact]
    public async Task Custom_Selector_Overrides_Property()
    {
        var p = new InMemoryGridDataProvider<Row>(Rows, new Dictionary<string, Func<Row, object?>> { ["Total"] = r => r.Qty * r.Price });
        var rows = await Query(p, [new SortDescriptor("Total", SortDirection.Descending)]);
        rows.Select(r => r.Name).Should().Equal("c", "a", "b"); // 2000, 900, 95
    }

    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo _old = CultureInfo.CurrentCulture;
        public CultureScope(string name) => CultureInfo.CurrentCulture = new CultureInfo(name);
        public void Dispose() => CultureInfo.CurrentCulture = _old;
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// MsDataGrid — Items, rows-per-page selector, saved views
// ═══════════════════════════════════════════════════════════════════════════════

public class MsDataGridGapTests : TestContext
{
    public sealed record Item(string Name, int Qty);

    public MsDataGridGapTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    private static readonly List<Item> Data = Enumerable.Range(1, 30).Select(i => new Item($"Item {i}", i)).ToList();

    private static RenderFragment Columns => b =>
    {
        b.OpenComponent<MsDataGridColumn<Item>>(0);
        b.AddAttribute(1, "Field", "Name");
        b.AddAttribute(2, "Title", "Name");
        b.CloseComponent();
        b.OpenComponent<MsDataGridColumn<Item>>(3);
        b.AddAttribute(4, "Field", "Qty");
        b.AddAttribute(5, "Title", "Qty");
        b.AddAttribute(6, "Filterable", true);
        b.CloseComponent();
    };

    private IRenderedComponent<MsDataGrid<Item>> RenderGrid(Action<ComponentParameterCollectionBuilder<MsDataGrid<Item>>>? extra = null) =>
        RenderComponent<MsDataGrid<Item>>(p =>
        {
            p.Add(x => x.Items, Data)
             .Add(x => x.Pageable, true)
             .Add(x => x.PageSize, 10)
             .Add(x => x.ChildContent, Columns);
            extra?.Invoke(p);
        });

    [Fact]
    public void Items_Without_DataProvider_Loads_First_Page()
    {
        var cut = RenderGrid();
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Page 1 of 3"));
        cut.Markup.Should().Contain("Item 10").And.NotContain("Item 11<");
    }

    [Fact]
    public void Sorting_Qty_Column_Is_Numeric()
    {
        var cut = RenderGrid();
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Page 1 of 3"));
        var qtyHeader = cut.FindAll("[role=columnheader]").First(h => h.TextContent.Contains("Qty"));
        qtyHeader.Click();
        qtyHeader = cut.FindAll("[role=columnheader]").First(h => h.TextContent.Contains("Qty"));
        qtyHeader.Click(); // descending
        // Numeric descending puts 30..21 on page 1; a text sort would put "Item 9" first.
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Item 30").And.Contain("Item 21").And.NotContain("Item 9<"));
    }

    [Fact]
    public void PageSizes_Renders_Selector_And_Changes_Page_Count()
    {
        int? raised = null;
        var cut = RenderGrid(p => p
            .Add(x => x.PageSizes, new[] { 10, 25 })
            .Add(x => x.PageSizeChanged, s => raised = s));
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Rows per page"));

        cut.Find("select").Change("25");

        raised.Should().Be(25);
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Page 1 of 2"));
    }

    [Fact]
    public void First_And_Last_Page_Buttons_Are_Labelled()
    {
        var cut = RenderGrid();
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Page 1 of 3"));
        cut.Find("button[aria-label='Last page']").Click();
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Page 3 of 3"));
        cut.Find("button[aria-label='First page']").Click();
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Page 1 of 3"));
    }

    [Fact]
    public async Task CaptureView_And_ApplyView_Roundtrip()
    {
        var cut = RenderGrid();
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Page 1 of 3"));

        var view = new GridView
        {
            Name = "Big",
            Sort = [new SortDescriptor("Qty", SortDirection.Descending)],
            MenuFilters = new Dictionary<string, GridViewFilter> { ["Qty"] = new("gt", "25") },
            HiddenColumns = ["Name"],
        };
        await cut.InvokeAsync(() => cut.Instance.ApplyViewAsync(view));

        cut.WaitForAssertion(() => cut.Markup.Should().Contain("5 rows"));
        var captured = cut.Instance.CaptureView("Again");
        captured.Sort.Should().ContainSingle(s => s.Field == "Qty" && s.Direction == SortDirection.Descending);
        captured.MenuFilters["Qty"].Should().Be(new GridViewFilter("gt", "25"));
        captured.HiddenColumns.Should().Equal("Name");
        captured.PageSize.Should().Be(10);
    }

    [Fact]
    public void Views_Menu_Saves_Current_View()
    {
        IReadOnlyList<GridView>? saved = null;
        var cut = RenderGrid(p => p
            .Add(x => x.Views, Array.Empty<GridView>())
            .Add(x => x.ViewsChanged, v => saved = v));
        cut.WaitForAssertion(() => cut.Markup.Should().Contain("Page 1 of 3"));

        cut.Find("button[aria-label='Saved views']").Click();
        cut.Find("input[placeholder='View name…']").Input("Mine");
        cut.FindAll("button").First(b => b.TextContent == "Save view").Click();

        saved.Should().NotBeNull();
        saved!.Single().Name.Should().Be("Mine");
    }

    [Fact]
    public void GridView_Roundtrips_Through_SystemTextJson()
    {
        var v = new GridView
        {
            Name = "x",
            Sort = [new SortDescriptor("Qty", SortDirection.Descending)],
            FilterRow = new Dictionary<string, string> { ["Name"] = "it" },
            MenuFilters = new Dictionary<string, GridViewFilter> { ["Qty"] = new("gte", "3") },
            PageSize = 50,
        };
        var json = System.Text.Json.JsonSerializer.Serialize(v);
        var back = System.Text.Json.JsonSerializer.Deserialize<GridView>(json)!;
        back.Sort.Single().Should().Be(v.Sort[0]);
        back.MenuFilters["Qty"].Should().Be(new GridViewFilter("gte", "3"));
        back.FilterRow["Name"].Should().Be("it");
        back.PageSize.Should().Be(50);
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// StackDataTable — typed sort
// ═══════════════════════════════════════════════════════════════════════════════

public class TypedValueComparerTests
{
    [Fact]
    public void Numbers_And_Dates_Compare_By_Value()
    {
        var c = TypedValueComparer.Instance;
        c.Compare(9, 10).Should().BeNegative();
        c.Compare(10, 9.5m).Should().BePositive(); // mixed numeric types
        c.Compare(new DateTime(2024, 12, 31), new DateTime(2025, 1, 1)).Should().BeNegative();
        c.Compare(null, 1).Should().BeNegative();
        c.Compare("apple", "Banana").Should().BeNegative();
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Lucide icons
// ═══════════════════════════════════════════════════════════════════════════════

public class LucideIconTests : TestContext
{
    [Theory]
    [InlineData("house", "house")]
    [InlineData("home", "house")]                 // legacy alias
    [InlineData("alert-triangle", "triangle-alert")]
    [InlineData("ChevronDown", "chevron-down")]   // PascalCase
    [InlineData("Loader2", "loader-circle")]      // digit + alias
    [InlineData("grid-2x2", "grid-2x2")]
    [InlineData("lucide-search", "search")]
    public void Resolve_Normalizes_Names(string input, string expected) =>
        LucideIcons.Resolve(input).Should().Be(expected);

    [Fact]
    public void Resolve_Unknown_Returns_Null() => LucideIcons.Resolve("definitely-not-an-icon").Should().BeNull();

    [Fact]
    public void Sprite_Contains_Every_Listed_Name()
    {
        var sprite = File.ReadAllText(Path.Combine(FindRepoRoot(), "src", "MyStackBlazor", "wwwroot", "icons", "lucide-sprite.svg"));
        LucideIcons.Names.Count.Should().BeGreaterThan(1500);
        foreach (var n in LucideIcons.Names.Take(50).Concat(["house", "triangle-alert", "loader-circle"]))
            sprite.Should().Contain($"<symbol id=\"{n}\"");
    }

    [Fact]
    public void Renders_Use_Reference_To_Sprite()
    {
        var cut = RenderComponent<StackLucideIcon>(p => p.Add(x => x.Name, "home").Add(x => x.PixelSize, 18));
        cut.Find("use").GetAttribute("href").Should().Be("_content/MyStackBlazor/icons/lucide-sprite.svg#house");
        cut.Find("svg").GetAttribute("width").Should().Be("18");
        cut.Find("svg").GetAttribute("aria-hidden").Should().Be("true");
    }

    [Fact]
    public void Unknown_Icon_Renders_Empty_And_Flagged()
    {
        var cut = RenderComponent<StackLucideIcon>(p => p.Add(x => x.Name, "nope-nope"));
        cut.FindAll("use").Should().BeEmpty();
        cut.Find("svg").GetAttribute("data-missing").Should().Be("true");
    }

    private static string FindRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "MyStackBlazor.slnx"))) dir = dir.Parent;
        return dir?.FullName ?? throw new InvalidOperationException("repo root not found");
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Nav links follow the current URL
// ═══════════════════════════════════════════════════════════════════════════════

public class NavLinkGapTests : TestContext
{
    [Theory]
    [InlineData("http://x/orders", "http://x/orders", NavLinkMatch.All, true)]
    [InlineData("http://x/orders/5", "http://x/orders", NavLinkMatch.Prefix, true)]
    [InlineData("http://x/orders/5", "http://x/orders", NavLinkMatch.All, false)]
    [InlineData("http://x/orders-archive", "http://x/orders", NavLinkMatch.Prefix, false)]
    [InlineData("http://x/orders/", "http://x/orders", NavLinkMatch.All, true)]
    public void Matcher_Follows_NavLink_Rules(string current, string target, NavLinkMatch match, bool expected) =>
        NavMatcher.IsMatch(current, target, match).Should().Be(expected);

    [Fact]
    public void StackNavLink_Becomes_Active_On_Navigation()
    {
        var nav = Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
        var cut = RenderComponent<StackNavLink>(p => p
            .Add(x => x.Href, "orders")
            .Add(x => x.ChildContent, "Orders"));

        cut.Find("a").HasAttribute("aria-current").Should().BeFalse();
        nav.NavigateTo("orders/42?tab=items");
        cut.WaitForAssertion(() => cut.Find("a").GetAttribute("aria-current").Should().Be("page"));
        nav.NavigateTo("customers");
        cut.WaitForAssertion(() => cut.Find("a").HasAttribute("aria-current").Should().BeFalse());
    }

    [Fact]
    public void StackNavLink_Collapsed_Hides_Text_But_Keeps_Label()
    {
        var cut = RenderComponent<StackNavLink>(p => p
            .Add(x => x.Href, "a")
            .Add(x => x.Collapsed, true)
            .Add(x => x.TitleText, "Dashboard"));
        cut.Find("a").GetAttribute("title").Should().Be("Dashboard");
        cut.Find(".sr-only").TextContent.Should().Be("Dashboard");
    }

    [Fact]
    public void NavigationMenuLink_Match_Overrides_Manual_Active()
    {
        var nav = Services.GetRequiredService<Microsoft.AspNetCore.Components.NavigationManager>();
        nav.NavigateTo("reports");
        var cut = RenderComponent<StackNavigationMenuLink>(p => p
            .Add(x => x.Href, "reports")
            .Add(x => x.Match, NavLinkMatch.All));
        cut.Find("a").GetAttribute("aria-current").Should().Be("page");

        var manual = RenderComponent<StackNavigationMenuLink>(p => p.Add(x => x.Href, "reports"));
        manual.Find("a").HasAttribute("aria-current").Should().BeFalse(); // unchanged default behaviour
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Period picker
// ═══════════════════════════════════════════════════════════════════════════════

public class PeriodPickerTests : TestContext
{
    private static readonly DateTime Wed = new(2026, 10, 21);

    [Fact]
    public void Week_Starts_On_Requested_Day()
    {
        var mon = DatePeriod.Create(PeriodType.Week, Wed, DayOfWeek.Monday);
        mon.Start.Should().Be(new DateTime(2026, 10, 19));
        mon.End.Should().Be(new DateTime(2026, 10, 25));

        var sun = DatePeriod.Create(PeriodType.Week, Wed, DayOfWeek.Sunday);
        sun.Start.Should().Be(new DateTime(2026, 10, 18));
    }

    [Fact]
    public void Quarter_And_Fiscal_Year()
    {
        var q = DatePeriod.Create(PeriodType.Quarter, Wed);
        (q.Start, q.End).Should().Be((new DateTime(2026, 10, 1), new DateTime(2026, 12, 31)));
        q.ToDisplayString(CultureInfo.InvariantCulture).Should().Be("Q4 2026");

        // April–March fiscal year: October is Q3 of FY2027.
        var fq = DatePeriod.Create(PeriodType.Quarter, Wed, fiscalYearStartMonth: 4);
        fq.ToDisplayString(CultureInfo.InvariantCulture, 4).Should().Be("Q3 FY2027");
        var fy = DatePeriod.Create(PeriodType.Year, Wed, fiscalYearStartMonth: 4);
        (fy.Start, fy.End).Should().Be((new DateTime(2026, 4, 1), new DateTime(2027, 3, 31)));
    }

    [Fact]
    public void Shift_Month_Handles_Month_Lengths()
    {
        var jan = DatePeriod.Create(PeriodType.Month, new DateTime(2026, 1, 31));
        var feb = jan.Shift(1);
        (feb.Start, feb.End).Should().Be((new DateTime(2026, 2, 1), new DateTime(2026, 2, 28)));
        feb.Shift(-2).Start.Should().Be(new DateTime(2025, 12, 1));
    }

    [Fact]
    public void Default_Presets_Resolve()
    {
        var presets = PeriodPreset.Defaults();
        presets.Single(p => p.Label == "Last month").Resolve(Wed).Start.Should().Be(new DateTime(2026, 9, 1));
        var ytd = presets.Single(p => p.Label == "Year to date").Resolve(Wed);
        (ytd.Start, ytd.End, ytd.Type).Should().Be((new DateTime(2026, 1, 1), Wed, PeriodType.Custom));
    }

    [Fact]
    public void Picker_Next_Button_Advances_And_Raises_ValueChanged()
    {
        DatePeriod? value = null;
        var cut = RenderComponent<StackPeriodPicker>(p => p
            .Add(x => x.Today, Wed)
            .Add(x => x.PeriodTypes, new[] { PeriodType.Month, PeriodType.Quarter })
            .Add(x => x.Culture, CultureInfo.InvariantCulture)
            .Add(x => x.ValueChanged, v => value = v));

        cut.Find("button[aria-label='Next period']").Click();
        value!.Value.Start.Should().Be(new DateTime(2026, 11, 1));

        cut.FindAll("[role=tab]").Single(t => t.TextContent == "Quarter").Click();
        value!.Value.Type.Should().Be(PeriodType.Quarter);
    }

    [Fact]
    public void Picker_Preset_Select_Sets_Value()
    {
        DatePeriod? value = null;
        var cut = RenderComponent<StackPeriodPicker>(p => p
            .Add(x => x.Today, Wed)
            .Add(x => x.ValueChanged, v => value = v));

        cut.Find("select[aria-label='Quick select period']").Change("4"); // Last month
        value!.Value.Start.Should().Be(new DateTime(2026, 9, 1));
    }

    [Fact]
    public void Picker_Max_Disables_Next()
    {
        var cut = RenderComponent<StackPeriodPicker>(p => p
            .Add(x => x.Today, Wed)
            .Add(x => x.PeriodTypes, new[] { PeriodType.Month })
            .Add(x => x.Max, Wed));
        cut.Find("button[aria-label='Next period']").HasAttribute("disabled").Should().BeTrue();
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// PIN pad
// ═══════════════════════════════════════════════════════════════════════════════

public class PinPadTests : TestContext
{
    [Fact]
    public void Completes_After_Length_Digits()
    {
        string? done = null;
        var cut = RenderComponent<StackPinPad>(p => p
            .Add(x => x.Length, 4)
            .Add(x => x.OnComplete, v => done = v));

        foreach (var d in "1234")
            cut.Find($"button[aria-label='{d}']").Click();

        done.Should().Be("1234");
    }

    [Fact]
    public void Backspace_And_Keyboard_Work()
    {
        string? value = null;
        var cut = RenderComponent<StackPinPad>(p => p.Add(x => x.ValueChanged, v => value = v));
        var root = cut.Find("[role=group]");
        root.KeyDown("7");
        root.KeyDown("8");
        root.KeyDown("Backspace");
        value.Should().Be("7");
    }

    [Fact]
    public void Error_Clears_Entry_And_Shows_Alert()
    {
        var cut = RenderComponent<StackPinPad>(p => p.Add(x => x.Value, "12"));
        cut.FindAll("span.bg-primary").Count.Should().Be(2);
        cut.SetParametersAndRender(p => p.Add(x => x.Error, "Wrong PIN"));
        cut.Find("[role=alert]").TextContent.Should().Be("Wrong PIN");
        cut.FindAll("span.bg-destructive").Should().BeEmpty();
    }

    [Fact]
    public void Shuffle_Keeps_All_Ten_Digits()
    {
        var cut = RenderComponent<StackPinPad>(p => p.Add(x => x.Shuffle, true));
        var labels = cut.FindAll("button[aria-label]").Select(b => b.GetAttribute("aria-label")).Where(l => l!.Length == 1).ToList();
        labels.Should().BeEquivalentTo(["0", "1", "2", "3", "4", "5", "6", "7", "8", "9"]);
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// JS-backed components (render + interop contracts)
// ═══════════════════════════════════════════════════════════════════════════════

public class JsBackedGapComponentTests : TestContext
{
    public JsBackedGapComponentTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        SetRendererInfo(new RendererInfo("WebAssembly", isInteractive: true));
    }

    [Fact]
    public void CountUp_Renders_Formatted_Value_For_Screen_Readers()
    {
        var cut = RenderComponent<StackCountUp>(p => p
            .Add(x => x.Value, 1234.5m)
            .Add(x => x.Decimals, 1)
            .Add(x => x.Prefix, "$")
            .Add(x => x.Culture, CultureInfo.InvariantCulture));
        cut.Find(".sr-only").TextContent.Should().Be("$1,234.5");
    }

    [Fact]
    public void CountUp_Animates_Via_Module_On_Change()
    {
        var module = JSInterop.SetupModule("./_content/MyStackBlazor/js/count-up.js");
        module.SetupVoid("animate", _ => true);
        var cut = RenderComponent<StackCountUp>(p => p.Add(x => x.Value, 10m));
        cut.SetParametersAndRender(p => p.Add(x => x.Value, 20m));
        module.VerifyInvoke("animate", calledTimes: 2); // initial 0→10, then 10→20
    }

    [Fact]
    public void BarcodeScanInput_Enter_Submits_Hardware_Scan()
    {
        string? scanned = null;
        var cut = RenderComponent<StackBarcodeScanInput>(p => p
            .Add(x => x.OnScan, s => scanned = s)
            .Add(x => x.ClearAfterScan, true));
        var input = cut.Find("input");
        input.Input(" 4006381333931 ");
        input.KeyDown("Enter");
        scanned.Should().Be("4006381333931");
        cut.Find("input").GetAttribute("value").Should().BeEmpty();
    }

    [Fact]
    public void BarcodeScanInput_Camera_Disabled_When_Unsupported()
    {
        var module = JSInterop.SetupModule("./_content/MyStackBlazor/js/barcode-scanner.js");
        module.Setup<bool>("isSupported", _ => true).SetResult(false);
        var cut = RenderComponent<StackBarcodeScanInput>();
        cut.WaitForAssertion(() => cut.Find("button[aria-label='Scan with camera']").HasAttribute("disabled").Should().BeTrue());
    }

    [Fact]
    public async Task BarcodeScanInput_Detected_Code_Raises_OnScan()
    {
        string? scanned = null;
        var cut = RenderComponent<StackBarcodeScanInput>(p => p.Add(x => x.OnScan, s => scanned = s));
        await cut.InvokeAsync(() => cut.Instance.HandleDetected("QR-42", "qr_code"));
        scanned.Should().Be("QR-42");
        cut.Find("input").GetAttribute("value").Should().Be("QR-42");
    }

    [Fact]
    public void VoiceSearch_Hidden_When_Unsupported_By_Default()
    {
        var module = JSInterop.SetupModule("./_content/MyStackBlazor/js/speech.js");
        module.Setup<bool>("isSupported", _ => true).SetResult(false);
        var cut = RenderComponent<StackVoiceSearchButton>();
        cut.WaitForAssertion(() => cut.FindAll("button").Should().BeEmpty());
    }

    [Fact]
    public async Task VoiceSearch_Result_Raises_OnResult()
    {
        var module = JSInterop.SetupModule("./_content/MyStackBlazor/js/speech.js");
        module.Setup<bool>("isSupported", _ => true).SetResult(true);
        string? result = null;
        var cut = RenderComponent<StackVoiceSearchButton>(p => p.Add(x => x.OnResult, r => result = r));
        cut.WaitForAssertion(() => cut.Find("button").GetAttribute("aria-label").Should().Be("Search by voice"));
        await cut.InvokeAsync(() => cut.Instance.HandleFinal("red shoes"));
        result.Should().Be("red shoes");
    }

    [Fact]
    public async Task PullToRefresh_Runs_OnRefresh_From_Js_Callback()
    {
        var calls = 0;
        var cut = RenderComponent<StackPullToRefresh>(p => p
            .Add(x => x.OnRefresh, () => { calls++; })
            .AddChildContent("<p>list</p>"));
        cut.Markup.Should().Contain("<p>list</p>");
        await cut.InvokeAsync(() => cut.Instance.OnPulledAsync());
        calls.Should().Be(1);
        cut.Instance.IsRefreshing.Should().BeFalse();
    }

    [Fact]
    public void Celebration_Shows_Message_When_Activated()
    {
        var cut = RenderComponent<StackCelebration>(p => p
            .Add(x => x.Title, "Goal reached!")
            .Add(x => x.Dismissible, true));
        cut.FindAll("[role=status]").Should().BeEmpty();

        cut.SetParametersAndRender(p => p.Add(x => x.Active, true));
        cut.WaitForAssertion(() => cut.Find("[role=status]").TextContent.Should().Contain("Goal reached!"));

        cut.FindAll("button").Single(b => b.TextContent == "Awesome!").Click();
        cut.FindAll("[role=status]").Should().BeEmpty();
    }
}
