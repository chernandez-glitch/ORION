namespace Orion.Application.Commands;

/// <summary>
/// Punto de entrada único del motor. TODA acción de ORION (UI, voz, IA, plugins)
/// debe pasar por aquí para ejecutar un comando; nadie ejecuta acciones directamente.
/// </summary>
public interface ICommandExecutor
{
    /// <summary>Ejecuta un comando por su clave (Id o alias) con parámetros explícitos.</summary>
    Task<CommandResult> ExecuteAsync(string commandKey, IReadOnlyDictionary<string, string?> parameters, CancellationToken cancellationToken = default);

    /// <summary>Interpreta una línea de entrada (parser) y ejecuta el comando resultante.</summary>
    Task<CommandResult> ExecuteAsync(string rawInput, CancellationToken cancellationToken = default);
}
