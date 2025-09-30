using SudoBox.UnifiedModule.Infrastructure.Certificates.Interceptors;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Org.BouncyCastle.Crypto;
using System.Numerics;

namespace SudoBox.UnifiedModule.Infrastructure.Certificates.DomainDatabaseSetup;

public static class TypeConverters {
    public static readonly ValueConverter<BigInteger, string> BigIntConverter = new(
        v => v.ToString(),
        v => BigInteger.Parse(v)
    );

    public static readonly ValueConverter<AsymmetricKeyParameter?, string> KeyConverter = new(
        v => ToBase64EncryptedKey(v),
        v => FromBase64EncryptedKey(v)
    );

    private static string ToBase64EncryptedKey(AsymmetricKeyParameter? keyParam) {
        if (keyParam is not EncryptedKeyParameter encryptedKey)
            throw new InvalidCastException($"Expected {nameof(EncryptedKeyParameter)}, got {keyParam?.GetType().Name}");
        return Convert.ToBase64String(encryptedKey.IV.Concat(encryptedKey.EncryptedBytes).ToArray());
    }

    private static EncryptedKeyParameter FromBase64EncryptedKey(string base64) {
        var allBytes = Convert.FromBase64String(base64);
        var iv = allBytes[..16];
        var cipher = allBytes[16..];
        return new EncryptedKeyParameter(cipher, iv);
    }
}