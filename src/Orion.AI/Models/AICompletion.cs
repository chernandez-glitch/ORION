namespace Orion.AI.Models;

/// <summary>Respuesta de un proveedor de IA.</summary>
/// <param name="Content">Texto generado.</param>
/// <param name="Model">Modelo que produjo la respuesta.</param>
/// <param name="PromptTokens">Tokens de entrada consumidos.</param>
/// <param name="CompletionTokens">Tokens de salida generados.</param>
public sealed record AICompletion(string Content, string Model, int PromptTokens, int CompletionTokens);
