using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Orion.Domain.Projects;

namespace Orion.Infrastructure.Persistence.Configurations;

internal sealed class FavoriteProjectConfiguration : IEntityTypeConfiguration<FavoriteProject>
{
    public void Configure(EntityTypeBuilder<FavoriteProject> builder)
    {
        builder.ToTable("FavoriteProjects");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).HasMaxLength(128).IsRequired();
        builder.Property(p => p.Path).HasMaxLength(1024).IsRequired();
        builder.Property(p => p.CreatedOnUtc).IsRequired();
        builder.Property(p => p.UpdatedOnUtc).IsRequired();

        builder.HasIndex(p => p.UserId);
        builder.Ignore(p => p.DomainEvents);
    }
}
