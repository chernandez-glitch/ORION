# Arquitectura

ORION AI sigue **Clean Architecture**: las dependencias apuntan siempre hacia el
núcleo. Las capas internas no conocen las externas.

```
        Presentation (WinUI 3) ── composition root, DI, Serilog
                 │  (depende de todo; nadie depende de él)
   Módulos ──────┤  AI · Voice · Automation · Memory · Plugins · Configuration · Installer
                 │
        Infrastructure ── EF Core SQLite, repos, métricas, logging
                 ▼
        Application ─► Domain ─► Shared
```

## Regla de dependencias

| Proyecto | Depende de |
|----------|-----------|
| Orion.Shared | — |
| Orion.Domain | Shared |
| Orion.Application | Domain, Shared, Automation, **Windows** |
| Orion.Infrastructure | Application, Domain, Shared |
| Orion.Memory | Application, Domain, Shared *(+ EF Core/SQLite propios: el Memory Engine)* |
| **Orion.Windows** | Shared *(SDK de control de Windows: P/Invoke + BCL)* |
| Orion.AI / Voice / Automation / Configuration / Installer | Shared |
| Orion.Plugins | Application, Shared |
| Orion.Presentation | Todos |

El grafo es **acíclico**. Los contratos que la aplicación consume (puertos) viven
en `Application` o en el módulo correspondiente; las implementaciones concretas
(adaptadores) viven en `Infrastructure` o en los módulos.

## Responsabilidad de cada capa

- **Shared** — `Result<T>`, `Error`, `Guard`, `IModuleStatusProvider`. Sin dependencias.
- **Domain** — Entidades (`OrionUser`, `Preference`, `FavoriteProject`, `FavoriteRoute`,
  `Conversation`, `CommandHistoryEntry`, `AutomationDefinition`), interfaces de
  repositorio, errores de dominio, eventos. Lógica pura y testeable.
- **Application** — Casos de uso: motor de comandos (`ICommandDispatcher`),
  fachada de memoria (`IMemoryService`), dashboard (`IDashboardService`), puertos
  (`ISystemMetrics`), DTOs.
- **Infrastructure** — `OrionDbContext` (SQLite), configuraciones EF, repositorios,
  `SystemMetrics`, wiring de Serilog. Implementa lo que Application define.
- **Orion.Windows** — Windows Automation Engine: 13 servicios (proceso, ventana,
  teclado, mouse, shell, archivos, portapapeles, sistema, tareas…) que encapsulan
  todo el interop de Windows tras interfaces. Ver [AUTOMATION.md](AUTOMATION.md).
- **Orion.Memory (Memory Engine)** — memoria permanente con EF Core/SQLite propio
  (`memory.db`): sesiones, conversaciones, proyectos, historial, favoritos,
  etiquetas, búsqueda y contexto. Se integra al Command Engine por la costura
  `ICommandHistory` sin modificarlo. Ver [MEMORY.md](MEMORY.md).
- **Módulos** — Cada capacidad enchufable con su `AddOrionXxx()` de DI.
- **Presentation** — Composition root (`CompositionRoot`), ventana, páginas y
  ViewModels (MVVM con CommunityToolkit.Mvvm).

## Flujo de arranque

1. `App.OnLaunched` → `CompositionRoot.Build()` registra todos los `AddOrion*()`.
2. `provider.InitializeDatabaseAsync()` aplica migraciones EF (crea la base si no existe).
3. Se carga la configuración (`IConfigurationService.LoadAsync`).
4. Se resuelve y activa `MainWindow`; se aplica el tema.

## Flujo de un comando

Todo pasa por el **Command Engine** (ver [COMMANDS.md](COMMANDS.md)); nadie
ejecuta acciones directamente:

```
Origen (UI / Command Palette / voz / IA / plugin)
   → ICommandExecutor.ExecuteAsync("abrir-app notepad")
   → ICommandParser interpreta (comando + parámetros)   [Fase 2: parser de IA]
   → ICommandRegistry resuelve el comando
   → se crea un ámbito de DI y se resuelve el ICommandHandler
   → ICommandPipeline: Validación → Autorización → Logging → Ejecución → Resultado → Historial
   → el comando usa puertos (IProcessAutomation real, IDialogService…)
   → CommandResult (Success/Warning/Failed/Cancelled) vuelve al origen
```

## Ámbitos de DI (importante)

El `DbContext` de EF es *scoped*. Para evitar dependencias cautivas, el
`CommandExecutor` y el `DashboardService` (singletons) crean **un ámbito por
operación** con `IServiceScopeFactory` y resuelven ahí el pipeline, el handler y
la memoria. Los comandos se registran como *transient* y se resuelven dentro de
ese ámbito.
