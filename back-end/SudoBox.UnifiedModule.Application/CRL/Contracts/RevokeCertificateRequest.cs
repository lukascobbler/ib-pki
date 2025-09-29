using SudoBox.UnifiedModule.Domain.CRL;

namespace SudoBox.UnifiedModule.Application.CRL.Contracts;

public record RevokeCertificateRequest
{
    public required string SerialNumber { get; set; }
    public required RevocationReason RevocationReason { get; set; }
}