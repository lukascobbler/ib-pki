using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math;
using SudoBox.UnifiedModule.Domain.Certificates.ExtensionValues;

namespace SudoBox.UnifiedModule.Application.Certificates.Contracts;

public record CreateCertificateDto {
    public required string SigningCertificateId { get; set; }
    public required string CommonName { get; set; }
    public required string Organization { get; set; }
    public required string OrganizationalUnit { get; set; }
    public required string Email { get; set; }
    public required string Country { get; set; }
    public DateTime? NotBefore { get; set; }
    public DateTime? NotAfter { get; set; }
    public ICollection<KeyUsageValue>? KeyUsage { get; set; }
    public ICollection<ExtendedKeyUsageValue>? ExtendedKeyUsage { get; set; }
    public ListOfNames? SubjectAlternativeNames { get; set; }
    public ListOfNames? IssuerAlternativeNames { get; set; }
    public NamesConstraintsValue? NameConstraints { get; set; }
    public BasicConstraintsValue? BasicConstraints { get; set; }
    public CertificatePolicy? CertificatePolicy { get; set; }

    public X509Name GetX509Name() {
        var attrs = new List<DerObjectIdentifier> { X509Name.CN };
        var values = new List<string> { CommonName };

        var optionalFields = new (DerObjectIdentifier oid, string value)[] {
            (X509Name.O, Organization),
            (X509Name.OU, OrganizationalUnit),
            (X509Name.EmailAddress, Email),
            (X509Name.C, Country)
        };

        foreach (var (oid, val) in optionalFields) {
            if (!string.IsNullOrEmpty(val)) {
                values.Add(val);
                attrs.Add(oid);
            }
        }

        return new X509Name(attrs, values);
    }
}