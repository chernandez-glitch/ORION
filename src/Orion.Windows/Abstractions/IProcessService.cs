using Orion.Shared.Results;
using Orion.Windows.Models;

namespace Orion.Windows.Abstractions;

/// <summary>Controla procesos del sistema.</summary>
public interface IProcessService
{
    Task<Result<IReadOnlyList<ProcessDetails>>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<ProcessDetails>>> FindAsync(string name, CancellationToken cancellationToken = default);

    Result<ProcessDetails> Start(string path, string? arguments = null);

    /// <summary>Cierra el proceso de forma ordenada (cierra su ventana principal).</summary>
    Task<Result> CloseAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>Termina el proceso de forma forzada (con guarda de seguridad).</summary>
    Task<Result> KillAsync(string name, CancellationToken cancellationToken = default);

    Task<Result> RestartAsync(string name, CancellationToken cancellationToken = default);

    bool Exists(string name);

    Task<Result> WaitForExitAsync(string name, TimeSpan timeout, CancellationToken cancellationToken = default);

    Task<Result<double>> GetCpuPercentAsync(string name, CancellationToken cancellationToken = default);

    Result<double> GetRamMb(string name);
}
