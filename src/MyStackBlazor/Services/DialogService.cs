namespace MyStackBlazor;

public class DialogService
{
    public ConfirmRequest? Current { get; private set; }

    public event Action? OnChanged;

    public Task<bool> ConfirmAsync(string title, string message, string confirmText = "Continue", string cancelText = "Cancel")
    {
        var tcs = new TaskCompletionSource<bool>();
        Current = new ConfirmRequest(title, message, confirmText, cancelText, tcs);
        OnChanged?.Invoke();
        return tcs.Task;
    }

    internal void Resolve(bool result)
    {
        var request = Current;
        if (request is null)
        {
            return;
        }

        Current = null;
        request.Tcs.TrySetResult(result);
        OnChanged?.Invoke();
    }
}

public record ConfirmRequest(string Title, string Message, string ConfirmText, string CancelText, TaskCompletionSource<bool> Tcs);
