using System.Globalization;
using System.Text.RegularExpressions;

namespace MyStackBlazor.Components.Form;

/// <summary>Text format of colour values.</summary>
public enum ColorFormat
{
    /// <summary>"#rrggbb" (or "#rrggbbaa" when not opaque).</summary>
    Hex,
    /// <summary>"rgb(r, g, b)" (or "rgba(r, g, b, a)" when not opaque).</summary>
    Rgb,
}

/// <summary>The views of a FlatColorPicker / ColorPicker popup.</summary>
public enum ColorPickerView
{
    Gradient,
    Palette,
}

/// <summary>Built-in palettes for <see cref="StackColorPalette"/>.</summary>
public enum ColorPalettePresets
{
    /// <summary>20 classic colours.</summary>
    Basic,
    /// <summary>10 theme colours with 5 tints/shades each.</summary>
    Office,
    /// <summary>Greys from white to black.</summary>
    Monochrome,
}

/// <summary>Arguments for colour picker open/close events. Set <see cref="IsCancelled"/> to prevent it.</summary>
public class ColorPickerOpenCloseEventArgs
{
    public bool IsCancelled { get; set; }
}

/// <summary>An RGBA colour with conversions to/from text and HSV.</summary>
public readonly record struct RgbaColor(byte R, byte G, byte B, double A = 1)
{
    private static readonly Regex RgbPattern = new(@"^rgba?\(\s*(\d{1,3})\s*,\s*(\d{1,3})\s*,\s*(\d{1,3})\s*(?:,\s*([\d.]+)\s*)?\)$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    /// <summary>Parses "#rgb", "#rrggbb", "#rrggbbaa", "rgb(…)" or "rgba(…)".</summary>
    public static RgbaColor? Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;
        var s = text.Trim();
        if (s.StartsWith('#'))
        {
            var hex = s[1..];
            if (hex.Length == 3) hex = string.Concat(hex.Select(c => $"{c}{c}"));
            if ((hex.Length != 6 && hex.Length != 8) || !hex.All(Uri.IsHexDigit)) return null;
            var r = Convert.ToByte(hex[..2], 16);
            var g = Convert.ToByte(hex[2..4], 16);
            var b = Convert.ToByte(hex[4..6], 16);
            var a = hex.Length == 8 ? Convert.ToByte(hex[6..8], 16) / 255.0 : 1;
            return new RgbaColor(r, g, b, Math.Round(a, 2));
        }
        var m = RgbPattern.Match(s);
        if (!m.Success) return null;
        int Channel(int i) => Math.Clamp(int.Parse(m.Groups[i].Value, CultureInfo.InvariantCulture), 0, 255);
        var alpha = m.Groups[4].Success && double.TryParse(m.Groups[4].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) ? Math.Clamp(parsed, 0, 1) : 1;
        return new RgbaColor((byte)Channel(1), (byte)Channel(2), (byte)Channel(3), alpha);
    }

    public string ToHex() => A >= 1
        ? $"#{R:x2}{G:x2}{B:x2}"
        : $"#{R:x2}{G:x2}{B:x2}{(byte)Math.Round(A * 255):x2}";

    public string ToRgb() => A >= 1
        ? $"rgb({R}, {G}, {B})"
        : $"rgba({R}, {G}, {B}, {A.ToString("0.##", CultureInfo.InvariantCulture)})";

    public string Format(ColorFormat format) => format == ColorFormat.Rgb ? ToRgb() : ToHex();

    /// <summary>Hue 0–360, saturation and value 0–1.</summary>
    public (double H, double S, double V) ToHsv()
    {
        double r = R / 255.0, g = G / 255.0, b = B / 255.0;
        double max = Math.Max(r, Math.Max(g, b)), min = Math.Min(r, Math.Min(g, b)), d = max - min;
        double h = d == 0 ? 0 : max == r ? 60 * (((g - b) / d) % 6) : max == g ? 60 * ((b - r) / d + 2) : 60 * ((r - g) / d + 4);
        if (h < 0) h += 360;
        return (h, max == 0 ? 0 : d / max, max);
    }

    public static RgbaColor FromHsv(double h, double s, double v, double a = 1)
    {
        h = ((h % 360) + 360) % 360;
        s = Math.Clamp(s, 0, 1);
        v = Math.Clamp(v, 0, 1);
        var c = v * s;
        var x = c * (1 - Math.Abs(h / 60 % 2 - 1));
        var m = v - c;
        var (r, g, b) = h switch
        {
            < 60  => (c, x, 0d),
            < 120 => (x, c, 0d),
            < 180 => (0d, c, x),
            < 240 => (0d, x, c),
            < 300 => (x, 0d, c),
            _     => (c, 0d, x),
        };
        static byte To(double value) => (byte)Math.Round(Math.Clamp(value, 0, 1) * 255);
        return new RgbaColor(To(r + m), To(g + m), To(b + m), Math.Clamp(a, 0, 1));
    }

    /// <summary>Mixes towards white (amount &gt; 0) or black (amount &lt; 0).</summary>
    public RgbaColor Tint(double amount)
    {
        byte Mix(byte c) => (byte)Math.Round(amount >= 0 ? c + (255 - c) * amount : c * (1 + amount));
        return new RgbaColor(Mix(R), Mix(G), Mix(B), A);
    }
}

internal static class ColorPresets
{
    private static readonly string[] Basic =
    [
        "#000000", "#7f7f7f", "#880015", "#ed1c24", "#ff7f27", "#fff200", "#22b14c", "#00a2e8", "#3f48cc", "#a349a4",
        "#ffffff", "#c3c3c3", "#b97a57", "#ffaec9", "#ffc90e", "#efe4b0", "#b5e61d", "#99d9ea", "#7092be", "#c8bfe7",
    ];

    private static readonly string[] OfficeBase =
        ["#ffffff", "#000000", "#e7e6e6", "#44546a", "#4472c4", "#ed7d31", "#a5a5a5", "#ffc000", "#5b9bd5", "#70ad47"];

    public static IReadOnlyList<string> Colors(ColorPalettePresets preset) => preset switch
    {
        ColorPalettePresets.Office =>
        [
            .. OfficeBase,
            // Rows of tints and shades of each base colour, like Office theme palettes.
            .. new[] { 0.8, 0.6, 0.4, -0.25, -0.5 }.SelectMany(amount => OfficeBase.Select(c => RgbaColor.Parse(c)!.Value.Tint(amount).ToHex())),
        ],
        ColorPalettePresets.Monochrome => Enumerable.Range(0, 10).Select(i => RgbaColor.FromHsv(0, 0, 1 - i / 9.0).ToHex()).ToList(),
        _ => Basic,
    };
}
