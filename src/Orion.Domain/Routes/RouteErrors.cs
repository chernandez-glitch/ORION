using Orion.Shared.Results;

namespace Orion.Domain.Routes;

public static class RouteErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Routes.NotFound", $"No existe una ruta con Id '{id}'.");

    public static Error AliasNotFound(string alias) =>
        Error.NotFound("Routes.AliasNotFound", $"No hay ninguna ruta con alias '{alias}'.");

    public static readonly Error AliasTaken =
        Error.Conflict("Routes.AliasTaken", "Ya existe una ruta con ese alias.");
}
