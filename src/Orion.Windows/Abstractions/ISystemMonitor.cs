using Orion.Shared.Results;
using Orion.Windows.Models;

namespace Orion.Windows.Abstractions;

/// <summary>
/// Métricas de carga del sistema en tiempo real (CPU, RAM, disco, red, procesos)
/// para los widgets del dashboard y el Automation Center.
/// </summary>
public interface ISystemMonitor
{
    Result<SystemLoad> GetLoad();
}
