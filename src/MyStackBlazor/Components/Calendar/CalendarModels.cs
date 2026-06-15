namespace MyStackBlazor.Components.Calendar;

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

    public bool IsMultiDay => Start.Date != End.Date;
}

public enum CalendarViewMode { Month, Week, Day }
