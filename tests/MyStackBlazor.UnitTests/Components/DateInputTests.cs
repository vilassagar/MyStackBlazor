using System.Globalization;
using MyStackBlazor.Components.Form;

namespace MyStackBlazor.UnitTests.Components;

public class DateInputFormatTests
{
    private static readonly CultureInfo EnUs = new("en-US");

    private static DateInputFormat Format(string format) => new(format, EnUs);

    private static int IndexOf(DateInputFormat f, DatePartKind kind) =>
        Enumerable.Range(0, f.Tokens.Count).First(i => f.Tokens[i].Kind == kind);

    [Fact]
    public void Standard_Format_Expands_With_Culture()
    {
        var f = Format("d");
        f.Tokens.Select(t => t.Kind).Should().Equal(
            DatePartKind.Month, DatePartKind.Literal, DatePartKind.Day, DatePartKind.Literal, DatePartKind.Year);
        f.HasDate.Should().BeTrue();
        f.HasTime.Should().BeFalse();
    }

    [Fact]
    public void Custom_Format_With_Quoted_Literal_And_Names()
    {
        var f = Format("dddd, dd 'of' MMMM yyyy hh:mm tt");
        f.Tokens.Where(t => t.IsEditable).Select(t => t.Kind).Should().Equal(
            DatePartKind.Day, DatePartKind.MonthName, DatePartKind.Year, DatePartKind.Hour12, DatePartKind.Minute, DatePartKind.AmPm);
        f.Tokens.Should().Contain(t => t.Kind == DatePartKind.Literal && t.Text == " of ");
        f.Tokens[0].Kind.Should().Be(DatePartKind.Weekday);
    }

    [Fact]
    public void Load_And_Display()
    {
        var f = Format("dd MMM yyyy");
        f.Load(new DateTime(2026, 3, 7));
        var built = f.Build(DateTime.Today, 68, true);
        built.Should().Be(new DateTime(2026, 3, 7));
        string.Concat(Enumerable.Range(0, f.Tokens.Count).Select(i => f.Display(i, DateInputFormatPlaceholder.Formatted, built)))
            .Should().Be("07 Mar 2026");
    }

    [Theory]
    [InlineData(DateInputFormatPlaceholder.Formatted, "MM/dd/yyyy")]
    [InlineData(DateInputFormatPlaceholder.Short, "m/d/y")]
    [InlineData(DateInputFormatPlaceholder.Wide, "month/day/year")]
    public void Empty_Parts_Show_Placeholder(DateInputFormatPlaceholder placeholder, string expected)
    {
        var f = Format("MM/dd/yyyy");
        f.Load(null);
        string.Concat(Enumerable.Range(0, f.Tokens.Count).Select(i => f.Display(i, placeholder, null))).Should().Be(expected);
    }

    [Fact]
    public void Typing_Month_Digits_Completes_And_Restarts()
    {
        var f = Format("MM/dd/yyyy");
        f.Load(null);
        var month = IndexOf(f, DatePartKind.Month);
        var buffer = "";

        f.Type(month, '1', ref buffer).Should().BeFalse("1 could still become 10–12");
        f.Values[month].Should().Be(1);
        f.Type(month, '2', ref buffer).Should().BeTrue();
        f.Values[month].Should().Be(12);

        buffer = "";
        f.Type(month, '5', ref buffer).Should().BeTrue("no month starts with 5x");
        f.Values[month].Should().Be(5);

        buffer = "";
        f.Type(month, '0', ref buffer).Should().BeFalse();
        f.Values[month].Should().BeNull("a leading 0 waits for the next digit");
        f.Type(month, '9', ref buffer).Should().BeTrue();
        f.Values[month].Should().Be(9);
    }

    [Fact]
    public void Typing_Month_Name_By_Letters()
    {
        var f = Format("dd MMMM yyyy");
        f.Load(null);
        var month = IndexOf(f, DatePartKind.MonthName);
        var buffer = "";
        f.Type(month, 'j', ref buffer).Should().BeFalse("January, June, July");
        f.Values[month].Should().Be(1);
        f.Type(month, 'u', ref buffer).Should().BeFalse("June, July");
        f.Type(month, 'l', ref buffer).Should().BeTrue();
        f.Values[month].Should().Be(7);
    }

    [Fact]
    public void Two_Digit_Years_Use_TwoDigitYearMax()
    {
        var f = Format("dd/MM/yy");
        f.Load(new DateTime(2000, 1, 1));
        var year = IndexOf(f, DatePartKind.Year);
        f.Values[year] = 45;
        f.Build(DateTime.Today, 68, true)!.Value.Year.Should().Be(2045);
        f.Values[year] = 85;
        f.Build(DateTime.Today, 68, true)!.Value.Year.Should().Be(1985);
    }

    [Fact]
    public void Step_Wraps_And_AutoCorrect_Clamps_Day()
    {
        var f = Format("MM/dd/yyyy");
        f.Load(new DateTime(2026, 1, 31));
        var month = IndexOf(f, DatePartKind.Month);
        f.Step(month, 1, DateTime.Today);
        f.Values[month].Should().Be(2);
        f.Build(DateTime.Today, 68, autoCorrect: false).Should().BeNull("31 Feb is not a date");
        f.CorrectDay(DateTime.Today);
        f.Build(DateTime.Today, 68, true).Should().Be(new DateTime(2026, 2, 28));

        f.Values[month] = 12;
        f.Step(month, 1, DateTime.Today);
        f.Values[month].Should().Be(1, "months wrap");
    }

    [Fact]
    public void Twelve_Hour_Clock_With_AmPm()
    {
        var f = Format("hh:mm tt");
        f.Load(new DateTime(2026, 1, 1, 9, 30, 0));
        var ampm = IndexOf(f, DatePartKind.AmPm);
        var buffer = "";
        f.Type(ampm, 'p', ref buffer).Should().BeTrue();
        f.Build(new DateTime(2026, 1, 1), 68, true).Should().Be(new DateTime(2026, 1, 1, 21, 30, 0));
    }
}

public class DateInputComponentTests : TestContext
{
    public DateInputComponentTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = new CultureInfo("en-US");
    }

    private static void Type(IRenderedFragment cut, string part, string keys)
    {
        foreach (var key in keys)
            cut.Find($"[role=spinbutton][data-part={part}]").KeyDown(new KeyboardEventArgs { Key = key.ToString() });
    }

    [Fact]
    public void Renders_Segments_For_Value()
    {
        var cut = RenderComponent<StackDateInput<DateTime?>>(p => p
            .Add(d => d.Value, new DateTime(2026, 7, 4))
            .Add(d => d.Format, "dd/MM/yyyy"));
        cut.FindAll("[role=spinbutton]").Select(s => s.TextContent).Should().Equal("04", "07", "2026");
        cut.Find("[data-part=Day]").GetAttribute("aria-valuenow").Should().Be("4");
    }

    [Fact]
    public void Typing_All_Parts_Sets_Value()
    {
        DateTime? value = null;
        var cut = RenderComponent<StackDateInput<DateTime?>>(p => p
            .Add(d => d.Format, "MM/dd/yyyy")
            .Add(d => d.ValueChanged, v => value = v));

        Type(cut, "Month", "12");
        Type(cut, "Day", "25");
        Type(cut, "Year", "2026");

        value.Should().Be(new DateTime(2026, 12, 25));
    }

    [Fact]
    public void Arrow_Keys_Step_The_Part()
    {
        DateTime? value = null;
        var cut = RenderComponent<StackDateInput<DateTime?>>(p => p
            .Add(d => d.Value, new DateTime(2026, 1, 15))
            .Add(d => d.Format, "MM/dd/yyyy")
            .Add(d => d.ValueChanged, v => value = v));
        cut.Find("[data-part=Day]").KeyDown(new KeyboardEventArgs { Key = "ArrowUp" });
        value.Should().Be(new DateTime(2026, 1, 16));
    }

    [Fact]
    public void Backspace_Clears_Nullable_Value()
    {
        DateTime? value = new DateTime(2026, 1, 15);
        var cut = RenderComponent<StackDateInput<DateTime?>>(p => p
            .Add(d => d.Value, value)
            .Add(d => d.ValueChanged, v => value = v));
        cut.Find("[data-part=Day]").KeyDown(new KeyboardEventArgs { Key = "Backspace" });
        value.Should().BeNull();
    }

    [Fact]
    public void Out_Of_Range_Is_Invalid_And_Not_Applied()
    {
        DateTime? value = new DateTime(2026, 1, 15);
        var changed = false;
        var cut = RenderComponent<StackDateInput<DateTime?>>(p => p
            .Add(d => d.Value, value)
            .Add(d => d.Format, "MM/dd/yyyy")
            .Add(d => d.Max, new DateTime(2026, 6, 30))
            .Add(d => d.ValueChanged, _ => changed = true));
        Type(cut, "Year", "2027");
        changed.Should().BeFalse();
        cut.Find("[role=group]").GetAttribute("aria-invalid").Should().Be("true");
    }

    [Fact]
    public void DateOnly_And_TimeOnly_Types()
    {
        DateOnly date = default;
        var dateCut = RenderComponent<StackDateInput<DateOnly>>(p => p
            .Add(d => d.Format, "yyyy-MM-dd")
            .Add(d => d.ValueChanged, v => date = v));
        Type(dateCut, "Year", "2026");
        Type(dateCut, "Month", "03");
        Type(dateCut, "Day", "09");
        date.Should().Be(new DateOnly(2026, 3, 9));

        TimeOnly? time = null;
        var timeCut = RenderComponent<StackDateInput<TimeOnly?>>(p => p
            .Add(d => d.Format, "HH:mm")
            .Add(d => d.ValueChanged, v => time = v));
        Type(timeCut, "Hour24", "18");
        Type(timeCut, "Minute", "45");
        time.Should().Be(new TimeOnly(18, 45));
    }

    [Fact]
    public void Placeholder_Shown_When_Empty()
    {
        var cut = RenderComponent<StackDateInput<DateTime?>>(p => p.Add(d => d.Placeholder, "Birthday"));
        cut.Markup.Should().Contain("Birthday");
    }

    [Fact]
    public void Clear_Button()
    {
        DateTime? value = new DateTime(2026, 1, 1);
        var cut = RenderComponent<StackDateInput<DateTime?>>(p => p
            .Add(d => d.Value, value)
            .Add(d => d.ShowClearButton, true)
            .Add(d => d.ValueChanged, v => value = v));
        cut.Find("button[aria-label=Clear]").Click();
        value.Should().BeNull();
    }
}

public class DateCalendarTests : TestContext
{
    private static readonly DateTime May6 = new(2026, 5, 6);

    public DateCalendarTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = new CultureInfo("en-US");
    }

    private static void Click(IRenderedFragment cut, DateTime date, bool ctrl = false, bool shift = false) =>
        cut.Find($"[data-date='{date:yyyy-MM-dd}']").Click(new MouseEventArgs { CtrlKey = ctrl, ShiftKey = shift });

    [Fact]
    public void Single_Selection()
    {
        DateTime? value = null;
        var cut = RenderComponent<StackDateCalendar>(p => p.Add(c => c.Date, May6).Add(c => c.ValueChanged, v => value = v));
        Click(cut, new DateTime(2026, 5, 20));
        value.Should().Be(new DateTime(2026, 5, 20));
        cut.Find("[data-date='2026-05-20']").GetAttribute("aria-selected").Should().Be("true");
    }

    [Fact]
    public void Multiple_Selection_With_Ctrl_And_Shift()
    {
        List<DateTime>? selected = null;
        var cut = RenderComponent<StackDateCalendar>(p => p
            .Add(c => c.SelectionMode, CalendarSelectionMode.Multiple)
            .Add(c => c.Date, May6)
            .Add(c => c.SelectedDatesChanged, v => selected = v));

        Click(cut, new DateTime(2026, 5, 4));
        Click(cut, new DateTime(2026, 5, 8), ctrl: true);
        selected.Should().Equal(new DateTime(2026, 5, 4), new DateTime(2026, 5, 8));

        Click(cut, new DateTime(2026, 5, 11), shift: true);
        selected.Should().HaveCount(4, "8th..11th from the last anchor").And.StartWith(new DateTime(2026, 5, 8));
    }

    [Fact]
    public void Range_Selection_And_Reverse()
    {
        DateTime? start = null, end = null;
        var cut = RenderComponent<StackDateCalendar>(p => p
            .Add(c => c.SelectionMode, CalendarSelectionMode.Range)
            .Add(c => c.Date, May6)
            .Add(c => c.AllowReverse, true)
            .Add(c => c.RangeStartChanged, v => start = v)
            .Add(c => c.RangeEndChanged, v => end = v));

        Click(cut, new DateTime(2026, 5, 10));
        Click(cut, new DateTime(2026, 5, 4));
        start.Should().Be(new DateTime(2026, 5, 4));
        end.Should().Be(new DateTime(2026, 5, 10));
        cut.Find("[data-date='2026-05-07']").ClassList.Should().Contain("bg-primary/15");
    }

    [Fact]
    public void Multiple_Views_And_Vertical_Orientation()
    {
        var cut = RenderComponent<StackDateCalendar>(p => p
            .Add(c => c.Date, May6)
            .Add(c => c.Views, 3)
            .Add(c => c.Orientation, CalendarOrientation.Vertical));
        cut.FindAll("[role=grid]").Select(g => g.GetAttribute("aria-label")).Should().Equal("May 2026", "June 2026", "July 2026");
        cut.Find("[role=application]").ClassList.Should().Contain("flex-col");
    }

    [Fact]
    public void Drill_Up_And_Down_Respects_Bottom_And_Top_Views()
    {
        CalendarView? view = null;
        var cut = RenderComponent<StackDateCalendar>(p => p
            .Add(c => c.Date, May6)
            .Add(c => c.TopView, CalendarView.Decade)
            .Add(c => c.ViewChanged, v => view = v));

        cut.Find("button[aria-label$='wider view']").Click();
        view.Should().Be(CalendarView.Year);
        cut.Find("button[aria-label$='wider view']").Click();
        view.Should().Be(CalendarView.Decade);
        cut.Find("button[aria-label$='wider view']").HasAttribute("disabled").Should().BeTrue("TopView is Decade");

        Click(cut, new DateTime(2027, 1, 1));
        view.Should().Be(CalendarView.Year, "clicking a year drills down");
    }

    [Fact]
    public void Min_Max_DisabledDates_And_Templates()
    {
        var cut = RenderComponent<StackDateCalendar>(p => p
            .Add(c => c.Date, May6)
            .Add(c => c.Min, new DateTime(2026, 5, 3))
            .Add(c => c.DisabledDates, [new DateTime(2026, 5, 10)])
            .Add(c => c.MonthCellTemplate, d => $"<i>{d.Day}</i>")
            .Add(c => c.OnCellRender, (CalendarCellRenderEventArgs a) => { if (a.Date.DayOfWeek == DayOfWeek.Saturday) a.Class = "weekend"; }));

        cut.Find("[data-date='2026-05-02']").GetAttribute("aria-disabled").Should().Be("true");
        cut.Find("[data-date='2026-05-10']").GetAttribute("aria-disabled").Should().Be("true");
        cut.Find("[data-date='2026-05-06'] i").TextContent.Should().Be("6");
        cut.Find("[data-date='2026-05-09']").ClassList.Should().Contain("weekend");
    }

    [Fact]
    public void Keyboard_Moves_And_Selects()
    {
        DateTime? value = null;
        var cut = RenderComponent<StackDateCalendar>(p => p.Add(c => c.Date, May6).Add(c => c.ValueChanged, v => value = v));
        var grid = cut.Find("[role=application]");
        grid.KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        cut.Find("[role=application]").KeyDown(new KeyboardEventArgs { Key = "ArrowRight" });
        cut.Find("[role=application]").KeyDown(new KeyboardEventArgs { Key = "Enter" });
        value.Should().Be(new DateTime(2026, 5, 14));
    }

    [Fact]
    public void Week_Numbers()
    {
        var cut = RenderComponent<StackDateCalendar>(p => p.Add(c => c.Date, May6).Add(c => c.ShowWeekNumbers, true));
        cut.FindAll("[role=rowheader]").Should().HaveCount(6);
    }
}

public class DatePickerTests : TestContext
{
    public DatePickerTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = new CultureInfo("en-US");
    }

    [Fact]
    public void Calendar_Button_Opens_And_Picking_Closes()
    {
        DateTime? value = null;
        object? changed = null;
        var cut = RenderComponent<StackDatePicker<DateTime?>>(p => p
            .Add(d => d.Value, new DateTime(2026, 5, 6))
            .Add(d => d.ValueChanged, v => value = v)
            .Add(d => d.OnChange, v => changed = v));

        cut.Find("button[aria-label='Open calendar']").Click();
        cut.Find("[data-date='2026-05-21']").Click();

        value.Should().Be(new DateTime(2026, 5, 21));
        changed.Should().Be(new DateTime(2026, 5, 21));
        cut.FindAll("[role=dialog]").Should().BeEmpty();
    }

    [Fact]
    public void Typing_Works_And_Format_Default_Is_Culture_Short_Date()
    {
        DateTime? value = null;
        var cut = RenderComponent<StackDatePicker<DateTime?>>(p => p.Add(d => d.ValueChanged, v => value = v));
        cut.FindAll("[role=spinbutton]").Select(s => s.GetAttribute("data-part")).Should().Equal("Month", "Day", "Year");
        foreach (var (part, keys) in new[] { ("Month", "3"), ("Day", "14"), ("Year", "2026") })
            foreach (var k in keys) cut.Find($"[data-part={part}]").KeyDown(new KeyboardEventArgs { Key = k.ToString() });
        value.Should().Be(new DateTime(2026, 3, 14));
    }

    [Fact]
    public void OnOpen_Cancel_And_Disabled()
    {
        var cancel = RenderComponent<StackDatePicker<DateTime?>>(p => p.Add(d => d.OnOpen, (DatePickerOpenEventArgs a) => a.IsCancelled = true));
        cancel.Find("button[aria-label='Open calendar']").Click();
        cancel.FindAll("[role=dialog]").Should().BeEmpty();

        var disabled = RenderComponent<StackDatePicker<DateTime?>>(p => p.Add(d => d.Disabled, true));
        disabled.Find("button[aria-label='Open calendar']").HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void BottomView_Year_Picks_Months()
    {
        DateOnly? value = null;
        var cut = RenderComponent<StackDatePicker<DateOnly?>>(p => p
            .Add(d => d.Value, new DateOnly(2026, 1, 1))
            .Add(d => d.BottomView, CalendarView.Year)
            .Add(d => d.Format, "MMMM yyyy")
            .Add(d => d.ValueChanged, v => value = v));
        cut.Find("button[aria-label='Open calendar']").Click();
        cut.Find("[data-date='2026-09-01']").Click();
        value.Should().Be(new DateOnly(2026, 9, 1));
    }
}

public class TimePickerTests : TestContext
{
    public TimePickerTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = new CultureInfo("en-US");
    }

    [Fact]
    public void Lists_Follow_Format_And_Steps()
    {
        var cut = RenderComponent<StackTimePicker<TimeOnly?>>(p => p
            .Add(t => t.Format, "HH:mm")
            .Add(t => t.Steps, new TimePickerSteps { Minute = 15 }));
        cut.Find("button[aria-label='Open time list']").Click();
        cut.FindAll("[role=listbox]").Select(l => l.GetAttribute("aria-label")).Should().Equal("Hour", "Min");
        cut.FindAll("[role=listbox]")[1].QuerySelectorAll("[role=option]").Select(o => o.TextContent.Trim())
            .Should().Equal("00", "15", "30", "45");
    }

    [Fact]
    public void Set_Applies_The_Selected_Time()
    {
        TimeOnly? value = null;
        var cut = RenderComponent<StackTimePicker<TimeOnly?>>(p => p
            .Add(t => t.Value, new TimeOnly(9, 0))
            .Add(t => t.Format, "HH:mm")
            .Add(t => t.ValueChanged, v => value = v));
        cut.Find("button[aria-label='Open time list']").Click();
        cut.FindAll("[role=listbox]")[0].QuerySelectorAll("[role=option]")[14].Click(); // 14:00
        value.Should().BeNull("nothing applies until Set");
        cut.FindAll("button").First(b => b.TextContent.Trim() == "Set").Click();
        value.Should().Be(new TimeOnly(14, 0));
    }

    [Fact]
    public void Earlier_TimeSpan_And_Use24Hour_Api()
    {
        TimeSpan? value = null;
        var cut = RenderComponent<StackTimePicker<TimeSpan?>>(p => p
            .Add(t => t.Use24Hour, false)
            .Add(t => t.ValueChanged, v => value = v));
        cut.FindAll("[role=spinbutton]").Select(s => s.GetAttribute("data-part")).Should().Equal("Hour12", "Minute", "AmPm");
        foreach (var (part, keys) in new[] { ("Hour12", "07"), ("Minute", "30"), ("AmPm", "p") })
            foreach (var k in keys) cut.Find($"[data-part={part}]").KeyDown(new KeyboardEventArgs { Key = k.ToString() });
        value.Should().Be(new TimeSpan(19, 30, 0));
    }

    [Fact]
    public void Min_Max_Disable_Hours()
    {
        var cut = RenderComponent<StackTimePicker<DateTime?>>(p => p
            .Add(t => t.Format, "HH:mm")
            .Add(t => t.Min, DateTime.Today.AddHours(9))
            .Add(t => t.Max, DateTime.Today.AddHours(17)));
        cut.Find("button[aria-label='Open time list']").Click();
        var hours = cut.FindAll("[role=listbox]")[0].QuerySelectorAll("[role=option]");
        hours[8].GetAttribute("aria-disabled").Should().Be("true");
        hours[9].HasAttribute("aria-disabled").Should().BeFalse();
        hours[18].GetAttribute("aria-disabled").Should().Be("true");
    }
}

public class DateTimePickerTests : TestContext
{
    public DateTimePickerTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = new CultureInfo("en-US");
    }

    [Fact]
    public void Date_Then_Time_Then_Set()
    {
        DateTime? value = null;
        var cut = RenderComponent<StackDateTimePicker<DateTime?>>(p => p
            .Add(d => d.Value, new DateTime(2026, 5, 6, 9, 0, 0))
            .Add(d => d.Format, "MM/dd/yyyy HH:mm")
            .Add(d => d.ValueChanged, v => value = v));

        cut.Find("button[aria-label='Open date and time picker']").Click();
        cut.Find("[data-date='2026-05-20']").Click();
        cut.Find("[role=tab][aria-selected=true]").TextContent.Should().Be("Time", "picking a date moves to the time tab");
        cut.FindAll("[role=listbox]")[0].QuerySelectorAll("[role=option]")[16].Click();
        cut.FindAll("button").First(b => b.TextContent.Trim() == "Set").Click();

        value.Should().Be(new DateTime(2026, 5, 20, 16, 0, 0));
    }

    [Fact]
    public void Cancel_Keeps_Value()
    {
        var changed = false;
        var cut = RenderComponent<StackDateTimePicker<DateTime?>>(p => p
            .Add(d => d.Value, new DateTime(2026, 5, 6, 9, 0, 0))
            .Add(d => d.ValueChanged, _ => changed = true));
        cut.Find("button[aria-label='Open date and time picker']").Click();
        cut.Find("[data-date='2026-05-20']").Click();
        cut.FindAll("button").First(b => b.TextContent.Trim() == "Cancel").Click();
        changed.Should().BeFalse();
        cut.FindAll("[role=dialog]").Should().BeEmpty();
    }
}
