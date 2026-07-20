using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.Versioning;
using Orion.Shared.Results;
using Orion.Windows.Abstractions;
using Orion.Windows.Models;
using Orion.Windows.Native;

namespace Orion.Windows.Services;

/// <summary>
/// Métricas del sistema en tiempo real. Mantiene el estado del muestreo anterior
/// para calcular tasas de CPU y red entre llamadas consecutivas.
/// </summary>
[SupportedOSPlatform("windows")]
internal sealed class WindowsSystemMonitor : ISystemMonitor
{
    private const double BytesPerMb = 1024d * 1024d;
    private const double BytesPerGb = 1024d * 1024d * 1024d;

    private readonly Lock _sync = new();
    private ulong _lastIdle, _lastKernel, _lastUser, _lastNetBytes;
    private long _lastNetTimestamp;

    public Result<SystemLoad> GetLoad()
    {
        lock (_sync)
        {
            var cpu = SampleCpu();
            var (ramUsedMb, ramTotalMb) = SampleRam();
            var (diskUsedGb, diskTotalGb) = SampleDisk();
            var networkKbps = SampleNetwork();
            var processes = Process.GetProcesses().Length;

            return Result.Success(new SystemLoad(cpu, ramUsedMb, ramTotalMb, diskUsedGb, diskTotalGb, networkKbps, processes));
        }
    }

    private double SampleCpu()
    {
        if (!NativeMethods.GetSystemTimes(out var idle, out var kernel, out var user))
        {
            return 0;
        }

        var idleNow = idle.ToUInt64();
        var kernelNow = kernel.ToUInt64();
        var userNow = user.ToUInt64();

        double cpu = 0;
        if (_lastKernel != 0)
        {
            var idleDelta = idleNow - _lastIdle;
            var totalDelta = (kernelNow - _lastKernel) + (userNow - _lastUser);
            if (totalDelta > 0)
            {
                cpu = Math.Clamp((1.0 - (double)idleDelta / totalDelta) * 100.0, 0, 100);
            }
        }

        _lastIdle = idleNow;
        _lastKernel = kernelNow;
        _lastUser = userNow;
        return Math.Round(cpu, 1);
    }

    private static (double UsedMb, double TotalMb) SampleRam()
    {
        var status = new MemoryStatusEx { Length = (uint)System.Runtime.InteropServices.Marshal.SizeOf<MemoryStatusEx>() };
        if (!NativeMethods.GlobalMemoryStatusEx(ref status))
        {
            return (0, 0);
        }

        var totalMb = status.TotalPhys / BytesPerMb;
        var usedMb = (status.TotalPhys - status.AvailPhys) / BytesPerMb;
        return (Math.Round(usedMb, 0), Math.Round(totalMb, 0));
    }

    private static (double UsedGb, double TotalGb) SampleDisk()
    {
        try
        {
            var systemDrive = Path.GetPathRoot(Environment.SystemDirectory) ?? "C:\\";
            var drive = new DriveInfo(systemDrive);
            if (!drive.IsReady)
            {
                return (0, 0);
            }

            var totalGb = drive.TotalSize / BytesPerGb;
            var usedGb = (drive.TotalSize - drive.TotalFreeSpace) / BytesPerGb;
            return (Math.Round(usedGb, 1), Math.Round(totalGb, 1));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            return (0, 0);
        }
    }

    private double SampleNetwork()
    {
        ulong totalBytes = 0;
        foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (nic.OperationalStatus != OperationalStatus.Up || nic.NetworkInterfaceType == NetworkInterfaceType.Loopback)
            {
                continue;
            }

            var stats = nic.GetIPStatistics();
            totalBytes += (ulong)(stats.BytesReceived + stats.BytesSent);
        }

        var now = Stopwatch.GetTimestamp();
        double kbps = 0;
        if (_lastNetTimestamp != 0)
        {
            var seconds = Stopwatch.GetElapsedTime(_lastNetTimestamp, now).TotalSeconds;
            if (seconds > 0 && totalBytes >= _lastNetBytes)
            {
                kbps = (totalBytes - _lastNetBytes) / 1024d / seconds;
            }
        }

        _lastNetBytes = totalBytes;
        _lastNetTimestamp = now;
        return Math.Round(kbps, 1);
    }
}
