using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orion.AI;
using Orion.Application;
using Orion.Automation;
using Orion.Configuration;
using Orion.Infrastructure;
using Orion.Installer;
using Orion.Memory;
using Orion.Memory.Engine;
using Orion.Memory.Engine.Services;
using Orion.Plugins;
using Orion.Windows;
using Orion.Presentation.Services;
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
        services.AddOrionMemoryEngine();
        // Integración: el Command Engine persiste su historial en el Memory Engine
        // (y clasifica apps/carpetas) sin modificar el motor — solo la costura ICommandHistory.
        services.AddScoped<Orion.Application.Commands.ICommandHistory, MemoryEngineCommandHistory>();
        services.AddOrionAI();
        services.AddOrionVoice();
        services.AddOrionAutomation();
        services.AddOrionWindows();
        services.AddOrionInstaller();
        services.AddOrionPlugins();

        // Adaptador real de auto-update (Velopack) — reemplaza al placeholder de fase.
        var feedUrl = Environment.GetEnvironmentVariable("ORION_UPDATE_FEED")
            ?? "https://github.com/chernandez-glitch/ORION";
        services.AddSingleton<Orion.Installer.IUpdateService>(sp =>
            new Orion.Installer.VelopackUpdateService(
                sp.GetRequiredService<ILogger<Orion.Installer.VelopackUpdateService>>(), feedUrl));

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
        // Servicios de UI (navegación, tema y diálogos).
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IThemeService, ThemeService>();
        services.AddSingleton<DialogService>();
        services.AddSingleton<Orion.Application.Commands.Abstractions.IDialogService>(
            sp => sp.GetRequiredService<DialogService>());

        // Ventana y shell.
        services.AddSingleton<MainWindow>();
        services.AddSingleton<ShellViewModel>();
        services.AddSingleton<CommandPaletteViewModel>();

        // Páginas.
        services.AddTransient<DashboardPage>();
        services.AddTransient<ConversationsPage>();
        services.AddTransient<AutomationPage>();
        services.AddTransient<CommandsPage>();
        services.AddTransient<MemoryPage>();
        services.AddTransient<MemoryCenterPage>();
        services.AddTransient<PluginsPage>();
        services.AddTransient<LogsPage>();
        services.AddTransient<SettingsPage>();
        services.AddTransient<AboutPage>();

        // ViewModels.
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<ConversationsViewModel>();
        services.AddTransient<AutomationViewModel>();
        services.AddTransient<CommandsViewModel>();
        services.AddTransient<MemoryViewModel>();
        services.AddTransient<MemoryCenterViewModel>();
        services.AddTransient<PluginsViewModel>();
        services.AddTransient<LogsViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<AboutViewModel>();
    }
}
