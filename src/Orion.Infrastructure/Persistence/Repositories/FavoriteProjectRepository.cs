using Microsoft.EntityFrameworkCore;
using Orion.Domain.Projects;

namespace Orion.Infrastructure.Persistence.Repositories;

internal sealed class FavoriteProjectRepository(OrionDbContext db) : IFavoriteProjectRepository
{
    public Task<FavoriteProject?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.FavoriteProjects.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<IReadOnlyList<FavoriteProject>> GetForUserAsync(Guid userId, CancellationToken cancellationToken = default) =>
        await db.FavoriteProjects
            .Where(p => p.UserId == userId)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task AddAsync(FavoriteProject project, CancellationToken cancellationToken = default) =>
        await db.FavoriteProjects.AddAsync(project, cancellationToken).ConfigureAwait(false);

    public void Update(FavoriteProject project) => db.FavoriteProjects.Update(project);

    public void Remove(FavoriteProject project) => db.FavoriteProjects.Remove(project);
}
