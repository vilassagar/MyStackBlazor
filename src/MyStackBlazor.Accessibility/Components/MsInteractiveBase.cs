using Microsoft.AspNetCore.Components;

namespace MyStackBlazor.Accessibility.Components;

/// <summary>
/// Base class for all interactive MyStackBlazor components.
/// Provides consistent ARIA attribute binding and a stable component ID.
/// </summary>
public abstract class MsInteractiveBase : ComponentBase
{
    [Parameter] public string? AriaLabel { get; set; }
    [Parameter] public string? AriaDescribedBy { get; set; }
    [Parameter] public string? AriaLabelledBy { get; set; }
    [Parameter] public bool Disabled { get; set; }

    /// <summary>Unique per-instance ID for aria-controls / aria-labelledby wiring.</summary>
    protected string ComponentId { get; } = $"ms-{Guid.NewGuid():N}";

    /// <summary>Returns "true" when Disabled, null otherwise — compatible with aria-disabled.</summary>
    protected string? AriaDisabled => Disabled ? "true" : null;
}
