using SudoBox.UnifiedModule.Domain.Users;
using SudoBox.BuildingBlocks.Domain;

namespace SudoBox.UnifiedModule.Domain.CertificateRequests;

public class CertificateRequest : Entity {
    public required string EncodedCSR { get; set; }
    public required User RequestedFor { get; set; }
    public required User RequestedFrom { get; set; }
    public required DateTime SubmittedOn { get; set; }
    public DateTime? NotBefore { get; set; }
    public DateTime? NotAfter { get; set; }
}