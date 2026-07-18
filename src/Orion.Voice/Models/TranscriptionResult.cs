namespace Orion.Voice.Models;

/// <summary>Resultado de una transcripción de voz a texto.</summary>
/// <param name="Text">Texto reconocido.</param>
/// <param name="Confidence">Confianza del reconocimiento, 0.0–1.0.</param>
/// <param name="Language">Idioma detectado (código BCP-47).</param>
public sealed record TranscriptionResult(string Text, double Confidence, string Language);
