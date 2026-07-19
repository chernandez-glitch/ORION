namespace Orion.Application.Commands;

/// <summary>Estado final uniforme de la ejecución de un comando.</summary>
public enum CommandStatus
{
    Success = 0,
    Warning = 1,
    Failed = 2,
    Cancelled = 3
}
