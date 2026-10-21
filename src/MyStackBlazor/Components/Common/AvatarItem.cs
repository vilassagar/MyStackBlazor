namespace MyStackBlazor.Components.Common;

/// <summary>One person in a <c>StackAvatarGroup</c>. Initials default to the first letters of <see cref="Name"/>.</summary>
public sealed record AvatarItem(string? Name, string? Src = null, string? Initials = null);
