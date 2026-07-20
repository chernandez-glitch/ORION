using Microsoft.Extensions.Logging;
using Orion.Application.Commands;
using Orion.Memory.Engine.Abstractions;

namespace Orion.Memory.Engine.Services;

/// <summary>
/// Implementación de <see cref="ICommandHistory"/> (Command Engine) que persiste
/// en el Memory Engine y CLASIFICA automáticamente aperturas de aplicaciones y
/// carpetas — integrando ambos engines por su costura, sin modificarlos.
/// </summary>
public sealed class MemoryEngineCommandHistory(
    IHistoryService history,
    IMemorySession session,
    ILogger<MemoryEngineCommandHistory> logger) : ICommandHistory
{
    public async Task RecordAsync(CommandExecution execution, CancellationToken cancellationToken = default)
    {
        await history.RecordCommandAsync(
            execution.CommandId,
            execution.CommandName,
            execution.ParametersText,
            execution.Status.ToString(),
            execution.Status == CommandStatus.Success,
            execution.DurationMs,
            session.CurrentSessionId,
            cancellationToken).ConfigureAwait(false);

        if (execution.Status == CommandStatus.Success)
        {
            await ClassifyAsync(execution, cancellationToken).ConfigureAwait(false);
        }

        logger.LogDebug("Comando '{CommandId}' registrado en la memoria permanente.", execution.CommandId);
    }

    public async Task<IReadOnlyList<CommandExecution>> GetRecentAsync(int take, CancellationToken cancellationToken = default)
    {
        var recent = await history.GetRecentCommandsAsync(take, cancellationToken).ConfigureAwait(false);
        if (recent.IsFailure)
        {
            return [];
        }

        return recent.Value
            .Select(c => new CommandExecution(
                c.Id, c.CommandId, c.CommandName, c.Parameters, Guid.Empty, "Usuario",
                c.Succeeded ? CommandStatus.Success : CommandStatus.Failed,
                c.Status, c.Succeeded ? null : c.Status, c.ExecutedOnUtc, c.DurationMs))
            .ToArray();
    }

    private async Task ClassifyAsync(CommandExecution execution, CancellationToken cancellationToken)
    {
        var argument = Argument(execution.ParametersText);

        switch (execution.CommandId)
        {
            case "apps.open" when argument.Length > 0:
                await history.RecordApplicationAsync(argument, null, cancellationToken).ConfigureAwait(false);
                break;
            case "folders.open" or "system.open-explorer" when argument.Length > 0:
                await history.RecordFolderAsync(argument, cancellationToken).ConfigureAwait(false);
                break;
            case "vscode.open":
                await history.RecordApplicationAsync("code", null, cancellationToken).ConfigureAwait(false);
                if (argument.Length > 0 && argument != ".")
                {
                    await history.RecordFolderAsync(argument, cancellationToken).ConfigureAwait(false);
                }

                break;
            case "internet.open-url" when argument.Length > 0:
                await history.RecordEventAsync("web", argument, cancellationToken).ConfigureAwait(false);
                break;
        }
    }

    private static string Argument(string parametersText)
    {
        var tokens = parametersText.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return tokens.Length <= 1 ? string.Empty : string.Join(' ', tokens.Skip(1)).Trim('"');
    }
}
