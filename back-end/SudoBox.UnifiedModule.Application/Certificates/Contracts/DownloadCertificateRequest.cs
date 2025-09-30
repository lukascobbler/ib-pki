namespace SudoBox.UnifiedModule.Application.Certificates.Contracts;

public record DownloadCertificateRequest(string CertificateSerialNumber, string Password);