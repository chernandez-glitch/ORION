using Microsoft.Extensions.Logging;
using Orion.Application.Abstractions;
using Orion.Application.Memory;
using Orion.Application.Memory.Dtos;
using Orion.Domain.Common;
using Orion.Domain.Conversations;
using Orion.Domain.History;
using Orion.Domain.Preferences;
using Orion.Domain.Projects;
using Orion.Domain.Routes;
using Orion.Domain.Users;
using Orion.Shared.Results;

namespace Orion.Memory;

/// <summary>
/// Implementación del sistema de memoria de ORION. Orquesta los repositorios de
/// dominio y la unidad de trabajo para recordar usuario, preferencias,
/// proyectos, rutas, historial y conversaciones. Todas las operaciones son
/// defensivas: un fallo de infraestructura se traduce a un <see cref="Result"/>
/// fallido en lugar de propagar la excepción.
/// </summary>
public sealed class MemoryService(
    IUserRepository users,
    IPreferenceRepository preferences,
    IFavoriteProjectRepository projects,
    IFavoriteRouteRepository routes,
    ICommandHistoryRepository history,
    IConversationRepository conversations,
    IUnitOfWork unitOfWork,
    IActiveUserContext activeUser,
    IClock clock,
    ILogger<MemoryService> logger) : IMemoryService
{
    private const string DefaultUserName = "Usuario";
    private const string DefaultCulture = "es-HN";

    public Task<Result<Guid>> GetOrCreateActiveUserAsync(CancellationToken cancellationToken = default) =>
        TryAsync("EnsureUser", async () =>
        {
            var userId = await EnsureUserIdAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success(userId);
        });

    public Task<Result<string?>> GetPreferenceAsync(string key, CancellationToken cancellationToken = default) =>
        TryAsync("GetPreference", async () =>
        {
            var userId = await EnsureUserIdAsync(cancellationToken).ConfigureAwait(false);
            var preference = await preferences.GetAsync(userId, key, cancellationToken).ConfigureAwait(false);
            return Result.Success(preference?.Value);
        });

    public Task<Result> SetPreferenceAsync(string key, string value, CancellationToken cancellationToken = default) =>
        TryAsync("SetPreference", async () =>
        {
            var userId = await EnsureUserIdAsync(cancellationToken).ConfigureAwait(false);
            var existing = await preferences.GetAsync(userId, key, cancellationToken).ConfigureAwait(false);

            if (existing is null)
            {
                var preference = Preference.Create(userId, key, value, clock.UtcNow);
                await preferences.AddAsync(preference, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                existing.UpdateValue(value, clock.UtcNow);
                preferences.Update(existing);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success();
        });

    public Task<Result<IReadOnlyList<FavoriteProjectDto>>> GetProjectsAsync(CancellationToken cancellationToken = default) =>
        TryAsync("GetProjects", async () =>
        {
            var userId = await EnsureUserIdAsync(cancellationToken).ConfigureAwait(false);
            var items = await projects.GetForUserAsync(userId, cancellationToken).ConfigureAwait(false);
            IReadOnlyList<FavoriteProjectDto> dtos = items.Select(Map).ToArray();
            return Result.Success(dtos);
        });

    public Task<Result<FavoriteProjectDto>> AddProjectAsync(string name, string path, CancellationToken cancellationToken = default) =>
        TryAsync("AddProject", async () =>
        {
            var userId = await EnsureUserIdAsync(cancellationToken).ConfigureAwait(false);
            var project = FavoriteProject.Create(userId, name, path, clock.UtcNow);
            await projects.AddAsync(project, cancellationToken).ConfigureAwait(false);
            await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success(Map(project));
        });

    public Task<Result<FavoriteProjectDto>> GetProjectByNameAsync(string name, CancellationToken cancellationToken = default) =>
        TryAsync("GetProjectByName", async () =>
        {
            var userId = await EnsureUserIdAsync(cancellationToken).ConfigureAwait(false);
            var items = await projects.GetForUserAsync(userId, cancellationToken).ConfigureAwait(false);
            var match = items.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));

            return match is null
                ? Result.Failure<FavoriteProjectDto>(
                    Error.NotFound("Projects.NameNotFound", $"No hay un proyecto favorito llamado '{name}'."))
                : Result.Success(Map(match));
        });

    public Task<Result<IReadOnlyList<FavoriteRouteDto>>> GetRoutesAsync(CancellationToken cancellationToken = default) =>
        TryAsync("GetRoutes", async () =>
        {
            var userId = await EnsureUserIdAsync(cancellationToken).ConfigureAwait(false);
            var items = await routes.GetForUserAsync(userId, cancellationToken).ConfigureAwait(false);
            IReadOnlyList<FavoriteRouteDto> dtos = items.Select(Map).ToArray();
            return Result.Success(dtos);
        });

    public Task<Result<FavoriteRouteDto>> AddRouteAsync(string alias, string path, CancellationToken cancellationToken = default) =>
        TryAsync("AddRoute", async () =>
        {
            var userId = await EnsureUserIdAsync(cancellationToken).ConfigureAwait(false);
            var existing = await routes.GetByAliasAsync(userId, alias, cancellationToken).ConfigureAwait(false);
            if (existing is not null)
            {
                return Result.Failure<FavoriteRouteDto>(RouteErrors.AliasTaken);
            }

            var route = FavoriteRoute.Create(userId, alias, path, clock.UtcNow);
            await routes.AddAsync(route, cancellationToken).ConfigureAwait(false);
            await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success(Map(route));
        });

    public Task<Result<string>> ResolveRouteAsync(string alias, CancellationToken cancellationToken = default) =>
        TryAsync("ResolveRoute", async () =>
        {
            var userId = await EnsureUserIdAsync(cancellationToken).ConfigureAwait(false);
            var route = await routes.GetByAliasAsync(userId, alias, cancellationToken).ConfigureAwait(false);
            return route is null
                ? Result.Failure<string>(RouteErrors.AliasNotFound(alias))
                : Result.Success(route.Path);
        });

    public Task<Result> RecordCommandAsync(
        string commandName,
        string rawInput,
        bool succeeded,
        string outcome,
        long durationMs,
        CancellationToken cancellationToken = default) =>
        TryAsync("RecordCommand", async () =>
        {
            var userId = await EnsureUserIdAsync(cancellationToken).ConfigureAwait(false);
            var entry = CommandHistoryEntry.Record(userId, commandName, rawInput, succeeded, outcome, durationMs, clock.UtcNow);
            await history.AddAsync(entry, cancellationToken).ConfigureAwait(false);
            await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success();
        });

    public Task<Result<IReadOnlyList<CommandHistoryDto>>> GetRecentCommandsAsync(int take, CancellationToken cancellationToken = default) =>
        TryAsync("GetRecentCommands", async () =>
        {
            var userId = await EnsureUserIdAsync(cancellationToken).ConfigureAwait(false);
            var items = await history.GetRecentAsync(userId, take, cancellationToken).ConfigureAwait(false);
            IReadOnlyList<CommandHistoryDto> dtos = items.Select(Map).ToArray();
            return Result.Success(dtos);
        });

    public Task<Result<ConversationDto>> StartConversationAsync(string title, CancellationToken cancellationToken = default) =>
        TryAsync("StartConversation", async () =>
        {
            var userId = await EnsureUserIdAsync(cancellationToken).ConfigureAwait(false);
            var conversation = Conversation.Start(userId, title, clock.UtcNow);
            await conversations.AddAsync(conversation, cancellationToken).ConfigureAwait(false);
            await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success(Map(conversation));
        });

    public Task<Result> AddMessageAsync(Guid conversationId, ConversationRole role, string content, CancellationToken cancellationToken = default) =>
        TryAsync("AddMessage", async () =>
        {
            var conversation = await conversations.GetByIdAsync(conversationId, cancellationToken).ConfigureAwait(false);
            if (conversation is null)
            {
                return Result.Failure(ConversationErrors.NotFound(conversationId));
            }

            conversation.AddMessage(role, content, clock.UtcNow);
            conversations.Update(conversation);
            await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return Result.Success();
        });

    public Task<Result<IReadOnlyList<ConversationDto>>> GetRecentConversationsAsync(int take, CancellationToken cancellationToken = default) =>
        TryAsync("GetRecentConversations", async () =>
        {
            var userId = await EnsureUserIdAsync(cancellationToken).ConfigureAwait(false);
            var items = await conversations.GetForUserAsync(userId, take, cancellationToken).ConfigureAwait(false);
            IReadOnlyList<ConversationDto> dtos = items.Select(Map).ToArray();
            return Result.Success(dtos);
        });

    private async Task<Guid> EnsureUserIdAsync(CancellationToken cancellationToken)
    {
        if (activeUser.HasUser)
        {
            return activeUser.CurrentUserId;
        }

        var existing = await users.GetAllAsync(cancellationToken).ConfigureAwait(false);
        OrionUser user;

        if (existing.Count > 0)
        {
            user = existing[0];
        }
        else
        {
            user = OrionUser.Create(DefaultUserName, DefaultCulture, clock.UtcNow);
            await users.AddAsync(user, cancellationToken).ConfigureAwait(false);
            await unitOfWork.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            logger.LogInformation("Se creó el usuario por defecto de ORION ({UserId}).", user.Id);
        }

        activeUser.SetUser(user.Id);
        return user.Id;
    }

    private async Task<Result<T>> TryAsync<T>(string operation, Func<Task<Result<T>>> action)
    {
        try
        {
            return await action().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fallo en la operación de memoria '{Operation}'.", operation);
            return Result.Failure<T>(Error.Unexpected($"Memory.{operation}", ex.Message));
        }
    }

    private async Task<Result> TryAsync(string operation, Func<Task<Result>> action)
    {
        try
        {
            return await action().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Fallo en la operación de memoria '{Operation}'.", operation);
            return Result.Failure(Error.Unexpected($"Memory.{operation}", ex.Message));
        }
    }

    private static FavoriteProjectDto Map(FavoriteProject p) => new(p.Id, p.Name, p.Path, p.LastOpenedOnUtc);

    private static FavoriteRouteDto Map(FavoriteRoute r) => new(r.Id, r.Alias, r.Path);

    private static CommandHistoryDto Map(CommandHistoryEntry e) =>
        new(e.CommandName, e.RawInput, e.Succeeded, e.Outcome, e.DurationMs, e.ExecutedOnUtc);

    private static ConversationDto Map(Conversation c) => new(c.Id, c.Title, c.CreatedOnUtc);
}
