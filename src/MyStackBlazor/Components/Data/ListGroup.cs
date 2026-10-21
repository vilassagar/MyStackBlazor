namespace MyStackBlazor.Components.Data;

/// <summary>One group of a grouped <c>StackListView</c>.</summary>
public sealed record ListGroup<TItem>(object? Key, IReadOnlyList<TItem> Items);
