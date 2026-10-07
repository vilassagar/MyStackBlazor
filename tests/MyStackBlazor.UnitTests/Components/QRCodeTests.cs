using MyStackBlazor.Components.Data;

namespace MyStackBlazor.UnitTests.Components;

public class QRCodeEncoderTests
{
    // The classic worked example: "HELLO WORLD", version 1-M, alphanumeric mode.
    private static readonly byte[] HelloWorldData = [32, 91, 11, 120, 209, 114, 220, 77, 67, 64, 236, 17, 236, 17, 236, 17];
    private static readonly byte[] HelloWorldEcc = [196, 35, 39, 119, 235, 215, 231, 226, 93, 23];

    [Fact]
    public void Hello_World_Data_Codewords()
    {
        var data = QRCodeEncoder.BuildDataCodewords("HELLO WORLD", QRCodeErrorCorrection.Medium, QRCodeEncoding.ISO_8859_1, out var version);
        version.Should().Be(1);
        data.Should().Equal(HelloWorldData);
    }

    [Fact]
    public void Hello_World_Reed_Solomon()
    {
        QRCodeEncoder.ReedSolomonRemainder(HelloWorldData, QRCodeEncoder.ReedSolomonDivisor(10)).Should().Equal(HelloWorldEcc);
    }

    [Fact]
    public void Hello_World_Matrix_Reads_Back()
    {
        var qr = QRCodeEncoder.Encode("HELLO WORLD", QRCodeErrorCorrection.Medium);
        qr.Version.Should().Be(1);
        qr.Size.Should().Be(21);

        // Format information (first copy, around the top-left finder) must match level M + chosen mask.
        var bits = 0;
        int[] xs = [8, 8, 8, 8, 8, 8, 8, 8, 7, 5, 4, 3, 2, 1, 0];
        int[] ys = [0, 1, 2, 3, 4, 5, 7, 8, 8, 8, 8, 8, 8, 8, 8];
        for (var i = 0; i < 15; i++) if (qr[xs[i], ys[i]]) bits |= 1 << i;
        bits.Should().Be(QRCodeEncoder.FormatInformation(QRCodeErrorCorrection.Medium, qr.Mask));

        ReadVersion1Codewords(qr).Should().Equal(HelloWorldData.Concat(HelloWorldEcc));
    }

    /// <summary>Independent reader for version 1: unmask and walk the zig-zag placement.</summary>
    private static byte[] ReadVersion1Codewords(QRCodeMatrix qr)
    {
        const int size = 21;
        static bool IsFunction(int x, int y) =>
            (x < 9 && y < 9) || (x >= size - 8 && y < 9) || (x < 9 && y >= size - 8) || x == 6 || y == 6;
        bool Mask(int x, int y) => qr.Mask switch
        {
            0 => (x + y) % 2 == 0,
            1 => y % 2 == 0,
            2 => x % 3 == 0,
            3 => (x + y) % 3 == 0,
            4 => (x / 3 + y / 2) % 2 == 0,
            5 => x * y % 2 + x * y % 3 == 0,
            6 => (x * y % 2 + x * y % 3) % 2 == 0,
            _ => ((x + y) % 2 + x * y % 3) % 2 == 0,
        };

        var result = new byte[26];
        var bit = 0;
        for (var right = size - 1; right >= 1; right -= 2)
        {
            if (right == 6) right = 5;
            for (var vert = 0; vert < size; vert++)
                for (var j = 0; j < 2; j++)
                {
                    var x = right - j;
                    var y = ((right + 1) & 2) == 0 ? size - 1 - vert : vert;
                    if (IsFunction(x, y) || bit >= 26 * 8) continue;
                    if (qr[x, y] ^ Mask(x, y)) result[bit >> 3] |= (byte)(1 << (7 - (bit & 7)));
                    bit++;
                }
        }
        return result;
    }

    [Fact]
    public void Format_Information_Known_Values()
    {
        QRCodeEncoder.FormatInformation(QRCodeErrorCorrection.Low, 0).Should().Be(0b111011111000100);
        QRCodeEncoder.FormatInformation(QRCodeErrorCorrection.Medium, 0).Should().Be(0b101010000010010);
        QRCodeEncoder.FormatInformation(QRCodeErrorCorrection.High, 7).Should().Be(0b000100000111011);
    }

    [Theory]
    [InlineData(7, new[] { 6, 22, 38 })]
    [InlineData(32, new[] { 6, 34, 60, 86, 112, 138 })]
    [InlineData(40, new[] { 6, 30, 58, 86, 114, 142, 170 })]
    public void Alignment_Pattern_Positions(int version, int[] expected) =>
        QRCodeEncoder.AlignmentPositions(version).Should().Equal(expected);

    [Theory]
    // Published capacities: (mode sample char, level, max chars at version 1, at version 40)
    [InlineData('7', QRCodeErrorCorrection.Low, 41, 7089)]
    [InlineData('A', QRCodeErrorCorrection.Low, 25, 4296)]
    [InlineData('a', QRCodeErrorCorrection.Low, 17, 2953)]
    [InlineData('a', QRCodeErrorCorrection.High, 7, 1273)]
    public void Capacity_Matches_The_Standard(char sample, QRCodeErrorCorrection ecl, int v1Max, int v40Max)
    {
        QRCodeEncoder.Encode(new string(sample, v1Max), ecl).Version.Should().Be(1);
        QRCodeEncoder.Encode(new string(sample, v1Max + 1), ecl).Version.Should().Be(2);
        QRCodeEncoder.Encode(new string(sample, v40Max), ecl).Version.Should().Be(40);
        FluentActions.Invoking(() => QRCodeEncoder.Encode(new string(sample, v40Max + 1), ecl)).Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Function_Patterns_Are_Present()
    {
        var qr = QRCodeEncoder.Encode("https://example.com/some/long/path?with=query", QRCodeErrorCorrection.Quartile);
        var n = qr.Size;
        // Finder pattern corners and centres
        foreach (var (x, y) in new[] { (0, 0), (n - 1, 0), (0, n - 1), (3, 3), (n - 4, 3), (3, n - 4) }) qr[x, y].Should().BeTrue();
        foreach (var (x, y) in new[] { (1, 1), (n - 2, 1), (1, n - 2) }) qr[x, y].Should().BeFalse();
        // Timing patterns alternate
        for (var i = 8; i < n - 8; i++) qr[i, 6].Should().Be(i % 2 == 0);
        // Always-dark module
        qr[8, n - 8].Should().BeTrue();
    }

    [Fact]
    public void Utf8_Needs_The_Utf8_Encoding()
    {
        FluentActions.Invoking(() => QRCodeEncoder.Encode("日本", QRCodeErrorCorrection.Low)).Should().Throw<ArgumentException>();
        QRCodeEncoder.Encode("日本", QRCodeErrorCorrection.Low, QRCodeEncoding.UTF_8).Version.Should().Be(1);
        QRCodeEncoder.Encode("Café", QRCodeErrorCorrection.Low).Version.Should().Be(1, "é is in ISO-8859-1");
    }
}

public class QRCodeComponentTests : TestContext
{
    [Fact]
    public void Renders_Svg_With_Quiet_Zone()
    {
        var cut = RenderComponent<StackQRCode>(p => p
            .Add(q => q.Value, "HELLO WORLD")
            .Add(q => q.ErrorCorrection, QRCodeErrorCorrection.Medium)
            .Add(q => q.Size, "180px"));

        var svg = cut.Find("svg");
        svg.GetAttribute("data-version").Should().Be("1");
        svg.GetAttribute("viewBox").Should().Be("0 0 29 29", "21 modules + 4 quiet modules on each side");
        svg.GetAttribute("width").Should().Be("180px");
        svg.GetAttribute("aria-label").Should().Be("QR code: HELLO WORLD");
        cut.Find("path").GetAttribute("d").Should().StartWith("M4,4h7v1h-7z", "the top row starts with the 7-module finder");
    }

    [Fact]
    public void Colors_And_Border()
    {
        var cut = RenderComponent<StackQRCode>(p => p
            .Add(q => q.Value, "x")
            .Add(q => q.Color, "#1e40af")
            .Add(q => q.Background, "#eff6ff")
            .AddChildContent<StackQRCodeBorder>(b => b.Add(x => x.Color, "red").Add(x => x.Width, 2)));
        cut.Find("path").GetAttribute("fill").Should().Be("#1e40af");
        cut.Find("rect").GetAttribute("fill").Should().Be("#eff6ff");
        cut.Find("svg").GetAttribute("style").Should().Contain("border:2px solid red");
    }

    [Fact]
    public void Image_And_Swiss_Overlays()
    {
        var image = RenderComponent<StackQRCode>(p => p
            .Add(q => q.Value, "https://example.com")
            .Add(q => q.ErrorCorrection, QRCodeErrorCorrection.High)
            .AddChildContent<StackQRCodeOverlay>(o => o.Add(x => x.ImageUrl, "logo.png")));
        image.Find("image[data-overlay=image]").GetAttribute("href").Should().Be("logo.png");

        var swiss = RenderComponent<StackQRCode>(p => p
            .Add(q => q.Value, "SPC")
            .AddChildContent<StackQRCodeOverlay>(o => o.Add(x => x.Type, QRCodeOverlayType.Swiss)));
        swiss.FindAll("[data-overlay=swiss] rect").Should().HaveCount(4);
    }

    [Fact]
    public void Too_Long_Value_Shows_Error()
    {
        var cut = RenderComponent<StackQRCode>(p => p
            .Add(q => q.Value, new string('a', 3000))
            .Add(q => q.ErrorCorrection, QRCodeErrorCorrection.High));
        cut.FindAll("svg").Should().BeEmpty();
        cut.Find("[role=alert]").TextContent.Should().Contain("too long");
    }
}
