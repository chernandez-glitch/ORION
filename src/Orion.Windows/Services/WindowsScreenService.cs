using System.Runtime.Versioning;
using Orion.Shared.Results;
using Orion.Windows.Abstractions;
using Orion.Windows.Models;
using Orion.Windows.Native;

namespace Orion.Windows.Services;

[SupportedOSPlatform("windows")]
internal sealed class WindowsScreenService : IScreenService
{
    public Result<ScreenInfo> GetPrimaryScreen()
    {
        var width = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXSCREEN);
        var height = NativeMethods.GetSystemMetrics(NativeMethods.SM_CYSCREEN);
        return Result.Success(new ScreenInfo(width, height));
    }
}
