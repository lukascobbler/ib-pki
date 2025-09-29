using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SudoBox.UnifiedModule.Application.CertificateRequests.Contracts;
using SudoBox.UnifiedModule.Application.CertificateRequests.Features;
using System.Security.Claims;

namespace SudoBox.UnifiedModule.API.CertificateRequests;

public static class CertificateRequestEndpoints {
    public static void MapCertificateRequestEndpoints(this WebApplication app) {
        var grp = app.MapGroup("/api/v1/certificate-requests");

        grp.MapPost("/form", async (HttpContext httpContext, CreateCertificateRequestDTO request, CertificateRequestService certificateRequestService) => {
            try {
                var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User must be logged in!");
                var keyPair = await certificateRequestService.CreateCertificateRequest(request, userId);
                return Results.Ok(keyPair);
            } catch (Exception e) {
                return Results.BadRequest(e.Message);
            }
        }).RequireAuthorization(new AuthorizeAttribute { Roles = "EeUser" });

        grp.MapPost("/csr", async (HttpRequest httpRequest, HttpContext httpContext, CertificateRequestService certificateRequestService) => {
            try {
                var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User must be logged in!");
                var form = await httpRequest.ReadFormAsync();

                if (!form.Files.Any(f => f.Name == "csrFile") || string.IsNullOrWhiteSpace(form["signingOrganization"]))
                    return Results.BadRequest("Missing required fields!");

                var csrFile = form.Files.GetFile("csrFile") ?? throw new Exception("Missing CSR file!");
                using var reader = new StreamReader(csrFile.OpenReadStream());
                string csrContent = await reader.ReadToEndAsync();

                var notAfter = DateTime.TryParse(form["notAfter"], out var dt) ? dt : (DateTime?)null;
                var signingOrganization = form["signingOrganization"].ToString();
                if (string.IsNullOrWhiteSpace(signingOrganization))
                    throw new Exception("Missing signingOrganization!");

                await certificateRequestService.CreateCertificateRequest(signingOrganization, csrContent, notAfter, userId);
                return Results.Ok();
            } catch (Exception e) {
                return Results.BadRequest(e.Message);
            }
        }).RequireAuthorization(new AuthorizeAttribute { Roles = "EeUser" });

        grp.MapGet("/", async (HttpContext httpContext, CertificateRequestService certificateRequestService) => {
            try {
                var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User must be logged in!");
                return Results.Ok(await certificateRequestService.GetCertificateRequests(userId));
            } catch (Exception e) {
                return Results.BadRequest(e.Message);
            }
        }).RequireAuthorization(new AuthorizeAttribute { Roles = "CaUser" });

        grp.MapPost("/reject", async (HttpContext httpContext, CertificateRequestService certificateRequestService, [FromBody] string requestId) => {
            try {
                var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User must be logged in!");
                await certificateRequestService.DeleteCertificateRequest(userId, requestId);
                return Results.Ok();
            } catch (Exception e) {
                return Results.BadRequest(e.Message);
            }
        }).RequireAuthorization(new AuthorizeAttribute { Roles = "CaUser" });

        grp.MapPost("/approve", async (HttpContext httpContext, CertificateRequestService certificateRequestService, ApproveCertificateRequest approveCertificateRequest) => {
            try {
                var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User must be logged in!");
                await certificateRequestService.ApproveCertificateRequest(userId, approveCertificateRequest);
                return Results.Ok();
            } catch (Exception e) {
                return Results.BadRequest(e.Message);
            }
        }).RequireAuthorization(new AuthorizeAttribute { Roles = "CaUser" });
    }
}