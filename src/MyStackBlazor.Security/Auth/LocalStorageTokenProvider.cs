using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MyStackBlazor.Security.Auth;

/// <summary>
/// Browser localStorage-backed token provider for Blazor WASM.
/// Falls back gracefully on MAUI (JS interop unavailable at startup).
/// </summary>
public sealed class LocalStorageTokenProvider : ITokenProvider, IAsyncDisposable
{
    private const string StorageKey = "ms_access_token";

    private readonly IJSRuntime _js;
    private string? _cachedToken;

    public LocalStorageTokenProvider(IJSRuntime js) => _js = js;

    public async ValueTask<string?> GetAccessTokenAsync(CancellationToken ct = default)
    {
        if (_cachedToken is not null) return _cachedToken;

        try
        {
            _cachedToken = await _js.InvokeAsync<string?>("localStorage.getItem", ct, StorageKey);
        }
        catch (JSException) { }

        return _cachedToken;
    }

    public async ValueTask SetTokenAsync(string token, CancellationToken ct = default)
    {
        _cachedToken = token;
        try
        {
            await _js.InvokeVoidAsync("localStorage.setItem", ct, StorageKey, token);
        }
        catch (JSException) { }
    }

    public async ValueTask ClearAsync(CancellationToken ct = default)
    {
        _cachedToken = null;
        try
        {
            await _js.InvokeVoidAsync("localStorage.removeItem", ct, StorageKey);
        }
        catch (JSException) { }
    }

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}
