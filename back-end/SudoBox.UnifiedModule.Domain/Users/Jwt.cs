namespace SudoBox.UnifiedModule.Domain.Users;

public class Jwt
{
    public required string Token { get; set; }
    public required DateTime ExpiresAt { get; set; }
    public required DateTime IssuedAt { get; set; }
    public required Guid UserId { get; set; }
    public required Role UserRole { get; set; }
    public required Guid JwtId { get; set; }
}