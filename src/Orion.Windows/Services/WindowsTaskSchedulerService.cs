using System.Runtime.Versioning;
using Microsoft.Extensions.Logging;
using Orion.Shared.Results;
using Orion.Windows.Abstractions;
using Orion.Windows.Internal;
using Orion.Windows.Models;

namespace Orion.Windows.Services;

[SupportedOSPlatform("windows")]
internal sealed class WindowsTaskSchedulerService(ILogger<WindowsTaskSchedulerService> logger) : ITaskSchedulerService
{
    public Task<Result> CreateAsync(string name, string program, string schedule, CancellationToken cancellationToken = default) =>
        RunAsync($"/create /tn \"{name}\" /tr \"{program}\" /sc {schedule} /f", $"crear tarea '{name}'", cancellationToken);

    public Task<Result> DeleteAsync(string name, CancellationToken cancellationToken = default) =>
        RunAsync($"/delete /tn \"{name}\" /f", $"eliminar tarea '{name}'", cancellationToken);

    public Task<Result> RunAsync(string name, CancellationToken cancellationToken = default) =>
        RunAsync($"/run /tn \"{name}\"", $"ejecutar tarea '{name}'", cancellationToken);

    public async Task<Result<IReadOnlyList<ScheduledTaskInfo>>> QueryAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await ProcessRunner.RunAsync("schtasks.exe", "/query /fo CSV /nh", cancellationToken).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return Result.Failure<IReadOnlyList<ScheduledTaskInfo>>(Error.Failure("Tasks.QueryFailed", result.Error));
            }

            IReadOnlyList<ScheduledTaskInfo> tasks = result.Output
                .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(ParseRow)
                .OfType<ScheduledTaskInfo>()
                .ToArray();

            return Result.Success(tasks);
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            return Result.Failure<IReadOnlyList<ScheduledTaskInfo>>(Error.Failure("Tasks.QueryFailed", ex.Message));
        }
    }

    private async Task<Result> RunAsync(string arguments, string action, CancellationToken cancellationToken)
    {
        try
        {
            var result = await ProcessRunner.RunAsync("schtasks.exe", arguments, cancellationToken).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                return Result.Failure(Error.Failure("Tasks.Failed", $"No se pudo {action}: {result.Error}"));
            }

            logger.LogInformation("TaskScheduler: {Action}.", action);
            return Result.Success();
        }
        catch (Exception ex) when (ex is System.ComponentModel.Win32Exception or InvalidOperationException)
        {
            return Result.Failure(Error.Failure("Tasks.Failed", ex.Message));
        }
    }

    private static ScheduledTaskInfo? ParseRow(string line)
    {
        var columns = line.Split("\",\"");
        if (columns.Length < 3)
        {
            return null;
        }

        var name = columns[0].Trim('"', ' ');
        var nextRun = columns[1].Trim('"', ' ');
        var status = columns[2].Trim('"', ' ');
        return string.IsNullOrWhiteSpace(name) ? null : new ScheduledTaskInfo(name, status, nextRun);
    }
}
