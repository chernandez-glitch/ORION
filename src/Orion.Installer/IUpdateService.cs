using Orion.Installer.Models;
using Orion.Shared.Results;

namespace Orion.Installer;

/// <summary>
/// Contrato de actualizaciones automáticas de ORION. Diseñado para respaldar
/// "ORION Setup.exe" con auto-update; la implementación real (descarga, firma,
/// aplicación) llega en la Fase 5.
/// </summary>
public interface IUpdateService
{
    Task<Result<UpdateCheckResult>> CheckForUpdatesAsync(UpdateChannel channel, CancellationToken cancellationToken = default);

    Task<Result> DownloadAndApplyAsync(UpdateInfo update, CancellationToken cancellationToken = default);
}
