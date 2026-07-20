using Orion.Memory.Engine.Entities;
using Orion.Shared.Results;

namespace Orion.Memory.Engine.Abstractions;

/// <summary>Gestiona favoritos (comandos, carpetas, aplicaciones, proyectos, conversaciones).</summary>
public interface IFavoriteService
{
    Task<Result<Favorite>> AddAsync(FavoriteKind kind, string reference, string label, CancellationToken cancellationToken = default);

    Task<Result> RemoveAsync(FavoriteKind kind, string reference, CancellationToken cancellationToken = default);

    Task<Result<bool>> IsFavoriteAsync(FavoriteKind kind, string reference, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<Favorite>>> GetAsync(FavoriteKind? kind = null, CancellationToken cancellationToken = default);
}
