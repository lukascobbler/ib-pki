using System.Security.Cryptography;

namespace SudoBox.UnifiedModule.Application.Users.Utils.Auth;

public static class TokenUtils
{
    public static byte[] NewSecureRandom(int bytes = 32)
    {
        var b = new byte[bytes];
        RandomNumberGenerator.Fill(b);
        return b;
    }

    public static string Sha256Hex(ReadOnlySpan<byte> data)
    {
        Span<byte> hash = stackalloc byte[32];
        SHA256.HashData(data, hash);
        return Convert.ToHexString(hash);
    }
}
