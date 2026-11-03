

# MyStackBlazor

A modern Blazor component library powered by Tailwind CSS v4.

[![NuGet](https://img.shields.io/nuget/v/MyStackBlazor?color=512BD4&logo=nuget&label=NuGet)](https://www.nuget.org/packages/MyStackBlazor)
[![NuGet Downloads](https://img.shields.io/nuget/dt/MyStackBlazor?color=512BD4&logo=nuget)](https://www.nuget.org/packages/MyStackBlazor)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com)
[![Tailwind CSS v4](https://img.shields.io/badge/Tailwind%20CSS-v4-06B6D4?logo=tailwindcss)](https://tailwindcss.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

192 components - Blazor WASM / Server / MAUI Hybrid - Light / Dark / System - Zero external JS dependencies

[Live Demo](https://vilassagar.github.io/MyStackBlazor/)



---

## Overview

MyStackBlazor is a production-ready UI library for .NET 10 Blazor applications.

- 192 components across Common, Form, Layout, Navigation, Overlays, Data, Charts, Blocks, and Templates
- Tailwind CSS v4 styling (precompiled CSS included)
- Built-in theming via `ThemeService` and `StackThemeProvider`
- Accessibility-focused patterns and keyboard support
- Optional modular packages for DataGrid, Security, Accessibility, Localization, and Core primitives

## Installation

```bash
dotnet add package MyStackBlazor
```

Or manually:

```xml
<PackageReference Include="MyStackBlazor" Version="1.4.0" />
```

## Setup

### 1. Register services

```csharp
builder.Services.AddMyStackBlazor();
```

### 2. Add static assets

Add in `wwwroot/index.html` (WASM) or your host/app shell:

```html
<link rel="stylesheet" href="_content/MyStackBlazor/css/mystackblazor.css" />
<script src="_content/MyStackBlazor/js/mystackblazor.js"></script>
```

### 3. Add global imports

In `_Imports.razor`:

```razor
@using MyStackBlazor
@using MyStackBlazor.Components
@using MyStackBlazor.Components.Common
@using MyStackBlazor.Components.Form
@using MyStackBlazor.Components.Navigation
@using MyStackBlazor.Components.Layout
@using MyStackBlazor.Components.Overlays
@using MyStackBlazor.Components.Data
@using MyStackBlazor.Components.Charts
@using MyStackBlazor.Components.Calendar
@using MyStackBlazor.Components.PivotGrid
```

### 4. Wrap your layout

```razor
@inherits LayoutComponentBase

<StackThemeProvider>
    <div class="min-h-screen bg-background text-foreground">
        <header>
            <StackThemeToggle />
        </header>
        <main>
            <StackToaster />
            @Body
        </main>
    </div>
</StackThemeProvider>
```

## Component Naming

All public components use the `Stack` prefix.

```razor
<StackButton>Save</StackButton>
<StackDialog @bind-IsOpen="_open">
    <StackDialogTitle>Confirm</StackDialogTitle>
</StackDialog>
```

Special names to note:

- `StackMsCalendar`
- `StackMsVirtualList`
- `StackMsPivotGrid`

## Quick Start Example

```razor
@page "/"

<StackCard Class="max-w-xl">
    <StackCardHeader>
        <StackCardTitle>Welcome</StackCardTitle>
        <StackCardDescription>MyStackBlazor is wired up.</StackCardDescription>
    </StackCardHeader>
    <StackCardContent>
        <StackInput @bind-Value="_name" Placeholder="Your name" />
    </StackCardContent>
    <StackCardFooter>
        <StackButton OnClick="Save">Save</StackButton>
    </StackCardFooter>
</StackCard>

@code {
    private string _name = string.Empty;
    private void Save() { }
}
```

## Theming

`StackThemeProvider` applies and persists theme state. Use `ThemeService` in any component:

```csharp
@inject ThemeService ThemeService

ThemeService.SetTheme(AppTheme.Light);
ThemeService.SetTheme(AppTheme.Dark);
ThemeService.SetTheme(AppTheme.System);
ThemeService.Toggle();

bool isDark = ThemeService.IsDark;
```

## Core Component Families

### Common

`StackButton`, `StackBadge`, `StackAvatar`, `StackIcon`, `StackTypography`, `StackLabel`, `StackSeparator`, `StackSkeleton`, `StackSpinner`, `StackThemeToggle`

### Form

`StackInput`, `StackTextarea`, `StackSelect`, `StackCheckbox`, `StackSwitch`, `StackRadioGroup`, `StackNumericInput`, `StackSlider`, `StackRangeSlider`, `StackDatePicker`, `StackDateRangePicker`, `StackDateTimePicker`, `StackTimePicker`, `StackInputOtp`, `StackColorPicker`, `StackColorPalette`, `StackMaskedTextBox`, `StackMultiSelect`, `StackCombobox`, `StackAutoComplete`, `StackListBox`, `StackFileUpload`

### Layout

`StackCard`, `StackAccordion`, `StackCollapsible`, `StackScrollArea`, `StackAspectRatio`, `StackGridLayout`, `StackLayout`, `StackSplitter`, `StackCarousel`, `StackPanelBar`, `StackLoader`, `StackAnimationContainer`

### Navigation

`StackTabs`, `StackBreadcrumb`, `StackPagination`, `StackSidebar`, `StackAppBar`, `StackMenubar`, `StackDropdownMenu`, `StackContextMenu`, `StackToggleButton`, `StackToolbar`, `StackStepper`, `StackWizard`, `StackWorkflow`

### Overlays

`StackDialog`, `StackAlertDialog`, `StackSheet`, `StackDrawer`, `StackPopover`, `StackPopup`, `StackTooltip`, `StackHoverCard`, `StackWindow`, `StackToaster`, `StackImageViewer`, `StackFileViewer`

### Data

`StackAlert`, `StackTable`, `StackDataTable`, `StackAdvancedTable`, `StackPivotGrid`, `StackTreeView`, `StackTreeList`, `StackListView`, `StackFileManager`, `StackMsVirtualList`, `StackMsCalendar`, `StackMsPivotGrid`

### Charts

`StackLineChart`, `StackBarChart`, `StackPieChart`, `StackGaugeChart`

### Blocks and Templates

Reusable page blocks and full templates exist under `src/MyStackBlazor/Components/Blocks` and `src/MyStackBlazor/Components/Templates`.

For a detailed catalog and notes, see `COMPONENTS.md`.

## Services

### ThemeService

Manages light/dark/system theme state.

### ToastService

Global notifications via `StackToaster` + `ToastService`:

```csharp
@inject ToastService Toast

Toast.Show("Saved successfully!", ToastType.Success);
Toast.Show("Something went wrong.", ToastType.Error);
```

## MAUI Blazor Hybrid

```csharp
builder.Services.AddMauiBlazorWebView();
builder.Services.AddMyStackBlazor();
```

Use the same `_content/MyStackBlazor/...` CSS/JS asset references in your MAUI web root.

## Modular Packages

- `MyStackBlazor` (main package, 192 components)
- `MyStackBlazor.Core`
- `MyStackBlazor.DataGrid`
- `MyStackBlazor.Calendar`
- `MyStackBlazor.Accessibility`
- `MyStackBlazor.Localization`
- `MyStackBlazor.Security`

Install any add-on package with:

```bash
dotnet add package MyStackBlazor.DataGrid
```

## Reliability and security

- Theme state is scoped per Blazor Server circuit. Keep `AddMyStackBlazor()` registrations scoped when customizing DI.
- `StackDialog` traps keyboard focus, restores focus on close, and closes on Escape by default. Use `AriaLabelledBy` to reference a visible title (or supply `AriaLabel`), and `AriaDescribedBy` for descriptive text. Set `CloseOnEscape="false"` for flows that require an explicit decision.
- `StackFileUpload` enforces `MaxFileSizeBytes` and the cumulative `MaxFiles` limit. Rejections appear inline and are available through `OnRejected`. Validate size, content, authorization, and storage names on the server as well; `Accept` is a browser hint.
- CSV exports protect against spreadsheet formulas by default. The three-argument `ExportToCsv(..., spreadsheetSafe: false)` overload is only for trusted machine-to-machine data. JSON exports preserve native numbers, booleans, and nulls.

## Local Development

```bash
# Restore
dotnet restore

# Build library
dotnet build src/MyStackBlazor/MyStackBlazor.csproj

# Build demo
dotnet build samples/MyStackBlazor.Demo/MyStackBlazor.Demo.csproj
```

Tailwind rebuild from `src/MyStackBlazor`:

```bash
npm ci
npm run build:css
```

Pack NuGet:

```powershell
./pack.ps1
```

## Validation and releases

CI builds CSS from `package-lock.json`, builds the solution, runs unit and Chromium browser tests, and checks the seven packed libraries and their internal dependency versions, and compiles a clean Razor consumer from those packages. Releases reuse this workflow and check out the exact release tag; the publishing job downloads the validated package artifact rather than rebuilding it.

NuGet Trusted Publishing must authorize the `ci.yml` publishing workflow. For the Release Please path, ensure your NuGet publishing policy also permits the reusable-workflow caller (`release-please.yml`) as required by your policy. OIDC authentication is verified when a release runs; no credentials belong in this repository.

The sanitizer tests run in a separate project so bUnit’s DOM parser dependency does not constrain the production sanitizer.

Run unit tests:

```bash
dotnet test tests/MyStackBlazor.UnitTests -c Release
dotnet test tests/MyStackBlazor.Security.Tests -c Release
```

Run browser tests after building the solution (PowerShell is required for browser installation):

```powershell
./tests/MyStackBlazor.E2ETests/bin/Release/net10.0/playwright.ps1 install chromium
# Start the demo in a separate terminal:
dotnet run --project samples/MyStackBlazor.Demo -c Release --no-launch-profile --urls http://127.0.0.1:5080
# Then:
dotnet test tests/MyStackBlazor.E2ETests -c Release
```

The browser tests use `http://127.0.0.1:5080/overlays` by default. Set `DEMO_BASE_URL` to the full overlays page URL to test another host.

## Contributing

1. Fork the repo
2. Create a branch: `git checkout -b feat/my-change`
3. Commit your changes
4. Open a pull request

## License

MIT (c) 2025 Vilas Sagar
