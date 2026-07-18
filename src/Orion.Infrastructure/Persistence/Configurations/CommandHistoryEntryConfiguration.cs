using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orion.Domain.History;

namespace Orion.Infrastructure.Persistence.Configurations;

internal sealed class CommandHistoryEntryConfiguration : IEntityTypeConfiguration<CommandHistoryEntry>
{
    public void Configure(EntityTypeBuilder<CommandHistoryEntry> builder)
    {
        builder.ToTable("CommandHistory");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.CommandName).HasMaxLength(128).IsRequired();
        builder.Property(e => e.RawInput).HasMaxLength(2048).IsRequired();
        builder.Property(e => e.Outcome).HasMaxLength(2048).IsRequired();
        builder.Property(e => e.Succeeded).IsRequired();
        builder.Property(e => e.DurationMs).IsRequired();
        builder.Property(e => e.ExecutedOnUtc).IsRequired();

        builder.HasIndex(e => new { e.UserId, e.ExecutedOnUtc });
        builder.Ignore(e => e.DomainEvents);
    }
}
