namespace MyStackBlazor.Components.Data;

/// <summary>An item dropped into a <c>StackDropZone</c>. <see cref="Index"/> is the position within the target zone's items.</summary>
public sealed record DropItemEventArgs<TItem>(TItem Item, string ZoneIdentifier, int Index);
