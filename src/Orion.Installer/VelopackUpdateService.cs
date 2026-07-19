using System.Reflection;
using Microsoft.Extensions.Logging;
using Orion.Shared.Results;
using Velopack;

namespace Orion.Installer;

/// <summary>
/// Implementación de <see cref="IUpdateService"/> respaldada por Velopack.
/// Comprueba, descarga y aplica actualizaciones desde el feed de releases.
/// Cuando la app no se instaló vía Velopack (p. ej. en desarrollo), informa que
/// está al día sin tocar la red.
/// </summary>
public sealed class VelopackUpdateService(ILogger<VelopackUpdateService> logger, string feedUrl) : IUpdateService
{
    public async Task<Result<Models.UpdateCheckResult>> CheckForUpdatesAsync(UpdateChannel channel, CancellationToken cancellationToken = default)
    {
        try
        {
            var manager = new UpdateManager(feedUrl);
            var current = ResolveCurrentVersion(manager);

            if (!manager.IsInstalled)
            {
                logger.LogInformation("La app no está instalada vía Velopack; se omite la comprobación de actualizaciones.");
                return Result.Success(new Models.UpdateCheckResult(false, current, null));
            }

            var update = await manager.CheckForUpdatesAsync().ConfigureAwait(false);
            if (update is null)
            {
                return Result.Success(new Models.UpdateCheckResult(false, current, null));
            }

            var asset = update.TargetFullRelease;
            var version = asset.Version;
            var available = new Models.UpdateInfo(
                new Version(version.Major, version.Minor, version.Patch),
                new Uri(feedUrl, UriKind.RelativeOrAbsolute),
                asset.NotesMarkdown ?? string.Empty,
                IsMandatory: false);

            return Result.Success(new Models.UpdateCheckResult(true, current, available));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fallo al comprobar actualizaciones desde {Feed}.", feedUrl);
            return Result.Failure<Models.UpdateCheckResult>(
                Error.Failure("Installer.CheckFailed", $"No se pudo comprobar actualizaciones: {ex.Message}"));
        }
    }

    public async Task<Result> DownloadAndApplyAsync(Models.UpdateInfo update, CancellationToken cancellationToken = default)
    {
        try
        {
            var manager = new UpdateManager(feedUrl);
            if (!manager.IsInstalled)
            {
                return Result.Failure(Error.Failure("Installer.NotInstalled", "La app no está instalada vía Velopack."));
            }

            var pending = await manager.CheckForUpdatesAsync().ConfigureAwait(false);
            if (pending is null)
            {
                return Result.Success();
            }

            await manager.DownloadUpdatesAsync(pending, null, cancellationToken).ConfigureAwait(false);
            manager.ApplyUpdatesAndRestart(pending);
            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fallo al aplicar la actualización.");
            return Result.Failure(Error.Failure("Installer.ApplyFailed", $"No se pudo aplicar la actualización: {ex.Message}"));
        }
    }

    private static Version ResolveCurrentVersion(UpdateManager manager)
    {
        var installed = manager.IsInstalled ? manager.CurrentVersion : null;
        return installed is not null
            ? new Version(installed.Major, installed.Minor, installed.Patch)
            : Assembly.GetEntryAssembly()?.GetName().Version ?? new Version(0, 1, 0);
    }
}
