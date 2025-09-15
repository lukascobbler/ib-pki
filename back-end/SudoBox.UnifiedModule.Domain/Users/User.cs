using SudoBox.BuildingBlocks.Domain;

namespace SudoBox.UnifiedModule.Domain.Users;

public class User: Entity
{
    public required Role Role { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Organization { get; set; }
    public required string Email { get; set; }
    public required bool EmailConfirmed { get; set; }
    public required string HashedPassword { get; set; }
    public required string RefreshToken { get; set; }
}