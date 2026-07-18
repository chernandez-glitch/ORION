using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Orion.Automation.Abstractions;
using Orion.Automation.NoOp;
using Orion.Shared.Modules;

namespace Orion.Automation;

public static class DependencyInjection
{
    /// <summary>
    /// Registra el módulo de automatización. En esta fase todos los puertos se
    /// satisfacen con el adaptador seguro <see cref="PhaseZeroAutomation"/>.
    /// La Fase 1 sustituirá estos registros por los adaptadores Windows reales.
    /// </summary>
    public static IServiceCollection AddOrionAutomation(this IServiceCollection services)
    {
        services.AddSingleton<PhaseZeroAutomation>();

        services.TryAddSingleton<IProcessAutomation>(sp => sp.GetRequiredService<PhaseZeroAutomation>());
        services.TryAddSingleton<IFileAutomation>(sp => sp.GetRequiredService<PhaseZeroAutomation>());
        services.TryAddSingleton<IShellAutomation>(sp => sp.GetRequiredService<PhaseZeroAutomation>());
        services.TryAddSingleton<IInputAutomation>(sp => sp.GetRequiredService<PhaseZeroAutomation>());
        services.TryAddSingleton<IWindowAutomation>(sp => sp.GetRequiredService<PhaseZeroAutomation>());
        services.TryAddSingleton<IPowerAutomation>(sp => sp.GetRequiredService<PhaseZeroAutomation>());

        services.AddSingleton<IModuleStatusProvider, AutomationStatusProvider>();

        return services;
    }
}
