using MyStackBlazor.Components.Common;

namespace MyStackBlazor.UnitTests.Components;

// ═══════════════════════════════════════════════════════════════════════════════
// Avatar
// ═══════════════════════════════════════════════════════════════════════════════

public class AvatarTests : TestContext
{
    [Fact]
    public void Avatar_Renders_RoleImg()
    {
        var cut = RenderComponent<Avatar>();
        cut.Find("[role=img]").Should().NotBeNull();
    }

    [Fact]
    public void Avatar_Src_RendersImageElement()
    {
        var cut = RenderComponent<Avatar>(p => p
            .Add(a => a.Src, "/img/user.png")
            .Add(a => a.Alt, "User photo"));

        cut.Find("img").GetAttribute("src").Should().Be("/img/user.png");
    }

    [Fact]
    public void Avatar_Alt_SetAsAriaLabelOnRoot()
    {
        var cut = RenderComponent<Avatar>(p => p
            .Add(a => a.Src, "/img/user.png")
            .Add(a => a.Alt, "Jane Doe"));

        // The <img> always has alt=""; Alt parameter maps to aria-label on the [role=img] wrapper
        cut.Find("[role=img]").GetAttribute("aria-label").Should().Be("Jane Doe");
    }

    [Fact]
    public void Avatar_Initials_Shown_WhenNoSrc()
    {
        var cut = RenderComponent<Avatar>(p => p
            .Add(a => a.Initials, "JD"));

        cut.Find("[role=img]").TextContent.Should().Contain("JD");
    }

    [Theory]
    [InlineData("sm")]
    [InlineData("default")]
    [InlineData("lg")]
    [InlineData("xl")]
    public void Avatar_SizeParameter_DoesNotThrow(string size)
    {
        var act = () => RenderComponent<Avatar>(p => p.Add(a => a.Size, size));
        act.Should().NotThrow();
    }

    [Fact]
    public void Avatar_AriaLabel_SetFromAlt()
    {
        var cut = RenderComponent<Avatar>(p => p
            .Add(a => a.Src, "/img/x.png")
            .Add(a => a.Alt, "Profile picture"));

        cut.Find("[role=img]").GetAttribute("aria-label").Should().Be("Profile picture");
    }

    [Fact]
    public void Avatar_NoSrc_NoInitials_RendersPlaceholder()
    {
        var act = () => RenderComponent<Avatar>();
        act.Should().NotThrow();
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Badge
// ═══════════════════════════════════════════════════════════════════════════════

public class BadgeTests : TestContext
{
    [Fact]
    public void Badge_Renders_ChildContent()
    {
        var cut = RenderComponent<Badge>(p => p.AddChildContent("New"));
        cut.Markup.Should().Contain("New");
    }

    [Theory]
    [InlineData("default",     "bg-primary")]
    [InlineData("secondary",   "bg-secondary")]
    [InlineData("destructive", "bg-destructive")]
    [InlineData("outline",     "text-foreground")]
    public void Badge_Variant_AppliesClass(string variant, string expectedClass)
    {
        var cut = RenderComponent<Badge>(p => p
            .Add(b => b.Variant, variant)
            .AddChildContent("label"));

        cut.Find("span").ClassName.Should().Contain(expectedClass);
    }

    [Fact]
    public void Badge_Default_Variant_Applied_WhenNotSpecified()
    {
        var cut = RenderComponent<Badge>(p => p.AddChildContent("x"));
        cut.Find("span").ClassName.Should().Contain("bg-primary");
    }

    [Fact]
    public void Badge_AdditionalAttributes_PassedThrough()
    {
        var cut = RenderComponent<Badge>(p => p
            .AddUnmatched("data-testid", "badge-1")
            .AddChildContent("x"));

        cut.Find("[data-testid=badge-1]").Should().NotBeNull();
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Skeleton
// ═══════════════════════════════════════════════════════════════════════════════

public class SkeletonTests : TestContext
{
    [Fact]
    public void Skeleton_Renders_StatusRole()
    {
        var cut = RenderComponent<Skeleton>();
        cut.Find("[role=status]").Should().NotBeNull();
    }

    [Fact]
    public void Skeleton_DefaultAriaLabel_IsLoading()
    {
        var cut = RenderComponent<Skeleton>();
        var el = cut.Find("[role=status]");
        el.GetAttribute("aria-label").Should().Be("Loading...");
    }

    [Fact]
    public void Skeleton_CustomLabel_SetOnElement()
    {
        var cut = RenderComponent<Skeleton>(p => p.Add(s => s.Label, "Loading content"));
        cut.Find("[role=status]").GetAttribute("aria-label").Should().Be("Loading content");
    }

    [Fact]
    public void Skeleton_Class_AppendedToRoot()
    {
        var cut = RenderComponent<Skeleton>(p => p.Add(s => s.Class, "h-4 w-32"));
        cut.Find("[role=status]").ClassName.Should().Contain("h-4");
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Spinner
// ═══════════════════════════════════════════════════════════════════════════════

public class SpinnerTests : TestContext
{
    [Fact]
    public void Spinner_Renders_StatusRole()
    {
        var cut = RenderComponent<Spinner>();
        cut.Find("[role=status]").Should().NotBeNull();
    }

    [Fact]
    public void Spinner_DefaultLabel_IsLoading()
    {
        var cut = RenderComponent<Spinner>();
        cut.Find("[role=status]").GetAttribute("aria-label").Should().Be("Loading...");
    }

    [Fact]
    public void Spinner_CustomLabel_SetOnElement()
    {
        var cut = RenderComponent<Spinner>(p => p.Add(s => s.Label, "Saving…"));
        cut.Find("[role=status]").GetAttribute("aria-label").Should().Be("Saving…");
    }

    [Theory]
    [InlineData("sm")]
    [InlineData("default")]
    [InlineData("lg")]
    public void Spinner_SizeVariant_DoesNotThrow(string size)
    {
        var act = () => RenderComponent<Spinner>(p => p.Add(s => s.Size, size));
        act.Should().NotThrow();
    }
}

// ═══════════════════════════════════════════════════════════════════════════════
// Typography
// ═══════════════════════════════════════════════════════════════════════════════

public class TypographyTests : TestContext
{
    [Theory]
    [InlineData("h1", "h1")]
    [InlineData("h2", "h2")]
    [InlineData("h3", "h3")]
    [InlineData("h4", "h4")]
    [InlineData("p",  "p")]
    public void Typography_Variant_RendersCorrectElement(string variant, string tag)
    {
        var cut = RenderComponent<Typography>(p => p
            .Add(t => t.Variant, variant)
            .AddChildContent("Sample text"));

        cut.Find(tag).Should().NotBeNull();
    }

    [Fact]
    public void Typography_ChildContent_Rendered()
    {
        var cut = RenderComponent<Typography>(p => p
            .Add(t => t.Variant, "p")
            .AddChildContent("Hello world"));

        cut.Markup.Should().Contain("Hello world");
    }

    [Fact]
    public void Typography_Class_AppendedToElement()
    {
        var cut = RenderComponent<Typography>(p => p
            .Add(t => t.Variant, "p")
            .Add(t => t.Class, "custom-class")
            .AddChildContent("text"));

        cut.Find("p").ClassName.Should().Contain("custom-class");
    }

    [Fact]
    public void Typography_Default_Variant_DoesNotThrow()
    {
        var act = () => RenderComponent<Typography>(p => p.AddChildContent("text"));
        act.Should().NotThrow();
    }
}
