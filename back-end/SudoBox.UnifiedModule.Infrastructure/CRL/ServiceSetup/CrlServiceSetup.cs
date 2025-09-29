using Microsoft.Extensions.DependencyInjection;
using SudoBox.UnifiedModule.Application.CRL.Features;

namespace SudoBox.UnifiedModule.Infrastructure.Crl.ServiceSetup;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCrlFeatures(this IServiceCollection services)
    {
        services.AddScoped<CrlService, CrlService>();
        return services;
    }
}
