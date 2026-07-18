namespace Orion.Plugins;

/// <summary>Descubre e instancia los plugins presentes en el directorio de plugins.</summary>
public interface IPluginLoader
{
    /// <summary>Directorio desde el que se cargan los plugins.</summary>
    string PluginsDirectory { get; }

    IReadOnlyList<IOrionPlugin> Load();
}
