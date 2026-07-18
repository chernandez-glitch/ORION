namespace Orion.Installer.Models;

/// <summary>Descripción de una actualización disponible.</summary>
/// <param name="Version">Versión ofrecida.</param>
/// <param name="PackageUrl">URL del paquete de instalación.</param>
/// <param name="ReleaseNotes">Notas de la versión.</param>
/// <param name="IsMandatory">Si la actualización es obligatoria.</param>
public sealed record UpdateInfo(Version Version, Uri PackageUrl, string ReleaseNotes, bool IsMandatory);

/// <summary>Resultado de comprobar actualizaciones.</summary>
/// <param name="IsUpdateAvailable">Si hay una versión más nueva.</param>
/// <param name="Current">Versión instalada.</param>
/// <param name="Available">Datos de la actualización, si la hay.</param>
public sealed record UpdateCheckResult(bool IsUpdateAvailable, Version Current, UpdateInfo? Available);
