using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Orion.Application.Abstractions;
using Orion.Application.Commands;
using Xunit;

namespace Orion.Tests.Application;

public sealed class CommandPipelineTests
{
    private readonly ICommandValidator _validator = Substitute.For<ICommandValidator>();
    private readonly ICommandAuthorizer _authorizer = Substitute.For<ICommandAuthorizer>();
    private readonly ICommandHistory _history = Substitute.For<ICommandHistory>();
    private readonly IClock _clock = Substitute.For<IClock>();

    public CommandPipelineTests()
    {
        _clock.UtcNow.Returns(new DateTime(2026, 7, 19, 12, 0, 0, DateTimeKind.Utc));
        _validator.Validate(Arg.Any<CommandInfo>(), Arg.Any<ICommandContext>()).Returns((CommandResult?)null);
        _authorizer.Authorize(Arg.Any<CommandInfo>(), Arg.Any<ICommandContext>()).Returns((CommandResult?)null);
    }

    private CommandPipeline CreateSut() =>
        new(_validator, _authorizer, _history, _clock, NullLogger<CommandPipeline>.Instance);

    private static CommandContext Context() =>
        new("test", "test", new Dictionary<string, string?>(), Guid.NewGuid(), "Usuario", CancellationToken.None);

    [Fact]
    public async Task Execute_WhenValid_ShouldRunHandlerAndRecordHistory()
    {
        var handler = new FakeHandler(CommandResult.Success("hecho"));
        var sut = CreateSut();

        var result = await sut.ExecuteAsync(TestCommands.NoParams(), handler, Context());

        result.IsSuccess.Should().BeTrue();
        handler.Calls.Should().Be(1);
        await _history.Received(1).RecordAsync(Arg.Any<CommandExecution>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Execute_WhenValidationFails_ShouldNotRunHandler()
    {
        _validator.Validate(Arg.Any<CommandInfo>(), Arg.Any<ICommandContext>())
            .Returns(CommandResult.Failed("faltan parámetros"));
        var handler = new FakeHandler();
        var sut = CreateSut();

        var result = await sut.ExecuteAsync(TestCommands.WithRequiredParam(), handler, Context());

        result.Status.Should().Be(CommandStatus.Failed);
        handler.Calls.Should().Be(0);
        await _history.Received(1).RecordAsync(Arg.Any<CommandExecution>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Execute_WhenHandlerThrows_ShouldReturnFailedNotThrow()
    {
        var handler = new FakeHandler(throws: new InvalidOperationException("boom"));
        var sut = CreateSut();

        var result = await sut.ExecuteAsync(TestCommands.NoParams(), handler, Context());

        result.Status.Should().Be(CommandStatus.Failed);
        result.Message.Should().Contain("boom");
        await _history.Received(1).RecordAsync(Arg.Any<CommandExecution>(), Arg.Any<CancellationToken>());
    }
}
