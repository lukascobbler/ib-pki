using SudoBox.UnifiedModule.Infrastructure.Certificates.KeyManagement;
using SudoBox.UnifiedModule.Infrastructure.Certificates.Interceptors;
using SudoBox.UnifiedModule.Application.Abstractions;
using SudoBox.UnifiedModule.Infrastructure.DbContext;
using Microsoft.Extensions.DependencyInjection;
using SudoBox.BuildingBlocks.Infrastructure;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace SudoBox.UnifiedModule.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        var schema = cfg["Database:Schema"] ?? "unified";
        var conn = DbConnectionStringBuilder.Build(schema);

        services.AddDbContext<UnifiedDbContext>((sp, opt) => {
            opt.UseNpgsql(conn, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", schema))
               .UseSnakeCaseNamingConvention()
               .AddInterceptors(
                    new PrivateKeyMaterializationInterceptor(() => sp.GetRequiredService<KeyManagementService>()),
                    new PrivateKeySaveInterceptor(() => sp.GetRequiredService<KeyManagementService>())
               );
        });

        services.AddMemoryCache();
        services.AddScoped(sp => {
            var cache = sp.GetRequiredService<IMemoryCache>();
            var cfg = sp.GetRequiredService<IConfiguration>();
            return new KeyManagementService(() => new UnifiedDbContextFactory().CreateDbContext([]), cache);
        });

        services.AddScoped<IUnifiedDbContext>(sp => sp.GetRequiredService<UnifiedDbContext>());

        return services;
    }
}