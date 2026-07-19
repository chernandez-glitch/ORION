namespace Orion.Application.Commands;

/// <summary>
/// Ejecuta un comando pasándolo por todas las etapas: validación, autorización,
/// logging, ejecución, resultado e historial.
/// </summary>
public interface ICommandPipeline
{
    Task<CommandResult> ExecuteAsync(CommandInfo command, ICommandHandler handler, ICommandContext context);
}
