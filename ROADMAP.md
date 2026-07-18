# Roadmap

ORION AI se construye por fases. Cada fase deja la solución compilando y con tests
en verde.

## Fase 0 — Arquitectura y esqueleto ✅ (esta entrega)

- 12 proyectos + tests bajo Clean Architecture, compilando sin errores ni warnings.
- Motor de comandos con descubrimiento automático y 8 comandos.
- Sistema de memoria implementado (SQLite + EF Core).
- Configuración persistente (JSON) + pantalla de ajustes.
- Sistema de plugins (host + loader + catálogo).
- UI WinUI 3: shell con sidebar, dashboard (CPU/RAM/módulos/historial), settings.
- Logging con Serilog, DI en todos los módulos.
- **Sin** IA, voz, automatización real, ni integraciones (por diseño).

## Fase 1 — Automatización Windows real

Adaptadores concretos para `Orion.Automation` (procesos, archivos, input,
ventanas, energía) con confirmaciones de seguridad.

## Fase 2 — Inteligencia Artificial

Implementar `IAIProvider` empezando por Ollama (local) y Claude. Interpretación de
lenguaje natural → comandos.

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
