using Microsoft.Extensions.DependencyInjection;
using MyStackBlazor.Accessibility.Services;

namespace MyStackBlazor.Accessibility;

public static class AccessibilityServiceCollectionExtensions
{
    /// <summary>
    /// Registers MyStackBlazor accessibility services.
    /// Call in Program.cs: builder.Services.AddMyStackBlazorAccessibility()
    /// </summary>
    public static IServiceCollection AddMyStackBlazorAccessibility(this IServiceCollection services)
    {
        services.AddScoped<IFocusManager, FocusManager>();
        return services;
    }
}
