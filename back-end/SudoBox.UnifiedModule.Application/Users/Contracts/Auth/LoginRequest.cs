namespace SudoBox.UnifiedModule.Application.Users.Contracts.Auth;

public record LoginRequest(string Email, string Password, string? DeviceId);

