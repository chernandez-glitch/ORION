using Microsoft.EntityFrameworkCore;
using Orion.Domain.Routes;

namespace Orion.Infrastructure.Persistence.Repositories;

internal sealed class FavoriteRouteRepository(OrionDbContext db) : IFavoriteRouteRepository
{
    public Task<FavoriteRoute?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.FavoriteRoutes.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

    public Task<FavoriteRoute?> GetByAliasAsync(Guid userId, string alias, CancellationToken cancellationToken = default) =>
        db.FavoriteRoutes.FirstOrDefaultAsync(r => r.UserId == userId && r.Alias == alias, cancellationToken);

    public async Task<IReadOnlyList<FavoriteRoute>> GetForUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await db.FavoriteRoutes
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.Alias)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task AddAsync(FavoriteRoute route, CancellationToken cancellationToken = default) =>
        await db.FavoriteRoutes.AddAsync(route, cancellationToken).ConfigureAwait(false);

    public void Update(FavoriteRoute route) => db.FavoriteRoutes.Update(route);

    public void Remove(FavoriteRoute route) => db.FavoriteRoutes.Remove(route);
}
