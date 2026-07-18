namespace Orion.Automation.Models;

/// <summary>Resultado de ejecutar un comando de shell.</summary>
/// <param name="ExitCode">Código de salida del proceso.</param>
/// <param name="StandardOutput">Salida estándar capturada.</param>
/// <param name="StandardError">Salida de error capturada.</param>
public sealed record ShellResult(int ExitCode, string StandardOutput, string StandardError);
