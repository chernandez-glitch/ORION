using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Orion.Infrastructure.Persistence;

/// <summary>
/// Fábrica usada por las herramientas de EF Core (dotnet ef) en tiempo de
/// diseño para crear migraciones sin arrancar la aplicación.
/// </summary>
public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OrionDbContext>
{
    public OrionDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<OrionDbContext>()
            .UseSqlite("Data Source=orion.design.db")
            .Options;

        return new OrionDbContext(options);
    }
}
