namespace SudoBox.UnifiedModule.Domain.Certificates.KeyManagement;

public class UserKey {
    public Guid UserId { get; set; }
    public string EncryptedKey { get; set; } = default!;
}