namespace SudoBox.UnifiedModule.Application.Users.Contracts.Registration;

public sealed record RegisterRequest(
    string Email,
    string Password,
    string ConfirmPassword,
    string Name,
    string Surname,
    string Organization
);
