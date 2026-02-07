using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace OpenMedSphere.Infrastructure.Persistence;

/// <summary>
/// Factory for creating <see cref="ApplicationDbContext"/> instances at design time (EF Core migrations).
/// </summary>
internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var dataSource = new Npgsql.NpgsqlDataSourceBuilder("Host=localhost;Database=openmedsphere-designtime")
            .EnableDynamicJson()
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(dataSource);

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}
