using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orion.Domain.Preferences;

namespace Orion.Infrastructure.Persistence.Configurations;

internal sealed class PreferenceConfiguration : IEntityTypeConfiguration<Preference>
{
    public void Configure(EntityTypeBuilder<Preference> builder)
    {
        builder.ToTable("Preferences");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Key).HasMaxLength(128).IsRequired();
        builder.Property(p => p.Value).IsRequired();
        builder.Property(p => p.CreatedOnUtc).IsRequired();
        builder.Property(p => p.UpdatedOnUtc).IsRequired();

        builder.HasIndex(p => new { p.UserId, p.Key }).IsUnique();
        builder.Ignore(p => p.DomainEvents);
    }
}
