using System.Globalization;
using MyStackBlazor.Calendar;

namespace MyStackBlazor.UnitTests.Components;

public class SchedulerRecurrenceRuleTests
{
    private static readonly DateTime Mon = new(2026, 5, 4, 9, 0, 0); // a Monday

    [Fact]
    public void Parse_And_ToString_RoundTrip()
    {
        var rule = SchedulerRecurrenceRule.Parse("FREQ=WEEKLY;INTERVAL=2;BYDAY=MO,WE;COUNT=5")!;
        rule.Frequency.Should().Be(RecurrenceFrequency.Weekly);
        rule.Interval.Should().Be(2);
        rule.ByDay.Should().Equal(DayOfWeek.Monday, DayOfWeek.Wednesday);
        rule.Count.Should().Be(5);
        rule.ToString().Should().Be("FREQ=WEEKLY;INTERVAL=2;BYDAY=MO,WE;COUNT=5");
    }

    [Fact]
    public void Parse_Returns_Null_For_Empty_Or_Invalid() =>
        new[] { null, "", "INTERVAL=2", "FREQ=HOURLY" }.Select(SchedulerRecurrenceRule.Parse).Should().AllSatisfy(r => r.Should().BeNull());

    [Fact]
    public void Weekly_ByDay_With_Count()
    {
        var rule = SchedulerRecurrenceRule.Parse("FREQ=WEEKLY;BYDAY=MO,WE;COUNT=3")!;
        rule.GetOccurrences(Mon, TimeSpan.FromHours(1), Mon.AddYears(-1), Mon.AddYears(1))
            .Should().Equal(Mon, Mon.AddDays(2), Mon.AddDays(7));
    }

    [Fact]
    public void Daily_Until_And_Range_Filtering()
    {
        var rule = SchedulerRecurrenceRule.Parse("FREQ=DAILY;INTERVAL=2;UNTIL=20260512T235959")!;
        rule.GetOccurrences(Mon, TimeSpan.FromHours(1), Mon.AddDays(3), Mon.AddDays(30))
            .Should().Equal(Mon.AddDays(4), Mon.AddDays(6), Mon.AddDays(8));
    }

    [Fact]
    public void Monthly_Skips_Short_Months()
    {
        var start = new DateTime(2026, 1, 31, 10, 0, 0);
        var rule = SchedulerRecurrenceRule.Parse("FREQ=MONTHLY;COUNT=3")!;
        rule.GetOccurrences(start, TimeSpan.Zero, start, start.AddYears(1))
            .Should().Equal(start, new DateTime(2026, 3, 31, 10, 0, 0), new DateTime(2026, 5, 31, 10, 0, 0));
    }
}

public class SchedulerTests : TestContext
{
    public class Appointment
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string? Description { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool IsAllDay { get; set; }
        public string? RecurrenceRule { get; set; }
        public List<DateTime>? RecurrenceExceptions { get; set; }
        public int? RecurrenceId { get; set; }
        public int? RoomId { get; set; }
    }

    public record Room(int Value, string Text, string Color);

    private static readonly DateTime Day = new(2026, 5, 6); // Wednesday
    private static readonly List<Room> Rooms = [new(1, "Red room", "#ef4444"), new(2, "Blue room", "#3b82f6")];

    public SchedulerTests()
    {
        JSInterop.Mode = JSRuntimeMode.Loose;
        CultureInfo.CurrentCulture = CultureInfo.CurrentUICulture = new CultureInfo("en-US");
    }

    private static Appointment Appt(int id, string title, DateTime start, double hours = 1, string? rule = null, int? room = null) =>
        new() { Id = id, Title = title, Start = start, End = start.AddHours(hours), RecurrenceRule = rule, RoomId = room };

    private static RenderFragment Views(params Type[] views) => b =>
    {
        foreach (var view in views)
        {
            b.OpenComponent(0, view);
            b.CloseComponent();
        }
    };

    private IRenderedComponent<StackScheduler<Appointment>> Render(
        IEnumerable<Appointment> data,
        SchedulerView view = SchedulerView.Day,
        Action<ComponentParameterCollectionBuilder<StackScheduler<Appointment>>>? configure = null) =>
        RenderComponent<StackScheduler<Appointment>>(p =>
        {
            p.Add(s => s.Data, data)
             .Add(s => s.Date, Day)
             .Add(s => s.View, view)
             .Add(s => s.SchedulerViews, Views(typeof(StackSchedulerDayView), typeof(StackSchedulerWeekView),
                                               typeof(StackSchedulerMonthView), typeof(StackSchedulerTimelineView)));
            configure?.Invoke(p);
        });

    private static IEnumerable<string> ItemTitles(IRenderedFragment cut) =>
        cut.FindAll("[data-item]").Select(i => i.GetAttribute("data-item")!);

    [Fact]
    public void Day_View_Shows_Items_Of_The_Day()
    {
        var cut = Render([Appt(1, "Standup", Day.AddHours(9)), Appt(2, "Tomorrow", Day.AddDays(1).AddHours(9))]);
        ItemTitles(cut).Should().Equal("Standup");
        cut.Markup.Should().Contain("Wednesday, May 6, 2026");
    }

    [Fact]
    public void Week_View_Has_Seven_Days_And_Week_Items()
    {
        var cut = Render([Appt(1, "Mon", new DateTime(2026, 5, 4, 10, 0, 0)), Appt(2, "Next week", Day.AddDays(7))], SchedulerView.Week);
        cut.FindAll("[data-slot^=allday]").Should().HaveCount(7);
        ItemTitles(cut).Should().Equal("Mon");
    }

    [Fact]
    public void All_Day_Items_Go_To_All_Day_Row()
    {
        var allDay = new Appointment { Id = 1, Title = "Holiday", Start = Day, End = Day.AddDays(1), IsAllDay = true };
        var cut = Render([allDay]);
        cut.Find($"[data-slot='allday-{Day:yyyy-MM-dd}'] [data-item]").GetAttribute("data-item").Should().Be("Holiday");
    }

    [Fact]
    public void Month_View_Limits_Items_Per_Slot()
    {
        var data = Enumerable.Range(1, 4).Select(i => Appt(i, $"Item {i}", Day.AddHours(8 + i))).ToList();
        var cut = Render(data, SchedulerView.Month);
        cut.FindAll($"[data-slot='day-{Day:yyyy-MM-dd}'] [data-item]").Should().HaveCount(2);
        cut.Markup.Should().Contain("+2 more");
    }

    [Fact]
    public void Recurring_Items_Expand_And_Skip_Exceptions()
    {
        var series = Appt(1, "Daily", new DateTime(2026, 5, 4, 9, 0, 0), rule: "FREQ=DAILY");
        series.RecurrenceExceptions = [new DateTime(2026, 5, 6, 9, 0, 0)];
        var cut = Render([series], SchedulerView.Week);
        ItemTitles(cut).Should().HaveCount(5, "Mon–Sat of the Sun-based week, minus the Wednesday exception");
        cut.FindAll("[aria-label*=recurring]").Should().HaveCount(5);
        cut.FindAll($"[data-slot='allday-{Day:yyyy-MM-dd}']").Should().HaveCount(1);
    }

    [Fact]
    public void Navigation_And_View_Buttons_Raise_Events()
    {
        DateTime? date = null;
        SchedulerView? view = null;
        var cut = Render([], SchedulerView.Week, p => p
            .Add(s => s.DateChanged, d => date = d)
            .Add(s => s.ViewChanged, v => view = v));

        cut.Find("button[aria-label=Next]").Click();
        date.Should().Be(Day.AddDays(7));

        cut.FindAll("[role=group][aria-label=Views] button").First(b => b.TextContent.Trim() == "Month").Click();
        view.Should().Be(SchedulerView.Month);
    }

    [Fact]
    public void ShowWorkHours_Limits_Visible_Slots()
    {
        var cut = Render([], configure: p => p.Add(s => s.ShowWorkHours, true));
        var slots = cut.FindAll("[data-slot^='2026-05-06T']").Select(s => s.GetAttribute("data-slot")).ToList();
        slots.First().Should().Be("2026-05-06T08:00");
        slots.Last().Should().Be("2026-05-06T16:30");
    }

    [Fact]
    public void DoubleClick_Slot_Creates_Item()
    {
        Appointment? created = null;
        var cut = Render([], configure: p => p
            .Add(s => s.AllowCreate, true)
            .Add(s => s.OnCreate, a => created = (Appointment)a.Item));

        cut.Find("[data-slot='2026-05-06T10:00']").DoubleClick();
        cut.Find("[role=dialog] input[type=text]").Input("Review");
        cut.FindAll("[role=dialog] button").First(b => b.TextContent.Trim() == "Save").Click();

        created!.Title.Should().Be("Review");
        created.Start.Should().Be(Day.AddHours(10));
        created.End.Should().Be(Day.AddHours(11));
    }

    [Fact]
    public void Save_Without_Title_Shows_Error()
    {
        var cut = Render([], configure: p => p.Add(s => s.AllowCreate, true));
        cut.Find("[data-slot='2026-05-06T10:00']").DoubleClick();
        cut.FindAll("[role=dialog] button").First(b => b.TextContent.Trim() == "Save").Click();
        cut.Find("[role=alert]").TextContent.Should().Contain("Title is required");
    }

    [Fact]
    public void OnEdit_Can_Cancel_Opening_The_Form()
    {
        var cut = Render([], configure: p => p
            .Add(s => s.AllowCreate, true)
            .Add(s => s.OnEdit, (SchedulerEditEventArgs a) => a.IsCancelled = true));
        cut.Find("[data-slot='2026-05-06T10:00']").DoubleClick();
        cut.FindAll("[role=dialog]").Should().BeEmpty();
    }

    [Fact]
    public void DoubleClick_Item_Edits_And_Updates()
    {
        Appointment? updated = null;
        var original = Appt(1, "Old", Day.AddHours(9));
        var cut = Render([original], configure: p => p
            .Add(s => s.AllowUpdate, true)
            .Add(s => s.OnUpdate, a => updated = (Appointment)a.Item));

        cut.Find("[data-item=Old]").DoubleClick();
        cut.Find("[role=dialog] input[type=text]").Input("New");
        cut.FindAll("[role=dialog] button").First(b => b.TextContent.Trim() == "Save").Click();

        updated!.Title.Should().Be("New");
        updated.Id.Should().Be(1);
        original.Title.Should().Be("Old", "the scheduler edits a copy");
    }

    [Fact]
    public void Edit_Form_Builds_Recurrence_Rule()
    {
        Appointment? created = null;
        var cut = Render([], configure: p => p
            .Add(s => s.AllowCreate, true)
            .Add(s => s.OnCreate, a => created = (Appointment)a.Item));

        cut.Find("[data-slot='2026-05-06T10:00']").DoubleClick();
        cut.Find("[role=dialog] input[type=text]").Input("Gym");
        cut.Find("[role=dialog] select").Change("Weekly");
        cut.FindAll("[role=dialog] input[type=radio]")[1].Change(true);
        cut.FindAll("[role=dialog] button").First(b => b.TextContent.Trim() == "Save").Click();

        created!.RecurrenceRule.Should().Be("FREQ=WEEKLY;BYDAY=WE;COUNT=10");
    }

    [Fact]
    public void Delete_Button_Deletes_And_ConfirmDelete_Prompts()
    {
        Appointment? deleted = null;
        var cut = Render([Appt(1, "Gone", Day.AddHours(9))], configure: p => p
            .Add(s => s.AllowDelete, true)
            .Add(s => s.OnDelete, a => deleted = (Appointment)a.Item));
        cut.Find("button[aria-label='Delete Gone']").Click();
        deleted!.Id.Should().Be(1);

        deleted = null;
        var confirm = Render([Appt(2, "Ask", Day.AddHours(9))], configure: p => p
            .Add(s => s.AllowDelete, true)
            .Add(s => s.ConfirmDelete, true)
            .Add(s => s.OnDelete, a => deleted = (Appointment)a.Item));
        confirm.Find("button[aria-label='Delete Ask']").Click();
        deleted.Should().BeNull();
        confirm.FindAll("[role=alertdialog] button").First(b => b.TextContent.Trim() == "Delete").Click();
        deleted!.Id.Should().Be(2);
    }

    [Fact]
    public void Deleting_An_Occurrence_Adds_An_Exception()
    {
        Appointment? updated = null;
        var series = Appt(1, "Daily", new DateTime(2026, 5, 1, 9, 0, 0), rule: "FREQ=DAILY");
        var cut = Render([series], configure: p => p
            .Add(s => s.AllowDelete, true)
            .Add(s => s.OnUpdate, a => updated = (Appointment)a.Item));

        cut.Find("button[aria-label='Delete Daily']").Click();
        cut.FindAll("[role=alertdialog] button").First(b => b.TextContent.Trim() == "Delete this occurrence").Click();

        updated!.Id.Should().Be(1);
        updated.RecurrenceExceptions.Should().Equal(Day.AddHours(9));
    }

    [Fact]
    public void Editing_An_Occurrence_Creates_Exception_Item()
    {
        Appointment? created = null, updated = null;
        var series = Appt(7, "Daily", new DateTime(2026, 5, 1, 9, 0, 0), rule: "FREQ=DAILY");
        var cut = Render([series], configure: p => p
            .Add(s => s.AllowUpdate, true)
            .Add(s => s.OnCreate, a => created = (Appointment)a.Item)
            .Add(s => s.OnUpdate, a => updated = (Appointment)a.Item));

        cut.Find("[data-item=Daily]").DoubleClick();
        cut.FindAll("[role=alertdialog] button").First(b => b.TextContent.Trim() == "Edit this occurrence").Click();
        cut.Find("[role=dialog] input[type=text]").Input("Moved");
        cut.FindAll("[role=dialog] button").First(b => b.TextContent.Trim() == "Save").Click();

        created!.Title.Should().Be("Moved");
        created.RecurrenceId.Should().Be(7);
        created.RecurrenceRule.Should().BeNull();
        created.Start.Should().Be(Day.AddHours(9));
        updated!.RecurrenceExceptions.Should().Equal(Day.AddHours(9));
    }

    [Fact]
    public void Drag_And_Drop_Moves_Item()
    {
        Appointment? updated = null;
        var cut = Render([Appt(1, "Move me", Day.AddHours(9), hours: 2)], configure: p => p
            .Add(s => s.AllowUpdate, true)
            .Add(s => s.OnUpdate, a => updated = (Appointment)a.Item));

        cut.Find("[data-item='Move me']").DragStart();
        cut.Find("[data-slot='2026-05-06T14:00']").Drop();

        updated!.Start.Should().Be(Day.AddHours(14));
        updated.End.Should().Be(Day.AddHours(16));
    }

    [Fact]
    public void Resources_Color_Items_And_Group_Columns()
    {
        RenderFragment resources = b =>
        {
            b.OpenComponent<StackSchedulerResource>(0);
            b.AddAttribute(1, nameof(StackSchedulerResource.Field), nameof(Appointment.RoomId));
            b.AddAttribute(2, nameof(StackSchedulerResource.Title), "Room");
            b.AddAttribute(3, nameof(StackSchedulerResource.Data), Rooms);
            b.CloseComponent();
        };
        RenderFragment settings = b =>
        {
            b.OpenComponent<StackSchedulerGroupSettings>(0);
            b.AddAttribute(1, nameof(StackSchedulerGroupSettings.Resources), new List<string> { nameof(Appointment.RoomId) });
            b.CloseComponent();
        };

        var cut = Render([Appt(1, "In red", Day.AddHours(9), room: 1)], configure: p => p
            .Add(s => s.SchedulerResources, resources)
            .Add(s => s.SchedulerSettings, settings));

        cut.Markup.Should().Contain("Red room").And.Contain("Blue room");
        cut.Find("[data-item='In red']").GetAttribute("style").Should().Contain("background-color:#ef4444");
        cut.FindAll("[data-slot='2026-05-06T09:00']").Should().HaveCount(2, "one column per room");
    }

    [Fact]
    public void Timeline_View_Positions_Items()
    {
        var cut = Render([Appt(1, "Line", Day.AddHours(2))], SchedulerView.Timeline);
        cut.Find("[data-item=Line]").GetAttribute("style").Should().Contain("left:202px");
    }

    [Fact]
    public void ItemTemplate_OnItemRender_And_OnCellRender()
    {
        var cut = Render([Appt(1, "Tpl", Day.AddHours(9))], configure: p => p
            .Add(s => s.ItemTemplate, item => $"<b class='tpl'>{item.Title}!</b>")
            .Add(s => s.OnItemRender, (SchedulerItemRenderEventArgs a) => a.Class = "custom-item")
            .Add(s => s.OnCellRender, (SchedulerCellRenderEventArgs a) => { if (a.Start.Hour == 12) a.Class = "lunch"; }));

        cut.Find(".tpl").TextContent.Should().Be("Tpl!");
        cut.Find("[data-item=Tpl]").ClassList.Should().Contain("custom-item");
        cut.Find("[data-slot='2026-05-06T12:00']").ClassList.Should().Contain("lunch");
    }

    [Fact]
    public void OnItemClick_Receives_Item()
    {
        object? clicked = null;
        var cut = Render([Appt(1, "Click", Day.AddHours(9))], configure: p => p
            .Add(s => s.OnItemClick, a => clicked = a.Item));
        cut.Find("[data-item=Click]").Click();
        ((Appointment)clicked!).Id.Should().Be(1);
    }
}
