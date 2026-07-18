namespace Orion.Plugins;

internal sealed class PluginCatalog(IReadOnlyList<PluginMetadata> plugins) : IPluginCatalog
{
    public IReadOnlyList<PluginMetadata> Plugins { get; } = plugins;
}
