using System.Numerics;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using SudoBox.UnifiedModule.Application.Abstractions;
using SudoBox.UnifiedModule.Application.CRL.Contracts;
using SudoBox.UnifiedModule.Domain.CRL;

namespace SudoBox.UnifiedModule.Application.CRL.Features;

public class CrlService(IUnifiedDbContext db)
{
    public async Task<List<RevokedCertificateResponse>> GetAll()
    {
        var revokedCertificateModels = await db.RevokedCertificates
            .Include(rc => rc.Certificate)
            .ToListAsync();
        
        return revokedCertificateModels.Select(RevokedCertificateResponse.CreateDto).ToList();
    }

    public async Task RevokeCertificate(RevokeCertificateRequest revokeCertificateRequest)
    {
        var certificate = await db.Certificates
            .FindAsync(BigInteger.Parse(revokeCertificateRequest.SerialNumber));
        if (certificate == null)
            throw new Exception("Certificate not found!");

        var revokedCertificate = await db.RevokedCertificates
            .Include(rc => rc.Certificate)
            .Where(rc => rc.Certificate.SerialNumber == BigInteger.Parse(revokeCertificateRequest.SerialNumber))
            .FirstOrDefaultAsync();

        if (revokedCertificate != null)
            throw new Exception("Certificate is already revoked!");

        var newRevokedCertificate = new RevokedCertificate
        {
            Certificate = certificate,
            RevocationReason = revokeCertificateRequest.RevocationReason
        };

        db.RevokedCertificates.Add(newRevokedCertificate);
        await db.SaveChangesAsync();
    }
}