namespace MyStackBlazor.Components.Charts;

public record ChartSeries(
    string Name,
    IReadOnlyList<double> Values,
    string? Color = null);

public record ChartSegment(
    string Label,
    double Value,
    string? Color = null);

/// <summary>
/// Color threshold for <see cref="GaugeChart"/>.
/// The gauge arc turns <see cref="Color"/> when the normalised fraction (0–1) is ≤ <see cref="At"/>.
/// </summary>
public record GaugeThreshold(double At, string Color);

public record ChartPoint(double X, double Y, string? Label = null, string? Color = null);

public record ChartPointSeries(
    string Name,
    IReadOnlyList<ChartPoint> Points,
    string? Color = null);

/// <summary>A point on a <c>StackTimeSeriesChart</c>. A null <see cref="Value"/> leaves a gap in the line.</summary>
public record TimePoint(DateTime Time, double? Value);

public record TimeSeries(
    string Name,
    IReadOnlyList<TimePoint> Points,
    string? Color = null);

/// <summary>A flow of <see cref="Value"/> from node <see cref="Source"/> to node <see cref="Target"/> in a <c>StackSankeyChart</c>.</summary>
public record SankeyLink(string Source, string Target, double Value);
