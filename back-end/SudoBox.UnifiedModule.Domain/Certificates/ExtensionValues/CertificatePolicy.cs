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

    public static CertificatePolicy FromPolicyInformation(PolicyInformation policyInfo) {
        var cp = new CertificatePolicy {
            PolicyIdentifier = policyInfo.PolicyIdentifier.Id,
            CpsUri = string.Empty,
            UserNotice = string.Empty
        };

        if (policyInfo.PolicyQualifiers != null) {
            foreach (Asn1Encodable enc in policyInfo.PolicyQualifiers) {
                var seq = Asn1Sequence.GetInstance(enc);
                var qualifierId = DerObjectIdentifier.GetInstance(seq[0]);

                if (qualifierId.Equals(PolicyQualifierID.IdQtCps) && seq.Count > 1)
                    cp.CpsUri = DerIA5String.GetInstance(seq[1]).GetString();

                if (qualifierId.Equals(PolicyQualifierID.IdQtUnotice) && seq.Count > 1)
                    cp.UserNotice = DerUtf8String.GetInstance(seq[1]).GetString();
            }
        }

        return cp;
    }
}