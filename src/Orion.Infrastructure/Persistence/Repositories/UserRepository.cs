using Microsoft.EntityFrameworkCore;
using Orion.Domain.Users;

namespace Orion.Infrastructure.Persistence.Repositories;

internal sealed class UserRepository(OrionDbContext db) : IUserRepository
{
    public Task<OrionUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<OrionUser?> GetByDisplayNameAsync(string displayName, CancellationToken cancellationToken = default) =>
        db.Users.FirstOrDefaultAsync(u => u.DisplayName == displayName, cancellationToken);

    public async Task<IReadOnlyList<OrionUser>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await db.Users.OrderBy(u => u.CreatedOnUtc).ToListAsync(cancellationToken).ConfigureAwait(false);

    public async Task AddAsync(OrionUser user, CancellationToken cancellationToken = default) =>
        await db.Users.AddAsync(user, cancellationToken).ConfigureAwait(false);

    public void Update(OrionUser user) => db.Users.Update(user);

    public void Remove(OrionUser user) => db.Users.Remove(user);
}
