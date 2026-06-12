# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

> From v1.0.2 onwards this file is maintained automatically by [release-please](https://github.com/googleapis/release-please).

---

## [1.0.1] - 2026-06-11

### Added

- Enterprise multi-package solution restructure: `MyStackBlazor.Core`, `MyStackBlazor.Security`, `MyStackBlazor.Accessibility`, `MyStackBlazor.DataGrid`, `MyStackBlazor.Localization`
- `MsDataGrid<TItem>` — virtualized enterprise data grid with sort, filter, search, and Excel export (ClosedXML)
- `MsDataGridColumn<TItem>` — non-rendering column definition component
- `GridExporter.ExportToExcel()` — ClosedXML byte array download
- `ITokenProvider` — auth token abstraction with `InMemoryTokenProvider` (SSR) and `LocalStorageTokenProvider` (WASM)
- `IHtmlSanitizer` / `DefaultHtmlSanitizer` — XSS sanitization via HtmlSanitizer 9.0.892
- `IFocusManager` / `FocusManager` — programmatic focus, modal focus trapping, `returnFocus`
- `MsInteractiveBase` — ARIA-aware component base (`AriaLabel`, `AriaDescribedBy`, `Disabled`, `ComponentId`)
- `MsStore<TState>` / `MsStoreConsumer<TStore>` — lightweight Fluxor-inspired state management
- `MsRtlProvider` — RTL/LTR wrapper based on `CultureInfo.CurrentUICulture`
- `Button.razor` — accessibility improvements: `aria-label`, `aria-describedby`, `aria-disabled`, `aria-busy`, keyboard Enter/Space handling
- `FileUpload` component
- `AdvancedTable` component
- GitHub Actions CI workflow: build + test on PR, pack + publish on `v*.*.*` tag
- 27 passing bUnit + xUnit + FluentAssertions unit tests

### Changed

- Solution migrated to `.slnx` format (`MyStackBlazor.slnx`)
- Removed demo/sample project references from solution

---

## [1.0.0] - initial

### Added

- Blazor Razor Class Library targeting .NET 10 with Tailwind CSS v4
- Light / Dark / System theme support via `ThemeService`
- `ToastService` — scoped toast notification queue
- Component namespaces: Common, Form, Navigation, Layout, Overlays, Data
- Blazor WASM, Blazor Server, and .NET MAUI Blazor Hybrid support

---

<!-- release-please-start -->
<!-- release-please-end -->
