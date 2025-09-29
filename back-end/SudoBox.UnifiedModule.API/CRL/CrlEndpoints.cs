using SudoBox.UnifiedModule.Application.CRL.Contracts;
using SudoBox.UnifiedModule.Application.CRL.Features;
using SudoBox.UnifiedModule.Domain.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using static System.Enum;

namespace SudoBox.UnifiedModule.API.CRL;

public static class CrlEndpoints {
    public static void MapCrlEndpoints(this IEndpointRouteBuilder app) {
        var grp = app.MapGroup("/api/v1/crl");

        grp.MapGet("/web", async (CrlService crlService) => Results.Ok(await crlService.GetAll())).AllowAnonymous();

        grp.MapPost("/revoke", async (RevokeCertificateRequest revokeCertificateRequest, CrlService crlService, HttpContext httpContext) => {
            try {
                var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var role = httpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value!;
                Role parsedRole = Parse<Role>(role);

                await crlService.RevokeCertificate(revokeCertificateRequest, Guid.Parse(userId!), parsedRole);
                return Results.NoContent();
            } catch (Exception e) {
                return Results.BadRequest(e.Message);
            }
        }).RequireAuthorization();

        grp.MapGet("/", async (CrlService crlService) => {
            try {
                var revocationFileBytes = await crlService.GetRevocationFile();

                return Results.File(
                    fileContents: revocationFileBytes,
                    contentType: "application/pkix-crl",
                    fileDownloadName: "revoked_certs.crl"
                );
            } catch (Exception e) {
                return Results.BadRequest(e.Message);
            }
        }).AllowAnonymous();
    }
}
