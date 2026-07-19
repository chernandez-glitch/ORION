using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Orion.Application.Abstractions;

namespace Orion.Application.Commands;

/// <summary>
/// Pipeline de ejecución. Orden de etapas:
/// 1) Validación · 2) Autorización · 3) Logging · 4) Ejecución · 5) Resultado · 6) Historial.
/// Nunca lanza excepciones al llamador: las traduce a un <see cref="CommandResult"/>.
/// </summary>
public sealed class CommandPipeline(
    ICommandValidator validator,
    ICommandAuthorizer authorizer,
    ICommandHistory history,
    IClock clock,
    ILogger<CommandPipeline> logger) : ICommandPipeline
{
    public async Task<CommandResult> ExecuteAsync(CommandInfo command, ICommandHandler handler, ICommandContext context)
    {
        var startedUtc = clock.UtcNow;

        // 1) Validación
        var validation = validator.Validate(command, context);
        if (validation is not null)
        {
            await RecordAsync(command, context, validation, 0, startedUtc).ConfigureAwait(false);
            return validation;
        }

        // 2) Autorización
        var authorization = authorizer.Authorize(command, context);
        if (authorization is not null)
        {
            await RecordAsync(command, context, authorization, 0, startedUtc).ConfigureAwait(false);
            return authorization;
        }

        // 3) Logging (inicio)
        logger.LogInformation("Ejecutando comando {CommandId} ({CommandName}) por {User}.", command.Id, command.Name, context.UserName);

        // 4) Ejecución (medida y protegida)
        var stopwatch = Stopwatch.StartNew();
        CommandResult result;
        try
        {
            result = context.CancellationToken.IsCancellationRequested
                ? CommandResult.Cancelled()
                : await handler.ExecuteAsync(context).ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            result = CommandResult.Cancelled();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Excepción no controlada en el comando {CommandId}.", command.Id);
            result = CommandResult.Failed($"El comando '{command.Name}' falló: {ex.Message}");
        }
        finally
        {
            stopwatch.Stop();
        }

        // 5) Resultado (logging de cierre)
        logger.Log(
            result.Status == CommandStatus.Failed ? LogLevel.Warning : LogLevel.Information,
            "Comando {CommandId} → {Status} en {ElapsedMs} ms.",
            command.Id,
            result.Status,
            stopwatch.ElapsedMilliseconds);

        // 6) Historial
        await RecordAsync(command, context, result, stopwatch.ElapsedMilliseconds, startedUtc).ConfigureAwait(false);

        return result;
    }

    private async Task RecordAsync(CommandInfo command, ICommandContext context, CommandResult result, long durationMs, DateTime startedUtc)
    {
        var execution = new CommandExecution(
            Guid.CreateVersion7(),
            command.Id,
            command.Name,
            string.IsNullOrWhiteSpace(context.RawInput) ? command.Id : context.RawInput,
            context.UserId,
            context.UserName,
            result.Status,
            result.Message,
            result.Status == CommandStatus.Failed ? result.Message : null,
            startedUtc,
            durationMs);

        try
        {
            await history.RecordAsync(execution, context.CancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "No se pudo registrar el comando {CommandId} en el historial.", command.Id);
        }
    }
}
