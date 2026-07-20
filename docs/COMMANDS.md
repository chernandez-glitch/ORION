# Motor de comandos (Command Engine)

**Toda acción de ORION se ejecuta a través del Command Engine.** La voz, la IA,
la automatización y los plugins nunca actúan directamente: envían un comando al
motor. Esto centraliza validación, autorización, logging e historial.

## Piezas

| Interfaz | Rol |
|----------|-----|
| `ICommand` | Metadatos: Id, Nombre, Descripción, Categoría, Permiso, Alias, Parámetros |
| `ICommandHandler` | Ejecuta la lógica (`ExecuteAsync(ICommandContext)`) |
| `CommandBase` | Base que une metadatos + ejecución (una clase por comando) |
| `ICommandContext` | Contexto: clave invocada, parámetros, usuario, cancelación |
| `ICommandRegistry` | Índice de comandos; resuelve por Id/alias y busca |
| `ICommandValidator` | Valida (p. ej. parámetros obligatorios) |
| `ICommandAuthorizer` | Autoriza según `CommandPermission` |
| `ICommandPipeline` | Orquesta las etapas de ejecución |
| `ICommandExecutor` | **Punto de entrada único** del motor |
| `ICommandHistory` | Persiste el historial (sobre memoria/SQLite) |
| `ICommandParser` | Interpreta texto → comando (preparado para IA) |

`CommandResult` es el resultado uniforme: `Success` · `Warning` · `Failed` ·
`Cancelled`, con mensaje y datos.

## Pipeline de ejecución

Todo comando pasa por (`CommandPipeline`):

```
1. Validación     → ICommandValidator (parámetros obligatorios)
2. Autorización   → ICommandAuthorizer (permiso del comando)
3. Logging        → Serilog (inicio)
4. Ejecución      → ICommandHandler.ExecuteAsync (medida y protegida)
5. Resultado      → CommandResult + Serilog (cierre)
6. Historial      → ICommandHistory (hora, duración, resultado, error, usuario, parámetros)
```

El pipeline **nunca lanza excepciones** al llamador: las traduce a un
`CommandResult.Failed`.

## Registro automático

Los comandos se **descubren por reflexión**: toda clase concreta que herede de
`CommandBase` se registra sola en DI (`AddOrionApplication`). No hay que
registrar nada a mano. El motor soporta cientos de comandos sin cambios de
configuración.

## Comandos incluidos

| Comando | Id | Categoría | Alias |
|---------|----|-----------|-------|
| Abrir aplicación | `apps.open` | Applications | abrir-app, run, ejecutar |
| Abrir carpeta | `folders.open` | Folders | abrir-carpeta, folder |
| Abrir URL | `internet.open-url` | Internet | abrir-url, url, web, navegador |
| Mostrar mensaje | `system.show-message` | System | mensaje, msg |
| Abrir Explorador | `system.open-explorer` | System | explorador, explorer |
| Abrir VS Code | `vscode.open` | VSCode | vscode, code |
| Abrir proyecto | `vscode.open-project` | VSCode | proyecto, project |
| Apagar / Reiniciar / Bloquear / Suspender | `system.shutdown` / `.restart` / `.lock` / `.sleep` | System | apagar, reiniciar, bloquear, suspender |
| Cerrar aplicación | `process.close` | Applications | cerrar-app, close-app |
| Terminar proceso | `process.kill` | System | matar-proceso, kill |
| Crear carpeta | `files.create-folder` | Folders | crear-carpeta, mkdir |
| Eliminar archivo | `files.delete-file` | Files | eliminar-archivo, del-file |
| Abrir PowerShell | `powershell.open` | PowerShell | powershell |
| Ejecutar PowerShell | `powershell.run` | PowerShell | ps, run-ps |
| Ejecutar CMD | `cmd.run` | System | cmd, run-cmd |

Estos comandos **funcionan de verdad**: los de proceso/archivo/shell usan el
**Windows Automation Engine** (`Orion.Windows`, ver [AUTOMATION.md](AUTOMATION.md));
`Mostrar mensaje` usa un `ContentDialog`. Los destructivos (matar/borrar/apagar)
requieren permiso `Elevated` y pasan por la guarda de seguridad.

## Command Palette (Ctrl+Shift+P)

Estilo VS Code / Cursor. Se abre con **Ctrl+Shift+P** y permite:

- **Buscar** por nombre, categoría, descripción y alias.
- **Ejecutar** (Enter ejecuta la línea escrita; los comandos con parámetros
  precargan la caja para completarlos).
- **Favoritos** (★, persistidos en la configuración).
- **Recientes** (del historial).

## Cómo agregar un comando nuevo

Una sola clase:

```csharp
public sealed class SaludarCommand : CommandBase
{
    public override string Id => "system.greet";
    public override string Name => "Saludar";
    public override string Description => "Devuelve un saludo.";
    public override CommandCategory Category => CommandCategory.System;
    public override IReadOnlyList<string> Aliases => ["hola", "saludar"];

    public override Task<CommandResult> ExecuteAsync(ICommandContext context) =>
        Task.FromResult(CommandResult.Success("¡Hola! Soy ORION."));
}
```

Se descubre y registra automáticamente. Si necesita dependencias (p. ej.
`IProcessAutomation`), se inyectan por constructor.

## Cómo lo usará la IA (Fase 2)

El parser está desacoplado tras `ICommandParser`. Hoy el `DefaultCommandParser`
es sintáctico (primer token = comando, resto = parámetros). En la Fase 2, un
`AiCommandParser` implementará la **misma interfaz** para traducir lenguaje
natural ("abre el bloc de notas") a `CommandParseResult` (comando + parámetros),
y el `ICommandExecutor` lo ejecutará por el mismo pipeline. **El resto del motor
no cambia**: la IA solo produce comandos; nunca ejecuta acciones directamente.
