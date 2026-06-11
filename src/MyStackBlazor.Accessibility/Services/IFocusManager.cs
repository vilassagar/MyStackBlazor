using Microsoft.AspNetCore.Components;

namespace MyStackBlazor.Accessibility.Services;

/// <summary>
/// Manages programmatic focus — use for modals, drawers, and complex keyboard flows.
/// </summary>
public interface IFocusManager
{
    /// <summary>Move focus to the given element.</summary>
    ValueTask FocusAsync(ElementReference element);

    /// <summary>
    /// Trap Tab/Shift+Tab within a container (e.g. an open modal).
    /// Call ReleaseFocusAsync to remove the trap.
    /// </summary>
    ValueTask TrapFocusAsync(ElementReference container);

    /// <summary>Remove a previously set focus trap.</summary>
    ValueTask ReleaseFocusAsync(ElementReference container);

    /// <summary>Return focus to whichever element had it before the last TrapFocusAsync.</summary>
    ValueTask ReturnFocusAsync();
}
