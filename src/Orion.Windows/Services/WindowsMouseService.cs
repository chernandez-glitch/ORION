using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Orion.Shared.Results;
using Orion.Windows.Abstractions;
using Orion.Windows.Models;
using Orion.Windows.Native;

namespace Orion.Windows.Services;

[SupportedOSPlatform("windows")]
internal sealed class WindowsMouseService(ILogger<WindowsMouseService> logger) : IMouseService
{
    public Result MoveTo(int x, int y)
    {
        NativeMethods.SetCursorPos(x, y);
        logger.LogInformation("Mouse movido a ({X}, {Y}).", x, y);
        return Result.Success();
    }

    public Result Click() =>
        Send([Mouse(NativeMethods.MOUSEEVENTF_LEFTDOWN), Mouse(NativeMethods.MOUSEEVENTF_LEFTUP)], "click izquierdo");

    public Result RightClick() =>
        Send([Mouse(NativeMethods.MOUSEEVENTF_RIGHTDOWN), Mouse(NativeMethods.MOUSEEVENTF_RIGHTUP)], "click derecho");

    public Result DoubleClick() =>
        Send(
        [
            Mouse(NativeMethods.MOUSEEVENTF_LEFTDOWN), Mouse(NativeMethods.MOUSEEVENTF_LEFTUP),
            Mouse(NativeMethods.MOUSEEVENTF_LEFTDOWN), Mouse(NativeMethods.MOUSEEVENTF_LEFTUP)
        ], "doble click");

    public Result Scroll(int amount) =>
        Send([Mouse(NativeMethods.MOUSEEVENTF_WHEEL, unchecked((uint)(amount * 120)))], $"scroll {amount}");

    public Result<MousePosition> GetPosition() =>
        NativeMethods.GetCursorPos(out var point)
            ? Result.Success(new MousePosition(point.X, point.Y))
            : Result.Failure<MousePosition>(Error.Failure("Mouse.PositionFailed", "No se pudo obtener la posición del cursor."));

    private Result Send(Input[] inputs, string action)
    {
        var sent = NativeMethods.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf<Input>());
        if (sent == 0)
        {
            return Result.Failure(Error.Failure("Mouse.SendFailed", "SendInput no envió ninguna entrada."));
        }

        logger.LogInformation("Mouse: {Action}.", action);
        return Result.Success();
    }

    private static Input Mouse(uint flags, uint data = 0) => new()
    {
        Type = NativeMethods.INPUT_MOUSE,
        U = new InputUnion { Mouse = new MouseInput { Flags = flags, MouseData = data } }
    };
}
