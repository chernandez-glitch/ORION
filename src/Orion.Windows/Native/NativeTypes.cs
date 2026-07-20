using System.Runtime.InteropServices;

namespace Orion.Windows.Native;

[StructLayout(LayoutKind.Sequential)]
internal struct Rect
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
}

[StructLayout(LayoutKind.Sequential)]
internal struct Point
{
    public int X;
    public int Y;
}

[StructLayout(LayoutKind.Sequential)]
internal struct Input
{
    public uint Type;
    public InputUnion U;
}

[StructLayout(LayoutKind.Explicit)]
internal struct InputUnion
{
    [FieldOffset(0)] public MouseInput Mouse;
    [FieldOffset(0)] public KeyboardInput Keyboard;
    [FieldOffset(0)] public HardwareInput Hardware;
}

[StructLayout(LayoutKind.Sequential)]
internal struct MouseInput
{
    public int Dx;
    public int Dy;
    public uint MouseData;
    public uint Flags;
    public uint Time;
    public IntPtr ExtraInfo;
}

[StructLayout(LayoutKind.Sequential)]
internal struct KeyboardInput
{
    public ushort VirtualKey;
    public ushort ScanCode;
    public uint Flags;
    public uint Time;
    public IntPtr ExtraInfo;
}

[StructLayout(LayoutKind.Sequential)]
internal struct HardwareInput
{
    public uint Msg;
    public ushort ParamL;
    public ushort ParamH;
}

[StructLayout(LayoutKind.Sequential)]
internal struct MemoryStatusEx
{
    public uint Length;
    public uint MemoryLoad;
    public ulong TotalPhys;
    public ulong AvailPhys;
    public ulong TotalPageFile;
    public ulong AvailPageFile;
    public ulong TotalVirtual;
    public ulong AvailVirtual;
    public ulong AvailExtendedVirtual;
}

[StructLayout(LayoutKind.Sequential)]
internal struct DropFiles
{
    public uint PFiles;
    public int PtX;
    public int PtY;
    public int FNc;
    public int FWide;
}

[StructLayout(LayoutKind.Sequential)]
internal struct FileTimeStruct
{
    public uint Low;
    public uint High;

    public readonly ulong ToUInt64() => ((ulong)High << 32) | Low;
}
