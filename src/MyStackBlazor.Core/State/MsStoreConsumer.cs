using Microsoft.AspNetCore.Components;

namespace MyStackBlazor.Core.State;

/// <summary>
/// Base class for components that consume an MsStore.
/// Subscribes to StateChanged on init and triggers re-render automatically.
/// </summary>
public abstract class MsStoreConsumer<TStore> : ComponentBase, IDisposable
    where TStore : class
{
    [Inject] protected TStore Store { get; set; } = default!;

    protected override void OnInitialized()
    {
        if (Store is IDisposable disposableStore)
        {
            // Subscribe to state changes via reflection on the event
        }

        // Subscribe through a dynamic cast to the base MsStore pattern
        SubscribeToStore();
    }

    private void SubscribeToStore()
    {
        var eventInfo = typeof(TStore).GetEvent("StateChanged");
        if (eventInfo is null) return;

        var handler = new Action(OnStateChanged);
        eventInfo.AddEventHandler(Store, handler);
    }

    private void UnsubscribeFromStore()
    {
        var eventInfo = typeof(TStore).GetEvent("StateChanged");
        if (eventInfo is null) return;

        var handler = new Action(OnStateChanged);
        eventInfo.RemoveEventHandler(Store, handler);
    }

    private void OnStateChanged() => InvokeAsync(StateHasChanged);

    public void Dispose() => UnsubscribeFromStore();
}
