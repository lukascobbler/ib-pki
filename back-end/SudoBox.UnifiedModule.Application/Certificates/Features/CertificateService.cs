using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.IO.Pem;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Asn1.Nist;
using SudoBox.UnifiedModule.Application.Abstractions;
using SudoBox.UnifiedModule.Application.Certificates.Contracts;
using SudoBox.UnifiedModule.Domain.Certificates;
using System.Numerics;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Pkcs;

namespace SudoBox.UnifiedModule.Application.Certificates.Features;

public class CertificateService(IUnifiedDbContext db) {

    public async Task CreateCertificate(CreateCertificateDto createCertificateDto, bool isAdmin) {
        var kpGen = new RsaKeyPairGenerator();
        kpGen.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
        var subjectKeyPair = kpGen.GenerateKeyPair();

        Certificate? signingCertificate = null;
        if (createCertificateDto.SigningCertificate != "SelfSign") {
            var signingSerialNumber = BigInteger.Parse(createCertificateDto.SigningCertificate);
            signingCertificate = await db.Certificates.FindAsync(signingSerialNumber);
            if (signingCertificate == null)
                throw new Exception("Signing certificate not found!");
        }
        if (!isAdmin && signingCertificate == null)
            throw new Exception("Only admin can issue self signing certificates!");
        if (!(signingCertificate?.CanSign ?? true))
            throw new Exception("Selected certificate can't be used for signing!");
        CertificateStatus? status = signingCertificate != null ? GetStatus(signingCertificate) : null;
        if (status != null && status != CertificateStatus.Active)
            throw new Exception($"Selected certificate is {status.ToString()!.ToLower()}!");
        if (signingCertificate != null && createCertificateDto.NotBefore < signingCertificate.NotBefore)
            throw new Exception("NotBefore cannot be earlier than the signing certificate's NotBefore!");
        if (signingCertificate != null && createCertificateDto.NotAfter > signingCertificate.NotAfter)
            throw new Exception("NotAfter cannot be later than the signing certificate's NotAfter!");
        if (createCertificateDto.NotBefore > createCertificateDto.NotAfter)
            throw new Exception("NotBefore cannot be later than the NotAfter!");

        Certificate certificate = CertificateBuilder.CreateCertificate(createCertificateDto, subjectKeyPair, signingCertificate);

        await db.Certificates.AddAsync(certificate);
        await db.SaveChangesAsync();
    }

    public async Task<ICollection<CertificateDto>> GetAllCertificates() {
        var allCertificatesModels = await db.Certificates.ToListAsync();

        return allCertificatesModels.Select(c =>
            CertificateDto.CreateDto(c, GetStatus(c).ToString(), GetDecryptedCertificate(c))
        ).ToList();
    }

    public async Task<ICollection<CertificateDto>> GetValidSigningCertificates() {
        var allSigningCertificates = await db.Certificates.Where(c => c.CanSign).ToListAsync();
        var allValidCertificates = allSigningCertificates.Where(c => GetStatus(c) == CertificateStatus.Active);

        return allValidCertificates.Select(c =>
            CertificateDto.CreateDto(c, GetStatus(c).ToString(), GetDecryptedCertificate(c))
        ).ToList();
    }

    // todo prevent anyone from downloading any certificate only by id
    public async Task<byte[]> GetCertificateAsPkcs12(string certificateId) {
        var signingSerialNumber = BigInteger.Parse(certificateId);
        var dbCertificate = await db.Certificates.FindAsync(signingSerialNumber);

        if (dbCertificate == null)
            throw new Exception("Signing certificate not found!");

        if (dbCertificate.EncodedValue == null || dbCertificate.PrivateKey == null)
            throw new Exception("Certificate does not have an encoded value!");

        var certBytes = Convert.FromBase64String(dbCertificate.EncodedValue);
        var certificate = new X509CertificateParser().ReadCertificate(certBytes);

        var builder = new Pkcs12StoreBuilder()
            .SetKeyAlgorithm(NistObjectIdentifiers.IdAes256Cbc, PkcsObjectIdentifiers.IdHmacWithSha256);
        var store = builder.Build();

        var alias = certificate.SubjectDN.ToString();
        var certEntry = new X509CertificateEntry(certificate);

        store.SetKeyEntry(alias, new AsymmetricKeyEntry(dbCertificate.PrivateKey), [certEntry]);

        await using var ms = new MemoryStream();
        store.Save(ms, "".ToCharArray(), new SecureRandom());

        return ms.ToArray();
    }

    private async Task<Certificate?> GetCertificate(BigInteger serialNumber) {
        return await db.Certificates.Include(c => c.SigningCertificate).FirstOrDefaultAsync(c => c.SerialNumber == serialNumber);
    }

    // todo prevent anyone from downloading any certificate only by id
    public async Task<string> GetCertificateChainAsPEM(string certificateId) {
        var chain = new List<X509Certificate>();
        var parser = new X509CertificateParser();
        var serialNumber = BigInteger.Parse(certificateId);
        var current = await GetCertificate(serialNumber) ?? throw new Exception("Certificate not found!");

        while (current != null) {
            if (string.IsNullOrWhiteSpace(current.EncodedValue))
                throw new Exception($"Certificate {current.SerialNumber} has no encoded value!");

            var bytes = Convert.FromBase64String(current.EncodedValue);
            chain.Add(parser.ReadCertificate(bytes));

            if (current.SigningCertificate != null)
                current = await GetCertificate(current.SigningCertificate.SerialNumber);
            else
                current = null;
        }

        chain.Reverse();
        using var sw = new StringWriter();
        var pemWriter = new PemWriter(sw);
        chain.ForEach(c => pemWriter.WriteObject(new PemObject("CERTIFICATE", c.GetEncoded())));
        pemWriter.Writer.Flush();
        return sw.ToString();
    }

    // todo: revoking checkup
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

    private static bool IsCertificateSignedBy(string? certB64, string? issuerB64) {
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

    private static string? ToPem(string base64) {
        try {
            var bytes = Convert.FromBase64String(base64);
            using var sw = new StringWriter();
            new PemWriter(sw).WriteObject(new PemObject("CERTIFICATE", bytes));
            return sw.ToString();
        } catch { return null; }
    }
}