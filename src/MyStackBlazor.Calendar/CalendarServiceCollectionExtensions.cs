using Microsoft.Extensions.DependencyInjection;

namespace MyStackBlazor.Calendar;

public static class CalendarServiceCollectionExtensions
{
    /// <summary>
    /// Registers MyStackBlazor.Calendar services.
    /// Call in Program.cs: builder.Services.AddMyStackBlazorCalendar()
    /// </summary>
    public static IServiceCollection AddMyStackBlazorCalendar(this IServiceCollection services)
    {
        return services;
    }
}
