using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using SudoBox.UnifiedModule.Application.Users.Contracts.Registration;
using SudoBox.UnifiedModule.Application.Users.Features.Registration;
using SudoBox.UnifiedModule.Application.Users.Features.ConfirmEmail;
using Microsoft.AspNetCore.Builder;

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
        });

        grp.MapGet("/confirm", async (string token, IEmailConfirmationService svc, CancellationToken ct) =>
        {
            var res = await svc.ConfirmAsync(token, ct);
            return Results.Json(res.Body, statusCode: res.StatusCode);
        });

        return app;
    }
}
