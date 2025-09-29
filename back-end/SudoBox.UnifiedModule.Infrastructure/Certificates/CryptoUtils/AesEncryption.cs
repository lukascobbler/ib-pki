using System.Security.Cryptography;
using System.Text;

namespace SudoBox.UnifiedModule.Infrastructure.Certificates.CryptoUtils;

public static class AesEncryption {
    public static byte[] Encrypt(byte[] data, byte[] key, byte[] iv) {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        return aes.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
    }

    public static byte[] Decrypt(byte[] data, byte[] key, byte[] iv) {
        using var aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        return aes.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
    }

    public static string Encrypt(string plainText, byte[] key, byte[] iv) {
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var encryptedBytes = Encrypt(plainBytes, key, iv);
        return Convert.ToBase64String(iv.Concat(encryptedBytes).ToArray());
    }

    public static string Decrypt(string cipherBase64, byte[] key) {
        var allBytes = Convert.FromBase64String(cipherBase64);
        var iv = allBytes[..16];
        var cipherBytes = allBytes[16..];
        var plainBytes = Decrypt(cipherBytes, key, iv);
        return Encoding.UTF8.GetString(plainBytes);
    }
}
