using Orion.Shared.Results;

namespace Orion.Application.Commands;

/// <summary>
/// Un comando ejecutable de ORION. Cada comando es una clase independiente que
/// se registra automáticamente en el motor (ver <c>AddOrionApplication</c>).
/// </summary>
public interface ICommand
{
    CommandDescriptor Descriptor { get; }

    Task<Result<CommandOutcome>> ExecuteAsync(CommandRequest request, CancellationToken cancellationToken = default);
}
