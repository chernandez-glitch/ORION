using Orion.Domain.Common;
using Orion.Shared.Guards;

namespace Orion.Domain.Conversations;

/// <summary>
/// Mensaje individual dentro de una <see cref="Conversation"/>. Entidad hija:
/// se crea siempre a través de su conversación contenedora.
/// </summary>
public sealed class ConversationMessage : Entity
{
    private ConversationMessage(Guid id, Guid conversationId, ConversationRole role, string content, DateTime timestampUtc)
        : base(id)
    {
        ConversationId = conversationId;
        Role = role;
        Content = content;
        TimestampUtc = timestampUtc;
    }

    public Guid ConversationId { get; private init; }

    public ConversationRole Role { get; private init; }

    public string Content { get; private init; }

    public DateTime TimestampUtc { get; private init; }

    internal static ConversationMessage Create(Guid conversationId, ConversationRole role, string content, DateTime nowUtc)
    {
        Guard.AgainstEmpty(conversationId);
        return new ConversationMessage(Guid.CreateVersion7(), conversationId, role, content ?? string.Empty, nowUtc);
    }
}
