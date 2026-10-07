using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MyStackBlazor.Components.Navigation;

/// <summary>Parameters shared by <see cref="StackButtonGroupButton"/> and <see cref="StackButtonGroupToggleButton"/>.</summary>
public abstract class ButtonGroupItemBase : ComponentBase, IButtonGroupItem, IDisposable
{
    [CascadingParameter] protected StackButtonGroup? Group { get; set; }

    [Parameter] public RenderFragment? ChildContent { get; set; }

    /// <summary>Fires when the button is clicked.</summary>
    [Parameter] public EventCallback<MouseEventArgs> OnClick { get; set; }

    [Parameter] public bool Enabled { get; set; } = true;
    [Parameter] public bool Visible { get; set; } = true;

    /// <summary>Icon name from <see cref="Icons"/> (e.g. "plus"), rendered before the content.</summary>
    [Parameter] public string? Icon { get; set; }

    /// <summary>Overrides the group's ThemeColor for this button.</summary>
    [Parameter] public string? ThemeColor { get; set; }

    [Parameter] public string? Id { get; set; }
    [Parameter] public string? Title { get; set; }
    [Parameter] public int? TabIndex { get; set; }
    [Parameter] public string? AriaLabel { get; set; }
    [Parameter] public string? Class { get; set; }
    [Parameter(CaptureUnmatchedValues = true)] public Dictionary<string, object>? AdditionalAttributes { get; set; }

    protected bool IsDisabled => !Enabled || Group is { Enabled: false };

    protected override void OnInitialized() => Group?.Register(this);

    void IButtonGroupItem.Refresh() => StateHasChanged();

    public virtual void Dispose() => Group?.Unregister(this);

    protected string ComputeClass(bool selected) =>
        Group?.ButtonCls(selected, IsDisabled, ThemeColor, Class)
        ?? $"inline-flex items-center justify-center gap-2 rounded-md border border-input bg-background px-4 h-10 text-sm font-medium {Class}".Trim();
}
