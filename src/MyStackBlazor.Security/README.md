# MyStackBlazor.Security

Security utilities for Blazor apps using [MyStackBlazor](https://www.nuget.org/packages/MyStackBlazor):

- `IHtmlSanitizer`: XSS-safe HTML sanitization (backed by [HtmlSanitizer](https://github.com/mganss/HtmlSanitizer)) for rich-text editor output and user content
- `ITokenProvider`: auth-token storage abstraction with in-memory (Blazor Server) and `localStorage` (WASM) implementations
- CSP-friendly: no inline script or `eval`

```bash
dotnet add package MyStackBlazor.Security
```

```csharp
// Blazor Server: in-memory tokens
builder.Services.AddMyStackBlazorSecurity();

// Blazor WASM: tokens in localStorage
builder.Services.AddMyStackBlazorSecurity(useLocalStorage: true);

// .NET MAUI: register your own ITokenProvider backed by SecureStorage
```

```razor
@inject IHtmlSanitizer Sanitizer

@((MarkupString)Sanitizer.Sanitize(userHtml))
```

- Live demo: https://vilassagar.github.io/MyStackBlazor/
- Source: https://github.com/vilassagar/MyStackBlazor
