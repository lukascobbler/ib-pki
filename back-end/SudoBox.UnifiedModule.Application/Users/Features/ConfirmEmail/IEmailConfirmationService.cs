namespace SudoBox.UnifiedModule.Application.Users.Features.ConfirmEmail;

public record ConfirmationResult(bool Ok, int StatusCode, object Body);

public interface IEmailConfirmationService
{
    Task<ConfirmationResult> ConfirmAsync(string token, CancellationToken ct);
}
