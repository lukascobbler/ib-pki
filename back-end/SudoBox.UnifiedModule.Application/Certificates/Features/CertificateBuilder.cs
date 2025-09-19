namespace SudoBox.UnifiedModule.Application.Certificates.Features;

using Contracts;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.Math;
using SudoBox.UnifiedModule.Domain.Certificates;
using SudoBox.UnifiedModule.Domain.Certificates.ExtensionValues;
using System.Collections.Generic;
using System.Linq;

public static class CertificateBuilder {
    public static Certificate CreateCertificate(CreateCertificateDto dto, AsymmetricCipherKeyPair subjectKeyPair, Certificate? issuerCertificate) {
        var guidBytes = Guid.NewGuid().ToByteArray();
        var serialNumber = new System.Numerics.BigInteger(guidBytes, true, false);
        var subjectName = dto.GetX509Name();
        var issuerName = issuerCertificate != null ? new X509Name(issuerCertificate.IssuedBy) : subjectName;

        var canSign = (dto.KeyUsage?.Contains(KeyUsageValue.CertificateSigning) ?? false) && (dto.BasicConstraints?.IsCa ?? false);
        var pathLen = dto.BasicConstraints?.PathLen ?? 0;

        var certGen = new X509V3CertificateGenerator();
        certGen.SetSerialNumber(new BigInteger(1, serialNumber.ToByteArray()));
        certGen.SetSubjectDN(subjectName);
        certGen.SetIssuerDN(issuerName);

        if (dto.NotBefore.HasValue)
            certGen.SetNotBefore(dto.NotBefore.Value);
        else
            certGen.SetNotBefore(DateTime.UtcNow);

        if (dto.NotAfter.HasValue)
            certGen.SetNotAfter(dto.NotAfter.Value);
        else
            certGen.SetNotAfter(DateTime.MaxValue);

        certGen.SetPublicKey(subjectKeyPair.Public);

        if (dto.BasicConstraints != null) {
            var basicConstraintsValue = dto.BasicConstraints.PathLen >= 0
                ? new BasicConstraints(dto.BasicConstraints.PathLen)
                : new BasicConstraints(dto.BasicConstraints.IsCa);
            certGen.AddExtension(X509Extensions.BasicConstraints, true, basicConstraintsValue);
        }

        if (dto.KeyUsage != null && dto.KeyUsage.Count != 0) {
            var usageBits = 0;
            foreach (var ku in dto.KeyUsage)
                if (KeyUsageMap.TryGetValue(ku, out int value))
                    usageBits |= value;

            certGen.AddExtension(X509Extensions.KeyUsage, true, new KeyUsage(usageBits));
        }

        if (dto.ExtendedKeyUsage != null && dto.ExtendedKeyUsage.Any()) {
            var ekuOids = new List<DerObjectIdentifier>();
            foreach (var eku in dto.ExtendedKeyUsage)
                if (ExtendedKeyUsageMap.TryGetValue(eku, out var oid))
                    ekuOids.Add(oid);

            certGen.AddExtension(X509Extensions.ExtendedKeyUsage, false, new ExtendedKeyUsage(ekuOids.ToArray()));
        }

        if (dto.SubjectAlternativeNames != null) {
            var san = new GeneralNames(dto.SubjectAlternativeNames.ToGeneralNames());
            certGen.AddExtension(X509Extensions.SubjectAlternativeName, false, san);
        }

        if (dto.IssuerAlternativeNames != null) {
            var ian = new GeneralNames(dto.IssuerAlternativeNames.ToGeneralNames());
            certGen.AddExtension(X509Extensions.IssuerAlternativeName, false, ian);
        }

        if (dto.NameConstraints != null) {
            var permitted = dto.NameConstraints.Permitted.ToGeneralSubtrees();
            var excluded = dto.NameConstraints.Excluded.ToGeneralSubtrees();
            var nameConstraints = new NameConstraints(permitted.Count > 0 ? permitted : null, excluded.Count > 0 ? excluded : null);
            certGen.AddExtension(X509Extensions.NameConstraints, true, nameConstraints);
        }

        if (dto.CertificatePolicy != null) {
            PolicyInformation policyInfo = dto.CertificatePolicy.ToPolicyInformation();
            certGen.AddExtension(X509Extensions.CertificatePolicies, false, new DerSequence(policyInfo));
        }

        var signer = new Asn1SignatureFactory("SHA256WithRSA", issuerCertificate?.PrivateKey ?? subjectKeyPair.Private, new SecureRandom());
        var certificate = certGen.Generate(signer);

        return new Certificate {
            SerialNumber = serialNumber,
            SigningCertificate = issuerCertificate,
            IssuedBy = issuerName.ToString(),
            IssuedTo = subjectName.ToString(),
            NotAfter = certificate.NotAfter.ToUniversalTime(),
            NotBefore = certificate.NotBefore.ToUniversalTime(),
            EncodedValue = Convert.ToBase64String(certificate.GetEncoded()),
            PrivateKey = subjectKeyPair.Private,
            IsApproved = true,
            CanSign = canSign,
            PathLen = pathLen
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