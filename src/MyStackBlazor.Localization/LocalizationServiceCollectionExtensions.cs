using Microsoft.Extensions.DependencyInjection;

namespace MyStackBlazor.Localization;

public static class LocalizationServiceCollectionExtensions
{
    /// <summary>
    /// Registers .NET localization services used by MyStackBlazor localization components.
    /// Call in Program.cs: builder.Services.AddMyStackBlazorLocalization()
    ///
    /// To configure supported cultures and request localization middleware, use the host app's
    /// app.UseRequestLocalization(opts => { ... }) instead.
    /// </summary>
    public static IServiceCollection AddMyStackBlazorLocalization(this IServiceCollection services)
    {
        services.AddLocalization();
        return services;
    }
}
