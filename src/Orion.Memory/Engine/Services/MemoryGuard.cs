using Microsoft.Extensions.Logging;
using Orion.Shared.Results;

namespace Orion.Memory.Engine.Services;

/// <summary>Envuelve operaciones de memoria: traduce excepciones de infraestructura a <see cref="Result"/>.</summary>
internal static class MemoryGuard
{
    public static async Task<Result<T>> TryAsync<T>(ILogger logger, string operation, Func<Task<Result<T>>> action)
    {
        try
        {
            return await action().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Operación de memoria '{Operation}' falló.", operation);
            return Result.Failure<T>(Error.Unexpected($"Memory.{operation}", ex.Message));
        }
    }

    public static async Task<Result> TryAsync(ILogger logger, string operation, Func<Task<Result>> action)
    {
        try
        {
            return await action().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Operación de memoria '{Operation}' falló.", operation);
            return Result.Failure(Error.Unexpected($"Memory.{operation}", ex.Message));
        }
    }
}
