namespace SudoBox.UnifiedModule.Domain.Certificates.ExtensionValues;

public class CertificatePolicies
{
    public required string PolicyIdentifier { get; set; }
    public required string CpsUri { get; set; }
    public required string UserNotice { get; set; }
}