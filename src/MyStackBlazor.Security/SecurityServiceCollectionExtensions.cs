using Microsoft.Extensions.DependencyInjection;
using MyStackBlazor.Security.Auth;

namespace MyStackBlazor.Security;

public static class SecurityServiceCollectionExtensions
{
    /// <summary>
    /// Registers MyStackBlazor security services.
    /// Call in Program.cs: builder.Services.AddMyStackBlazorSecurity()
    ///
    /// Token storage defaults to in-memory (Blazor Server safe).
    /// For WASM, pass useLocalStorage: true.
    /// For MAUI, register your own ITokenProvider using SecureStorage.
    /// </summary>
    public static IServiceCollection AddMyStackBlazorSecurity(
        this IServiceCollection services,
        Action<SecurityOptions>? configure = null,
        bool useLocalStorage = false)
    {
        var options = new SecurityOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<IHtmlSanitizer>(sp =>
            new DefaultHtmlSanitizer(sp.GetRequiredService<SecurityOptions>()));

        if (useLocalStorage)
            services.AddScoped<ITokenProvider, LocalStorageTokenProvider>();
        else
            services.AddScoped<ITokenProvider, InMemoryTokenProvider>();

        return services;
    }
}
