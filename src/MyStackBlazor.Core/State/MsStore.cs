namespace MyStackBlazor.Core.State;

/// <summary>
/// Lightweight Fluxor-inspired store base. Inherit, call SetState() in action methods,
/// and subscribe components via StateChanged.
/// </summary>
public abstract class MsStore<TState> : IDisposable
    where TState : class, new()
{
    private TState _state = new();

    public TState State => _state;

    public event Action? StateChanged;

    protected void SetState(Func<TState, TState> updater)
    {
        _state = updater(_state);
        StateChanged?.Invoke();
    }

    public void Dispose() => StateChanged = null;
}
