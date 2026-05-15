namespace MyStackBlazor.Components.Blocks;

public record FeatureItem(string Title, string Description, string? IconSvg = null);

public record TestimonialItem(
    string Quote,
    string Author,
    string Role,
    string? AvatarUrl   = null,
    string? AvatarInit  = null,
    int     Rating      = 5);

public record PricingFeature(string Text, bool Included = true);

public record PricingTier(
    string Name,
    string Price,
    string Period,
    string?               Description = null,
    IList<PricingFeature>? Features   = null,
    bool   IsPopular   = false,
    string ButtonText  = "Get started");

public record StatItem(
    string Value,
    string Label,
    string? Description = null,
    string? Trend       = null,
    bool    TrendUp     = true);

public record PartnerItem(string Name, string? LogoUrl = null);

public record IncentiveItem(string Title, string Description, string? IconSvg = null);

public record LocationItem(
    string  Name,
    string  Address,
    string? Phone  = null,
    string? Email  = null,
    string? Hours  = null);

public record BlockLink(string Text, string Href);

public record FooterSection(string Title, IList<BlockLink> Links);

public record CardBlockItem(
    string  Title,
    string? Description = null,
    string? ImageUrl    = null,
    string? BadgeText   = null,
    string? Href        = null);

public record FilterCategory(string Name, IList<FilterOption> Options);

public record FilterOption(string Label, string Value, int Count = 0);

public record SortOption(string Label, string Value);

public record NavCategory(string Name, string Href = "#", IList<BlockLink>? Children = null);

public record DashboardNavItem(string Label, string Href, string? IconSvg = null, bool IsActive = false);

public record NavBreadcrumb(string Label, string? Href = null);

public record ProductItem(
    string  Title,
    string  Price,
    string? ImageUrl    = null,
    string? BadgeText   = null,
    decimal Rating      = 0,
    int     ReviewCount = 0,
    bool    IsInStock   = true);
