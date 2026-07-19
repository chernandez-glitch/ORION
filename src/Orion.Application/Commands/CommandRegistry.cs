using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Orion.Application.Commands;

/// <summary>
/// Construye el índice de comandos a partir de todas las implementaciones de
/// <see cref="ICommand"/>. Al iniciarse resuelve los comandos en un ámbito
/// temporal solo para leer sus metadatos; no retiene las instancias.
/// </summary>
public sealed class CommandRegistry : ICommandRegistry
{
    private readonly Dictionary<string, CommandInfo> _byKey = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<CommandInfo> _commands = [];

    public CommandRegistry(IServiceScopeFactory scopeFactory, ILogger<CommandRegistry> logger)
    {
        using var scope = scopeFactory.CreateScope();

        foreach (var command in scope.ServiceProvider.GetServices<ICommand>())
        {
            var info = new CommandInfo(
                command.Id,
                command.Name,
                command.Description,
                command.Category,
                command.Permission,
                command.Aliases,
                command.Parameters,
                command.GetType());

            _commands.Add(info);

            foreach (var key in info.Keys)
            {
                if (!_byKey.TryAdd(key, info))
                {
                    logger.LogWarning("Clave de comando duplicada '{Key}'; se ignora la de {Type}.", key, info.HandlerType.Name);
                }
            }
        }

        _commands.Sort((a, b) =>
        {
            var byCategory = a.Category.CompareTo(b.Category);
            return byCategory != 0 ? byCategory : string.Compare(a.Name, b.Name, StringComparison.Ordinal);
        });

        logger.LogInformation("Motor de comandos: {Count} comandos registrados.", _commands.Count);
    }

    public IReadOnlyList<CommandInfo> Commands => _commands;

    public bool TryGet(string key, [NotNullWhen(true)] out CommandInfo? command) =>
        _byKey.TryGetValue(key, out command);

    public IReadOnlyList<CommandInfo> Search(string term) =>
        _commands.Where(c => c.Matches(term)).ToArray();
}
