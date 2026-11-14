# MyStackBlazor.Accessibility

WCAG 2.2 helpers for Blazor, used by every [MyStackBlazor](https://www.nuget.org/packages/MyStackBlazor) component and usable on their own:

- `IFocusManager`: focus an element, trap focus inside dialogs, and return focus afterwards
- `MsInteractiveBase`: base class that wires ARIA attributes and keyboard handling

```bash
dotnet add package MyStackBlazor.Accessibility
```

```csharp
builder.Services.AddMyStackBlazorAccessibility();
```

```razor
@inject IFocusManager Focus

<div @ref="_panel">...</div>

@code {
    ElementReference _panel;
    Task OpenAsync() => Focus.TrapFocusAsync(_panel).AsTask();
    Task CloseAsync() => Focus.ReturnFocusAsync().AsTask();
}
```

- Live demo: https://vilassagar.github.io/MyStackBlazor/
- Source: https://github.com/vilassagar/MyStackBlazor
