using Org.BouncyCastle.Crypto;
using System.Numerics;

namespace SudoBox.UnifiedModule.Domain.Certificates;

public class Certificate {
    public required BigInteger SerialNumber { get; set; }
    public required string IssuedBy { get; set; }
    public required string IssuedTo { get; set; }
    public required DateTime NotBefore { get; set; }
    public required DateTime NotAfter { get; set; }
    public string? EncodedValue { get; set; }
    public required bool IsApproved { get; set; }
    public required bool CanSign { get; set; }
    public required int PathLen { get; set; }
    public AsymmetricKeyParameter? PrivateKey { get; set; }
}