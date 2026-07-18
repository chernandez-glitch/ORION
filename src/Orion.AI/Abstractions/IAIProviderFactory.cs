using Orion.Shared.Results;

namespace Orion.AI.Abstractions;

/// <summary>
/// Resuelve un <see cref="IAIProvider"/> por su <see cref="AIProviderKind"/>.
/// Permite cambiar de proveedor en caliente desde la configuración.
/// </summary>
public interface IAIProviderFactory
{
    Result<IAIProvider> Create(AIProviderKind kind);

    IReadOnlyList<AIProviderKind> AvailableProviders { get; }
}
