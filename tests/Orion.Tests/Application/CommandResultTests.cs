using FluentAssertions;
using Orion.Application.Commands;
using Xunit;

namespace Orion.Tests.Application;

public sealed class CommandResultTests
{
    [Fact]
    public void Success_ShouldHaveSuccessStatus()
    {
        var result = CommandResult.Success("ok", 42);

        result.Status.Should().Be(CommandStatus.Success);
        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("ok");
        result.Data.Should().Be(42);
    }

    [Theory]
    [InlineData(CommandStatus.Failed)]
    [InlineData(CommandStatus.Warning)]
    [InlineData(CommandStatus.Cancelled)]
    public void NonSuccess_ShouldNotBeSuccess(CommandStatus status)
    {
        CommandResult result = status switch
        {
            CommandStatus.Failed => CommandResult.Failed("x"),
            CommandStatus.Warning => CommandResult.Warning("x"),
            _ => CommandResult.Cancelled()
        };

        result.Status.Should().Be(status);
        result.IsSuccess.Should().BeFalse();
    }
}
