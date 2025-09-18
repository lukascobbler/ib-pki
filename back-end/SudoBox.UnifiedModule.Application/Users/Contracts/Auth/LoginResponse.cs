namespace SudoBox.UnifiedModule.Application.Users.Contracts.Auth;
public sealed record LoginResponse(
    string AccessToken,
    DateTimeOffset AccessExpiresAt,
    string RefreshToken,
    DateTimeOffset RefreshExpiresAt,
    string UserId,
    string Role,
    string? Name,
    string? Surname
);
