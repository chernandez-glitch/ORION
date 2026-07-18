# Motor de comandos

El motor traduce una línea de texto en la ejecución de un `ICommand`.

## Piezas

- **`ICommand`** — un comando ejecutable con su `CommandDescriptor` (nombre, alias,
  descripción, categoría) y `ExecuteAsync(CommandRequest)`.
- **`ICommandRegistry`** — índice token → tipo de comando. Se construye por
  reflexión al arrancar.
- **`ICommandDispatcher`** — parsea, resuelve, ejecuta (midiendo duración) y
  registra en el historial de memoria.
- **`CommandLineParser`** — tokeniza respetando comillas dobles.

## Comandos incluidos

| Comando | Alias | Categoría |
|---------|-------|-----------|
| `abrir-app` | app, open-app, ejecutar | System |
| `abrir-carpeta` | carpeta, folder | Files |
| `abrir-navegador` | navegador, web, browser | Web |
| `abrir-vscode` | vscode, code | Development |
| `abrir-proyecto` | proyecto, project | Development |
| `apagar` | shutdown | Power |
| `reiniciar` | restart | Power |
| `bloquear` | lock | Power |

> En Fase 0 los comandos delegan en los puertos de `Orion.Automation`, cuyo
> adaptador es un *no-op seguro*: devuelven un `Result` de error controlado
> ("automatización no implementada en esta fase") en lugar de actuar sobre el
> sistema. La Fase 1 conecta los adaptadores Windows reales.

## Cómo crear un comando

1. Crea una clase que implemente `ICommand` en `Orion.Application/Commands/BuiltIn/`.
2. Define su `CommandDescriptor` (nombre, descripción, categoría, alias).
3. Inyecta los puertos que necesite (p. ej. `IProcessAutomation`, `IMemoryService`).
4. Devuelve `Result<CommandOutcome>`.

No hay que registrar nada: el motor lo **descubre automáticamente**.

```csharp
public sealed class SaludarCommand : ICommand
{
    public CommandDescriptor Descriptor { get; } = new(
        "saludar", "Devuelve un saludo.", CommandCategory.System, "hola");

    public Task<Result<CommandOutcome>> ExecuteAsync(CommandRequest request, CancellationToken ct = default)
        => Task.FromResult(Result.Success(CommandOutcome.Ok("¡Hola! Soy ORION.")));
}
```
