using System.Security.Claims;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using SudoBox.UnifiedModule.Application.Users.Contracts.Registration;
using SudoBox.UnifiedModule.Application.Users.Features.Registration;
using SudoBox.UnifiedModule.Application.Users.Features.ConfirmEmail;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.HttpSys;
using SudoBox.UnifiedModule.Application.Certificates.Contracts;
using SudoBox.UnifiedModule.Application.Certificates.Features;
using SudoBox.UnifiedModule.Application.Users.Features.Auth;
using SudoBox.UnifiedModule.Application.Users.Contracts.Auth;
using SudoBox.UnifiedModule.Application.Users.Contracts.UserManagement;
using SudoBox.UnifiedModule.Application.Users.Features.UserManagement;

namespace SudoBox.UnifiedModule.API.Users;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var grp = app.MapGroup("/api/v1/users");

        grp.MapPost("/register", async (RegisterRequest req, IRegistrationService svc, CancellationToken ct) =>
        {
            var res = await svc.RegisterAsync(req, ct, false);
            return Results.Json(res.Response, statusCode: res.StatusCode);
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

        grp.MapGet("/get-all-ca-users", async (UserManagementService userManagementService) => Results.Ok((object?)await userManagementService.GetAllCaUsers()))
            .RequireAuthorization();
        
        grp.MapPost("/register-ca", async (CaRegisterRequest req, IRegistrationService svc, CertificateService certificateService, CancellationToken ct) =>
        {
            var caReq = new RegisterRequest(req.Email, req.Password, req.Password, req.Name, req.Surname,
                req.Organization);
            
            var res = await svc.RegisterAsync(caReq, ct, true);

            await certificateService.AddCertificateToCaUser(new AddCertificateToCaUserRequest(res.Response.Id!, req.InitialSigningCertificateId));
            
            return Results.Json(res.Response, statusCode: res.StatusCode);
        }).AllowAnonymous();
    }
}
