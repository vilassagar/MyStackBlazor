namespace MyStackBlazor.Security.Auth;

/// <summary>
/// In-memory token provider for SSR / Blazor Server where localStorage isn't available.
/// Scoped lifetime — token is per-circuit, not persisted across page reloads.
/// </summary>
public sealed class InMemoryTokenProvider : ITokenProvider
{
    private string? _token;

    public ValueTask<string?> GetAccessTokenAsync(CancellationToken ct = default)
        => new(_token);

    public ValueTask SetTokenAsync(string token, CancellationToken ct = default)
    {
        _token = token;
        return ValueTask.CompletedTask;
    }

    public ValueTask ClearAsync(CancellationToken ct = default)
    {
        _token = null;
        return ValueTask.CompletedTask;
    }
}
