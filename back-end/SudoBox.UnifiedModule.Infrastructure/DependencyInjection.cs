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

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg) {
        var schema = cfg["Database:Schema"] ?? "unified";
        var conn = DbConnectionStringBuilder.Build(schema);

        services.AddMemoryCache();
        services.AddScoped(sp => new PrivateKeyMaterializationInterceptor(() => sp.GetRequiredService<KeyManagementService>()));
        services.AddScoped(sp => new PrivateKeySaveInterceptor(() => sp.GetRequiredService<KeyManagementService>()));

        services.AddDbContext<UnifiedDbContext>((sp, opt) => {
            opt.UseNpgsql(conn, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", schema))
               .UseSnakeCaseNamingConvention();
        });

        services.AddScoped<IUnifiedDbContext>(sp => sp.GetRequiredService<UnifiedDbContext>());
        services.AddScoped<KeyManagementService>(sp => {
            var cache = sp.GetRequiredService<IMemoryCache>();
            var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
            return new KeyManagementService(scopeFactory, cache);
        });

        return services;
    }
}