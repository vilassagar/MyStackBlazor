# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

> From v1.0.2 onwards this file is maintained automatically by [release-please](https://github.com/googleapis/release-please).

---

## [1.3.1](https://github.com/vilassagar/MyStackBlazor/compare/v1.3.0...v1.3.1) (2026-07-12)


### Bug Fixes

* correct stale Switch unchecked-state test assertion ([2d7531c](https://github.com/vilassagar/MyStackBlazor/commit/2d7531c24d8e8bea7e1faaed3a8732fe2629ea6a))

## [1.2.2] - 2026-06-20

### Bug Fixes

1. Restore solution build after the Stack-prefixed component migration by adding unit-test compatibility mappings for legacy component names.
2. Fix unit test compilation for renamed generic component types such as `ChipList<T>`, `TreeView<T>`, `TreeList<T>`, `PivotGrid<T>`, `DropDownTree<T>`, `MultiColumnComboBox<T>`, and `MsPivotGrid<T>`.
3. Align test imports with current component namespaces so older test coverage continues to build against the renamed public component surface.

---

## [1.2.1] - 2026-06-18

### Bug Fixes

1. Fix form component border visibility in light and dark themes — changed all form input borders from `border` (1 px) to `border-2` (2 px) across `Input`, `Textarea`, `Checkbox`, `Combobox`, `AutoComplete`, `Select`, `ListBox`, `DatePicker`, `DateRangePicker`, `DateTimePicker`, `TimePicker`, `NumericInput`, `InputOtp`, `MaskedTextBox`, `MultiColumnComboBox`, `DropDownTree`, `FloatingLabel`, and `ColorPicker`.

### New

2. Add `ControlShowcase` template — a single-page component that demos every form and common component in one view.
3. Add new Heroicons to `Icons.cs` — expanded bundled SVG icon set.

---

## [1.2.0] - 2026-06-17

### New Components

1. Add `MultiSelect<T>` — fully-featured searchable multi-select dropdown with tag pills, Select All, per-group selection, indeterminate checkbox state, `MaxSelection` limit, custom `ItemTemplate`, keyboard navigation (↑↓ Enter Escape Backspace), and `+N more` tag overflow.

### Calendar Enhancements (`MsCalendar`)

2. Add **Year view** — 12-month grid with colored dot event indicators and per-month drill-down.
3. Add **Agenda view** — scrollable date-grouped event list for the next 60 days with owner and recurrence icons.
4. Add **Timeline view** — year Gantt chart with month rows, day columns, today indicator, and slot-stacked event bars.
5. Add **Recurring events** — `Daily`, `Weekly`, `Monthly`, and `Yearly` recurrence with configurable interval, end date, and occurrence count. Events are auto-expanded within any date range.
6. Add **Individual / Department focus mode** — toggle between your own calendar and the full team view.
7. Add **People / calendar sidebar** — colored checkbox buttons per team member (`CalendarPerson`) with per-person visibility toggle.
8. Add **Category filter** — sidebar section to filter events by category with active highlight.
9. Add **Live search** — toolbar search bar that filters events by title, location, description, and category across all views.
10. Add `People` and `DefaultFocus` parameters to `MsCalendar`.
11. Add `CalendarPerson` model and `FocusMode` enum to `CalendarModels.cs`.
12. Add recurrence fields (`RecurrenceType`, `RecurrenceInterval`, `RecurrenceEndDate`, `RecurrenceCount`) to `CalendarEvent`.
13. Add recurrence UI to Add/Edit modal — repeat type dropdown, interval input, end date picker.
14. Add owner display to event detail modal — colored dot + person name.

### Bug Fixes

15. Fix Radio button border not visible in light theme — changed from `border-primary` (light) to `border-2 border-gray-400` (unchecked) / `border-primary` (checked).
16. Fix Switch control border and track not visible in light theme — unchecked state now uses `bg-gray-200 border-gray-500` instead of `bg-primary/15 border-primary/30`.

---

## [1.1.0] - 2026-06-15

### Bug Fixes & Polish

1. Fix CSS styling issues across multiple components.
2. Fix light/dark theme token inconsistencies — improved contrast for `text-muted-foreground`, `border-input`, and `bg-accent` tokens.
3. Fix `PivotGrid` layout and aggregate rendering issues.
4. Fix unlimited/virtual scroll behaviour in `MsDataGrid` and `MsVirtualList`.
5. Fix `MsCalendar` rendering issues — event chip overflow, multi-day spanning, and week view alignment.
6. Add demo layout and data components to the demo project.

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
