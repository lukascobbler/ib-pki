using SudoBox.UnifiedModule.Application.Users.Contracts.Registration;

namespace SudoBox.UnifiedModule.Application.Users.Features.Registration;

public sealed record RegistrationResult(bool Ok, int StatusCode, object Body);

public interface IRegistrationService
{
    Task<RegistrationResult> RegisterAsync(RegisterRequest req, CancellationToken ct);
}
