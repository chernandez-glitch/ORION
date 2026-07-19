namespace Orion.Application.Commands;

/// <summary>
/// Resultado uniforme que devuelve todo comando: estado, mensaje legible y
/// datos opcionales. Es el "Result Pattern" del motor de comandos.
/// </summary>
public sealed class CommandResult
{
    private CommandResult(CommandStatus status, string message, object? data)
    {
        Status = status;
        Message = message;
        Data = data;
    }

    public CommandStatus Status { get; }

    public string Message { get; }

    public object? Data { get; }

    public bool IsSuccess => Status is CommandStatus.Success;

    public static CommandResult Success(string message, object? data = null) =>
        new(CommandStatus.Success, message, data);

    public static CommandResult Warning(string message, object? data = null) =>
        new(CommandStatus.Warning, message, data);

    public static CommandResult Failed(string message, object? data = null) =>
        new(CommandStatus.Failed, message, data);

    public static CommandResult Cancelled(string message = "Comando cancelado.") =>
        new(CommandStatus.Cancelled, message, null);
}
