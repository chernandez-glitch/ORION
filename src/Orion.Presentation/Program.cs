using System.Threading;
using Microsoft.UI.Dispatching;
using Velopack;

namespace Orion.Presentation;

/// <summary>
/// Punto de entrada de la aplicación. Reemplaza al Main autogenerado de WinUI
/// (ver DISABLE_XAML_GENERATED_MAIN) para poder ejecutar Velopack antes que
/// cualquier código de UI: así se gestionan correctamente los hooks del
/// instalador (instalación, actualización, desinstalación).
/// </summary>
public static class Program
{
    [System.STAThread]
    private static void Main(string[] args)
    {
        VelopackApp.Build().Run();

        WinRT.ComWrappersSupport.InitializeComWrappers();
        Microsoft.UI.Xaml.Application.Start(p =>
        {
            var context = new DispatcherQueueSynchronizationContext(DispatcherQueue.GetForCurrentThread());
            SynchronizationContext.SetSynchronizationContext(context);
            _ = new App();
        });
    }
}
