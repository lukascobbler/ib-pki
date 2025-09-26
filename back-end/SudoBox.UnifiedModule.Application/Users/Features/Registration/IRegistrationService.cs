using SudoBox.UnifiedModule.Application.Users.Contracts.Registration;
using SudoBox.UnifiedModule.Application.Users.Contracts.UserManagement;

namespace SudoBox.UnifiedModule.Application.Users.Features.Registration;

public record RegistrationResult(bool Ok, int StatusCode, RegisterResponse Response);

public interface IRegistrationService
{
    Task<RegistrationResult> RegisterAsync(
        RegisterRequest req, 
        CancellationToken ct, 
        bool creatingCaUser
        );
}
