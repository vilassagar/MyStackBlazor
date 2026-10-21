using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace MyStackBlazor.Components.Navigation;

/// <summary>
/// Same matching rules as Blazor's <c>NavLink</c>: <see cref="NavLinkMatch.All"/> matches the whole path,
/// <see cref="NavLinkMatch.Prefix"/> matches the link and anything below it. Query strings and fragments are ignored.
/// </summary>
public static class NavMatcher
{
    public static bool IsActive(NavigationManager nav, string? href, NavLinkMatch match)
    {
        if (href is null) return false;
        var target = StripQueryAndFragment(nav.ToAbsoluteUri(href).AbsoluteUri);
        var current = StripQueryAndFragment(nav.Uri);
        return IsMatch(current, target, match);
    }

    internal static bool IsMatch(string current, string target, NavLinkMatch match)
    {
        if (EqualsIgnoringTrailingSlash(current, target)) return true;
        if (match != NavLinkMatch.Prefix || target.Length == 0) return false;
        if (!current.StartsWith(target, StringComparison.OrdinalIgnoreCase)) return false;
        // "/orders" must match "/orders/5" but not "/orders-archive".
        return target.EndsWith('/') || current[target.Length] == '/';
    }

    private static bool EqualsIgnoringTrailingSlash(string a, string b) =>
        string.Equals(a.TrimEnd('/'), b.TrimEnd('/'), StringComparison.OrdinalIgnoreCase);

    private static string StripQueryAndFragment(string uri)
    {
        var i = uri.IndexOfAny(['?', '#']);
        return i < 0 ? uri : uri[..i];
    }
}
