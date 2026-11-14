# MyStackBlazor.DataGrid

Enterprise data grid for Blazor: server-side paging, multi-column sorting, filtering, grouping, row virtualization, column chooser, and Excel export (via ClosedXML). It is part of the [MyStackBlazor](https://www.nuget.org/packages/MyStackBlazor) component library.

```bash
dotnet add package MyStackBlazor.DataGrid
```

```csharp
builder.Services.AddMyStackBlazor();
builder.Services.AddMyStackBlazorDataGrid();
```

```razor
@using MyStackBlazor.DataGrid

<MsDataGrid TItem="Product" DataProvider="_provider" BodyHeight="400px" AriaLabel="Products">
    <MsDataGridColumn TItem="Product" Field="Id"       Title="ID" Width="72px" />
    <MsDataGridColumn TItem="Product" Field="Name"     Title="Name" Filterable="true" />
    <MsDataGridColumn TItem="Product" Field="Category" Title="Category" Groupable="true" />
</MsDataGrid>
```

Implement `IGridDataProvider<TItem>.GetDataAsync(GridRequest, CancellationToken)` to page, sort and filter on the server.

- Live demo: https://vilassagar.github.io/MyStackBlazor/
- Source: https://github.com/vilassagar/MyStackBlazor
