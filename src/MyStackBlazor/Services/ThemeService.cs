namespace MyStackBlazor;

public enum AppTheme { Light, Dark, System }

public class ThemeService
{
    private AppTheme _theme = AppTheme.System;
    private bool _systemIsDark;

    public AppTheme Theme => _theme;

    /// <summary>True when the effective resolved theme is dark.</summary>
    public bool IsDark => _theme switch
    {
        AppTheme.Dark   => true,
        AppTheme.Light  => false,
        _               => _systemIsDark,
    };

    public event Action? OnThemeChanged;

    public void SetTheme(AppTheme theme)
    {
        _theme = theme;
        OnThemeChanged?.Invoke();
    }

    public void Toggle() =>
        SetTheme(IsDark ? AppTheme.Light : AppTheme.Dark);

    /// <summary>Called by ThemeProvider after detecting the OS preference.</summary>
    internal void NotifySystemPreference(bool isDark)
    {
        _systemIsDark = isDark;
        if (_theme == AppTheme.System)
            OnThemeChanged?.Invoke();
    }
}
