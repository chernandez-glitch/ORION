namespace Orion.Application.Commands;

/// <summary>
/// Nivel de permiso requerido para ejecutar un comando. La etapa de
/// autorización del pipeline lo evalúa contra el contexto del usuario.
/// </summary>
public enum CommandPermission
{
    /// <summary>Cualquiera puede ejecutarlo.</summary>
    User = 0,

    /// <summary>Requiere privilegios elevados (UAC).</summary>
    Elevated = 1,

    /// <summary>Reservado a administradores.</summary>
    Admin = 2
}
