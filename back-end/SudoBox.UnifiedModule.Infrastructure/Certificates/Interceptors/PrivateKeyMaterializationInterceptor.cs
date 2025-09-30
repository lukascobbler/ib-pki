using SudoBox.UnifiedModule.Infrastructure.Certificates.KeyManagement;
using SudoBox.UnifiedModule.Infrastructure.Certificates.CryptoUtils;
using SudoBox.UnifiedModule.Domain.Certificates;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Security;

namespace SudoBox.UnifiedModule.Infrastructure.Certificates.Interceptors;

public class PrivateKeyMaterializationInterceptor(IServiceProvider provider) : IMaterializationInterceptor {
    public object InitializedInstance(MaterializationInterceptionData materializationData, object entity) {
        if (entity is Certificate cert && cert.PrivateKey is EncryptedKeyParameter encrypted) {
            using var scope = provider.CreateScope();
            var kms = scope.ServiceProvider.GetRequiredService<KeyManagementService>();
            var userKey = kms.GetUserKey(cert.SignedById);
            var decryptedBytes = AesEncryption.Decrypt(encrypted.EncryptedBytes, userKey, encrypted.IV);
            var pkInfo = PrivateKeyInfo.GetInstance(decryptedBytes);
            cert.PrivateKey = PrivateKeyFactory.CreateKey(pkInfo);
        }
        return entity;
    }

    public ValueTask<object> InitializedInstanceAsync(MaterializationInterceptionData materializationData, object entity, CancellationToken cancellationToken = default) {
        return new ValueTask<object>(InitializedInstance(materializationData, entity));
    }
}