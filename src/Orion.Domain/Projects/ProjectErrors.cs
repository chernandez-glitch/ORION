using Orion.Shared.Results;

namespace Orion.Domain.Projects;

public static class ProjectErrors
{
    public static Error NotFound(Guid id) =>
        Error.NotFound("Projects.NotFound", $"No existe un proyecto favorito con Id '{id}'.");
}
