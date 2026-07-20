using System.Globalization;

namespace Orion.Windows.Models;

/// <summary>Detalle de un proceso del sistema.</summary>
public sealed record ProcessDetails(
    int Id,
    string Name,
    string? MainWindowTitle,
    long WorkingSetBytes,
    bool Responding)
{
    public double WorkingSetMb => Math.Round(WorkingSetBytes / (1024d * 1024d), 1);

    public string MemoryText => string.Create(CultureInfo.InvariantCulture, $"{WorkingSetMb:0} MB");
}

/// <summary>Detalle de una ventana de nivel superior.</summary>
public sealed record WindowDetails(long Handle, int ProcessId, string Title, bool IsVisible)
{
    public string ProcessText => string.Create(CultureInfo.InvariantCulture, $"PID {ProcessId}");
}

/// <summary>Resultado de ejecutar un shell (PowerShell/CMD).</summary>
public sealed record ShellResult(int ExitCode, string Output, string Error)
{
    public bool Succeeded => ExitCode == 0;
}

/// <summary>Información del sistema y del usuario.</summary>
public sealed record SystemInfo(string UserName, string MachineName, string OsVersion, string OsDescription);

/// <summary>Tarea programada.</summary>
public sealed record ScheduledTaskInfo(string Name, string Status, string NextRun);

/// <summary>Posición del cursor.</summary>
public sealed record MousePosition(int X, int Y);

/// <summary>Información de pantalla.</summary>
public sealed record ScreenInfo(int PrimaryWidth, int PrimaryHeight);

/// <summary>Carga del sistema para los widgets del dashboard.</summary>
public sealed record SystemLoad(
    double CpuPercent,
    double RamUsedMb,
    double RamTotalMb,
    double DiskUsedGb,
    double DiskTotalGb,
    double NetworkKbps,
    int ProcessCount)
{
    public double RamPercent => RamTotalMb <= 0 ? 0 : Math.Round(RamUsedMb / RamTotalMb * 100, 1);

    public double DiskPercent => DiskTotalGb <= 0 ? 0 : Math.Round(DiskUsedGb / DiskTotalGb * 100, 1);

    public static SystemLoad Empty { get; } = new(0, 0, 0, 0, 0, 0, 0);
}
