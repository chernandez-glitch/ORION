using FluentAssertions;
using Orion.Windows.Models;
using Xunit;

namespace Orion.Tests.Windows;

public sealed class SystemLoadTests
{
    [Fact]
    public void Percentages_ShouldBeComputedFromUsedAndTotal()
    {
        var load = new SystemLoad(CpuPercent: 25, RamUsedMb: 8000, RamTotalMb: 16000, DiskUsedGb: 250, DiskTotalGb: 500, NetworkKbps: 12, ProcessCount: 200);

        load.RamPercent.Should().Be(50);
        load.DiskPercent.Should().Be(50);
    }

    [Fact]
    public void Percentages_ShouldBeZeroWhenTotalIsZero()
    {
        var load = SystemLoad.Empty;

        load.RamPercent.Should().Be(0);
        load.DiskPercent.Should().Be(0);
    }

    [Fact]
    public void ProcessDetails_MemoryText_ShouldFormatMegabytes()
    {
        var process = new ProcessDetails(10, "notepad", null, 52_428_800, true);

        process.WorkingSetMb.Should().Be(50);
        process.MemoryText.Should().Be("50 MB");
    }
}
