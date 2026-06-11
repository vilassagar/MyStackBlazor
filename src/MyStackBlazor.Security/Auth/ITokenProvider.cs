namespace MyStackBlazor.Security.Auth;

/// <summary>
/// Abstraction over token storage. Swap implementations for web (localStorage/sessionStorage),
/// MAUI (SecureStorage), or server (encrypted cookie / in-memory).
/// </summary>
public interface ITokenProvider
{
    ValueTask<string?> GetAccessTokenAsync(CancellationToken ct = default);
    ValueTask SetTokenAsync(string token, CancellationToken ct = default);
    ValueTask ClearAsync(CancellationToken ct = default);
}
