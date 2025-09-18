namespace SudoBox.UnifiedModule.Application.Users.Contracts.Auth;

public sealed record LoginRequest(string Email, string Password, string? DeviceId);

