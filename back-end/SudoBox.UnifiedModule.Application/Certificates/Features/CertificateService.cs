using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using SudoBox.UnifiedModule.Application.Certificates.Contracts;
using SudoBox.UnifiedModule.Domain.Certificates;

namespace SudoBox.UnifiedModule.Application.Certificates.Features;

public class CertificateService {
    public string? CreateCertificate(CreateCertificateDto createCertificateDto) {
        var kpGen = new RsaKeyPairGenerator();
        kpGen.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
        AsymmetricCipherKeyPair subjectKeyPair = kpGen.GenerateKeyPair();

        //Certificate signingCertificate = findCertificate(createCertificateDto.SigningCertificateId);

        Certificate certificate = CertificateBuilder.CreateCertificate(createCertificateDto, subjectKeyPair, null); // signingCertificate);

        // Save the certificate to the database or any storage

        return certificate.EncodedValue;
    }
}