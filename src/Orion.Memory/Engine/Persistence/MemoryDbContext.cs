using Microsoft.EntityFrameworkCore;
using Orion.Memory.Engine.Entities;

namespace Orion.Memory.Engine.Persistence;

/// <summary>
/// Contexto EF Core del Memory Engine (SQLite, base independiente
/// <c>memory.db</c>). Es la memoria permanente de ORION.
/// </summary>
public sealed class MemoryDbContext(DbContextOptions<MemoryDbContext> options) : DbContext(options)
{
    public DbSet<Session> Sessions => Set<Session>();

    public DbSet<Conversation> Conversations => Set<Conversation>();

    public DbSet<ConversationMessage> ConversationMessages => Set<ConversationMessage>();

    public DbSet<Project> Projects => Set<Project>();

    public DbSet<Workspace> Workspaces => Set<Workspace>();

    public DbSet<Favorite> Favorites => Set<Favorite>();

    public DbSet<History> History => Set<History>();

    public DbSet<CommandHistory> CommandHistory => Set<CommandHistory>();

    public DbSet<ApplicationHistory> ApplicationHistory => Set<ApplicationHistory>();

    public DbSet<FolderHistory> FolderHistory => Set<FolderHistory>();

    public DbSet<RecentFile> RecentFiles => Set<RecentFile>();

    public DbSet<Tag> Tags => Set<Tag>();

    public DbSet<MemoryItem> MemoryItems => Set<MemoryItem>();

    public DbSet<SettingSnapshot> SettingSnapshots => Set<SettingSnapshot>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Session>(e =>
        {
            e.Property(x => x.MachineName).HasMaxLength(128);
            e.Property(x => x.UserName).HasMaxLength(128);
            e.HasIndex(x => x.StartedOnUtc);
        });

        modelBuilder.Entity<Conversation>(e =>
        {
            e.Property(x => x.Title).HasMaxLength(256).IsRequired();
            e.HasMany(x => x.Messages).WithOne().HasForeignKey(m => m.ConversationId).OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.UpdatedOnUtc);
        });

        modelBuilder.Entity<ConversationMessage>(e =>
        {
            e.Property(x => x.Role).HasConversion<string>().HasMaxLength(16);
            e.Property(x => x.Content).IsRequired();
            e.HasIndex(x => x.ConversationId);
        });

        modelBuilder.Entity<Project>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(128).IsRequired();
            e.Property(x => x.Path).HasMaxLength(1024).IsRequired();
            e.HasIndex(x => x.Path).IsUnique();
        });

        modelBuilder.Entity<Workspace>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(128).IsRequired();
            e.Property(x => x.Path).HasMaxLength(1024);
        });

        modelBuilder.Entity<Favorite>(e =>
        {
            e.Property(x => x.Kind).HasConversion<string>().HasMaxLength(24);
            e.Property(x => x.Reference).HasMaxLength(1024).IsRequired();
            e.Property(x => x.Label).HasMaxLength(256);
            e.HasIndex(x => new { x.Kind, x.Reference }).IsUnique();
        });

        modelBuilder.Entity<History>(e =>
        {
            e.Property(x => x.Category).HasMaxLength(64);
            e.Property(x => x.Description).HasMaxLength(2048);
            e.HasIndex(x => x.OccurredOnUtc);
        });

        modelBuilder.Entity<CommandHistory>(e =>
        {
            e.Property(x => x.CommandId).HasMaxLength(128);
            e.Property(x => x.CommandName).HasMaxLength(128);
            e.Property(x => x.Parameters).HasMaxLength(2048);
            e.Property(x => x.Status).HasMaxLength(24);
            e.HasIndex(x => x.ExecutedOnUtc);
        });

        modelBuilder.Entity<ApplicationHistory>(e =>
        {
            e.Property(x => x.Application).HasMaxLength(256).IsRequired();
            e.Property(x => x.Path).HasMaxLength(1024);
            e.HasIndex(x => x.Application).IsUnique();
        });

        modelBuilder.Entity<FolderHistory>(e =>
        {
            e.Property(x => x.Path).HasMaxLength(1024).IsRequired();
            e.HasIndex(x => x.Path).IsUnique();
        });

        modelBuilder.Entity<RecentFile>(e =>
        {
            e.Property(x => x.Path).HasMaxLength(1024).IsRequired();
            e.Property(x => x.Name).HasMaxLength(256);
            e.HasIndex(x => x.AccessedOnUtc);
        });

        modelBuilder.Entity<Tag>(e =>
        {
            e.Property(x => x.Name).HasMaxLength(64).IsRequired();
            e.Property(x => x.Color).HasMaxLength(16);
            e.HasIndex(x => x.Name).IsUnique();
        });

        modelBuilder.Entity<MemoryItem>(e =>
        {
            e.Property(x => x.Type).HasConversion<string>().HasMaxLength(24);
            e.Property(x => x.Title).HasMaxLength(256);
            e.Property(x => x.Category).HasMaxLength(64);
            e.HasMany(x => x.Tags).WithMany(t => t.Items);
        });

        modelBuilder.Entity<SettingSnapshot>(e =>
        {
            e.Property(x => x.Label).HasMaxLength(128);
        });

        base.OnModelCreating(modelBuilder);
    }
}
