using SudoBox.UnifiedModule.Infrastructure.Certificates.KeyManagement;
using SudoBox.UnifiedModule.Infrastructure.Certificates.Interceptors;
using SudoBox.UnifiedModule.Application.Abstractions;
using SudoBox.UnifiedModule.Infrastructure.DbContext;
using Microsoft.Extensions.DependencyInjection;
using SudoBox.BuildingBlocks.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace SudoBox.UnifiedModule.Infrastructure;

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg) {
        var schema = cfg["Database:Schema"] ?? "unified";
        var conn = DbConnectionStringBuilder.Build(schema);

        services.AddMemoryCache();
        services.AddScoped<KeyManagementService>();
        services.AddSingleton<PrivateKeySaveInterceptor>();
        services.AddSingleton<PrivateKeyMaterializationInterceptor>();

        services.AddDbContext<UnifiedDbContext>((sp, opt) => {
            var materializationInterceptor = sp.GetRequiredService<PrivateKeyMaterializationInterceptor>();
            var saveInterceptor = sp.GetRequiredService<PrivateKeySaveInterceptor>();
            opt.UseNpgsql(conn, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", schema))
               .UseSnakeCaseNamingConvention()
               .AddInterceptors(materializationInterceptor, saveInterceptor);
        });

        services.AddScoped<IUnifiedDbContext>(sp => sp.GetRequiredService<UnifiedDbContext>());

        return services;
    }
}