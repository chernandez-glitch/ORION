# Instalación

## Requisitos de desarrollo

- **Windows 10 (build 19041+) o Windows 11.**
- **.NET 10 SDK** — `dotnet --version` debe reportar 10.x.
- **Tooling de WinUI 3.** La UI (`Orion.Presentation`) necesita las tareas MSBuild
  de empaquetado/PRI, que **no** vienen con el SDK de .NET. Se obtienen instalando
  Visual Studio Build Tools con el componente de Windows App SDK:

  ```powershell
  winget install --id Microsoft.VisualStudio.2022.BuildTools --override `
    "--quiet --wait `
     --add Microsoft.VisualStudio.Workload.ManagedDesktopBuildTools `
     --add Microsoft.VisualStudio.ComponentGroup.WindowsAppSDK.Cs `
     --add Microsoft.VisualStudio.Component.Windows11SDK.22621"
  ```

  > Sin este componente, las capas no-UI compilan igual; solo `Orion.Presentation`
  > requiere el tooling.

## Compilar y ejecutar

```bash
dotnet build ORION.slnx                        # toda la solución
dotnet test tests/Orion.Tests/Orion.Tests.csproj
dotnet run --project src/Orion.Presentation    # arranca la app WinUI 3
```

## Datos locales

Al primer arranque se crean automáticamente:

- `%AppData%\OrionAI\orion.db` — base de datos SQLite (memoria).
- `%AppData%\OrionAI\settings.json` — configuración.
- `%AppData%\OrionAI\logs\` — logs rotativos de Serilog.

## Migraciones EF

```bash
dotnet ef migrations add <Nombre> \
  --project src/Orion.Infrastructure \
  --startup-project src/Orion.Infrastructure \
  --output-dir Persistence/Migrations
```

## Instalador con auto-update (Velopack)

ORION se distribuye como `ORION Setup.exe` con **actualizaciones automáticas**
mediante [Velopack](https://velopack.io). El cliente de update ya está integrado
(`VelopackApp.Build().Run()` en `Program.Main` y `VelopackUpdateService : IUpdateService`).

### Generar el instalador

```powershell
pwsh build/pack-installer.ps1 -Version 0.1.0
```

Esto publica self-contained y ejecuta `vpk pack`, produciendo en `releases/`:

| Artefacto | Uso |
|-----------|-----|
| `Orion-win-Setup.exe` | Instalador para el usuario final (~80 MB) |
| `Orion-<ver>-full.nupkg` | Paquete completo para el feed de actualizaciones |
| `releases.win.json`, `RELEASES` | Metadatos del feed |
| `Orion-win-Portable.zip` | Versión portable (sin instalar) |

### Configurar el feed de actualizaciones

1. Sube el contenido de `releases/` a un host (GitHub Releases, servidor web o
   carpeta de red).
2. Apunta la app a ese feed con la variable de entorno `ORION_UPDATE_FEED`
   (o cambia la URL por defecto en `CompositionRoot`).
3. Para publicar una nueva versión: `pwsh build/pack-installer.ps1 -Version 0.2.0`
   y sube de nuevo `releases/`. La app detectará y aplicará la actualización.

### Firma de código

El `Setup.exe` se genera **sin firmar** (SmartScreen mostrará un aviso). Para
distribución pública, firma con un certificado añadiendo `--signParams` a
`vpk pack` (ver comentario en `build/pack-installer.ps1`).
