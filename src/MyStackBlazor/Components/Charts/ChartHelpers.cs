using System.Globalization;

namespace MyStackBlazor.Components.Charts;

/// <summary>
/// Shared rendering helpers used by the inline-SVG chart components
/// (StackBarChart, StackLineChart, StackPieChart, and friends).
/// Ported verbatim from StackBarChart.razor's @code block (identical to
/// StackLineChart.razor's copy) so behavior/output is unchanged.
/// </summary>
internal static class ChartHelpers
{
    public static readonly string[] Palette =
    [
        "#6366f1", "#22c55e", "#f59e0b", "#ef4444",
        "#8b5cf6", "#06b6d4", "#f97316", "#ec4899"
    ];

    /// <summary>Returns the explicit color if provided, otherwise the palette color for the given index.</summary>
    public static string ResolveColor(string? explicitColor, int index)
        => explicitColor ?? Palette[index % Palette.Length];

    public static (double min, double max, int ticks) NiceScale(double rawMin, double rawMax)
    {
        var range = rawMax - rawMin;
        if (range <= 0) { rawMax = rawMin + 10; range = 10; }
        var step = NiceNum(range / 5.0, true);
        var nMin = Math.Floor(rawMin / step) * step;
        var nMax = Math.Ceiling(rawMax / step) * step;
        var t    = (int)Math.Round((nMax - nMin) / step);
        return (nMin, nMax, Math.Max(Math.Min(t, 8), 2));
    }

    public static double NiceNum(double x, bool round)
    {
        var exp = Math.Floor(Math.Log10(x));
        var f   = x / Math.Pow(10, exp);
        var nf  = round ? (f < 1.5 ? 1 : f < 3.5 ? 2 : f < 7.5 ? 5 : 10.0)
                        : (f <= 1  ? 1 : f <= 2   ? 2 : f <= 5   ? 5 : 10.0);
        return nf * Math.Pow(10, exp);
    }

    public static string FormatY(double v)
    {
        if (Math.Abs(v) >= 1_000_000) return $"{v / 1_000_000:F1}M";
        if (Math.Abs(v) >= 1_000)     return $"{v / 1_000:F1}k";
        if (v == Math.Floor(v))        return v.ToString("0", CultureInfo.InvariantCulture);
        return v.ToString("0.#", CultureInfo.InvariantCulture);
    }

    public static string SvgText(double x, double y, string anchor, int fontSize, string fill, string content, string fontWeight = "normal")
    {
        var escaped = System.Net.WebUtility.HtmlEncode(content);
        return $"<text x=\"{x.ToString("F1", CultureInfo.InvariantCulture)}\" y=\"{y.ToString("F1", CultureInfo.InvariantCulture)}\" text-anchor=\"{anchor}\" font-size=\"{fontSize}\" font-weight=\"{fontWeight}\" fill=\"{fill}\" aria-hidden=\"true\" focusable=\"false\">{escaped}</text>";
    }
}
