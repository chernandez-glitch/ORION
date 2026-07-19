namespace Orion.Application.Commands;

/// <summary>
/// Parser sintáctico: el primer token es la clave del comando y los siguientes
/// se mapean posicionalmente a sus parámetros declarados (el último parámetro
/// absorbe el resto, para admitir textos con espacios). Respeta comillas dobles.
/// </summary>
public sealed class DefaultCommandParser : ICommandParser
{
    public CommandParseResult Parse(string input, ICommandRegistry registry)
    {
        var tokens = CommandLineParser.Tokenize(input);
        if (tokens.Count == 0)
        {
            return CommandParseResult.Fail(input, "No se indicó ningún comando.");
        }

        var key = tokens[0];
        if (!registry.TryGet(key, out var command))
        {
            return CommandParseResult.Fail(input, $"No se reconoce el comando '{key}'.");
        }

        var args = tokens.Skip(1).ToArray();
        var parameters = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < command.Parameters.Count; i++)
        {
            var name = command.Parameters[i].Name;
            var isLast = i == command.Parameters.Count - 1;

            parameters[name] = isLast
                ? args.Length > i ? string.Join(' ', args.Skip(i)) : null
                : args.Length > i ? args[i] : null;
        }

        return CommandParseResult.Ok(command.Id, parameters, input);
    }
}
