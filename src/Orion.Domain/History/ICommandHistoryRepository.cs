namespace Orion.Domain.History;

public interface ICommandHistoryRepository
{
    Task<IReadOnlyList<CommandHistoryEntry>> GetRecentAsync(Guid userId, int take, CancellationToken cancellationToken = default);

    Task AddAsync(CommandHistoryEntry entry, CancellationToken cancellationToken = default);
}
