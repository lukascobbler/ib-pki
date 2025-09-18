using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using SudoBox.BuildingBlocks.Infrastructure;

namespace SudoBox.UnifiedModule.Infrastructure.DbContext;

public sealed class UnifiedDbContextFactory : IDesignTimeDbContextFactory<UnifiedDbContext>
{
    public UnifiedDbContext CreateDbContext(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var basePath = Directory.GetCurrentDirectory();

        var cfg = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{env}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var schema = cfg["Database:Schema"] ?? "unified";
        var conn = DbConnectionStringBuilder.Build(schema);

        var options = new DbContextOptionsBuilder<UnifiedDbContext>()
            .UseNpgsql(conn, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", schema))
            .UseSnakeCaseNamingConvention()
            .Options;

        return new UnifiedDbContext(options, cfg);
    }
}
