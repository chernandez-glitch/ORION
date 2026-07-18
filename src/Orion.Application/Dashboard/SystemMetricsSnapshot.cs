namespace Orion.Application.Dashboard;

/// <summary>Instantánea de uso de recursos del equipo para el dashboard.</summary>
/// <param name="CpuPercent">Uso de CPU del proceso/equipo, 0–100.</param>
/// <param name="RamUsedMb">Memoria en uso (MB).</param>
/// <param name="RamTotalMb">Memoria total disponible (MB).</param>
public sealed record SystemMetricsSnapshot(double CpuPercent, double RamUsedMb, double RamTotalMb)
{
    public double RamPercent => RamTotalMb <= 0 ? 0 : Math.Round(RamUsedMb / RamTotalMb * 100, 1);

    public static SystemMetricsSnapshot Empty { get; } = new(0, 0, 0);
}
