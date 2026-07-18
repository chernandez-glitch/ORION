using Orion.Application.Memory.Dtos;
using Orion.Shared.Modules;

namespace Orion.Application.Dashboard;

/// <summary>Vista agregada que alimenta el dashboard en una sola lectura.</summary>
/// <param name="Modules">Estado de cada módulo (micrófono, IA, memoria, plugins...).</param>
/// <param name="Metrics">CPU y RAM actuales.</param>
/// <param name="RecentCommands">Últimos comandos ejecutados.</param>
/// <param name="RegisteredCommandCount">Número de comandos disponibles en el motor.</param>
public sealed record DashboardSnapshot(
    IReadOnlyList<ModuleStatusReport> Modules,
    SystemMetricsSnapshot Metrics,
    IReadOnlyList<CommandHistoryDto> RecentCommands,
    int RegisteredCommandCount);
