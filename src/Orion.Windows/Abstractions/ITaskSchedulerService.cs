using Orion.Shared.Results;
using Orion.Windows.Models;

namespace Orion.Windows.Abstractions;

/// <summary>Gestiona tareas programadas (vía schtasks.exe).</summary>
public interface ITaskSchedulerService
{
    /// <summary>Crea una tarea. <paramref name="schedule"/> ej.: "DAILY", "ONLOGON", "HOURLY".</summary>
    Task<Result> CreateAsync(string name, string program, string schedule, CancellationToken cancellationToken = default);

    Task<Result> DeleteAsync(string name, CancellationToken cancellationToken = default);

    Task<Result> RunAsync(string name, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<ScheduledTaskInfo>>> QueryAsync(CancellationToken cancellationToken = default);
}
