# Módulo de automatización

`Orion.Automation` define los **puertos** para que ORION controle Windows.

> **Estado (Fase 0):** solo diseño. Existe un adaptador *no-op seguro*
> (`PhaseZeroAutomation`) que implementa todos los puertos y **rechaza** cada
> acción con un `Result` de error controlado. Ninguna acción real se ejecuta
> sobre el sistema hasta la Fase 1.

## Puertos

| Puerto | Capacidad |
|--------|-----------|
| `IProcessAutomation` | Lanzar, cerrar y buscar procesos |
| `IFileAutomation` | Copiar, mover, eliminar archivos |
| `IShellAutomation` | Ejecutar comandos en PowerShell/CMD |
| `IInputAutomation` | Enviar teclas, mover el mouse |
| `IWindowAutomation` | Enumerar y enfocar ventanas |
| `IPowerAutomation` | Apagar, reiniciar, bloquear |

## Fase 1 — implementación real

Para activar la automatización real basta con sustituir los registros de DI en
`AddOrionAutomation` por adaptadores Windows concretos (p. ej. `WindowsProcessAutomation`
usando `System.Diagnostics.Process`, o `WindowsPowerAutomation` invocando `shutdown.exe`).
Los comandos y la UI **no cambian**: dependen solo de los puertos.

## Seguridad

Las acciones destructivas (apagar, eliminar, ejecutar shell) deben pedir
confirmación explícita antes de ejecutarse. Los comandos de energía ya aplican un
margen de 15 s. Ver [SECURITY.md](SECURITY.md).
