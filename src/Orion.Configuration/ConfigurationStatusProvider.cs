using Orion.Shared.Modules;

namespace Orion.Configuration;

internal sealed class ConfigurationStatusProvider : IModuleStatusProvider
{
    public ModuleStatusReport GetStatus() =>
        new("Configuración", ModuleStatus.Ready, "Ajustes persistidos en JSON.");
}
