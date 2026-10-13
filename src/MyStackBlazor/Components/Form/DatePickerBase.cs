using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MyStackBlazor.Components.Form;

/// <summary>Arguments for a picker's <c>OnOpen</c>. Set <see cref="IsCancelled"/> to keep the popup closed.</summary>
public class DatePickerOpenEventArgs
{
    public bool IsCancelled { get; set; }
}

/// <summary>Arguments for a picker's <c>OnClose</c>. Set <see cref="IsCancelled"/> to keep the popup open.</summary>
public class DatePickerCloseEventArgs
{
    public bool IsCancelled { get; set; }
}

/// <summary>Step sizes for the time lists (Telerik <c>TimePickerSteps</c>).</summary>
public class TimePickerSteps
{
    public int Hour { get; set; } = 1;
    public int Minute { get; set; } = 1;
    public int Second { get; set; } = 1;
}

/// <summary>
/// Parameters and behaviour shared by StackDatePicker, StackDateTimePicker and StackTimePicker:
/// a segmented StackDateInput plus a popup opened by a button, Alt+Down or a click.
/// </summary>
public abstract class DatePickerBase<T> : ComponentBase
{
    // ── Value ────────────────────────────────────────────────────────────────
    [Parameter] public T? Value { get; set; }
    [Parameter] public EventCallback<T?> ValueChanged { get; set; }
    [Parameter] public Expression<Func<T?>>? ValueExpression { get; set; }

    /// <summary>.NET date/time format of the input.</summary>
    [Parameter] public string? Format { get; set; }
    [Parameter] public DateTime Min { get; set; } = new(1900, 1, 1);
    [Parameter] public DateTime Max { get; set; } = new(2099, 12, 31);

    // ── Input behaviour (Telerik DateInput names) ────────────────────────────
    [Parameter] public string? Placeholder { get; set; }
    [Parameter] public DateInputFormatPlaceholder FormatPlaceholder { get; set; } = DateInputFormatPlaceholder.Formatted;
    [Parameter] public bool AutoSwitchParts { get; set; } = true;
    [Parameter] public List<string> AutoSwitchKeys { get; set; } = [".", "/", "-", " ", ":", ","];
    [Parameter] public bool AutoCorrectParts { get; set; } = true;
    [Parameter] public int TwoDigitYearMax { get; set; } = 68;
    [Parameter] public bool ShowClearButton { get; set; }

    // ── State ────────────────────────────────────────────────────────────────
    [Parameter] public bool Enabled { get; set; } = true;
    /// <summary>Alias for <c>Enabled="false"</c> (earlier API).</summary>
    [Parameter] public bool Disabled { get; set; }
    [Parameter] public bool ReadOnly { get; set; }

    // ── Appearance ───────────────────────────────────────────────────────────
    [Parameter] public string Size { get; set; } = DropDownListTheme.Size.Medium;
    [Parameter] public string Rounded { get; set; } = DropDownListTheme.Rounded.Medium;
    [Parameter] public string FillMode { get; set; } = DropDownListTheme.FillMode.Solid;
    [Parameter] public string? Width { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter] public string? PopupClass { get; set; }
    [Parameter] public AdaptiveMode AdaptiveMode { get; set; } = AdaptiveMode.None;
    /// <summary>Header of the popup action sheet when <see cref="AdaptiveMode"/> is Auto.</summary>
    [Parameter] public string? Title { get; set; }

    [Parameter] public string? Id { get; set; }
    [Parameter] public int? TabIndex { get; set; }
    [Parameter] public string? AriaLabel { get; set; }

    // ── Events ───────────────────────────────────────────────────────────────
    [Parameter] public EventCallback<object?> OnChange { get; set; }
    [Parameter] public EventCallback OnBlur { get; set; }
    [Parameter] public EventCallback<DatePickerOpenEventArgs> OnOpen { get; set; }
    [Parameter] public EventCallback<DatePickerCloseEventArgs> OnClose { get; set; }

    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

    // ── State ────────────────────────────────────────────────────────────────
    protected StackDateInput<T>? Input;
    protected bool IsOpen;
    protected readonly string Uid = $"dp-{Guid.NewGuid():N}"[..11];
    private bool _changedWhileOpen;

    protected bool IsDisabled => !Enabled || Disabled;
    protected DateTime? CurrentDate => DateValue<T>.ToDateTime(Value);
    protected string PopupId => $"{Id ?? Uid}-popup";

    /// <summary>The format used when <see cref="Format"/> is not set.</summary>
    protected abstract string DefaultFormat { get; }
    protected string EffectiveFormat => string.IsNullOrEmpty(Format) ? DefaultFormat : Format;

    /// <summary>Called when the popup opens, so it can show the current value.</summary>
    protected virtual void OnPopupOpening() { }

    // ── Public API ───────────────────────────────────────────────────────────
    /// <summary>Opens the popup. Does not fire <see cref="OnOpen"/>.</summary>
    public void Open()
    {
        if (IsDisabled || ReadOnly || IsOpen) return;
        OnPopupOpening();
        IsOpen = true;
        StateHasChanged();
    }

    /// <summary>Closes the popup. Does not fire <see cref="OnClose"/>.</summary>
    public void Close()
    {
        if (!IsOpen) return;
        IsOpen = false;
        StateHasChanged();
    }

    public void Refresh() => _ = InvokeAsync(StateHasChanged);

    public async Task FocusAsync()
    {
        if (Input is not null) await Input.FocusAsync();
    }

    // ── Open / close ─────────────────────────────────────────────────────────
    protected async Task ToggleAsync()
    {
        if (IsOpen) await CloseFromUserAsync(refocus: true);
        else await OpenFromUserAsync();
    }

    protected async Task OpenFromUserAsync()
    {
        if (IsDisabled || ReadOnly || IsOpen) return;
        var args = new DatePickerOpenEventArgs();
        await OnOpen.InvokeAsync(args);
        if (args.IsCancelled) return;
        OnPopupOpening();
        IsOpen = true;
    }

    protected async Task<bool> CloseFromUserAsync(bool refocus)
    {
        if (!IsOpen) return true;
        var args = new DatePickerCloseEventArgs();
        await OnClose.InvokeAsync(args);
        if (args.IsCancelled) return false;
        IsOpen = false;
        if (_changedWhileOpen)
        {
            _changedWhileOpen = false;
            await OnChange.InvokeAsync(Value);
        }
        if (refocus) await FocusAsync();
        return true;
    }

    protected async Task OnOverlayClickAsync()
    {
        if (await CloseFromUserAsync(refocus: false)) await OnBlur.InvokeAsync();
    }

    /// <summary>Alt+Down opens, Alt+Up / Escape close (handled on the wrapper so it works from any input part).</summary>
    protected async Task OnWrapperKeyDownAsync(KeyboardEventArgs e)
    {
        if (e.AltKey && e.Key == "ArrowDown") await OpenFromUserAsync();
        else if ((e.AltKey && e.Key == "ArrowUp") || e.Key == "Escape") await CloseFromUserAsync(refocus: true);
    }

    /// <summary>Sets the value from the popup (calendar click, Set button).</summary>
    protected async Task SetFromPopupAsync(DateTime? date, bool close)
    {
        var value = DateValue<T>.FromDateTime(date);
        if (!EqualityComparer<T?>.Default.Equals(value, Value))
        {
            Value = value;
            _changedWhileOpen = true;
            await ValueChanged.InvokeAsync(value);
        }
        if (close) await CloseFromUserAsync(refocus: true);
    }

    /// <summary>Typed edits flow through the inner DateInput.</summary>
    protected async Task OnInputValueChangedAsync(T? value)
    {
        Value = value;
        await ValueChanged.InvokeAsync(value);
    }

    protected Task OnInputBlurAsync() => IsOpen ? Task.CompletedTask : OnBlur.InvokeAsync();

    // ── CSS ──────────────────────────────────────────────────────────────────
    protected string OverlayCls => "fixed inset-0 z-40" + (AdaptiveMode == AdaptiveMode.Auto ? " max-sm:bg-black/40" : "");

    protected string PopupCls =>
        "absolute left-0 top-full z-50 mt-1 overflow-hidden rounded-md border bg-popover text-popover-foreground shadow-lg animate-fade-in " +
        (AdaptiveMode == AdaptiveMode.Auto
            ? "max-sm:fixed max-sm:inset-x-0 max-sm:bottom-0 max-sm:top-auto max-sm:mt-0 max-sm:max-h-[85vh] max-sm:overflow-y-auto max-sm:rounded-b-none max-sm:rounded-t-xl "
            : "") +
        PopupClass;

    protected const string ToggleButtonCls =
        "flex h-6 w-6 shrink-0 items-center justify-center rounded text-muted-foreground transition-colors hover:bg-accent hover:text-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring";

    protected const string FooterButtonCls = "rounded-md border px-3 py-1.5 text-xs font-medium transition-colors hover:bg-accent";
    protected const string PrimaryFooterButtonCls = "rounded-md bg-primary px-3 py-1.5 text-xs font-medium text-primary-foreground hover:bg-primary/90";
}
