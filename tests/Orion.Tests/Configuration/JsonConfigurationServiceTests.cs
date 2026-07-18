using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Orion.Configuration;
using Orion.Configuration.Models;
using Xunit;

namespace Orion.Tests.Configuration;

public sealed class JsonConfigurationServiceTests : IDisposable
{
    private readonly string _tempFile = Path.Combine(Path.GetTempPath(), $"orion-test-{Guid.NewGuid():N}.json");

    [Fact]
    public async Task Load_WhenNoFile_ShouldReturnDefaults()
    {
        var service = new JsonConfigurationService(_tempFile, NullLogger<JsonConfigurationService>.Instance);

        var result = await service.LoadAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.Assistant.Name.Should().Be("Orion");
    }

    [Fact]
    public async Task SaveThenLoad_ShouldRoundTrip()
    {
        var service = new JsonConfigurationService(_tempFile, NullLogger<JsonConfigurationService>.Instance);
        var settings = new OrionSettings();
        settings.Assistant.Name = "Jarvis";
        settings.Appearance.Theme = ThemePreference.Dark;

        var save = await service.SaveAsync(settings);
        save.IsSuccess.Should().BeTrue();

        var reloaded = await new JsonConfigurationService(_tempFile, NullLogger<JsonConfigurationService>.Instance).LoadAsync();

        reloaded.Value.Assistant.Name.Should().Be("Jarvis");
        reloaded.Value.Appearance.Theme.Should().Be(ThemePreference.Dark);
    }

    [Fact]
    public async Task Save_ShouldRaiseChangedEvent()
    {
        var service = new JsonConfigurationService(_tempFile, NullLogger<JsonConfigurationService>.Instance);
        OrionSettings? raised = null;
        service.Changed += (_, s) => raised = s;

        await service.SaveAsync(new OrionSettings());

        raised.Should().NotBeNull();
    }

    public void Dispose()
    {
        if (File.Exists(_tempFile))
        {
            File.Delete(_tempFile);
        }
    }
}
