using SudoBox.UnifiedModule.Domain.Certificates.ExtensionValues;
using SudoBox.UnifiedModule.Domain.CertificateRequests;
using SudoBox.UnifiedModule.Domain.Users;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Pkcs;

namespace SudoBox.UnifiedModule.Application.CertificateRequests.Utils;
using Contracts;

public static class CertificateRequestBuilder {
    public static CertificateRequest CreateCertificateRequest(CreateCertificateRequestDTO request, AsymmetricCipherKeyPair keyPair, User requestedFrom, User requestedFor) {
        var extGen = new X509ExtensionsGenerator();

        var subjectName = request.GetX509Name();

        if (request.BasicConstraints != null) {
            var bcValue = request.BasicConstraints.IsCa ? new BasicConstraints(request.BasicConstraints.PathLen ?? 0) : new BasicConstraints(false);
            extGen.AddExtension(X509Extensions.BasicConstraints, true, bcValue);
        }

        if (request.KeyUsage != null && request.KeyUsage.Count > 0) {
            int usageBits = 0;
            foreach (var ku in request.KeyUsage)
                if (KeyUsageMap.TryGetValue(ku, out int value))
                    usageBits |= value;

            extGen.AddExtension(X509Extensions.KeyUsage, true, new KeyUsage(usageBits));
        }

        if (request.ExtendedKeyUsage != null && request.ExtendedKeyUsage.Count != 0) {
            var ekuOids = new List<DerObjectIdentifier>();
            foreach (var eku in request.ExtendedKeyUsage)
                if (ExtendedKeyUsageMap.TryGetValue(eku, out var oid))
                    ekuOids.Add(oid);

            extGen.AddExtension(X509Extensions.ExtendedKeyUsage, false, new ExtendedKeyUsage(ekuOids.ToArray()));
        }

        if (request.SubjectAlternativeNames != null) {
            var san = new GeneralNames(request.SubjectAlternativeNames.ToGeneralNames());
            extGen.AddExtension(X509Extensions.SubjectAlternativeName, false, san);
        }

        if (request.IssuerAlternativeNames != null) {
            var ian = new GeneralNames(request.IssuerAlternativeNames.ToGeneralNames());
            extGen.AddExtension(X509Extensions.IssuerAlternativeName, false, ian);
        }

        if (request.NameConstraints != null) {
            var permitted = request.NameConstraints.Permitted.ToGeneralSubtrees();
            var excluded = request.NameConstraints.Excluded.ToGeneralSubtrees();
            var nameConstraints = new NameConstraints(permitted.Count > 0 ? permitted : null, excluded.Count > 0 ? excluded : null);
            extGen.AddExtension(X509Extensions.NameConstraints, true, nameConstraints);
        }

        if (request.CertificatePolicy != null) {
            PolicyInformation policyInfo = request.CertificatePolicy.ToPolicyInformation();
            extGen.AddExtension(X509Extensions.CertificatePolicies, false, new DerSequence(policyInfo));
        }

        var extAttr = new AttributePkcs(PkcsObjectIdentifiers.Pkcs9AtExtensionRequest, new DerSet(extGen.Generate()));
        var csr = new Pkcs10CertificationRequest("SHA256WithRSA", subjectName, keyPair.Public, new DerSet(extAttr), keyPair.Private);

        return new CertificateRequest {
            EncodedCSR = Convert.ToBase64String(csr.GetEncoded()),
            RequestedFor = requestedFor,
            RequestedFrom = requestedFrom,
            NotBefore = request.NotBefore,
            NotAfter = request.NotAfter,
            SubmittedOn = DateTime.UtcNow
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