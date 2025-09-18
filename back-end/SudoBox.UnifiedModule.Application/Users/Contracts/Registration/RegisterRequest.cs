namespace SudoBox.UnifiedModule.Application.Users.Contracts.Registration;

public record RegisterRequest(
    string Email,
    string Password,
    string ConfirmPassword,
    string Name,
    string Surname,
    string Organization
);
