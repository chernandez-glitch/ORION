using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Orion.Shared.Results;
using Orion.Windows.Abstractions;

namespace Orion.Windows.Services;

/// <summary>
/// Crea accesos directos (.lnk) mediante WScript.Shell (COM tardío por
/// reflexión, sin dependencias extra).
/// </summary>
[SupportedOSPlatform("windows")]
internal sealed class WindowsShortcutService(ILogger<WindowsShortcutService> logger) : IShortcutService
{
    public Result Create(string shortcutPath, string targetPath, string? arguments = null, string? description = null)
    {
        var shellType = Type.GetTypeFromProgID("WScript.Shell");
        if (shellType is null)
        {
            return Result.Failure(Error.Failure("Shortcut.NoWsh", "WScript.Shell no está disponible."));
        }

        try
        {
            var culture = CultureInfo.InvariantCulture;
            var shell = Activator.CreateInstance(shellType)!;
            var shortcut = shellType.InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, null, shell, [shortcutPath], culture)!;
            var type = shortcut.GetType();

            type.InvokeMember("TargetPath", BindingFlags.SetProperty, null, shortcut, [targetPath], culture);
            if (!string.IsNullOrWhiteSpace(arguments))
            {
                type.InvokeMember("Arguments", BindingFlags.SetProperty, null, shortcut, [arguments], culture);
            }

            if (!string.IsNullOrWhiteSpace(description))
            {
                type.InvokeMember("Description", BindingFlags.SetProperty, null, shortcut, [description], culture);
            }

            type.InvokeMember("Save", BindingFlags.InvokeMethod, null, shortcut, [], culture);
            logger.LogInformation("Acceso directo creado: {Path}", shortcutPath);
            return Result.Success();
        }
        catch (TargetInvocationException ex)
        {
            return Result.Failure(Error.Failure("Shortcut.CreateFailed", ex.InnerException?.Message ?? ex.Message));
        }
    }

    public Result Delete(string shortcutPath)
    {
        try
        {
            if (File.Exists(shortcutPath))
            {
                File.Delete(shortcutPath);
            }

            return Result.Success();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            return Result.Failure(Error.Failure("Shortcut.DeleteFailed", ex.Message));
        }
    }

    public Result Open(string shortcutPath)
    {
        if (!File.Exists(shortcutPath))
        {
            return Result.Failure(Error.NotFound("Shortcut.NotFound", $"No existe el acceso directo '{shortcutPath}'."));
        }

        try
        {
            Process.Start(new ProcessStartInfo { FileName = shortcutPath, UseShellExecute = true });
            return Result.Success();
        }
        catch (System.ComponentModel.Win32Exception ex)
        {
            return Result.Failure(Error.Failure("Shortcut.OpenFailed", ex.Message));
        }
    }
}
