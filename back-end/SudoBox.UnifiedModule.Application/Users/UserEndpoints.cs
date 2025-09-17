using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using SudoBox.UnifiedModule.Application.Users.Contracts.Registration;
using SudoBox.UnifiedModule.Application.Users.Features.Registration;
using SudoBox.UnifiedModule.Application.Users.Features.ConfirmEmail;
using Microsoft.AspNetCore.Builder;
using SudoBox.UnifiedModule.Application.Users.Features.Auth;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using SudoBox.UnifiedModule.Application.Users.Contracts.Auth;


namespace SudoBox.UnifiedModule.Application.Users;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var grp = app.MapGroup("/api/users");

        grp.MapPost("/register", async (RegisterRequest req, IRegistrationService svc, CancellationToken ct) =>
        {
            var res = await svc.RegisterAsync(req, ct);
            return Results.Json(res.Body, statusCode: res.StatusCode);
        }).AllowAnonymous();

        grp.MapGet("/confirm", async (string token, IEmailConfirmationService svc, CancellationToken ct) =>
        {
            var res = await svc.ConfirmAsync(token, ct);
            return Results.Json(res.Body, statusCode: res.StatusCode);
        }).AllowAnonymous();

        grp.MapPost("/login", async (LoginRequest req, IAuthService svc, CancellationToken ct) =>
        {
            var (ok, status, err) = await svc.LoginAsync(req, ct);
            return ok is not null ? Results.Json(ok, statusCode: status)
                                  : Results.Json(new { error = err }, statusCode: status);
        }).AllowAnonymous();

        grp.MapPost("/refresh", async (RefreshRequest req, IAuthService svc, CancellationToken ct) =>
        {
            var (ok, status, err) = await svc.RefreshAsync(req, ct);
            return ok is not null ? Results.Json(ok, statusCode: status)
                                  : Results.Json(new { error = err }, statusCode: status);
        }).AllowAnonymous();

        grp.MapPost("/logout", async (IAuthService svc, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)
                           ?? user.FindFirstValue("sub")!);
            var status = await svc.LogoutAsync(userId, ct);
            return Results.StatusCode(status);
        }).RequireAuthorization();

        grp.MapPost("/logout-all", async (IAuthService svc, ClaimsPrincipal user, CancellationToken ct) =>
        {
            var userId = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)
                           ?? user.FindFirstValue("sub")!);
            var status = await svc.LogoutAllAsync(userId, ct);
            return Results.StatusCode(status);
        }).RequireAuthorization("Admin");


        return app;
    }
}
