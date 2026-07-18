using Orion.Domain.Common;
using Orion.Shared.Guards;

namespace Orion.Domain.Conversations;

/// <summary>
/// Hilo de conversación entre el usuario y ORION. Agregado que controla el
/// ciclo de vida de sus <see cref="ConversationMessage"/>.
/// </summary>
public sealed class Conversation : AuditableEntity
{
    private readonly List<ConversationMessage> _messages = [];

    private Conversation(Guid id, Guid userId, string title, DateTime createdOnUtc)
        : base(id, createdOnUtc)
    {
        UserId = userId;
        Title = title;
    }

    public Guid UserId { get; private init; }

    public string Title { get; private set; }

    public IReadOnlyList<ConversationMessage> Messages => _messages.AsReadOnly();

    public static Conversation Start(Guid userId, string title, DateTime nowUtc)
    {
        Guard.AgainstEmpty(userId);
        Guard.AgainstNullOrWhiteSpace(title);

        return new Conversation(Guid.CreateVersion7(), userId, title.Trim(), nowUtc);
    }

    public ConversationMessage AddMessage(ConversationRole role, string content, DateTime nowUtc)
    {
        var message = ConversationMessage.Create(Id, role, content, nowUtc);
        _messages.Add(message);
        Touch(nowUtc);
        return message;
    }

    public void Rename(string title, DateTime nowUtc)
    {
        Title = Guard.AgainstNullOrWhiteSpace(title).Trim();
        Touch(nowUtc);
    }
}
