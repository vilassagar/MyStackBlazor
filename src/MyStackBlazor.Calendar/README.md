# MyStackBlazor.Calendar

Full-featured calendar and scheduler for Blazor WASM, Blazor Server and .NET MAUI Blazor Hybrid. It is part of the [MyStackBlazor](https://www.nuget.org/packages/MyStackBlazor) component library.

- Day, Week, Work week, Month, Year, Agenda and Timeline views
- Recurring events, multi-person calendars, category filtering
- Mini-calendar sidebar, drag and drop, add/edit/delete dialogs

```bash
dotnet add package MyStackBlazor.Calendar
```

```csharp
builder.Services.AddMyStackBlazor();
builder.Services.AddMyStackBlazorCalendar();
```

```razor
@using MyStackBlazor.Calendar

<StackCalendar Events="_events"
               OnEventAdd="e => _events.Add(e)"
               OnEventDelete="e => _events.Remove(e)" />
```

- Live demo: https://vilassagar.github.io/MyStackBlazor/
- Source: https://github.com/vilassagar/MyStackBlazor
