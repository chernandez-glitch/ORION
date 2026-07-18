using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Orion.Plugins;
using Xunit;

namespace Orion.Tests.Plugins;

public sealed class PluginLoaderTests
{
    [Fact]
    public void Load_WhenDirectoryMissing_ShouldReturnEmpty()
    {
        var loader = new PluginLoader(Path.Combine(Path.GetTempPath(), $"no-such-{Guid.NewGuid():N}"), NullLogger<PluginLoader>.Instance);

        loader.Load().Should().BeEmpty();
    }

    [Fact]
    public void Load_WhenDirectoryEmpty_ShouldReturnEmpty()
    {
        var dir = Path.Combine(Path.GetTempPath(), $"orion-plugins-{Guid.NewGuid():N}");
        Directory.CreateDirectory(dir);
        try
        {
            var loader = new PluginLoader(dir, NullLogger<PluginLoader>.Instance);
            loader.Load().Should().BeEmpty();
        }
        finally
        {
            Directory.Delete(dir, recursive: true);
        }
    }
}
