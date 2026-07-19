using System.Diagnostics.CodeAnalysis;
using Orion.Application.Commands;

namespace Orion.Tests.Application;

/// <summary>Registro de comandos en memoria para pruebas.</summary>
internal sealed class FakeCommandRegistry(params CommandInfo[] commands) : ICommandRegistry
{
    private readonly Dictionary<string, CommandInfo> _byKey = commands
        .SelectMany(c => c.Keys.Select(k => (k, c)))
        .ToDictionary(x => x.k, x => x.c, StringComparer.OrdinalIgnoreCase);

    public IReadOnlyList<CommandInfo> Commands { get; } = commands;

    public bool TryGet(string key, [NotNullWhen(true)] out CommandInfo? command) =>
        _byKey.TryGetValue(key, out command);

    public IReadOnlyList<CommandInfo> Search(string term) =>
        Commands.Where(c => c.Matches(term)).ToArray();
}

/// <summary>Handler de prueba que devuelve un resultado fijo o lanza una excepción.</summary>
internal sealed class FakeHandler(CommandResult? result = null, Exception? throws = null) : ICommandHandler
{
    public int Calls { get; private set; }

    public Task<CommandResult> ExecuteAsync(ICommandContext context)
    {
        Calls++;
        if (throws is not null)
        {
            throw throws;
        }

        return Task.FromResult(result ?? CommandResult.Success("ok"));
    }
}

internal static class TestCommands
{
    public static CommandInfo WithRequiredParam(string id = "test.cmd", string param = "value") =>
        new(id, "Comando de prueba", "desc", CommandCategory.System, CommandPermission.User,
            ["alias"], [new CommandParameter(param, "p", IsRequired: true)], typeof(FakeHandler));

    public static CommandInfo NoParams(string id = "test.noparam") =>
        new(id, "Sin parámetros", "desc", CommandCategory.System, CommandPermission.User,
            [], [], typeof(FakeHandler));
}
