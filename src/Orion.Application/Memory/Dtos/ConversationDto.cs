using Orion.Domain.Conversations;

namespace Orion.Application.Memory.Dtos;

public sealed record ConversationDto(Guid Id, string Title, DateTime CreatedOnUtc);

public sealed record ConversationMessageDto(ConversationRole Role, string Content, DateTime TimestampUtc);
