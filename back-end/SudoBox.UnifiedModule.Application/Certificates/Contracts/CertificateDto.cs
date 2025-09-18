using SudoBox.UnifiedModule.Domain.Certificates;

namespace SudoBox.UnifiedModule.Application.Certificates.Contracts;

public record CertificateDto
{
    public required string SerialNumber { get; set; }
    public required string IssuedBy { get; set; }
    public required string IssuedTo { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public required string Status { get; set; }
    public required string DecryptedCertificate { get; set; }
    public required bool CanSign { get; set; }
    public required int PathLen { get; set; }

    public static CertificateDto CreateDto(Certificate c, string status, string decryptedCertificate)
    {
        return new CertificateDto
        {
            SerialNumber = c.SerialNumber.ToString(),
            IssuedBy = c.IssuedBy,
            IssuedTo = c.IssuedTo,
            ValidFrom = c.NotBefore,
            ValidUntil = c.NotAfter,
            Status = status,
            DecryptedCertificate = decryptedCertificate,
            CanSign = c.CanSign,
            PathLen = c.PathLen
        };
    }
}