namespace SudoBox.UnifiedModule.Application.Certificates.Contracts;

public record AddCertificateToCaUserRequest(string CaUserId, string NewCertificateSerialNumber);