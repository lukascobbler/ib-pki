using SudoBox.UnifiedModule.Domain.Users;

namespace SudoBox.UnifiedModule.Application.Users.Contracts.UserManagement;

public class CaUserResponse(User u) {
    public required string Id { get; set; } = u.Id.ToString();
    public required string Email { get; set; } = u.Email;
    public string? Name { get; set; } = u.Name;
    public string? Surname { get; set; } = u.Surname;
    public string? Organization { get; set; } = u.Organization;
    public DateTime? MinValidFrom { get; set; }
    public DateTime? MaxValidUntil { get; set; }
}