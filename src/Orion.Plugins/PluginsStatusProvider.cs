using Orion.Shared.Modules;

namespace Orion.Plugins;

internal sealed class PluginsStatusProvider(IPluginCatalog catalog) : IModuleStatusProvider
{
    public ModuleStatusReport GetStatus()
    {
        var count = catalog.Plugins.Count;
        var detail = count == 0
            ? "Sistema de plugins activo. 0 plugins cargados."
            : $"{count} plugin(s) cargado(s).";

        return new ModuleStatusReport("Plugins", ModuleStatus.Ready, detail);
    }
}
