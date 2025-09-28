using System.Numerics;
using System.Security.Cryptography.Xml;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using SudoBox.UnifiedModule.Application.Abstractions;
using SudoBox.UnifiedModule.Application.CRL.Contracts;
using SudoBox.UnifiedModule.Domain.CRL;
using SudoBox.UnifiedModule.Domain.Users;

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

    public async Task RevokeCertificate(RevokeCertificateRequest revokeCertificateRequest, Guid requesterId, Role requesterRole)
    {
        var certificate = await db.Certificates
            .Include(c => c.SignedBy)
            .Where(c => c.SerialNumber == BigInteger.Parse(revokeCertificateRequest.SerialNumber))
            .FirstOrDefaultAsync();
        if (certificate == null)
            throw new Exception("Certificate not found!");

        var revokedCertificate = await db.RevokedCertificates
            .Include(rc => rc.Certificate)
            .Where(rc => rc.Certificate.SerialNumber == BigInteger.Parse(revokeCertificateRequest.SerialNumber))
            .FirstOrDefaultAsync();

        if (revokedCertificate != null)
            throw new Exception("Certificate is already revoked!");
        
        switch (requesterRole)
        {
            case Role.CaUser when certificate.SignedBy.Id != requesterId:
                throw new Exception("A CA user can only revoke certificates signed by them!");
            case Role.EeUser:
            {
                var user = await db.Users
                    .Include(u => u.MyCertificates)
                    .Where(u => u.Id == requesterId)
                    .FirstOrDefaultAsync();

                if (!user!.MyCertificates.Contains(certificate))
                {
                    throw new Exception("An EE user can only revoke certificates requested by them!");
                }
                
                break;
            }
        };

        var newRevokedCertificate = new RevokedCertificate
        {
            Certificate = certificate,
            RevocationReason = revokeCertificateRequest.RevocationReason
        };

        db.RevokedCertificates.Add(newRevokedCertificate);
        await db.SaveChangesAsync();
    }
}