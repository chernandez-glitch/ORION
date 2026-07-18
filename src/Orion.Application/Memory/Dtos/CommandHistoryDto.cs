namespace Orion.Application.Memory.Dtos;

public sealed record CommandHistoryDto(
    string CommandName,
    string RawInput,
    bool Succeeded,
    string Outcome,
    long DurationMs,
    DateTime ExecutedOnUtc);
