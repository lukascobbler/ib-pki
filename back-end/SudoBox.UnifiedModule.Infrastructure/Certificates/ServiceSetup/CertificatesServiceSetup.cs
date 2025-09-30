using Microsoft.Extensions.DependencyInjection;
using SudoBox.UnifiedModule.Application.Certificates.Features;

namespace SudoBox.UnifiedModule.Infrastructure.Certificates.ServiceSetup;

public static class CertificatesServiceSetup
{
    public static IServiceCollection ConfigureCertificates(this IServiceCollection services)
    {
        services.AddScoped<CertificateService, CertificateService>();
        
        return services;
    }
}