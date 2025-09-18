using Microsoft.AspNetCore.Http;
using SudoBox.UnifiedModule.Application.Certificates.Features;
using SudoBox.UnifiedModule.Application.Certificates.Contracts;

namespace SudoBox.UnifiedModule.API.Certificates;
using Microsoft.AspNetCore.Builder;

public static class CertificateEndpoints {
    public static void MapCertificateEndpoints(this WebApplication app)
    {
        app.MapPost("api/v1/certificates/issue",
            async (CreateCertificateDto createCertificateDto, CertificateService certificateService) =>
            {
                try
                {
                    var response= await certificateService.CreateCertificate(createCertificateDto);
                    return Results.Ok(response);
                }
                catch (Exception e)
                {
                    return Results.BadRequest(e.Message);
                }
            }).AllowAnonymous(); // .RequireAuthorization("Admin"); // todo or CA

        app.MapGet("api/v1/certificates/get-all", async (CertificateService certificateService) =>
        {
            var response = await certificateService.GetAllCertificates();
            return Results.Ok(response);
        }).AllowAnonymous();
        
        app.MapGet("api/v1/certificates/get-all-signing", async (CertificateService certificateService) =>
        {
            var response = await certificateService.GetAllSigningCertificates();
            return Results.Ok(response);
        }).AllowAnonymous();
    }
}