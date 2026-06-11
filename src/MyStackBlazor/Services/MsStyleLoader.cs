using Microsoft.JSInterop;

namespace MyStackBlazor.Services;

/// <summary>
/// Lazy-loads per-feature CSS bundles at runtime.
/// Ensures a stylesheet is only injected once regardless of how many components request it.
/// </summary>
public sealed class MsStyleLoader
{
    private readonly HashSet<string> _loaded = [];
    private readonly IJSRuntime _js;

    public MsStyleLoader(IJSRuntime js) => _js = js;

    /// <summary>
    /// Injects the stylesheet for the given feature name if not already present.
    /// Expects files at: _content/MyStackBlazor/css/{featureName}.min.css
    /// </summary>
    public async ValueTask EnsureLoadedAsync(string featureName)
    {
        if (!_loaded.Add(featureName)) return;

        try
        {
            await _js.InvokeVoidAsync(
                "msBlazor.loadStylesheet",
                $"_content/MyStackBlazor/css/{featureName}.min.css");
        }
        catch (JSException)
        {
            _loaded.Remove(featureName);
        }
    }
}
