namespace Orion.Application.Abstractions;

/// <summary>
/// Fuente de tiempo inyectable. Evita llamar a <see cref="DateTime.UtcNow"/>
/// directamente en la lógica de dominio/aplicación, lo que la hace testeable.
/// </summary>
public interface IClock
{
    DateTime UtcNow { get; }
}
