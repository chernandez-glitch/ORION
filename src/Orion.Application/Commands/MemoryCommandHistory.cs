using Orion.Application.Memory;

namespace Orion.Application.Commands;

/// <summary>
/// Implementación de <see cref="ICommandHistory"/> sobre el sistema de memoria
/// (persiste en SQLite vía <see cref="IMemoryService"/>).
/// </summary>
public sealed class MemoryCommandHistory(IMemoryService memory) : ICommandHistory
{
    public Task RecordAsync(CommandExecution execution, CancellationToken cancellationToken = default) =>
        memory.RecordCommandAsync(
            execution.CommandName,
            execution.ParametersText,
            execution.Status == CommandStatus.Success,
            execution.Error ?? execution.Message,
            execution.DurationMs,
            cancellationToken);

    public async Task<IReadOnlyList<CommandExecution>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
    {
        var result = await memory.GetRecentCommandsAsync(take, cancellationToken).ConfigureAwait(false);
        if (result.IsFailure)
        {
            return [];
        }

        return result.Value
            .Select(dto => new CommandExecution(
                Guid.Empty,
                dto.CommandName,
                dto.CommandName,
                dto.RawInput,
                Guid.Empty,
                "Usuario",
                dto.Succeeded ? CommandStatus.Success : CommandStatus.Failed,
                dto.Outcome,
                dto.Succeeded ? null : dto.Outcome,
                dto.ExecutedOnUtc,
                dto.DurationMs))
            .ToArray();
    }
}
