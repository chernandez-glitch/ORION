namespace Orion.Application.Memory.Dtos;

public sealed record FavoriteProjectDto(Guid Id, string Name, string Path, DateTime? LastOpenedOnUtc);
