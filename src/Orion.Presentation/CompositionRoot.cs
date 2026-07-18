using Microsoft.Extensions.DependencyInjection;
using Orion.AI;
using Orion.Application;
using Orion.Automation;
using Orion.Configuration;
using Orion.Infrastructure;
using Orion.Installer;
using Orion.Memory;
using Orion.Plugins;
using Orion.Presentation.ViewModels;
using Orion.Presentation.Views;
using Orion.Voice;

namespace Orion.Presentation;

/// <summary>
/// Ensambla el grafo completo de dependencias de ORION: infraestructura,
/// aplicación y todos los módulos, más las vistas y ViewModels de la UI.
/// Es el único lugar donde se conocen todas las capas.
/// </summary>
public static class CompositionRoot
{
    public static IServiceProvider Build()
    {
        var services = new ServiceCollection();

        // Transversal + módulos (el orden no importa salvo que Memory/Application
        // dependen de repos registrados por Infrastructure en tiempo de ejecución).
        services.AddOrionLogging();
        services.AddOrionConfiguration();
        services.AddOrionInfrastructure();
        services.AddOrionApplication();
        services.AddOrionMemory();
        services.AddOrionAI();
        services.AddOrionVoice();
        services.AddOrionAutomation();
        services.AddOrionInstaller();
        services.AddOrionPlugins();

        // UI: ventana, vistas y ViewModels.
        RegisterPresentation(services);

        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = false
        });
    }

    private static void RegisterPresentation(IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();

        services.AddTransient<DashboardPage>();
        services.AddTransient<CommandsPage>();
        services.AddTransient<MemoryPage>();
        services.AddTransient<SettingsPage>();

        services.AddTransient<DashboardViewModel>();
        services.AddTransient<CommandsViewModel>();
        services.AddTransient<MemoryViewModel>();
        services.AddTransient<SettingsViewModel>();
    }
}
