using Orion.AI.Models;
using Orion.Shared.Results;

namespace Orion.AI.Abstractions;

/// <summary>
/// Contrato de un proveedor de IA. Diseñado en esta fase; las implementaciones
/// concretas (OpenAI, Claude, Gemini, Ollama, Azure) llegan en la Fase 2.
/// </summary>
public interface IAIProvider
{
    AIProviderKind Kind { get; }

    /// <summary>Indica si el proveedor tiene credenciales/config válidas.</summary>
    bool IsConfigured { get; }

    Task<Result<AICompletion>> CompleteAsync(AIPrompt prompt, CancellationToken cancellationToken = default);
}
