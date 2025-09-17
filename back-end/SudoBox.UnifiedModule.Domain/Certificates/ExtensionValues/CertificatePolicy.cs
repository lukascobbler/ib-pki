using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;

namespace SudoBox.UnifiedModule.Domain.Certificates.ExtensionValues;

public class CertificatePolicy {
    public required string PolicyIdentifier { get; set; }
    public required string CpsUri { get; set; }
    public required string UserNotice { get; set; }

    public PolicyInformation ToPolicyInformation() {
        var policyId = new DerObjectIdentifier(PolicyIdentifier);
        Asn1EncodableVector qualifiers = [];

        if (!string.IsNullOrEmpty(CpsUri))
            qualifiers.Add(new DerSequence(PolicyQualifierID.IdQtCps, new DerIA5String(CpsUri)));

        if (!string.IsNullOrEmpty(UserNotice))
            qualifiers.Add(new DerSequence(PolicyQualifierID.IdQtUnotice, new DerUtf8String(UserNotice)));

        return qualifiers.Count > 0 ? new PolicyInformation(policyId, new DerSequence(qualifiers)) : new PolicyInformation(policyId);
    }
}