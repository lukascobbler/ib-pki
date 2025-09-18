using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SudoBox.BuildingBlocks.Infrastructure;
using SudoBox.UnifiedModule.Application.Abstractions;

namespace SudoBox.UnifiedModule.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        var schema = cfg["Database:Schema"] ?? "unified";
        var conn = DbConnectionStringBuilder.Build(schema);

        services.AddDbContext<UnifiedDbContext>(opt =>
            opt.UseNpgsql(conn, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", schema))
               .UseSnakeCaseNamingConvention());

        services.AddScoped<IUnifiedDbContext>(sp => sp.GetRequiredService<UnifiedDbContext>());

        return services;
    }
}