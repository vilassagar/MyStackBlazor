using Microsoft.JSInterop;

namespace MyStackBlazor;

/// <summary>Tailwind screen sizes: Xs &lt; 640px ≤ Sm &lt; 768 ≤ Md &lt; 1024 ≤ Lg &lt; 1280 ≤ Xl &lt; 1536 ≤ Xxl.</summary>
public enum Breakpoint { Xs, Sm, Md, Lg, Xl, Xxl }

/// <summary>
/// Current screen breakpoint, with change notifications (MudBreakpointProvider equivalent).
/// Registered by <c>AddMyStackBlazor()</c>. One browser listener is shared by all subscribers.
/// </summary>
public sealed class BreakpointService : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;
    private DotNetObjectReference<BreakpointService>? _ref;
    private Task? _starting;
    private int _watchId;

    public BreakpointService(IJSRuntime js) => _js = js;

    /// <summary>Last known breakpoint. <see cref="Breakpoint.Lg"/> until the browser has reported (e.g. during prerendering).</summary>
    public Breakpoint Current { get; private set; } = Breakpoint.Lg;

    /// <summary>True once the real breakpoint is known.</summary>
    public bool IsKnown { get; private set; }

    /// <summary>Raised when the window crosses a breakpoint.</summary>
    public event Action<Breakpoint>? Changed;

    /// <summary>Starts listening (once) and returns the current breakpoint.</summary>
    public async Task<Breakpoint> GetAsync()
    {
        await (_starting ??= StartAsync());
        return Current;
    }

    private async Task StartAsync()
    {
        try
        {
            _module = await _js.InvokeAsync<IJSObjectReference>("import", "./_content/MyStackBlazor/js/interactions.js");
            _ref = DotNetObjectReference.Create(this);
            var r = await _module.InvokeAsync<WatchResult>("watchBreakpoint", _ref);
            _watchId = r.Id;
            Current = Parse(r.Breakpoint);
            IsKnown = true;
        }
        catch (JSDisconnectedException) { }
        catch (InvalidOperationException) { _starting = null; } // prerendering: try again later
    }

    [JSInvokable]
    public void OnBreakpointChanged(string breakpoint)
    {
        var bp = Parse(breakpoint);
        if (bp == Current && IsKnown) return;
        Current = bp;
        IsKnown = true;
        Changed?.Invoke(bp);
    }

    internal static Breakpoint Parse(string? s) => s switch
    {
        "xs"  => Breakpoint.Xs,
        "sm"  => Breakpoint.Sm,
        "md"  => Breakpoint.Md,
        "xl"  => Breakpoint.Xl,
        "2xl" => Breakpoint.Xxl,
        _     => Breakpoint.Lg,
    };

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (_module is not null)
            {
                if (_watchId != 0) await _module.InvokeVoidAsync("dispose", _watchId);
                await _module.DisposeAsync();
            }
        }
        catch (JSDisconnectedException) { }
        _ref?.Dispose();
    }

    internal sealed record WatchResult(int Id, string Breakpoint);
}
