namespace SudoBox.UnifiedModule.Domain.Certificates.ExtensionValues;

public enum KeyUsageValue {
    DigitalSignature,
    NonRepudiation,
    KeyEncipherment,
    DataEncipherment,
    KeyAgreement,
    CertificateSigning,
    CrlSigning,
    EncipherOnly,
    DecipherOnly
}