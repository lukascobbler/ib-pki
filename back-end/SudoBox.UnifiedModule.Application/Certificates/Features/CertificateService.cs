using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using SudoBox.UnifiedModule.Application.Abstractions;
using SudoBox.UnifiedModule.Application.Certificates.Contracts;
using SudoBox.UnifiedModule.Domain.Certificates;

namespace SudoBox.UnifiedModule.Application.Certificates.Features;

public class CertificateService {
    private readonly IUnifiedDbContext _db;
    public CertificateService(IUnifiedDbContext db)
    {
        _db = db;
    }
    
    public string? CreateCertificate(CreateCertificateDto createCertificateDto) {
        var kpGen = new RsaKeyPairGenerator();
        kpGen.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
        AsymmetricCipherKeyPair subjectKeyPair = kpGen.GenerateKeyPair();

        //Certificate signingCertificate = findCertificate(createCertificateDto.SigningCertificateId);

        Certificate certificate = CertificateBuilder.CreateCertificate(createCertificateDto, subjectKeyPair, null); // signingCertificate);

        // Save the certificate to the database or any storage

        return certificate.EncodedValue;
    }

    public async Task<ICollection<CertificateDto>> GetAllCertificates()
    {
        var allCertificatesModels = await _db.Certificates.ToListAsync();

        return allCertificatesModels.Select(c => 
            CertificateDto.CreateDto(c, GetStatus(c), GetDecryptedCertificate(c))
        ).ToList();
    }

    public async Task<ICollection<CertificateDto>> GetAllSigningCertificates()
    {
        var allCertificatesModels = await _db.Certificates
            .Where(c => c.CanSign)
            .ToListAsync();

        return allCertificatesModels.Select(c => 
            CertificateDto.CreateDto(c, GetStatus(c), GetDecryptedCertificate(c))
        ).ToList();
    }

    private string GetStatus(Certificate certificate)
    {
        return "Active";
    }

    private string GetDecryptedCertificate(Certificate certificate)
    {
        return "Mocked for now";
    }
}