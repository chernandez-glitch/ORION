# Windows Automation Engine (`Orion.Windows`)

El **Windows Automation Engine** es el SDK que controla Windows. **Toda acción
física sobre el sistema operativo pasa por este módulo** — la UI, los comandos y
(en el futuro) la IA nunca tocan Windows directamente.

Es un proyecto `net10.0` que encapsula todo el interop de Windows (P/Invoke a
`user32`/`kernel32`/`shell32`, `Process`, BCL) tras interfaces limpias, de modo
que la capa de aplicación (`net10.0`, portable) puede consumirlo.

## Los 13 servicios

| Servicio | Qué hace |
|----------|----------|
| `IProcessService` | Listar, buscar, abrir, cerrar, matar, reiniciar procesos; CPU/RAM; esperar; existe |
| `IWindowService` | Listar, buscar, traer al frente, min/max, cerrar, ocultar/mostrar, mover, redimensionar |
| `IExplorerService` | Abrir carpetas, seleccionar archivo, propiedades |
| `IKeyboardService` | Enviar texto, teclas y combinaciones (Ctrl/Shift/Alt/Win, F1-F12, Tab, Enter, Esc, Delete) |
| `IMouseService` | Mover, click izq/der, doble click, scroll, posición |
| `IScreenService` | Resolución de pantalla |
| `IPowerShellService` | Ejecutar scripts/archivos, capturar salida/errores, cancelar |
| `ICmdService` | Ejecutar comandos, capturar respuesta, ejecutar como admin |
| `IClipboardService` | Leer/escribir texto, pegar, copiar archivos (CF_HDROP) |
| `IFileSystemService` | Crear/borrar/mover/copiar carpetas y archivos, renombrar, ZIP/UNZIP |
| `IShortcutService` | Crear, abrir, eliminar accesos directos (.lnk) |
| `ISystemService` | Apagar, reiniciar, suspender, bloquear, cerrar sesión, info del sistema |
| `ITaskSchedulerService` | Crear, eliminar, ejecutar, consultar tareas programadas |

Más `ISystemMonitor` (CPU/RAM/disco/red/procesos en tiempo real, para el
dashboard y el Automation Center).

Todos devuelven el patrón `Result<T>` y registran cada acción con Serilog.

## Seguridad

`ISafetyGuard` (`WindowsSafetyGuard`) es la última línea de defensa:

- **Bloquea borrar** rutas raíz (`C:\`) o de sistema (`Windows`, `Program Files`).
- **Bloquea matar** procesos críticos (`lsass`, `winlogon`, `explorer`, la propia app…).

Los comandos destructivos (matar, borrar, apagar) llevan además `Permission =
Elevated` y márgenes de seguridad. La confirmación al usuario la añade la capa
de UI/comandos.

## Interop aislado

Todo el P/Invoke vive en `Native/NativeMethods.cs` y `Native/NativeTypes.cs`, y
las clases de servicio están marcadas con `[SupportedOSPlatform("windows")]`. El
resto del SDK no ve un solo `DllImport`.

## Cómo agregar un servicio nuevo

1. Define la interfaz en `Abstractions/IMiServicio.cs` (devuelve `Result`).
2. Impleméntala en `Services/WindowsMiServicio.cs` (`[SupportedOSPlatform("windows")]`, con `ILogger`).
3. Regístrala en `AddOrionWindows()`.
4. (Opcional) Expón la capacidad como comando en el Command Engine.

## Cómo lo usa el Command Engine

Los comandos reales inyectan estos servicios por constructor. Ejemplo real
(`RunPowerShellScriptCommand`):

```csharp
public sealed class RunPowerShellScriptCommand(IPowerShellService powerShell) : CommandBase
{
    public override string Id => "powershell.run";
    // …metadatos…
    public override async Task<CommandResult> ExecuteAsync(ICommandContext ctx)
    {
        var r = await powerShell.RunScriptAsync(ctx.GetParameter("script")!, ctx.CancellationToken);
        return r.IsSuccess ? CommandResult.Success(r.Value.Output) : CommandResult.Failed(r.Error.Message);
    }
}
```

## Cómo lo usará la IA

La IA nunca ejecutará acciones directamente. Producirá un **comando** (vía
`ICommandParser` de IA en la Fase 2) que el `ICommandExecutor` ejecutará por su
pipeline; ese comando usará estos servicios. Así, cada acción de la IA queda
validada, autorizada, registrada y auditada por el mismo camino que todo lo demás.
