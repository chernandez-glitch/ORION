using System.Diagnostics.CodeAnalysis;

namespace Orion.Application.Commands;

/// <summary>
/// Índice de todos los comandos registrados. Resuelve el <see cref="Type"/> de
/// un comando por su nombre canónico o alias (sin distinguir mayúsculas) y
/// expone sus descriptores para la UI. No retiene instancias: cada ejecución
/// crea una nueva dentro de su propio ámbito de DI.
/// </summary>
public interface ICommandRegistry
{
    IReadOnlyList<CommandDescriptor> Descriptors { get; }

    bool TryGetCommandType(string nameOrAlias, [NotNullWhen(true)] out Type? commandType);
}
