using Org.BouncyCastle.Math;
using SudoBox.UnifiedModule.Domain.Certificates.ExtensionValues;

namespace SudoBox.UnifiedModule.Application.Certificates.Contracts;

public record CreateCertificateDto
{
    public required BigInteger SigningCertificateId { get; set; }
    public required string CommonName { get; set; }
    public required string Organization { get; set; }
    public required string OrganizationalUnit { get; set; }
    public required string Email { get; set; }
    public required string Country { get; set; }
    public DateTime? NotBefore { get; set; }
    public DateTime? NotAfter { get; set; }
    public ICollection<KeyUsageValue>? KeyUsage { get; set; }
    public AlternativeNames? SubjectAlternativeNames { get; set; }
    public AlternativeNames? IssuerAlternativeNames { get; set; }
    public AlternativeNames? NameConstraints { get; set; }
    public BasicConstraints? BasicConstraints { get; set; }
    public CertificatePolicies? CertificatePolicies { get; set; }
    public ICollection<ExtendedKeyUsageValue>? ExtendedKeyUsage { get; set; }
}