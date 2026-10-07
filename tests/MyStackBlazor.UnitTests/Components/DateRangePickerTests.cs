using System.Globalization;
using MyStackBlazor.Components.Form;

namespace MyStackBlazor.UnitTests.Components;

public class DateRangePickerTests : TestContext
{
    private static readonly DateTime May1 = new(2026, 5, 1);

    public DateRangePickerTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = new CultureInfo("en-US");
    }

    private IRenderedComponent<StackDateRangePicker<DateTime?>> Render(
        Action<ComponentParameterCollectionBuilder<StackDateRangePicker<DateTime?>>>? configure = null) =>
        RenderComponent<StackDateRangePicker<DateTime?>>(p =>
        {
            p.Add(d => d.Id, "drp");
            configure?.Invoke(p);
        });

    private static void ClickDate(IRenderedFragment cut, DateTime date) =>
        cut.Find($"[data-date='{date:yyyy-MM-dd}']").Click();

    private static void OpenAt(IRenderedComponent<StackDateRangePicker<DateTime?>> cut, DateTime date)
    {
        cut.Find("#drp-start").Click();
        cut.InvokeAsync(() => cut.Instance.NavigateTo(date, CalendarView.Month));
    }

    [Fact]
    public void Renders_Two_Inputs_With_Formatted_Values()
    {
        var cut = Render(p => p
            .Add(d => d.StartValue, new DateTime(2026, 5, 3))
            .Add(d => d.EndValue, new DateTime(2026, 5, 9))
            .Add(d => d.Format, "dd/MM/yyyy"));
        cut.Find("#drp-start").GetAttribute("value").Should().Be("03/05/2026");
        cut.Find("#drp-end").GetAttribute("value").Should().Be("09/05/2026");
    }

    [Fact]
    public void Click_Opens_Popup_With_Two_Months()
    {
        var cut = Render();
        OpenAt(cut, May1);
        cut.FindAll("[role=dialog]").Should().HaveCount(1);
        cut.FindAll("[role=grid][aria-label]").Select(g => g.GetAttribute("aria-label"))
           .Should().Equal("May 2026", "June 2026");
    }

    [Fact]
    public void Picking_Start_Then_End_Sets_Range_And_Closes_And_Fires_OnChange()
    {
        DateTime? start = null, end = null;
        DateRangePickerChangeEventArgs? change = null;
        var cut = Render(p => p
            .Add(d => d.StartValueChanged, v => start = v)
            .Add(d => d.EndValueChanged, v => end = v)
            .Add(d => d.OnChange, a => change = a));

        OpenAt(cut, May1);
        ClickDate(cut, new DateTime(2026, 5, 4));
        ClickDate(cut, new DateTime(2026, 5, 12));

        start.Should().Be(new DateTime(2026, 5, 4));
        end.Should().Be(new DateTime(2026, 5, 12));
        cut.FindAll("[role=dialog]").Should().BeEmpty();
        change!.StartValue.Should().Be(new DateTime(2026, 5, 4));
        change.EndValue.Should().Be(new DateTime(2026, 5, 12));
    }

    [Fact]
    public void Earlier_End_Restarts_Range_Without_AllowReverse()
    {
        DateTime? start = null, end = null;
        var cut = Render(p => p
            .Add(d => d.StartValueChanged, v => start = v)
            .Add(d => d.EndValueChanged, v => end = v));

        OpenAt(cut, May1);
        ClickDate(cut, new DateTime(2026, 5, 10));
        ClickDate(cut, new DateTime(2026, 5, 3));

        start.Should().Be(new DateTime(2026, 5, 3));
        end.Should().BeNull();
        cut.FindAll("[role=dialog]").Should().HaveCount(1);
    }

    [Fact]
    public void AllowReverse_Swaps_Dates()
    {
        DateTime? start = null, end = null;
        var cut = Render(p => p
            .Add(d => d.AllowReverse, true)
            .Add(d => d.StartValueChanged, v => start = v)
            .Add(d => d.EndValueChanged, v => end = v));

        OpenAt(cut, May1);
        ClickDate(cut, new DateTime(2026, 5, 10));
        ClickDate(cut, new DateTime(2026, 5, 3));

        start.Should().Be(new DateTime(2026, 5, 3));
        end.Should().Be(new DateTime(2026, 5, 10));
    }

    [Fact]
    public void Range_Cells_Are_Highlighted()
    {
        var cut = Render(p => p
            .Add(d => d.StartValue, new DateTime(2026, 5, 4))
            .Add(d => d.EndValue, new DateTime(2026, 5, 6)));
        cut.Find("#drp-start").Click();

        cut.Find("[data-date='2026-05-04']").GetAttribute("aria-selected").Should().Be("true");
        cut.Find("[data-date='2026-05-06']").GetAttribute("aria-selected").Should().Be("true");
        cut.Find("[data-date='2026-05-05']").ClassList.Should().Contain("bg-primary/15");
    }

    [Fact]
    public void Min_Max_And_DisabledDates_Block_Selection()
    {
        DateTime? start = null;
        var cut = Render(p => p
            .Add(d => d.Min, new DateTime(2026, 5, 5))
            .Add(d => d.Max, new DateTime(2026, 6, 20))
            .Add(d => d.DisabledDates, [new DateTime(2026, 5, 8)])
            .Add(d => d.StartValueChanged, v => start = v));

        OpenAt(cut, May1);
        cut.Find("[data-date='2026-05-04']").GetAttribute("aria-disabled").Should().Be("true");
        cut.Find("[data-date='2026-05-08']").GetAttribute("aria-disabled").Should().Be("true");
        cut.Find("[data-date='2026-06-21']").GetAttribute("aria-disabled").Should().Be("true");

        ClickDate(cut, new DateTime(2026, 5, 8));
        start.Should().BeNull();
        cut.Find("button[aria-label=Previous]").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void Typing_A_Date_Sets_Value()
    {
        DateTime? start = null;
        var cut = Render(p => p
            .Add(d => d.Format, "yyyy-MM-dd")
            .Add(d => d.StartValueChanged, v => start = v));
        cut.Find("#drp-start").Change("2026-07-15");
        start.Should().Be(new DateTime(2026, 7, 15));
    }

    [Fact]
    public void Typing_Invalid_Text_Reverts()
    {
        var cut = Render(p => p
            .Add(d => d.Format, "yyyy-MM-dd")
            .Add(d => d.StartValue, new DateTime(2026, 1, 2)));
        cut.Find("#drp-start").Input("garbage");
        cut.Find("#drp-start").Change("garbage");
        cut.Find("#drp-start").GetAttribute("value").Should().Be("2026-01-02");
    }

    [Fact]
    public void Title_Click_Goes_Up_And_BottomView_Year_Selects_Months()
    {
        DateTime? start = null;
        var cut = Render(p => p
            .Add(d => d.BottomView, CalendarView.Year)
            .Add(d => d.View, CalendarView.Year)
            .Add(d => d.StartValueChanged, v => start = v));

        cut.Find("#drp-start").Click();
        cut.InvokeAsync(() => cut.Instance.NavigateTo(May1, CalendarView.Year));
        ClickDate(cut, new DateTime(2026, 3, 1));
        start.Should().Be(new DateTime(2026, 3, 1));
    }

    [Fact]
    public void Header_Navigates_Up_To_Decade()
    {
        CalendarView? view = null;
        var cut = Render(p => p.Add(d => d.ViewChanged, v => view = v));
        OpenAt(cut, May1);
        cut.FindAll("[role=dialog] button[aria-label$='wider view']")[0].Click();
        view.Should().Be(CalendarView.Year);
        cut.Find("[data-date='2026-03-01']").TextContent.Trim().Should().Be("Mar");
    }

    [Fact]
    public void Keyboard_Navigation_And_Enter_Select()
    {
        DateTime? start = null;
        var cut = Render(p => p.Add(d => d.StartValueChanged, v => start = v));
        OpenAt(cut, May1);
        var grid = cut.Find("[role=application]");
        grid.Focus();
        cut.Find("[role=application]").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        cut.Find("[role=application]").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        cut.Find("[role=application]").KeyDown(new KeyboardEventArgs { Key = "Enter" });
        start.Should().Be(new DateTime(2026, 5, 9));
    }

    [Fact]
    public void ShowWeekNumbers_Renders_Row_Headers()
    {
        var cut = Render(p => p.Add(d => d.ShowWeekNumbers, true));
        OpenAt(cut, May1);
        cut.FindAll("[role=rowheader]").Should().HaveCount(12);
    }

    [Fact]
    public void ShowOtherMonthDays_False_Hides_Adjacent_Days()
    {
        var cut = Render(p => p.Add(d => d.ShowOtherMonthDays, false));
        OpenAt(cut, May1);
        cut.FindAll("[data-date='2026-04-30']").Should().BeEmpty();
    }

    [Fact]
    public void OnOpen_And_OnClose_Can_Cancel()
    {
        var cancelOpen = Render(p => p.Add(d => d.OnOpen, (DateRangePickerOpenEventArgs a) => a.IsCancelled = true));
        cancelOpen.Find("#drp-start").Click();
        cancelOpen.FindAll("[role=dialog]").Should().BeEmpty();

        var cancelClose = Render(p => p.Add(d => d.OnClose, (DateRangePickerCloseEventArgs a) => a.IsCancelled = true));
        OpenAt(cancelClose, May1);
        ClickDate(cancelClose, new DateTime(2026, 5, 4));
        ClickDate(cancelClose, new DateTime(2026, 5, 5));
        cancelClose.FindAll("[role=dialog]").Should().HaveCount(1);
    }

    [Fact]
    public void ShowClearButton_Clears_Both()
    {
        DateTime? start = May1, end = May1;
        var cut = Render(p => p
            .Add(d => d.StartValue, May1).Add(d => d.EndValue, May1.AddDays(2))
            .Add(d => d.ShowClearButton, true)
            .Add(d => d.StartValueChanged, v => start = v)
            .Add(d => d.EndValueChanged, v => end = v));
        cut.Find("button[aria-label=Clear]").Click();
        start.Should().BeNull();
        end.Should().BeNull();
    }

    [Fact]
    public void Disabled_And_ReadOnly()
    {
        var disabled = Render(p => p.Add(d => d.Enabled, false));
        disabled.Find("#drp-start").HasAttribute("disabled").Should().BeTrue();

        var readOnly = Render(p => p.Add(d => d.ReadOnly, true));
        readOnly.Find("#drp-start").Click();
        readOnly.FindAll("[role=dialog]").Should().BeEmpty();
        readOnly.Find("#drp-end").HasAttribute("readonly").Should().BeTrue();
    }

    [Fact]
    public void OnCalendarCellRender_Adds_Class()
    {
        var cut = Render(p => p.Add(d => d.OnCalendarCellRender,
            (DateRangePickerCalendarCellRenderEventArgs a) => { if (a.Date.DayOfWeek == DayOfWeek.Sunday) a.Class = "weekend"; }));
        OpenAt(cut, May1);
        cut.Find("[data-date='2026-05-03']").ClassList.Should().Contain("weekend");
    }

    [Fact]
    public void DateOnly_Type_Is_Supported()
    {
        DateOnly start = default;
        var cut = RenderComponent<StackDateRangePicker<DateOnly>>(p => p
            .Add(d => d.Id, "drp")
            .Add(d => d.StartValueChanged, v => start = v));
        cut.Find("#drp-start").Click();
        cut.InvokeAsync(() => cut.Instance.NavigateTo(May1, CalendarView.Month));
        ClickDate(cut, new DateTime(2026, 5, 7));
        start.Should().Be(new DateOnly(2026, 5, 7));
    }

    [Fact]
    public void Ids_And_Appearance()
    {
        var cut = Render(p => p
            .Add(d => d.StartId, "from")
            .Add(d => d.EndId, "to")
            .Add(d => d.Size, "lg")
            .Add(d => d.Rounded, "full"));
        cut.Find("#from").Should().NotBeNull();
        cut.Find("#to").ParentElement!.ClassList.Should().Contain(["h-12", "rounded-full"]);
    }
}
