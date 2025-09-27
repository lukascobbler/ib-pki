using Org.BouncyCastle.Crypto;
using System.Numerics;
using Org.BouncyCastle.Utilities.IO.Pem;
using SudoBox.UnifiedModule.Domain.Users;
using PemWriter = Org.BouncyCastle.OpenSsl.PemWriter;

namespace SudoBox.UnifiedModule.Domain.Certificates;

public class Certificate {
    public required BigInteger SerialNumber { get; set; }
    public required Certificate? SigningCertificate { get; set; }
    public required string IssuedBy { get; set; }
    public required string IssuedTo { get; set; }
    public required DateTime NotBefore { get; set; }
    public required DateTime NotAfter { get; set; }
    public string? EncodedValue { get; set; }
    public required bool IsApproved { get; set; }
    public required bool CanSign { get; set; }
    public required int PathLen { get; set; }
    public AsymmetricKeyParameter? PrivateKey { get; set; }
    public required User SignedBy { get; set; }
    
    public string GetPem() {
        if (EncodedValue == null)
            return "Certificate is empty!";
        return ToPem(EncodedValue) ?? "Malformed certificate";
    }

    private static string? ToPem(string base64) {
        try {
            var bytes = Convert.FromBase64String(base64);
            using var sw = new StringWriter();
            new PemWriter(sw).WriteObject(new PemObject("CERTIFICATE", bytes));
            return sw.ToString();
        } catch { return null; }
    }
}