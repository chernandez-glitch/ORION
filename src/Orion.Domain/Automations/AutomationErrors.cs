using Orion.Shared.Results;

namespace Orion.Domain.Automations;

public static class AutomationErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Automations.NotFound", $"No existe una automatización con Id '{id}'.");
}
