using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Orion.Voice.Native;

/// <summary>P/Invoke a winmm para enumerar dispositivos de audio. Aislado aquí.</summary>
[SupportedOSPlatform("windows")]
internal static class VoiceNativeMethods
{
    [DllImport("winmm.dll")]
    internal static extern uint waveInGetNumDevs();

    [DllImport("winmm.dll", CharSet = CharSet.Unicode, EntryPoint = "waveInGetDevCapsW")]
    internal static extern uint waveInGetDevCaps(nuint deviceId, ref WaveInCaps caps, uint size);

    [DllImport("winmm.dll")]
    internal static extern uint waveOutGetNumDevs();

    [DllImport("winmm.dll", CharSet = CharSet.Unicode, EntryPoint = "waveOutGetDevCapsW")]
    internal static extern uint waveOutGetDevCaps(nuint deviceId, ref WaveOutCaps caps, uint size);
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct WaveInCaps
{
    public ushort ManufacturerId;
    public ushort ProductId;
    public uint DriverVersion;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string ProductName;

    public uint Formats;
    public ushort Channels;
    public ushort Reserved;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct WaveOutCaps
{
    public ushort ManufacturerId;
    public ushort ProductId;
    public uint DriverVersion;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    public string ProductName;

    public uint Formats;
    public ushort Channels;
    public ushort Reserved;
    public uint Support;
}
