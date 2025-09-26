namespace SudoBox.UnifiedModule.Application.Users.Contracts.Registration;

public record CaRegisterRequest(
    string Email,
    string Password,
    string ConfirmPassword,
    string Name,
    string Surname,
    string Organization,
    string InitialSigningCertificateId
);