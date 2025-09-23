using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SudoBox.UnifiedModule.Application.Certificates.Contracts;
using SudoBox.UnifiedModule.Application.Certificates.Features;
using System.Security.Claims;
using System.Text;

namespace SudoBox.UnifiedModule.API.Certificates;

public static class CertificateEndpoints {
    public static void MapCertificateEndpoints(this WebApplication app) {
        app.MapPost("api/v1/certificates/issue", async (CreateCertificateDto createCertificateDto, CertificateService certificateService, HttpContext httpContext) => {
            try {
                var role = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                await certificateService.CreateCertificate(createCertificateDto, role == "Admin");
                return Results.Ok();
            } catch (Exception e) {
                return Results.BadRequest(e.Message);
            }
        }).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin,CaUser" });

        app.MapGet("api/v1/certificates/get-all", async (CertificateService certificateService) => {
            var response = await certificateService.GetAllCertificates();
            return Results.Ok(response);
        }).AllowAnonymous();

        app.MapGet("api/v1/certificates/get-valid-signing", async (CertificateService certificateService) => {
            var response = await certificateService.GetValidSigningCertificates();
            return Results.Ok(response);
        }).AllowAnonymous();

        app.MapGet("api/v1/certificates/download/{id}", async (string id, CertificateService certificateService) => {
            try {
                var pfxBytes = await certificateService.GetCertificateAsPkcs12(id);

                return Results.File(
                    fileContents: pfxBytes,
                    contentType: "application/x-pkcs12",
                    fileDownloadName: $"certificate_{id}.pfx"
                );
            } catch (Exception e) {
                return Results.BadRequest(e.Message);
            }
        }).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin,CaUser,EeUser" });
    }
}