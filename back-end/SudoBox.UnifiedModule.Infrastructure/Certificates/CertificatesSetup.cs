namespace SudoBox.UnifiedModule.Infrastructure.Certificates;

using Microsoft.Extensions.DependencyInjection;
using SudoBox.UnifiedModule.Application.Certificates.Features;

public static class CertificatesSetup
{
    public static IServiceCollection ConfigureCertificates(this IServiceCollection services)
    {
        services.AddScoped<CertificateService, CertificateService>();
        
        return services;
    }
}