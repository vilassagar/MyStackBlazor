# MyStackBlazor

A modern Blazor component library styled with **Tailwind CSS v4**, inspired by shadcn/ui.  
Targets **.NET 10** — works on **Blazor WASM**, **Blazor Server**, and **.NET MAUI Blazor Hybrid**.

---

## Table of Contents

- [Installation](#installation)
- [Setup](#setup)
- [Theming](#theming-light--dark--system)
- [MAUI Setup](#maui-blazor-hybrid-setup)
- [Common Components](#common-components)
  - [Button](#button)
  - [Badge](#badge)
  - [Label](#label)
  - [Separator](#separator)
  - [Skeleton](#skeleton)
  - [Typography](#typography)
  - [Avatar](#avatar)
  - [Chip / ChipList](#chip--chiplist)
  - [Spinner](#spinner)
  - [Icon](#icon)
  - [ThemeToggle](#themetoggle)
- [Form Components](#form-components)
  - [Input](#input)
  - [Textarea](#textarea)
  - [Checkbox](#checkbox)
  - [Switch](#switch)
  - [RadioGroup](#radiogroup)
  - [Select](#select)
  - [Slider](#slider)
  - [NumericInput](#numericinput)
  - [Combobox](#combobox)
  - [AutoComplete](#autocomplete)
  - [DatePicker](#datepicker)
  - [TimePicker](#timepicker)
  - [InputOtp](#inputotp)
  - [ColorPicker](#colorpicker)
  - [FileUpload](#fileupload)
  - [Rating](#rating)
  - [FormField](#formfield)
- [Layout Components](#layout-components)
  - [Card](#card)
  - [Accordion](#accordion)
  - [Collapsible](#collapsible)
  - [ScrollArea](#scrollarea)
  - [AspectRatio](#aspectratio)
  - [GridLayout](#gridlayout)
  - [StackLayout](#stacklayout)
  - [Carousel](#carousel)
  - [PanelBar](#panelbar)
  - [Loader](#loader)
  - [Progress / ChunkProgressBar](#progress--chunkprogressbar)
  - [AnimationContainer](#animationcontainer)
- [Navigation Components](#navigation-components)
  - [Tabs](#tabs)
  - [Breadcrumb](#breadcrumb)
  - [Pagination](#pagination)
  - [Sidebar](#sidebar)
  - [Menubar](#menubar)
  - [DropdownMenu](#dropdownmenu)
  - [ContextMenu](#contextmenu)
  - [AppBar](#appbar)
  - [Toolbar](#toolbar)
  - [ButtonGroup / ToggleButton](#buttongroup--togglebutton)
  - [FloatingActionButton](#floatingactionbutton)
  - [DropDownButton / SplitButton](#dropdownbutton--splitbutton)
  - [Stepper](#stepper)
  - [Wizard](#wizard)
  - [Workflow](#workflow)
- [Overlay Components](#overlay-components)
  - [Dialog](#dialog)
  - [AlertDialog](#alertdialog)
  - [Sheet](#sheet)
  - [Drawer](#drawer)
  - [Popover](#popover)
  - [Tooltip](#tooltip)
  - [HoverCard](#hovercard)
  - [Toaster](#toaster)
  - [Window](#window)
  - [FileViewer / ImageViewer](#fileviewer--imageviewer)
- [Data Components](#data-components)
  - [Alert](#alert)
  - [Table](#table)
  - [DataTable](#datatable)
  - [TreeView](#treeview)
- [Publishing the NuGet Package](#publishing-the-nuget-package)
- [License](#license)

---

## Installation

```bash
dotnet add package MyStackBlazor
```

---

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

## Theming (Light / Dark / System)

`ThemeProvider` wraps its children in a `.dark` CSS class when dark mode is active, switching all Tailwind design tokens automatically.

**Via `ThemeService` (injectable anywhere):**

```csharp
@inject ThemeService ThemeService

ThemeService.SetTheme(AppTheme.Light);
ThemeService.SetTheme(AppTheme.Dark);
ThemeService.SetTheme(AppTheme.System);  // follows OS preference
ThemeService.Toggle();                   // flip between light ↔ dark

bool isDark = ThemeService.IsDark;
```

---

## MAUI Blazor Hybrid Setup

```csharp
// MauiProgram.cs
builder.Services.AddMauiBlazorWebView();
builder.Services.AddMyStackBlazor();
```

Same `link` / `script` tags in `wwwroot/index.html`.

> **Note:** Theme persistence in MAUI uses `localStorage`. For a native preferences fallback, subscribe to `ThemeService.OnThemeChanged` and store the value via `Microsoft.Maui.Storage.Preferences`.

---

## Common Components

### Button

Renders a styled button with variant, size, loading, and disabled states.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Variant` | `string` | `"default"` | `default` · `destructive` · `outline` · `secondary` · `ghost` · `link` |
| `Size` | `string` | `"default"` | `default` · `sm` · `lg` · `icon` |
| `Disabled` | `bool` | `false` | Disables the button |
| `IsLoading` | `bool` | `false` | Shows a spinner and disables clicks |
| `Type` | `string` | `"button"` | HTML button type (`button` · `submit` · `reset`) |
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

<Button Type="submit" Variant="default" OnClick="Save">Save Changes</Button>

@code {
    async Task Save(MouseEventArgs e) { /* ... */ }
}
```

---

### Badge

Inline label for status or category indicators.

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

Accessible form label.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `For` | `string` | — | Associates label with an input `id` |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Label For="email">Email address</Label>
<Input Id="email" Type="email" Placeholder="you@example.com" />
```

---

### Separator

Horizontal or vertical divider.

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

Animated placeholder for content that is loading.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string` | — | Controls shape and size |

```razor
<Skeleton Class="h-4 w-48 rounded" />
<Skeleton Class="h-10 w-full rounded-md" />
<Skeleton Class="h-12 w-12 rounded-full" />

@* Card placeholder *@
<div class="flex gap-4">
    <Skeleton Class="h-12 w-12 rounded-full" />
    <div class="flex flex-col gap-2">
        <Skeleton Class="h-4 w-32" />
        <Skeleton Class="h-4 w-24" />
    </div>
</div>
```

---

### Typography

Semantic text elements with consistent styling.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Variant` | `string` | `"p"` | `h1` · `h2` · `h3` · `h4` · `p` · `lead` · `large` · `small` · `muted` · `blockquote` · `code` |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Typography Variant="h1">Page Title</Typography>
<Typography Variant="h2">Section Heading</Typography>
<Typography Variant="h3">Sub-section</Typography>
<Typography Variant="lead">A concise introduction paragraph.</Typography>
<Typography Variant="p">Regular body text.</Typography>
<Typography Variant="muted">Secondary / helper text.</Typography>
<Typography Variant="small">Small print</Typography>
<Typography Variant="blockquote">
    "Design is not just what it looks like; design is how it works."
</Typography>
<Typography Variant="code">npm install mystackblazor</Typography>
```

---

### Avatar

User avatar with image fallback to initials or custom content.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Src` | `string` | — | Image URL |
| `Alt` | `string` | — | Image alt text |
| `Initials` | `string` | — | Fallback initials when image fails |
| `Size` | `string` | `"default"` | `xs` · `sm` · `default` · `lg` · `xl` |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Avatar Src="/images/user.jpg" Alt="Jane Doe" />
<Avatar Initials="JD" Size="lg" />
<Avatar Size="sm" Initials="AB" />

@* Custom fallback content *@
<Avatar>
    <Icon Name="user" />
</Avatar>
```

---

### Chip / ChipList

Removable tag / chip element.

**Chip parameters:**

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Variant` | `string` | `"default"` | `default` · `secondary` · `destructive` · `success` · `warning` · `outline` |
| `Size` | `string` | `"default"` | `default` · `sm` · `lg` |
| `OnRemove` | `EventCallback` | — | Fires when the × button is clicked |

```razor
<Chip>React</Chip>
<Chip Variant="success">Active</Chip>
<Chip Variant="warning">Pending</Chip>
<Chip Variant="destructive" OnRemove="RemoveTag">Blazor</Chip>

@* Generic ChipList *@
<ChipList Items="_tags"
          ItemLabel="t => t.Name"
          OnRemove="RemoveTag" />

@code {
    record Tag(string Name);
    List<Tag> _tags = [new("Blazor"), new("C#"), new(".NET")];
    void RemoveTag(Tag t) => _tags.Remove(t);
}
```

---

### Spinner

Accessible loading spinner.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Size` | `string` | `"default"` | `xs` · `sm` · `default` · `lg` · `xl` |
| `Label` | `string` | `"Loading..."` | Screen-reader label |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Spinner />
<Spinner Size="lg" Label="Processing payment…" />
<Spinner Size="sm" Class="text-primary" />
```

---

### Icon

Renders a named icon from the bundled icon set.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Name` | `string` | **required** | Icon name (e.g. `"home"`, `"search"`, `"check"`) |
| `Size` | `string` | `"default"` | `xs` · `sm` · `default` · `lg` · `xl` · `2xl` |
| `Filled` | `bool` | `false` | Solid / filled variant |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Icon Name="home" />
<Icon Name="search" Size="lg" />
<Icon Name="heart" Filled="true" Class="text-red-500" />
<Icon Name="check" Size="sm" Class="text-green-600" />
```

---

### ThemeToggle

Button that toggles between light and dark mode.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ShowLabel` | `bool` | `false` | Shows "Light" / "Dark" text beside the icon |
| `Class` | `string` | — | Extra CSS classes |

```razor
<ThemeToggle />
<ThemeToggle ShowLabel="true" />
```

---

## Form Components

### Input

Styled text input with two-way binding.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Id` | `string` | — | HTML `id` attribute |
| `Type` | `string` | `"text"` | HTML input type |
| `Placeholder` | `string` | — | Placeholder text |
| `Value` | `string` | — | Bound value |
| `Disabled` | `bool` | `false` | Disables the input |
| `ReadOnly` | `bool` | `false` | Read-only mode |
| `Class` | `string` | — | Extra CSS classes |
| `ValueChanged` | `EventCallback<string>` | — | Fires on value change |

```razor
<Input Placeholder="Enter your name" @bind-Value="_name" />
<Input Type="email" Placeholder="you@example.com" @bind-Value="_email" />
<Input Type="password" @bind-Value="_password" />
<Input ReadOnly="true" Value="Read-only value" />
<Input Disabled="true" Placeholder="Disabled" />

@code {
    string _name = "";
    string _email = "";
    string _password = "";
}
```

---

### Textarea

Multi-line text input.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Id` | `string` | — | HTML `id` |
| `Placeholder` | `string` | — | Placeholder text |
| `Value` | `string` | — | Bound value |
| `Rows` | `int` | `4` | Visible rows |
| `Disabled` | `bool` | `false` | Disables the textarea |
| `ReadOnly` | `bool` | `false` | Read-only mode |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Textarea Placeholder="Write your message…" @bind-Value="_message" />
<Textarea Rows="8" @bind-Value="_notes" />

@code {
    string _message = "";
    string _notes = "";
}
```

---

### Checkbox

Styled checkbox with label support.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Id` | `string` | — | HTML `id` |
| `Checked` | `bool` | `false` | Checked state |
| `CheckedChanged` | `EventCallback<bool>` | — | Fires when checked state changes |
| `Label` | `string` | — | Inline label text |
| `Disabled` | `bool` | `false` | Disables the checkbox |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Checkbox Id="terms" Label="Accept terms and conditions" @bind-Checked="_accepted" />
<Checkbox Id="newsletter" Label="Subscribe to newsletter" @bind-Checked="_subscribe" Disabled="true" />

@code {
    bool _accepted = false;
    bool _subscribe = true;
}
```

---

### Switch

Toggle switch (on/off).

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Id` | `string` | — | HTML `id` |
| `Checked` | `bool` | `false` | Checked state |
| `CheckedChanged` | `EventCallback<bool>` | — | Fires when state changes |
| `Label` | `string` | — | Inline label text |
| `Disabled` | `bool` | `false` | Disables the switch |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Switch Id="notifications" Label="Enable notifications" @bind-Checked="_notifyEnabled" />
<Switch Id="darkMode" Label="Dark mode" @bind-Checked="_dark" />

@code {
    bool _notifyEnabled = true;
    bool _dark = false;
}
```

---

### RadioGroup

Group of mutually exclusive radio buttons.

```razor
<RadioGroup @bind-Value="_plan">
    <RadioGroupItem Id="free"    Value="free"       Label="Free" />
    <RadioGroupItem Id="pro"     Value="pro"        Label="Pro — $9/mo" />
    <RadioGroupItem Id="team"    Value="team"       Label="Team — $29/mo" />
    <RadioGroupItem Id="disabled" Value="enterprise" Label="Enterprise" Disabled="true" />
</RadioGroup>

<p>Selected: @_plan</p>

@code {
    string _plan = "free";
}
```

---

### Select

Native-style dropdown select.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string` | — | Selected value |
| `ValueChanged` | `EventCallback<string>` | — | Fires on selection change |
| `Placeholder` | `string` | — | Placeholder option |
| `Disabled` | `bool` | `false` | Disables the select |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Select @bind-Value="_country" Placeholder="Select a country">
    <option value="us">United States</option>
    <option value="gb">United Kingdom</option>
    <option value="ca">Canada</option>
    <option value="au">Australia</option>
</Select>

@code {
    string _country = "";
}
```

---

### Slider

Range slider for numeric input.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `double` | `0` | Current value |
| `ValueChanged` | `EventCallback<double>` | — | Fires on value change |
| `Min` | `double` | `0` | Minimum value |
| `Max` | `double` | `100` | Maximum value |
| `Step` | `double` | `1` | Step increment |
| `Disabled` | `bool` | `false` | Disables the slider |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Slider @bind-Value="_volume" Min="0" Max="100" Step="5" />
<p>Volume: @_volume</p>

<Slider @bind-Value="_price" Min="0" Max="1000" Step="10" />

@code {
    double _volume = 50;
    double _price = 200;
}
```

---

### NumericInput

Number input with optional increment/decrement stepper buttons.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `double?` | — | Numeric value |
| `ValueChanged` | `EventCallback<double?>` | — | Fires on value change |
| `Min` | `double?` | — | Minimum value |
| `Max` | `double?` | — | Maximum value |
| `Step` | `double` | `1` | Step increment |
| `ShowStepper` | `bool` | `true` | Show +/− stepper buttons |
| `Placeholder` | `string` | — | Placeholder text |
| `Disabled` | `bool` | `false` | Disables the input |
| `Class` | `string` | — | Extra CSS classes |

```razor
<NumericInput @bind-Value="_quantity" Min="1" Max="99" Step="1" />
<NumericInput @bind-Value="_price" Min="0" Step="0.01" Placeholder="0.00" ShowStepper="false" />

@code {
    double? _quantity = 1;
    double? _price = null;
}
```

---

### Combobox

Searchable dropdown with custom options.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string` | — | Selected value |
| `ValueChanged` | `EventCallback<string>` | — | Fires on selection |
| `Items` | `List<ComboboxItem>` | — | Available items |
| `Placeholder` | `string` | — | Trigger placeholder |
| `SearchPlaceholder` | `string` | — | Search input placeholder |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Combobox @bind-Value="_framework"
          Items="_frameworks"
          Placeholder="Select framework…"
          SearchPlaceholder="Search…" />

@code {
    string _framework = "";
    List<ComboboxItem> _frameworks =
    [
        new("blazor",  "Blazor"),
        new("react",   "React"),
        new("angular", "Angular"),
        new("vue",     "Vue"),
    ];
}
```

---

### AutoComplete

Async-capable type-ahead search input.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Id` | `string` | — | HTML `id` |
| `Placeholder` | `string` | — | Placeholder text |
| `Disabled` | `bool` | `false` | Disables the input |
| `Class` | `string` | — | Extra CSS classes |

```razor
<AutoComplete TItem="string"
              Placeholder="Search cities…"
              SearchFunc="SearchCities"
              @bind-Value="_city"
              ItemLabel="c => c" />

@code {
    string _city = "";

    async Task<IEnumerable<string>> SearchCities(string query)
    {
        string[] cities = ["New York", "London", "Tokyo", "Paris", "Sydney"];
        return cities.Where(c => c.Contains(query, StringComparison.OrdinalIgnoreCase));
    }
}
```

---

### DatePicker

Calendar date picker.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `DateTime?` | — | Selected date |
| `ValueChanged` | `EventCallback<DateTime?>` | — | Fires on date selection |
| `Format` | `string` | — | Display format (e.g. `"yyyy-MM-dd"`) |
| `Placeholder` | `string` | — | Trigger placeholder |
| `Disabled` | `bool` | `false` | Disables the picker |
| `Class` | `string` | — | Extra CSS classes |

```razor
<DatePicker @bind-Value="_dob" Placeholder="Pick a date" Format="dd/MM/yyyy" />
<DatePicker @bind-Value="_deadline" Placeholder="Select deadline" />

@code {
    DateTime? _dob;
    DateTime? _deadline;
}
```

---

### DateRangePicker

Select a start and end date range.

```razor
<DateRangePicker @bind-StartDate="_start" @bind-EndDate="_end" Placeholder="Select date range" />

@code {
    DateTime? _start;
    DateTime? _end;
}
```

---

### TimePicker

Time selection input.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `TimeSpan?` | — | Selected time |
| `ValueChanged` | `EventCallback<TimeSpan?>` | — | Fires on time selection |
| `Placeholder` | `string` | — | Placeholder text |
| `Disabled` | `bool` | `false` | Disables the picker |
| `Class` | `string` | — | Extra CSS classes |

```razor
<TimePicker @bind-Value="_meetingTime" Placeholder="Select time" />

@code {
    TimeSpan? _meetingTime;
}
```

For 12-hour format use `TimePicker12`; for 24-hour format use `TimePicker24`.

---

### InputOtp

One-time password / PIN input with grouped slots.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Length` | `int` | `6` | Total number of digits |
| `GroupAt` | `int` | `3` | Position of the separator |
| `Value` | `string` | — | Current OTP value |
| `ValueChanged` | `EventCallback<string>` | — | Fires when value changes |
| `Class` | `string` | — | Extra CSS classes |

```razor
<InputOtp @bind-Value="_otp" Length="6" GroupAt="3" />
<p>Code: @_otp</p>

@* 4-digit PIN with no separator *@
<InputOtp @bind-Value="_pin" Length="4" GroupAt="0" />

@code {
    string _otp = "";
    string _pin = "";
}
```

---

### ColorPicker

HSV color picker with hex output.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `string` | — | Hex color string (e.g. `"#3b82f6"`) |
| `ValueChanged` | `EventCallback<string>` | — | Fires on color change |
| `ShowAlpha` | `bool` | `false` | Show alpha/opacity slider |
| `ShowSliders` | `bool` | `true` | Show RGB sliders |
| `Class` | `string` | — | Extra CSS classes |

```razor
<ColorPicker @bind-Value="_brandColor" ShowAlpha="true" />
<div class="w-10 h-10 rounded" style="background:@_brandColor"></div>

@code {
    string _brandColor = "#3b82f6";
}
```

---

### FileUpload

Drag-and-drop file upload area.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Accept` | `string` | — | Accepted MIME types / extensions |
| `Multiple` | `bool` | `false` | Allow multiple files |
| `MaxFileSize` | `long` | — | Maximum file size in bytes |
| `HintText` | `string` | — | Helper text shown below the drop zone |
| `Files` | `List<FileUploadItem>` | — | Currently selected files |
| `OnFilesChanged` | `EventCallback<InputFileChangeEventArgs>` | — | Fires when files are selected |

```razor
<FileUpload Accept=".pdf,.docx"
            Multiple="true"
            MaxFileSize="5242880"
            HintText="PDF or Word, max 5 MB"
            OnFilesChanged="HandleFiles" />

@code {
    async Task HandleFiles(InputFileChangeEventArgs e)
    {
        foreach (var file in e.GetMultipleFiles())
            Console.WriteLine(file.Name);
    }
}
```

---

### Rating

Star rating input.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `int` | `0` | Current rating (1–Max) |
| `ValueChanged` | `EventCallback<int>` | — | Fires when rating changes |
| `Max` | `int` | `5` | Number of stars |
| `ReadOnly` | `bool` | `false` | Display-only mode |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Rating @bind-Value="_stars" Max="5" />
<p>Rating: @_stars / 5</p>

<Rating Value="4" ReadOnly="true" />

@code {
    int _stars = 3;
}
```

---

### FormField

Wrapper that wires a label, helper text, and validation error to a form control.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Label` | `string` | — | Field label |
| `For` | `string` | — | Associated input `id` |
| `Description` | `string` | — | Helper / hint text |
| `ErrorMessage` | `string` | — | Explicit error message |
| `Required` | `bool` | `false` | Marks field as required |
| `Disabled` | `bool` | `false` | Disables the field visually |
| `Class` | `string` | — | Extra CSS classes |

```razor
<EditForm Model="_model" OnValidSubmit="Submit">
    <DataAnnotationsValidator />

    <FormField Label="Email" For="email" Description="We'll never share your email."
               Required="true" FieldId="@(FieldIdentifier.Create(() => _model.Email))">
        <Input Id="email" Type="email" @bind-Value="_model.Email" />
    </FormField>

    <FormField Label="Password" For="password" Required="true"
               FieldId="@(FieldIdentifier.Create(() => _model.Password))">
        <Input Id="password" Type="password" @bind-Value="_model.Password" />
    </FormField>

    <Button Type="submit">Sign in</Button>
</EditForm>

@code {
    LoginModel _model = new();
    void Submit() { /* ... */ }

    class LoginModel
    {
        [Required, EmailAddress] public string Email { get; set; } = "";
        [Required, MinLength(8)] public string Password { get; set; } = "";
    }
}
```

---

## Layout Components

### Card

Container with header, content, and footer sections.

```razor
<Card>
    <CardHeader>
        <CardTitle>Account Settings</CardTitle>
        <CardDescription>Manage your profile and preferences.</CardDescription>
    </CardHeader>
    <CardContent>
        <Input Placeholder="Display name" @bind-Value="_name" />
    </CardContent>
    <CardFooter>
        <Button Variant="outline" OnClick="Cancel">Cancel</Button>
        <Button OnClick="Save">Save</Button>
    </CardFooter>
</Card>

@code {
    string _name = "";
    void Cancel() { }
    void Save() { }
}
```

---

### Accordion

Collapsible sections; supports single or multi-open.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `AllowMultiple` | `bool` | `false` | Allow multiple sections open at once |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Accordion>
    <AccordionItem Title="What is Blazor?" Value="q1">
        Blazor is a .NET web framework for building interactive UIs using C# instead of JavaScript.
    </AccordionItem>
    <AccordionItem Title="Does it work with MAUI?" Value="q2">
        Yes — use Blazor Hybrid to embed Blazor inside a native MAUI app.
    </AccordionItem>
    <AccordionItem Title="What CSS framework does MyStackBlazor use?" Value="q3">
        Tailwind CSS v4.
    </AccordionItem>
</Accordion>

@* Allow multiple open at once *@
<Accordion AllowMultiple="true">
    <AccordionItem Title="Section A" Value="a">Content A</AccordionItem>
    <AccordionItem Title="Section B" Value="b">Content B</AccordionItem>
</Accordion>
```

---

### Collapsible

Simple show/hide panel with a custom trigger.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `IsOpen` | `bool` | `false` | Open state |
| `IsOpenChanged` | `EventCallback<bool>` | — | Fires when open state changes |
| `Trigger` | `RenderFragment` | — | Trigger element |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Collapsible @bind-IsOpen="_showDetails">
    <Trigger>
        <Button Variant="ghost">
            Show details
            <Icon Name="@(_showDetails ? "chevron-up" : "chevron-down")" Size="sm" />
        </Button>
    </Trigger>
    <ChildContent>
        <p class="p-4 text-muted-foreground">Hidden details revealed on expand.</p>
    </ChildContent>
</Collapsible>

@code {
    bool _showDetails = false;
}
```

---

### ScrollArea

Overflow container with a custom scrollbar.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `MaxHeight` | `string` | — | CSS max-height (e.g. `"300px"`) |
| `MaxWidth` | `string` | — | CSS max-width |
| `Class` | `string` | — | Extra CSS classes |

```razor
<ScrollArea MaxHeight="300px">
    @foreach (var item in _longList)
    {
        <div class="px-4 py-2 border-b">@item</div>
    }
</ScrollArea>
```

---

### AspectRatio

Locks a child element to a given aspect ratio.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Ratio` | `double` | `1.778` (16/9) | Width ÷ height ratio |
| `Class` | `string` | — | Extra CSS classes |

```razor
@* 16:9 video embed *@
<AspectRatio Ratio="1.778">
    <iframe src="https://www.youtube-nocookie.com/embed/dQw4w9WgXcQ"
            class="w-full h-full" allowfullscreen></iframe>
</AspectRatio>

@* 1:1 square image *@
<AspectRatio Ratio="1">
    <img src="/images/product.jpg" class="object-cover w-full h-full rounded" alt="Product" />
</AspectRatio>
```

---

### GridLayout

Responsive CSS grid wrapper.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Columns` | `int` | `2` | Number of columns |
| `Gap` | `int` | `4` | Tailwind gap scale (e.g. `4` = `gap-4`) |
| `Class` | `string` | — | Extra CSS classes |

```razor
<GridLayout Columns="3" Gap="6">
    <Card><CardContent>Item 1</CardContent></Card>
    <Card><CardContent>Item 2</CardContent></Card>
    <Card><CardContent>Item 3</CardContent></Card>
    <Card><CardContent>Item 4</CardContent></Card>
</GridLayout>
```

---

### StackLayout

Flex row or column layout with alignment controls.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Orientation` | `string` | `"horizontal"` | `horizontal` · `vertical` |
| `Gap` | `int` | `4` | Tailwind gap scale |
| `Wrap` | `bool` | `false` | Allow wrapping |
| `Align` | `string` | `"start"` | `start` · `center` · `end` · `stretch` |
| `Justify` | `string` | `"start"` | `start` · `center` · `end` · `between` · `around` |
| `Class` | `string` | — | Extra CSS classes |

```razor
@* Horizontal toolbar *@
<StackLayout Align="center" Justify="between">
    <Typography Variant="h3">Dashboard</Typography>
    <StackLayout Gap="2">
        <Button Variant="ghost">Export</Button>
        <Button>New item</Button>
    </StackLayout>
</StackLayout>

@* Vertical form *@
<StackLayout Orientation="vertical" Gap="4">
    <Input Placeholder="Name" @bind-Value="_name" />
    <Input Placeholder="Email" @bind-Value="_email" />
    <Button Type="submit">Submit</Button>
</StackLayout>
```

---

### Carousel

Slideshow with arrows and dot indicators.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ShowArrows` | `bool` | `true` | Show previous/next arrows |
| `ShowDots` | `bool` | `true` | Show dot page indicators |
| `Loop` | `bool` | `false` | Loop back to first slide |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Carousel ShowArrows="true" ShowDots="true" Loop="true">
    <CarouselSlide>
        <img src="/slides/slide1.jpg" class="w-full object-cover rounded" alt="Slide 1" />
    </CarouselSlide>
    <CarouselSlide>
        <img src="/slides/slide2.jpg" class="w-full object-cover rounded" alt="Slide 2" />
    </CarouselSlide>
    <CarouselSlide>
        <div class="p-8 bg-primary text-primary-foreground rounded text-center">
            <Typography Variant="h2">Special Offer</Typography>
            <Button Variant="secondary" Class="mt-4">Learn more</Button>
        </div>
    </CarouselSlide>
</Carousel>
```

---

### PanelBar

Accordion-style panel navigation.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `AllowMultiple` | `bool` | `false` | Allow multiple panels open |
| `Class` | `string` | — | Extra CSS classes |

```razor
<PanelBar>
    <PanelBarItem Title="General Settings" Key="general">
        <PanelBarItem Title="Profile" Key="profile">Profile settings here</PanelBarItem>
        <PanelBarItem Title="Security" Key="security">Security options here</PanelBarItem>
    </PanelBarItem>
    <PanelBarItem Title="Notifications" Key="notifications">
        Notification preferences here
    </PanelBarItem>
</PanelBar>
```

---

### Loader

Full-page or inline loading overlay.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Visible` | `bool` | `true` | Show/hide the loader |
| `FullScreen` | `bool` | `false` | Cover the entire viewport |
| `Text` | `string` | — | Loading message |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Loader Visible="_loading" Text="Fetching data…" />

@* Full-screen overlay during navigation *@
<Loader Visible="_navigating" FullScreen="true" Text="Loading page…" />

@code {
    bool _loading = true;
    bool _navigating = false;
}
```

---

### Progress / ChunkProgressBar

**Progress** — continuous progress bar.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `double` | — | Current value |
| `Max` | `double` | `100` | Maximum value |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Progress Value="65" Max="100" />
<Progress Value="@_uploadProgress" />

@code {
    double _uploadProgress = 0;
}
```

**ChunkProgressBar** — segmented step progress.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Value` | `double` | — | Current value |
| `Max` | `double` | `100` | Maximum value |
| `Chunks` | `int` | `5` | Number of segments |
| `Class` | `string` | — | Extra CSS classes |

```razor
<ChunkProgressBar Value="3" Max="5" Chunks="5" />
```

---

### AnimationContainer

Conditionally shows/hides content with CSS transitions.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Visible` | `bool` | `true` | Show/hide content |
| `EnterClass` | `string` | `"animate-fade-in"` | CSS class applied on enter |
| `ExitClass` | `string` | `"opacity-0"` | CSS class applied on exit |
| `HideWhenInvisible` | `bool` | `true` | Adds `hidden` when not visible |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Button OnClick="Toggle">Toggle panel</Button>

<AnimationContainer Visible="_show" EnterClass="animate-fade-in" ExitClass="opacity-0">
    <Card>
        <CardContent>Animated content</CardContent>
    </Card>
</AnimationContainer>

@code {
    bool _show = true;
    void Toggle() => _show = !_show;
}
```

---

## Navigation Components

### Tabs

Tabbed panel set.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ActiveTab` | `string` | — | Currently active tab value |
| `ActiveTabChanged` | `EventCallback<string>` | — | Fires when active tab changes |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Tabs @bind-ActiveTab="_tab">
    <TabsList>
        <TabsTrigger Value="account">Account</TabsTrigger>
        <TabsTrigger Value="password">Password</TabsTrigger>
        <TabsTrigger Value="billing">Billing</TabsTrigger>
    </TabsList>

    <TabsContent Value="account">
        <Card>
            <CardContent>
                <Input Placeholder="Display name" @bind-Value="_name" />
            </CardContent>
        </Card>
    </TabsContent>

    <TabsContent Value="password">
        <Card>
            <CardContent>
                <Input Type="password" Placeholder="New password" @bind-Value="_pwd" />
            </CardContent>
        </Card>
    </TabsContent>

    <TabsContent Value="billing">
        <Card><CardContent>Billing info here.</CardContent></Card>
    </TabsContent>
</Tabs>

@code {
    string _tab = "account";
    string _name = "";
    string _pwd = "";
}
```

---

### Breadcrumb

Navigation trail.

```razor
<Breadcrumb>
    <BreadcrumbItem Href="/">Home</BreadcrumbItem>
    <BreadcrumbItem Href="/products">Products</BreadcrumbItem>
    <BreadcrumbItem IsCurrentPage="true">Laptop Pro X</BreadcrumbItem>
</Breadcrumb>
```

---

### Pagination

Page navigation controls.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `CurrentPage` | `int` | — | Active page number |
| `TotalPages` | `int` | — | Total page count |
| `OnPageChange` | `EventCallback<int>` | — | Fires when a page is selected |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Pagination CurrentPage="_page" TotalPages="_total" OnPageChange="GoToPage" />

@code {
    int _page = 1;
    int _total = 10;
    void GoToPage(int p) => _page = p;
}
```

---

### Sidebar

Collapsible side navigation with main content area.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `IsCollapsed` | `bool` | `false` | Collapsed state |
| `IsCollapsedChanged` | `EventCallback<bool>` | — | Fires when collapsed state changes |
| `ShowToggle` | `bool` | `true` | Show the collapse toggle button |
| `CollapsedWidth` | `string` | `"4rem"` | Width when collapsed |
| `ExpandedWidth` | `string` | `"16rem"` | Width when expanded |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Sidebar @bind-IsCollapsed="_collapsed">
    <ChildContent>
        <nav class="p-4 space-y-2">
            <a href="/" class="flex items-center gap-2 p-2 rounded hover:bg-accent">
                <Icon Name="home" Size="sm" />
                <span>Dashboard</span>
            </a>
            <a href="/users" class="flex items-center gap-2 p-2 rounded hover:bg-accent">
                <Icon Name="users" Size="sm" />
                <span>Users</span>
            </a>
        </nav>
    </ChildContent>
    <MainContent>
        <main class="p-6">Main content goes here</main>
    </MainContent>
</Sidebar>

@code {
    bool _collapsed = false;
}
```

---

### Menubar

Horizontal application menu bar.

```razor
<Menubar>
    <MenubarMenu Label="File">
        <DropdownMenuItem OnClick="NewFile">New</DropdownMenuItem>
        <DropdownMenuItem OnClick="OpenFile">Open</DropdownMenuItem>
        <Separator />
        <DropdownMenuItem OnClick="Save">Save</DropdownMenuItem>
    </MenubarMenu>
    <MenubarMenu Label="Edit">
        <DropdownMenuItem>Undo</DropdownMenuItem>
        <DropdownMenuItem>Redo</DropdownMenuItem>
        <Separator />
        <DropdownMenuItem>Cut</DropdownMenuItem>
        <DropdownMenuItem>Copy</DropdownMenuItem>
        <DropdownMenuItem>Paste</DropdownMenuItem>
    </MenubarMenu>
    <MenubarMenu Label="View">
        <DropdownMenuItem>Zoom In</DropdownMenuItem>
        <DropdownMenuItem>Zoom Out</DropdownMenuItem>
    </MenubarMenu>
</Menubar>

@code {
    void NewFile() { }
    void OpenFile() { }
    void Save() { }
}
```

---

### DropdownMenu

Trigger + floating dropdown menu.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Align` | `string` | `"start"` | `start` · `center` · `end` |
| `Class` | `string` | — | Extra CSS classes |

```razor
<DropdownMenu>
    <Trigger>
        <Button Variant="outline">
            Options <Icon Name="chevron-down" Size="sm" Class="ml-1" />
        </Button>
    </Trigger>
    <Content>
        <DropdownMenuItem OnClick="Edit">
            <Icon>
                <Icon Name="pencil" Size="sm" />
            </Icon>
            Edit
        </DropdownMenuItem>
        <DropdownMenuItem OnClick="Duplicate">Duplicate</DropdownMenuItem>
        <Separator />
        <DropdownMenuItem Destructive="true" OnClick="Delete">
            <Icon>
                <Icon Name="trash" Size="sm" />
            </Icon>
            Delete
        </DropdownMenuItem>
    </Content>
</DropdownMenu>

@code {
    void Edit() { }
    void Duplicate() { }
    void Delete() { }
}
```

---

### ContextMenu

Right-click context menu.

```razor
<ContextMenu>
    <ChildContent>
        <div class="p-8 border rounded select-none">Right-click me</div>
    </ChildContent>
    <MenuContent>
        <ContextMenuItem OnClick="Open">Open</ContextMenuItem>
        <ContextMenuItem OnClick="Rename">Rename</ContextMenuItem>
        <Separator />
        <ContextMenuItem Destructive="true" OnClick="Delete">Delete</ContextMenuItem>
    </MenuContent>
</ContextMenu>

@code {
    void Open() { }
    void Rename() { }
    void Delete() { }
}
```

---

### AppBar

Top application bar with start / center / end slots.

```razor
<AppBar>
    <StartContent>
        <Button Variant="ghost" Size="icon">
            <Icon Name="menu" />
        </Button>
        <Typography Variant="h4" Class="ml-2">MyApp</Typography>
    </StartContent>
    <ChildContent>
        <Input Placeholder="Search…" Class="w-64" />
    </ChildContent>
    <EndContent>
        <ThemeToggle />
        <Avatar Initials="VS" Size="sm" />
    </EndContent>
</AppBar>
```

---

### Toolbar

Horizontal toolbar strip.

```razor
<Toolbar>
    <Button Variant="ghost" Size="icon"><Icon Name="bold" /></Button>
    <Button Variant="ghost" Size="icon"><Icon Name="italic" /></Button>
    <Button Variant="ghost" Size="icon"><Icon Name="underline" /></Button>
    <Separator Orientation="vertical" Class="h-6 mx-1" />
    <Button Variant="ghost" Size="icon"><Icon Name="align-left" /></Button>
    <Button Variant="ghost" Size="icon"><Icon Name="align-center" /></Button>
    <Button Variant="ghost" Size="icon"><Icon Name="align-right" /></Button>
</Toolbar>
```

---

### ButtonGroup / ToggleButton

**ButtonGroup** — visually joined set of buttons.

```razor
<ButtonGroup>
    <Button Variant="outline">Day</Button>
    <Button Variant="outline">Week</Button>
    <Button>Month</Button>
</ButtonGroup>
```

**ToggleButton** — pressed/unpressed toggle.

```razor
<ToggleButton @bind-IsPressed="_bold">
    <Icon Name="bold" Size="sm" />
</ToggleButton>

@code {
    bool _bold = false;
}
```

---

### FloatingActionButton

Fixed-position primary action button.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Size` | `string` | `"md"` | `sm` · `md` · `lg` |
| `Position` | `string` | `"bottom-right"` | `bottom-right` · `bottom-left` · `top-right` · `top-left` |
| `OnClick` | `EventCallback` | — | Click handler |
| `Class` | `string` | — | Extra CSS classes |

```razor
<FloatingActionButton OnClick="CreateNew" Position="bottom-right">
    <Icon Name="plus" />
</FloatingActionButton>

@code {
    void CreateNew() { /* open create dialog */ }
}
```

---

### DropDownButton / SplitButton

**DropDownButton** — button that opens a dropdown.

```razor
<DropDownButton Text="Export">
    <MenuContent>
        <DropdownMenuItem OnClick="ExportCsv">Export as CSV</DropdownMenuItem>
        <DropdownMenuItem OnClick="ExportPdf">Export as PDF</DropdownMenuItem>
        <DropdownMenuItem OnClick="ExportExcel">Export as Excel</DropdownMenuItem>
    </MenuContent>
</DropDownButton>

@code {
    void ExportCsv() { }
    void ExportPdf() { }
    void ExportExcel() { }
}
```

**SplitButton** — primary action + dropdown for additional actions.

```razor
<SplitButton Text="Save" OnClick="Save">
    <MenuContent>
        <DropdownMenuItem OnClick="SaveDraft">Save as Draft</DropdownMenuItem>
        <DropdownMenuItem OnClick="SavePublish">Save and Publish</DropdownMenuItem>
    </MenuContent>
</SplitButton>

@code {
    void Save() { }
    void SaveDraft() { }
    void SavePublish() { }
}
```

---

### Stepper

Step progress indicator.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Vertical` | `bool` | `false` | Vertical layout |
| `Linear` | `bool` | `false` | Enforce sequential completion |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Stepper>
    <StepperStep Title="Account" Description="Create your account" />
    <StepperStep Title="Profile" Description="Set up your profile" />
    <StepperStep Title="Review" Description="Confirm your details" />
</Stepper>

<Stepper Vertical="true">
    <StepperStep Title="Order placed" Description="Your order has been received." />
    <StepperStep Title="Processing" Description="We are preparing your order." />
    <StepperStep Title="Shipped" Description="On the way to you." />
    <StepperStep Title="Delivered" Description="Package delivered." />
</Stepper>
```

---

### Wizard

Multi-step form wizard with built-in navigation.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ShowNavigation` | `bool` | `true` | Show Previous / Next buttons |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Wizard>
    <WizardStep Title="Personal Info">
        <Input Placeholder="Full name" @bind-Value="_name" />
        <Input Type="email" Placeholder="Email" @bind-Value="_email" />
    </WizardStep>
    <WizardStep Title="Address">
        <Input Placeholder="Street address" @bind-Value="_address" />
        <Input Placeholder="City" @bind-Value="_city" />
    </WizardStep>
    <WizardStep Title="Confirm">
        <p>Name: @_name</p>
        <p>Email: @_email</p>
        <p>Address: @_address, @_city</p>
    </WizardStep>
</Wizard>

@code {
    string _name = "", _email = "", _address = "", _city = "";
}
```

---

### Workflow

Visual process/workflow diagram.

```razor
<Workflow>
    <WorkflowStep Title="Submit Request" Status="completed" />
    <WorkflowStep Title="Manager Approval" Status="active" />
    <WorkflowStep Title="Finance Review" Status="pending" />
    <WorkflowStep Title="Completed" Status="pending" />
</Workflow>
```

---

## Overlay Components

### Dialog

Modal dialog with backdrop.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `IsOpen` | `bool` | — | Open state |
| `IsOpenChanged` | `EventCallback<bool>` | — | Fires when open state changes |
| `CloseOnBackdropClick` | `bool` | `true` | Close when backdrop is clicked |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Button OnClick="@(() => _open = true)">Open Dialog</Button>

<Dialog @bind-IsOpen="_open">
    <DialogHeader>
        <DialogTitle>Edit Profile</DialogTitle>
    </DialogHeader>
    <div class="py-4">
        <Input Placeholder="Display name" @bind-Value="_name" />
    </div>
    <DialogFooter>
        <Button Variant="outline" OnClick="@(() => _open = false)">Cancel</Button>
        <Button OnClick="SaveAndClose">Save changes</Button>
    </DialogFooter>
</Dialog>

@code {
    bool _open = false;
    string _name = "";

    void SaveAndClose()
    {
        _open = false;
    }
}
```

---

### AlertDialog

Confirmation dialog with cancel and confirm actions.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `IsOpen` | `bool` | — | Open state |
| `IsOpenChanged` | `EventCallback<bool>` | — | Fires when open state changes |
| `Title` | `string` | — | Dialog title |
| `Description` | `string` | — | Dialog body text |
| `CancelText` | `string` | `"Cancel"` | Cancel button label |
| `ConfirmText` | `string` | `"Continue"` | Confirm button label |
| `OnConfirm` | `EventCallback` | — | Fires when confirmed |
| `OnCancel` | `EventCallback` | — | Fires when cancelled |

```razor
<Button Variant="destructive" OnClick="@(() => _confirmDelete = true)">Delete account</Button>

<AlertDialog @bind-IsOpen="_confirmDelete"
             Title="Are you absolutely sure?"
             Description="This action cannot be undone. Your account and all data will be permanently deleted."
             ConfirmText="Yes, delete account"
             CancelText="Cancel"
             OnConfirm="DeleteAccount"
             OnCancel="@(() => _confirmDelete = false)" />

@code {
    bool _confirmDelete = false;
    void DeleteAccount() { /* delete logic */ }
}
```

---

### Sheet

Slide-in panel from any side.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `IsOpen` | `bool` | — | Open state |
| `IsOpenChanged` | `EventCallback<bool>` | — | Fires when open state changes |
| `Side` | `string` | `"right"` | `right` · `left` · `top` · `bottom` |
| `CloseOnBackdropClick` | `bool` | `true` | Close on backdrop click |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Button OnClick="@(() => _sheetOpen = true)">Open Sheet</Button>

<Sheet @bind-IsOpen="_sheetOpen" Side="right">
    <div class="p-6">
        <Typography Variant="h3">Filters</Typography>
        <Separator Class="my-4" />
        <Label For="status">Status</Label>
        <Select @bind-Value="_status" Placeholder="All statuses">
            <option value="active">Active</option>
            <option value="inactive">Inactive</option>
        </Select>
        <Button Class="mt-6 w-full" OnClick="ApplyFilters">Apply</Button>
    </div>
</Sheet>

@code {
    bool _sheetOpen = false;
    string _status = "";
    void ApplyFilters() => _sheetOpen = false;
}
```

---

### Drawer

Bottom-sheet / drawer overlay.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `IsOpen` | `bool` | — | Open state |
| `IsOpenChanged` | `EventCallback<bool>` | — | Fires when open state changes |
| `CloseOnBackdropClick` | `bool` | `true` | Close on backdrop click |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Button OnClick="@(() => _drawerOpen = true)">Open Drawer</Button>

<Drawer @bind-IsOpen="_drawerOpen">
    <div class="p-6">
        <Typography Variant="h3">Quick Actions</Typography>
        <StackLayout Orientation="vertical" Gap="2" Class="mt-4">
            <Button Variant="outline">Export report</Button>
            <Button Variant="outline">Send notification</Button>
            <Button Variant="destructive">Reset data</Button>
        </StackLayout>
    </div>
</Drawer>

@code {
    bool _drawerOpen = false;
}
```

---

### Popover

Floating content anchored to a trigger.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Align` | `string` | `"start"` | `start` · `center` · `end` |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Popover>
    <Trigger>
        <Button Variant="outline">Open popover</Button>
    </Trigger>
    <Content>
        <div class="p-4 w-64">
            <Typography Variant="h4">Dimensions</Typography>
            <Typography Variant="small" Class="text-muted-foreground mt-1">
                Set the dimensions for the layer.
            </Typography>
            <div class="grid grid-cols-2 gap-2 mt-3">
                <Label For="width">Width</Label>
                <Input Id="width" Placeholder="100px" />
                <Label For="height">Height</Label>
                <Input Id="height" Placeholder="25px" />
            </div>
        </div>
    </Content>
</Popover>
```

---

### Tooltip

Small informational label on hover.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Text` | `string` | — | Tooltip text (simple form) |
| `Side` | `string` | `"top"` | `top` · `bottom` · `left` · `right` |
| `Class` | `string` | — | Extra CSS classes |

```razor
@* Simple text tooltip *@
<Tooltip Text="Save your changes" Side="top">
    <Button Size="icon" Variant="ghost"><Icon Name="save" /></Button>
</Tooltip>

@* Rich content tooltip *@
<Tooltip Side="right">
    <ChildContent>
        <Badge>Pro</Badge>
    </ChildContent>
    <Content>
        <Typography Variant="small">Upgrade to Pro for unlimited access.</Typography>
    </Content>
</Tooltip>
```

---

### HoverCard

Richer hover preview card.

```razor
<HoverCard>
    <Trigger>
        <a href="/users/vilas" class="underline text-primary">@vilas_sagar</a>
    </Trigger>
    <Content>
        <div class="flex gap-3 p-2">
            <Avatar Initials="VS" Size="lg" />
            <div>
                <Typography Variant="h4">Vilas Sagar</Typography>
                <Typography Variant="small" Class="text-muted-foreground">
                    Creator of MyStackBlazor
                </Typography>
            </div>
        </div>
    </Content>
</HoverCard>
```

---

### Toaster

Toast notification system — place `<Toaster />` once in your layout, then inject `ToastService` anywhere.

```razor
@* MainLayout.razor — add once *@
<Toaster />
```

```razor
@* Any page or component *@
@inject ToastService Toast

<Button OnClick="Notify">Show toast</Button>

@code {
    void Notify()
    {
        Toast.ShowSuccess("Saved!", "Your changes have been saved successfully.");
        Toast.ShowError("Error", "Something went wrong.");
        Toast.ShowWarning("Warning", "Low disk space.");
        Toast.ShowInfo("Info", "New version available.");
    }
}
```

---

### Window

Draggable, resizable application window inside the page.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `IsOpen` | `bool` | — | Open state |
| `IsOpenChanged` | `EventCallback<bool>` | — | Fires when open state changes |
| `Title` | `string` | — | Window title bar text |
| `Modal` | `bool` | `true` | Show backdrop overlay |
| `ShowCloseButton` | `bool` | `true` | Show × close button |
| `Width` | `string` | `"md"` | `sm` · `md` · `lg` · `xl` |
| `Height` | `string` | — | Custom CSS height |

```razor
<Button OnClick="@(() => _winOpen = true)">Open Window</Button>

<Window @bind-IsOpen="_winOpen" Title="Log Viewer" Width="lg" Modal="false">
    <ChildContent>
        <ScrollArea MaxHeight="400px">
            @foreach (var line in _logs)
            {
                <div class="font-mono text-xs px-4 py-1 border-b">@line</div>
            }
        </ScrollArea>
    </ChildContent>
    <FooterContent>
        <Button Variant="outline" OnClick="@(() => _winOpen = false)">Close</Button>
    </FooterContent>
</Window>

@code {
    bool _winOpen = false;
    List<string> _logs = ["[INFO] App started", "[WARN] Low memory", "[ERROR] Unhandled exception"];
}
```

---

### FileViewer / ImageViewer

**FileViewer** — renders a file (PDF, text, etc.) inside the page.

```razor
<FileViewer Url="/docs/report.pdf" FileName="report.pdf" />
```

**ImageViewer** — lightbox-style image viewer.

```razor
<ImageViewer Src="/images/photo.jpg" Alt="Landscape photo" />
```

---

## Data Components

### Alert

Inline status message.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Variant` | `string` | `"default"` | `default` · `destructive` |
| `Class` | `string` | — | Extra CSS classes |

```razor
<Alert>
    <Icon>
        <Icon Name="info" Size="sm" />
    </Icon>
    <AlertTitle>Heads up!</AlertTitle>
    <AlertDescription>You can add components to your app using the CLI.</AlertDescription>
</Alert>

<Alert Variant="destructive">
    <Icon>
        <Icon Name="alert-triangle" Size="sm" />
    </Icon>
    <AlertTitle>Error</AlertTitle>
    <AlertDescription>Your session has expired. Please log in again.</AlertDescription>
</Alert>
```

---

### Table

Semantic HTML table with consistent styling.

```razor
<Table>
    <TableHeader>
        <TableRow>
            <TableHead>Name</TableHead>
            <TableHead>Email</TableHead>
            <TableHead>Role</TableHead>
            <TableHead>Status</TableHead>
        </TableRow>
    </TableHeader>
    <TableBody>
        @foreach (var user in _users)
        {
            <TableRow>
                <TableCell>@user.Name</TableCell>
                <TableCell>@user.Email</TableCell>
                <TableCell>@user.Role</TableCell>
                <TableCell>
                    <Badge Variant="@(user.Active ? "default" : "secondary")">
                        @(user.Active ? "Active" : "Inactive")
                    </Badge>
                </TableCell>
            </TableRow>
        }
    </TableBody>
</Table>

@code {
    record UserRow(string Name, string Email, string Role, bool Active);

    List<UserRow> _users =
    [
        new("Alice Smith",  "alice@example.com",  "Admin",  true),
        new("Bob Jones",    "bob@example.com",    "Editor", true),
        new("Carol White",  "carol@example.com",  "Viewer", false),
    ];
}
```

---

### DataTable

Feature-rich data grid with search, sorting, virtualization, and infinite scroll.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Items` | `IEnumerable<TItem>` | — | Data source |
| `Columns` | `List<DataTableColumn<TItem>>` | — | Column definitions |
| `ShowSearch` | `bool` | `false` | Show search box |
| `Striped` | `bool` | `false` | Alternating row colors |
| `Hover` | `bool` | `false` | Row hover highlight |
| `Bordered` | `bool` | `false` | Cell borders |
| `Compact` | `bool` | `false` | Reduced row padding |
| `Virtualize` | `bool` | `false` | Virtual scrolling for large datasets |
| `VirtualizeHeight` | `string` | — | Container height for virtual scroll |
| `InfiniteScroll` | `bool` | `false` | Load more on scroll |
| `OnLoadMore` | `EventCallback` | — | Fires when more data is needed |
| `ToolbarContent` | `RenderFragment` | — | Slot for toolbar actions |

```razor
<DataTable TItem="Product"
           Items="_products"
           Columns="_columns"
           ShowSearch="true"
           Striped="true"
           Hover="true">
    <ToolbarContent>
        <Button Size="sm">Export</Button>
    </ToolbarContent>
</DataTable>

@code {
    record Product(int Id, string Name, decimal Price, string Category);

    List<Product> _products =
    [
        new(1, "Laptop Pro",  1299.99m, "Electronics"),
        new(2, "Desk Chair",   349.00m, "Furniture"),
        new(3, "Monitor 27\"", 499.00m, "Electronics"),
    ];

    List<DataTableColumn<Product>> _columns =
    [
        new() { Header = "ID",       Value = p => p.Id.ToString() },
        new() { Header = "Name",     Value = p => p.Name },
        new() { Header = "Category", Value = p => p.Category },
        new() { Header = "Price",    Value = p => p.Price.ToString("C") },
    ];
}
```

---

### TreeView

Hierarchical tree with expand/collapse and item selection.

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Items` | `IEnumerable<TItem>` | — | Root-level items |
| `GetChildren` | `Func<TItem, IEnumerable<TItem>>` | — | Returns children of an item |
| `GetLabel` | `Func<TItem, string>` | — | Returns display label for an item |
| `SelectedItem` | `TItem` | — | Currently selected item |
| `SelectedItemChanged` | `EventCallback<TItem>` | — | Fires when selection changes |
| `ItemTemplate` | `RenderFragment<TItem>` | — | Custom item renderer |
| `Class` | `string` | — | Extra CSS classes |

```razor
<TreeView TItem="FileNode"
          Items="_root"
          GetChildren="n => n.Children"
          GetLabel="n => n.Name"
          @bind-SelectedItem="_selected" />

@if (_selected is not null)
{
    <p class="mt-2 text-sm text-muted-foreground">Selected: @_selected.Name</p>
}

@code {
    class FileNode
    {
        public string Name { get; set; } = "";
        public List<FileNode> Children { get; set; } = [];
    }

    FileNode? _selected;

    List<FileNode> _root =
    [
        new FileNode
        {
            Name = "src",
            Children =
            [
                new() { Name = "Components", Children = [new() { Name = "Button.razor" }] },
                new() { Name = "Pages",      Children = [new() { Name = "Index.razor"  }] },
            ]
        },
        new FileNode
        {
            Name = "wwwroot",
            Children = [new() { Name = "css" }, new() { Name = "js" }]
        },
    ];
}
```

# License

MIT © 2025 Vilas Sagar
