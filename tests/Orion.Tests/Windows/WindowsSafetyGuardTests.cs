using FluentAssertions;
using Orion.Windows.Safety;
using Xunit;

namespace Orion.Tests.Windows;

public sealed class WindowsSafetyGuardTests
{
    private readonly WindowsSafetyGuard _guard = new();

    [Theory]
    [InlineData("")]
    [InlineData("C:\\")]
    [InlineData("C:\\Windows\\System32")]
    public void EnsureSafeToDelete_ShouldBlockProtectedPaths(string path)
    {
        _guard.EnsureSafeToDelete(path).IsFailure.Should().BeTrue();
    }

    [Fact]
    public void EnsureSafeToDelete_ShouldAllowNormalPath()
    {
        _guard.EnsureSafeToDelete("C:\\Temp\\Orion\\archivo.txt").IsSuccess.Should().BeTrue();
    }

    [Theory]
    [InlineData("explorer")]
    [InlineData("lsass.exe")]
    [InlineData("winlogon")]
    public void EnsureSafeToKill_ShouldBlockCriticalProcesses(string name)
    {
        _guard.EnsureSafeToKill(name).IsFailure.Should().BeTrue();
    }

    [Fact]
    public void EnsureSafeToKill_ShouldAllowNormalProcess()
    {
        _guard.EnsureSafeToKill("notepad").IsSuccess.Should().BeTrue();
    }
}
