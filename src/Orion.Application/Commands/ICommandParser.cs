namespace Orion.Application.Commands;

/// <summary>
/// Interpreta una entrada de texto y la convierte en una invocación de comando
/// (clave + parámetros). El parser por defecto es sintáctico; en la Fase 2, un
/// parser basado en IA implementará esta misma interfaz para traducir lenguaje
/// natural a comandos, sin que el resto del motor cambie.
/// </summary>
public interface ICommandParser
{
    CommandParseResult Parse(string input, ICommandRegistry registry);
}
