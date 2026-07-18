# Sistema de plugins

Cada integración de ORION es un **plugin independiente**: un ensamblado que aporta
sus propios servicios y comandos.

> **Estado (Fase 0):** el sistema (contrato + loader + catálogo) está implementado
> y funciona con **cero plugins**. Las integraciones concretas (SAP, SQL, GitHub,
> Docker, Power BI, Outlook, Teams, Azure) llegan en la Fase 4.

## Piezas

- **`IOrionPlugin`** — `Metadata` + `Register(IServiceCollection)`.
- **`IPluginLoader`** / `PluginLoader` — descubre `*.dll` en el directorio de
  plugins y carga los tipos que implementan `IOrionPlugin`.
- **`IPluginCatalog`** — plugins efectivamente cargados (para el dashboard).

## Directorio de plugins

Por defecto `<directorio de la app>\plugins`. Si no existe, no se carga nada (la
app funciona igual). Los fallos de un ensamblado se registran y se omiten.

## Cómo crear un plugin

```csharp
public sealed class MiPlugin : IOrionPlugin
{
    public PluginMetadata Metadata { get; } = new(
        "orion.miplugin", "Mi Plugin", new Version(1, 0, 0), "Integra X", "Grupo Platino");

    public void Register(IServiceCollection services)
    {
        // registra servicios y comandos (ICommand) del plugin
    }
}
```

Compila el plugin como librería y copia su `.dll` al directorio de plugins.
