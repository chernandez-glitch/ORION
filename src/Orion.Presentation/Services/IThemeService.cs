using Microsoft.UI.Xaml;
using Orion.Configuration;

namespace Orion.Presentation.Services;

/// <summary>Aplica y persiste el tema de la interfaz.</summary>
public interface IThemeService
{
    ThemePreference Current { get; }

    void Initialize(FrameworkElement root);

    void Apply(ThemePreference theme);

    /// <summary>Alterna entre claro y oscuro, aplica y persiste.</summary>
    void Toggle();
}
