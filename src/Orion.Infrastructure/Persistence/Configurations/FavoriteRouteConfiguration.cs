using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orion.Domain.Routes;

namespace Orion.Infrastructure.Persistence.Configurations;

internal sealed class FavoriteRouteConfiguration : IEntityTypeConfiguration<FavoriteRoute>
{
    public void Configure(EntityTypeBuilder<FavoriteRoute> builder)
    {
        builder.ToTable("FavoriteRoutes");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Alias).HasMaxLength(64).IsRequired();
        builder.Property(r => r.Path).HasMaxLength(1024).IsRequired();
        builder.Property(r => r.CreatedOnUtc).IsRequired();
        builder.Property(r => r.UpdatedOnUtc).IsRequired();

        builder.HasIndex(r => new { r.UserId, r.Alias }).IsUnique();
        builder.Ignore(r => r.DomainEvents);
    }
}
