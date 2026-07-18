using Microsoft.EntityFrameworkCore;
using Orion.Domain.Automations;
using Orion.Domain.Common;
using Orion.Domain.Conversations;
using Orion.Domain.History;
using Orion.Domain.Preferences;
using Orion.Domain.Projects;
using Orion.Domain.Routes;
using Orion.Domain.Users;

namespace Orion.Infrastructure.Persistence;

/// <summary>
/// Contexto de EF Core que respalda el sistema de memoria de ORION sobre SQLite.
/// Implementa <see cref="IUnitOfWork"/> para que la capa de aplicación confirme
/// cambios sin conocer EF.
/// </summary>
public sealed class OrionDbContext(DbContextOptions<OrionDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<OrionUser> Users => Set<OrionUser>();

    public DbSet<Preference> Preferences => Set<Preference>();

    public DbSet<FavoriteProject> FavoriteProjects => Set<FavoriteProject>();

    public DbSet<FavoriteRoute> FavoriteRoutes => Set<FavoriteRoute>();

    public DbSet<Conversation> Conversations => Set<Conversation>();

    public DbSet<ConversationMessage> ConversationMessages => Set<ConversationMessage>();

    public DbSet<CommandHistoryEntry> CommandHistory => Set<CommandHistoryEntry>();

    public DbSet<AutomationDefinition> Automations => Set<AutomationDefinition>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrionDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
