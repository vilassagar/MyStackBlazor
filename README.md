<div align="center">

# MyStackBlazor

**A modern, production-ready Blazor component library powered by Tailwind CSS v4**

[![NuGet](https://img.shields.io/nuget/v/MyStackBlazor?color=512BD4&logo=nuget&label=NuGet)](https://www.nuget.org/packages/MyStackBlazor)
[![NuGet Downloads](https://img.shields.io/nuget/dt/MyStackBlazor?color=512BD4&logo=nuget)](https://www.nuget.org/packages/MyStackBlazor)
[![.NET 10](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com)
[![Tailwind CSS v4](https://img.shields.io/badge/Tailwind%20CSS-v4-06B6D4?logo=tailwindcss)](https://tailwindcss.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

**166 components** · **Blazor WASM / Server / MAUI Hybrid** · **Light · Dark · System theme** · **Zero external JS deps**

</div>

---

## Table of Contents

- [Overview](#overview)
- [Installation](#installation)
- [Setup](#setup)
  - [1 · Register Services](#1--register-services)
  - [2 · Add Assets](#2--add-assets)
  - [3 · Global Imports](#3--global-imports)
  - [4 · Wrap Root Layout](#4--wrap-root-layout)
- [Theming](#theming)
- [MAUI Blazor Hybrid](#maui-blazor-hybrid)
- [Common Components](#common-components)
  - [Button](#button)
  - [Badge](#badge)
  - [Label](#label)
  - [Separator](#separator)
  - [Skeleton](#skeleton)
  - [Spinner](#spinner)
  - [Avatar](#avatar)
  - [Chip · ChipList](#chip--chiplist)
  - [Icon](#icon)
  - [Typography](#typography)
  - [ThemeToggle](#themetoggle)
- [Form Components](#form-components)
  - [Input](#input)
  - [Textarea](#textarea)
  - [FloatingLabel](#floatinglabel)
  - [Checkbox](#checkbox)
  - [Switch](#switch)
  - [RadioGroup](#radiogroup)
  - [Select](#select)
  - [Combobox](#combobox)
  - [MultiSelect](#multiselect)
  - [AutoComplete](#autocomplete)
  - [ListBox](#listbox)
  - [Slider](#slider)
  - [RangeSlider](#rangeslider)
  - [NumericInput](#numericinput)
  - [Rating](#rating)
  - [DatePicker](#datepicker)
  - [DateRangePicker](#daterangepicker)
  - [DateTimePicker](#datetimepicker)
  - [TimePicker](#timepicker)
  - [InputOtp](#inputotp)
  - [ColorPicker](#colorpicker)
  - [ColorPalette](#colorpalette)
  - [MaskedTextBox](#maskedtextbox)
  - [MultiColumnComboBox](#multicolumncombobox)
  - [DropDownTree](#dropdowntree)
  - [MarkdownEditor](#markdowneditor)
  - [FileUpload](#fileupload)
  - [FileDownload](#filedownload)
  - [FormField](#formfield)
- [Layout Components](#layout-components)
  - [Card](#card)
  - [Accordion](#accordion)
  - [Collapsible](#collapsible)
  - [ScrollArea](#scrollarea)
  - [AspectRatio](#aspectratio)
  - [GridLayout](#gridlayout)
  - [StackLayout](#stacklayout)
  - [Splitter](#splitter)
  - [Carousel](#carousel)
  - [PanelBar](#panelbar)
  - [AnimationContainer](#animationcontainer)
  - [Loader](#loader)
  - [Progress · ChunkProgressBar](#progress--chunkprogressbar)
- [Navigation Components](#navigation-components)
  - [Tabs](#tabs)
  - [Breadcrumb](#breadcrumb)
  - [Pagination](#pagination)
  - [Sidebar](#sidebar)
  - [AppBar](#appbar)
  - [Menubar](#menubar)
  - [DropdownMenu](#dropdownmenu)
  - [ContextMenu](#contextmenu)
  - [DropDownButton · SplitButton](#dropdownbutton--splitbutton)
  - [ButtonGroup · ToggleButton](#buttongroup--togglebutton)
  - [FloatingActionButton](#floatingactionbutton)
  - [Toolbar](#toolbar)
  - [Stepper](#stepper)
  - [Wizard](#wizard)
  - [Workflow](#workflow)
- [Overlay Components](#overlay-components)
  - [Dialog](#dialog)
  - [AlertDialog](#alertdialog)
  - [Sheet](#sheet)
  - [Drawer](#drawer)
  - [Popup · Popover](#popup--popover)
  - [Tooltip](#tooltip)
  - [HoverCard](#hovercard)
  - [Toaster](#toaster)
  - [Window](#window)
  - [ImageViewer · FileViewer](#imageviewer--fileviewer)
- [Data Components](#data-components)
  - [Alert](#alert)
  - [Table](#table)
  - [DataTable](#datatable)
  - [AdvancedTable](#advancedtable)
  - [PivotGrid](#pivotgrid)
  - [TreeView](#treeview)
  - [TreeList](#treelist)
  - [ListView](#listview)
  - [FileManager](#filemanager)
- [Chart Components](#chart-components)
  - [LineChart](#linechart)
  - [BarChart](#barchart)
  - [PieChart](#piechart)
  - [GaugeChart](#gaugechart)
- [Blocks](#blocks)
  - [Dashboard Blocks](#dashboard-blocks)
  - [Landing Page Blocks](#landing-page-blocks)
  - [Listing Blocks](#listing-blocks)
- [Templates](#templates)
  - [Dashboard Templates](#dashboard-templates)
  - [Landing Page Templates](#landing-page-templates)
  - [Listing Templates](#listing-templates)
- [Services](#services)
- [Modular Packages](#modular-packages)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

MyStackBlazor is a **comprehensive component library** for .NET Blazor applications. It ships 166 ready-to-use Razor components organized across 9 functional categories, all styled with **Tailwind CSS v4** and built with zero external JavaScript dependencies.

| Feature | Detail |
|---------|--------|
| **Framework** | Blazor WASM · Blazor Server · .NET MAUI Blazor Hybrid |
| **.NET** | .NET 10 |
| **Styling** | Tailwind CSS v4 (pre-compiled, zero config needed) |
| **Icons** | 150+ Heroicons v2 (bundled SVG, no icon font) |
| **Theming** | Light / Dark / System (injectable `ThemeService`) |
| **Accessibility** | ARIA labels · keyboard navigation · screen reader support |
| **Components** | 166 across Common, Form, Layout, Navigation, Overlay, Data, Charts, Blocks, Templates |
| **License** | MIT |

---

## Installation

```bash
dotnet add package MyStackBlazor
```

Or add via the NuGet Package Manager:

```xml
<PackageReference Include="MyStackBlazor" Version="1.0.1" />
```

---

## Setup

### 1 · Register Services

In `Program.cs` (Blazor WASM or Server):

```csharp
builder.Services.AddMyStackBlazor();
```

### 2 · Add Assets

In `wwwroot/index.html` (WASM) or `Pages/_Host.cshtml` / `App.razor` (Server):

```html
<link rel="stylesheet" href="_content/MyStackBlazor/css/mystackblazor.css" />
<script src="_content/MyStackBlazor/js/mystackblazor.js"></script>
```

### 3 · Global Imports

Add to `_Imports.razor`:

```razor
@using MyStackBlazor
@using MyStackBlazor.Components.Common
@using MyStackBlazor.Components.Form
@using MyStackBlazor.Components.Navigation
@using MyStackBlazor.Components.Layout
@using MyStackBlazor.Components.Overlays
@using MyStackBlazor.Components.Data
@using MyStackBlazor.Components.Charts
```

### 4 · Wrap Root Layout

In `MainLayout.razor`:

```razor
@inherits LayoutComponentBase

<ThemeProvider>
    <div class="min-h-screen bg-background text-foreground">
        <header>
            <ThemeToggle />
        </header>
        <main>
            <Toaster />
            @Body
        </main>
    </div>
</ThemeProvider>
```

---

## Theming

`ThemeProvider` applies a `.dark` CSS class to switch all Tailwind design tokens automatically. The active theme is tracked by the injectable `ThemeService`.

```csharp
@inject ThemeService ThemeService

// Set a specific theme
ThemeService.SetTheme(AppTheme.Light);
ThemeService.SetTheme(AppTheme.Dark);
ThemeService.SetTheme(AppTheme.System);  // follows OS preference

// Toggle light ↔ dark
ThemeService.Toggle();

// Read current state
bool isDark = ThemeService.IsDark;

// Subscribe to changes
ThemeService.OnThemeChanged += () => InvokeAsync(StateHasChanged);
```

---

## MAUI Blazor Hybrid

```csharp
// MauiProgram.cs
builder.Services.AddMauiBlazorWebView();
builder.Services.AddMyStackBlazor();
```

Add the same `<link>` and `<script>` tags to `wwwroot/index.html`.

> **Note:** Theme persistence uses `localStorage`. For native preferences fallback, subscribe to `ThemeService.OnThemeChanged` and persist via `Microsoft.Maui.Storage.Preferences`.

---

## Common Components

### Button

Styled button with variants, sizes, loading state, and disabled state.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Variant` | `string` | `"default"` | `default` · `destructive` · `outline` · `secondary` · `ghost` · `link` |
| `Size` | `string` | `"default"` | `default` · `sm` · `lg` · `icon` |
| `Disabled` | `bool` | `false` | Disables the button |
| `IsLoading` | `bool` | `false` | Shows spinner and blocks clicks |
| `Type` | `string` | `"button"` | HTML type: `button` · `submit` · `reset` |
| `Class` | `string` | — | Extra CSS classes |
| `OnClick` | `EventCallback<MouseEventArgs>` | — | Click handler |

```razor
<Button>Default</Button>
<Button Variant="destructive">Delete</Button>
<Button Variant="outline">Cancel</Button>
<Button Variant="secondary">Secondary</Button>
<Button Variant="ghost">Ghost</Button>
<Button Variant="link">Link</Button>

<Button Size="sm">Small</Button>
<Button Size="lg">Large</Button>

<Button IsLoading="true">Saving…</Button>
<Button Disabled="true">Disabled</Button>

<Button Type="submit" OnClick="Save">Save Changes</Button>

@code {
    async Task Save(MouseEventArgs e) { /* ... */ }
}
```

---

### Badge

Compact inline label for status, category, or metadata.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Variant` | `string` | `"default"` | `default` · `secondary` · `destructive` · `outline` |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Badge>New</Badge>
<Badge Variant="secondary">Beta</Badge>
<Badge Variant="destructive">Error</Badge>
<Badge Variant="outline">Draft</Badge>
```

---

### Label

Accessible `<label>` element for associating with form inputs.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `For` | `string` | — | Matches the `id` of the target input |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Label For="email">Email address</Label>
<Input Id="email" Type="email" Placeholder="you@example.com" />
```

---

### Separator

Horizontal or vertical visual divider, optionally with label content.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Orientation` | `string` | `"horizontal"` | `horizontal` · `vertical` |
| `Decorative` | `bool` | `true` | When `true`, hidden from screen readers |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Separator />

<Separator Orientation="vertical" Class="h-6" />

<Separator>
    <span class="px-2 text-muted-foreground text-sm">OR</span>
</Separator>
```

---

### Skeleton

Animated pulse placeholder for content that is still loading.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string` | — | Controls shape and size |

```razor
<Skeleton Class="h-4 w-48 rounded" />
<Skeleton Class="h-10 w-full rounded-md" />
<Skeleton Class="h-12 w-12 rounded-full" />

@* Card skeleton *@
<div class="flex gap-4">
    <Skeleton Class="h-12 w-12 rounded-full" />
    <div class="flex flex-col gap-2">
        <Skeleton Class="h-4 w-32" />
        <Skeleton Class="h-4 w-24" />
    </div>
</div>
```

---

### Spinner

Inline loading spinner for use inside buttons, forms, or panels.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Size` | `string` | `"default"` | `sm` · `default` · `lg` |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Spinner />
<Spinner Size="sm" />
<Spinner Size="lg" Class="text-primary" />
```

---

### Avatar

User avatar with image source and initials fallback.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Src` | `string` | — | Image URL |
| `Alt` | `string` | — | Alt text |
| `Fallback` | `string` | — | Initials shown when image fails |
| `Size` | `string` | `"default"` | `sm` · `default` · `lg` |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Avatar Src="/avatar.jpg" Alt="Jane Doe" />
<Avatar Fallback="JD" />
<Avatar Src="/broken.jpg" Fallback="VS" Size="lg" />
```

---

### Chip · ChipList

Removable tag/pill element. `ChipList<TItem>` manages a typed collection.

| Parameter (Chip) | Type | Default | Description |
|------------------|------|---------|-------------|
| `Label` | `string` | — | Display text |
| `OnRemove` | `EventCallback` | — | Fires when × is clicked |
| `Class` | `string` | — | Extra CSS classes |

| Parameter (ChipList) | Type | Default | Description |
|----------------------|------|---------|-------------|
| `Items` | `List<TItem>` | — | Bound item list |
| `GetLabel` | `Func<TItem, string>` | — | Label selector |
| `OnRemove` | `EventCallback<TItem>` | — | Fires on item removal |

```razor
<Chip Label="Blazor" OnRemove="() => Console.WriteLine("removed")" />

<ChipList TItem="string"
          Items="_tags"
          GetLabel="t => t"
          OnRemove="t => _tags.Remove(t)" />

@code {
    List<string> _tags = ["Blazor", ".NET", "Tailwind"];
}
```

---

### Icon

SVG icon from the bundled Heroicons v2 set (150+ icons).

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Name` | `string` | — | Icon name, e.g. `"home"`, `"user"`, `"cog-6-tooth"` |
| `Size` | `int` | `24` | Width and height in pixels |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Icon Name="home" />
<Icon Name="user" Size="20" />
<Icon Name="cog-6-tooth" Class="text-muted-foreground" />
<Icon Name="check-circle" Size="32" Class="text-green-500" />
```

---

### Typography

Semantic text elements with consistent design-token styling.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Variant` | `string` | `"p"` | `h1` · `h2` · `h3` · `h4` · `p` · `lead` · `muted` · `blockquote` · `code` · `small` · `large` |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Typography Variant="h1">Page Title</Typography>
<Typography Variant="h2">Section Heading</Typography>
<Typography Variant="lead">Introductory paragraph text.</Typography>
<Typography Variant="p">Body paragraph.</Typography>
<Typography Variant="muted">Supporting caption text.</Typography>
<Typography Variant="code">console.log("hello")</Typography>
```

---

### ThemeToggle

Pre-built button that toggles between light and dark mode.

```razor
<ThemeToggle />
```

---

## Form Components

### Input

Text input with two-way binding, type control, and placeholder.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string` | — | Bound value |
| `ValueChanged` | `EventCallback<string>` | — | Two-way binding callback |
| `Type` | `string` | `"text"` | `text` · `email` · `password` · `number` · `search` · `url` |
| `Placeholder` | `string` | — | Placeholder text |
| `Disabled` | `bool` | `false` | Disables the input |
| `Id` | `string` | — | HTML id attribute |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Input @bind-Value="_email" Type="email" Placeholder="you@example.com" />
<Input @bind-Value="_search" Type="search" Placeholder="Search…" />
<Input @bind-Value="_pwd" Type="password" />

@code {
    string _email = "";
    string _search = "";
    string _pwd = "";
}
```

---

### Textarea

Multi-line text input.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string` | — | Bound value |
| `ValueChanged` | `EventCallback<string>` | — | Two-way binding callback |
| `Rows` | `int` | `3` | Visible row count |
| `Placeholder` | `string` | — | Placeholder text |
| `Disabled` | `bool` | `false` | Disables the textarea |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Textarea @bind-Value="_notes" Rows="5" Placeholder="Add your notes…" />
```

---

### FloatingLabel

CSS peer-based animated floating label — the label rises when the input has focus or value.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Id` | `string` | — | Links label to input |
| `Label` | `string` | — | Label text |
| `Value` | `string` | — | Bound value |
| `ValueChanged` | `EventCallback<string>` | — | Two-way binding callback |
| `Type` | `string` | `"text"` | Input type |
| `Class` | `string` | — | Extra CSS classes |

```razor
<FloatingLabel Id="fname" Label="First Name" @bind-Value="_firstName" />
<FloatingLabel Id="email" Label="Email" @bind-Value="_email" Type="email" />
```

---

### Checkbox

Accessible checkbox with optional inline label.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Checked` | `bool` | `false` | Bound checked state |
| `CheckedChanged` | `EventCallback<bool>` | — | Two-way binding callback |
| `Label` | `string` | — | Inline label text |
| `Disabled` | `bool` | `false` | Disables the checkbox |
| `Id` | `string` | — | HTML id |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Checkbox @bind-Checked="_agree" Label="I agree to the terms" />

@code {
    bool _agree;
}
```

---

### Switch

Toggle on/off switch control.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Checked` | `bool` | `false` | Bound state |
| `CheckedChanged` | `EventCallback<bool>` | — | Two-way binding callback |
| `Label` | `string` | — | Accessible label |
| `Disabled` | `bool` | `false` | Disables the switch |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Switch @bind-Checked="_notify" Label="Enable notifications" />
```

---

### RadioGroup

Mutually exclusive radio button group.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string` | — | Bound selected value |
| `ValueChanged` | `EventCallback<string>` | — | Two-way binding callback |
| `Orientation` | `string` | `"vertical"` | `vertical` · `horizontal` |
| `Class` | `string` | — | Extra CSS classes |

```razor
<RadioGroup @bind-Value="_plan" Orientation="horizontal">
    <RadioGroupItem Value="free" Label="Free" />
    <RadioGroupItem Value="pro" Label="Pro" />
    <RadioGroupItem Value="enterprise" Label="Enterprise" />
</RadioGroup>

@code {
    string _plan = "free";
}
```

---

### Select

Native HTML `<select>` dropdown styled with design tokens.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string` | — | Bound selected value |
| `ValueChanged` | `EventCallback<string>` | — | Two-way binding callback |
| `Placeholder` | `string` | — | Placeholder option text |
| `Disabled` | `bool` | `false` | Disables the select |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Select @bind-Value="_country" Placeholder="Select country">
    <option value="us">United States</option>
    <option value="uk">United Kingdom</option>
    <option value="ca">Canada</option>
</Select>
```

---

### Combobox

Searchable single-select dropdown with custom option list.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Items` | `List<string>` | — | Options list |
| `Value` | `string` | — | Bound value |
| `ValueChanged` | `EventCallback<string>` | — | Two-way binding callback |
| `Placeholder` | `string` | — | Placeholder text |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Combobox Items="_frameworks"
          @bind-Value="_selected"
          Placeholder="Select framework…" />

@code {
    string _selected = "";
    List<string> _frameworks = ["Blazor", "React", "Vue", "Angular"];
}
```

---

### MultiSelect

Tag-style selector for multiple values with search.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Items` | `List<string>` | — | Available options |
| `SelectedItems` | `List<string>` | — | Bound selected values |
| `SelectedItemsChanged` | `EventCallback<List<string>>` | — | Two-way binding callback |
| `Placeholder` | `string` | — | Placeholder text |
| `Class` | `string` | — | Extra CSS classes |

```razor
<MultiSelect Items="_skills"
             @bind-SelectedItems="_chosen"
             Placeholder="Select skills…" />

@code {
    List<string> _chosen = [];
    List<string> _skills = ["C#", "Blazor", "TypeScript", "SQL", "Docker"];
}
```

---

### AutoComplete

Type-ahead search input with async data loading support.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Items` | `List<string>` | — | Static suggestions |
| `SearchFunc` | `Func<string, Task<List<string>>>` | — | Async search function |
| `Value` | `string` | — | Bound value |
| `ValueChanged` | `EventCallback<string>` | — | Two-way binding callback |
| `Placeholder` | `string` | — | Placeholder text |
| `MinLength` | `int` | `1` | Minimum chars before suggestions appear |

```razor
@* Static list *@
<AutoComplete Items="_cities" @bind-Value="_city" Placeholder="City…" />

@* Async search *@
<AutoComplete SearchFunc="SearchUsers" @bind-Value="_user" Placeholder="Search users…" />

@code {
    string _city = "";
    string _user = "";
    List<string> _cities = ["London", "New York", "Tokyo", "Sydney"];

    async Task<List<string>> SearchUsers(string query)
        => await _api.SearchUsersAsync(query);
}
```

---

### ListBox

Scrollable list with single or multi-select support.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Items` | `List<string>` | — | Display items |
| `SelectedItem` | `string` | — | Single selection (bound) |
| `SelectedItems` | `List<string>` | — | Multi-selection (bound) |
| `MultiSelect` | `bool` | `false` | Enable multi-select |
| `Class` | `string` | — | Extra CSS classes |

```razor
<ListBox Items="_options" @bind-SelectedItem="_pick" />

<ListBox Items="_options"
         @bind-SelectedItems="_picks"
         MultiSelect="true" />

@code {
    string _pick = "";
    List<string> _picks = [];
    List<string> _options = ["Alpha", "Beta", "Gamma", "Delta"];
}
```

---

### Slider

Single-handle range slider.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `double` | `0` | Bound value |
| `ValueChanged` | `EventCallback<double>` | — | Two-way binding callback |
| `Min` | `double` | `0` | Minimum value |
| `Max` | `double` | `100` | Maximum value |
| `Step` | `double` | `1` | Increment step |
| `Disabled` | `bool` | `false` | Disables the slider |

```razor
<Slider @bind-Value="_volume" Min="0" Max="100" Step="5" />
<p>Volume: @_volume</p>

@code {
    double _volume = 50;
}
```

---

### RangeSlider

Dual-handle slider for selecting a numeric range.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `MinValue` | `double` | `0` | Lower handle value (bound) |
| `MaxValue` | `double` | `100` | Upper handle value (bound) |
| `Min` | `double` | `0` | Minimum boundary |
| `Max` | `double` | `100` | Maximum boundary |
| `Step` | `double` | `1` | Increment step |
| `MinValueChanged` | `EventCallback<double>` | — | Lower bound callback |
| `MaxValueChanged` | `EventCallback<double>` | — | Upper bound callback |

```razor
<RangeSlider @bind-MinValue="_minPrice"
             @bind-MaxValue="_maxPrice"
             Min="0" Max="1000" Step="10" />

<p>Range: @_minPrice – @_maxPrice</p>

@code {
    double _minPrice = 100;
    double _maxPrice = 800;
}
```

---

### NumericInput

Number input with stepper increment/decrement buttons and min/max/step constraints.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `double` | `0` | Bound value |
| `ValueChanged` | `EventCallback<double>` | — | Two-way binding callback |
| `Min` | `double?` | — | Minimum value |
| `Max` | `double?` | — | Maximum value |
| `Step` | `double` | `1` | Increment step |
| `Disabled` | `bool` | `false` | Disables the control |
| `Class` | `string` | — | Extra CSS classes |

```razor
<NumericInput @bind-Value="_qty" Min="1" Max="99" Step="1" />
```

---

### Rating

Star rating input with configurable maximum stars.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `int` | `0` | Bound rating value |
| `ValueChanged` | `EventCallback<int>` | — | Two-way binding callback |
| `Max` | `int` | `5` | Maximum number of stars |
| `ReadOnly` | `bool` | `false` | Display-only mode |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Rating @bind-Value="_stars" Max="5" />
<Rating Value="4" ReadOnly="true" />
```

---

### DatePicker

Calendar popover date selector.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `DateTime?` | — | Bound date |
| `ValueChanged` | `EventCallback<DateTime?>` | — | Two-way binding callback |
| `Placeholder` | `string` | `"Pick a date"` | Input placeholder |
| `Format` | `string` | `"yyyy-MM-dd"` | Display format |
| `Disabled` | `bool` | `false` | Disables the picker |
| `Class` | `string` | — | Extra CSS classes |

```razor
<DatePicker @bind-Value="_date" Placeholder="Select date" />

@code {
    DateTime? _date;
}
```

---

### DateRangePicker

Two-calendar picker for a start + end date range with range highlighting.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `StartDate` | `DateTime?` | — | Range start (bound) |
| `EndDate` | `DateTime?` | — | Range end (bound) |
| `StartDateChanged` | `EventCallback<DateTime?>` | — | Start callback |
| `EndDateChanged` | `EventCallback<DateTime?>` | — | End callback |
| `Placeholder` | `string` | — | Placeholder text |
| `Class` | `string` | — | Extra CSS classes |

```razor
<DateRangePicker @bind-StartDate="_from"
                 @bind-EndDate="_to"
                 Placeholder="Select date range" />

@code {
    DateTime? _from;
    DateTime? _to;
}
```

---

### DateTimePicker

Combined calendar + time selector in a single popover.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `DateTime?` | — | Bound datetime value |
| `ValueChanged` | `EventCallback<DateTime?>` | — | Two-way binding callback |
| `Placeholder` | `string` | — | Placeholder text |
| `Class` | `string` | — | Extra CSS classes |

```razor
<DateTimePicker @bind-Value="_meeting" Placeholder="Schedule meeting" />
```

---

### TimePicker

Hour/minute time selector. Available in 12-hour and 24-hour variants.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `TimeSpan?` | — | Bound time value |
| `ValueChanged` | `EventCallback<TimeSpan?>` | — | Two-way binding callback |
| `Is24Hour` | `bool` | `false` | Use 24-hour format |
| `Placeholder` | `string` | — | Placeholder text |

```razor
@* 12-hour *@
<TimePicker @bind-Value="_time" Placeholder="Select time" />

@* 24-hour *@
<TimePicker @bind-Value="_time" Is24Hour="true" />

@code {
    TimeSpan? _time;
}
```

---

### InputOtp

One-time password digit-box input (6 digits by default, configurable grouping).

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string` | — | Bound OTP value |
| `ValueChanged` | `EventCallback<string>` | — | Two-way binding callback |
| `Length` | `int` | `6` | Number of digit boxes |
| `GroupSize` | `int` | `3` | Digits per group (adds spacer) |
| `OnComplete` | `EventCallback<string>` | — | Fires when all digits are filled |

```razor
<InputOtp @bind-Value="_otp" OnComplete="VerifyOtp" />

@code {
    string _otp = "";

    async Task VerifyOtp(string code)
    {
        var valid = await _authService.VerifyAsync(code);
    }
}
```

---

### ColorPicker

HSL/HEX color wheel picker with optional alpha channel.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string` | `"#000000"` | Bound hex color |
| `ValueChanged` | `EventCallback<string>` | — | Two-way binding callback |
| `ShowAlpha` | `bool` | `false` | Show alpha channel slider |
| `Class` | `string` | — | Extra CSS classes |

```razor
<ColorPicker @bind-Value="_color" ShowAlpha="true" />
<div class="h-8 w-full rounded" style="background: @_color"></div>

@code {
    string _color = "#3B82F6";
}
```

---

### ColorPalette

Clickable swatch-grid color picker with 28 default colors.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string` | — | Bound selected color |
| `ValueChanged` | `EventCallback<string>` | — | Two-way binding callback |
| `Colors` | `List<string>` | 28 defaults | Custom swatch colors |
| `Class` | `string` | — | Extra CSS classes |

```razor
<ColorPalette @bind-Value="_accent" />
```

---

### MaskedTextBox

Input with pattern-based masking: `0` = digit, `a` = letter, `*` = any character.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string` | — | Bound unmasked value |
| `ValueChanged` | `EventCallback<string>` | — | Two-way binding callback |
| `Mask` | `string` | — | Mask pattern |
| `Placeholder` | `string` | — | Placeholder text |

```razor
<MaskedTextBox @bind-Value="_phone" Mask="(000) 000-0000" Placeholder="Phone number" />
<MaskedTextBox @bind-Value="_zip" Mask="00000-0000" />
<MaskedTextBox @bind-Value="_date" Mask="00/00/0000" />
```

---

### MultiColumnComboBox

Searchable dropdown that displays results in a multi-column grid.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Items` | `List<TItem>` | — | Data source |
| `Columns` | `List<ColumnDef>` | — | Column definitions |
| `Value` | `TItem` | — | Bound selected item |
| `ValueChanged` | `EventCallback<TItem>` | — | Two-way binding callback |
| `Placeholder` | `string` | — | Placeholder text |

```razor
<MultiColumnComboBox TItem="Product"
                     Items="_products"
                     Columns="_cols"
                     @bind-Value="_selected"
                     Placeholder="Search products…" />

@code {
    Product? _selected;
    List<Product> _products = [ /* ... */ ];
    List<ColumnDef> _cols =
    [
        new("SKU",  p => p.Sku),
        new("Name", p => p.Name),
        new("Price", p => p.Price.ToString("C")),
    ];
}
```

---

### DropDownTree

Hierarchical tree in a dropdown with expand/collapse and search.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Items` | `List<TreeNode>` | — | Root nodes |
| `Value` | `TreeNode` | — | Bound selected node |
| `ValueChanged` | `EventCallback<TreeNode>` | — | Two-way binding callback |
| `Placeholder` | `string` | — | Placeholder text |
| `GetChildren` | `Func<TreeNode, List<TreeNode>>` | — | Children selector |
| `GetLabel` | `Func<TreeNode, string>` | — | Label selector |

```razor
<DropDownTree Items="_tree"
              GetChildren="n => n.Children"
              GetLabel="n => n.Name"
              @bind-Value="_selected"
              Placeholder="Select category…" />
```

---

### MarkdownEditor

Rich Markdown editor with split edit/preview panes and 13 toolbar actions.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string` | — | Bound Markdown text |
| `ValueChanged` | `EventCallback<string>` | — | Two-way binding callback |
| `Height` | `string` | `"400px"` | Editor height |
| `Placeholder` | `string` | — | Placeholder text |
| `ShowPreview` | `bool` | `true` | Show live preview pane |

```razor
<MarkdownEditor @bind-Value="_content"
                Height="500px"
                Placeholder="Write your article…" />
```

---

### FileUpload

Drag-and-drop file uploader with multi-file support and image preview.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `OnFilesSelected` | `EventCallback<IReadOnlyList<IBrowserFile>>` | — | Fires when files are picked |
| `Accept` | `string` | — | Accepted MIME types, e.g. `"image/*"` |
| `Multiple` | `bool` | `false` | Allow multiple files |
| `MaxSize` | `long` | `10 MB` | Max file size in bytes |
| `ShowPreview` | `bool` | `true` | Show image previews |
| `Class` | `string` | — | Extra CSS classes |

```razor
<FileUpload Accept="image/*"
            Multiple="true"
            OnFilesSelected="HandleFiles" />

@code {
    async Task HandleFiles(IReadOnlyList<IBrowserFile> files)
    {
        foreach (var file in files)
            await UploadAsync(file);
    }
}
```

---

### FileDownload

Utility component to trigger a file download from a URL or byte array.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Url` | `string` | — | Download URL |
| `FileName` | `string` | — | Suggested file name |
| `Content` | `byte[]` | — | File bytes (alternative to URL) |
| `ContentType` | `string` | — | MIME type |

```razor
<FileDownload Url="/reports/monthly.pdf" FileName="monthly-report.pdf">
    <Button>Download Report</Button>
</FileDownload>
```

---

### FormField

Composes `Label` + input + validation error message into a single accessible unit.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Label` | `string` | — | Field label text |
| `For` | `string` | — | Input id |
| `Error` | `string` | — | Validation error message |
| `Required` | `bool` | `false` | Shows required asterisk |
| `Class` | `string` | — | Extra CSS classes |

```razor
<FormField Label="Email" For="email" Error="@_emailError" Required="true">
    <Input Id="email" @bind-Value="_email" Type="email" />
</FormField>
```

---

## Layout Components

### Card

Container with semantic header, content, and footer sub-components.

```razor
<Card>
    <CardHeader>
        <CardTitle>Account Summary</CardTitle>
        <CardDescription>Your current plan and usage.</CardDescription>
    </CardHeader>
    <CardContent>
        <p>Pro plan · 14 days remaining</p>
    </CardContent>
    <CardFooter>
        <Button Variant="outline">Manage Plan</Button>
    </CardFooter>
</Card>
```

---

### Accordion

Collapsible sections with single or multiple open mode.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Type` | `string` | `"single"` | `single` (one open) · `multiple` (many open) |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Accordion Type="single">
    <AccordionItem Value="item-1" Title="What is Blazor?">
        Blazor lets you build interactive web UIs using C# instead of JavaScript.
    </AccordionItem>
    <AccordionItem Value="item-2" Title="Is it production-ready?">
        Yes — Blazor has been production-ready since .NET 5.
    </AccordionItem>
</Accordion>
```

---

### Collapsible

Simple show/hide panel with a custom trigger element.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Open` | `bool` | `false` | Bound open state |
| `OpenChanged` | `EventCallback<bool>` | — | Two-way binding callback |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Collapsible @bind-Open="_open">
    <CollapsibleTrigger>
        <Button Variant="ghost">Toggle details</Button>
    </CollapsibleTrigger>
    <CollapsibleContent>
        <p class="mt-2 text-sm text-muted-foreground">Hidden content here.</p>
    </CollapsibleContent>
</Collapsible>
```

---

### ScrollArea

Custom scrollbar container with design-token styled scrollbars.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Height` | `string` | — | Fixed height, e.g. `"300px"` |
| `Class` | `string` | — | Extra CSS classes |

```razor
<ScrollArea Height="300px">
    @foreach (var item in _longList)
    {
        <div class="p-2 border-b">@item</div>
    }
</ScrollArea>
```

---

### AspectRatio

Constrains children to a fixed aspect ratio.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Ratio` | `double` | `1.777` | Width/height ratio (16/9 = 1.777) |
| `Class` | `string` | — | Extra CSS classes |

```razor
<AspectRatio Ratio="16.0/9">
    <img src="/hero.jpg" class="w-full h-full object-cover rounded-md" />
</AspectRatio>
```

---

### GridLayout

CSS Grid wrapper with column count and gap control.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Columns` | `int` | `3` | Number of columns |
| `Gap` | `string` | `"4"` | Tailwind gap class suffix |
| `Class` | `string` | — | Extra CSS classes |

```razor
<GridLayout Columns="3" Gap="6">
    <Card>Card 1</Card>
    <Card>Card 2</Card>
    <Card>Card 3</Card>
</GridLayout>
```

---

### StackLayout

Flexbox container for horizontal or vertical stacks with align and justify control.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Orientation` | `string` | `"vertical"` | `vertical` · `horizontal` |
| `Gap` | `string` | `"4"` | Tailwind gap class suffix |
| `Align` | `string` | `"start"` | `start` · `center` · `end` · `stretch` |
| `Justify` | `string` | `"start"` | `start` · `center` · `end` · `between` |
| `Class` | `string` | — | Extra CSS classes |

```razor
<StackLayout Orientation="horizontal" Gap="3" Align="center">
    <Icon Name="user" />
    <Typography>John Doe</Typography>
    <Badge>Admin</Badge>
</StackLayout>
```

---

### Splitter

Resizable split pane container. Children are `SplitterPane` components.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Orientation` | `string` | `"horizontal"` | `horizontal` · `vertical` |
| `Class` | `string` | — | Extra CSS classes |

| Parameter (SplitterPane) | Type | Default | Description |
|--------------------------|------|---------|-------------|
| `Size` | `string` | `"50%"` | Initial pane size |
| `Min` | `string` | `"100px"` | Minimum pane size |

```razor
<Splitter Orientation="horizontal" Class="h-96">
    <SplitterPane Size="30%" Min="150px">
        <p>Left pane</p>
    </SplitterPane>
    <SplitterPane>
        <p>Right pane</p>
    </SplitterPane>
</Splitter>
```

---

### Carousel

Image/card slideshow with prev/next arrows and dot indicators.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `AutoPlay` | `bool` | `false` | Auto-advance slides |
| `Interval` | `int` | `3000` | Auto-play interval in ms |
| `ShowDots` | `bool` | `true` | Show dot navigation |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Carousel AutoPlay="true" Interval="4000">
    <CarouselSlide>
        <img src="/slide1.jpg" class="w-full" />
    </CarouselSlide>
    <CarouselSlide>
        <img src="/slide2.jpg" class="w-full" />
    </CarouselSlide>
</Carousel>
```

---

### PanelBar

Accordion-style vertical navigation panel.

| Parameter (PanelBarItem) | Type | Default | Description |
|--------------------------|------|---------|-------------|
| `Title` | `string` | — | Panel header text |
| `Icon` | `string` | — | Heroicon name |
| `Expanded` | `bool` | `false` | Initially expanded |

```razor
<PanelBar>
    <PanelBarItem Title="Dashboard" Icon="home" Expanded="true">
        <a href="/dashboard">Overview</a>
        <a href="/analytics">Analytics</a>
    </PanelBarItem>
    <PanelBarItem Title="Settings" Icon="cog-6-tooth">
        <a href="/settings/profile">Profile</a>
        <a href="/settings/billing">Billing</a>
    </PanelBarItem>
</PanelBar>
```

---

### AnimationContainer

Wraps content and applies enter/exit CSS animation classes on show/hide.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Visible` | `bool` | `false` | Controls visibility and animation |
| `EnterClass` | `string` | `"animate-fade-in"` | CSS class on show |
| `ExitClass` | `string` | `"animate-fade-out"` | CSS class on hide |
| `Class` | `string` | — | Extra CSS classes |

```razor
<AnimationContainer Visible="_show" EnterClass="animate-slide-in">
    <Card>This card animates in!</Card>
</AnimationContainer>

<Button OnClick="() => _show = !_show">Toggle</Button>
```

---

### Loader

Full-page or contained loading overlay.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Visible` | `bool` | `false` | Show/hide the overlay |
| `FullPage` | `bool` | `false` | Cover the entire viewport |
| `Text` | `string` | — | Loading message |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Loader Visible="_loading" Text="Loading data…" />
<Loader Visible="_loading" FullPage="true" />
```

---

### Progress · ChunkProgressBar

`Progress` renders a linear progress bar. `ChunkProgressBar` displays step-based segmented progress.

| Parameter (Progress) | Type | Default | Description |
|----------------------|------|---------|-------------|
| `Value` | `double` | `0` | Current value (0–100) |
| `Max` | `double` | `100` | Maximum value |
| `ShowLabel` | `bool` | `false` | Show percentage label |
| `Class` | `string` | — | Extra CSS classes |

| Parameter (ChunkProgressBar) | Type | Default | Description |
|------------------------------|------|---------|-------------|
| `Steps` | `int` | `5` | Total number of steps |
| `CurrentStep` | `int` | `0` | Active step index |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Progress Value="72" ShowLabel="true" />

<ChunkProgressBar Steps="5" CurrentStep="3" />
```

---

## Navigation Components

### Tabs

Tabbed panel with trigger list and content panels.

```razor
<Tabs DefaultValue="account">
    <TabsList>
        <TabsTrigger Value="account">Account</TabsTrigger>
        <TabsTrigger Value="security">Security</TabsTrigger>
        <TabsTrigger Value="billing">Billing</TabsTrigger>
    </TabsList>
    <TabsContent Value="account">
        <p>Account settings here.</p>
    </TabsContent>
    <TabsContent Value="security">
        <p>Security settings here.</p>
    </TabsContent>
    <TabsContent Value="billing">
        <p>Billing settings here.</p>
    </TabsContent>
</Tabs>
```

---

### Breadcrumb

Navigation trail for hierarchical page structure.

```razor
<Breadcrumb>
    <BreadcrumbItem Href="/">Home</BreadcrumbItem>
    <BreadcrumbItem Href="/products">Products</BreadcrumbItem>
    <BreadcrumbItem>Details</BreadcrumbItem>
</Breadcrumb>
```

---

### Pagination

Page navigation with prev/next buttons and page number links.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `CurrentPage` | `int` | `1` | Active page number |
| `TotalPages` | `int` | — | Total page count |
| `OnPageChange` | `EventCallback<int>` | — | Fires when page changes |
| `MaxVisible` | `int` | `7` | Max visible page numbers |

```razor
<Pagination CurrentPage="_page"
            TotalPages="_total"
            OnPageChange="p => _page = p" />

@code {
    int _page = 1;
    int _total = 20;
}
```

---

### Sidebar

Collapsible side navigation with expand/collapse toggle.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Collapsed` | `bool` | `false` | Bound collapsed state |
| `CollapsedChanged` | `EventCallback<bool>` | — | Two-way binding callback |
| `Width` | `string` | `"240px"` | Expanded width |
| `CollapsedWidth` | `string` | `"64px"` | Collapsed width |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Sidebar @bind-Collapsed="_sidebarCollapsed">
    <nav class="flex flex-col gap-1 p-2">
        <a href="/dashboard" class="nav-link">Dashboard</a>
        <a href="/reports" class="nav-link">Reports</a>
    </nav>
</Sidebar>
```

---

### AppBar

Sticky top application bar with start, center, and end slot zones.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Sticky` | `bool` | `true` | Sticky top positioning |
| `Class` | `string` | — | Extra CSS classes |

```razor
<AppBar>
    <Start>
        <Icon Name="squares-2x2" />
        <span class="font-semibold ml-2">MyApp</span>
    </Start>
    <Center>
        <Input Type="search" Placeholder="Search…" Class="w-64" />
    </Center>
    <End>
        <ThemeToggle />
        <Avatar Fallback="VS" Size="sm" />
    </End>
</AppBar>
```

---

### Menubar

Horizontal application menu bar with keyboard-navigable submenus.

```razor
<Menubar>
    <MenubarMenu Label="File">
        <DropdownMenuItem>New</DropdownMenuItem>
        <DropdownMenuItem>Open</DropdownMenuItem>
        <Separator />
        <DropdownMenuItem>Save</DropdownMenuItem>
    </MenubarMenu>
    <MenubarMenu Label="Edit">
        <DropdownMenuItem>Undo</DropdownMenuItem>
        <DropdownMenuItem>Redo</DropdownMenuItem>
    </MenubarMenu>
</Menubar>
```

---

### DropdownMenu

Generic dropdown with a custom trigger element and menu items.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Align` | `string` | `"end"` | `start` · `center` · `end` |
| `Class` | `string` | — | Extra CSS classes |

```razor
<DropdownMenu>
    <DropdownMenuTrigger>
        <Button Variant="ghost" Size="icon"><Icon Name="ellipsis-vertical" /></Button>
    </DropdownMenuTrigger>
    <DropdownMenuContent>
        <DropdownMenuItem>Edit</DropdownMenuItem>
        <DropdownMenuItem>Duplicate</DropdownMenuItem>
        <Separator />
        <DropdownMenuItem Class="text-destructive">Delete</DropdownMenuItem>
    </DropdownMenuContent>
</DropdownMenu>
```

---

### ContextMenu

Right-click context menu.

```razor
<ContextMenu>
    <ContextMenuTrigger>
        <div class="border rounded p-8">Right-click me</div>
    </ContextMenuTrigger>
    <ContextMenuContent>
        <ContextMenuItem>Open</ContextMenuItem>
        <ContextMenuItem>Rename</ContextMenuItem>
        <ContextMenuItem Class="text-destructive">Delete</ContextMenuItem>
    </ContextMenuContent>
</ContextMenu>
```

---

### DropDownButton · SplitButton

`DropDownButton` is a button that opens a dropdown on click. `SplitButton` has a primary action button and a separate arrow to open the dropdown.

```razor
<DropDownButton Label="Actions">
    <DropdownMenuItem>Export CSV</DropdownMenuItem>
    <DropdownMenuItem>Export PDF</DropdownMenuItem>
</DropDownButton>

<SplitButton Label="Save" OnPrimaryClick="Save">
    <DropdownMenuItem>Save as draft</DropdownMenuItem>
    <DropdownMenuItem>Save and publish</DropdownMenuItem>
</SplitButton>
```

---

### ButtonGroup · ToggleButton

`ButtonGroup` renders a row of joined buttons. `ToggleButton` is a pressed/unpressed binary button.

```razor
<ButtonGroup>
    <Button Variant="outline" Size="sm">Bold</Button>
    <Button Variant="outline" Size="sm">Italic</Button>
    <Button Variant="outline" Size="sm">Underline</Button>
</ButtonGroup>

<ToggleButton @bind-Pressed="_pinned">
    <Icon Name="bookmark" />
</ToggleButton>
```

---

### FloatingActionButton

Fixed circular FAB with 4 positioning options and 3 sizes.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Position` | `string` | `"bottom-right"` | `top-left` · `top-right` · `bottom-left` · `bottom-right` |
| `Size` | `string` | `"default"` | `sm` · `default` · `lg` |
| `Icon` | `string` | — | Heroicon name |
| `OnClick` | `EventCallback<MouseEventArgs>` | — | Click handler |

```razor
<FloatingActionButton Icon="plus"
                      Position="bottom-right"
                      OnClick="OpenCreateDialog" />
```

---

### Toolbar

Bordered icon/button toolbar container.

```razor
<Toolbar>
    <Button Variant="ghost" Size="icon"><Icon Name="bold" /></Button>
    <Button Variant="ghost" Size="icon"><Icon Name="italic" /></Button>
    <Separator Orientation="vertical" Class="h-6" />
    <Button Variant="ghost" Size="icon"><Icon Name="link" /></Button>
</Toolbar>
```

---

### Stepper

Step-by-step progress indicator in horizontal or vertical layout.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ActiveStep` | `int` | `0` | Current step index |
| `Orientation` | `string` | `"horizontal"` | `horizontal` · `vertical` |

```razor
<Stepper ActiveStep="_step">
    <StepperStep Title="Account" Description="Basic details" />
    <StepperStep Title="Profile" Description="About you" />
    <StepperStep Title="Review" Description="Confirm info" />
</Stepper>

<Button OnClick="() => _step++">Next</Button>

@code {
    int _step = 0;
}
```

---

### Wizard

Tab-style multi-step wizard with step validation support.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ActiveStep` | `int` | `0` | Current step (bound) |
| `ActiveStepChanged` | `EventCallback<int>` | — | Step change callback |
| `OnComplete` | `EventCallback` | — | Fires when last step is submitted |

```razor
<Wizard @bind-ActiveStep="_step" OnComplete="Submit">
    <WizardStep Title="Personal Info">
        <Input @bind-Value="_name" Placeholder="Full name" />
    </WizardStep>
    <WizardStep Title="Address">
        <Input @bind-Value="_address" Placeholder="Street address" />
    </WizardStep>
    <WizardStep Title="Confirm">
        <p>Review and confirm your details.</p>
    </WizardStep>
</Wizard>
```

---

### Workflow

Visual process/workflow diagram for status-driven pipelines.

```razor
<Workflow>
    <WorkflowStep Label="Submitted" Status="completed" />
    <WorkflowStep Label="Under Review" Status="active" />
    <WorkflowStep Label="Approved" Status="pending" />
    <WorkflowStep Label="Published" Status="pending" />
</Workflow>
```

---

## Overlay Components

### Dialog

Modal dialog with header, scrollable content, and footer.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Open` | `bool` | `false` | Bound open state |
| `OpenChanged` | `EventCallback<bool>` | — | Two-way binding callback |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Button OnClick="() => _open = true">Open Dialog</Button>

<Dialog @bind-Open="_open">
    <DialogHeader>
        <DialogTitle>Confirm Action</DialogTitle>
    </DialogHeader>
    <p>Are you sure you want to proceed?</p>
    <DialogFooter>
        <Button Variant="outline" OnClick="() => _open = false">Cancel</Button>
        <Button OnClick="Confirm">Confirm</Button>
    </DialogFooter>
</Dialog>

@code {
    bool _open;
    void Confirm() { /* ... */ _open = false; }
}
```

---

### AlertDialog

Confirmation dialog following the cancel/confirm accessibility pattern.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Open` | `bool` | `false` | Bound open state |
| `OpenChanged` | `EventCallback<bool>` | — | Two-way binding callback |
| `Title` | `string` | — | Dialog title |
| `Description` | `string` | — | Dialog description |
| `ConfirmText` | `string` | `"Continue"` | Confirm button label |
| `CancelText` | `string` | `"Cancel"` | Cancel button label |
| `OnConfirm` | `EventCallback` | — | Fires on confirm |
| `OnCancel` | `EventCallback` | — | Fires on cancel |

```razor
<AlertDialog @bind-Open="_confirmOpen"
             Title="Delete item?"
             Description="This action cannot be undone."
             ConfirmText="Delete"
             OnConfirm="DeleteItem" />
```

---

### Sheet

Full-height slide-in panel anchored to any of the 4 sides.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Open` | `bool` | `false` | Bound open state |
| `OpenChanged` | `EventCallback<bool>` | — | Two-way binding callback |
| `Side` | `string` | `"right"` | `top` · `right` · `bottom` · `left` |
| `Title` | `string` | — | Sheet title |

```razor
<Sheet @bind-Open="_sheetOpen" Side="right" Title="Edit Profile">
    <Input @bind-Value="_name" Placeholder="Name" />
    <Button Class="mt-4" OnClick="Save">Save</Button>
</Sheet>

<Button OnClick="() => _sheetOpen = true">Edit Profile</Button>
```

---

### Drawer

Bottom-sheet / drawer overlay, typically used on mobile.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Open` | `bool` | `false` | Bound open state |
| `OpenChanged` | `EventCallback<bool>` | — | Two-way binding callback |
| `Title` | `string` | — | Drawer title |
| `Height` | `string` | `"50vh"` | Drawer height |

```razor
<Drawer @bind-Open="_drawerOpen" Title="Filters">
    <RadioGroup @bind-Value="_sort">
        <RadioGroupItem Value="newest" Label="Newest" />
        <RadioGroupItem Value="popular" Label="Most popular" />
    </RadioGroup>
</Drawer>
```

---

### Popup · Popover

`Popup` is anchor-relative (4 placements). `Popover` is a click-triggered floating content panel.

```razor
<Popover>
    <PopoverTrigger>
        <Button Variant="outline">Open Popover</Button>
    </PopoverTrigger>
    <PopoverContent>
        <p class="text-sm">Popover content here.</p>
    </PopoverContent>
</Popover>
```

---

### Tooltip

Hover tooltip with 4 placement options.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Text` | `string` | — | Tooltip label text |
| `Side` | `string` | `"top"` | `top` · `right` · `bottom` · `left` |
| `Delay` | `int` | `300` | Delay before show (ms) |

```razor
<Tooltip Text="Copy to clipboard" Side="top">
    <Button Variant="ghost" Size="icon"><Icon Name="clipboard" /></Button>
</Tooltip>
```

---

### HoverCard

Rich preview card that appears on hover over a trigger.

```razor
<HoverCard>
    <HoverCardTrigger>
        <a href="/users/vilas" class="underline">@vilas</a>
    </HoverCardTrigger>
    <HoverCardContent>
        <div class="flex gap-3">
            <Avatar Fallback="VS" />
            <div>
                <p class="font-semibold">Vilas Sagar</p>
                <p class="text-sm text-muted-foreground">Component library author</p>
            </div>
        </div>
    </HoverCardContent>
</HoverCard>
```

---

### Toaster

Toast notification system. Add `<Toaster />` once in `MainLayout.razor`, then inject `ToastService` anywhere.

```razor
@* MainLayout.razor *@
<Toaster />
```

```csharp
@inject ToastService Toast

// Show toasts
Toast.Show("Saved successfully!", ToastType.Success);
Toast.Show("Something went wrong.", ToastType.Error);
Toast.Show("New message received.", ToastType.Info);
Toast.Show("File deleted.", ToastType.Warning);

// With custom duration (ms)
Toast.Show("Profile updated!", ToastType.Success, duration: 5000);
```

---

### Window

Floating, draggable, and optionally resizable window panel.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Title` | `string` | — | Window title bar text |
| `Open` | `bool` | `false` | Bound open state |
| `OpenChanged` | `EventCallback<bool>` | — | Two-way binding callback |
| `Resizable` | `bool` | `true` | Allow resize |
| `InitialWidth` | `string` | `"400px"` | Starting width |
| `InitialHeight` | `string` | `"300px"` | Starting height |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Window @bind-Open="_winOpen" Title="Inspector" Resizable="true">
    <p>Floating window content.</p>
</Window>
```

---

### ImageViewer · FileViewer

`ImageViewer` is a lightbox-style image viewer. `FileViewer` renders PDF and text files inline.

```razor
<ImageViewer Src="/photos/landscape.jpg" Alt="Mountain landscape" />

<FileViewer Url="/docs/report.pdf" ContentType="application/pdf" />
```

---

## Data Components

### Alert

Inline status message with title and description.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Variant` | `string` | `"default"` | `default` · `destructive` |
| `Icon` | `string` | — | Heroicon name |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Alert Icon="check-circle">
    <AlertTitle>Success</AlertTitle>
    <AlertDescription>Your changes have been saved.</AlertDescription>
</Alert>

<Alert Variant="destructive" Icon="x-circle">
    <AlertTitle>Error</AlertTitle>
    <AlertDescription>Failed to save changes. Please try again.</AlertDescription>
</Alert>
```

---

### Table

Semantic HTML table with styled sub-components.

```razor
<Table>
    <TableHeader>
        <TableRow>
            <TableHead>Name</TableHead>
            <TableHead>Email</TableHead>
            <TableHead>Status</TableHead>
        </TableRow>
    </TableHeader>
    <TableBody>
        @foreach (var user in _users)
        {
            <TableRow>
                <TableCell>@user.Name</TableCell>
                <TableCell>@user.Email</TableCell>
                <TableCell><Badge>@user.Status</Badge></TableCell>
            </TableRow>
        }
    </TableBody>
</Table>
```

---

### DataTable

Enterprise data grid with sort, global search, column filters, pagination, row selection, virtualization, and infinite scroll.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Items` | `IEnumerable<TItem>` | — | Data source |
| `Columns` | `List<DataColumn<TItem>>` | — | Column definitions |
| `PageSize` | `int` | `20` | Rows per page |
| `Filterable` | `bool` | `false` | Enable global search |
| `Sortable` | `bool` | `false` | Enable column sort |
| `Selectable` | `bool` | `false` | Enable row checkboxes |
| `Virtualize` | `bool` | `false` | Virtual scroll for large datasets |
| `InfiniteScroll` | `bool` | `false` | Load more on scroll |
| `OnLoadMore` | `EventCallback` | — | Fires at bottom of list |
| `Loading` | `bool` | `false` | Show loading state |
| `Class` | `string` | — | Extra CSS classes |

```razor
<DataTable TItem="Order"
           Items="_orders"
           Columns="_columns"
           Filterable="true"
           Sortable="true"
           PageSize="25" />

@code {
    List<Order> _orders = [];

    List<DataColumn<Order>> _columns =
    [
        new("ID",       o => o.Id),
        new("Customer", o => o.CustomerName),
        new("Date",     o => o.CreatedAt.ToString("d")),
        new("Total",    o => o.Total.ToString("C")),
        new("Status",   o => o.Status),
    ];
}
```

---

### AdvancedTable

Feature-rich table variant with additional column-level filtering, grouping, and custom cell renderers.

```razor
<AdvancedTable TItem="Product"
               Items="_products"
               Columns="_cols"
               Filterable="true"
               Groupable="true" />
```

---

### PivotGrid

Cross-tabulation matrix grid with row/column aggregation.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Items` | `IEnumerable<TItem>` | — | Data source |
| `RowField` | `Func<TItem, string>` | — | Row dimension selector |
| `ColumnField` | `Func<TItem, string>` | — | Column dimension selector |
| `ValueField` | `Func<TItem, double>` | — | Numeric value selector |
| `Aggregation` | `string` | `"sum"` | `sum` · `avg` · `count` · `min` · `max` |

```razor
<PivotGrid TItem="Sale"
           Items="_sales"
           RowField="s => s.Region"
           ColumnField="s => s.Quarter"
           ValueField="s => s.Revenue"
           Aggregation="sum" />
```

---

### TreeView

Recursive, collapsible tree component for hierarchical data.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Items` | `List<TItem>` | — | Root nodes |
| `GetChildren` | `Func<TItem, List<TItem>>` | — | Children selector |
| `GetLabel` | `Func<TItem, string>` | — | Label selector |
| `SelectedItem` | `TItem` | — | Bound selected node |
| `SelectedItemChanged` | `EventCallback<TItem>` | — | Selection callback |
| `ItemTemplate` | `RenderFragment<TItem>` | — | Custom node renderer |

```razor
<TreeView TItem="FileNode"
          Items="_root"
          GetChildren="n => n.Children"
          GetLabel="n => n.Name"
          @bind-SelectedItem="_selected" />

@code {
    record FileNode(string Name, List<FileNode> Children = null!);

    FileNode? _selected;
    List<FileNode> _root =
    [
        new("src",
            Children:
            [
                new("Components", Children: [new("Button.razor")]),
                new("Pages",      Children: [new("Index.razor")]),
            ]),
        new("wwwroot", Children: [new("css"), new("js")]),
    ];
}
```

---

### TreeList

Hierarchical data table with expand/collapse rows and full ARIA treegrid support.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Items` | `IEnumerable<TItem>` | — | Root items |
| `Columns` | `List<TreeColumn<TItem>>` | — | Column definitions |
| `GetChildren` | `Func<TItem, IEnumerable<TItem>>` | — | Children selector |

```razor
<TreeList TItem="Department"
          Items="_depts"
          GetChildren="d => d.SubDepartments"
          Columns="_cols" />
```

---

### ListView

Scrollable, virtualized typed item list with custom item template.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Items` | `IEnumerable<TItem>` | — | Data source |
| `ItemTemplate` | `RenderFragment<TItem>` | — | Item renderer |
| `Height` | `string` | — | Fixed height for virtualization |

```razor
<ListView TItem="Notification" Items="_notes" Height="400px">
    <ItemTemplate Context="note">
        <div class="flex items-center gap-3 p-3 border-b">
            <Icon Name="bell" />
            <span>@note.Message</span>
            <span class="ml-auto text-xs text-muted-foreground">@note.Time</span>
        </div>
    </ItemTemplate>
</ListView>
```

---

### FileManager

Full file manager with tree sidebar, breadcrumb navigation, and grid/list view toggle.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Items` | `List<FileItem>` | — | File system nodes |
| `OnOpen` | `EventCallback<FileItem>` | — | Fires when item is opened |
| `OnDelete` | `EventCallback<FileItem>` | — | Fires on delete action |
| `OnRename` | `EventCallback<FileItem>` | — | Fires on rename action |
| `View` | `string` | `"grid"` | `grid` · `list` |

```razor
<FileManager Items="_files"
             OnOpen="OpenFile"
             OnDelete="DeleteFile" />
```

---

## Chart Components

All charts are pure SVG — no external chart library required. They automatically respect the active light/dark theme.

### LineChart

Multi-series line chart with optional area fill and hover tooltips.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Series` | `List<ChartSeries>` | — | Data series |
| `XLabels` | `List<string>` | — | X-axis labels |
| `Height` | `int` | `300` | Chart height in px |
| `ShowArea` | `bool` | `false` | Fill area under lines |
| `ShowGrid` | `bool` | `true` | Show grid lines |
| `ShowLegend` | `bool` | `true` | Show series legend |

```razor
<LineChart XLabels="_months"
           Series="_revenue"
           Height="300"
           ShowArea="true" />

@code {
    List<string> _months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun"];

    List<ChartSeries> _revenue =
    [
        new ChartSeries { Name = "2024", Data = [12000, 15000, 11000, 18000, 22000, 25000] },
        new ChartSeries { Name = "2025", Data = [14000, 17000, 13000, 20000, 26000, 30000] },
    ];
}
```

---

### BarChart

Grouped or stacked bar chart with multi-series support and hover tooltips.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Series` | `List<ChartSeries>` | — | Data series |
| `XLabels` | `List<string>` | — | Category labels |
| `Height` | `int` | `300` | Chart height in px |
| `Stacked` | `bool` | `false` | Stack bars instead of grouping |
| `ShowGrid` | `bool` | `true` | Show grid lines |
| `ShowLegend` | `bool` | `true` | Show legend |

```razor
<BarChart XLabels="_quarters"
          Series="_sales"
          Stacked="false" />

@code {
    List<string> _quarters = ["Q1", "Q2", "Q3", "Q4"];

    List<ChartSeries> _sales =
    [
        new ChartSeries { Name = "Product A", Data = [4200, 5800, 6100, 7300] },
        new ChartSeries { Name = "Product B", Data = [3100, 4200, 5500, 6800] },
    ];
}
```

---

### PieChart

Pie or donut chart with percentage labels and configurable center label.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Segments` | `List<ChartSegment>` | — | Data segments |
| `Size` | `int` | `250` | SVG size in px |
| `InnerRadius` | `double` | `0` | `> 0` for donut chart (0–1 ratio) |
| `CenterLabel` | `string` | — | Text in donut center |
| `ShowLegend` | `bool` | `true` | Show segment legend |
| `ShowPercentages` | `bool` | `true` | Show % labels on slices |

```razor
<PieChart Segments="_traffic"
          InnerRadius="0.6"
          CenterLabel="Traffic"
          Size="280" />

@code {
    List<ChartSegment> _traffic =
    [
        new ChartSegment { Label = "Organic",   Value = 42, Color = "#3B82F6" },
        new ChartSegment { Label = "Paid",      Value = 28, Color = "#10B981" },
        new ChartSegment { Label = "Social",    Value = 18, Color = "#F59E0B" },
        new ChartSegment { Label = "Referral",  Value = 12, Color = "#EF4444" },
    ];
}
```

---

### GaugeChart

Half-circle gauge with threshold color zones.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `double` | `0` | Current value |
| `Min` | `double` | `0` | Minimum value |
| `Max` | `double` | `100` | Maximum value |
| `Thresholds` | `List<GaugeThreshold>` | — | Color zones |
| `Label` | `string` | — | Center label |
| `Unit` | `string` | — | Value unit (e.g. `"%"`, `"rpm"`) |
| `Size` | `int` | `250` | SVG size in px |

```razor
<GaugeChart Value="73"
            Min="0"
            Max="100"
            Label="CPU Usage"
            Unit="%"
            Thresholds="_cpuThresholds" />

@code {
    List<GaugeThreshold> _cpuThresholds =
    [
        new GaugeThreshold { UpTo = 60,  Color = "#10B981" },  // green
        new GaugeThreshold { UpTo = 80,  Color = "#F59E0B" },  // amber
        new GaugeThreshold { UpTo = 100, Color = "#EF4444" },  // red
    ];
}
```

---

## Blocks

Pre-built, composable page sections. Drop them into any page to assemble complete layouts rapidly.

### Dashboard Blocks

| Component | Description |
|-----------|-------------|
| `DashboardCard` | KPI metric card with value, label, trend, and icon |
| `CompactCard` | Compact metric tile for dense dashboards |
| `DashboardSidebar` | Full left-sidebar nav with logo, menu, and footer |
| `TopNavbar` | Top navigation bar for dashboard shells |

```razor
<DashboardCard Title="Total Revenue"
               Value="$128,450"
               Trend="+12.5%"
               TrendUp="true"
               Icon="currency-dollar" />
```

---

### Landing Page Blocks

| Component | Description |
|-----------|-------------|
| `HeroBlock` | Full-width hero section with headline, subtext, and CTA buttons |
| `FeaturesBlock` | Feature grid with icon, title, and description per item |
| `StatsBlock` | Horizontal stats strip with key metrics |
| `TestimonialsBlock` | Customer quote cards |
| `PricingBlock` | Pricing tier cards with feature lists |
| `PartnersBlock` | Logo carousel for partner/sponsor brands |
| `NewsletterBlock` | Email signup section |
| `IncentivesBlock` | Icon + text incentive row |
| `ContactFormBlock` | Pre-wired contact form with validation |
| `LocationsBlock` | Office/branch location cards |
| `CardsBlock` | Generic content card grid |
| `FooterBlock` | Multi-column site footer with links |

```razor
<HeroBlock Headline="Build faster with Blazor"
           Subtext="Production-ready components, zero config."
           PrimaryAction="Get Started"
           PrimaryHref="/docs"
           SecondaryAction="View on GitHub"
           SecondaryHref="https://github.com/vilassagar/MyStackBlazor" />

<FeaturesBlock Features="_features" Columns="3" />

<PricingBlock Plans="_plans" />
```

---

### Listing Blocks

| Component | Description |
|-----------|-------------|
| `FilterPanel` | Sidebar filter panel with checkboxes, ranges, and reset |
| `SortBar` | Sort-by dropdown + results count strip |
| `ProductCard` | E-commerce product card with image, price, and CTA |
| `FlyoutNavigation` | Off-canvas category navigation |
| `LoginForm` | Pre-styled sign-in form |
| `SignUpForm` | Pre-styled registration form |

```razor
<div class="flex gap-6">
    <FilterPanel Filters="_filters" OnFilterChange="ApplyFilters" />
    <div class="flex-1">
        <SortBar ResultCount="_products.Count" @bind-SortBy="_sort" />
        <GridLayout Columns="3" Gap="4">
            @foreach (var p in _products)
            {
                <ProductCard Product="p" OnAddToCart="AddToCart" />
            }
        </GridLayout>
    </div>
</div>
```

---

## Templates

Full-page demo templates. Import them directly or use as a starting point.

### Dashboard Templates

| Component | Use Case |
|-----------|----------|
| `FintechDashboard` | Banking / financial analytics |
| `HealthcareAdmin` | Patient & clinical data management |
| `ProjectTracker` | Agile project and task management |
| `AIUsageMonitoring` | AI API usage metrics and cost tracking |
| `MachineManufacturing` | Industrial IoT machine monitoring |
| `SocialMediaManagement` | Social media analytics and scheduling |

```razor
@page "/demo/fintech"

<FintechDashboard />
```

---

### Landing Page Templates

| Component | Use Case |
|-----------|----------|
| `SaaSProduct` | SaaS product marketing page |
| `InternationalBank` | Banking / finance landing page |
| `AutomotiveIndustry` | Automotive brand landing page |
| `Travel` | Travel and hospitality landing page |

```razor
@page "/demo/saas"

<SaaSProduct />
```

---

### Listing Templates

| Component | Use Case |
|-----------|----------|
| `FashionCatalogue` | Apparel e-commerce catalog |
| `RealEstate` | Property listing page |

```razor
@page "/demo/real-estate"

<RealEstate />
```

---

## Services

Services are registered automatically by `AddMyStackBlazor()` and are available via DI anywhere in the app.

### ThemeService

```csharp
@inject ThemeService ThemeService

ThemeService.SetTheme(AppTheme.Light);
ThemeService.SetTheme(AppTheme.Dark);
ThemeService.SetTheme(AppTheme.System);
ThemeService.Toggle();

bool isDark = ThemeService.IsDark;
AppTheme current = ThemeService.CurrentTheme;

ThemeService.OnThemeChanged += () => InvokeAsync(StateHasChanged);
```

### ToastService

```csharp
@inject ToastService Toast

Toast.Show("Message text", ToastType.Success);
Toast.Show("Message text", ToastType.Error);
Toast.Show("Message text", ToastType.Info);
Toast.Show("Message text", ToastType.Warning);

// Custom duration in milliseconds (default 3000)
Toast.Show("Saved!", ToastType.Success, duration: 5000);
```

---

## Modular Packages

MyStackBlazor ships optional add-on packages for specialized needs.

| Package | Description |
|---------|-------------|
| `MyStackBlazor` | Core library — all 166 components |
| `MyStackBlazor.Core` | Base abstractions and state primitives |
| `MyStackBlazor.DataGrid` | Extended enterprise data grid features |
| `MyStackBlazor.Accessibility` | WCAG 2.2 helpers and ARIA utilities |
| `MyStackBlazor.Localization` | i18n support and RTL layout helpers |
| `MyStackBlazor.Security` | HTML sanitization and auth component helpers |

Install additional packages:

```bash
dotnet add package MyStackBlazor.DataGrid
dotnet add package MyStackBlazor.Accessibility
dotnet add package MyStackBlazor.Localization
dotnet add package MyStackBlazor.Security
```

---

## Contributing

Contributions, issues, and feature requests are welcome.

1. Fork the repository
2. Create a feature branch: `git checkout -b feat/my-component`
3. Commit your changes following conventional commits
4. Open a pull request

To build and test locally:

```bash
# Restore dependencies
dotnet restore

# Build the library
dotnet build src/MyStackBlazor/MyStackBlazor.csproj

# Run tests
dotnet test

# Rebuild Tailwind CSS
cd src/MyStackBlazor
npm install
npx tailwindcss -i tailwind/input.css -o wwwroot/css/mystackblazor.css --minify
```

To pack a new NuGet version:

```powershell
./pack.ps1
```

---

## License

MIT © 2025 [Vilas Sagar](https://github.com/vilassagar)
