using MyStackBlazor.Components.Form;

namespace MyStackBlazor.UnitTests.Components;

public class RgbaColorTests
{
    [Theory]
    [InlineData("#ff8000", 255, 128, 0, 1.0)]
    [InlineData("#f80", 255, 136, 0, 1.0)]
    [InlineData("#ff800080", 255, 128, 0, 0.5)]
    [InlineData("rgb(10, 20, 30)", 10, 20, 30, 1.0)]
    [InlineData("rgba(10,20,30,0.25)", 10, 20, 30, 0.25)]
    public void Parses_Hex_And_Rgb(string text, int r, int g, int b, double a)
    {
        var c = RgbaColor.Parse(text)!.Value;
        (c.R, c.G, c.B, c.A).Should().Be(((byte)r, (byte)g, (byte)b, a));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("red")]
    [InlineData("#12345")]
    public void Rejects_Invalid(string? text) => RgbaColor.Parse(text).Should().BeNull();

    [Fact]
    public void Formats_And_Hsv_Round_Trip()
    {
        var c = new RgbaColor(52, 101, 164);
        c.ToHex().Should().Be("#3465a4");
        c.ToRgb().Should().Be("rgb(52, 101, 164)");
        (c with { A = 0.5 }).ToRgb().Should().Be("rgba(52, 101, 164, 0.5)");
        var (h, s, v) = c.ToHsv();
        RgbaColor.FromHsv(h, s, v).Should().Be(c);
        RgbaColor.FromHsv(0, 1, 1).ToHex().Should().Be("#ff0000");
        RgbaColor.FromHsv(240, 1, 0.5).ToHex().Should().Be("#000080");
    }
}

public class ColorPaletteTests : TestContext
{
    public ColorPaletteTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Presets_And_Columns()
    {
        var cut = RenderComponent<StackColorPalette>(p => p.Add(c => c.Palette, ColorPalettePresets.Office).Add(c => c.Columns, 10));
        cut.FindAll("[role=gridcell]").Should().HaveCount(60);
        cut.Find("[role=grid]").GetAttribute("style").Should().StartWith("grid-template-columns:repeat(10");

        var mono = RenderComponent<StackColorPalette>(p => p.Add(c => c.Palette, ColorPalettePresets.Monochrome));
        mono.FindAll("[role=gridcell]").First().GetAttribute("data-color").Should().Be("#ffffff");
        mono.FindAll("[role=gridcell]").Last().GetAttribute("data-color").Should().Be("#000000");
    }

    [Fact]
    public void Click_And_Keyboard_Select()
    {
        string? value = null;
        var cut = RenderComponent<StackColorPalette>(p => p
            .Add(c => c.Colors, new[] { "#111111", "#222222", "#333333", "#444444" })
            .Add(c => c.Columns, 2)
            .Add(c => c.ValueChanged, v => value = v));

        cut.Find("[data-color='#222222']").Click();
        value.Should().Be("#222222");

        cut.Find("[role=grid]").Focus();
        cut.Find("[role=grid]").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" });
        cut.Find("[role=grid]").KeyDown(new KeyboardEventArgs { Key = "Enter" });
        value.Should().Be("#444444");
    }

    [Fact]
    public void Earlier_Api_Label_ShowHex_And_Tile_Size()
    {
        var cut = RenderComponent<StackColorPalette>(p => p
            .Add(c => c.Label, "Brand")
            .Add(c => c.ShowHex, true)
            .Add(c => c.Value, "#ef4444")
            .Add(c => c.TileWidth, "20px"));
        cut.Markup.Should().Contain("Brand").And.Contain("Selected: #ef4444");
        cut.Find("[aria-selected=true]").GetAttribute("style").Should().Contain("width:20px");
    }
}

public class ColorGradientTests : TestContext
{
    public ColorGradientTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public async Task Hue_Alpha_And_Area_Update_Value()
    {
        string? value = null;
        var cut = RenderComponent<StackColorGradient>(p => p
            .Add(g => g.Value, "#ff0000")
            .Add(g => g.ValueChanged, v => value = v));

        await cut.Find("input[aria-label=Hue]").InputAsync(new ChangeEventArgs { Value = "120" });
        value.Should().Be("#00ff00");

        await cut.Find("input[aria-label=Opacity]").InputAsync(new ChangeEventArgs { Value = "50" });
        value.Should().Be("#00ff0080");

        cut.Find("[role=slider]").PointerDown(new PointerEventArgs { OffsetX = 0, OffsetY = 160 });
        value.Should().Be("#00000080", "bottom-left is black");
    }

    [Fact]
    public async Task Hex_Input_And_Rgb_Format()
    {
        string? value = null;
        var cut = RenderComponent<StackColorGradient>(p => p
            .Add(g => g.Format, ColorFormat.Rgb)
            .Add(g => g.ShowOpacityEditor, false)
            .Add(g => g.ValueChanged, v => value = v));

        await cut.Find("input[aria-label=R]").ChangeAsync(new ChangeEventArgs { Value = "200" });
        value.Should().Be("rgb(200, 0, 0)");
        cut.FindAll("input[aria-label=Opacity]").Should().BeEmpty();

        cut.Find("button[aria-label='Switch input format']").Click();
        await cut.Find("input[aria-label=Hex]").ChangeAsync(new ChangeEventArgs { Value = "0080ff" });
        value.Should().Be("rgb(0, 128, 255)");
    }

    [Fact]
    public void Keyboard_Moves_Saturation()
    {
        string? value = null;
        var cut = RenderComponent<StackColorGradient>(p => p.Add(g => g.Value, "#808080").Add(g => g.ValueChanged, v => value = v));
        cut.Find("[role=slider]").KeyDown(new KeyboardEventArgs { Key = "ArrowRight", ShiftKey = true });
        value.Should().NotBeNull().And.NotBe("#808080");
    }
}

public class FlatColorPickerTests : TestContext
{
    public FlatColorPickerTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Apply_Commits_And_Cancel_Reverts()
    {
        string? value = "#000000";
        var cancelled = false;
        var cut = RenderComponent<StackFlatColorPicker>(p => p
            .Add(f => f.Value, "#000000")
            .Add(f => f.View, ColorPickerView.Palette)
            .Add(f => f.Colors, new[] { "#000000", "#ff0000" })
            .Add(f => f.ValueChanged, v => value = v)
            .Add(f => f.OnCancel, () => cancelled = true));

        cut.Find("[data-color='#ff0000']").Click();
        value.Should().Be("#000000", "nothing applies before Apply");
        cut.Find("[aria-label^='New color']").GetAttribute("aria-label").Should().Be("New color #ff0000");
        cut.FindAll("button").First(b => b.TextContent.Trim() == "Apply").Click();
        value.Should().Be("#ff0000");

        cut.Find("[data-color='#000000']").Click();
        cut.FindAll("button").First(b => b.TextContent.Trim() == "Cancel").Click();
        cancelled.Should().BeTrue();
        cut.Find("[aria-label^='New color']").GetAttribute("aria-label").Should().Be("New color #ff0000");
    }

    [Fact]
    public void Without_Buttons_Changes_Apply_Immediately_And_Views_Switch()
    {
        string? value = null;
        var cut = RenderComponent<StackFlatColorPicker>(p => p
            .Add(f => f.ShowButtons, false)
            .Add(f => f.ValueChanged, v => value = v));
        cut.Find("[role=tab][aria-selected=true]").TextContent.Should().Be("Gradient");
        cut.FindAll("[role=tab]").Last().Click();
        cut.FindAll("[role=gridcell]")[1].Click();
        value.Should().NotBeNull();
        cut.Find("button[aria-label='Clear color']").Click();
        value.Should().BeNull();
    }
}

public class ColorPickerTests : TestContext
{
    public ColorPickerTests() => JSInterop.Mode = JSRuntimeMode.Loose;

    [Fact]
    public void Opens_Popup_And_Pick_Closes()
    {
        string? value = null;
        var cut = RenderComponent<StackColorPicker>(p => p
            .Add(c => c.Value, "#000000")
            .Add(c => c.Views, [ColorPickerView.Palette])
            .Add(c => c.ShowButtons, false)
            .Add(c => c.Colors, new[] { "#000000", "#00ff00" })
            .Add(c => c.ValueChanged, v => value = v));

        cut.Find("button[aria-haspopup=dialog]").Click();
        cut.Find("[role=dialog]").Should().NotBeNull();
        cut.Find("[data-color='#00ff00']").Click();
        value.Should().Be("#00ff00");
        cut.FindAll("[role=dialog]").Should().BeEmpty();
    }

    [Fact]
    public void Earlier_Api_Swatches_And_No_Sliders()
    {
        var cut = RenderComponent<StackColorPicker>(p => p
            .Add(c => c.ShowSliders, false)
            .Add(c => c.Swatches, ["#abcdef"]));
        cut.Find("button[aria-haspopup=dialog]").Click();
        cut.FindAll("[role=tab]").Should().BeEmpty("only the palette view");
        cut.Find("[data-color='#abcdef']").Should().NotBeNull();
        StackColorPicker.DefaultSwatches.Should().HaveCount(10);
    }

    [Fact]
    public void OnOpen_Can_Cancel_And_Disabled()
    {
        var cut = RenderComponent<StackColorPicker>(p => p.Add(c => c.OnOpen, (ColorPickerOpenCloseEventArgs a) => a.IsCancelled = true));
        cut.Find("button[aria-haspopup=dialog]").Click();
        cut.FindAll("[role=dialog]").Should().BeEmpty();

        var disabled = RenderComponent<StackColorPicker>(p => p.Add(c => c.Enabled, false));
        disabled.Find("button[aria-haspopup=dialog]").HasAttribute("disabled").Should().BeTrue();
    }
}
