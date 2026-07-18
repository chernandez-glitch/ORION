namespace Orion.Application.Dashboard;

/// <summary>
/// Puerto para leer métricas de CPU y RAM. La implementación (basada en
/// contadores del sistema) vive en Infraestructura.
/// </summary>
public interface ISystemMetrics
{
    Task<SystemMetricsSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default);
}
