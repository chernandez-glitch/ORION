namespace Orion.Shared.Modules;

/// <summary>
/// Implementado por cada módulo para reportar su estado al dashboard. Se
/// resuelven todos vía DI como una colección, de modo que agregar un módulo
/// nuevo no obliga a tocar el dashboard.
/// </summary>
public interface IModuleStatusProvider
{
    ModuleStatusReport GetStatus();
}
