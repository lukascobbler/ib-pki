using SudoBox.UnifiedModule.Domain.Certificates.ExtensionValues;

namespace SudoBox.UnifiedModule.Application.CertificateRequests.Contracts;

public record CertificateRequestResponse {
    public required string Id { get; set; }
    public required DateTime SubmittedOn { get; set; }
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
}