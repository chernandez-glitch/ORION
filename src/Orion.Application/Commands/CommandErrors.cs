using Orion.Shared.Results;

namespace Orion.Application.Commands;

public static class CommandErrors
{
    public static readonly Error Empty =
        Error.Validation("Command.Empty", "No se indicó ningún comando.");

    public static Error NotFound(string name) =>
        Error.NotFound("Command.NotFound", $"No se reconoce el comando '{name}'.");

    public static Error MissingArgument(string argument) =>
        Error.Validation("Command.MissingArgument", $"Falta el argumento requerido: {argument}.");

    public static Error Unexpected(string commandName, string detail) =>
        Error.Unexpected("Command.Unexpected", $"El comando '{commandName}' falló inesperadamente: {detail}");
}
