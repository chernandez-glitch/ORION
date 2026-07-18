namespace Orion.Shared.Results;

/// <summary>
/// Representa un error de negocio de forma explícita, evitando el uso de
/// excepciones para el flujo de control. Cada error tiene un código estable
/// (apto para i18n y telemetría), un mensaje legible y un <see cref="ErrorType"/>.
/// </summary>
public sealed record Error
{
    /// <summary>Error nulo: ausencia de error. Usado por resultados exitosos.</summary>
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);

    private Error(string code, string message, ErrorType type)
    {
        Code = code;
        Message = message;
        Type = type;
    }

    public string Code { get; }

    public string Message { get; }

    public ErrorType Type { get; }

    public static Error Failure(string code, string message) => new(code, message, ErrorType.Failure);

    public static Error Validation(string code, string message) => new(code, message, ErrorType.Validation);

    public static Error NotFound(string code, string message) => new(code, message, ErrorType.NotFound);

    public static Error Conflict(string code, string message) => new(code, message, ErrorType.Conflict);

    public static Error Unauthorized(string code, string message) => new(code, message, ErrorType.Unauthorized);

    public static Error Forbidden(string code, string message) => new(code, message, ErrorType.Forbidden);

    public static Error Unexpected(string code, string message) => new(code, message, ErrorType.Unexpected);

    public override string ToString() => string.IsNullOrEmpty(Code) ? Message : $"{Code}: {Message}";
}
