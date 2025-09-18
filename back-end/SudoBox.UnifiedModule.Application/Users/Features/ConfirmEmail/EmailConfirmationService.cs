using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using SudoBox.UnifiedModule.Application.Abstractions;
using SudoBox.UnifiedModule.Domain.Users;

namespace SudoBox.UnifiedModule.Application.Users.Features.ConfirmEmail;

public class EmailConfirmationService(IUnifiedDbContext db) : IEmailConfirmationService
{
    public async Task<ConfirmationResult> ConfirmAsync(string token, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(token))
            return new(false, 400, new { error = "Missing token." });

        var tokenHashHex = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));

        var vt = await db.VerificationTokens
            .Include(t => t.User!)
            .FirstOrDefaultAsync(t => t.TokenHashHex == tokenHashHex && t.Purpose == VerificationPurpose.EmailConfirmation, ct);

        if (vt is null) return new(false, 400, new { error = "Invalid token." });
        if (vt.UsedAt is not null) return new(false, 400, new { error = "Token already used." });
        if (vt.ExpiresAt < DateTimeOffset.UtcNow) return new(false, 400, new { error = "Token expired." });

        vt.UsedAt = DateTimeOffset.UtcNow;
        vt.User!.EmailConfirmed = true;

        await db.SaveChangesAsync(ct);
        return new(true, 200, new { message = "Email confirmed. You can now sign in." });
    }
}
