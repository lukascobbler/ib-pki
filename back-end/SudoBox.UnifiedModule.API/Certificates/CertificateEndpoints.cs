using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using SudoBox.UnifiedModule.Application.Certificates.Contracts;
using SudoBox.UnifiedModule.Application.Certificates.Features;
using System.Security.Claims;
using System.Text;

namespace SudoBox.UnifiedModule.API.Certificates;

public static class CertificateEndpoints {
    public static void MapCertificateEndpoints(this WebApplication app) 
    {
        var grp = app.MapGroup("/api/v1/certificates");
        
        grp.MapPost("/issue", async
            (CreateCertificateRequest createCertificateRequest, CertificateService certificateService, HttpContext httpContext) => {
            try {
                var role = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
                var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                await certificateService.CreateCertificate(createCertificateRequest, role == "Admin", userId);
                return Results.Ok();
            } catch (Exception e) {
                return Results.BadRequest(e.Message);
            }
        }).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin,CaUser" });

        grp.MapGet("/get-all", async (CertificateService certificateService) => {
            var response = await certificateService.GetAllCertificates();
            return Results.Ok(response);
        }).AllowAnonymous();

        grp.MapGet("/get-all-valid-signing", async (CertificateService certificateService) => {
            try {
                var response = await certificateService.GetAllValidSigningCertificates();
                return Results.Ok(response);
            } catch (Exception e) {
                return Results.BadRequest(e.Message);
            }
        }).AllowAnonymous();
        
        grp.MapGet("/{caUserId}/get-valid-signing", async (string caUserId, CertificateService certificateService) => {
            try {
                var response = await certificateService.GetValidSigningCertificatesForCaUser(caUserId);
                return Results.Ok(response);
            } catch (Exception e) {
                return Results.BadRequest(e.Message);
            }
        }).RequireAuthorization("Admin");
        
        grp.MapPut("/add-certificate-to-ca-user", async (AddCertificateToCaUserRequest addCertificateToCaUserRequest, CertificateService certificateService) => {
            try {
                await certificateService.AddCertificateToCaUser(addCertificateToCaUserRequest);
                return Results.Ok();
            } catch (Exception e) {
                return Results.BadRequest(e.Message);
            }
        }).RequireAuthorization("Admin");

        grp.MapGet("/download/{id}", async (string id, CertificateService certificateService) => {
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