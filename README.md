# ORION AI

> Asistente inteligente para Windows inspirado en Jarvis. Plataforma modular de
> IA capaz de entender lenguaje natural y controlar Windows, herramientas de
> desarrollo e integraciones empresariales.

**Estado:** Fase 0 — Arquitectura y esqueleto de producción. La interfaz,
la memoria, la configuración, el motor de comandos y el sistema de plugins están
implementados y compilando. IA, voz, automatización real e integraciones (SAP,
Power BI, GitHub…) están **diseñadas** (interfaces) y se implementan en fases
posteriores. Ver [ROADMAP.md](ROADMAP.md).

## Tecnologías

- **.NET 10** · **C#** · **WinUI 3** (Windows App SDK)
- **Clean Architecture** · SOLID · DDD ligero · CQRS-ready
- **SQLite** (EF Core) → SQL Server más adelante
- **Serilog** · **Microsoft.Extensions.DependencyInjection** · **xUnit**

## Estructura de la solución

```
ORION.slnx
├── src/
│   ├── Orion.Shared           Result<T>, Error, Guard, contratos de módulo
│   ├── Orion.Domain           Entidades, repos, errores y eventos de dominio
│   ├── Orion.Application       Motor de comandos, memoria (fachada), dashboard
│   ├── Orion.Infrastructure    EF Core SQLite, repos, Serilog, métricas
│   ├── Orion.AI                Interfaces de proveedores de IA (diseño)
│   ├── Orion.Voice             Interfaces STT/TTS/WakeWord (diseño)
│   ├── Orion.Automation        Puertos de automatización Windows + no-op seguro
│   ├── Orion.Memory            Implementación del sistema de memoria
│   ├── Orion.Plugins           Contrato + host/loader de plugins
│   ├── Orion.Configuration     Ajustes tipados persistidos en JSON
│   ├── Orion.Installer         Contratos de auto-actualización
│   └── Orion.Presentation      App WinUI 3 (shell, dashboard, settings)
├── tests/
│   └── Orion.Tests            xUnit
└── docs/                      Documentación técnica
```

## Requisitos

- Windows 10 (19041+) / Windows 11
- .NET 10 SDK
- Para compilar la UI WinUI 3: **Visual Studio Build Tools** con el componente
  *Windows App SDK C# Templates* (aporta las tareas MSBuild de empaquetado/PRI).
  Ver [docs/INSTALL.md](docs/INSTALL.md).

## Puesta en marcha

```bash
dotnet build ORION.slnx                                   # compila toda la solución
dotnet test tests/Orion.Tests/Orion.Tests.csproj          # ejecuta los tests
dotnet run --project src/Orion.Presentation               # arranca la app
```

La base de datos SQLite y los logs se crean automáticamente en
`%AppData%\OrionAI\`.

## Documentación

| Documento | Contenido |
|-----------|-----------|
| [ARCHITECTURE](docs/ARCHITECTURE.md) | Capas, dependencias y flujo |
| [PATTERNS](docs/PATTERNS.md) | Result, DI, puertos/adaptadores, MVVM |
| [COMMANDS](docs/COMMANDS.md) | Motor de comandos y cómo crear uno |
| [MEMORY](docs/MEMORY.md) | Sistema de memoria |
| [AUTOMATION](docs/AUTOMATION.md) | Módulo de automatización |
| [AI](docs/AI.md) | Módulo de IA |
| [VOICE](docs/VOICE.md) | Módulo de voz |
| [PLUGINS](docs/PLUGINS.md) | Sistema de plugins |
| [CONFIGURATION](docs/CONFIGURATION.md) | Ajustes |
| [SECURITY](docs/SECURITY.md) | Consideraciones de seguridad |
| [INSTALL](docs/INSTALL.md) | Instalación de tooling y de la app |
| [ROADMAP](ROADMAP.md) · [CONTRIBUTING](CONTRIBUTING.md) · [CHANGELOG](CHANGELOG.md) | |

## Licencia

Propiedad de Grupo Platino. Uso interno.
