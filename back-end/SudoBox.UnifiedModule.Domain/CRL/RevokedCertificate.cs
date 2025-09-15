using SudoBox.BuildingBlocks.Domain;

namespace SudoBox.UnifiedModule.Domain.CRL;

public class RevokedCertificate: Entity
{
    public required string Fingerprint { get; set; }
}