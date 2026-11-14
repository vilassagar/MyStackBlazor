# MyStackBlazor.Localization

Internationalization and right-to-left support for [MyStackBlazor](https://www.nuget.org/packages/MyStackBlazor):

- `MsRtlProvider`: sets `dir="rtl"` for RTL cultures (Arabic, Hebrew, Urdu, Persian...) so every component mirrors correctly
- `MsResources`: resource-based strings for built-in component labels

```bash
dotnet add package MyStackBlazor.Localization
```

```csharp
builder.Services.AddMyStackBlazorLocalization();
```

```razor
@using MyStackBlazor.Localization

<MsRtlProvider Culture="@CultureInfo.CurrentUICulture">
    @Body
</MsRtlProvider>
```

- Live demo: https://vilassagar.github.io/MyStackBlazor/
- Source: https://github.com/vilassagar/MyStackBlazor
