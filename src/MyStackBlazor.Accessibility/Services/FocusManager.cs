using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace MyStackBlazor.Accessibility.Services;

public sealed class FocusManager : IFocusManager, IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _module;

    public FocusManager(IJSRuntime js) => _js = js;

    private async ValueTask<IJSObjectReference> GetModuleAsync()
        => _module ??= await _js.InvokeAsync<IJSObjectReference>(
               "import", "./_content/MyStackBlazor.Accessibility/js/focus-manager.js");

    public async ValueTask FocusAsync(ElementReference element)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("focusElement", element);
    }

    public async ValueTask TrapFocusAsync(ElementReference container)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("trapFocus", container);
    }

    public async ValueTask ReleaseFocusAsync(ElementReference container)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("releaseFocus", container);
    }

    public async ValueTask ReturnFocusAsync()
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("returnFocus");
    }

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
            await _module.DisposeAsync();
    }
}
