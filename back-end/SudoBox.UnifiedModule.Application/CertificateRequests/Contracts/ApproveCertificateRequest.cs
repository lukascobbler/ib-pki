using SudoBox.UnifiedModule.Application.Certificates.Contracts;

namespace SudoBox.UnifiedModule.Application.CertificateRequests.Contracts;

public record ApproveCertificateRequest {
    public required string RequestId { get; set; }
    public required IssueCertificateDTO RequestForm { get; set; }
}