namespace SudoBox.UnifiedModule.Application.Users.Contracts.Auth;

public sealed record RefreshResponse(
    string AccessToken,
    DateTimeOffset AccessExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshExpiresAt,
    string UserId
);