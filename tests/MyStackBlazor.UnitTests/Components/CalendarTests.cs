using MyStackBlazor.Components.Calendar;

namespace MyStackBlazor.UnitTests.Components;

public class CalendarTests : TestContext
{
    // ── Helpers ──────────────────────────────────────────────────────────────

    private static CalendarEvent MakeEvent(string title, int offsetDays = 0, bool allDay = false, string color = "blue") =>
        new CalendarEvent
        {
            Title   = title,
            Start   = DateTime.Today.AddDays(offsetDays),
            End     = DateTime.Today.AddDays(offsetDays).AddHours(allDay ? 0 : 1),
            AllDay  = allDay,
            Color   = color,
        };

    // ── Rendering ─────────────────────────────────────────────────────────────

    [Fact]
    public void Calendar_Renders_Toolbar()
    {
        var cut = RenderComponent<MsCalendar>(p => p
            .Add(c => c.Events, []));

        // Toolbar has Today, Previous, Next, view buttons
        cut.FindAll("button").Count.Should().BeGreaterThan(3);
    }

    [Fact]
    public void Calendar_Renders_WithNoEvents()
    {
        var act = () => RenderComponent<MsCalendar>(p => p.Add(c => c.Events, []));
        act.Should().NotThrow();
    }

    [Fact]
    public void Calendar_MonthView_Default_ShowsDayHeaders()
    {
        var cut = RenderComponent<MsCalendar>(p => p
            .Add(c => c.Events, [])
            .Add(c => c.DefaultView, CalendarViewMode.Month));

        // Month view shows day-of-week headers (Sun/Mon etc.)
        var text = cut.Markup;
        text.Should().ContainAny("Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat");
    }

    [Fact]
    public void Calendar_WeekView_ShowsTimeGrid()
    {
        var cut = RenderComponent<MsCalendar>(p => p
            .Add(c => c.Events, [])
            .Add(c => c.DefaultView, CalendarViewMode.Week));

        // Week view has a time grid with hour labels
        cut.Markup.Should().ContainAny("AM", "PM", "12 PM", "1 PM");
    }

    [Fact]
    public void Calendar_DayView_ShowsSingleDayHeader()
    {
        var cut = RenderComponent<MsCalendar>(p => p
            .Add(c => c.Events, [])
            .Add(c => c.DefaultView, CalendarViewMode.Day));

        // Day view shows the time grid with hour labels
        cut.Markup.Should().ContainAny("AM", "PM");
    }

    // ── View switcher ─────────────────────────────────────────────────────────

    [Fact]
    public void Calendar_MonthWeekDay_ViewButtons_Present()
    {
        var cut = RenderComponent<MsCalendar>(p => p.Add(c => c.Events, []));

        var btnTexts = cut.FindAll("button").Select(b => b.TextContent.Trim()).ToList();
        btnTexts.Should().Contain("Month");
        btnTexts.Should().Contain("Week");
        btnTexts.Should().Contain("Day");
    }

    [Fact]
    public async Task Calendar_ClickWeek_SwitchesToWeekView()
    {
        var cut = RenderComponent<MsCalendar>(p => p
            .Add(c => c.Events, [])
            .Add(c => c.DefaultView, CalendarViewMode.Month));

        var weekBtn = cut.FindAll("button").First(b => b.TextContent.Trim() == "Week");
        await weekBtn.ClickAsync(new MouseEventArgs());

        // After clicking Week, time grid should appear
        cut.Markup.Should().ContainAny("AM", "PM");
    }

    [Fact]
    public async Task Calendar_ClickDay_SwitchesToDayView()
    {
        var cut = RenderComponent<MsCalendar>(p => p
            .Add(c => c.Events, [])
            .Add(c => c.DefaultView, CalendarViewMode.Month));

        var dayBtn = cut.FindAll("button").First(b => b.TextContent.Trim() == "Day");
        await dayBtn.ClickAsync(new MouseEventArgs());

        cut.Markup.Should().ContainAny("AM", "PM");
    }

    // ── Today / navigation ────────────────────────────────────────────────────

    [Fact]
    public void Calendar_TodayButton_Present()
    {
        var cut = RenderComponent<MsCalendar>(p => p.Add(c => c.Events, []));
        cut.FindAll("button").Select(b => b.TextContent.Trim())
           .Should().Contain("Today");
    }

    [Fact]
    public async Task Calendar_ClickNext_UpdatesPeriodLabel()
    {
        var cut = RenderComponent<MsCalendar>(p => p
            .Add(c => c.Events, [])
            .Add(c => c.DefaultView, CalendarViewMode.Month));

        var prevLabel = cut.Find("h2").TextContent;
        var nextBtn = cut.Find("[aria-label=Next]");
        await nextBtn.ClickAsync(new MouseEventArgs());

        cut.Find("h2").TextContent.Should().NotBe(prevLabel);
    }

    [Fact]
    public async Task Calendar_ClickPrev_UpdatesPeriodLabel()
    {
        var cut = RenderComponent<MsCalendar>(p => p
            .Add(c => c.Events, [])
            .Add(c => c.DefaultView, CalendarViewMode.Month));

        var prevLabel = cut.Find("h2").TextContent;
        var prevBtn = cut.Find("[aria-label=Previous]");
        await prevBtn.ClickAsync(new MouseEventArgs());

        cut.Find("h2").TextContent.Should().NotBe(prevLabel);
    }

    // ── Events rendering ──────────────────────────────────────────────────────

    [Fact]
    public void Calendar_MonthView_ShowsEventTitle()
    {
        var cut = RenderComponent<MsCalendar>(p => p
            .Add(c => c.Events, [MakeEvent("Team Standup")])
            .Add(c => c.DefaultView, CalendarViewMode.Month));

        cut.Markup.Should().Contain("Team Standup");
    }

    [Fact]
    public void Calendar_MonthView_ShowsAllDayEvent()
    {
        var cut = RenderComponent<MsCalendar>(p => p
            .Add(c => c.Events, [MakeEvent("Holiday", allDay: true)])
            .Add(c => c.DefaultView, CalendarViewMode.Month));

        cut.Markup.Should().Contain("Holiday");
    }

    [Fact]
    public void Calendar_MultipleEvents_AllShown_InMarkup()
    {
        var events = new List<CalendarEvent>
        {
            MakeEvent("Standup"),
            MakeEvent("Review", offsetDays: 2),
            MakeEvent("Planning", offsetDays: 4),
        };

        var cut = RenderComponent<MsCalendar>(p => p
            .Add(c => c.Events, events)
            .Add(c => c.DefaultView, CalendarViewMode.Month));

        cut.Markup.Should().Contain("Standup");
        cut.Markup.Should().Contain("Review");
        cut.Markup.Should().Contain("Planning");
    }

    // ── AllowAdd ──────────────────────────────────────────────────────────────

    [Fact]
    public void Calendar_AllowAdd_True_ShowsAddEventButton()
    {
        var cut = RenderComponent<MsCalendar>(p => p
            .Add(c => c.Events, [])
            .Add(c => c.AllowAdd, true));

        cut.FindAll("button").Select(b => b.TextContent)
           .Should().ContainMatch("*Add*");
    }

    [Fact]
    public void Calendar_AllowAdd_False_HidesAddEventButton()
    {
        var cut = RenderComponent<MsCalendar>(p => p
            .Add(c => c.Events, [])
            .Add(c => c.AllowAdd, false));

        cut.FindAll("button").Select(b => b.TextContent.Trim())
           .Should().NotContainMatch("*Add event*");
    }

    // ── ShowMiniCalendar ──────────────────────────────────────────────────────

    [Fact]
    public void Calendar_ShowMiniCalendar_False_HidesSidebar()
    {
        var cut = RenderComponent<MsCalendar>(p => p
            .Add(c => c.Events, [])
            .Add(c => c.ShowMiniCalendar, false));

        cut.FindAll("aside").Count.Should().Be(0);
    }

    [Fact]
    public void Calendar_ShowMiniCalendar_True_HasAside()
    {
        var cut = RenderComponent<MsCalendar>(p => p
            .Add(c => c.Events, [])
            .Add(c => c.ShowMiniCalendar, true));

        // Aside is hidden on mobile via CSS class "hidden lg:flex"
        cut.FindAll("aside").Count.Should().BeGreaterThan(0);
    }

    // ── Callbacks ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task Calendar_AddModal_SaveNewEvent_FiresOnEventAdd()
    {
        CalendarEvent? added = null;
        var cut = RenderComponent<MsCalendar>(p => p
            .Add(c => c.Events, [])
            .Add(c => c.AllowAdd, true)
            .Add(c => c.OnEventAdd,
                EventCallback.Factory.Create<CalendarEvent>(this, e => added = e)));

        // Click the "Add event" button to open modal
        var addBtn = cut.FindAll("button").First(b => b.TextContent.Contains("Add"));
        await addBtn.ClickAsync(new MouseEventArgs());

        // Fill in title
        var titleInput = cut.Find("input[placeholder='Event title']");
        await titleInput.InputAsync(new ChangeEventArgs { Value = "New Meeting" });

        // Click Save
        var saveBtn = cut.FindAll("button").First(b => b.TextContent.Trim() == "Save");
        await saveBtn.ClickAsync(new MouseEventArgs());

        added.Should().NotBeNull();
        added!.Title.Should().Be("New Meeting");
    }

    // ── Colors ───────────────────────────────────────────────────────────────

    [Theory]
    [InlineData("blue")]
    [InlineData("red")]
    [InlineData("green")]
    [InlineData("purple")]
    [InlineData("orange")]
    [InlineData("teal")]
    [InlineData("yellow")]
    [InlineData("pink")]
    [InlineData("indigo")]
    public void Calendar_EventWithColor_RendersWithoutError(string color)
    {
        var act = () => RenderComponent<MsCalendar>(p => p
            .Add(c => c.Events, [MakeEvent("Event", color: color)])
            .Add(c => c.DefaultView, CalendarViewMode.Month));

        act.Should().NotThrow();
    }
}
