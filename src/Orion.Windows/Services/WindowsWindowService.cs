using System.Runtime.Versioning;
using System.Text;
using Microsoft.Extensions.Logging;
using Orion.Shared.Results;
using Orion.Windows.Abstractions;
using Orion.Windows.Models;
using Orion.Windows.Native;

namespace Orion.Windows.Services;

[SupportedOSPlatform("windows")]
internal sealed class WindowsWindowService(ILogger<WindowsWindowService> logger) : IWindowService
{
    public Result<IReadOnlyList<WindowDetails>> GetOpenWindows()
    {
        IReadOnlyList<WindowDetails> windows = Enumerate()
            .Where(w => w.IsVisible && !string.IsNullOrWhiteSpace(w.Title))
            .ToArray();

        return Result.Success(windows);
    }

    public Result<WindowDetails> Find(string title)
    {
        var match = Enumerate().FirstOrDefault(w =>
            w.IsVisible && w.Title.Contains(title, StringComparison.OrdinalIgnoreCase));

        return match is not null
            ? Result.Success(match)
            : Result.Failure<WindowDetails>(Error.NotFound("Window.NotFound", $"No se encontró una ventana '{title}'."));
    }

    public Result BringToFront(string title) => WithHandle(title, "traer al frente", h =>
    {
        NativeMethods.ShowWindow(h, NativeMethods.SW_RESTORE);
        NativeMethods.SetForegroundWindow(h);
    });

    public Result Minimize(string title) => WithHandle(title, "minimizar", h => NativeMethods.ShowWindow(h, NativeMethods.SW_SHOWMINIMIZED));

    public Result Maximize(string title) => WithHandle(title, "maximizar", h => NativeMethods.ShowWindow(h, NativeMethods.SW_MAXIMIZE));

    public Result Hide(string title) => WithHandle(title, "ocultar", h => NativeMethods.ShowWindow(h, NativeMethods.SW_HIDE));

    public Result Show(string title) => WithHandle(title, "mostrar", h => NativeMethods.ShowWindow(h, NativeMethods.SW_SHOW));

    public Result Close(string title) => WithHandle(title, "cerrar", h =>
        NativeMethods.PostMessage(h, NativeMethods.WM_CLOSE, IntPtr.Zero, IntPtr.Zero));

    public Result Resize(string title, int width, int height) => WithHandle(title, "redimensionar", h =>
    {
        if (NativeMethods.GetWindowRect(h, out var rect))
        {
            NativeMethods.MoveWindow(h, rect.Left, rect.Top, width, height, true);
        }
    });

    public Result Move(string title, int x, int y) => WithHandle(title, "mover", h =>
    {
        if (NativeMethods.GetWindowRect(h, out var rect))
        {
            NativeMethods.MoveWindow(h, x, y, rect.Right - rect.Left, rect.Bottom - rect.Top, true);
        }
    });

    private Result WithHandle(string title, string action, Action<IntPtr> operation)
    {
        var window = Find(title);
        if (window.IsFailure)
        {
            return Result.Failure(window.Error);
        }

        operation(new IntPtr(window.Value.Handle));
        logger.LogInformation("Ventana '{Title}': {Action}.", window.Value.Title, action);
        return Result.Success();
    }

    private static List<WindowDetails> Enumerate()
    {
        var windows = new List<WindowDetails>();

        NativeMethods.EnumWindows((hWnd, _) =>
        {
            var length = NativeMethods.GetWindowTextLength(hWnd);
            var title = string.Empty;
            if (length > 0)
            {
                var buffer = new StringBuilder(length + 1);
                NativeMethods.GetWindowText(hWnd, buffer, buffer.Capacity);
                title = buffer.ToString();
            }

            NativeMethods.GetWindowThreadProcessId(hWnd, out var pid);
            windows.Add(new WindowDetails(hWnd.ToInt64(), (int)pid, title, NativeMethods.IsWindowVisible(hWnd)));
            return true;
        }, IntPtr.Zero);

        return windows;
    }
}
