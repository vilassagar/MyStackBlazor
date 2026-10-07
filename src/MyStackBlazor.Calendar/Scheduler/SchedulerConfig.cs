using Microsoft.AspNetCore.Components;

namespace MyStackBlazor.Calendar;

/// <summary>Implemented by <see cref="StackScheduler{TItem}"/>; lets its declarative children register.</summary>
internal interface IStackSchedulerHost
{
    void Register(object child);
    void Unregister(object child);
    void ConfigChanged();
}

/// <summary>
/// Base for the non-rendering view declarations placed inside <c>&lt;SchedulerViews&gt;</c>.
/// Only the time-of-day part of the DateTime parameters is used.
/// </summary>
public abstract class SchedulerViewBase : ComponentBase, IDisposable
{
    [CascadingParameter] private IStackSchedulerHost? Host { get; set; }

    /// <summary>First visible time of day (default 00:00).</summary>
    [Parameter] public DateTime StartTime { get; set; } = DateTime.Today;
    /// <summary>Last visible time of day; midnight means end of day (default).</summary>
    [Parameter] public DateTime EndTime { get; set; } = DateTime.Today;
    /// <summary>Start of business hours, shown when <c>ShowWorkHours</c> is on (default 08:00).</summary>
    [Parameter] public DateTime WorkDayStart { get; set; } = DateTime.Today.AddHours(8);
    /// <summary>End of business hours (default 17:00).</summary>
    [Parameter] public DateTime WorkDayEnd { get; set; } = DateTime.Today.AddHours(17);
    /// <summary>Length of a labelled time slot in minutes (default 60).</summary>
    [Parameter] public int SlotDuration { get; set; } = 60;
    /// <summary>Number of rows each slot is divided into (default 2).</summary>
    [Parameter] public int SlotDivisions { get; set; } = 2;

    public abstract SchedulerView ViewType { get; }

    private object? _snapshot;

    protected virtual object Snapshot() =>
        (StartTime.TimeOfDay, EndTime.TimeOfDay, WorkDayStart.TimeOfDay, WorkDayEnd.TimeOfDay, SlotDuration, SlotDivisions);

    protected override void OnInitialized() => Host?.Register(this);

    protected override void OnParametersSet()
    {
        var snapshot = Snapshot();
        if (_snapshot is not null && !snapshot.Equals(_snapshot)) Host?.ConfigChanged();
        _snapshot = snapshot;
    }

    public void Dispose() => Host?.Unregister(this);

    internal TimeSpan StartOfDay => StartTime.TimeOfDay;
    internal TimeSpan EndOfDay => EndTime.TimeOfDay == TimeSpan.Zero ? TimeSpan.FromDays(1) : EndTime.TimeOfDay;
    internal TimeSpan WorkStart => WorkDayStart.TimeOfDay;
    internal TimeSpan WorkEnd => WorkDayEnd.TimeOfDay == TimeSpan.Zero ? TimeSpan.FromDays(1) : WorkDayEnd.TimeOfDay;
    internal int SafeSlotDuration => Math.Clamp(SlotDuration, 5, 24 * 60);
    internal int SafeSlotDivisions => Math.Clamp(SlotDivisions, 1, 12);
}

/// <summary>One day in a time grid.</summary>
public class StackSchedulerDayView : SchedulerViewBase
{
    public override SchedulerView ViewType => SchedulerView.Day;
}

/// <summary>Seven days in a time grid, starting on the culture's first day of the week.</summary>
public class StackSchedulerWeekView : SchedulerViewBase
{
    public override SchedulerView ViewType => SchedulerView.Week;
}

/// <summary>A configurable number of days in a time grid, starting on the current date.</summary>
public class StackSchedulerMultiDayView : SchedulerViewBase
{
    [Parameter] public int NumberOfDays { get; set; } = 3;

    public override SchedulerView ViewType => SchedulerView.MultiDay;
    protected override object Snapshot() => (base.Snapshot(), NumberOfDays);
}

/// <summary>A month grid with a limited number of items per day.</summary>
public class StackSchedulerMonthView : SchedulerViewBase
{
    /// <summary>Items shown per day before a "+N more" link (default 2).</summary>
    [Parameter] public int ItemsPerSlot { get; set; } = 2;

    public override SchedulerView ViewType => SchedulerView.Month;
    protected override object Snapshot() => (base.Snapshot(), ItemsPerSlot);
}

/// <summary>Time slots laid out horizontally, with one row per resource when grouped.</summary>
public class StackSchedulerTimelineView : SchedulerViewBase
{
    [Parameter] public int NumberOfDays { get; set; } = 1;
    /// <summary>Width of one time slot in pixels (default 100).</summary>
    [Parameter] public int ColumnWidth { get; set; } = 100;

    public override SchedulerView ViewType => SchedulerView.Timeline;
    protected override object Snapshot() => (base.Snapshot(), NumberOfDays, ColumnWidth);
}

/// <summary>
/// Declares a resource (placed inside <c>&lt;SchedulerResources&gt;</c>): a field of the item whose
/// values come from <see cref="Data"/>, for colouring, grouping and the edit form.
/// </summary>
public class StackSchedulerResource : ComponentBase, IDisposable
{
    [CascadingParameter] private IStackSchedulerHost? Host { get; set; }

    /// <summary>The item property that holds the resource value.</summary>
    [Parameter, EditorRequired] public string Field { get; set; } = "";
    /// <summary>Label shown in the edit form.</summary>
    [Parameter] public string? Title { get; set; }
    [Parameter] public IEnumerable<object>? Data { get; set; }
    [Parameter] public string TextField { get; set; } = "Text";
    [Parameter] public string ValueField { get; set; } = "Value";
    /// <summary>Property of the data items holding a CSS colour used to paint the scheduler items.</summary>
    [Parameter] public string ColorField { get; set; } = "Color";

    private object? _snapshot;

    protected override void OnInitialized() => Host?.Register(this);

    protected override void OnParametersSet()
    {
        object snapshot = (Field, Title, Data, TextField, ValueField, ColorField);
        if (_snapshot is not null && !snapshot.Equals(_snapshot)) Host?.ConfigChanged();
        _snapshot = snapshot;
    }

    public void Dispose() => Host?.Unregister(this);

    internal IReadOnlyList<ResourceOption> Options =>
        (Data ?? []).Select(d => new ResourceOption(
            Read(d, ValueField) ?? d,
            Read(d, TextField)?.ToString() ?? d.ToString() ?? "",
            Read(d, ColorField)?.ToString())).ToList();

    private static object? Read(object item, string property) =>
        item.GetType().GetProperty(property)?.GetValue(item);
}

/// <summary>A resource value with its display text and colour.</summary>
internal sealed record ResourceOption(object Value, string Text, string? Color);

/// <summary>Groups the views by resources (placed inside <c>&lt;SchedulerSettings&gt;</c>).</summary>
public class StackSchedulerGroupSettings : ComponentBase, IDisposable
{
    [CascadingParameter] private IStackSchedulerHost? Host { get; set; }

    /// <summary>Field names of the resources to group by. The first one is used.</summary>
    [Parameter] public List<string>? Resources { get; set; }
    [Parameter] public SchedulerGroupOrientation Orientation { get; set; } = SchedulerGroupOrientation.Horizontal;

    private string? _snapshot;

    protected override void OnInitialized() => Host?.Register(this);

    protected override void OnParametersSet()
    {
        var snapshot = $"{string.Join("|", Resources ?? [])}:{Orientation}";
        if (_snapshot is not null && snapshot != _snapshot) Host?.ConfigChanged();
        _snapshot = snapshot;
    }

    public void Dispose() => Host?.Unregister(this);
}
