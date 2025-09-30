using SudoBox.BuildingBlocks.Domain;

namespace SudoBox.UnifiedModule.Domain.Users;

public enum VerificationPurpose { EmailConfirmation }

public class VerificationToken : Entity
{
    public required Guid UserId { get; set; }
    public required VerificationPurpose Purpose { get; set; }
    public required string TokenHashHex { get; set; }   // SHA-256 hex of plaintext token
    public required DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? UsedAt { get; set; }

    public User? User { get; set; }
}
