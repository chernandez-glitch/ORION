using Microsoft.EntityFrameworkCore;
using Orion.Domain.Conversations;

namespace Orion.Infrastructure.Persistence.Repositories;

internal sealed class ConversationRepository(OrionDbContext db) : IConversationRepository
{
    public Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        db.Conversations.Include(c => c.Messages).FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Conversation>> GetForUserAsync(Guid userId, int take, CancellationToken cancellationToken = default) =>
        await db.Conversations
            .Where(c => c.UserId == userId)
            .OrderByDescending(c => c.UpdatedOnUtc)
            .Take(take)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

    public async Task AddAsync(Conversation conversation, CancellationToken cancellationToken = default) =>
        await db.Conversations.AddAsync(conversation, cancellationToken).ConfigureAwait(false);

    public void Update(Conversation conversation) => db.Conversations.Update(conversation);

    public void Remove(Conversation conversation) => db.Conversations.Remove(conversation);
}
