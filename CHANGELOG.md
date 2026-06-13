# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

> From v1.0.2 onwards this file is maintained automatically by [release-please](https://github.com/googleapis/release-please).

---

## [1.0.2] - 2026-06-13

MyStackBlazor v1.0.2 — new components, charts, enterprise packages, bug fixes & CI:

### New Components

 1. Add `BarChart`, `LineChart`, `PieChart`, `GaugeChart` — pure SVG charts, zero JS dependencies, light/dark themed, hover tooltips, ARIA labels.
 2. Add `FileManager` — tree sidebar + grid/list view, breadcrumb navigation, search, delete callback.
 3. Add `TreeList` — hierarchical data table with expand/collapse and `aria-treegrid` semantics.
 4. Add `PivotGrid` — cross-tab matrix with row/column totals and custom aggregate function.
 5. Add `MsVirtualList<T>` — virtualized list using Blazor's `Virtualize` component.
 6. Add `MarkdownEditor` — split edit/preview pane, 13-action toolbar, live parser, word/char count.
 7. Add `DropDownTree` — hierarchical tree in a dropdown with search and `AllowBranchSelect`.
 8. Add `MultiColumnComboBox` — multi-column grid dropdown, searchable across all columns.
 9. Add `AppBar` — sticky top navigation bar with Start/End slots.
10. Add `ButtonGroup` — joins buttons with shared border radius.
11. Add `ToggleButton` — pressed/unpressed with `IsPressedChanged` two-way binding.
12. Add `Toolbar` — bordered icon/button toolbar.
13. Add `FloatingActionButton` — fixed circular FAB, 4 positions, 3 sizes.
14. Add `DropDownButton` — button that opens a positioned dropdown.
15. Add `SplitButton` — primary action with separate dropdown arrow.
16. Add `Wizard` + `WizardStep` — tab-style multi-step wizard with free navigation.
17. Add `FloatingLabel` — CSS peer-based animated floating label.
18. Add `ListBox<T>` — scrollable list with single/multi-select.
19. Add `MultiSelect<T>` — tag-style multi-value selector with search.
20. Add `DateRangePicker` — start + end date picker with in-range highlight.
21. Add `DateTimePicker` — calendar + hour/minute time selector combined.
22. Add `RangeSlider` — dual-handle slider for value ranges.
23. Add `MaskedTextBox` — pattern masking (`0`=digit, `a`=letter, `*`=any).
24. Add `ColorPalette` — 28-colour swatch grid with customisable palette.
25. Add `ChipList<T>` — typed, removable tag collection.
26. Add `GridLayout` — CSS grid wrapper with `Columns` + `Gap` parameters.
27. Add `StackLayout` — flex stack with `Orientation`, `Align`, `Justify`.
28. Add `Carousel` + `CarouselSlide` — sliding carousel with arrows and dot indicators.
29. Add `PanelBar` + `PanelBarItem` — panel-styled accordion.
30. Add `Splitter` + `SplitterPane` — resizable split panes.
31. Add `Loader` — contained or full-screen loading overlay.
32. Add `ChunkProgressBar` — segmented/chunked progress bar.
33. Add `AnimationContainer` — show/hide with configurable enter/exit CSS classes.
34. Add `Window` — floating window with title bar, footer, and size variants.
35. Add `Popup` — anchor-relative popup with all 4 placements.
36. Add `ListView<T>` — scrollable typed item list with selection.

### Enterprise Packages

37. Add `MyStackBlazor.Core` — `MsStore<TState>` (Fluxor-inspired store) and `MsStoreConsumer<TStore>` (auto-subscribing ComponentBase).
38. Add `MyStackBlazor.Security` — `IHtmlSanitizer` / `DefaultHtmlSanitizer` (XSS via HtmlSanitizer 9), `ITokenProvider` with `InMemoryTokenProvider` (SSR) and `LocalStorageTokenProvider` (WASM).
39. Add `MyStackBlazor.Accessibility` — `MsInteractiveBase` (ARIA-aware base), `IFocusManager` / `FocusManager` (programmatic focus, modal trapping, `returnFocus`).
40. Add `MyStackBlazor.DataGrid` — `MsDataGrid<TItem>` (virtualized enterprise grid, sort/filter/search/export), `GridExporter.ExportToExcel()` (ClosedXML).
41. Add `MyStackBlazor.Localization` — `MsRtlProvider` (RTL/LTR by `CultureInfo`), `AddMyStackBlazorLocalization()`.

### Services & Infrastructure

42. Add `MsStyleLoader` — scoped lazy CSS chunk loader via JS interop.
43. Add GitHub Actions CI workflow — build + unit tests on every PR.
44. Add GitHub Actions release workflow — auto-pack all 6 NuGet packages and push to NuGet.org on `v*.*.*` tags.
45. Add GitHub Pages deploy workflow for the demo site.

### Bug Fixes & Updates

46. Fix `Button.razor` missing ARIA attributes — added `aria-label`, `aria-describedby`, `aria-disabled`, `aria-busy`, and SR-only loading span.
47. Fix `Button.razor` click handler not guarding against disabled/loading state.
48. Add keyboard Enter/Space handling to `Button.razor` (guards `Disabled`/`IsLoading`).
49. Fix `DataTable<T>` missing column filter support — per-column + global search now built in.
50. Fix `DataTable<T>` missing infinite scroll — added alongside existing pagination.
51. Fix `AccordionItem.razor` open/close transition — smooth CSS grid animation.
52. Fix `Stepper.razor` — added vertical orientation mode.
53. Fix various rendering issues in `Common`, `Form`, `Navigation`, `Layout`, and `Data` components.

### Tests

54. Add 7 bUnit + xUnit + FluentAssertions test files: `ButtonTests`, `ChartTests`, `ChipListTests`, `ComplexDataTests`, `ComplexEditorTests`, `FileManagementTests`, `HtmlSanitizerTests`.

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
