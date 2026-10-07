using Microsoft.AspNetCore.Components;

namespace MyStackBlazor.Components.Data;

/// <summary>Implemented by <see cref="StackQRCode"/>; lets its settings children register.</summary>
internal interface IQRCodeHost
{
    void SetBorder(StackQRCodeBorder? border);
    void SetOverlay(StackQRCodeOverlay? overlay);
}

/// <summary>What is drawn in the centre of the QR code.</summary>
public enum QRCodeOverlayType
{
    /// <summary>The image from <c>ImageUrl</c>.</summary>
    Image,
    /// <summary>The Swiss cross used on Swiss QR-bill payment codes.</summary>
    Swiss,
}

/// <summary>Draws a border around the QR code. Place it inside <see cref="StackQRCode"/>.</summary>
public class StackQRCodeBorder : ComponentBase, IDisposable
{
    [CascadingParameter] private IQRCodeHost? Host { get; set; }

    [Parameter] public string Color { get; set; } = "currentColor";
    /// <summary>Border width in pixels (default 1).</summary>
    [Parameter] public double Width { get; set; } = 1;

    protected override void OnParametersSet() => Host?.SetBorder(this);
    public void Dispose() => Host?.SetBorder(null);
}

/// <summary>
/// An image or Swiss cross in the centre of the QR code. Place it inside <see cref="StackQRCode"/>
/// and use ErrorCorrection High so the covered modules can be recovered.
/// </summary>
public class StackQRCodeOverlay : ComponentBase, IDisposable
{
    [CascadingParameter] private IQRCodeHost? Host { get; set; }

    [Parameter] public QRCodeOverlayType Type { get; set; } = QRCodeOverlayType.Image;
    [Parameter] public string? ImageUrl { get; set; }
    /// <summary>Overlay width in pixels. Defaults to about a fifth of the code.</summary>
    [Parameter] public double? Width { get; set; }
    /// <summary>Overlay height in pixels. Defaults to the width.</summary>
    [Parameter] public double? Height { get; set; }

    protected override void OnParametersSet() => Host?.SetOverlay(this);
    public void Dispose() => Host?.SetOverlay(null);
}
