using System.Text;

namespace Orion.Application.Commands;

/// <summary>
/// Divide una línea de entrada en tokens respetando comillas dobles, de modo
/// que rutas con espacios (p. ej. <c>abrir "C:\Mis Documentos"</c>) se traten
/// como un único argumento.
/// </summary>
public static class CommandLineParser
{
    public static IReadOnlyList<string> Tokenize(string input)
    {
        var tokens = new List<string>();
        if (string.IsNullOrWhiteSpace(input))
        {
            return tokens;
        }

        var current = new StringBuilder();
        var inQuotes = false;

        foreach (var ch in input)
        {
            switch (ch)
            {
                case '"':
                    inQuotes = !inQuotes;
                    break;
                case ' ' when !inQuotes:
                    if (current.Length > 0)
                    {
                        tokens.Add(current.ToString());
                        current.Clear();
                    }

                    break;
                default:
                    current.Append(ch);
                    break;
            }
        }

        if (current.Length > 0)
        {
            tokens.Add(current.ToString());
        }

        return tokens;
    }
}
