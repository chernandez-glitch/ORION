using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using Orion.Windows.Abstractions;
using Orion.Windows.Safety;
using Orion.Windows.Services;

namespace Orion.Windows;

public static class DependencyInjection
{
    /// <summary>
    /// Registra el Windows Automation Engine: los 13 servicios, el monitor de
    /// sistema y la guarda de seguridad.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static IServiceCollection AddOrionWindows(this IServiceCollection services)
    {
        services.AddSingleton<ISafetyGuard, WindowsSafetyGuard>();

        services.AddSingleton<IProcessService, WindowsProcessService>();
        services.AddSingleton<IWindowService, WindowsWindowService>();
        services.AddSingleton<IExplorerService, WindowsExplorerService>();
        services.AddSingleton<IKeyboardService, WindowsKeyboardService>();
        services.AddSingleton<IMouseService, WindowsMouseService>();
        services.AddSingleton<IScreenService, WindowsScreenService>();
        services.AddSingleton<IPowerShellService, WindowsPowerShellService>();
        services.AddSingleton<ICmdService, WindowsCmdService>();
        services.AddSingleton<IClipboardService, WindowsClipboardService>();
        services.AddSingleton<IFileSystemService, WindowsFileSystemService>();
        services.AddSingleton<IShortcutService, WindowsShortcutService>();
        services.AddSingleton<ISystemService, WindowsSystemService>();
        services.AddSingleton<ITaskSchedulerService, WindowsTaskSchedulerService>();
        services.AddSingleton<ISystemMonitor, WindowsSystemMonitor>();

        return services;
    }
}
