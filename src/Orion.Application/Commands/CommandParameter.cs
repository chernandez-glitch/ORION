namespace Orion.Application.Commands;

/// <summary>
/// Descripción de un parámetro que acepta un comando. Es metadata: el valor
/// concreto viaja en el <see cref="ICommandContext"/>.
/// </summary>
/// <param name="Name">Nombre/clave del parámetro.</param>
/// <param name="Description">Para qué sirve.</param>
/// <param name="IsRequired">Si es obligatorio (lo valida el pipeline).</param>
/// <param name="Example">Ejemplo de valor, para la UI.</param>
public sealed record CommandParameter(string Name, string Description, bool IsRequired, string? Example = null);
