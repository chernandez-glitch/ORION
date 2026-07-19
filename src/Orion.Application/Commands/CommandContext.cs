namespace Orion.Application.Commands;

/// <summary>Implementación inmutable de <see cref="ICommandContext"/>.</summary>
public sealed class CommandContext(
    string commandKey,
    string rawInput,
    IReadOnlyDictionary<string, string?> parameters,
    Guid userId,
    string userName,
    CancellationToken cancellationToken) : ICommandContext
{
    public string CommandKey { get; } = commandKey;

    public string RawInput { get; } = rawInput;

    public IReadOnlyDictionary<string, string?> Parameters { get; } = parameters;

    public Guid UserId { get; } = userId;

    public string UserName { get; } = userName;

    public CancellationToken CancellationToken { get; } = cancellationToken;

    public string? GetParameter(string name) =>
        Parameters.TryGetValue(name, out var value) ? value : null;

    public bool HasParameter(string name) =>
        Parameters.TryGetValue(name, out var value) && !string.IsNullOrWhiteSpace(value);
}
