namespace MyStackBlazor;

public class ToastService
{
    private readonly List<ToastMessage> _toasts = [];

    public IReadOnlyList<ToastMessage> Toasts => _toasts;

    public event Action? OnChanged;

    public void Show(string message, string? title = null, string variant = "default", int duration = 5000)
    {
        var toast = new ToastMessage(Guid.NewGuid().ToString("N"), message, title, variant, duration);
        _toasts.Add(toast);
        OnChanged?.Invoke();

        if (duration > 0)
        {
            _ = Task.Delay(duration).ContinueWith(_ =>
            {
                Remove(toast.Id);
            });
        }
    }

    public void Remove(string id)
    {
        var toast = _toasts.FirstOrDefault(t => t.Id == id);
        if (toast is not null)
        {
            _toasts.Remove(toast);
            OnChanged?.Invoke();
        }
    }

    public void Success(string message, string? title = null) => Show(message, title, "success");
    public void Error(string message, string? title = null) => Show(message, title, "destructive");
    public void Warning(string message, string? title = null) => Show(message, title, "warning");
    public void Info(string message, string? title = null) => Show(message, title, "default");
}

public record ToastMessage(string Id, string Message, string? Title, string Variant, int Duration);
