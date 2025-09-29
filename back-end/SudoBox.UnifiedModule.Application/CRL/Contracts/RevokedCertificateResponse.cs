using SudoBox.UnifiedModule.Application.Certificates.Contracts;
using SudoBox.UnifiedModule.Domain.CRL;

namespace SudoBox.UnifiedModule.Application.CRL.Contracts;

public record RevokedCertificateResponse
{
    public required string SerialNumber { get; set; }
    public required string PrettySerialNumber { get; set; }
    public required string IssuedBy { get; set; }
    public required string IssuedTo { get; set; }
    public required string DecryptedCertificate { get; set; }
    public required RevocationReason RevocationReason { get; set; }

    public static RevokedCertificateResponse CreateDto(RevokedCertificate revokedCertificate)
    {
        return new RevokedCertificateResponse {
            SerialNumber = revokedCertificate.Certificate.SerialNumber.ToString(),
            PrettySerialNumber = CertificateResponse.ConvertToHexDisplay(revokedCertificate.Certificate.SerialNumber),
            IssuedBy = revokedCertificate.Certificate.IssuedBy,
            IssuedTo = revokedCertificate.Certificate.IssuedTo,
            DecryptedCertificate = revokedCertificate.Certificate.GetPem(),
            RevocationReason = revokedCertificate.RevocationReason
        };
    }
}