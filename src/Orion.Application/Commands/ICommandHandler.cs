namespace Orion.Application.Commands;

/// <summary>
/// Ejecuta la lógica de un comando. En este motor, cada comando es a la vez su
/// propio handler (una sola clase por comando), pero la interfaz permanece
/// separada para permitir handlers dedicados en el futuro.
/// </summary>
public interface ICommandHandler
{
    Task<CommandResult> ExecuteAsync(ICommandContext context);
}
