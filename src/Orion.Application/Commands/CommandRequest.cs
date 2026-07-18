namespace Orion.Application.Commands;

/// <summary>
/// Petición ya parseada que se entrega a un <see cref="ICommand"/>: el nombre
/// invocado, sus argumentos y la línea original.
/// </summary>
public sealed record CommandRequest(string CommandName, IReadOnlyList<string> Arguments, string RawInput)
{
    public bool HasArguments => Arguments.Count > 0;

    /// <summary>Argumentos unidos como una sola cadena (útil para rutas con espacios ya sin comillas).</summary>
    public string ArgumentLine => string.Join(' ', Arguments);

    public string? FirstArgument => Arguments.Count > 0 ? Arguments[0] : null;
}
