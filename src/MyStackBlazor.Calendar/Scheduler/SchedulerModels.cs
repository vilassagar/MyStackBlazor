namespace MyStackBlazor.Calendar;

/// <summary>The views a <see cref="StackScheduler{TItem}"/> can show.</summary>
public enum SchedulerView
{
    Day,
    Week,
    MultiDay,
    Month,
    Timeline,
}

/// <summary>How resource groups are laid out.</summary>
public enum SchedulerGroupOrientation
{
    /// <summary>Groups side by side (columns in day/week views, rows in the timeline).</summary>
    Horizontal,
    /// <summary>Reserved for parity with Telerik; currently rendered like Horizontal.</summary>
    Vertical,
}

/// <summary>Arguments for <c>OnCreate</c>. Add <see cref="Item"/> to your data.</summary>
public class SchedulerCreateEventArgs
{
    public required object Item { get; init; }
}

/// <summary>Arguments for <c>OnUpdate</c>. Replace the item with the same id in your data.</summary>
public class SchedulerUpdateEventArgs
{
    public required object Item { get; init; }
}

/// <summary>Arguments for <c>OnDelete</c>. Remove the item from your data.</summary>
public class SchedulerDeleteEventArgs
{
    public required object Item { get; init; }
}

/// <summary>Arguments for <c>OnEdit</c>, fired before the edit form opens. Set <see cref="IsCancelled"/> to prevent it.</summary>
public class SchedulerEditEventArgs
{
    /// <summary>The edited item; null when creating.</summary>
    public object? Item { get; init; }
    public bool IsNew { get; init; }
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public bool IsAllDay { get; init; }
    /// <summary>Resource values of the slot the user double-clicked (when creating in a grouped view).</summary>
    public Dictionary<string, object?> Resources { get; init; } = [];
    public bool IsCancelled { get; set; }
}

/// <summary>Arguments for <c>OnCancel</c>, fired when the edit form is closed without saving.</summary>
public class SchedulerCancelEventArgs
{
    public object? Item { get; init; }
    public bool IsNew { get; init; }
}

/// <summary>Arguments for <c>OnItemClick</c>, <c>OnItemDoubleClick</c> and <c>OnItemContextMenu</c>.</summary>
public class SchedulerItemClickEventArgs
{
    public required object Item { get; init; }
    public EventArgs? EventArgs { get; init; }
    /// <summary>Set to true to re-render the scheduler after the handler.</summary>
    public bool ShouldRender { get; set; }
}

/// <summary>Arguments for <c>OnItemRender</c>. Set <see cref="Class"/> to style the item.</summary>
public class SchedulerItemRenderEventArgs
{
    public required object Item { get; init; }
    public string? Class { get; set; }
}

/// <summary>Arguments for <c>OnCellRender</c>. Set <see cref="Class"/> to style the slot.</summary>
public class SchedulerCellRenderEventArgs
{
    public DateTime Start { get; init; }
    public DateTime End { get; init; }
    public bool IsAllDay { get; init; }
    public SchedulerView View { get; init; }
    /// <summary>Value of the grouping resource for this slot, if the view is grouped.</summary>
    public object? Resource { get; init; }
    public string? Class { get; set; }
}

/// <summary>Recurrence frequency (the RRULE <c>FREQ</c> part).</summary>
public enum RecurrenceFrequency
{
    Daily,
    Weekly,
    Monthly,
    Yearly,
}
