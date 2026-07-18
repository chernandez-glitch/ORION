using Orion.Shared.Results;

namespace Orion.Application.Commands;

/// <summary>
/// Punto de entrada del motor de comandos: recibe la línea de entrada del
/// usuario, resuelve el comando, lo ejecuta y registra el resultado en memoria.
/// </summary>
public interface ICommandDispatcher
{
    Task<Result<CommandOutcome>> DispatchAsync(string rawInput, CancellationToken cancellationToken = default);
}
