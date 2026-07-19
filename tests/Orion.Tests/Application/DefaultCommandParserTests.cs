using FluentAssertions;
using Orion.Application.Commands;
using Xunit;

namespace Orion.Tests.Application;

public sealed class DefaultCommandParserTests
{
    private readonly DefaultCommandParser _parser = new();

    [Fact]
    public void Parse_ShouldResolveCommandAndMapSingleParam()
    {
        var registry = new FakeCommandRegistry(TestCommands.WithRequiredParam("apps.open", "app"));

        var result = _parser.Parse("alias notepad", registry);

        result.Success.Should().BeTrue();
        result.CommandKey.Should().Be("apps.open");
        result.Parameters["app"].Should().Be("notepad");
    }

    [Fact]
    public void Parse_ShouldLetLastParamAbsorbRemainingTokens()
    {
        var command = new CommandInfo("system.show-message", "Msg", "d", CommandCategory.System,
            CommandPermission.User, ["msg"],
            [new CommandParameter("title", "t", true), new CommandParameter("message", "m", true)],
            typeof(FakeHandler));
        var registry = new FakeCommandRegistry(command);

        var result = _parser.Parse("msg Hola mundo cruel", registry);

        result.Parameters["title"].Should().Be("Hola");
        result.Parameters["message"].Should().Be("mundo cruel");
    }

    [Fact]
    public void Parse_UnknownCommand_ShouldFail()
    {
        var registry = new FakeCommandRegistry();

        var result = _parser.Parse("inexistente", registry);

        result.Success.Should().BeFalse();
        result.Error.Should().NotBeNull();
    }

    [Fact]
    public void Parse_EmptyInput_ShouldFail()
    {
        _parser.Parse("   ", new FakeCommandRegistry()).Success.Should().BeFalse();
    }
}
