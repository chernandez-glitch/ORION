using Orion.Memory.Engine.Entities;
using Orion.Shared.Results;

namespace Orion.Memory.Engine.Abstractions;

/// <summary>Recuerda conversaciones y sus mensajes.</summary>
public interface IConversationService
{
    Task<Result<Conversation>> StartAsync(string title, Guid? sessionId = null, CancellationToken cancellationToken = default);

    Task<Result> AddMessageAsync(Guid conversationId, MemoryMessageRole role, string content, CancellationToken cancellationToken = default);

    Task<Result<Conversation>> GetAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<Conversation>>> GetRecentAsync(int take, CancellationToken cancellationToken = default);

    Task<Result<int>> CountAsync(CancellationToken cancellationToken = default);
}
