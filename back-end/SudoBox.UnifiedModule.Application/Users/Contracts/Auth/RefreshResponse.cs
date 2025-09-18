namespace SudoBox.UnifiedModule.Application.Users.Contracts.Auth;

public record RefreshResponse(
    string AccessToken,
    DateTimeOffset AccessExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshExpiresAt,
    string UserId
);