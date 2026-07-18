namespace Orion.Application.Commands;

/// <summary>Resultado exitoso de un comando: un mensaje legible y datos opcionales.</summary>
public sealed record CommandOutcome(string Message, object? Data = null)
{
    public static CommandOutcome Ok(string message) => new(message);

    public static CommandOutcome Ok(string message, object data) => new(message, data);
}
