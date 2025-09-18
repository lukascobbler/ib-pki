using Microsoft.AspNetCore.Http;
using SudoBox.UnifiedModule.Application.Certificates.Features;
using SudoBox.UnifiedModule.Application.Certificates.Contracts;

namespace SudoBox.UnifiedModule.API.Certificates;
using Microsoft.AspNetCore.Builder;

public static class CertificateEndpoints {
    public static void MapCertificateEndpoints(this WebApplication app) {
        app.MapPost("api/v1/certificates/issue", (CreateCertificateDto createCertificateDto, CertificateService certificateService) => {
            try {
                var response = certificateService.CreateCertificate(createCertificateDto);
                return Results.Ok(response);
            } catch (Exception e) {
                return Results.BadRequest(e.Message);
            }
        });
    }
}