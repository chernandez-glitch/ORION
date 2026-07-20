# Roadmap

Estado de ORION AI por fases. Cada fase se apoya en la anterior **sin reescribir**
lo ya hecho: las capacidades nuevas entran por costuras (interfaces + DI) ya
presentes.

## ✅ Fase 0 — Arquitectura base

Clean Architecture (Shared → Domain → Application → Infrastructure + módulos), 14
proyectos, `Result<T>`, DI, Serilog, configuración, instalador (Velopack +
auto-update por GitHub Releases), suite de pruebas. Ver
[ARCHITECTURE.md](ARCHITECTURE.md).

## ✅ Fase 1 — Aplicación funcional (sin IA)

- **UI premium (WinUI 3)** — shell con barra de título Mica, sidebar, navegación,
  temas, MVVM.
- **Command Engine** — pipeline (validación → autorización → logging → ejecución →
  historial), registro por reflexión, Command Palette (Ctrl+Shift+P). Ver
  [COMMANDS.md](COMMANDS.md).
- **Windows Automation Engine** (`Orion.Windows`) — 13 servicios de interop
  (proceso, ventana, teclado, mouse, shell, archivos…). Ver [AUTOMATION.md](AUTOMATION.md).
- **Memory Engine** (`Orion.Memory`) — memoria permanente EF Core/SQLite, integrada
  al Command Engine por `ICommandHistory`. Ver [MEMORY.md](MEMORY.md).
- **Voice Engine** (`Orion.Voice`) — micrófono → wake word → STT → parser. Llega
  hasta *Audio → Texto → Parser → Mostrar resultado*, **sin IA ni ejecución**. Ver
  [VOICE.md](VOICE.md).

## 🔜 Fase 2 — Inteligencia (IA)

- Parser de lenguaje natural como `ICommandParser` basado en IA (misma interfaz):
  texto/voz → comando. El Command Engine y el Voice Engine no cambian.
- **Ejecución desde voz:** conectar `VoicePipeline` → `ICommandExecutor` (hoy solo
  reconoce y muestra).
- Contexto de la IA desde el Memory Engine (`IContextService`, `ISearchMemoryService`).
- Conversaciones asistidas (modelos Claude).

## 🔜 Fase 3 — Voz de extremo a extremo

- Proveedores de **wake word** reales (OpenWakeWord/Porcupine/Custom) con modelos.
- **STT** real (Whisper local/API, Azure, Windows) tras `ISpeechRecognitionService`.
- **TTS** real (Piper/Windows/Azure/ElevenLabs/OpenAI) tras `ITextToSpeechService`:
  respuesta hablada.
- Captura y medición de audio reales; cancelación de ruido.

## 🔮 Fases siguientes

- Runtime de plugins de terceros ([PLUGINS.md](PLUGINS.md)) con permisos.
- Embeddings/RAG detrás de `ISearchMemoryService` (búsqueda semántica).
- Sincronización/backup opcional; telemetría local.

> Regla de oro del roadmap: **cada capacidad nueva entra por una interfaz que ya
> existe**. Si una fase exige reescribir un motor previo, el diseño de la costura
> estaba mal — se corrige la costura, no el motor.
