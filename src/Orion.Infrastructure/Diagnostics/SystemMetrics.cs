using System.Diagnostics;
using Orion.Application.Abstractions;
using Orion.Application.Dashboard;

namespace Orion.Infrastructure.Diagnostics;

/// <summary>
/// Métricas de recursos basadas en el proceso de ORION. El uso de CPU se calcula
/// como el incremento de tiempo de procesador entre dos lecturas consecutivas
/// (normalizado por el número de núcleos), y la RAM como el working set del
/// proceso frente a la memoria disponible. Es portable y no requiere APIs de
/// Windows ni contadores de rendimiento.
/// </summary>
public sealed class SystemMetrics(IClock clock) : ISystemMetrics
{
    private const double BytesPerMb = 1024d * 1024d;

    private readonly Process _process = Process.GetCurrentProcess();
    private readonly Lock _sync = new();

    private DateTime _lastSampleUtc;
    private TimeSpan _lastCpuTime;

    public Task<SystemMetricsSnapshot> GetSnapshotAsync(CancellationToken cancellationToken = default)
    {
        double cpuPercent;

        lock (_sync)
        {
            _process.Refresh();
            var now = clock.UtcNow;
            var cpu = _process.TotalProcessorTime;

            if (_lastSampleUtc == default)
            {
                cpuPercent = 0;
            }
            else
            {
                var wallMs = (now - _lastSampleUtc).TotalMilliseconds;
                var cpuMs = (cpu - _lastCpuTime).TotalMilliseconds;
                cpuPercent = wallMs > 0
                    ? Math.Clamp(cpuMs / (wallMs * Environment.ProcessorCount) * 100d, 0, 100)
                    : 0;
            }

            _lastSampleUtc = now;
            _lastCpuTime = cpu;
        }

        var usedMb = Math.Round(_process.WorkingSet64 / BytesPerMb, 1);
        var totalMb = Math.Round(GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / BytesPerMb, 1);

        var snapshot = new SystemMetricsSnapshot(Math.Round(cpuPercent, 1), usedMb, totalMb);
        return Task.FromResult(snapshot);
    }
}
