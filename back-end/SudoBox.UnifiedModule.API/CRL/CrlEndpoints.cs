using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using SudoBox.UnifiedModule.Application.CRL.Contracts;
using SudoBox.UnifiedModule.Application.CRL.Features;

namespace SudoBox.UnifiedModule.API.CRL;

public static class CrlEndpoints
{
    public static void MapCrlEndpoints(this IEndpointRouteBuilder app)
    {
        var grp = app.MapGroup("/api/v1/crl");

        grp.MapGet("/get-all", async (CrlService crlService) => Results.Ok(await crlService.GetAll()))
            .AllowAnonymous();

        grp.MapPost("/revoke", 
            async (RevokeCertificateRequest revokeCertificateRequest, CrlService crlService) =>
        {
            try
            {
                await crlService.RevokeCertificate(revokeCertificateRequest);
                return Results.NoContent();
            } catch (Exception e) {
                return Results.BadRequest(e.Message);
            }
        }).RequireAuthorization();
    }
}
