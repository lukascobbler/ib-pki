using System.Numerics;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;

namespace SudoBox.UnifiedModule.Infrastructure.Certificates.DomainDatabaseSetup;

public static class TypeConverters
{
    public static readonly ValueConverter BigIntConverter = new ValueConverter<BigInteger, string>(
        v => v.ToString(),
        v => BigInteger.Parse(v)
    );

    public static readonly ValueConverter KeyConverter = new ValueConverter<AsymmetricKeyParameter?, string?>(
        v => ToPem(v),
        v => FromPem(v)
    );
    
    private static string? ToPem(AsymmetricKeyParameter? key)
    {
        if (key == null) return null;
        using var sw = new StringWriter();
        var pw = new PemWriter(sw);
        pw.WriteObject(key);
        pw.Writer.Flush();
        return sw.ToString();
    }

    private static AsymmetricKeyParameter? FromPem(string? pem)
    {
        if (string.IsNullOrWhiteSpace(pem))
            return null;

        using var sr = new StringReader(pem);
        var pr = new PemReader(sr);
        var obj = pr.ReadObject();

        if (obj is AsymmetricCipherKeyPair keyPair)
        {
            return keyPair.Private;
        }
        
        throw new InvalidOperationException("Private key not stored correctly: " + obj?.GetType().Name);
    }
}