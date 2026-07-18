using Orion.Shared.Results;

namespace Orion.Application.Dashboard;

/// <summary>Compone la instantánea del dashboard a partir de todos los módulos.</summary>
public interface IDashboardService
{
    Task<Result<DashboardSnapshot>> GetSnapshotAsync(CancellationToken cancellationToken = default);
}
