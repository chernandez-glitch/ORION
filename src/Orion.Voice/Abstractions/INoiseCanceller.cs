using Orion.Shared.Results;

namespace Orion.Voice.Abstractions;

/// <summary>Cancelación de ruido sobre un flujo de audio. Implementación en Fase 3.</summary>
public interface INoiseCanceller
{
    Task<Result<Stream>> ProcessAsync(Stream audio, CancellationToken cancellationToken = default);
}
