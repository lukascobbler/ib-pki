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
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Pkcs;

namespace SudoBox.UnifiedModule.Application.Certificates.Features;

public class CertificateService(IUnifiedDbContext db) {

    public async Task CreateCertificate(CreateCertificateRequest createCertificateRequest, bool isAdmin, string? userId) {
        if (userId == null) 
            throw new Exception("User must be logged in!");
        var user = await db.Users.FindAsync(Guid.Parse(userId));
        if (user == null)
            throw new Exception("User not found!");
        
        var kpGen = new RsaKeyPairGenerator();
        kpGen.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
        var subjectKeyPair = kpGen.GenerateKeyPair();

        Certificate? signingCertificate = null;
        if (createCertificateRequest.SigningCertificate != "SelfSign") {
            var signingSerialNumber = BigInteger.Parse(createCertificateRequest.SigningCertificate);
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
        if (signingCertificate != null && createCertificateRequest.NotBefore < signingCertificate.NotBefore)
            throw new Exception("NotBefore cannot be earlier than the signing certificate's NotBefore!");
        if (signingCertificate != null && createCertificateRequest.NotAfter > signingCertificate.NotAfter)
            throw new Exception("NotAfter cannot be later than the signing certificate's NotAfter!");
        if (createCertificateRequest.NotBefore > createCertificateRequest.NotAfter)
            throw new Exception("NotBefore cannot be later than the NotAfter!");
        
        Certificate certificate = CertificateBuilder.CreateCertificate(createCertificateRequest, subjectKeyPair, signingCertificate, user);

        await db.Certificates.AddAsync(certificate);
        await db.SaveChangesAsync();
    }

    public async Task<ICollection<CertificateResponse>> GetAllCertificates() {
        var allCertificatesModels = await db.Certificates.ToListAsync();

        return allCertificatesModels.Select(c =>
            CertificateResponse.CreateDto(c, GetStatus(c).ToString(), GetDecryptedCertificate(c))
        ).ToList();
    }

    public async Task<ICollection<CertificateResponse>> GetAllValidSigningCertificates()
    {
        var allSigningCertificates = await db.Certificates
            .Where(c => c.CanSign)
            .ToListAsync();
        var allValidCertificates = allSigningCertificates.Where(c => GetStatus(c) == CertificateStatus.Active);

        return allValidCertificates.Select(c =>
            CertificateResponse.CreateDto(c, GetStatus(c).ToString(), GetDecryptedCertificate(c))
        ).ToList();
    }

    public async Task<ICollection<CertificateResponse>> GetValidSigningCertificatesForCaUser(string caUserId)
    {
        var user = await db.Users
            .Where(u => u.Id == Guid.Parse(caUserId))
            .Include(u => u.MyCertificates)
            .FirstOrDefaultAsync();
        if (user == null)
            throw new Exception("User not found!");

        var allSigningCertificates = await db.Certificates
            .Include(c => c.SignedBy)
            .Where(c => c.CanSign)
            .ToListAsync();

        var allSigningAndNotByUser = allSigningCertificates.Where(c => !user.MyCertificates.Contains(c));
        
        var allValidCertificates = allSigningAndNotByUser.Where(c => GetStatus(c) == CertificateStatus.Active);

        return allValidCertificates.Select(c =>
            CertificateResponse.CreateDto(c, GetStatus(c).ToString(), GetDecryptedCertificate(c))
        ).ToList();
    }

    public async Task AddCertificateToCaUser(AddCertificateToCaUserRequest addCertificateToCaUserRequest)
    {
        var user = await db.Users
            .Where(u => u.Id == Guid.Parse(addCertificateToCaUserRequest.CaUserId))
            .Include(u => u.MyCertificates)
            .FirstOrDefaultAsync();
        if (user == null)
            throw new Exception("User not found!");

        var certificate = await db.Certificates
            .FindAsync(BigInteger.Parse(addCertificateToCaUserRequest.NewCertificateSerialNumber));
        if (certificate == null)
            throw new Exception("Certificate not found!");
        
        user.MyCertificates.Add(certificate);
        await db.SaveChangesAsync();
    }

    private async Task<Certificate?> GetCertificate(BigInteger serialNumber) {
        return await db.Certificates.Include(c => c.SigningCertificate).FirstOrDefaultAsync(c => c.SerialNumber == serialNumber);
    }

    // todo prevent someone from downloading any certificate
    public async Task<byte[]> GetCertificateAsPkcs12(string certificateId) {
        var chain = new List<X509Certificate>();
        var parser = new X509CertificateParser();
        var serialNumber = BigInteger.Parse(certificateId);
        var eeCertificate = await GetCertificate(serialNumber) ?? throw new Exception("Certificate not found!");
        var current = eeCertificate;

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

        var alias = chain[0].SubjectDN.ToString();
        var store = new Pkcs12StoreBuilder().Build();
        var eeCertEntry = new X509CertificateEntry(chain[0]);
        var intermediateEntries = chain.Skip(1).Select(c => new X509CertificateEntry(c)).ToArray();

        if (eeCertificate.PrivateKey != null) {
            store.SetKeyEntry(alias, new AsymmetricKeyEntry(eeCertificate.PrivateKey), [eeCertEntry, .. intermediateEntries]);
        } else {
            store.SetCertificateEntry(alias, eeCertEntry);
            for (var i = 0; i < intermediateEntries.Length; i++)
                store.SetCertificateEntry($"{alias}-chain-{i}", intermediateEntries[i]);
        }

        var password = "change-me"; // TODO
        await using var ms = new MemoryStream();
        store.Save(ms, password.ToCharArray(), new SecureRandom());
        return ms.ToArray();
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