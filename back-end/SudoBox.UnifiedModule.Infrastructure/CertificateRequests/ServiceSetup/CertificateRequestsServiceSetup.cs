using Microsoft.Extensions.DependencyInjection;
using SudoBox.UnifiedModule.Application.CertificateRequests.Features;

namespace SudoBox.UnifiedModule.Infrastructure.CertificateRequests.ServiceSetup;

public static class CertificateRequestsServiceSetup {
    public static IServiceCollection ConfigureCertificateRequests(this IServiceCollection services) {
        services.AddScoped<CertificateRequestService, CertificateRequestService>();
        return services;
    }
}