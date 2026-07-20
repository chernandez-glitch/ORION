# Memory Engine (cerebro y memoria)

ORION **recuerda todo**. El Memory Engine (`Orion.Memory`, namespace
`Orion.Memory.Engine`) es la memoria permanente: un módulo autocontenido con su
propio **EF Core + SQLite** (`memory.db`), pensado para que la IA (Fase 2) la
consulte por interfaces — sin IA/embeddings/RAG todavía.

> Convive con la memoria de Fase 0 (`Orion.Application.Memory.IMemoryService`,
> que da historial al Command Engine): el Memory Engine es la capa rica y
> definitiva; nada de lo anterior se tocó.

## Qué recuerda (entidades)

`Session`, `Conversation` + `ConversationMessage`, `Project`, `Workspace`,
`Favorite`, `History`, `CommandHistory`, `ApplicationHistory`, `FolderHistory`,
`RecentFile`, `Tag`, `MemoryItem`, `SettingSnapshot`.

## Servicios (interfaces)

| Servicio | Rol |
|----------|-----|
| `ISessionService` | Sesiones (inicio/fin/duración/eventos) |
| `IConversationService` | Conversaciones y mensajes |
| `IProjectMemoryService` | Proyectos (upsert con conteo de aperturas) |
| `IHistoryService` | Comandos, aplicaciones, carpetas, archivos, eventos |
| `IFavoriteService` | Favoritos (comando/carpeta/app/proyecto/conversación) |
| `ITagService` | Etiquetas y etiquetado |
| `ISearchMemoryService` | Búsqueda por texto/proyecto/fecha/etiqueta/tipo/categoría |
| `IContextService` | Instantánea de contexto actual (para la IA) |
| `IMemoryExportService` | Exportar (JSON/CSV) e importar (JSON) |
| `IMemoryRepository<T>` | Acceso genérico a cualquier entidad |
| `IMemoryEngine` | Fachada que reúne todo lo anterior |

Todos devuelven `Result<T>` y registran con Serilog.

## Flujo de memoria

```
1. Al arrancar → ISessionService.StartSessionAsync() crea la sesión activa
   (IMemorySession guarda su Id).

2. Cada comando ejecutado → el Command Engine llama a ICommandHistory, cuya
   implementación MemoryEngineCommandHistory:
     • guarda el comando en CommandHistory (con la sesión activa),
     • CLASIFICA automáticamente: apps.open → ApplicationHistory,
       folders.open → FolderHistory, vscode.open → app + carpeta, etc.
   (integración por la costura ICommandHistory, sin tocar el Command ni el
   Windows Engine).

3. Proyectos, favoritos, conversaciones, notas → vía los servicios.

4. Búsqueda y contexto → ISearchMemoryService / IContextService leen todo.
```

## Persistencia

- Base: `%AppData%\OrionAI\memory.db` (SQLite, independiente de `orion.db`).
- Migraciones: `src/Orion.Memory/Engine/Persistence/Migrations`.
- Se aplican al arrancar (`InitializeMemoryDatabaseAsync`).

## Exportar / importar

```csharp
await engine.Export.ExportJsonAsync(@"C:\backup\memory.json");   // completo
await engine.Export.ExportCsvAsync(@"C:\backup\memory-csv");     // una CSV por tabla
await engine.Export.ImportJsonAsync(@"C:\backup\memory.json");   // restaurar (merge por Id)
```

## Cómo la IA usará este módulo (Fase 2)

La IA **no accederá a la base directamente**. Consultará la memoria por
interfaces:

- **`IContextService.GetCurrentContextAsync()`** → sesión actual, comandos,
  proyectos, apps, archivos y conversaciones recientes: el "en qué está
  trabajando el usuario".
- **`ISearchMemoryService.SearchAsync(query)`** → recuperar recuerdos relevantes
  por texto/fecha/proyecto/etiqueta.
- **`IConversationService`** → leer/continuar conversaciones previas.

Cuando lleguen embeddings/RAG (fase posterior), se añadirán **detrás de estas
mismas interfaces** (p. ej. un `SemanticSearchService : ISearchMemoryService`),
sin cambiar el resto del sistema. La infraestructura ya está lista.
