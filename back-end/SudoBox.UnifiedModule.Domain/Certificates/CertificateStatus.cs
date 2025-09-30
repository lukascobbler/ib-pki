namespace SudoBox.UnifiedModule.Domain.Certificates;

public enum CertificateStatus {
    Active,
    Dormant,
    Expired,
    Revoked,
    Circural,
    Invalid,
    Prohibited
}