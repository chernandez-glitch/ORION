using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using Orion.Presentation.Views;

namespace Orion.Presentation.Services;

/// <summary>
/// Implementación de <see cref="INavigationService"/> sobre un <see cref="Frame"/>.
/// El mapa clave → tipo de página es la única fuente de verdad de la navegación.
/// </summary>
public sealed class NavigationService : INavigationService
{
    private static readonly Dictionary<string, Type> Routes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["dashboard"] = typeof(DashboardPage),
        ["conversations"] = typeof(ConversationsPage),
        ["automation"] = typeof(AutomationPage),
        ["commands"] = typeof(CommandsPage),
        ["memory"] = typeof(MemoryPage),
        ["plugins"] = typeof(PluginsPage),
        ["logs"] = typeof(LogsPage),
        ["settings"] = typeof(SettingsPage),
        ["about"] = typeof(AboutPage)
    };

    private Frame? _frame;

    public event EventHandler<string>? Navigated;

    public bool CanGoBack => _frame?.CanGoBack ?? false;

    public void Initialize(Frame frame) => _frame = frame;

    public bool NavigateTo(string key)
    {
        if (_frame is null || !Routes.TryGetValue(key, out var pageType))
        {
            return false;
        }

        if (_frame.CurrentSourcePageType == pageType)
        {
            return true;
        }

        var navigated = _frame.Navigate(pageType, null, new DrillInNavigationTransitionInfo());
        if (navigated)
        {
            Navigated?.Invoke(this, key);
        }

        return navigated;
    }

    public void GoBack()
    {
        if (_frame?.CanGoBack == true)
        {
            _frame.GoBack();
        }
    }
}
