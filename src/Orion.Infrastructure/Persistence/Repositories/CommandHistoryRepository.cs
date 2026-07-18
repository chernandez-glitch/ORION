using Microsoft.EntityFrameworkCore;
using Orion.Domain.History;

namespace Orion.Infrastructure.Persistence.Repositories;

internal sealed class CommandHistoryRepository(OrionDbContext db) : ICommandHistoryRepository
{
    public async Task<IReadOnlyList<CommandHistoryEntry>> GetRecentAsync(Guid userId, int take, CancellationToken cancellationToken = default) =>
        await db.CommandHistory
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.ExecutedOnUtc)
            .Take(take)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task AddAsync(CommandHistoryEntry entry, CancellationToken cancellationToken = default) =>
        await db.CommandHistory.AddAsync(entry, cancellationToken).ConfigureAwait(false);
}
