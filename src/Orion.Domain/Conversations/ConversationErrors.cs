using Orion.Shared.Results;

namespace Orion.Domain.Conversations;

public static class ConversationErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Conversations.NotFound", $"No existe una conversación con Id '{id}'.");
}
