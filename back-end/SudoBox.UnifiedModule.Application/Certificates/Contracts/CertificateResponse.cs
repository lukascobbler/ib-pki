using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.X509;
using SudoBox.UnifiedModule.Domain.Certificates;
using System.Numerics;

namespace SudoBox.UnifiedModule.Application.Certificates.Contracts;

public record CertificateResponse {
    public required string SerialNumber { get; set; }
    public required string PrettySerialNumber { get; set; }
    public required string IssuedBy { get; set; }
    public required string IssuedTo { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidUntil { get; set; }
    public required string Status { get; set; }
    public required string DecryptedCertificate { get; set; }
    public required bool CanSign { get; set; }
    public required int PathLen { get; set; }

    public static CertificateResponse CreateDto(Certificate c, string status, string decryptedCertificate) {
        return new CertificateResponse {
            SerialNumber = c.SerialNumber.ToString(),
            PrettySerialNumber = ConvertToHexDisplay(c.SerialNumber),
            IssuedBy = ExtractX509Values(c.IssuedBy),
            IssuedTo = ExtractX509Values(c.IssuedTo),
            ValidFrom = c.NotBefore,
            ValidUntil = c.NotAfter,
            Status = status,
            DecryptedCertificate = decryptedCertificate,
            CanSign = c.CanSign,
            PathLen = c.PathLen
        };
    }

    private static string ConvertToHexDisplay(BigInteger number) {
        byte[] bytes = number.ToByteArray();

        if (bytes.Length > 1 && bytes[^1] == 0) {
            bytes = bytes.Take(bytes.Length - 1).ToArray();
        }

        Array.Reverse(bytes);

        var desiredLength = 16;
        if (bytes.Length < desiredLength) {
            bytes = Enumerable.Repeat((byte)0x00, desiredLength - bytes.Length)
                .Concat(bytes)
                .ToArray();
        }

        return string.Join(":", bytes.Select(b => b.ToString("X2")));
    }

    private static string ExtractX509Values(string dn) {
        var x = new X509Name(dn);
        string? Get(DerObjectIdentifier oid) => x.GetValueList(oid).Cast<string>().FirstOrDefault();
        var lines = new[] {
            new[] { Get(X509Name.CN) },
            [Get(X509Name.O), Get(X509Name.OU)],
            [Get(X509Name.L), Get(X509Name.ST), Get(X509Name.C)]
        };
        return string.Join("\n", lines.Select(l => string.Join(", ", l.Where(v => !string.IsNullOrEmpty(v)))));
    }
}