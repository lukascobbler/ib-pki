using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Security;
using SudoBox.UnifiedModule.Application.Certificates.Dtos;

namespace SudoBox.UnifiedModule.Application.Certificates.Features;

public class CertificateService
{
    public void CreateCertificate(CreateCertificateDto createCertificateDto)
    {
        AsymmetricCipherKeyPair subjectKeyPair;
        var kpGen = new RsaKeyPairGenerator();
        kpGen.Init(new KeyGenerationParameters(new SecureRandom(), 2048));
        subjectKeyPair = kpGen.GenerateKeyPair();
    }
}