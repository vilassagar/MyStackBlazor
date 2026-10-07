using Microsoft.AspNetCore.Components;

namespace MyStackBlazor.Components.Data;

/// <summary>Implemented by <see cref="StackBarcode"/>; lets its settings children register.</summary>
internal interface IBarcodeHost
{
    void SetBorder(StackBarcodeBorder? border);
    void SetText(StackBarcodeText? text);
}

/// <summary>Kept for Telerik parity; <see cref="StackBarcode"/> always renders SVG.</summary>
public enum BarcodeRenderingMode
{
    Svg,
    Canvas,
}

/// <summary>Line style of the barcode border.</summary>
public enum BarcodeDashType
{
    Solid,
    Dash,
    Dot,
    LongDash,
}

/// <summary>Draws a border around the barcode. Place it inside <see cref="StackBarcode"/>.</summary>
public class StackBarcodeBorder : ComponentBase, IDisposable
{
    [CascadingParameter] private IBarcodeHost? Host { get; set; }

    [Parameter] public string Color { get; set; } = "currentColor";
    /// <summary>Border width in pixels (default 1).</summary>
    [Parameter] public double Width { get; set; } = 1;
    [Parameter] public BarcodeDashType DashType { get; set; } = BarcodeDashType.Solid;

    protected override void OnParametersSet() => Host?.SetBorder(this);
    public void Dispose() => Host?.SetBorder(null);
}

/// <summary>Configures the human-readable text under the bars. Place it inside <see cref="StackBarcode"/>.</summary>
public class StackBarcodeText : ComponentBase, IDisposable
{
    [CascadingParameter] private IBarcodeHost? Host { get; set; }

    [Parameter] public bool Visible { get; set; } = true;
    /// <summary>Text colour. Defaults to the barcode's Color.</summary>
    [Parameter] public string? Color { get; set; }
    /// <summary>CSS font shorthand; the pixel size sets the height reserved for the text.</summary>
    [Parameter] public string Font { get; set; } = "16px Consolas, Monaco, 'Sans Mono', monospace, sans-serif";
    /// <summary>Space between the bars and the text, in pixels.</summary>
    [Parameter] public double Margin { get; set; } = 2;

    protected override void OnParametersSet() => Host?.SetText(this);
    public void Dispose() => Host?.SetText(null);
}
