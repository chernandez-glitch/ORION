using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orion.Domain.Users;

namespace Orion.Infrastructure.Persistence.Configurations;

internal sealed class OrionUserConfiguration : IEntityTypeConfiguration<OrionUser>
{
    public void Configure(EntityTypeBuilder<OrionUser> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.DisplayName).HasMaxLength(128).IsRequired();
        builder.Property(u => u.Culture).HasMaxLength(16).IsRequired();
        builder.Property(u => u.CreatedOnUtc).IsRequired();
        builder.Property(u => u.UpdatedOnUtc).IsRequired();

        builder.HasIndex(u => u.DisplayName).IsUnique();
        builder.Ignore(u => u.DomainEvents);
    }
}
