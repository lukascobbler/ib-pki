using SudoBox.UnifiedModule.Application.CRL.Contracts;
using SudoBox.UnifiedModule.Application.Abstractions;
using SudoBox.UnifiedModule.Domain.Users;
using Org.BouncyCastle.Crypto.Operators;
using SudoBox.UnifiedModule.Domain.CRL;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace SudoBox.UnifiedModule.Application.CRL.Features;

public class CrlService(IUnifiedDbContext db) {
    public async Task<List<RevokedCertificateResponse>> GetAll() {
        var revokedCertificateModels = await db.RevokedCertificates
            .Include(rc => rc.Certificate)
            .ToListAsync();

        return revokedCertificateModels.Select(RevokedCertificateResponse.CreateDto).ToList();
    }

    public async Task RevokeCertificate(RevokeCertificateRequest revokeCertificateRequest, Guid requesterId, Role requesterRole) {
        var certificate = await db.Certificates
            .Include(c => c.SignedBy)
            .Where(c => c.SerialNumber == BigInteger.Parse(revokeCertificateRequest.SerialNumber))
            .FirstOrDefaultAsync() ?? throw new Exception("Certificate not found!");

        var revokedCertificate = await db.RevokedCertificates
            .Include(rc => rc.Certificate)
            .Where(rc => rc.Certificate.SerialNumber == BigInteger.Parse(revokeCertificateRequest.SerialNumber))
            .FirstOrDefaultAsync();

        if (revokedCertificate != null)
            throw new Exception("Certificate is already revoked!");

        if (requesterRole == Role.CaUser && certificate.SignedBy.Id != requesterId) {
            throw new Exception("A CA user can only revoke certificates signed by them!");
        } else if (requesterRole == Role.EeUser) {
            var user = await db.Users.Include(u => u.MyCertificates).Where(u => u.Id == requesterId).FirstOrDefaultAsync();
            if (!user!.MyCertificates.Contains(certificate))
                throw new Exception("An EE user can only revoke certificates requested by them!");
        }

        var newRevokedCertificate = new RevokedCertificate {
            Certificate = certificate,
            RevocationReason = revokeCertificateRequest.RevocationReason
        };

        db.RevokedCertificates.Add(newRevokedCertificate);
        await db.SaveChangesAsync();
    }

    public async Task<byte[]> GetRevocationFile() {
        // hardcoded main certificate that should sign the CRL
        var singingCertificate = await db.Certificates.FindAsync(BigInteger.Parse("325237371638983214349629308374620880901")); // todo generate certificate with serial number 0

        var issuerDn = new Org.BouncyCastle.Asn1.X509.X509Name(singingCertificate!.IssuedBy);
        var now = DateTime.UtcNow;

        var crlGen = new Org.BouncyCastle.X509.X509V2CrlGenerator();
        crlGen.SetIssuerDN(issuerDn);
        crlGen.SetThisUpdate(now);
        crlGen.SetNextUpdate(now.AddDays(7));

        var revokedCerts = await db.RevokedCertificates
            .Include(rc => rc.Certificate)
            .ToListAsync();

        foreach (var revokedCertificate in revokedCerts) {
            var serialNumber = new Org.BouncyCastle.Math.BigInteger(revokedCertificate.Certificate.SerialNumber.ToString(), 10);
            var reason = revokedCertificate.RevocationReason switch {
                RevocationReason.Unspecified => Org.BouncyCastle.Asn1.X509.CrlReason.Unspecified,
                RevocationReason.KeyCompromise => Org.BouncyCastle.Asn1.X509.CrlReason.KeyCompromise,
                RevocationReason.AffiliationChanged => Org.BouncyCastle.Asn1.X509.CrlReason.AffiliationChanged,
                RevocationReason.Superseded => Org.BouncyCastle.Asn1.X509.CrlReason.Superseded,
                RevocationReason.CessationOfOperation => Org.BouncyCastle.Asn1.X509.CrlReason.CessationOfOperation,
                RevocationReason.PrivilegeWithdrawn => Org.BouncyCastle.Asn1.X509.CrlReason.PrivilegeWithdrawn,
                _ => throw new ArgumentOutOfRangeException()
            };

            crlGen.AddCrlEntry(serialNumber, now, reason);
        }

        var sig = new Asn1SignatureFactory("SHA256WithRSA", singingCertificate.PrivateKey);
        var crl = crlGen.Generate(sig);

        return crl.GetEncoded();
    }
}