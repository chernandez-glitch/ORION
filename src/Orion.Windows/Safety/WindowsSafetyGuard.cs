using Orion.Shared.Results;

namespace Orion.Windows.Safety;

/// <summary>
/// Política de seguridad por defecto: impide borrar rutas raíz/de sistema y matar
/// procesos críticos de Windows. Es la última línea de defensa del SDK; la
/// confirmación al usuario la añade la capa de comandos/UI por encima.
/// </summary>
public sealed class WindowsSafetyGuard : ISafetyGuard
{
    private static readonly string[] CriticalProcesses =
    [
        "system", "wininit", "winlogon", "csrss", "smss", "services",
        "lsass", "svchost", "explorer", "dwm", "orion.presentation"
    ];

    public Result EnsureSafeToDelete(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return Result.Failure(Error.Validation("Safety.EmptyPath", "La ruta no puede estar vacía."));
        }

        string full;
        try
        {
            full = System.IO.Path.GetFullPath(path);
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or PathTooLongException)
        {
            return Result.Failure(Error.Validation("Safety.InvalidPath", $"Ruta inválida: {ex.Message}"));
        }

        var trimmed = full.TrimEnd(System.IO.Path.DirectorySeparatorChar);

        // Raíz de unidad (C:\) o rutas de sistema.
        if (trimmed.Length <= 2 || IsSystemPath(full))
        {
            return Result.Failure(Error.Forbidden("Safety.ProtectedPath", $"Operación bloqueada: '{full}' es una ruta protegida del sistema."));
        }

        return Result.Success();
    }

    public Result EnsureSafeToKill(string processName)
    {
        var normalized = (processName ?? string.Empty)
            .Replace(".exe", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Trim();

        if (string.IsNullOrEmpty(normalized))
        {
            return Result.Failure(Error.Validation("Safety.EmptyProcess", "El nombre de proceso no puede estar vacío."));
        }

        return CriticalProcesses.Contains(normalized, StringComparer.OrdinalIgnoreCase)
            ? Result.Failure(Error.Forbidden("Safety.CriticalProcess", $"Operación bloqueada: '{normalized}' es un proceso crítico del sistema."))
            : Result.Success();
    }

    private static bool IsSystemPath(string fullPath)
    {
        var windows = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
        var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        var programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

        return StartsWith(fullPath, windows) || Equals(fullPath, programFiles) || Equals(fullPath, programFilesX86);
    }

    private static bool StartsWith(string path, string prefix) =>
        !string.IsNullOrEmpty(prefix) && path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);

    private static bool Equals(string a, string b) =>
        !string.IsNullOrEmpty(b) && string.Equals(a.TrimEnd('\\'), b.TrimEnd('\\'), StringComparison.OrdinalIgnoreCase);
}
