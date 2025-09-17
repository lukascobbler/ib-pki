namespace SudoBox.UnifiedModule.Domain.Users;

public sealed class RefreshToken
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string TokenHashHex { get; set; } = default!;

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }

    public DateTimeOffset? ConsumedAt { get; set; }            // set on successful rotation
    public DateTimeOffset? RevokedAt { get; set; }            
    public string? ReplacedByHashHex { get; set; }             // chain to the next token in the family

    // optional telemetry
    public string? DeviceId { get; set; }
    public string? Ip { get; set; }
    public string? UserAgent { get; set; }
}
