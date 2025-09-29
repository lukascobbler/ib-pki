using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Math;
using SudoBox.UnifiedModule.Domain.Users;
using SudoBox.UnifiedModule.Domain.Certificates;
using SudoBox.UnifiedModule.Domain.Certificates.ExtensionValues;

namespace SudoBox.UnifiedModule.Application.Certificates.Utils;
using Contracts;

public static class CertificateBuilder {
    public static Certificate CreateCertificate(IssueCertificateRequest request, AsymmetricKeyParameter subjectPublicKey,
        AsymmetricKeyParameter? subjectPrivateKey, Certificate? issuerCertificate, User user) {
        var guidBytes = Guid.NewGuid().ToByteArray();
        var serialNumber = new System.Numerics.BigInteger(guidBytes, true, false);
        var subjectName = request.GetX509Name();
        var issuerName = issuerCertificate != null ? new X509Name(issuerCertificate.IssuedTo) : subjectName;

        var canSign = (request.KeyUsage?.Contains(KeyUsageValue.CertificateSigning) ?? false) && (request.BasicConstraints?.IsCa ?? false);
        var pathLen = request.BasicConstraints?.PathLen ?? 0;

        var certGen = new X509V3CertificateGenerator();
        certGen.SetSerialNumber(new BigInteger(1, serialNumber.ToByteArray()));
        certGen.SetSubjectDN(subjectName);
        certGen.SetIssuerDN(issuerName);

        certGen.SetNotBefore(request.NotBefore ?? issuerCertificate?.NotBefore ?? DateTime.UtcNow);
        certGen.SetNotAfter(request.NotAfter ?? issuerCertificate?.NotAfter ?? DateTime.MaxValue);

        certGen.SetPublicKey(subjectPublicKey);

        if (request.BasicConstraints != null) {
            var basicConstraintsValue = request.BasicConstraints.PathLen >= 0
                ? new BasicConstraints(request.BasicConstraints.PathLen)
                : new BasicConstraints(request.BasicConstraints.IsCa);
            certGen.AddExtension(X509Extensions.BasicConstraints, true, basicConstraintsValue);
        }

        if (request.KeyUsage != null && request.KeyUsage.Count != 0) {
            var usageBits = 0;
            foreach (var ku in request.KeyUsage)
                if (KeyUsageMap.TryGetValue(ku, out int value))
                    usageBits |= value;

            certGen.AddExtension(X509Extensions.KeyUsage, true, new KeyUsage(usageBits));
        }

        if (request.ExtendedKeyUsage != null && request.ExtendedKeyUsage.Any()) {
            var ekuOids = new List<DerObjectIdentifier>();
            foreach (var eku in request.ExtendedKeyUsage)
                if (ExtendedKeyUsageMap.TryGetValue(eku, out var oid))
                    ekuOids.Add(oid);

            certGen.AddExtension(X509Extensions.ExtendedKeyUsage, false, new ExtendedKeyUsage(ekuOids.ToArray()));
        }

        if (request.SubjectAlternativeNames != null) {
            var san = new GeneralNames(request.SubjectAlternativeNames.ToGeneralNames());
            certGen.AddExtension(X509Extensions.SubjectAlternativeName, false, san);
        }

        if (request.IssuerAlternativeNames != null) {
            var ian = new GeneralNames(request.IssuerAlternativeNames.ToGeneralNames());
            certGen.AddExtension(X509Extensions.IssuerAlternativeName, false, ian);
        }

        if (request.NameConstraints != null) {
            var permitted = request.NameConstraints.Permitted.ToGeneralSubtrees();
            var excluded = request.NameConstraints.Excluded.ToGeneralSubtrees();
            var nameConstraints = new NameConstraints(permitted.Count > 0 ? permitted : null, excluded.Count > 0 ? excluded : null);
            certGen.AddExtension(X509Extensions.NameConstraints, true, nameConstraints);
        }

        if (request.CertificatePolicy != null) {
            PolicyInformation policyInfo = request.CertificatePolicy.ToPolicyInformation();
            certGen.AddExtension(X509Extensions.CertificatePolicies, false, new DerSequence(policyInfo));
        }

        const string crlUrl = "https://localhost:8081/api/v1/crl/get-crl";
        var distPointName = new DistributionPointName(new GeneralNames(new GeneralName(GeneralName.UniformResourceIdentifier, crlUrl)));
        var distPoint = new DistributionPoint(distPointName, null, null);
        certGen.AddExtension(X509Extensions.CrlDistributionPoints, false, new DerSequence(distPoint));

        var signer = new Asn1SignatureFactory("SHA256WithRSA", issuerCertificate?.PrivateKey ?? subjectPrivateKey, new SecureRandom());
        var certificate = certGen.Generate(signer);

        return new Certificate {
            SerialNumber = serialNumber,
            SigningCertificate = issuerCertificate,
            IssuedBy = issuerName.ToString(),
            IssuedTo = subjectName.ToString(),
            NotAfter = certificate.NotAfter.ToUniversalTime(),
            NotBefore = certificate.NotBefore.ToUniversalTime(),
            EncodedValue = Convert.ToBase64String(certificate.GetEncoded()),
            PrivateKey = subjectPrivateKey,
            IsApproved = true,
            CanSign = canSign,
            PathLen = pathLen,
            SignedBy = user
        };
    }

    private static readonly Dictionary<KeyUsageValue, int> KeyUsageMap = new() {
        { KeyUsageValue.DigitalSignature, KeyUsage.DigitalSignature },
        { KeyUsageValue.NonRepudiation, KeyUsage.NonRepudiation },
        { KeyUsageValue.KeyEncipherment, KeyUsage.KeyEncipherment },
        { KeyUsageValue.DataEncipherment, KeyUsage.DataEncipherment },
        { KeyUsageValue.KeyAgreement, KeyUsage.KeyAgreement },
        { KeyUsageValue.CertificateSigning, KeyUsage.KeyCertSign },
        { KeyUsageValue.CrlSigning, KeyUsage.CrlSign },
        { KeyUsageValue.EncipherOnly, KeyUsage.EncipherOnly },
        { KeyUsageValue.DecipherOnly, KeyUsage.DecipherOnly }
    };

    private static readonly Dictionary<ExtendedKeyUsageValue, DerObjectIdentifier> ExtendedKeyUsageMap = new() {
        { ExtendedKeyUsageValue.ServerAuthentication, KeyPurposeID.id_kp_serverAuth },
        { ExtendedKeyUsageValue.ClientAuthentication, KeyPurposeID.id_kp_clientAuth },
        { ExtendedKeyUsageValue.CodeSigning, KeyPurposeID.id_kp_codeSigning },
        { ExtendedKeyUsageValue.EmailProtection, KeyPurposeID.id_kp_emailProtection },
        { ExtendedKeyUsageValue.IpSecEndSystem, KeyPurposeID.id_kp_ipsecEndSystem },
        { ExtendedKeyUsageValue.IpSecTunnel, KeyPurposeID.id_kp_ipsecTunnel },
        { ExtendedKeyUsageValue.IpSecUser, KeyPurposeID.id_kp_ipsecUser },
        { ExtendedKeyUsageValue.TimeStamping, KeyPurposeID.id_kp_timeStamping },
        { ExtendedKeyUsageValue.OcspSigning, KeyPurposeID.id_kp_OCSPSigning },
        { ExtendedKeyUsageValue.Dvcs, KeyPurposeID.id_kp_dvcs }
    };
}