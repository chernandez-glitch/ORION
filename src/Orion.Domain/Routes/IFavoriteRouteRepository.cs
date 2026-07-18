namespace Orion.Domain.Routes;

public interface IFavoriteRouteRepository
{
    Task<FavoriteRoute?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<FavoriteRoute?> GetByAliasAsync(Guid userId, string alias, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<FavoriteRoute>> GetForUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(FavoriteRoute route, CancellationToken cancellationToken = default);

    void Update(FavoriteRoute route);

    void Remove(FavoriteRoute route);
}
