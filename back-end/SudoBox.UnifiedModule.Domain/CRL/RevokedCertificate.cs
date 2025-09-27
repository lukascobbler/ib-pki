using SudoBox.BuildingBlocks.Domain;
using SudoBox.UnifiedModule.Domain.Certificates;

namespace SudoBox.UnifiedModule.Domain.CRL;

public class RevokedCertificate: Entity
{
    public required Certificate Certificate { get; set; }
    public required RevocationReason RevocationReason { get; set; }
}