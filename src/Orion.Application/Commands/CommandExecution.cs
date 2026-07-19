namespace Orion.Application.Commands;

/// <summary>
/// Registro histórico de una ejecución de comando: qué, quién, con qué
/// parámetros, resultado, error, cuándo y cuánto tardó.
/// </summary>
public sealed record CommandExecution(
    Guid Id,
    string CommandId,
    string CommandName,
    string ParametersText,
    Guid UserId,
    string UserName,
    CommandStatus Status,
    string Message,
    string? Error,
    DateTime StartedOnUtc,
    long DurationMs);
