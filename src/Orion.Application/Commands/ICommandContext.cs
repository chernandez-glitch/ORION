namespace Orion.Application.Commands;

/// <summary>
/// Contexto de ejecución de un comando: quién lo invoca, con qué parámetros y
/// bajo qué cancelación. Lo construye el executor y lo reciben validador,
/// autorizador y handler.
/// </summary>
public interface ICommandContext
{
    /// <summary>Clave con la que se invocó (Id o alias).</summary>
    string CommandKey { get; }

    /// <summary>Entrada original del usuario (si la hubo).</summary>
    string RawInput { get; }

    IReadOnlyDictionary<string, string?> Parameters { get; }

    Guid UserId { get; }

    string UserName { get; }

    CancellationToken CancellationToken { get; }

    string? GetParameter(string name);

    bool HasParameter(string name);
}
