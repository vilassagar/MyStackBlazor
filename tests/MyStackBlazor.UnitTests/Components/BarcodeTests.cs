using System.Text;
using MyStackBlazor.Components.Data;

namespace MyStackBlazor.UnitTests.Components;

public class BarcodeEncoderTests
{
    /// <summary>Bar/space run widths in modules, starting with the first bar.</summary>
    private static string Widths(BarcodeEncoding e)
    {
        var sb = new StringBuilder();
        int? previousEnd = null;
        foreach (var bar in e.Bars)
        {
            if (previousEnd is int end) sb.Append(bar.Start - end);
            sb.Append(bar.Width);
            previousEnd = bar.Start + bar.Width;
        }
        return sb.ToString();
    }

    /// <summary>The module string ('1' bar, '0' space) of the whole symbol.</summary>
    private static string Modules(BarcodeEncoding e)
    {
        var chars = new char[e.Modules];
        Array.Fill(chars, '0');
        foreach (var bar in e.Bars) for (var i = 0; i < bar.Width; i++) chars[bar.Start + i] = '1';
        return new string(chars);
    }

    // Published worked examples

    [Theory]
    [InlineData("590123412345", "5901234123457")]
    [InlineData("400638133393", "4006381333931")]
    public void Ean13_Adds_Check_Digit(string value, string expected) =>
        BarcodeEncoder.Encode(BarcodeType.EAN13, value).EncodedValue.Should().Be(expected);

    [Fact]
    public void Ean13_Rejects_Wrong_Check_Digit() =>
        FluentActions.Invoking(() => BarcodeEncoder.Encode(BarcodeType.EAN13, "5901234123450"))
            .Should().Throw<ArgumentException>().WithMessage("*expected 7*");

    [Fact]
    public void Ean13_Structure()
    {
        var e = BarcodeEncoder.Encode(BarcodeType.EAN13, "5901234123457");
        e.Modules.Should().Be(95 + 18, "95 modules plus two 9-module quiet zones");
        var m = Modules(e)[9..^9];
        m[..3].Should().Be("101");
        m[45..50].Should().Be("01010");
        m[^3..].Should().Be("101");
        // First digit 5 → parity LGGLLG; second digit 9 encoded with L
        m.Substring(3, 7).Should().Be("0001011");
        // Last digit 7 encoded with R
        m.Substring(85, 7).Should().Be("1000100");
        e.Text.Select(t => t.Text).Should().Equal("5", "901234", "123457");
    }

    [Fact]
    public void UpcA_And_Ean8_Check_Digits()
    {
        BarcodeEncoder.Encode(BarcodeType.UPCA, "03600029145").EncodedValue.Should().Be("036000291452");
        BarcodeEncoder.Encode(BarcodeType.EAN8, "9638507").EncodedValue.Should().Be("96385074");
        BarcodeEncoder.Encode(BarcodeType.EAN8, "9638507").Modules.Should().Be(67 + 18);
    }

    [Fact]
    public void UpcE_Check_Digit_From_Expanded_UpcA()
    {
        var e = BarcodeEncoder.Encode(BarcodeType.UPCE, "654321");
        e.EncodedValue.Should().Be("06543217");
        e.Modules.Should().Be(51 + 18);
        Modules(e)[9..^9][^6..].Should().Be("010101");
    }

    [Fact]
    public void Code93_Checksums_Match_The_TEST93_Example()
    {
        // TEST93 → check characters C = '+' and K = '6'
        var widths = Widths(BarcodeEncoder.Encode(BarcodeType.Code93, "TEST93"));
        var symbols = Enumerable.Range(0, widths.Length / 6).Select(i => widths.Substring(i * 6, 6)).ToList();
        symbols[0].Should().Be("111141", "start");
        symbols[1].Should().Be("211221", "T");
        symbols[7].Should().Be("113121", "check C = '+'");
        symbols[8].Should().Be("121311", "check K = '6'");
        symbols[9].Should().Be("111141", "stop");
        widths[^1].Should().Be('1', "termination bar");
    }

    [Fact]
    public void Code128_Uses_Subset_C_For_Digits_And_Correct_Checksum()
    {
        // "123456" in subset C: start C (105) + 12, 34, 56. Checksum = (105 + 12·1 + 34·2 + 56·3) mod 103 = 44.
        var widths = Widths(BarcodeEncoder.Encode(BarcodeType.Code128, "123456"));
        var symbols = Enumerable.Range(0, (widths.Length - 7) / 6).Select(i => widths.Substring(i * 6, 6)).ToList();
        symbols.Should().Equal("211232", "112232", "131123", "331121", "132131");
        widths[^7..].Should().Be("2331112", "stop");
    }

    [Fact]
    public void Code128_Switches_From_B_To_C_For_Long_Digit_Runs()
    {
        // "AB1234": start B, A(33), B(34), Code C (99), 12, 34. Checksum = (104+33+68+297+48+170) mod 103 = 102.
        var widths = Widths(BarcodeEncoder.Encode(BarcodeType.Code128, "AB1234"));
        var symbols = Enumerable.Range(0, (widths.Length - 7) / 6).Select(i => widths.Substring(i * 6, 6)).ToList();
        symbols.Should().Equal("211214", "111323", "131123", "113141", "112232", "131123", "411131");
    }

    [Fact]
    public void Code128C_Rejects_Odd_Length() =>
        FluentActions.Invoking(() => BarcodeEncoder.Encode(BarcodeType.Code128C, "123"))
            .Should().Throw<ArgumentException>();

    [Fact]
    public void Gs1128_Starts_With_Fnc1_And_Strips_Parentheses()
    {
        var e = BarcodeEncoder.Encode(BarcodeType.GS1128, "(01)09501101530003(10)ABC");
        e.EncodedValue.Should().Be("010950110153000310ABC");
        var widths = Widths(e);
        widths[..6].Should().Be("211232", "start C");
        widths.Substring(6, 6).Should().Be("411131", "FNC1");
        e.Text.Single().Text.Should().Be("(01)09501101530003(10)ABC");
    }

    [Fact]
    public void Code39_Width_And_Optional_Checksum()
    {
        // Each character is 6 narrow + 3 wide (3 modules) = 15 modules, plus a 1-module gap.
        BarcodeEncoder.Encode(BarcodeType.Code39, "A").Modules.Should().Be(15 * 3 + 2);
        // Mod 43: "CODE39" → values 12+24+13+14+3+9 = 75 → 75 mod 43 = 32 → 'W'
        BarcodeEncoder.Encode(BarcodeType.Code39, "CODE39", checksum: true).EncodedValue.Should().Be("CODE39W");
    }

    [Fact]
    public void Code39_Lowercase_Needs_Extended()
    {
        BarcodeEncoder.Encode(BarcodeType.Code39, "abc").EncodedValue.Should().Be("ABC");
        BarcodeEncoder.Encode(BarcodeType.Code39Extended, "a!").EncodedValue.Should().Be("+A/A");
        FluentActions.Invoking(() => BarcodeEncoder.Encode(BarcodeType.Code39, "a#")).Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Msi_Check_Digits()
    {
        BarcodeEncoder.Encode(BarcodeType.MSImod10, "1234567").EncodedValue.Should().Be("12345674");
        BarcodeEncoder.Encode(BarcodeType.MSImod1010, "1234567").EncodedValue.Should().Be("123456741");
        // Each digit is 4 bits of 3 modules; start 3 + stop 4.
        BarcodeEncoder.Encode(BarcodeType.MSImod10, "1").Modules.Should().Be(3 + 2 * 12 + 4);
    }

    [Fact]
    public void Code11_With_Checksum()
    {
        // "123-45": C = (5·1 + 4·2 + 10·3 + 3·4 + 2·5 + 1·6) mod 11 = 71 mod 11 = 5
        BarcodeEncoder.Encode(BarcodeType.Code11, "123-45", checksum: true).EncodedValue.Should().Be("123-455");
    }

    [Fact]
    public void Postnet_Full_And_Half_Bars()
    {
        var e = BarcodeEncoder.Encode(BarcodeType.POSTNET, "12345");
        e.EncodedValue.Should().Be("123455");
        e.Bars.Should().HaveCount(2 + 6 * 5);
        e.Bars.Count(b => b.Top == 0).Should().Be(2 + 6 * 2, "each digit has two full bars, plus the frame bars");
    }

    [Theory]
    [InlineData(BarcodeType.EAN13, "12345")]
    [InlineData(BarcodeType.Code128C, "12AB")]
    [InlineData(BarcodeType.POSTNET, "1234")]
    [InlineData(BarcodeType.MSImod10, "12A")]
    public void Invalid_Values_Throw(BarcodeType type, string value) =>
        FluentActions.Invoking(() => BarcodeEncoder.Encode(type, value)).Should().Throw<ArgumentException>();
}

public class BarcodeComponentTests : TestContext
{
    [Fact]
    public void Renders_Svg_With_Bars_And_Text()
    {
        var cut = RenderComponent<StackBarcode>(p => p
            .Add(b => b.Type, BarcodeType.EAN13)
            .Add(b => b.Value, "590123412345")
            .Add(b => b.Width, 300)
            .Add(b => b.Height, 120));

        var svg = cut.Find("svg");
        svg.GetAttribute("width").Should().Be("300px");
        svg.GetAttribute("data-encoded").Should().Be("5901234123457");
        svg.GetAttribute("aria-label").Should().Be("EAN13 barcode 590123412345");
        cut.FindAll("g[data-bars] rect").Count.Should().BeGreaterThan(20);
        cut.FindAll("text").Select(t => t.TextContent).Should().Equal("5", "901234", "123457");
    }

    [Fact]
    public void Colors_Background_And_Border()
    {
        var cut = RenderComponent<StackBarcode>(p => p
            .Add(b => b.Value, "HELLO")
            .Add(b => b.Color, "#123456")
            .Add(b => b.Background, "#fafafa")
            .AddChildContent<StackBarcodeBorder>(b => b
                .Add(x => x.Color, "red")
                .Add(x => x.Width, 2)
                .Add(x => x.DashType, BarcodeDashType.Dash)));

        cut.Find("svg > rect").GetAttribute("fill").Should().Be("#fafafa");
        cut.Find("g[data-bars]").GetAttribute("fill").Should().Be("#123456");
        var border = cut.Find("rect[stroke=red]");
        border.GetAttribute("stroke-width").Should().Be("2");
        border.GetAttribute("stroke-dasharray").Should().Be("8 4");
    }

    [Fact]
    public void Text_Can_Be_Hidden_Or_Styled()
    {
        var hidden = RenderComponent<StackBarcode>(p => p
            .Add(b => b.Value, "HELLO")
            .AddChildContent<StackBarcodeText>(t => t.Add(x => x.Visible, false)));
        hidden.FindAll("text").Should().BeEmpty();

        var styled = RenderComponent<StackBarcode>(p => p
            .Add(b => b.Value, "HELLO")
            .AddChildContent<StackBarcodeText>(t => t.Add(x => x.Color, "green").Add(x => x.Font, "20px serif")));
        var group = styled.Find("g[text-anchor=middle]");
        group.GetAttribute("fill").Should().Be("green");
        group.GetAttribute("style").Should().Contain("20px serif");
    }

    [Fact]
    public void Invalid_Value_Shows_Error()
    {
        var cut = RenderComponent<StackBarcode>(p => p
            .Add(b => b.Type, BarcodeType.EAN8)
            .Add(b => b.Value, "abc"));
        cut.FindAll("svg").Should().BeEmpty();
        cut.Find("[role=alert]").TextContent.Should().Contain("Cannot render EAN8");
    }

    [Fact]
    public void Padding_Shrinks_Bar_Area()
    {
        var cut = RenderComponent<StackBarcode>(p => p
            .Add(b => b.Value, "A")
            .Add(b => b.Padding, 10)
            .Add(b => b.Width, 200));
        cut.FindAll("g[data-bars] rect")[0].GetAttribute("x").Should().Be("10");
    }
}
