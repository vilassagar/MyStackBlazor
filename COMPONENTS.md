# MyStackBlazor — Component Reference

> **Stack:** Blazor (.NET 10) · Tailwind CSS v4 · No external JS dependencies  
> **Version:** 1.2.0  
> **Legend:** ✅ Available · 🆕 Added in v1.2.0 · ❌ Not implemented · ⚠️ Partial / equivalent exists

---

## Table of Contents

- [Common / Display](#common--display)
- [Navigation](#navigation)
- [Form / Editors](#form--editors)
- [Layout](#layout)
- [Data Display](#data-display)
- [Charts](#charts)
- [Overlays](#overlays)
- [Enterprise Packages](#enterprise-packages)
- [Missing — Requires External Library](#missing--requires-external-library)

---

## Common / Display

| Status | Component | File | Notes |
|--------|-----------|------|-------|
| ✅ | **Avatar** | `Components/Common/Avatar.razor` | Image, initials, fallback |
| ✅ | **Badge** | `Components/Common/Badge.razor` | Variant colours |
| ✅ | **Button** | `Components/Common/Button.razor` | Variants: default, destructive, outline, ghost, link |
| ✅ | **Chip** | `Components/Common/Chip.razor` | Single removable tag |
| 🆕 | **ChipList** | `Components/Common/ChipList.razor` | Typed collection of removable chips |
| ✅ | **Icon** | `Components/Common/Icon.razor` | SVG icon wrapper (FontIcon / SvgIcon equivalent) |
| ✅ | **Label** | `Components/Common/Label.razor` | Accessible form label |
| ✅ | **Separator** | `Components/Common/Separator.razor` | Horizontal / vertical divider |
| ✅ | **Skeleton** | `Components/Common/Skeleton.razor` | Loading placeholder |
| ✅ | **Spinner** | `Components/Common/Spinner.razor` | Inline loading spinner |
| ✅ | **ThemeProvider** | `Components/Common/ThemeProvider.razor` | Light / dark / system theme root |
| ✅ | **ThemeToggle** | `Components/Common/ThemeToggle.razor` | Theme switch button |
| ✅ | **Typography** | `Components/Common/Typography.razor` | h1–h4, p, blockquote, code, lead, muted |

---

## Navigation

| Status | Component | File | Notes |
|--------|-----------|------|-------|
| 🆕 | **AppBar** | `Components/Navigation/AppBar.razor` | Sticky top bar with Start/End slots |
| ✅ | **Breadcrumb** | `Components/Navigation/Breadcrumb.razor` | With `BreadcrumbItem` |
| 🆕 | **ButtonGroup** | `Components/Navigation/ButtonGroup.razor` | Joins buttons with shared border |
| 🆕 | **CarouselSlide** | `Components/Navigation/CarouselSlide.razor` | Slide child for `Carousel` |
| ✅ | **ContextMenu** | `Components/Navigation/ContextMenu.razor` | Right-click menu with `ContextMenuItem` |
| 🆕 | **DropDownButton** | `Components/Navigation/DropDownButton.razor` | Button that opens a dropdown |
| ✅ | **DropdownMenu** | `Components/Navigation/DropdownMenu.razor` | Generic dropdown with `DropdownMenuItem` |
| 🆕 | **FloatingActionButton** | `Components/Navigation/FloatingActionButton.razor` | Fixed circular FAB, 4 positions, 3 sizes |
| ✅ | **Menubar** | `Components/Navigation/Menubar.razor` | Horizontal menu bar with `MenubarMenu` |
| ✅ | **Pagination** | `Components/Navigation/Pagination.razor` | Page navigation (Pager equivalent) |
| ✅ | **Sidebar** | `Components/Navigation/Sidebar.razor` | Collapsible side navigation |
| 🆕 | **SplitButton** | `Components/Navigation/SplitButton.razor` | Primary action + separate dropdown arrow |
| ✅ | **Stepper** | `Components/Navigation/Stepper.razor` | Linear / non-linear step indicator; horizontal & **vertical** modes |
| ✅ | **StepperStep** | `Components/Navigation/StepperStep.razor` | Step content panel |
| ✅ | **Tabs** | `Components/Navigation/Tabs.razor` | With `TabsList`, `TabsTrigger`, `TabsContent` |
| 🆕 | **ToggleButton** | `Components/Navigation/ToggleButton.razor` | Pressed / unpressed with `IsPressedChanged` |
| 🆕 | **Toolbar** | `Components/Navigation/Toolbar.razor` | Bordered icon/button toolbar |
| ✅ | **TreeView** | `Components/Data/TreeView.razor` | Collapsible tree with `TreeViewNode` |
| 🆕 | **Wizard** | `Components/Navigation/Wizard.razor` | Tab-style wizard with free navigation |
| 🆕 | **WizardStep** | `Components/Navigation/WizardStep.razor` | Content panel per wizard step |

---

## Form / Editors

| Status | Component | File | Notes |
|--------|-----------|------|-------|
| ✅ | **AutoComplete** | `Components/Form/AutoComplete.razor` | Type-ahead suggestion input |
| ✅ | **Checkbox** | `Components/Form/Checkbox.razor` | Accessible checkbox |
| ✅ | **ColorPicker** | `Components/Form/ColorPicker.razor` | HSL/HEX color wheel picker |
| 🆕 | **ColorPalette** | `Components/Form/ColorPalette.razor` | Swatch grid, 28 default colours, customisable |
| ✅ | **Combobox** | `Components/Form/Combobox.razor` | Searchable single-select dropdown |
| ✅ | **DatePicker** | `Components/Form/DatePicker.razor` | Calendar popover date selector |
| 🆕 | **DateRangePicker** | `Components/Form/DateRangePicker.razor` | Start + end date with in-range highlight |
| 🆕 | **DateTimePicker** | `Components/Form/DateTimePicker.razor` | Calendar + hour / minute time selector |
| ✅ | **FileUpload** | `Components/Form/FileUpload.razor` | Drag-and-drop + browse (DropZone / Upload equivalent) |
| 🆕 | **FloatingLabel** | `Components/Form/FloatingLabel.razor` | CSS-only animated floating label |
| ✅ | **FormField** | `Components/Form/FormField.razor` | Label + input + validation message wrapper |
| ✅ | **Input** | `Components/Form/Input.razor` | Textbox with all standard attributes |
| ✅ | **InputOtp** | `Components/Form/InputOtp.razor` | One-time-password digit boxes |
| 🆕 | **ListBox** | `Components/Form/ListBox.razor` | Scrollable list, single / multi-select |
| 🆕 | **MaskedTextBox** | `Components/Form/MaskedTextBox.razor` | Pattern mask (`0`=digit, `a`=letter, `*`=any) |
| ✅ | **MultiSelect** | `Components/Form/MultiSelect.razor` | Generic `TItem` multi-select: tag pills, search, grouping, Select All, `MaxSelection`, custom `ItemTemplate`, keyboard nav (↑↓ Enter Backspace Esc) |
| ✅ | **NumericInput** | `Components/Form/NumericInput.razor` | Number input with step/min/max (NumericTextBox equivalent) |
| ✅ | **RadioGroup** | `Components/Form/RadioGroup.razor` | Radio button group with `RadioGroupItem` |
| 🆕 | **RangeSlider** | `Components/Form/RangeSlider.razor` | Dual-handle slider for value ranges |
| ✅ | **Rating** | `Components/Form/Rating.razor` | Star rating input |
| ✅ | **Select** | `Components/Form/Select.razor` | Native-style select (DropDownList equivalent) |
| ✅ | **Slider** | `Components/Form/Slider.razor` | Single-handle range slider |
| ✅ | **Switch** | `Components/Form/Switch.razor` | Toggle on/off switch |
| ✅ | **Textarea** | `Components/Form/Textarea.razor` | Multi-line text input |
| ✅ | **TimePicker** | `Components/Form/TimePicker.razor` | Hour / minute / AM-PM selector |

---

## Layout

| Status | Component | File | Notes |
|--------|-----------|------|-------|
| ✅ | **Accordion** | `Components/Layout/Accordion.razor` | With `AccordionItem`; animated open/close |
| 🆕 | **AnimationContainer** | `Components/Layout/AnimationContainer.razor` | Show/hide with configurable enter/exit class |
| ✅ | **AspectRatio** | `Components/Layout/AspectRatio.razor` | Maintain aspect ratio wrapper |
| ✅ | **Card** | `Components/Layout/Card.razor` | With Header, Title, Description, Content, Footer |
| 🆕 | **Carousel** | `Components/Layout/Carousel.razor` | Sliding carousel with arrows and dot indicators |
| 🆕 | **ChunkProgressBar** | `Components/Layout/ChunkProgressBar.razor` | Segmented / chunked progress bar |
| ✅ | **Collapsible** | `Components/Layout/Collapsible.razor` | Simple show/hide region |
| 🆕 | **GridLayout** | `Components/Layout/GridLayout.razor` | CSS grid wrapper with `Columns` + `Gap` |
| 🆕 | **Loader** | `Components/Layout/Loader.razor` | Contained or `FullScreen` loading overlay |
| 🆕 | **PanelBar** | `Components/Layout/PanelBar.razor` | Panel-styled accordion with `PanelBarItem` |
| ✅ | **ScrollArea** | `Components/Layout/ScrollArea.razor` | Custom scrollbar container |
| 🆕 | **StackLayout** | `Components/Layout/StackLayout.razor` | Flex stack with `Orientation`, `Align`, `Justify` |
| 🆕 | **Splitter** | `Components/Layout/Splitter.razor` | Resizable split panes with `SplitterPane` |

---

## Data Display

| Status | Component | File | Notes |
|--------|-----------|------|-------|
| ✅ | **Alert** | `Components/Data/Alert.razor` | With `AlertTitle` and `AlertDescription` |
| ✅ | **AdvancedTable** | `Components/Data/AdvancedTable.razor` | Extended table with column-level filtering, grouping, custom cell renderers |
| 🆕 | **Calendar (MsCalendar)** | `Components/Calendar/MsCalendar.razor` | 6 views (Day/Week/Month/Year/Agenda/Timeline), recurring events, multi-person calendars, Individual/Department focus, category sidebar, live search |
| ✅ | **DataTable** | `Components/Data/DataTable.razor` | Sort · global search · column filters · pagination · infinite scroll · virtualize · row selection |
| ✅ | **FileManager** | `Components/Data/FileManager.razor` | Tree sidebar + grid/list view, breadcrumb nav, search, rename/delete callbacks |
| ✅ | **ListView** | `Components/Data/ListView.razor` | Scrollable typed item list with custom `ItemTemplate` and virtualization |
| ✅ | **PivotGrid** | `Components/Data/PivotGrid.razor` | Cross-tab matrix with row/column totals and custom aggregate function (sum/avg/count/min/max) |
| ✅ | **Progress** | `Components/Data/Progress.razor` | Linear progress bar |
| ✅ | **Table** | `Components/Data/Table.razor` | Semantic HTML table primitives |
| ✅ | **TreeList** | `Components/Data/TreeList.razor` | Hierarchical data table with expand/collapse rows and `aria-treegrid` semantics |
| ✅ | **TreeView** | `Components/Data/TreeView.razor` | Recursive collapsible tree with `TreeViewNode` |
| ✅ | **VirtualList** | `Components/Data/MsVirtualList.razor` | Virtualized list powered by Blazor's `Virtualize` component |

---

## Charts

> Pure SVG, zero JS dependencies. Light/dark themed via CSS custom properties. Hover tooltips, legends, accessible ARIA.

| Status | Component | File | Notes |
|--------|-----------|------|-------|
| 🆕 | **BarChart** | `Components/Charts/BarChart.razor` | Grouped or stacked bars; multi-series; hover tooltip |
| 🆕 | **GaugeChart** | `Components/Charts/GaugeChart.razor` | Half-circle gauge; threshold color zones; unit label |
| 🆕 | **LineChart** | `Components/Charts/LineChart.razor` | Multi-series lines; optional area fill; hover tooltip |
| 🆕 | **PieChart** | `Components/Charts/PieChart.razor` | Pie or donut; percentage labels; center label; hover tooltip |

**Shared models** (`Components/Charts/ChartModels.cs`): `ChartSeries`, `ChartSegment`, `GaugeThreshold`

---

## Enterprise Packages

Installed separately — see [Modular Packages](#modular-packages) in README.

| Status | Component / Service | Package | Notes |
|--------|---------------------|---------|-------|
| ✅ | **MsDataGrid** | `MyStackBlazor.DataGrid` | Virtualized enterprise grid, server-side sort/filter/search, Excel export (ClosedXML) |
| ✅ | **MsDataGridColumn** | `MyStackBlazor.DataGrid` | Non-rendering column definition child component |
| ✅ | **GridExporter** | `MyStackBlazor.DataGrid` | `ExportToExcel()` → `byte[]` download |
| ✅ | **MsStore / MsStoreConsumer** | `MyStackBlazor.Core` | Lightweight Fluxor-style state management |
| ✅ | **IHtmlSanitizer / DefaultHtmlSanitizer** | `MyStackBlazor.Security` | XSS sanitization via HtmlSanitizer 9 |
| ✅ | **ITokenProvider** | `MyStackBlazor.Security` | Auth token abstraction (In-memory / LocalStorage) |
| ✅ | **IFocusManager / FocusManager** | `MyStackBlazor.Accessibility` | Programmatic focus, modal trap, `returnFocus` |
| ✅ | **MsInteractiveBase** | `MyStackBlazor.Accessibility` | ARIA-aware `ComponentBase` with `AriaLabel`, `Disabled`, `ComponentId` |
| ✅ | **MsRtlProvider** | `MyStackBlazor.Localization` | RTL/LTR wrapper based on `CultureInfo.CurrentUICulture` |

---

## Overlays

| Status | Component | File | Notes |
|--------|-----------|------|-------|
| ✅ | **AlertDialog** | `Components/Overlays/AlertDialog.razor` | Confirmation dialog |
| ✅ | **Dialog** | `Components/Overlays/Dialog.razor` | Modal dialog with Header / Title / Footer |
| ✅ | **Drawer** | `Components/Overlays/Drawer.razor` | Side-anchored slide-in panel |
| ✅ | **HoverCard** | `Components/Overlays/HoverCard.razor` | Hover-triggered card preview |
| ✅ | **Popover** | `Components/Overlays/Popover.razor` | Click-triggered positioned panel |
| 🆕 | **Popup** | `Components/Overlays/Popup.razor` | Anchor-relative popup, all 4 placements |
| ✅ | **Sheet** | `Components/Overlays/Sheet.razor` | Full-height slide-in sheet |
| ✅ | **Toaster** | `Components/Overlays/Toaster.razor` | Toast notification system |
| ✅ | **Tooltip** | `Components/Overlays/Tooltip.razor` | Hover tooltip, 4 sides |
| 🆕 | **Window** | `Components/Overlays/Window.razor` | Floating window with title bar, footer, size variants |

---

## Missing — Requires External Library

The following categories need dedicated third-party rendering engines and are **not included** in MyStackBlazor. Recommended integrations are listed.

### 📊 Charts (advanced)
Basic charts (Line, Bar, Pie, Gauge) are now built-in — see [Charts](#charts) above. Complex types still require a library.

| Component | Recommended Library |
|-----------|-------------------|
| Bubble / Scatter Chart | [ApexCharts.Blazor](https://github.com/apexcharts/Blazor-ApexCharts) |
| Candlestick / OHLC / Stock Chart | ApexCharts.Blazor |
| Heatmap Chart | ApexCharts.Blazor |
| Radar / Spider Chart | ApexCharts.Blazor |
| Sankey / Waterfall Chart | ApexCharts.Blazor |

### 🔵 Gauges (advanced)
`GaugeChart` (half-circle) is now built-in — see [Charts](#charts) above. Full circular and linear gauges still need a library.

| Component | Recommended Library |
|-----------|-------------------|
| Circular / Radial Gauge | ApexCharts.Blazor |
| Linear Gauge | ApexCharts.Blazor |

### 📦 Barcodes
| Component | Recommended Library |
|-----------|-------------------|
| Barcode | [BarcodeLib](https://github.com/barnhill/barcodelib) + render as image |
| QR Code | [QRCoder](https://github.com/codebude/QRCoder) |

### 📄 PDF
| Component | Recommended Library |
|-----------|-------------------|
| PDF Viewer | [Syncfusion Blazor PDF Viewer](https://blazor.syncfusion.com/documentation/pdfviewer/getting-started) or [PSPDFKit](https://pspdfkit.com/guides/blazor) |

### 📅 Scheduling
| Component | Status | Notes |
|-----------|--------|-------|
| Calendar | ✅ | `MsCalendar` — Day/Week/Month/Year/Agenda/Timeline views, recurring events, multi-person, search |
| Gantt | ✅ | `MsCalendar` Timeline view provides a year Gantt (month rows × day columns with event bars) |
| Scheduler (time-slot) | ⚠️ | Day/Week views provide time-slot scheduling; for resource scheduling use [Radzen Blazor Scheduler](https://blazor.radzen.com/scheduler) |

### 🗺️ Diagrams & Maps
| Component | Recommended Library |
|-----------|-------------------|
| Diagram / Flowchart | [Syncfusion Blazor Diagram](https://blazor.syncfusion.com/documentation/diagram/getting-started) |
| Map | [Blazor Leaflet](https://github.com/rungwiroon/BlazorLeaflet) or [Radzen Map](https://blazor.radzen.com/map) |

### 🤖 AI / Interactivity (Advanced)
| Component | Notes |
|-----------|-------|
| AIPrompt / InlineAIPrompt | Requires Anthropic / OpenAI API integration |
| Chat | Requires SignalR or streaming API |
| SmartPasteButton | Requires AI backend |
| SpeechToTextButton | Requires `window.SpeechRecognition` Web API |

### 🗂️ Complex Data Components
| Component | Status | Notes |
|-----------|--------|-------|
| TreeList | ✅ | `TreeList<T>` — hierarchical data table with expand/collapse, ARIA treegrid semantics |
| PivotGrid | ✅ | `PivotGrid<T>` — cross-tab matrix with aggregation, row/column totals, custom aggregate func |
| Calendar / Scheduler | ✅ | `MsCalendar` — 6 views, recurring events, multi-person, search, Individual/Department mode |
| Enterprise DataGrid | ✅ | `MsDataGrid<T>` (`MyStackBlazor.DataGrid` package) — server-side sort/filter/search/export |
| Spreadsheet | ❌ | Use [NPOI](https://github.com/tonyqus/npoi) for server-side processing |
| Grid (advanced) | ✅ | `AdvancedTable` covers sorting, filtering, pagination, virtualization |
| Filter (standalone) | ✅ | Per-column + global search built into `DataTable` and `AdvancedTable` |
| DockManager | ❌ | Very complex layout engine — no pure-Blazor equivalent |

### ✏️ Editors (Complex)
| Component | Status | Notes |
|-----------|--------|-------|
| MarkdownEditor | ✅ | Split edit/preview, toolbar with 13 format actions, live markdown parser, word/char count |
| DropDownTree | ✅ | Hierarchical tree in a dropdown, expand/collapse, search, `AllowBranchSelect` |
| MultiColumnComboBox | ✅ | Multi-column grid dropdown, searchable across all columns |
| Rich Text Editor (WYSIWYG) | ❌ | Use [Blazor.Quill](https://github.com/Blazored/BlazoredTextEditor) or [TinyMCE](https://www.tiny.cloud/docs/integrations/blazor) |
| Signature | ❌ | Requires canvas + pointer events JS |
| ColorGradient | ❌ | Advanced canvas-based picker — use `ColorPicker` for basic needs |
| MaskedTextBox | ✅ | Basic pattern masking included (`0`=digit, `a`=letter, `*`=any) |

### 📁 File Management
| Component | Status | Notes |
|-----------|--------|-------|
| DropZone / Upload | ✅ | `FileUpload` — drag-drop + browse, image preview, multi-file |
| FileManager | ✅ | Tree sidebar + grid/list view, breadcrumb nav, search, rename/delete callbacks |
| FileSelect | ✅ | `FileUpload` covers file selection |

---

## Component Count Summary

| Category | Available (✅ + 🆕) | Missing (❌) |
|----------|---------------------|--------------|
| Common / Display | 13 | 0 |
| Navigation | 20 | 0 |
| Form / Editors | 25 | 3 (rich editor, signature, color gradient) |
| Layout | 13 | 0 |
| Data Display | 12 | 1 (Spreadsheet) |
| Charts | 4 | 5 (advanced: Bubble, Scatter, Heatmap, Radar, Candlestick) |
| Overlays | 10 | 0 |
| Enterprise Packages | 9 | 0 |
| **Total UI components** | **106** | **9** |
| Barcodes | 0 | 2 |
| PDF / Maps / Diagrams | 0 | 3 |
| Scheduling | ✅ (built-in) | 0 |
| AI | 0 | 5 |

---

## Release History

### v1.2.0 — 2026-06-17

| Component | Category | Key Feature |
|-----------|----------|-------------|
| `MsCalendar` — Year view | Calendar | 12-month card grid with colored dot event indicators |
| `MsCalendar` — Agenda view | Calendar | Date-grouped scrollable event list for next 60 days |
| `MsCalendar` — Timeline view | Calendar | Year Gantt: month rows × day columns with stacked event bars |
| `MsCalendar` — Recurring events | Calendar | Daily/Weekly/Monthly/Yearly with interval, end date, count |
| `MsCalendar` — People sidebar | Calendar | Per-person colored checkbox toggles (`CalendarPerson`) |
| `MsCalendar` — Individual/Department | Calendar | Focus mode toggle: own calendar vs full team view |
| `MsCalendar` — Category filter | Calendar | Sidebar active-highlight category filtering |
| `MsCalendar` — Live search | Calendar | Filters events by title, location, description, category |
| `MultiSelect<T>` (full rewrite) | Form | Generic; tag pills; search; per-group Select All; `MaxSelection`; `ItemTemplate`; keyboard nav |

**Bug fixes:** Radio button border too light in light theme → `border-2 border-gray-400`. Switch track/border invisible in light theme → `bg-gray-200 border-gray-500`.

---

### v1.1.0 — 2026-06-15

Bug fixes: CSS styling, light/dark theme token contrast, PivotGrid layout, unlimited scroll in DataGrid, Calendar rendering.

---

### v1.0.2 — 2026-06-13

| Component | Category | Key Feature |
|-----------|----------|-------------|
| `ChipList<T>` | Common | Typed, removable tag collection |
| `AppBar` | Navigation | Sticky top navigation bar |
| `ButtonGroup` | Navigation | Connected button group styling |
| `ToggleButton` | Navigation | Press/unpress two-way binding |
| `Toolbar` | Navigation | Bordered icon toolbar |
| `FloatingActionButton` | Navigation | Fixed circular FAB |
| `DropDownButton` | Navigation | Button with open/close dropdown |
| `SplitButton` | Navigation | Primary action + dropdown arrow |
| `Wizard` + `WizardStep` | Navigation | Tab-style multi-step wizard |
| `Stepper` (updated) | Navigation | Added vertical orientation mode |
| `FloatingLabel` | Form | CSS peer-based animated label |
| `ListBox<T>` | Form | Scrollable list, single/multi-select |
| `MultiSelect<T>` | Form | Initial tag-style multi-value selector |
| `DateRangePicker` | Form | Start + end with range highlight |
| `DateTimePicker` | Form | Calendar + time combined |
| `RangeSlider` | Form | Dual-handle range input |
| `MaskedTextBox` | Form | Pattern masking (phone, date, etc.) |
| `ColorPalette` | Form | Swatch grid colour picker |
| `MarkdownEditor` | Form | Split edit/preview, 13-action toolbar |
| `DropDownTree` | Form | Hierarchical tree dropdown |
| `MultiColumnComboBox` | Form | Multi-column searchable dropdown |
| `GridLayout` | Layout | CSS grid wrapper |
| `StackLayout` | Layout | Flex stack wrapper |
| `Carousel` + `CarouselSlide` | Layout | Sliding carousel with dots & arrows |
| `PanelBar` + `PanelBarItem` | Layout | Panel-bar accordion |
| `Splitter` + `SplitterPane` | Layout | Resizable split panes |
| `Loader` | Layout | Contained / full-screen overlay |
| `ChunkProgressBar` | Layout | Segmented progress bar |
| `AnimationContainer` | Layout | Configurable enter/exit animations |
| `AdvancedTable` | Data | Column-level filtering, grouping, custom cell renderers |
| `PivotGrid<T>` | Data | Cross-tab matrix with aggregates |
| `TreeList<T>` | Data | Hierarchical data table, ARIA treegrid |
| `ListView<T>` | Data | Scrollable typed item list |
| `FileManager` | Data | Tree sidebar, grid/list view, breadcrumb nav |
| `MsVirtualList<T>` | Data | Virtualized list via Blazor `Virtualize` |
| `BarChart` | Charts | Grouped/stacked bars, multi-series |
| `LineChart` | Charts | Multi-series lines, optional area fill |
| `PieChart` | Charts | Pie/donut with % labels and center label |
| `GaugeChart` | Charts | Half-circle gauge with threshold zones |
| `Window` | Overlays | Floating window with title bar |
| `Popup` | Overlays | Anchor-relative popup, 4 placements |

---

## Setup

```html
<!-- In index.html / App.razor -->
<link rel="stylesheet" href="_content/MyStackBlazor/css/mystackblazor.css" />
<script src="_content/MyStackBlazor/js/mystackblazor.js"></script>
```

```csharp
// In Program.cs
builder.Services.AddMyStackBlazor();
```

```razor
<!-- Wrap root layout for theming -->
<ThemeProvider>
    @Body
</ThemeProvider>
```

```xml
<!-- Global usings -->
@using MyStackBlazor.Components.Common
@using MyStackBlazor.Components.Form
@using MyStackBlazor.Components.Navigation
@using MyStackBlazor.Components.Layout
@using MyStackBlazor.Components.Data
@using MyStackBlazor.Components.Overlays
```
