using Microsoft.Extensions.DependencyInjection;
using MyStackBlazor.Services;

namespace MyStackBlazor;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers MyStackBlazor services. Call this in Program.cs:
    ///   builder.Services.AddMyStackBlazor();
    ///
    /// Add to your index.html / App.razor / MainPage.xaml (MAUI):
    ///   &lt;link rel="stylesheet" href="_content/MyStackBlazor/css/mystackblazor.css" /&gt;
    ///   &lt;script src="_content/MyStackBlazor/js/mystackblazor.js"&gt;&lt;/script&gt;
    ///
    /// Wrap your root layout in &lt;ThemeProvider&gt; to enable light/dark theming.
    /// </summary>
    public static IServiceCollection AddMyStackBlazor(this IServiceCollection services)
    {
        services.AddSingleton<ThemeService>();
        services.AddScoped<ToastService>();
        services.AddScoped<DialogService>();
        services.AddScoped<MsStyleLoader>();
        return services;
    }
}
