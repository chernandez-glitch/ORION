using Orion.Shared.Results;

namespace Orion.Windows.Abstractions;

/// <summary>Crea, abre y elimina accesos directos (.lnk).</summary>
public interface IShortcutService
{
    Result Create(string shortcutPath, string targetPath, string? arguments = null, string? description = null);

    Result Delete(string shortcutPath);

    Result Open(string shortcutPath);
}
