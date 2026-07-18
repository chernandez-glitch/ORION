using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Orion.Shared.Modules;

namespace Orion.Plugins;

public static class DependencyInjection
{
    /// <summary>
    /// Registra el sistema de plugins: descubre los plugins del directorio
    /// indicado (por defecto <c>&lt;base&gt;\plugins</c>), deja que cada uno aporte
    /// sus servicios/comandos al contenedor y publica el catálogo resultante.
    /// </summary>
    public static IServiceCollection AddOrionPlugins(this IServiceCollection services, string? pluginsDirectory = null)
    {
        var directory = pluginsDirectory ?? DefaultPluginsDirectory();
        var loader = new PluginLoader(directory, NullLogger<PluginLoader>.Instance);
        var plugins = loader.Load();

        foreach (var plugin in plugins)
        {
            plugin.Register(services);
        }

        var catalog = new PluginCatalog(plugins.Select(p => p.Metadata).ToArray());

        services.AddSingleton<IPluginLoader>(loader);
        services.AddSingleton<IPluginCatalog>(catalog);
        services.AddSingleton<IModuleStatusProvider>(new PluginsStatusProvider(catalog));

        return services;
    }

    public static string DefaultPluginsDirectory() => Path.Combine(AppContext.BaseDirectory, "plugins");
}
