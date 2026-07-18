using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Orion.Application.Commands;

/// <summary>
/// Construye el índice de comandos a partir de todas las implementaciones de
/// <see cref="ICommand"/>. Al iniciarse, resuelve los comandos en un ámbito
/// temporal solo para leer sus descriptores y mapear token → tipo; no conserva
/// las instancias. Registrar un comando nuevo es tan simple como crear la clase.
/// </summary>
public sealed class CommandRegistry : ICommandRegistry
{
    private readonly Dictionary<string, Type> _byToken = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<CommandDescriptor> _descriptors = [];

    public CommandRegistry(IServiceScopeFactory scopeFactory, ILogger<CommandRegistry> logger)
    {
        using var scope = scopeFactory.CreateScope();

        foreach (var command in scope.ServiceProvider.GetServices<ICommand>())
        {
            _descriptors.Add(command.Descriptor);
            var type = command.GetType();

            foreach (var token in command.Descriptor.AllTokens)
            {
                if (_byToken.TryAdd(token, type))
                {
                    continue;
                }

                logger.LogWarning("El token de comando '{Token}' está duplicado; se ignora el de {Type}.", token, type.Name);
            }
        }

        logger.LogInformation("Motor de comandos: {Count} comandos registrados.", _descriptors.Count);
    }

    public IReadOnlyList<CommandDescriptor> Descriptors => _descriptors;

    public bool TryGetCommandType(string nameOrAlias, [NotNullWhen(true)] out Type? commandType) =>
        _byToken.TryGetValue(nameOrAlias, out commandType);
}
