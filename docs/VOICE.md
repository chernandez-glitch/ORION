# Módulo de voz

`Orion.Voice` define los contratos para escucha y habla.

> **Estado (Fase 0):** solo interfaces. Implementación en Fase 3.

## Contratos

- **`ISpeechToText`** — transcripción (incluye `StreamAsync` para escucha continua).
- **`ITextToSpeech`** — síntesis de voz.
- **`IWakeWordDetector`** — detección de la palabra de activación (evento
  `WakeWordDetected`).
- **`INoiseCanceller`** — limpieza de audio.

## Fase 3 — implementación

Motores candidatos: Windows Speech / Azure Speech / Whisper local para STT;
Azure/Windows TTS para voz; un detector de wake word ligero (p. ej. Porcupine)
para "Orion". La palabra de activación y los dispositivos ya están en la
configuración (`AssistantSettings.ActivationWord`, `VoiceSettings`).
