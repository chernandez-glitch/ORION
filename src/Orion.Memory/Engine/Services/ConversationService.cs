using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orion.Memory.Engine.Abstractions;
using Orion.Memory.Engine.Entities;
using Orion.Memory.Engine.Persistence;
using Orion.Shared.Results;

namespace Orion.Memory.Engine.Services;

internal sealed class ConversationService(MemoryDbContext db, ILogger<ConversationService> logger) : IConversationService
{
    public Task<Result<Conversation>> StartAsync(string title, Guid? sessionId = null, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "StartConversation", async () =>
        {
            var conversation = new Conversation
            {
                Title = string.IsNullOrWhiteSpace(title) ? "Conversación" : title.Trim(),
                UpdatedOnUtc = DateTime.UtcNow,
                SessionId = sessionId
            };

            await db.Conversations.AddAsync(conversation, cancellationToken).ConfigureAwait(false);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success(conversation);
        });

    public Task<Result> AddMessageAsync(Guid conversationId, MemoryMessageRole role, string content, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "AddMessage", async () =>
        {
            var conversation = await db.Conversations.FirstOrDefaultAsync(c => c.Id == conversationId, cancellationToken).ConfigureAwait(false);
            if (conversation is null)
            {
                return Result.Failure(Error.NotFound("Memory.ConversationNotFound", "Conversación no encontrada."));
            }

            db.ConversationMessages.Add(new ConversationMessage
            {
                ConversationId = conversationId,
                Role = role,
                Content = content ?? string.Empty,
                TimestampUtc = DateTime.UtcNow
            });
            conversation.UpdatedOnUtc = DateTime.UtcNow;

            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success();
        });

    public Task<Result<Conversation>> GetAsync(Guid id, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "GetConversation", async () =>
        {
            var conversation = await db.Conversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken)
                .ConfigureAwait(false);

            return conversation is null
                ? Result.Failure<Conversation>(Error.NotFound("Memory.ConversationNotFound", "Conversación no encontrada."))
                : Result.Success(conversation);
        });

    public Task<Result<IReadOnlyList<Conversation>>> GetRecentAsync(int take, CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "GetRecentConversations", async () =>
        {
            IReadOnlyList<Conversation> conversations = await db.Conversations
                .OrderByDescending(c => c.UpdatedOnUtc)
                .Take(take)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            return Result.Success(conversations);
        });

    public Task<Result<int>> CountAsync(CancellationToken cancellationToken = default) =>
        MemoryGuard.TryAsync(logger, "CountConversations", async () =>
            Result.Success(await db.Conversations.CountAsync(cancellationToken).ConfigureAwait(false)));
}
