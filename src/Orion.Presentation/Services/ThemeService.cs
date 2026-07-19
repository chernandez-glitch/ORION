using Microsoft.UI.Xaml;
using Orion.Configuration;

namespace Orion.Presentation.Services;

/// <summary>
/// Aplica el <see cref="ThemePreference"/> al elemento raíz del shell y lo
/// persiste vía <see cref="IConfigurationService"/>.
/// </summary>
public sealed class ThemeService(IConfigurationService configuration) : IThemeService
{
    private FrameworkElement? _root;

    public ThemePreference Current => configuration.Current.Appearance.Theme;

    public void Initialize(FrameworkElement root)
    {
        _root = root;
        Apply(Current);
    }

    public void Apply(ThemePreference theme)
    {
        if (_root is not null)
        {
            _root.RequestedTheme = theme switch
            {
                ThemePreference.Light => ElementTheme.Light,
                ThemePreference.Dark => ElementTheme.Dark,
                _ => ElementTheme.Default
            };
        }
    }

    public void Toggle()
    {
        var effective = ResolveEffective();
        var next = effective == ElementTheme.Dark ? ThemePreference.Light : ThemePreference.Dark;

        Apply(next);

        var settings = configuration.Current;
        settings.Appearance.Theme = next;
        _ = configuration.SaveAsync(settings);
    }

    private ElementTheme ResolveEffective()
    {
        if (_root is { ActualTheme: var actual })
        {
            return actual;
        }

        return Current switch
        {
            ThemePreference.Light => ElementTheme.Light,
            ThemePreference.Dark => ElementTheme.Dark,
            _ => ElementTheme.Default
        };
    }
}
