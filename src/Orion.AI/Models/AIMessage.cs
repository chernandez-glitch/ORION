namespace Orion.AI.Models;

/// <summary>Un mensaje dentro de un <see cref="AIPrompt"/>.</summary>
/// <param name="Role">Autor del mensaje.</param>
/// <param name="Content">Texto del mensaje.</param>
public sealed record AIMessage(AIRole Role, string Content);
