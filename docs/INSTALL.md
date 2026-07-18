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

## Instalador (Fase 5)

El objetivo es generar `ORION Setup.exe` con actualizaciones automáticas. Los
contratos ya existen en `Orion.Installer` (`IUpdateService`, `UpdateInfo`).
