using FluentAssertions;
using Orion.Application.Commands;
using Xunit;

namespace Orion.Tests.Application;

public sealed class CommandValidatorTests
{
    private readonly CommandValidator _validator = new();

    private static CommandContext Context(params (string Key, string? Value)[] parameters) =>
        new("test", "test", parameters.ToDictionary(p => p.Key, p => p.Value, StringComparer.OrdinalIgnoreCase),
            Guid.NewGuid(), "Usuario", CancellationToken.None);

    [Fact]
    public void Validate_WhenRequiredParamMissing_ShouldFail()
    {
        var command = TestCommands.WithRequiredParam(param: "app");

        var result = _validator.Validate(command, Context());

        result.Should().NotBeNull();
        result!.Status.Should().Be(CommandStatus.Failed);
        result.Message.Should().Contain("app");
    }

    [Fact]
    public void Validate_WhenRequiredParamPresent_ShouldPass()
    {
        var command = TestCommands.WithRequiredParam(param: "app");

        var result = _validator.Validate(command, Context(("app", "notepad")));

        result.Should().BeNull();
    }

    [Fact]
    public void Validate_WhenNoParams_ShouldPass()
    {
        _validator.Validate(TestCommands.NoParams(), Context()).Should().BeNull();
    }
}
