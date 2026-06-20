<div align="center">

# MyStackBlazor

A modern Blazor component library powered by Tailwind CSS v4.

[![NuGet](https://img.shields.io/nuget/v/MyStackBlazor?color=512BD4&logo=nuget&label=NuGet)](https://www.nuget.org/packages/MyStackBlazor)
[![NuGet Downloads](https://img.shields.io/nuget/dt/MyStackBlazor?color=512BD4&logo=nuget)](https://www.nuget.org/packages/MyStackBlazor)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com)
[![Tailwind CSS v4](https://img.shields.io/badge/Tailwind%20CSS-v4-06B6D4?logo=tailwindcss)](https://tailwindcss.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

192 components - Blazor WASM / Server / MAUI Hybrid - Light / Dark / System - Zero external JS dependencies

[Live Demo](https://vilassagar.github.io/MyStackBlazor/)

</div>

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
<PackageReference Include="MyStackBlazor" Version="1.2.2" />
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

`StackCard`, `StackAccordion`, `StackCollapsible`, `StackScrollArea`, `StackAspectRatio`, `StackGridLayout`, `StackStackLayout`, `StackSplitter`, `StackCarousel`, `StackPanelBar`, `StackLoader`, `StackAnimationContainer`

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
- `MyStackBlazor.Accessibility`
- `MyStackBlazor.Localization`
- `MyStackBlazor.Security`

Install any add-on package with:

```bash
dotnet add package MyStackBlazor.DataGrid
```

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
npm install
npx @tailwindcss/cli -i ./tailwind/input.css -o ./wwwroot/css/mystackblazor.css --minify
```

Pack NuGet:

```powershell
./pack.ps1
```

## Contributing

1. Fork the repo
2. Create a branch: `git checkout -b feat/my-change`
3. Commit your changes
4. Open a pull request

## License

MIT (c) 2025 Vilas Sagar
