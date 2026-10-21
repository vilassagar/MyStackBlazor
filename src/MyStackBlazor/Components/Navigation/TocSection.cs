namespace MyStackBlazor.Components.Navigation;

/// <summary>One entry of a <c>StackTableOfContents</c>: the target element id, its text and heading level (2–4).</summary>
public sealed record TocSection(string Id, string Text, int Level = 2);
