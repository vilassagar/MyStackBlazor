namespace MyStackBlazor;

/// <summary>
/// Lucide icon names available to <c>&lt;StackLucideIcon&gt;</c>. The glyphs live in a static SVG sprite
/// (<c>_content/MyStackBlazor/icons/lucide-sprite.svg</c>) that the browser downloads once and caches,
/// so the icon set adds nothing to the app's .dll size.
/// </summary>
public static partial class LucideIcons
{
    /// <summary>Default URL of the bundled sprite.</summary>
    public const string DefaultSpriteUrl = "_content/MyStackBlazor/icons/lucide-sprite.svg";

    /// <summary>Every current (non-alias) icon name in the sprite, e.g. "house", "triangle-alert".</summary>
    public static IReadOnlyCollection<string> Names => _names;

    /// <summary>
    /// Normalizes a name to its sprite id: trims, lower-cases, converts PascalCase/camelCase to kebab-case
    /// ("ChevronDown" → "chevron-down") and maps legacy names ("home" → "house", "alert-triangle" → "triangle-alert").
    /// Returns null when the icon does not exist.
    /// </summary>
    public static string? Resolve(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        var s = name.Trim();
        if (s.StartsWith("lucide-", StringComparison.OrdinalIgnoreCase)) s = s[7..];

        // Lucide usually dashes digits ("Loader2" → "loader-2") but not always ("Grid2x2" → "grid-2x2"), so try both.
        return Lookup(ToKebab(s, dashDigits: true)) ?? Lookup(ToKebab(s, dashDigits: false));
    }

    /// <summary>True when <paramref name="name"/> (or its legacy alias) exists in the sprite.</summary>
    public static bool Exists(string? name) => Resolve(name) is not null;

    private static string? Lookup(string key) =>
        _names.Contains(key) ? key : _aliases.GetValueOrDefault(key);

    private static string ToKebab(string s, bool dashDigits)
    {
        var sb = new System.Text.StringBuilder(s.Length + 8);
        for (var i = 0; i < s.Length; i++)
        {
            var c = s[i];
            if (c is '_' or ' ') { sb.Append('-'); continue; }
            var prev = i > 0 ? s[i - 1] : '\0';
            var boundary = char.IsUpper(c) && i > 0 && prev != '-'
                        || dashDigits && char.IsDigit(c) && char.IsLetter(prev);
            if (boundary && sb.Length > 0 && sb[^1] != '-') sb.Append('-');
            sb.Append(char.ToLowerInvariant(c));
        }
        return sb.ToString();
    }
}
