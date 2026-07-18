using FluentAssertions;
using Orion.Shared.Results;
using Xunit;

namespace Orion.Tests.Shared;

public sealed class ResultTests
{
    [Fact]
    public void Success_ShouldNotBeFailure()
    {
        var result = Result.Success();

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_ShouldCarryError()
    {
        var error = Error.Validation("Test.Invalid", "inválido");

        var result = Result.Failure(error);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void SuccessWithValue_ShouldExposeValue()
    {
        Result<int> result = 42;

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(42);
    }

    [Fact]
    public void AccessingValueOnFailure_ShouldThrow()
    {
        Result<int> result = Error.Failure("X", "boom");

        var act = () => result.Value;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Map_ShouldTransformSuccess()
    {
        var result = Result.Success(10).Map(x => x * 2);

        result.Value.Should().Be(20);
    }

    [Fact]
    public void Bind_ShouldShortCircuitOnFailure()
    {
        var error = Error.Failure("X", "boom");

        var result = Result.Failure<int>(error).Bind(x => Result.Success(x + 1));

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be(error);
    }

    [Fact]
    public void Match_ShouldSelectBranch()
    {
        Result.Success(5).Match(_ => "ok", _ => "err").Should().Be("ok");
        Result.Failure<int>(Error.Failure("X", "b")).Match(_ => "ok", _ => "err").Should().Be("err");
    }

    [Fact]
    public void Result_ShouldForbidSuccessCarryingAnError()
    {
        var act = () => new InvalidSuccess();

        act.Should().Throw<InvalidOperationException>();
    }

    private sealed class InvalidSuccess() : Result(true, Error.Failure("X", "no debería"));
}
