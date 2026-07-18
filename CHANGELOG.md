# Changelog

Formato basado en [Keep a Changelog](https://keepachangelog.com/es-ES/1.1.0/).
El proyecto sigue versionado semántico.

## [0.1.0] — 2026-07-18

### Añadido (Fase 0 — Arquitectura y esqueleto)

- Solución .NET 10 con 12 proyectos + tests bajo Clean Architecture.
- **Shared**: patrón `Result<T>`, `Error`, `Guard`, contratos de módulo.
- **Domain**: entidades de memoria, interfaces de repositorio, errores y eventos.
- **Application**: motor de comandos (`ICommandDispatcher`) con descubrimiento
  automático y 8 comandos; fachada de memoria; servicio de dashboard.
- **Infrastructure**: EF Core + SQLite, 8 configuraciones, 7 repositorios,
  Serilog, métricas de CPU/RAM, migración inicial.
- **Memory**: implementación completa del sistema de memoria.
- **Configuration**: ajustes tipados persistidos en JSON.
- **Plugins**: contrato `IOrionPlugin`, loader y catálogo.
- **AI / Voice / Automation / Installer**: contratos e interfaces (diseño).
- **Presentation**: app WinUI 3 con shell + sidebar, dashboard, comandos, memoria
  y configuración; MVVM con CommunityToolkit; temas claro/oscuro.
- **Tests**: 35 pruebas xUnit (Result, Guard, dominio, parser, config, memoria, plugins).
- Documentación completa en `docs/`.
- Pins de seguridad para dependencias transitivas (NU1903).

### Notas

- IA, voz, automatización real e integraciones están **diseñadas**, no implementadas.
  Ver [ROADMAP.md](ROADMAP.md).
