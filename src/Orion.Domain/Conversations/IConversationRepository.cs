namespace Orion.Domain.Conversations;

public interface IConversationRepository
{
    Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Conversation>> GetForUserAsync(Guid userId, int take, CancellationToken cancellationToken = default);

    Task AddAsync(Conversation conversation, CancellationToken cancellationToken = default);

    void Update(Conversation conversation);

    void Remove(Conversation conversation);
}
