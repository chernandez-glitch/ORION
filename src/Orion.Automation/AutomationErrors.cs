using Orion.Shared.Results;

namespace Orion.Automation;

public static class AutomationErrors
{
    /// <summary>
    /// La automatización está diseñada pero aún no implementada en esta fase.
    /// Los adaptadores no-op devuelven este error en lugar de ejecutar acciones.
    /// </summary>
    public static Error NotImplemented(string capability) =>
        Error.Failure(
            "Automation.NotImplemented",
            $"La capacidad de automatización '{capability}' aún no está implementada en esta fase del proyecto.");
}
