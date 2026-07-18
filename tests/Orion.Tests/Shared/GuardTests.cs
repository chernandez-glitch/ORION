using FluentAssertions;
using Orion.Shared.Guards;
using Xunit;

namespace Orion.Tests.Shared;

public sealed class GuardTests
{
    [Fact]
    public void AgainstNull_ShouldThrowOnNull()
    {
        string? value = null;
        var act = () => Guard.AgainstNull(value);
        act.Should().Throw<ArgumentNullException>();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void AgainstNullOrWhiteSpace_ShouldThrow(string? value)
    {
        var act = () => Guard.AgainstNullOrWhiteSpace(value);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AgainstEmpty_ShouldThrowOnEmptyGuid()
    {
        var act = () => Guard.AgainstEmpty(Guid.Empty);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AgainstNegative_ShouldThrowOnNegative()
    {
        var act = () => Guard.AgainstNegative(-1);
        act.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Guards_ShouldReturnValueWhenValid()
    {
        Guard.AgainstNullOrWhiteSpace("ok").Should().Be("ok");
        Guard.AgainstNegative(5).Should().Be(5);
    }
}
