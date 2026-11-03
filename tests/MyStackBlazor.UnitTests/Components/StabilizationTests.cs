using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.DependencyInjection;
using MyStackBlazor.Components.Form;
using MyStackBlazor.DataGrid.Services;

namespace MyStackBlazor.UnitTests.Components;

public class ThemeIsolationTests
{
    [Fact]
    public void Themes_Are_Shared_Within_A_Scope_And_Isolated_Between_Users()
    {
        using var provider = new ServiceCollection().AddMyStackBlazor().BuildServiceProvider();
        using var first = provider.CreateScope();
        using var second = provider.CreateScope();
        var a = first.ServiceProvider.GetRequiredService<ThemeService>();
        var b = second.ServiceProvider.GetRequiredService<ThemeService>();
        var notified = false;
        b.OnThemeChanged += () => notified = true;
        a.SetTheme(AppTheme.Dark);
        a.Should().BeSameAs(first.ServiceProvider.GetRequiredService<ThemeService>());
        b.Theme.Should().Be(AppTheme.System);
        notified.Should().BeFalse();
    }
}

public class ExportSafetyTests
{
    [Theory]
    [InlineData("=1+1")]
    [InlineData("+SUM(A1:A2)")]
    [InlineData("-1+2")]
    [InlineData("@SUM(1,2)")]
    [InlineData("  =1+1")]
    [InlineData("\t=1+1")]
    public void Csv_Neutralizes_Formula_Text(string value)
    {
        var bytes = GridExporter.ExportToCsv(new[] { value }, new[] { new ColumnDefinition("Value", o => o) });
        var row = Encoding.UTF8.GetString(bytes).Split(Environment.NewLine)[1];
        row.TrimStart('"').Should().StartWith("'");
    }

    [Fact]
    public void Csv_Quotes_Carriage_Return_And_Allows_Explicit_Trusted_Export()
    {
        var columns = new[] { new ColumnDefinition("Value", o => o) };
        Encoding.UTF8.GetString(GridExporter.ExportToCsv(new[] { "a\rb" }, columns))
            .Should().Contain("\"a\rb\"");
        Encoding.UTF8.GetString(GridExporter.ExportToCsv(new[] { "=1+1" }, columns, false))
            .Should().Contain("\n=1+1");
    }

    [Fact]
    public void Csv_Protects_Headers_While_Preserving_Real_Negative_Numbers()
    {
        var csv = Encoding.UTF8.GetString(GridExporter.ExportToCsv(new[] { -12.5m },
            new[] { new ColumnDefinition("=1+1", o => o) }));
        csv.Should().StartWith("'=1+1");
        csv.Should().Contain("\n-12.5");
    }

    [Fact]
    public void Json_Preserves_Number_Boolean_And_Null()
    {
        var bytes = GridExporter.ExportToJson(new[] { 1 }, new[] {
            new ColumnDefinition("Amount", _ => 12.50m),
            new ColumnDefinition("Paid", _ => true),
            new ColumnDefinition("Note", _ => null)
        });
        using var json = JsonDocument.Parse(bytes);
        var row = json.RootElement[0];
        row.GetProperty("Amount").GetDecimal().Should().Be(12.50m);
        row.GetProperty("Paid").GetBoolean().Should().BeTrue();
        row.GetProperty("Note").ValueKind.Should().Be(JsonValueKind.Null);
    }
}

public class FileUploadLimitTests : TestContext
{
    public FileUploadLimitTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Oversized_File_Is_Rejected_With_A_Visible_Error()
    {
        IReadOnlyList<StackFileUpload.UploadRejection>? rejected = null;
        var cut = RenderComponent<StackFileUpload>(p => p
            .Add(f => f.MaxFileSizeBytes, 3)
            .Add(f => f.OnRejected, errors => rejected = errors));
        cut.FindComponent<InputFile>().UploadFiles(InputFileContent.CreateFromText("large", "a.txt"));
        cut.Instance.Files.Should().BeEmpty();
        rejected!.Single().FileName.Should().Be("a.txt");
        cut.Find("[role=alert]").TextContent.Should().Contain("Maximum file size");
    }

    [Fact]
    public void Multiple_Selections_Respect_The_Total_Limit_Without_Mutating_The_Parent_List()
    {
        var original = new List<StackFileUpload.UploadFile> { new() { Name = "existing.txt" } };
        var cut = RenderComponent<StackFileUpload>(p => p
            .Add(f => f.Files, original).Add(f => f.MaxFiles, 2));
        cut.FindComponent<InputFile>().UploadFiles(InputFileContent.CreateFromText("x", "a.txt"));
        cut.FindComponent<InputFile>().UploadFiles(InputFileContent.CreateFromText("x", "b.txt"));
        cut.Instance.Files.Select(f => f.Name).Should().Equal("existing.txt", "a.txt");
        original.Should().HaveCount(1);
        cut.Find("[role=alert]").TextContent.Should().Contain("Maximum number");
    }

    [Fact]
    public void Excessive_Batch_Is_Reported_Instead_Of_Throwing()
    {
        var cut = RenderComponent<StackFileUpload>(p => p.Add(f => f.MaxFiles, 1));
        cut.FindComponent<InputFile>().UploadFiles(
            InputFileContent.CreateFromText("x", "a.txt"), InputFileContent.CreateFromText("x", "b.txt"));
        cut.Instance.Files.Should().ContainSingle();
        cut.Find("[role=alert]").TextContent.Should().Contain("b.txt");
    }

    [Fact]
    public void Invalid_Single_Replacement_Keeps_Previous_Selection()
    {
        var cut = RenderComponent<StackFileUpload>(p => p
            .Add(f => f.Multiple, false).Add(f => f.MaxFileSizeBytes, 1));
        cut.FindComponent<InputFile>().UploadFiles(InputFileContent.CreateFromText("x", "a.txt"));
        cut.FindComponent<InputFile>().UploadFiles(InputFileContent.CreateFromText("too large", "b.txt"));
        cut.Instance.Files.Single().Name.Should().Be("a.txt");
    }
}
