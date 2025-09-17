using Microsoft.AspNetCore.Http;
using SudoBox.UnifiedModule.Application.Certificates.Dtos;
using SudoBox.UnifiedModule.Application.Certificates.Features;

namespace SudoBox.UnifiedModule.API.Certificates;
using Microsoft.AspNetCore.Builder;

public static class CertificateEndpoints
{
    public static void MapCertificateEndpoints(this WebApplication app)
    {
        app.MapPost("api/v1/certificates/create-certificate", 
            (CreateCertificateDto createCertificateDto, CertificateService certificateService) =>
        {
            try
            {
                certificateService.CreateCertificate(createCertificateDto);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);    
            }
            
            return Results.Ok();
        });
    }
}