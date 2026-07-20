using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orion.Memory.Engine.Abstractions;
using Orion.Memory.Engine.Entities;
using Orion.Memory.Engine.Persistence;
using Orion.Shared.Results;

namespace Orion.Memory.Engine.Services;

internal sealed class FavoriteService(MemoryDbContext db, ILogger<FavoriteService> logger) : IFavoriteService
{
    public Task<Result<Favorite>> AddAsync(FavoriteKind kind, string reference, string label, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "AddFavorite", async () =>
        {
            var existing = await db.Favorites.FirstOrDefaultAsync(f => f.Kind == kind && f.Reference == reference, cancellationToken).ConfigureAwait(false);
            if (existing is not null)
            {
                return Result.Success(existing);
            }

            var favorite = new Favorite { Kind = kind, Reference = reference, Label = string.IsNullOrWhiteSpace(label) ? reference : label };
            await db.Favorites.AddAsync(favorite, cancellationToken).ConfigureAwait(false);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success(favorite);
        });

    public Task<Result> RemoveAsync(FavoriteKind kind, string reference, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "RemoveFavorite", async () =>
        {
            var favorite = await db.Favorites.FirstOrDefaultAsync(f => f.Kind == kind && f.Reference == reference, cancellationToken).ConfigureAwait(false);
            if (favorite is not null)
            {
                db.Favorites.Remove(favorite);
                await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }

            return Result.Success();
        });

    public Task<Result<bool>> IsFavoriteAsync(FavoriteKind kind, string reference, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "IsFavorite", async () =>
            Result.Success(await db.Favorites.AnyAsync(f => f.Kind == kind && f.Reference == reference, cancellationToken).ConfigureAwait(false)));

    public Task<Result<IReadOnlyList<Favorite>>> GetAsync(FavoriteKind? kind = null, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "GetFavorites", async () =>
        {
            var query = db.Favorites.AsQueryable();
            if (kind is not null)
            {
                query = query.Where(f => f.Kind == kind);
            }

            IReadOnlyList<Favorite> favorites = await query.OrderByDescending(f => f.CreatedOnUtc).ToListAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success(favorites);
        });
}
