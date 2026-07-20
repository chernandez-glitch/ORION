using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Orion.Shared.Results;
using Orion.Voice.Engine.Abstractions;
using Orion.Voice.Native;

namespace Orion.Voice.Engine.Services;

/// <summary>Enumera dispositivos de audio reales vía winmm.</summary>
[SupportedOSPlatform("windows")]
internal sealed class AudioDeviceService(ILogger<AudioDeviceService> logger) : IAudioDeviceService
{
    public Result<IReadOnlyList<AudioDevice>> GetInputDevices()
    {
        try
        {
            var count = VoiceNativeMethods.waveInGetNumDevs();
            var devices = new List<AudioDevice>((int)count);
            for (uint i = 0; i < count; i++)
            {
                var caps = default(WaveInCaps);
                if (VoiceNativeMethods.waveInGetDevCaps((nuint)i, ref caps, (uint)Marshal.SizeOf<WaveInCaps>()) == 0)
                {
                    devices.Add(new AudioDevice((int)i, caps.ProductName, IsInput: true));
                }
            }

            return Result.Success<IReadOnlyList<AudioDevice>>(devices);
        }
        catch (Exception ex) when (ex is DllNotFoundException or EntryPointNotFoundException)
        {
            logger.LogWarning(ex, "No se pudieron enumerar los micrófonos.");
            return Result.Success<IReadOnlyList<AudioDevice>>([]);
        }
    }

    public Result<IReadOnlyList<AudioDevice>> GetOutputDevices()
    {
        try
        {
            var count = VoiceNativeMethods.waveOutGetNumDevs();
            var devices = new List<AudioDevice>((int)count);
            for (uint i = 0; i < count; i++)
            {
                var caps = default(WaveOutCaps);
                if (VoiceNativeMethods.waveOutGetDevCaps((nuint)i, ref caps, (uint)Marshal.SizeOf<WaveOutCaps>()) == 0)
                {
                    devices.Add(new AudioDevice((int)i, caps.ProductName, IsInput: false));
                }
            }

            return Result.Success<IReadOnlyList<AudioDevice>>(devices);
        }
        catch (Exception ex) when (ex is DllNotFoundException or EntryPointNotFoundException)
        {
            logger.LogWarning(ex, "No se pudieron enumerar los altavoces.");
            return Result.Success<IReadOnlyList<AudioDevice>>([]);
        }
    }
}
