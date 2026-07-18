using Orion.Domain.Common;
using Orion.Shared.Guards;

namespace Orion.Domain.History;

/// <summary>
/// Registro histórico de un comando ejecutado por ORION. Alimenta el panel de
/// "últimos comandos" del dashboard y el aprendizaje de hábitos.
/// </summary>
public sealed class CommandHistoryEntry : Entity
{
    private CommandHistoryEntry(
        Guid id,
        Guid userId,
        string commandName,
        string rawInput,
        bool succeeded,
        string outcome,
        long durationMs,
        DateTime executedOnUtc)
        : base(id)
    {
        UserId = userId;
        CommandName = commandName;
        RawInput = rawInput;
        Succeeded = succeeded;
        Outcome = outcome;
        DurationMs = durationMs;
        ExecutedOnUtc = executedOnUtc;
    }

    public Guid UserId { get; private init; }

    public string CommandName { get; private init; }

    public string RawInput { get; private init; }

    public bool Succeeded { get; private init; }

    public string Outcome { get; private init; }

    public long DurationMs { get; private init; }

    public DateTime ExecutedOnUtc { get; private init; }

    public static CommandHistoryEntry Record(
        Guid userId,
        string commandName,
        string rawInput,
        bool succeeded,
        string outcome,
        long durationMs,
        DateTime nowUtc)
    {
        Guard.AgainstEmpty(userId);
        Guard.AgainstNullOrWhiteSpace(commandName);

        return new CommandHistoryEntry(
            Guid.CreateVersion7(),
            userId,
            commandName.Trim(),
            rawInput ?? string.Empty,
            succeeded,
            outcome ?? string.Empty,
            durationMs < 0 ? 0 : durationMs,
            nowUtc);
    }
}
