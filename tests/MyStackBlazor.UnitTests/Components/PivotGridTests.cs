using MyStackBlazor.Components.PivotGrid;

namespace MyStackBlazor.UnitTests.Components;

// ── Test model ────────────────────────────────────────────────────────────────

internal sealed class Product
{
    public int    Id       { get; init; }
    public string Name     { get; init; } = "";
    public string Category { get; init; } = "";
    public decimal Price   { get; init; }
    public bool   Active   { get; init; } = true;
}

// ═══════════════════════════════════════════════════════════════════════════════
// MsPivotGrid
// ═══════════════════════════════════════════════════════════════════════════════

public class MsPivotGridTests : TestContext
{
    // ── Shared fixtures ───────────────────────────────────────────────────────

    private static readonly List<Product> SampleItems =
    [
        new() { Id = 1, Name = "Apple",      Category = "Fruit",     Price = 1.20m  },
        new() { Id = 2, Name = "Banana",     Category = "Fruit",     Price = 0.80m  },
        new() { Id = 3, Name = "Carrot",     Category = "Vegetable", Price = 0.50m  },
        new() { Id = 4, Name = "Daikon",     Category = "Vegetable", Price = 1.80m  },
        new() { Id = 5, Name = "Elderberry", Category = "Fruit",     Price = 4.50m  },
    ];

    private static List<PivotColumn<Product>> Columns =>
    [
        new() { Key = "Id",       Title = "ID",       ValueSelector = x => x.Id,       Sortable = true,  Filterable = false },
        new() { Key = "Name",     Title = "Name",     ValueSelector = x => x.Name,     Sortable = true,  Filterable = true  },
        new() { Key = "Category", Title = "Category", ValueSelector = x => x.Category, Sortable = true,  Filterable = true  },
        new() { Key = "Price",    Title = "Price",    ValueSelector = x => x.Price,    Sortable = true,  Filterable = false,
                Type = PivotColumnType.Number, Format = "N2" },
        new() { Key = "Active",   Title = "Active",   ValueSelector = x => x.Active,   Sortable = false, Filterable = false,
                Type = PivotColumnType.Boolean },
    ];

    // ── Rendering ─────────────────────────────────────────────────────────────

    [Fact]
    public void PivotGrid_Renders_TableElement()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns));

        cut.Find("table").Should().NotBeNull();
    }

    [Fact]
    public void PivotGrid_RendersAllItems_AsRows()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns)
            .Add(g => g.ShowPagination, false));

        // tbody rows = SampleItems count
        cut.FindAll("tbody tr").Count.Should().Be(SampleItems.Count);
    }

    [Fact]
    public void PivotGrid_RendersColumnHeaders()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns));

        var headerText = cut.Find("thead").TextContent;
        headerText.Should().Contain("Name");
        headerText.Should().Contain("Category");
        headerText.Should().Contain("Price");
    }

    [Fact]
    public void PivotGrid_RendersItemValues_InCells()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns)
            .Add(g => g.ShowPagination, false));

        cut.Find("tbody").TextContent.Should().Contain("Apple");
        cut.Find("tbody").TextContent.Should().Contain("Banana");
        cut.Find("tbody").TextContent.Should().Contain("Carrot");
    }

    // ── Empty state ───────────────────────────────────────────────────────────

    [Fact]
    public void PivotGrid_EmptyItems_ShowsEmptyMessage()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, [])
            .Add(g => g.Columns, Columns)
            .Add(g => g.EmptyMessage, "No products found"));

        cut.Markup.Should().Contain("No products found");
    }

    [Fact]
    public void PivotGrid_EmptyItems_NoDataRows()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, [])
            .Add(g => g.Columns, Columns));

        // Only the empty-state row
        cut.FindAll("tbody tr").Count.Should().Be(1);
    }

    // ── Row count badge ───────────────────────────────────────────────────────

    [Fact]
    public void PivotGrid_RowCountBadge_ShowsTotalCount()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns));

        cut.Markup.Should().Contain($"{SampleItems.Count} rows");
    }

    // ── Search bar ────────────────────────────────────────────────────────────

    [Fact]
    public void PivotGrid_ShowSearchBar_True_RendersSearchInput()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns)
            .Add(g => g.ShowSearchBar, true));

        cut.Find("input[placeholder='Search all columns…']").Should().NotBeNull();
    }

    [Fact]
    public void PivotGrid_ShowSearchBar_False_HidesSearchInput()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns)
            .Add(g => g.ShowSearchBar, false));

        cut.FindAll("input[placeholder='Search all columns…']").Count.Should().Be(0);
    }

    [Fact]
    public async Task PivotGrid_GlobalSearch_FiltersRows()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns)
            .Add(g => g.ShowSearchBar, true)
            .Add(g => g.ShowPagination, false));

        var searchInput = cut.Find("input[placeholder='Search all columns…']");
        await searchInput.InputAsync(new ChangeEventArgs { Value = "apple" });

        cut.Find("tbody").TextContent.Should().Contain("Apple");
        cut.Find("tbody").TextContent.Should().NotContain("Banana");
    }

    [Fact]
    public async Task PivotGrid_GlobalSearch_CaseInsensitive()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns)
            .Add(g => g.ShowSearchBar, true)
            .Add(g => g.ShowPagination, false));

        await cut.Find("input[placeholder='Search all columns…']")
                 .InputAsync(new ChangeEventArgs { Value = "FRUIT" });

        // Apple, Banana, Elderberry are fruits
        cut.Find("tbody").TextContent.Should().Contain("Apple");
        cut.Find("tbody").TextContent.Should().NotContain("Carrot");
    }

    // ── Column filters ────────────────────────────────────────────────────────

    [Fact]
    public async Task PivotGrid_ColumnFilter_FiltersToMatchingRows()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns)
            .Add(g => g.ShowPagination, false));

        // Column filter for "Name" column
        var filterInputs = cut.FindAll("input[placeholder='Name…']");
        await filterInputs[0].InputAsync(new ChangeEventArgs { Value = "carrot" });

        cut.Find("tbody").TextContent.Should().Contain("Carrot");
        cut.Find("tbody").TextContent.Should().NotContain("Apple");
    }

    // ── Sorting ───────────────────────────────────────────────────────────────

    [Fact]
    public async Task PivotGrid_ClickSortableHeader_SortsAscending()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns)
            .Add(g => g.ShowPagination, false));

        // Click the "Name" header to sort ascending
        var nameHeader = cut.FindAll("thead th div")
            .First(th => th.TextContent.Contains("Name"));
        await nameHeader.ClickAsync(new MouseEventArgs());

        var firstRow = cut.FindAll("tbody tr")[0].TextContent;
        firstRow.Should().Contain("Apple"); // A comes first alphabetically
    }

    [Fact]
    public async Task PivotGrid_ClickSortableHeader_Twice_SortsDescending()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns)
            .Add(g => g.ShowPagination, false));

        var nameHeader = cut.FindAll("thead th div")
            .First(th => th.TextContent.Contains("Name"));
        await nameHeader.ClickAsync(new MouseEventArgs()); // asc

        // Re-find after the re-render triggered by the first click
        nameHeader = cut.FindAll("thead th div")
            .First(th => th.TextContent.Contains("Name"));
        await nameHeader.ClickAsync(new MouseEventArgs()); // desc

        var firstRow = cut.FindAll("tbody tr")[0].TextContent;
        // "Elderberry" comes last alphabetically so it's first descending
        firstRow.Should().NotContain("Apple");
    }

    // ── Row numbers ───────────────────────────────────────────────────────────

    [Fact]
    public void PivotGrid_ShowRowNumbers_True_RendersNumberColumn()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns)
            .Add(g => g.ShowRowNumbers, true));

        // First header cell is the "#" column
        cut.Find("thead th").TextContent.Trim().Should().Be("#");
    }

    [Fact]
    public void PivotGrid_ShowRowNumbers_False_NoNumberColumn()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns)
            .Add(g => g.ShowRowNumbers, false));

        cut.Find("thead th").TextContent.Trim().Should().NotBe("#");
    }

    // ── Alternate row colour ──────────────────────────────────────────────────

    [Fact]
    public void PivotGrid_AlternateRowColor_True_SecondRowHasAltClass()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns)
            .Add(g => g.AlternateRowColor, true)
            .Add(g => g.ShowPagination, false));

        var rows = cut.FindAll("tbody tr");
        // Default alt class contains "bg-muted"
        rows[1].ClassName.Should().Contain("bg-muted");
    }

    // ── Row click ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task PivotGrid_RowClick_FiresOnRowClick()
    {
        Product? clicked = null;
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns)
            .Add(g => g.ShowPagination, false)
            .Add(g => g.OnRowClick,
                EventCallback.Factory.Create<Product>(this, p2 => clicked = p2)));

        await cut.FindAll("tbody tr")[0].ClickAsync(new MouseEventArgs());

        clicked.Should().NotBeNull();
        clicked!.Name.Should().Be("Apple");
    }

    // ── Cell click ────────────────────────────────────────────────────────────

    [Fact]
    public async Task PivotGrid_CellClick_FiresOnCellClick()
    {
        CellClickArgs<Product>? args = null;
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns)
            .Add(g => g.ShowPagination, false)
            .Add(g => g.ShowRowNumbers, false)  // row-number td has no click handler; skip it
            .Add(g => g.OnCellClick,
                EventCallback.Factory.Create<CellClickArgs<Product>>(this, a => args = a)));

        var firstBodyCell = cut.FindAll("tbody tr td").First();
        await firstBodyCell.ClickAsync(new MouseEventArgs());

        args.Should().NotBeNull();
        args!.Row.Should().NotBeNull();
    }

    // ── Column visibility ─────────────────────────────────────────────────────

    [Fact]
    public void PivotGrid_HiddenColumn_NotVisibleInHeader()
    {
        var columns = Columns.ToList();
        columns.Add(new PivotColumn<Product>
        {
            Key = "Hidden", Title = "Secret", Hidden = true,
            ValueSelector = x => x.Id,
        });

        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, columns));

        cut.Find("thead").TextContent.Should().NotContain("Secret");
    }

    [Fact]
    public void PivotGrid_ShowColumnToggle_True_RendersColumnsButton()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns)
            .Add(g => g.ShowColumnToggle, true));

        cut.FindAll("button").Select(b => b.TextContent)
           .Should().ContainMatch("*Columns*");
    }

    // ── Pagination ────────────────────────────────────────────────────────────

    [Fact]
    public void PivotGrid_ShowPagination_True_RendersPaginationFooter()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns)
            .Add(g => g.ShowPagination, true));

        // Pagination footer contains "Showing" or page buttons
        cut.Markup.Should().ContainAny("Showing", "page");
    }

    [Fact]
    public void PivotGrid_ShowPagination_False_HidesPaginationFooter()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns)
            .Add(g => g.ShowPagination, false));

        cut.Markup.Should().NotContain("Showing");
    }

    [Fact]
    public void PivotGrid_PageSize_LimitingRows()
    {
        // 5 items, page size 2 → first page shows 2 rows
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns)
            .Add(g => g.ShowPagination, true)
            .Add(g => g.PageSize, 2));

        cut.FindAll("tbody tr").Count.Should().Be(2);
    }

    // ── Number formatting ─────────────────────────────────────────────────────

    [Fact]
    public void PivotGrid_NumberColumn_FormattedWithFormat()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, [new Product { Id = 1, Name = "Apple", Price = 1.20m }])
            .Add(g => g.Columns, Columns)
            .Add(g => g.ShowPagination, false));

        // Price formatted as N2 → "1.20"
        cut.Find("tbody").TextContent.Should().Contain("1.20");
    }

    [Fact]
    public void PivotGrid_BooleanColumn_ShowsYesNo()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, [
                new Product { Id = 1, Name = "A", Active = true },
                new Product { Id = 2, Name = "B", Active = false },
            ])
            .Add(g => g.Columns, Columns)
            .Add(g => g.ShowPagination, false));

        var bodyText = cut.Find("tbody").TextContent;
        bodyText.Should().Contain("Yes");
        bodyText.Should().Contain("No");
    }

    // ── Clear filters ─────────────────────────────────────────────────────────

    [Fact]
    public async Task PivotGrid_ClearFilters_RemovesSearchFilter()
    {
        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems)
            .Add(g => g.Columns, Columns)
            .Add(g => g.ShowSearchBar, true)
            .Add(g => g.ShowPagination, false));

        // Apply filter
        await cut.Find("input[placeholder='Search all columns…']")
                 .InputAsync(new ChangeEventArgs { Value = "apple" });

        cut.Find("tbody").TextContent.Should().Contain("Apple");
        cut.Find("tbody").TextContent.Should().NotContain("Banana");

        // Clear filters button appears
        var clearBtn = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Trim() == "Clear filters");
        clearBtn.Should().NotBeNull();
        await clearBtn!.ClickAsync(new MouseEventArgs());

        // All rows visible again
        cut.Find("tbody").TextContent.Should().Contain("Banana");
    }

    // ── Custom CellClass ──────────────────────────────────────────────────────

    [Fact]
    public void PivotGrid_CellClass_AppliedToMatchingCells()
    {
        var columns = new List<PivotColumn<Product>>
        {
            new()
            {
                Key           = "Price",
                Title         = "Price",
                ValueSelector = x => x.Price,
                CellClass     = x => x.Price > 3m ? "font-bold text-green-600" : null,
                Filterable    = false,
            },
        };

        var cut = RenderComponent<MsPivotGrid<Product>>(p => p
            .Add(g => g.Items, SampleItems) // Elderberry has Price = 4.50
            .Add(g => g.Columns, columns)
            .Add(g => g.ShowPagination, false));

        cut.FindAll("tbody td")
           .Any(td => td.ClassName.Contains("font-bold"))
           .Should().BeTrue();
    }
}
