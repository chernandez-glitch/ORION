using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orion.Application.Memory;
using Orion.Shared.Results;

namespace Orion.Application.Commands;

/// <summary>
/// Implementación del motor de comandos. Crea un ámbito de DI por ejecución
/// (para dar a cada comando su propia unidad de trabajo/DbContext), parsea la
/// entrada, resuelve el comando, mide su duración, registra la ejecución en el
/// historial y devuelve el resultado sin lanzar excepciones.
/// </summary>
public sealed class CommandDispatcher(
    IServiceScopeFactory scopeFactory,
    ICommandRegistry registry,
    ILogger<CommandDispatcher> logger) : ICommandDispatcher
{
    public async Task<Result<CommandOutcome>> DispatchAsync(string rawInput, CancellationToken cancellationToken = default)
    {
        var tokens = CommandLineParser.Tokenize(rawInput);
        if (tokens.Count == 0)
        {
            return Result.Failure<CommandOutcome>(CommandErrors.Empty);
        }

        var name = tokens[0];

        await using var scope = scopeFactory.CreateAsyncScope();
        var memory = scope.ServiceProvider.GetRequiredService<IMemoryService>();

        if (!registry.TryGetCommandType(name, out var commandType))
        {
            var notFound = CommandErrors.NotFound(name);
            await RecordAsync(memory, name, rawInput, succeeded: false, notFound.Message, durationMs: 0, cancellationToken).ConfigureAwait(false);
            return Result.Failure<CommandOutcome>(notFound);
        }

        var command = (ICommand)scope.ServiceProvider.GetRequiredService(commandType);
        var request = new CommandRequest(name, tokens.Skip(1).ToArray(), rawInput);
        var stopwatch = Stopwatch.StartNew();
        Result<CommandOutcome> result;

        try
        {
            result = await command.ExecuteAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Excepción no controlada ejecutando el comando '{Command}'.", command.Descriptor.Name);
            result = Result.Failure<CommandOutcome>(CommandErrors.Unexpected(command.Descriptor.Name, ex.Message));
        }
        finally
        {
            stopwatch.Stop();
        }

        var outcome = result.IsSuccess ? result.Value.Message : result.Error.Message;
        await RecordAsync(memory, command.Descriptor.Name, rawInput, result.IsSuccess, outcome, stopwatch.ElapsedMilliseconds, cancellationToken)
            .ConfigureAwait(false);

        return result;
    }

    private async Task RecordAsync(
        IMemoryService memory,
        string commandName,
        string rawInput,
        bool succeeded,
        string outcome,
        long durationMs,
        CancellationToken cancellationToken)
    {
        var recorded = await memory
            .RecordCommandAsync(commandName, rawInput, succeeded, outcome, durationMs, cancellationToken)
            .ConfigureAwait(false);

        if (recorded.IsFailure)
        {
            logger.LogWarning("No se pudo registrar el comando '{Command}' en el historial: {Error}", commandName, recorded.Error);
        }
    }
}
