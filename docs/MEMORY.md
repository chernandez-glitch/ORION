# Sistema de memoria

ORION recuerda: usuario activo, preferencias, proyectos favoritos, rutas,
historial de comandos y conversaciones. Es el único módulo de "estado" que se
**implementa** por completo en Fase 0.

## Piezas

- **`IMemoryService`** (Application) — fachada única de la memoria.
- **`MemoryService`** (Orion.Memory) — implementación sobre los repositorios de
  dominio y la unidad de trabajo.
- **Entidades de dominio** — `OrionUser`, `Preference`, `FavoriteProject`,
  `FavoriteRoute`, `Conversation` + `ConversationMessage`, `CommandHistoryEntry`,
  `AutomationDefinition`.
- **Persistencia** — EF Core + SQLite (`Orion.Infrastructure`).

## Usuario activo

La app de escritorio es mono-usuario por sesión. `GetOrCreateActiveUserAsync`
crea un usuario por defecto ("Usuario") la primera vez y cachea su Id en
`IActiveUserContext`. El resto de operaciones lo resuelven internamente.

## Operaciones

```csharp
await memory.SetPreferenceAsync("tema", "oscuro");
await memory.AddProjectAsync("AppTemplate", @"C:\repos\app-template");
await memory.AddRouteAsync("descargas", @"C:\Users\yo\Downloads");
var recientes = await memory.GetRecentCommandsAsync(10);
```

Toda operación devuelve `Result`/`Result<T>` y es **defensiva**: un fallo de
infraestructura se traduce a un resultado fallido, nunca a una excepción sin
controlar.

## Persistencia

- Base de datos: `%AppData%\OrionAI\orion.db` (SQLite).
- Migraciones EF: `src/Orion.Infrastructure/Persistence/Migrations`.
- Se aplican automáticamente al arrancar (`InitializeDatabaseAsync`).

Para añadir un campo: modifica la entidad + su `IEntityTypeConfiguration`, genera
una migración y reconstruye.
