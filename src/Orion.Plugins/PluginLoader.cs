using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.Logging;

namespace Orion.Plugins;

/// <summary>
/// Carga plugins desde ensamblados <c>*.dll</c> en el directorio de plugins.
/// Cada ensamblado se inspecciona en busca de tipos que implementen
/// <see cref="IOrionPlugin"/> con constructor sin parámetros. Los fallos de un
/// ensamblado se registran y se omiten, sin tumbar la aplicación.
/// </summary>
public sealed class PluginLoader(string pluginsDirectory, ILogger<PluginLoader> logger) : IPluginLoader
{
    public string PluginsDirectory { get; } = pluginsDirectory;

    public IReadOnlyList<IOrionPlugin> Load()
    {
        if (!Directory.Exists(PluginsDirectory))
        {
            logger.LogInformation("No existe el directorio de plugins '{Directory}'; no se cargan plugins.", PluginsDirectory);
            return [];
        }

        var plugins = new List<IOrionPlugin>();

        foreach (var dll in Directory.EnumerateFiles(PluginsDirectory, "*.dll", SearchOption.TopDirectoryOnly))
        {
            try
            {
                var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(Path.GetFullPath(dll));
                plugins.AddRange(InstantiatePlugins(assembly));
            }
            catch (Exception ex) when (ex is BadImageFormatException or FileLoadException or ReflectionTypeLoadException)
            {
                logger.LogWarning(ex, "No se pudo cargar el ensamblado de plugin '{Dll}'.", dll);
            }
        }

        logger.LogInformation("Se cargaron {Count} plugin(s) desde '{Directory}'.", plugins.Count, PluginsDirectory);
        return plugins;
    }

    private IEnumerable<IOrionPlugin> InstantiatePlugins(Assembly assembly)
    {
        foreach (var type in assembly.GetExportedTypes())
        {
            if (type is not { IsClass: true, IsAbstract: false } || !typeof(IOrionPlugin).IsAssignableFrom(type))
            {
                continue;
            }

            if (Activator.CreateInstance(type) is IOrionPlugin plugin)
            {
                logger.LogInformation("Plugin descubierto: {Plugin} ({Type}).", plugin.Metadata.Name, type.FullName);
                yield return plugin;
            }
        }
    }
}
