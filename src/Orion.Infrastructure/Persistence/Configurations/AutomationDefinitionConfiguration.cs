using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orion.Domain.Automations;

namespace Orion.Infrastructure.Persistence.Configurations;

internal sealed class AutomationDefinitionConfiguration : IEntityTypeConfiguration<AutomationDefinition>
{
    public void Configure(EntityTypeBuilder<AutomationDefinition> builder)
    {
        builder.ToTable("Automations");
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name).HasMaxLength(128).IsRequired();
        builder.Property(a => a.Description).HasMaxLength(1024).IsRequired();
        builder.Property(a => a.Trigger).HasMaxLength(256).IsRequired();
        builder.Property(a => a.IsEnabled).IsRequired();
        builder.Property(a => a.CreatedOnUtc).IsRequired();
        builder.Property(a => a.UpdatedOnUtc).IsRequired();

        builder.HasIndex(a => a.UserId);
        builder.Ignore(a => a.DomainEvents);
    }
}
