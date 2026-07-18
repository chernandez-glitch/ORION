using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Orion.Application.Abstractions;
using Orion.Domain.Automations;
using Orion.Domain.Common;
using Orion.Domain.Conversations;
using Orion.Domain.History;
using Orion.Domain.Preferences;
using Orion.Domain.Projects;
using Orion.Domain.Routes;
using Orion.Domain.Users;
using Orion.Memory;
using Xunit;

namespace Orion.Tests.Memory;

public sealed class MemoryServiceTests
{
    private static readonly DateTime Now = new(2026, 7, 18, 12, 0, 0, DateTimeKind.Utc);

    private readonly IUserRepository _users = Substitute.For<IUserRepository>();
    private readonly IPreferenceRepository _preferences = Substitute.For<IPreferenceRepository>();
    private readonly IFavoriteProjectRepository _projects = Substitute.For<IFavoriteProjectRepository>();
    private readonly IFavoriteRouteRepository _routes = Substitute.For<IFavoriteRouteRepository>();
    private readonly ICommandHistoryRepository _history = Substitute.For<ICommandHistoryRepository>();
    private readonly IConversationRepository _conversations = Substitute.For<IConversationRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ActiveUserContext _activeUser = new();
    private readonly IClock _clock = Substitute.For<IClock>();

    public MemoryServiceTests()
    {
        _clock.UtcNow.Returns(Now);
        _users.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<OrionUser>>([]));
    }

    private MemoryService CreateSut() => new(
        _users, _preferences, _projects, _routes, _history, _conversations,
        _unitOfWork, _activeUser, _clock, NullLogger<MemoryService>.Instance);

    [Fact]
    public async Task GetOrCreateActiveUser_WhenNoUsers_ShouldCreateDefaultAndPersist()
    {
        var sut = CreateSut();

        var result = await sut.GetOrCreateActiveUserAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBe(Guid.Empty);
        _activeUser.HasUser.Should().BeTrue();
        await _users.Received(1).AddAsync(Arg.Any<OrionUser>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetOrCreateActiveUser_WhenUserExists_ShouldReuseIt()
    {
        var existing = OrionUser.Create("Ana", "es", Now);
        _users.GetAllAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<IReadOnlyList<OrionUser>>([existing]));
        var sut = CreateSut();

        var result = await sut.GetOrCreateActiveUserAsync();

        result.Value.Should().Be(existing.Id);
        await _users.DidNotReceive().AddAsync(Arg.Any<OrionUser>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddRoute_WhenAliasExists_ShouldFailWithConflict()
    {
        _activeUser.SetUser(Guid.NewGuid());
        var existing = FavoriteRoute.Create(_activeUser.CurrentUserId, "descargas", "C:\\d", Now);
        _routes.GetByAliasAsync(_activeUser.CurrentUserId, "descargas", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<FavoriteRoute?>(existing));
        var sut = CreateSut();

        var result = await sut.AddRouteAsync("descargas", "C:\\otro");

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Routes.AliasTaken");
    }

    [Fact]
    public async Task SetPreference_WhenNew_ShouldAddAndSave()
    {
        _activeUser.SetUser(Guid.NewGuid());
        _preferences.GetAsync(_activeUser.CurrentUserId, "tema", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<Preference?>(null));
        var sut = CreateSut();

        var result = await sut.SetPreferenceAsync("tema", "oscuro");

        result.IsSuccess.Should().BeTrue();
        await _preferences.Received(1).AddAsync(Arg.Any<Preference>(), Arg.Any<CancellationToken>());
        await _unitOfWork.Received().SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task RecordCommand_ShouldPersistHistoryEntry()
    {
        _activeUser.SetUser(Guid.NewGuid());
        var sut = CreateSut();

        var result = await sut.RecordCommandAsync("apagar", "apagar", true, "ok", 12);

        result.IsSuccess.Should().BeTrue();
        await _history.Received(1).AddAsync(Arg.Any<CommandHistoryEntry>(), Arg.Any<CancellationToken>());
    }
}
