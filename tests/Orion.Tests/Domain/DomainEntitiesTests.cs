using FluentAssertions;
using Orion.Domain.Conversations;
using Orion.Domain.History;
using Orion.Domain.Routes;
using Orion.Domain.Users;
using Xunit;

namespace Orion.Tests.Domain;

public sealed class DomainEntitiesTests
{
    private static readonly DateTime Now = new(2026, 7, 18, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void OrionUser_Create_ShouldTrimAndInitializeAudit()
    {
        var user = OrionUser.Create("  Carlos  ", "es-HN", Now);

        user.Id.Should().NotBe(Guid.Empty);
        user.DisplayName.Should().Be("Carlos");
        user.CreatedOnUtc.Should().Be(Now);
        user.UpdatedOnUtc.Should().Be(Now);
    }

    [Fact]
    public void OrionUser_Rename_ShouldTouchUpdatedOn()
    {
        var user = OrionUser.Create("A", "es", Now);
        var later = Now.AddHours(1);

        user.Rename("B", later);

        user.DisplayName.Should().Be("B");
        user.UpdatedOnUtc.Should().Be(later);
    }

    [Fact]
    public void FavoriteRoute_Create_ShouldRequireAliasAndPath()
    {
        var act = () => FavoriteRoute.Create(Guid.NewGuid(), " ", "C:\\x", Now);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Conversation_AddMessage_ShouldAppendAndTouch()
    {
        var conversation = Conversation.Start(Guid.NewGuid(), "Chat", Now);
        var later = Now.AddMinutes(5);

        conversation.AddMessage(ConversationRole.User, "hola", later);

        conversation.Messages.Should().HaveCount(1);
        conversation.Messages[0].Role.Should().Be(ConversationRole.User);
        conversation.UpdatedOnUtc.Should().Be(later);
    }

    [Fact]
    public void CommandHistoryEntry_Record_ShouldClampNegativeDuration()
    {
        var entry = CommandHistoryEntry.Record(Guid.NewGuid(), "apagar", "apagar", true, "ok", -50, Now);

        entry.DurationMs.Should().Be(0);
        entry.CommandName.Should().Be("apagar");
        entry.Succeeded.Should().BeTrue();
    }
}
