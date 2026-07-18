using Orion.Shared.Modules;

namespace Orion.Memory;

internal sealed class MemoryStatusProvider : IModuleStatusProvider
{
    public ModuleStatusReport GetStatus() =>
        new("Memoria", ModuleStatus.Ready, "Persistencia activa (usuario, preferencias, historial, proyectos, rutas).");
}
