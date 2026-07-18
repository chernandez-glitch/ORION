using Orion.Shared.Results;

namespace Orion.Domain.Users;

public static class UserErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Users.NotFound", $"No existe un usuario con Id '{id}'.");

    public static readonly Error DisplayNameTaken =
        Error.Conflict("Users.DisplayNameTaken", "Ya existe un usuario con ese nombre.");
}
