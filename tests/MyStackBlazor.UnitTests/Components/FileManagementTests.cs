using MyStackBlazor.Components.Data;

namespace MyStackBlazor.UnitTests.Components;

public class FileManagerTests : TestContext
{
    // ── Sample file system tree ────────────────────────────────────────────

    private static FileManagerItem MakeRoot() => new FileManagerItem
    {
        Name = "Home",
        IsDirectory = true,
        Children =
        [
            new FileManagerItem
            {
                Name = "Documents",
                IsDirectory = true,
                Children =
                [
                    new FileManagerItem { Name = "Report.pdf",   SizeBytes = 204_800, Modified = new DateTime(2025, 3, 1) },
                    new FileManagerItem { Name = "Notes.txt",    SizeBytes = 1_024,   Modified = new DateTime(2025, 2, 14) },
                    new FileManagerItem { Name = "Projects", IsDirectory = true, Children = [] },
                ]
            },
            new FileManagerItem
            {
                Name = "Downloads",
                IsDirectory = true,
                Children =
                [
                    new FileManagerItem { Name = "Setup.exe", SizeBytes = 10_485_760 },
                ]
            },
            new FileManagerItem { Name = "README.md", SizeBytes = 512 },
        ]
    };

    // ── Structure ─────────────────────────────────────────────────────────

    [Fact]
    public void FileManager_Renders_WithRoot()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot()));

        cut.Markup.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void FileManager_Shows_RootName_InBreadcrumb()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot()));

        cut.Find("[aria-label='Folder location']").TextContent
           .Should().Contain("Home");
    }

    [Fact]
    public void FileManager_Shows_Root_Children_Initially()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot()));

        var markup = cut.Markup;
        markup.Should().Contain("Documents");
        markup.Should().Contain("Downloads");
        markup.Should().Contain("README.md");
    }

    // ── Navigation ────────────────────────────────────────────────────────

    [Fact]
    public void FileManager_NavigatesInto_FolderOnDoubleClick()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot())
            .Add(c => c.ShowTree, false));

        // Double-click "Documents" in grid
        cut.FindAll("[role=option]")
           .First(e => e.TextContent.Contains("Documents"))
           .TriggerEvent("ondblclick", new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        cut.Markup.Should().Contain("Report.pdf");
        cut.Markup.Should().Contain("Notes.txt");
    }

    [Fact]
    public void FileManager_Breadcrumb_Updates_AfterNavigate()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot())
            .Add(c => c.ShowTree, false));

        cut.FindAll("[role=option]")
           .First(e => e.TextContent.Contains("Documents"))
           .TriggerEvent("ondblclick", new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        cut.Find("[aria-label='Folder location']").TextContent
           .Should().Contain("Home").And.Contain("Documents");
    }

    [Fact]
    public void FileManager_NavigateUp_GoesBack()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot())
            .Add(c => c.ShowTree, false));

        cut.FindAll("[role=option]")
           .First(e => e.TextContent.Contains("Documents"))
           .TriggerEvent("ondblclick", new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        cut.Find("button[aria-label='Go up one folder']").Click();

        cut.Markup.Should().Contain("Downloads");
        cut.Markup.Should().NotContain("Report.pdf");
    }

    [Fact]
    public void FileManager_BreadcrumbClick_NavigatesToRoot()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot())
            .Add(c => c.ShowTree, false));

        cut.FindAll("[role=option]")
           .First(e => e.TextContent.Contains("Documents"))
           .TriggerEvent("ondblclick", new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        // Click the root breadcrumb ("Home")
        cut.Find("[aria-label='Folder location'] button").Click();

        cut.Markup.Should().Contain("Downloads");
        cut.Markup.Should().NotContain("Report.pdf");
    }

    [Fact]
    public void FileManager_UpButton_Disabled_AtRoot()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot()));

        var upBtn = cut.Find("button[aria-label='Go up one folder']");
        upBtn.HasAttribute("disabled").Should().BeTrue();
    }

    [Fact]
    public void FileManager_UpButton_Enabled_InSubfolder()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot())
            .Add(c => c.ShowTree, false));

        cut.FindAll("[role=option]")
           .First(e => e.TextContent.Contains("Documents"))
           .TriggerEvent("ondblclick", new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        cut.Find("button[aria-label='Go up one folder']").HasAttribute("disabled").Should().BeFalse();
    }

    // ── View modes ────────────────────────────────────────────────────────

    [Fact]
    public void FileManager_GridView_ShowsItemsAsOptions()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot())
            .Add(c => c.ShowTree, false));

        cut.FindAll("[role=option]").Count.Should().Be(3); // 2 dirs + 1 file at root
    }

    [Fact]
    public void FileManager_ListViewButton_SwitchesToTable()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot())
            .Add(c => c.ShowTree, false));

        cut.Find("button[aria-label='List view']").Click();

        cut.Find("table[role=grid]").Should().NotBeNull();
    }

    [Fact]
    public void FileManager_ListView_ShowsRows()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot())
            .Add(c => c.ShowTree, false));

        cut.Find("button[aria-label='List view']").Click();

        cut.FindAll("[role=row]").Count.Should().BeGreaterThan(1); // header + data rows
    }

    // ── Search ────────────────────────────────────────────────────────────

    [Fact]
    public void FileManager_Search_FiltersItems()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot())
            .Add(c => c.ShowTree, false));

        cut.Find("input[type=search]").Input("Down");

        var options = cut.FindAll("[role=option]");
        options.Count.Should().Be(1);
        options[0].TextContent.Should().Contain("Downloads");
    }

    [Fact]
    public void FileManager_EmptySearch_ShowsAllItems()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot())
            .Add(c => c.ShowTree, false));

        cut.Find("input[type=search]").Input("xyz");
        cut.Find("input[type=search]").Input("");

        cut.FindAll("[role=option]").Count.Should().Be(3);
    }

    [Fact]
    public void FileManager_NoSearchMatch_ShowsNoResultsMessage()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot())
            .Add(c => c.ShowTree, false));

        cut.Find("input[type=search]").Input("zzz_no_match");

        cut.Markup.Should().Contain("No results");
    }

    // ── Callbacks ─────────────────────────────────────────────────────────

    [Fact]
    public void FileManager_OnFileOpen_FiredForFile()
    {
        FileManagerItem? opened = null;
        var root = MakeRoot();

        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, root)
            .Add(c => c.ShowTree, false)
            .Add(c => c.OnFileOpen, item => opened = item));

        var readme = root.Children.First(c => c.Name == "README.md");

        cut.FindAll("[role=option]")
           .First(e => e.TextContent.Contains("README.md"))
           .TriggerEvent("ondblclick", new Microsoft.AspNetCore.Components.Web.MouseEventArgs());

        opened.Should().BeSameAs(readme);
    }

    [Fact]
    public void FileManager_OnSelect_FiredOnClick()
    {
        FileManagerItem? selected = null;
        var root = MakeRoot();

        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, root)
            .Add(c => c.ShowTree, false)
            .Add(c => c.OnSelect, item => selected = item));

        cut.FindAll("[role=option]")
           .First(e => e.TextContent.Contains("README.md"))
           .Click();

        selected?.Name.Should().Be("README.md");
    }

    // ── Delete ────────────────────────────────────────────────────────────

    [Fact]
    public void FileManager_AllowDeleteFalse_HidesDeleteButtons()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot())
            .Add(c => c.ShowTree, false)
            .Add(c => c.AllowDelete, false));

        cut.Find("button[aria-label='List view']").Click();

        cut.FindAll("button[aria-label^='Delete']").Should().BeEmpty();
    }

    [Fact]
    public void FileManager_AllowDeleteTrue_ShowsDeleteButtons()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot())
            .Add(c => c.ShowTree, false)
            .Add(c => c.AllowDelete, true));

        cut.Find("button[aria-label='List view']").Click();

        cut.FindAll("button[aria-label^='Delete']").Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public void FileManager_OnDelete_FiredWhenDeleteClicked()
    {
        FileManagerItem? deleted = null;
        var root = MakeRoot();

        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, root)
            .Add(c => c.ShowTree, false)
            .Add(c => c.AllowDelete, true)
            .Add(c => c.OnDelete, item => deleted = item));

        cut.Find("button[aria-label='List view']").Click();

        cut.FindAll("button[aria-label^='Delete']").First().Click();

        deleted.Should().NotBeNull();
    }

    // ── Tree panel ────────────────────────────────────────────────────────

    [Fact]
    public void FileManager_ShowTreeTrue_RendersSidebar()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot())
            .Add(c => c.ShowTree, true));

        cut.Find("[role=tree]").Should().NotBeNull();
    }

    [Fact]
    public void FileManager_ShowTreeFalse_HidesSidebar()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot())
            .Add(c => c.ShowTree, false));

        cut.FindAll("[role=tree]").Should().BeEmpty();
    }

    [Fact]
    public void FileManager_Tree_ShowsOnlyDirectories()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot())
            .Add(c => c.ShowTree, true));

        var treeText = cut.Find("[role=tree]").TextContent;
        treeText.Should().Contain("Documents").And.Contain("Downloads");
        treeText.Should().NotContain("README.md"); // file — must not appear in tree
    }

    // ── Status bar ────────────────────────────────────────────────────────

    [Fact]
    public void FileManager_StatusBar_ShowsItemCount()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot())
            .Add(c => c.ShowTree, false));

        cut.Markup.Should().Contain("3 items");
    }

    [Fact]
    public void FileManager_StatusBar_ShowsSelectedFileName()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot())
            .Add(c => c.ShowTree, false));

        cut.FindAll("[role=option]")
           .First(e => e.TextContent.Contains("README.md"))
           .Click();

        cut.Markup.Should().Contain("README.md");
    }

    // ── Sort order ────────────────────────────────────────────────────────

    [Fact]
    public void FileManager_FoldersListedBeforeFiles()
    {
        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, MakeRoot())
            .Add(c => c.ShowTree, false));

        var labels = cut.FindAll("[role=option]")
                        .Select(e => e.TextContent.Trim())
                        .ToList();

        var firstFileIdx = labels.FindIndex(l => l.Contains("README.md"));
        var firstDirIdx  = labels.FindIndex(l => l.Contains("Documents") || l.Contains("Downloads"));

        firstDirIdx.Should().BeLessThan(firstFileIdx);
    }

    // ── Empty folder ──────────────────────────────────────────────────────

    [Fact]
    public void FileManager_EmptyFolder_ShowsEmptyMessage()
    {
        var root = new FileManagerItem { Name = "Empty", IsDirectory = true, Children = [] };

        var cut = RenderComponent<FileManager>(p => p
            .Add(c => c.Root, root)
            .Add(c => c.ShowTree, false));

        cut.Markup.Should().Contain("This folder is empty");
    }
}
