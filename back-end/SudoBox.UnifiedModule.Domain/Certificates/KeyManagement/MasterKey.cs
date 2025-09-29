using SudoBox.BuildingBlocks.Domain;

namespace SudoBox.UnifiedModule.Domain.Certificates.KeyManagement;

public class MasterKey : Entity{
    public string EncryptedKey { get; set; } = default!;
}