using Orion.Shared.Modules;

namespace Orion.Automation;

internal sealed class AutomationStatusProvider : IModuleStatusProvider
{
    public ModuleStatusReport GetStatus() =>
        new("Automatización", ModuleStatus.Disabled, "Puertos diseñados. Adaptadores Windows llegan en Fase 1.");
}
