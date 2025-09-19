using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO.Pem;
using Org.BouncyCastle.X509;
using SudoBox.UnifiedModule.Application.Abstractions;
using SudoBox.UnifiedModule.Application.Certificates.Contracts;
using SudoBox.UnifiedModule.Domain.Certificates;
using System.Numerics;

namespace SudoBox.UnifiedModule.Application.Certificates.Features;

public class CertificateService(IUnifiedDbContext db) {
    private readonly IUnifiedDbContext _db = db;

    public async Task CreateCertificate(CreateCertificateDto createCertificateDto, bool isAdmin) {
        var kpGen = new RsaKeyPairGenerator();
        kpGen.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
        var subjectKeyPair = kpGen.GenerateKeyPair();

        Certificate? signingCertificate = null;
        if (createCertificateDto.SigningCertificate != "SelfSign") {
            var signingSerialNumber = BigInteger.Parse(createCertificateDto.SigningCertificate);
            signingCertificate = await _db.Certificates.FindAsync(signingSerialNumber);
            if (signingCertificate == null)
                throw new Exception("Signing certificate not found!");
        }
        if (!isAdmin && signingCertificate == null)
            throw new Exception("Only admin can issue self signing certificates!");
        if (!(signingCertificate?.CanSign ?? true))
            throw new Exception("Selected certificate can't be used for signing!");
        CertificateStatus? status = signingCertificate != null ? GetStatus(signingCertificate) : null;
        if (status != null && status != CertificateStatus.Active)
            throw new Exception($"Selected certificate is {status?.ToString().ToLower()}!");

        Certificate certificate = CertificateBuilder.CreateCertificate(createCertificateDto, subjectKeyPair, signingCertificate);

        await _db.Certificates.AddAsync(certificate);
        await _db.SaveChangesAsync();
    }

    public async Task<ICollection<CertificateDto>> GetAllCertificates() {
        var allCertificatesModels = await _db.Certificates.ToListAsync();

        return allCertificatesModels.Select(c =>
            CertificateDto.CreateDto(c, GetStatus(c).ToString(), GetDecryptedCertificate(c))
        ).ToList();
    }

    public async Task<ICollection<CertificateDto>> GetValidSigningCertificates() {
        var allSigningCertificates = await _db.Certificates.Where(c => c.CanSign).ToListAsync();
        var allValidCertificates = allSigningCertificates.Where(c => GetStatus(c) == CertificateStatus.Active);

        return allValidCertificates.Select(c =>
            CertificateDto.CreateDto(c, GetStatus(c).ToString(), GetDecryptedCertificate(c))
        ).ToList();
    }

    // todo: revoking chekup
    private static CertificateStatus GetStatus(Certificate certificate, Certificate? original = null) {
        if (certificate.SigningCertificate != null && !IsCertificateSignedBy(certificate.EncodedValue, certificate.SigningCertificate.EncodedValue))
            return CertificateStatus.Invalid;
        if (certificate.SerialNumber == original?.SerialNumber)
            return CertificateStatus.Circural;
        if (DateTime.UtcNow < certificate.NotBefore)
            return CertificateStatus.Dormant;
        if (DateTime.UtcNow > certificate.NotAfter)
            return CertificateStatus.Expired;
        if (certificate.SigningCertificate == null)
            return CertificateStatus.Active;
        return GetStatus(certificate.SigningCertificate, original ?? certificate);
    }

    public static bool IsCertificateSignedBy(string? certB64, string? issuerB64) {
        if (string.IsNullOrEmpty(certB64) || string.IsNullOrEmpty(issuerB64))
            return false;
        try {
            var parser = new X509CertificateParser();
            var cert = parser.ReadCertificate(Convert.FromBase64String(certB64));
            var issuer = parser.ReadCertificate(Convert.FromBase64String(issuerB64));
            cert.Verify(issuer.GetPublicKey());
            return true;
        } catch { return false; }
    }

    private static string GetDecryptedCertificate(Certificate certificate) {
        if (certificate.EncodedValue == null)
            return "Certificate is empty!";
        return ToPem(certificate.EncodedValue) ?? "Malformed certificate";
    }

    public static string? ToPem(string base64) {
        try {
            var bytes = Convert.FromBase64String(base64);
            using var sw = new StringWriter();
            new PemWriter(sw).WriteObject(new PemObject("CERTIFICATE", bytes));
            return sw.ToString();
        } catch { return null; }
    }
}