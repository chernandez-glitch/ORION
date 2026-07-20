# Roadmap

ORION AI se construye por fases. Cada fase deja la solución compilando y con tests
en verde.

## Fase 0 — Arquitectura y esqueleto ✅ (esta entrega)

- 12 proyectos + tests bajo Clean Architecture, compilando sin errores ni warnings.
- **Command Engine profesional**: pipeline (validación→autorización→logging→ejecución→resultado→historial),
  registro automático, parser desacoplado, permisos, parámetros, `CommandResult`
  uniforme, historial persistido y **Command Palette** (Ctrl+Shift+P) con búsqueda,
  favoritos y recientes. Comandos reales (abrir app/carpeta/URL/Explorador, mensaje).
- Sistema de memoria implementado (SQLite + EF Core).
- Configuración persistente (JSON) + pantalla de ajustes.
- Sistema de plugins (host + loader + catálogo).
- UI WinUI 3: shell con sidebar, dashboard (CPU/RAM/módulos/historial), settings.
- Logging con Serilog, DI en todos los módulos.
- **Sin** IA, voz, automatización real, ni integraciones (por diseño).

## Fase 1 — Automatización Windows real ✅ (entregada)

**Windows Automation Engine** (`Orion.Windows`): SDK con 13 servicios reales
(proceso, ventana, teclado, mouse, explorer, PowerShell, CMD, portapapeles,
archivos, accesos directos, sistema, tareas) vía P/Invoke + BCL, con guarda de
seguridad y logging. Integrado al Command Engine con comandos reales y expuesto
en el **Automation Center** y los widgets del dashboard (CPU/RAM/disco/red/procesos).

## Fase 2 — Inteligencia Artificial

Implementar `IAIProvider` empezando por Ollama (local) y Claude. Un
`AiCommandParser : ICommandParser` traducirá lenguaje natural a comandos del
Command Engine (sin cambiar el resto del motor). La IA **produce comandos**;
nunca ejecuta acciones directamente.

## Fase 3 — Voz

STT, TTS, wake word ("Orion") y cancelación de ruido. Escucha continua.

## Fase 4 — Plugins de integración

SAP Business One, SQL Server, Power BI, GitHub, Docker, Outlook, Teams, Azure.

## Fase 5 — Distribución

`ORION Setup.exe` con auto-update firmado (`IUpdateService`). Migración de SQLite a
SQL Server para escenarios multi-equipo.

## Más allá

- Aprendizaje de hábitos y automatizaciones sugeridas.
- Telemetría opt-in y panel de métricas de uso.
