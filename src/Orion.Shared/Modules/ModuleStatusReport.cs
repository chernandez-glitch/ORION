namespace Orion.Shared.Modules;

/// <summary>
/// Instantánea del estado de un módulo para el dashboard.
/// </summary>
/// <param name="Module">Nombre visible del módulo (p. ej. "IA", "Memoria").</param>
/// <param name="Status">Estado operativo actual.</param>
/// <param name="Detail">Descripción corta legible del estado.</param>
public sealed record ModuleStatusReport(string Module, ModuleStatus Status, string Detail);
