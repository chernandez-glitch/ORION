using Microsoft.UI.Xaml.Controls;

namespace Orion.Presentation.Services;

/// <summary>
/// Navegación desacoplada de la UI: mapea una clave lógica a la página que la
/// implementa y la muestra en el Frame del shell.
/// </summary>
public interface INavigationService
{
    event EventHandler<string>? Navigated;

    bool CanGoBack { get; }

    void Initialize(Frame frame);

    bool NavigateTo(string key);

    void GoBack();
}
