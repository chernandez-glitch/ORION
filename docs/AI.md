# Módulo de IA

`Orion.AI` define los contratos para conversar con modelos de lenguaje.

> **Estado (Fase 0):** solo interfaces. No hay proveedores concretos todavía.

## Contratos

- **`IAIProvider`** — `CompleteAsync(AIPrompt) → Result<AICompletion>`, más `Kind`
  e `IsConfigured`.
- **`IAIProviderFactory`** — resuelve un proveedor por `AIProviderKind`.
- **Modelos** — `AIPrompt`, `AIMessage`, `AIRole`, `AICompletion`.

## Proveedores previstos (`AIProviderKind`)

`OpenAI` · `Claude` · `Gemini` · `Ollama` · `AzureOpenAI`

## Fase 2 — implementación

Cada proveedor será un adaptador de `IAIProvider` (p. ej. `OllamaProvider`,
`ClaudeProvider`) registrado vía DI. La selección de proveedor/modelo ya está en
la configuración (`AISettings`) y editable desde la pantalla de ajustes.

> Recomendación de arranque: empezar por **Ollama** (local, sin coste ni clave) y
> **Claude** para calidad. Usar siempre los modelos Claude más recientes.
