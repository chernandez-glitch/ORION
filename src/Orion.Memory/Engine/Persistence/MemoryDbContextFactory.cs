using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Orion.Memory.Engine.Persistence;

/// <summary>Fábrica de diseño para las herramientas de EF Core (dotnet ef).</summary>
public sealed class MemoryDbContextFactory : IDesignTimeDbContextFactory<MemoryDbContext>
{
    public MemoryDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<MemoryDbContext>()
            .UseSqlite("Data Source=memory.design.db")
            .Options;

        return new MemoryDbContext(options);
    }
}
