using SudoBox.UnifiedModule.Infrastructure.Certificates.CryptoUtils;
using SudoBox.UnifiedModule.Domain.Certificates.KeyManagement;
using SudoBox.UnifiedModule.Infrastructure.DbContext;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace SudoBox.UnifiedModule.Infrastructure.Certificates.KeyManagement;

public class KeyManagementService(Func<UnifiedDbContext> dbFactory, IMemoryCache cache) {
    public byte[] GetUserKey(Guid userId) {
        return cache.GetOrCreate(userId, entry => {
            entry.Priority = CacheItemPriority.NeverRemove;
            using var db = dbFactory();
            var record = db.UserKeys.AsNoTracking().FirstOrDefault(x => x.UserId == userId);

            if (record == null) {
                var newKey = RandomNumberGenerator.GetBytes(32);
                var iv = RandomNumberGenerator.GetBytes(16);
                var encrypted = AesEncryption.Encrypt(newKey, GetMasterKey(), iv);

                db.UserKeys.Add(new UserKey {
                    UserId = userId,
                    EncryptedKey = Convert.ToBase64String(iv.Concat(encrypted).ToArray())
                });
                db.SaveChanges();

                return newKey;
            } else {
                var encryptedBytes = Convert.FromBase64String(record.EncryptedKey);
                var iv = encryptedBytes[..16];
                var ciphertext = encryptedBytes[16..];

                return AesEncryption.Decrypt(ciphertext, GetMasterKey(), iv);
            }
        })!;
    }

    private byte[] GetMasterKey(int timeoutSeconds = 60) {
        return cache.GetOrCreate("master_key", entry => {
            entry.Priority = CacheItemPriority.NeverRemove;

            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2? cert = null;

            var endTime = DateTime.UtcNow.AddSeconds(timeoutSeconds);
            string[] dots = [".", "..", "..."];
            int i = 0;
            while (DateTime.UtcNow < endTime) {
                cert = store.Certificates.OfType<X509Certificate2>().FirstOrDefault(c => c.Issuer.Contains("MUP Republike Srbije"));
                if (cert != null)
                    break;
                Console.Write($"\rWaiting for smart card{dots[i % dots.Length]}   ");
                i++;
                Thread.Sleep(500);
            }

            if (cert == null)
                throw new Exception($"Certificate not found after {timeoutSeconds} seconds.");

            var masterKey = dbFactory().MasterKey.AsNoTracking().SingleOrDefault() ?? throw new Exception("Unable to find master key in the database!");
            var decodedBytes = Convert.FromBase64String(masterKey.EncryptedKey);

            return cert.GetRSAPrivateKey()?.Decrypt(decodedBytes, RSAEncryptionPadding.OaepSHA256) ?? throw new Exception("Unable to decrypt master key!");
        })!;
    }
}