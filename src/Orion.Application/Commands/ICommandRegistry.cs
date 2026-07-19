using System.Diagnostics.CodeAnalysis;

namespace Orion.Application.Commands;

/// <summary>
/// Índice de todos los comandos descubiertos. Resuelve por Id o alias y permite
/// buscar por nombre, categoría, descripción y alias (para la Command Palette).
/// </summary>
public interface ICommandRegistry
{
    IReadOnlyList<CommandInfo> Commands { get; }

    bool TryGet(string key, [NotNullWhen(true)] out CommandInfo? command);

    IReadOnlyList<CommandInfo> Search(string term);
}
