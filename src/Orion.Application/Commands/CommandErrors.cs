namespace Orion.Application.Commands;

/// <summary>Resultados de error estándar del motor de comandos.</summary>
public static class CommandErrors
{
    public static CommandResult Empty() =>
        CommandResult.Failed("No se indicó ningún comando.");

    public static CommandResult NotFound(string key) =>
        CommandResult.Failed($"No se reconoce el comando '{key}'.");

    public static CommandResult MissingArgument(string argument) =>
        CommandResult.Failed($"Falta el argumento requerido: {argument}.");
}
