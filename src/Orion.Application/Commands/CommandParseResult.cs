namespace Orion.Application.Commands;

/// <summary>Resultado de interpretar una entrada como comando + parámetros.</summary>
public sealed record CommandParseResult(
    bool Success,
    string CommandKey,
    IReadOnlyDictionary<string, string?> Parameters,
    string RawInput,
    string? Error)
{
    public static CommandParseResult Ok(string key, IReadOnlyDictionary<string, string?> parameters, string rawInput) =>
        new(true, key, parameters, rawInput, null);

    public static CommandParseResult Fail(string rawInput, string error) =>
        new(false, string.Empty, EmptyParameters, rawInput, error);

    private static readonly IReadOnlyDictionary<string, string?> EmptyParameters =
        new Dictionary<string, string?>();
}
