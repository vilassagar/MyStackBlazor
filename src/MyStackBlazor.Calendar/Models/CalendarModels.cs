namespace MyStackBlazor.Calendar.Models;

public class CalendarEvent
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public bool AllDay { get; set; }
    public string Color { get; set; } = "blue";
    public string? Category { get; set; }
    public string? Location { get; set; }
    public string? PersonId { get; set; }   // null = "my calendar"

    // Recurrence
    public RecurrenceType RecurrenceType { get; set; } = RecurrenceType.None;
    public int RecurrenceInterval { get; set; } = 1;
    public DateTime? RecurrenceEndDate { get; set; }
    public int? RecurrenceCount { get; set; }

    // Enterprise fields
    public EventStatus Status { get; set; } = EventStatus.Confirmed;
    public EventPriority Priority { get; set; } = EventPriority.Medium;
    public List<string> Tags { get; set; } = [];
    public List<string> Attendees { get; set; } = [];
    public string? Url { get; set; }
    public bool IsPrivate { get; set; }

    public bool IsMultiDay => Start.Date != End.Date;
    public bool IsRecurring => RecurrenceType != RecurrenceType.None;
}

public class CalendarPerson
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = "";
    public string Color { get; set; } = "blue";
    public bool IsVisible { get; set; } = true;
}

/// <summary>
/// Calendar view modes. WorkWeek shows Monday–Friday only in the time grid.
/// </summary>
public enum CalendarViewMode
{
    Month,
    Week,
    WorkWeek,
    Day,
    Year,
    Agenda,
    Timeline,
}

public enum RecurrenceType { None, Daily, Weekly, Monthly, Yearly }

public enum EventStatus { Draft, Scheduled, Confirmed, InProgress, Completed, Cancelled }

public enum EventPriority { Low, Medium, High, Critical }

public enum FocusMode { Individual, Department }
