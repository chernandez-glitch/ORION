# Configuración

`Orion.Configuration` persiste los ajustes en JSON y notifica cambios.

## Ubicación

`%AppData%\OrionAI\settings.json` (escritura atómica: archivo temporal + reemplazo).

## Secciones (`OrionSettings`)

| Sección | Campos |
|---------|--------|
| `Assistant` | `Name`, `ActivationWord`, `Language` |
| `AI` | `Provider`, `Model` |
| `Voice` | `Microphone`, `Speaker`, `NoiseCancellation` |
| `Appearance` | `Theme` (System / Light / Dark) |
| `Shortcuts` | mapa nombre → combinación de teclas |
| `FavoriteRoutes`, `FavoriteProjects` | listas |

## API

```csharp
var settings = await config.LoadAsync();     // Result<OrionSettings>
settings.Value.Assistant.Name = "Jarvis";
await config.SaveAsync(settings.Value);       // dispara el evento Changed
```

La pantalla de **Configuración** de la app edita y guarda estos valores, y aplica
el tema al instante.
