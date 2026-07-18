namespace Orion.Plugins;

/// <summary>Catálogo de los plugins efectivamente cargados (para la UI/dashboard).</summary>
public interface IPluginCatalog
{
    IReadOnlyList<PluginMetadata> Plugins { get; }
}
