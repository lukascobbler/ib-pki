namespace SudoBox.UnifiedModule.Application.CertificateRequests.Contracts;

public record KeyPair {
    public required string PublicKey { get; set; }
    public required string PrivateKey { get; set; }
}