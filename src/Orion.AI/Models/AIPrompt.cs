using System.Collections.ObjectModel;

namespace Orion.AI.Models;

/// <summary>Petición de completado enviada a un proveedor de IA.</summary>
/// <param name="Messages">Historial de mensajes (system/user/assistant).</param>
/// <param name="Temperature">Aleatoriedad del muestreo, 0.0–2.0.</param>
/// <param name="MaxTokens">Límite de tokens de salida, si aplica.</param>
public sealed record AIPrompt(
    IReadOnlyList<AIMessage> Messages,
    double Temperature = 0.7,
    int? MaxTokens = null)
{
    public static AIPrompt FromUser(string content, string? system = null)
    {
        var messages = new List<AIMessage>();
        if (!string.IsNullOrWhiteSpace(system))
        {
            messages.Add(new AIMessage(AIRole.System, system));
        }

        messages.Add(new AIMessage(AIRole.User, content));
        return new AIPrompt(new ReadOnlyCollection<AIMessage>(messages));
    }
}
