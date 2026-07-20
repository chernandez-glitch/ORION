using Orion.Shared.Results;

namespace Orion.Memory.Engine.Abstractions;

/// <summary>
/// Provee la instantánea de contexto actual. Es el punto de entrada que la IA
/// usará (Fase 2) para saber en qué está trabajando el usuario.
/// </summary>
public interface IContextService
{
    Task<Result<MemoryContext>> GetCurrentContextAsync(CancellationToken cancellationToken = default);
}
