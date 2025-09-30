using SudoBox.UnifiedModule.Domain.Certificates;
using System.Numerics;

namespace SudoBox.UnifiedModule.Domain.CRL;

public class RevokedCertificate {
    public required Certificate Certificate { get; set; }
    public required RevocationReason RevocationReason { get; set; }
    public BigInteger CertificateSerialNumber { get; set; }
}