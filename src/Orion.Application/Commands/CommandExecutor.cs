using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orion.Application.Memory;

namespace Orion.Application.Commands;

/// <summary>
/// Implementación del executor. Crea un ámbito de DI por ejecución (para dar a
/// cada comando su unidad de trabajo/DbContext), resuelve el handler y el
/// pipeline, arma el contexto y delega en el pipeline.
/// </summary>
public sealed class CommandExecutor(
    IServiceScopeFactory scopeFactory,
    ICommandRegistry registry,
    ICommandParser parser,
    ILogger<CommandExecutor> logger) : ICommandExecutor
{
    private const string DefaultUserName = "Usuario";

    public Task<CommandResult> ExecuteAsync(string rawInput, CancellationToken cancellationToken = default)
    {
        var parsed = parser.Parse(rawInput, registry);
        return parsed.Success
            ? ExecuteAsync(parsed.CommandKey, parsed.Parameters, cancellationToken)
            : Task.FromResult(CommandResult.Failed(parsed.Error ?? "Entrada no reconocida."));
    }

    public async Task<CommandResult> ExecuteAsync(string commandKey, IReadOnlyDictionary<string, string?> parameters, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(commandKey))
        {
            return CommandErrors.Empty();
        }

        if (!registry.TryGet(commandKey, out var command))
        {
            return CommandErrors.NotFound(commandKey);
        }

        await using var scope = scopeFactory.CreateAsyncScope();
        var provider = scope.ServiceProvider;

        var handler = (ICommandHandler)provider.GetRequiredService(command.HandlerType);
        var pipeline = provider.GetRequiredService<ICommandPipeline>();
        var userId = await ResolveUserAsync(provider, cancellationToken).ConfigureAwait(false);

        var rawInput = parameters.Count == 0
            ? command.Id
            : $"{command.Id} {string.Join(' ', parameters.Values.Where(v => !string.IsNullOrWhiteSpace(v)))}".Trim();

        var context = new CommandContext(commandKey, rawInput, parameters, userId, DefaultUserName, cancellationToken);
        return await pipeline.ExecuteAsync(command, handler, context).ConfigureAwait(false);
    }

    private async Task<Guid> ResolveUserAsync(IServiceProvider provider, CancellationToken cancellationToken)
    {
        try
        {
            var memory = provider.GetRequiredService<IMemoryService>();
            var user = await memory.GetOrCreateActiveUserAsync(cancellationToken).ConfigureAwait(false);
            return user.IsSuccess ? user.Value : Guid.Empty;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "No se pudo resolver el usuario activo; se usa un id vacío.");
            return Guid.Empty;
        }
    }
}
