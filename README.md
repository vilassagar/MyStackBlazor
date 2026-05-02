# MyStackBlazor

A modern Blazor component library styled with **Tailwind CSS v4**, inspired by shadcn/ui.  
Targets **.NET 10** — works on **Blazor WASM**, **Blazor Server**, and **.NET MAUI Blazor Hybrid**.

## Installation

```bash
dotnet add package MyStackBlazor
```

## Setup

### 1. Register services in `Program.cs`

```csharp
builder.Services.AddMyStackBlazor();
```

### 2. Add assets to `index.html` (web) or `wwwroot/index.html` (MAUI)

```html
<link rel="stylesheet" href="_content/MyStackBlazor/css/mystackblazor.css" />
<script src="_content/MyStackBlazor/js/mystackblazor.js"></script>
```

### 3. Add to `_Imports.razor`

```razor
@using MyStackBlazor
@using MyStackBlazor.Components.Common
@using MyStackBlazor.Components.Form
@using MyStackBlazor.Components.Navigation
@using MyStackBlazor.Components.Layout
@using MyStackBlazor.Components.Overlays
@using MyStackBlazor.Components.Data
```

### 4. Wrap your root layout with `<ThemeProvider>`

```razor
@* MainLayout.razor *@
@inherits LayoutComponentBase

<ThemeProvider>
    <div class="min-h-screen bg-background text-foreground">
        <header>
            <ThemeToggle />  @* light/dark toggle button *@
        </header>
        <main>
            <Toaster />
            @Body
        </main>
    </div>
</ThemeProvider>
```

---

## Theming (Light / Dark / System)

MyStackBlazor ships with two built-in themes. `ThemeProvider` wraps its children in a
`.dark` CSS class when dark mode is active, switching all Tailwind design tokens automatically.

**Theme via `ThemeService` (injected anywhere):**

```csharp
@inject ThemeService ThemeService

ThemeService.SetTheme(AppTheme.Light);
ThemeService.SetTheme(AppTheme.Dark);
ThemeService.SetTheme(AppTheme.System);  // follows OS preference
ThemeService.Toggle();                   // flip between light ↔ dark

bool isDark = ThemeService.IsDark;
```

**Theme toggle button:**
```razor
<ThemeToggle />                  <!-- icon only -->
<ThemeToggle ShowLabel="true" /> <!-- icon + label -->
```

---

## MAUI Blazor Hybrid Setup

The library works identically in MAUI. In `MauiProgram.cs`:

```csharp
builder.Services.AddMauiBlazorWebView();
builder.Services.AddMyStackBlazor();
```

`wwwroot/index.html` — same link/script tags as above.

> **Note:** Theme persistence in MAUI uses `localStorage` (available in WinUI/Android/iOS
> WebView). For a native preferences fallback, subscribe to `ThemeService.OnThemeChanged`
> and store the value via `Microsoft.Maui.Storage.Preferences`.

---

## Components

| Group | Components |
|-------|-----------|
| **Common** | `Button` · `Badge` · `Label` · `Separator` · `Skeleton` · `Typography` · `ThemeProvider` · `ThemeToggle` |
| **Form** | `Input` · `Textarea` · `Checkbox` · `RadioGroup` · `Select` · `Switch` · `Slider` · `Combobox` · `DatePicker` · `InputOtp` |
| **Navigation** | `Tabs` · `Breadcrumb` · `Pagination` · `DropdownMenu` · `ContextMenu` · `Menubar` · `Sidebar` |
| **Layout** | `Card` · `Accordion` · `Collapsible` · `ScrollArea` · `AspectRatio` |
| **Overlays** | `Dialog` · `AlertDialog` · `Sheet` · `Drawer` · `Popover` · `HoverCard` · `Tooltip` · `Toaster` |
| **Data** | `Alert` · `Progress` · `Table` · `DataTable` |

---

## Usage Example

```razor
<Button Variant="outline" @onclick="Save">Save Changes</Button>

<Card>
  <CardHeader>
    <CardTitle>Hello</CardTitle>
    <CardDescription>Welcome to MyStackBlazor.</CardDescription>
  </CardHeader>
  <CardContent>
    <Input Placeholder="Your name..." @bind-Value="_name" />
  </CardContent>
  <CardFooter>
    <Button>Submit</Button>
  </CardFooter>
</Card>
```

## License

MIT
