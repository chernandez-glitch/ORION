namespace Orion.Plugins;

/// <summary>Identidad de un plugin de ORION.</summary>
/// <param name="Id">Identificador estable (p. ej. "orion.sap").</param>
/// <param name="Name">Nombre visible.</param>
/// <param name="Version">Versión del plugin.</param>
/// <param name="Description">Qué integra o aporta.</param>
/// <param name="Author">Autor o equipo responsable.</param>
public sealed record PluginMetadata(string Id, string Name, Version Version, string Description, string Author);
