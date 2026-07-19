namespace Orion.Application.Commands;

/// <summary>Almacena y consulta el historial de comandos ejecutados.</summary>
public interface ICommandHistory
{
    Task RecordAsync(CommandExecution execution, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CommandExecution>> GetRecentAsync(int take, CancellationToken cancellationToken = default);
}
