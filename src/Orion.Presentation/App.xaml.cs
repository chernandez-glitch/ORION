using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Orion.Configuration;
using Orion.Infrastructure;

namespace Orion.Presentation;

/// <summary>
/// Punto de entrada de la aplicación. Construye el contenedor de DI (composition
/// root), inicializa la base de datos y la configuración, y muestra la ventana
/// principal.
/// </summary>
public partial class App : Microsoft.UI.Xaml.Application
{
    private Window? _window;

    public App() => InitializeComponent();

    /// <summary>Contenedor de servicios accesible por las vistas para resolver sus ViewModels.</summary>
    public static IServiceProvider Services { get; private set; } = null!;

    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        Services = CompositionRoot.Build();
        var logger = Services.GetRequiredService<ILogger<App>>();

        try
        {
            await Services.InitializeDatabaseAsync().ConfigureAwait(true);

            var configuration = Services.GetRequiredService<IConfigurationService>();
            await configuration.LoadAsync().ConfigureAwait(true);

            _window = Services.GetRequiredService<MainWindow>();
            ApplyTheme(configuration.Current.Appearance.Theme);
            _window.Activate();

            logger.LogInformation("ORION AI iniciado correctamente.");
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Fallo crítico durante el arranque de ORION.");
            throw;
        }
    }

    /// <summary>Aplica la preferencia de tema a la ventana principal.</summary>
    public void ApplyTheme(ThemePreference theme)
    {
        if (_window?.Content is FrameworkElement root)
        {
            root.RequestedTheme = theme switch
            {
                ThemePreference.Light => ElementTheme.Light,
                ThemePreference.Dark => ElementTheme.Dark,
                _ => ElementTheme.Default
            };
        }
    }
}
