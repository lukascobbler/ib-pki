using SudoBox.UnifiedModule.Domain.Certificates.ExtensionValues;
using SudoBox.UnifiedModule.Domain.CertificateRequests;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Pkcs;

namespace SudoBox.UnifiedModule.Application.CertificateRequests.Utils;
using Contracts;

public static class CertificateRequestDecoder {
    public static CertificateRequestResponse DecodeCertificateRequest(CertificateRequest request) {
        var csrBytes = Convert.FromBase64String(request.EncodedCSR);
        var csr = new Pkcs10CertificationRequest(csrBytes);

        var info = csr.GetCertificationRequestInfo();
        var x509Name = new X509Name(info.Subject.ToString());

        var dto = new CertificateRequestResponse {
            Id = request.Id.ToString(),
            CommonName = x509Name.GetValueList(X509Name.CN).Cast<string>().FirstOrDefault() ?? string.Empty,
            Organization = x509Name.GetValueList(X509Name.O).Cast<string>().FirstOrDefault() ?? string.Empty,
            OrganizationalUnit = x509Name.GetValueList(X509Name.OU).Cast<string>().FirstOrDefault() ?? string.Empty,
            Email = x509Name.GetValueList(X509Name.EmailAddress).Cast<string>().FirstOrDefault() ?? string.Empty,
            Country = x509Name.GetValueList(X509Name.C).Cast<string>().FirstOrDefault() ?? string.Empty,
            NotBefore = request.NotBefore,
            NotAfter = request.NotAfter,
            SubmittedOn = request.SubmittedOn
        };

        var extensionAttr = info.Attributes.Cast<Asn1Encodable>().Select(a => AttributePkcs.GetInstance(a))
            .FirstOrDefault(a => a.AttrType.Equals(PkcsObjectIdentifiers.Pkcs9AtExtensionRequest));

        if (extensionAttr != null) {
            var extensions = X509Extensions.GetInstance(extensionAttr.AttrValues[0]);

            var bcExt = extensions.GetExtension(X509Extensions.BasicConstraints);
            if (bcExt != null) {
                var bc = BasicConstraints.GetInstance(bcExt.GetParsedValue());
                dto.BasicConstraints = new BasicConstraintsValue { IsCa = bc.IsCA(), PathLen = bc.PathLenConstraint?.IntValue ?? -1 };
            }

            var kuExt = extensions.GetExtension(X509Extensions.KeyUsage);
            if (kuExt != null) {
                var ku = KeyUsage.GetInstance(kuExt.GetParsedValue());
                dto.KeyUsage = KeyUsageMap.Where(kvp => (ku.IntValue & kvp.Value) != 0).Select(kvp => kvp.Key).ToList();
            }

            var ekuExt = extensions.GetExtension(X509Extensions.ExtendedKeyUsage);
            if (ekuExt != null) {
                var eku = ExtendedKeyUsage.GetInstance(ekuExt.GetParsedValue());
                dto.ExtendedKeyUsage = ExtendedKeyUsageMap.Where(kvp => eku.HasKeyPurposeId(kvp.Value)).Select(kvp => kvp.Key).ToList();
            }

            var sanExt = extensions.GetExtension(X509Extensions.SubjectAlternativeName);
            if (sanExt != null)
                dto.SubjectAlternativeNames = GeneralNames.GetInstance(sanExt.GetParsedValue()).ToListOfNames();

            var ianExt = extensions.GetExtension(X509Extensions.IssuerAlternativeName);
            if (ianExt != null)
                dto.IssuerAlternativeNames = GeneralNames.GetInstance(ianExt.GetParsedValue()).ToListOfNames();

            var ncExt = extensions.GetExtension(X509Extensions.NameConstraints);
            if (ncExt != null)
                dto.NameConstraints = NameConstraints.GetInstance(ncExt.GetParsedValue()).ToNamesConstraintsValue();

            var cpExt = extensions.GetExtension(X509Extensions.CertificatePolicies);
            if (cpExt != null) {
                var seq = Asn1Sequence.GetInstance(cpExt.GetParsedValue());
                dto.CertificatePolicy = CertificatePolicy.FromPolicyInformation(PolicyInformation.GetInstance(seq[0]));
            }
        }

        return dto;
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

    private static readonly Dictionary<ExtendedKeyUsageValue, KeyPurposeID> ExtendedKeyUsageMap = new() {
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
