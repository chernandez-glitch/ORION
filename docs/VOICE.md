# Voice Engine (escucha y habla)

ORION escucha por voz. El **Voice Engine** (`Orion.Voice`, namespace
`Orion.Voice.Engine`) es un módulo autocontenido que convierte **audio → texto →
comando reconocido**, con la palabra de activación **"Orion"** como disparador.

> **Límite de esta fase (deliberado):** el pipeline llega hasta *Audio → Texto →
> Parser → Mostrar resultado*. **No** hay conexión con IA y **no** se ejecuta
> ningún comando automáticamente. Todo queda preparado para enchufar el Command
> Engine y la IA más adelante sin tocar el motor de voz.

## Flujo del pipeline

```
Micrófono ──▶ Wake Word ("Orion") ──▶ Grabación ──▶ STT (audio→texto)
                                                          │
                                                          ▼
                                              Command Parser (interpreta)
                                                          │
                                                          ▼
                              Resultado mostrado  ──▶  Historial de voz
                              ("Comando reconocido: 'X' — no ejecutado")
```

Estados del pipeline (`VoiceState`): `Idle → Listening → Recording → Processing →
(Speaking) → Listening`. Ante error, `Error`.

## Contratos (interfaces)

| Interfaz | Rol |
|----------|-----|
| `IVoiceEngine` | Fachada del motor (estado + arranque/parada + subservicios) |
| `IVoicePipeline` | Orquesta el flujo mic→wake→STT→parser→resultado |
| `IWakeWordService` | Detección de la palabra de activación (evento `WakeWordDetected`) |
| `ISpeechRecognitionService` | STT: audio/entrada → `SpeechResult` (texto + confianza + proveedor) |
| `ITextToSpeechService` | TTS: texto → voz (síntesis) |
| `IMicrophoneService` | Micrófono activo y nivel de entrada |
| `IAudioDeviceService` | Enumera micrófonos y altavoces (winmm) |
| `IVoiceConfigurationService` | Opciones (`VoiceOptions`) + evento `Changed` |
| `IVoiceHistory` | Historial de interacciones de voz (en memoria) |

Todos los servicios devuelven `Result`/`Result<T>` donde puede fallar, y registran
con Serilog. Se registran vía `AddOrionVoiceEngine()` (todos *singleton*).

## Proveedores intercambiables (por configuración)

El motor está diseñado para **múltiples proveedores seleccionables sin recompilar**.
En esta fase solo está disponible el proveedor base de cada tipo; el resto son
interfaces preparadas.

| Tipo | Enum | Disponible ahora | Preparados (fases siguientes) |
|------|------|------------------|-------------------------------|
| Wake word | `WakeWordEngine` | `Manual` | `OpenWakeWord`, `Porcupine`, `Custom` |
| STT | `SttProvider` | `Simulated` | `WhisperLocal`, `WhisperApi`, `AzureSpeech`, `WindowsSpeech` |
| TTS | `TtsProvider` | `None` | `Piper`, `WindowsSpeech`, `AzureSpeech`, `ElevenLabs`, `OpenAI` |

- **Manual** (wake word): un disparo explícito (`Trigger()`, botón *Simular "Orion"*)
  hace de palabra de activación mientras no hay modelo acústico cargado.
- **Simulated** (STT): devuelve el texto provisto (recortado) como si lo hubiera
  transcrito, con proveedor `"Simulado"`. Permite ejercitar todo el pipeline sin
  audio real.
- Los demás proveedores devuelven `Result.Failure` (`Voice.*Unavailable`) hasta que
  se implementen: la costura ya existe.

## Configuración (`VoiceOptions`)

`MicrophoneId`, `SpeakerId`, `WakeWord` ("Orion"), `WakeWordEngine`, `Stt`, `Tts`,
`Sensitivity` (0–1), `Language` ("es-HN"). Se editan en **Voice Center → Wake Word**
y se aplican con `IVoiceConfigurationService.Update()`.

## Dispositivos de audio

`IAudioDeviceService` enumera micrófonos (`waveInGetDevCaps`) y altavoces
(`waveOutGetDevCaps`) vía **winmm** (P/Invoke clásico, aislado en `Native/`,
`[SupportedOSPlatform("windows")]`). El micrófono seleccionado y su nivel de
entrada viven en `IMicrophoneService`.

## Historial de voz

Cada interacción registra: `RecognizedText`, `Provider`, `OccurredOnUtc`,
`DurationMs`, `ResultText` y `CommandRecognized`. En esta fase es **en memoria**
(`IVoiceHistory`); podrá enrutarse al Memory Engine sin cambiar la interfaz.

## UI — Voice Center

Pantalla `voice` (sidebar "Voice Center") con pestañas:

- **Control** — estado del pipeline, nivel de micrófono, *Escuchar/Detener*,
  *Simular "Orion"* y un cuadro para **simular habla reconocida** (texto → pipeline).
- **Dispositivos** — micrófonos y altavoces detectados (con *Actualizar*).
- **Wake Word** — palabra de activación, motor, sensibilidad, proveedor STT/TTS,
  micrófono e idioma; *Guardar*.
- **Historial** — interacciones recientes con su resultado.

El estado y el nivel del micrófono se refrescan por sondeo (`Poll()`, timer de
200 ms).

## Cómo se conectará la IA y el Command Engine (fases siguientes)

El punto de integración es **un solo lugar**: `VoicePipeline.SubmitSpeechAsync`.
Hoy hace `parser.Parse(text)` y **muestra** el resultado. Para activar la ejecución:

1. **IA (Fase 2):** el `ICommandParser` inyectado se sustituye por un parser basado
   en IA (misma interfaz) que traduce lenguaje natural → comando. El motor de voz
   no cambia: ya recibe `ICommandParser` por DI.
2. **Ejecución:** tras un parseo exitoso, en lugar de solo formatear el texto, se
   invoca `ICommandExecutor.ExecuteAsync(...)` y el `CommandResult` se convierte en
   respuesta (y, con TTS activo, en voz vía `ITextToSpeechService`).
3. **Voz de respuesta:** `ITextToSpeechService.SpeakAsync(resultado)` con un
   proveedor real (`Piper`/`Windows`/`Azure`…).

Nada de esto requiere reescribir el pipeline: son costuras ya presentes
(`ICommandParser`, `ICommandRegistry`, `ITextToSpeechService`).

## Pruebas

`tests/Orion.Tests/Voice`: `VoicePipelineTests` (wake → STT → parser → historial,
y la garantía de **no ejecución**) y `VoiceServicesTests` (configuración, historial,
STT simulado, wake word, micrófono).
