namespace SudoBox.UnifiedModule.Application.Users.Contracts.Registration;

public class RegisterResponse
{
    public required string Message { get; set; }
    public string? Id { get; set; } = null;
    public string? Name { get; set; } = null;
    public string? Surname { get; set; } = null;
    public string? Email { get; set; } = null; 
    public string? Organization { get; set; } = null;
}
