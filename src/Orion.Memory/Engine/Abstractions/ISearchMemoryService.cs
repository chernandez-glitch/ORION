using Orion.Shared.Results;

namespace Orion.Memory.Engine.Abstractions;

/// <summary>
/// Búsqueda rápida (por texto/proyecto/fecha/etiqueta/tipo/categoría) sobre toda
/// la memoria. Sin IA/embeddings todavía: filtrado estructurado sobre la base.
/// </summary>
public interface ISearchMemoryService
{
    Task<Result<IReadOnlyList<MemorySearchResult>>> SearchAsync(MemoryQuery query, CancellationToken cancellationToken = default);
}
