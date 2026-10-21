namespace MyStackBlazor.Components.Layout;

public enum SwipeDirection { Left, Right, Up, Down }

/// <summary>A detected swipe. <see cref="Distance"/> is in pixels along the swipe axis (negative = left/up).</summary>
public sealed record SwipeEventArgs(SwipeDirection Direction, double Distance);
