using SudoBox.UnifiedModule.Infrastructure.Certificates.KeyManagement;
using SudoBox.UnifiedModule.Infrastructure.Certificates.CryptoUtils;
using SudoBox.UnifiedModule.Domain.Certificates;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;

namespace SudoBox.UnifiedModule.Infrastructure.Certificates.Interceptors;
public class PrivateKeySaveInterceptor(Func<KeyManagementService> kmsFactory) : SaveChangesInterceptor {
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result) {
        EncryptKeys(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default) {
        EncryptKeys(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void EncryptKeys(Microsoft.EntityFrameworkCore.DbContext? context) {
        if (context == null)
            return;

        var keyCache = new Dictionary<Guid, byte[]>();
        var certs = context.ChangeTracker.Entries<Certificate>()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
            .Select(e => e.Entity);

        foreach (var cert in certs) {
            if (cert.PrivateKey != null) {
                if (!keyCache.TryGetValue(cert.SignedBy.Id, out var userKey)) {
                    userKey = kmsFactory().GetUserKey(cert.SignedBy.Id);
                    keyCache[cert.SignedBy.Id] = userKey;
                }

                var iv = RandomNumberGenerator.GetBytes(16);
                var keyBytes = PrivateKeyInfoFactory.CreatePrivateKeyInfo(cert.PrivateKey).GetEncoded();
                var encrypted = AesEncryption.Encrypt(keyBytes, userKey, iv);
                cert.PrivateKey = new EncryptedKeyParameter(encrypted, iv);
            }
        }
    }
}

public class EncryptedKeyParameter(byte[] encrypted, byte[] iv) : AsymmetricKeyParameter(true) {
    public byte[] EncryptedBytes { get; } = encrypted;
    public byte[] IV { get; } = iv;
}