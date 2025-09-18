using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SudoBox.UnifiedModule.Application.Users.Contracts.Registration;
using SudoBox.UnifiedModule.Application.Users.Utils.Password;
using SudoBox.UnifiedModule.Domain.Users;
using SudoBox.UnifiedModule.Application.Users.Utils.Email;
using static SudoBox.UnifiedModule.Application.Users.Utils.Password.PasswordPolicy;
using SudoBox.UnifiedModule.Application.Abstractions;

namespace SudoBox.UnifiedModule.Application.Users.Features.Registration;

public class RegistrationService(
    IUnifiedDbContext db,
    IEmailSender email,
    ICommonPasswordStore commonStore,
    IConfiguration cfg
) : IRegistrationService
{
    public async Task<RegistrationResult> RegisterAsync(RegisterRequest req, CancellationToken ct)
    {
        var emailNorm = (req.Email ?? "").Trim();
        if (string.IsNullOrWhiteSpace(emailNorm))
            return new(false, 400, new { error = "Email is required." });

        if (req.Password != req.ConfirmPassword)
            return new(false, 400, new { error = "Passwords do not match." });

        if (await db.Users.AnyAsync(u => u.Email == emailNorm, ct))
            return new(false, 409, new { error = "An account with this email already exists." });

        var policy = Evaluate(req.Password, emailNorm, req.Name, req.Surname, commonStore);
        if (!policy.Ok) return new(false, 400, new { error = "Weak password.", details = policy.Errors });

        var pwdHash = BCrypt.Net.BCrypt.HashPassword(req.Password, workFactor: 12);

        var user = new User
        {
            Role = Role.EeUser,
            Name = string.IsNullOrWhiteSpace(req.Name) ? null : req.Name.Trim(),
            Surname = string.IsNullOrWhiteSpace(req.Surname) ? null : req.Surname.Trim(),
            Organization = string.IsNullOrWhiteSpace(req.Organization) ? null : req.Organization.Trim(),
            Email = emailNorm,
            EmailConfirmed = false,
            HashedPassword = pwdHash,
            RefreshToken = ""
        };
        await db.Users.AddAsync(user, ct);

        var tokenPlain = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)); // 64 hex chars
        var tokenHashHex = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(tokenPlain)));


        var ttlMinutes = int.TryParse(cfg["Auth:EmailConfirmation:TokenTtlMinutes"], out var m) ? m : 60;
        var vt = new VerificationToken
        {
            UserId = user.Id,
            Purpose = VerificationPurpose.EmailConfirmation,
            TokenHashHex = tokenHashHex,
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(ttlMinutes),
            UsedAt = null
        };
        await db.VerificationTokens.AddAsync(vt, ct);
        await db.SaveChangesAsync(ct);

        var publicBase = (cfg["Auth:EmailConfirmation:PublicBaseUrl"] ?? "https://localhost:8081").TrimEnd('/');
        var confirmUrl = $"{publicBase}/api/users/confirm?token={tokenPlain}";

        var html = $@"
            <p>Hi {System.Net.WebUtility.HtmlEncode(user.Name ?? "there")},</p>
            <p>Confirm your email by clicking the link below. This link expires in {ttlMinutes} minutes and can be used once.</p>
            <p><a href=""{confirmUrl}"">Confirm my email</a></p>
            <p>If you didn't sign up, please ignore this message.</p>";

        await email.SendAsync(user.Email, "Confirm your SudoBox account", html, ct);

        return new(true, 202, new RegisterResponse("Registration received. Check your email to confirm."));
    }
}
