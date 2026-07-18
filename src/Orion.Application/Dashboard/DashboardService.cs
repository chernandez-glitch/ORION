using Microsoft.Extensions.DependencyInjection;
using Orion.Application.Commands;
using Orion.Application.Memory;
using Orion.Application.Memory.Dtos;
using Orion.Shared.Modules;
using Orion.Shared.Results;

namespace Orion.Application.Dashboard;

/// <summary>
/// Reúne el estado de todos los módulos, las métricas del sistema y los últimos
/// comandos. Es agnóstico de la UI: devuelve datos, no widgets. Usa un ámbito de
/// DI por lectura para acceder a la memoria (DbContext con vida de ámbito).
/// </summary>
public sealed class DashboardService(
    IEnumerable<IModuleStatusProvider> statusProviders,
    ISystemMetrics systemMetrics,
    IServiceScopeFactory scopeFactory,
    ICommandRegistry commandRegistry) : IDashboardService
{
    private const int RecentCommandsToShow = 10;

    public async Task<Result<DashboardSnapshot>> GetSnapshotAsync(CancellationToken cancellationToken = default)
    {
        var modules = statusProviders
            .Select(p => p.GetStatus())
            .OrderBy(r => r.Module, StringComparer.CurrentCulture)
            .ToArray();

        var metrics = await systemMetrics.GetSnapshotAsync(cancellationToken).ConfigureAwait(false);

        await using var scope = scopeFactory.CreateAsyncScope();
        var memory = scope.ServiceProvider.GetRequiredService<IMemoryService>();
        var recentResult = await memory.GetRecentCommandsAsync(RecentCommandsToShow, cancellationToken).ConfigureAwait(false);
        IReadOnlyList<CommandHistoryDto> recent = recentResult.IsSuccess ? recentResult.Value : [];

        var snapshot = new DashboardSnapshot(modules, metrics, recent, commandRegistry.Descriptors.Count);
        return Result.Success(snapshot);
    }
}
