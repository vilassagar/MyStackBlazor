using Microsoft.AspNetCore.Components;

namespace MyStackBlazor;

public class DialogService
{
    public ConfirmRequest? Current { get; private set; }

    /// <summary>The message box currently shown by <c>StackConfirmDialogHost</c>, if any.</summary>
    public MessageBoxRequest? CurrentMessageBox { get; private set; }

    public event Action? OnChanged;

    public Task<bool> ConfirmAsync(string title, string message, string confirmText = "Continue", string cancelText = "Cancel")
    {
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        Current = new ConfirmRequest(title, message, confirmText, cancelText, tcs);
        OnChanged?.Invoke();
        return tcs.Task;
    }

    /// <summary>
    /// Shows a message box with up to three buttons. Completes with <c>true</c> (yes), <c>false</c> (no)
    /// or <c>null</c> (cancel / Escape). Requires <c>&lt;StackConfirmDialogHost /&gt;</c> in the layout.
    /// </summary>
    public Task<bool?> ShowMessageBoxAsync(MessageBoxOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);
        // A newer box replaces an unanswered one; the old caller gets "cancel".
        CurrentMessageBox?.Tcs.TrySetResult(null);
        var tcs = new TaskCompletionSource<bool?>(TaskCreationOptions.RunContinuationsAsynchronously);
        CurrentMessageBox = new MessageBoxRequest(options, tcs);
        OnChanged?.Invoke();
        return tcs.Task;
    }

    /// <inheritdoc cref="ShowMessageBoxAsync(MessageBoxOptions)"/>
    public Task<bool?> ShowMessageBoxAsync(string title, string message, string yesText = "OK", string? noText = null, string? cancelText = null) =>
        ShowMessageBoxAsync(new MessageBoxOptions
        {
            Title = title,
            Message = message,
            YesText = yesText,
            NoText = noText,
            CancelText = cancelText,
        });

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

    internal void ResolveMessageBox(bool? result)
    {
        var request = CurrentMessageBox;
        if (request is null) return;
        CurrentMessageBox = null;
        request.Tcs.TrySetResult(result);
        OnChanged?.Invoke();
    }
}

public record ConfirmRequest(string Title, string Message, string ConfirmText, string CancelText, TaskCompletionSource<bool> Tcs);

public record MessageBoxRequest(MessageBoxOptions Options, TaskCompletionSource<bool?> Tcs);

/// <summary>Options for <see cref="DialogService.ShowMessageBoxAsync(MessageBoxOptions)"/>.</summary>
public sealed class MessageBoxOptions
{
    public string? Title { get; init; }
    public string? Message { get; init; }
    /// <summary>Custom body content (e.g. a list of affected items).</summary>
    public RenderFragment? Content { get; init; }
    public string YesText { get; init; } = "OK";
    /// <summary>Second answer such as "Don't save". Null hides the button.</summary>
    public string? NoText { get; init; }
    /// <summary>Dismiss button. Null hides it (Escape still cancels).</summary>
    public string? CancelText { get; init; }
    public bool Destructive { get; init; }
    public bool CloseOnBackdropClick { get; init; }
}
