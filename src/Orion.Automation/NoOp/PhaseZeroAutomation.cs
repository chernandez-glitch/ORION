using Microsoft.Extensions.Logging;
using Orion.Automation.Abstractions;
using Orion.Automation.Models;
using Orion.Shared.Results;

namespace Orion.Automation.NoOp;

/// <summary>
/// Adaptador de automatización de la fase de diseño. Implementa todos los
/// puertos pero NO ejecuta ninguna acción real sobre el sistema: cada método
/// devuelve un fallo controlado. Esto permite que los comandos que dependen de
/// automatización compilen y se ejecuten de forma segura hasta que lleguen los
/// adaptadores Windows reales (Fase 1).
/// </summary>
internal sealed class PhaseZeroAutomation(ILogger<PhaseZeroAutomation> logger)
    : IProcessAutomation, IFileAutomation, IShellAutomation, IInputAutomation, IWindowAutomation, IPowerAutomation
{
    private Result Refuse(string capability)
    {
        logger.LogWarning("Automatización '{Capability}' solicitada pero no implementada en esta fase.", capability);
        return Result.Failure(AutomationErrors.NotImplemented(capability));
    }

    private Result<T> Refuse<T>(string capability)
    {
        logger.LogWarning("Automatización '{Capability}' solicitada pero no implementada en esta fase.", capability);
        return Result.Failure<T>(AutomationErrors.NotImplemented(capability));
    }

    public Task<Result<ProcessInfo>> LaunchAsync(string path, string? arguments = null, CancellationToken cancellationToken = default) =>
        Task.FromResult(Refuse<ProcessInfo>("process.launch"));

    public Task<Result> CloseAsync(string processName, CancellationToken cancellationToken = default) =>
        Task.FromResult(Refuse("process.close"));

    public Task<Result<IReadOnlyList<ProcessInfo>>> FindAsync(string processName, CancellationToken cancellationToken = default) =>
        Task.FromResult(Refuse<IReadOnlyList<ProcessInfo>>("process.find"));

    public Task<Result> CopyAsync(string source, string destination, bool overwrite, CancellationToken cancellationToken = default) =>
        Task.FromResult(Refuse("file.copy"));

    public Task<Result> MoveAsync(string source, string destination, CancellationToken cancellationToken = default) =>
        Task.FromResult(Refuse("file.move"));

    public Task<Result> DeleteAsync(string path, CancellationToken cancellationToken = default) =>
        Task.FromResult(Refuse("file.delete"));

    public Task<Result<ShellResult>> RunAsync(ShellKind shell, string command, CancellationToken cancellationToken = default) =>
        Task.FromResult(Refuse<ShellResult>($"shell.{shell}"));

    public Task<Result> SendKeysAsync(string keys, CancellationToken cancellationToken = default) =>
        Task.FromResult(Refuse("input.sendkeys"));

    public Task<Result> MoveMouseAsync(int x, int y, CancellationToken cancellationToken = default) =>
        Task.FromResult(Refuse("input.mouse"));

    public Task<Result<IReadOnlyList<ProcessInfo>>> ListWindowsAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(Refuse<IReadOnlyList<ProcessInfo>>("window.list"));

    public Task<Result> FocusAsync(string windowTitle, CancellationToken cancellationToken = default) =>
        Task.FromResult(Refuse("window.focus"));

    public Task<Result> ShutdownAsync(TimeSpan delay, CancellationToken cancellationToken = default) =>
        Task.FromResult(Refuse("power.shutdown"));

    public Task<Result> RestartAsync(TimeSpan delay, CancellationToken cancellationToken = default) =>
        Task.FromResult(Refuse("power.restart"));

    public Task<Result> LockAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult(Refuse("power.lock"));
}
