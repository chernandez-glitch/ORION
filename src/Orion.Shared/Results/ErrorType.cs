namespace Orion.Shared.Results;

/// <summary>
/// Clasifica la naturaleza de un <see cref="Error"/> para que las capas
/// superiores (UI, API) puedan mapearlo a un resultado apropiado sin
/// inspeccionar el texto del mensaje.
/// </summary>
public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Unauthorized = 4,
    Forbidden = 5,
    Unexpected = 6
}
