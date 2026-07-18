using System.Reflection;
using Microsoft.Extensions.Logging;
using Orion.Installer.Models;
using Orion.Shared.Results;

namespace Orion.Installer;

/// <summary>
/// Servicio de actualización de la fase de diseño: informa siempre que ORION
/// está al día y rechaza aplicar paquetes. La infraestructura de auto-update
/// real se implementa en la Fase 5.
/// </summary>
internal sealed class PhaseZeroUpdateService(ILogger<PhaseZeroUpdateService> logger) : IUpdateService
{
    public Task<Result<UpdateCheckResult>> CheckForUpdatesAsync(UpdateChannel channel, CancellationToken cancellationToken = default)
    {
        var current = Assembly.GetEntryAssembly()?.GetName().Version ?? new Version(0, 1, 0);
        logger.LogInformation("Comprobación de actualizaciones ({Channel}); auto-update llega en Fase 5.", channel);
        var result = new UpdateCheckResult(IsUpdateAvailable: false, Current: current, Available: null);
        return Task.FromResult(Result.Success(result));
    }

    public Task<Result> DownloadAndApplyAsync(UpdateInfo update, CancellationToken cancellationToken = default) =>
        Task.FromResult(Result.Failure(
            Error.Failure("Installer.NotImplemented", "La aplicación de actualizaciones se implementa en la Fase 5.")));
}
