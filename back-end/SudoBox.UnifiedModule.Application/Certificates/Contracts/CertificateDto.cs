using System.Numerics;
using Org.BouncyCastle.Asn1.X509;
using SudoBox.UnifiedModule.Domain.Certificates;

namespace SudoBox.UnifiedModule.Application.Certificates.Contracts;

public record CertificateDto {
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

    public static CertificateDto CreateDto(Certificate c, string status, string decryptedCertificate) {
        return new CertificateDto {
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

    private static string ConvertToHexDisplay(BigInteger number)
    {
        byte[] bytes = number.ToByteArray();

        if (bytes.Length > 1 && bytes[^1] == 0)
        {
            bytes = bytes.Take(bytes.Length - 1).ToArray();
        }

        Array.Reverse(bytes);

        var desiredLength = 16;
        if (bytes.Length < desiredLength)
        {
            bytes = Enumerable.Repeat((byte)0x00, desiredLength - bytes.Length)
                .Concat(bytes)
                .ToArray();
        }

        return string.Join(":", bytes.Select(b => b.ToString("X2")));
    }

    private static string ExtractX509Values(string distinguishedName) {
        var values = new X509Name(distinguishedName).GetValueList();
        return string.Join(", ", values.Cast<string>());
    }
}