using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SudoBox.UnifiedModule.Application.Users.Contracts.Auth;
using SudoBox.UnifiedModule.Application.Users.Utils.Auth;
using SudoBox.UnifiedModule.Domain.Users;
using SudoBox.UnifiedModule.Application.Abstractions;
using BC = BCrypt.Net.BCrypt;

namespace SudoBox.UnifiedModule.Application.Users.Features.Auth;

public interface IAuthService
{
    Task<(LoginResponse? ok, int status, string? err)> LoginAsync(LoginRequest req, CancellationToken ct);
    Task<(RefreshResponse? ok, int status, string? err)> RefreshAsync(RefreshRequest req, CancellationToken ct);
    Task<int> LogoutAsync(Guid userId, CancellationToken ct);
    Task<int> LogoutAllAsync(Guid userId, CancellationToken ct);
}

public class AuthService : IAuthService
{
    private readonly IUnifiedDbContext _db;
    private readonly JwtSecurityTokenHandler _jwt = new();
    private readonly RSA _rsa;
    private readonly string _issuer, _audience;
    private readonly int _accessMinutes;
    private readonly TimeSpan _refreshTtl;

    public AuthService(IConfiguration cfg, IUnifiedDbContext db)
    {
        _db = db;

        _issuer = cfg["Auth:Jwt:Issuer"]!;
        _audience = cfg["Auth:Jwt:Audience"]!;
        _accessMinutes = int.Parse(cfg["Auth:Jwt:AccessTokenMinutes"] ?? "1");  // 1 minute
        _refreshTtl = TimeSpan.FromDays(int.Parse(cfg["Auth:Refresh:TtlDays"] ?? "14"));

        var pfxPath = cfg["Auth:Jwt:SigningCertificate:Path"]!;
        if (!Path.IsPathRooted(pfxPath))
            pfxPath = Path.Combine(Directory.GetCurrentDirectory(), pfxPath);
        var pfxPass = cfg["Auth:Jwt:SigningCertificate:Password"]!;
        var cert = X509CertificateLoader.LoadPkcs12FromFile(pfxPath, pfxPass);
        _rsa = cert.GetRSAPrivateKey()!;
    }

    public async Task<(LoginResponse? ok, int status, string? err)> LoginAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Email == req.Email, ct);
        if (user is null) return (null, 401, "Invalid credentials");
        if (!user.EmailConfirmed) return (null, 403, "Email not confirmed");
        if (!BC.Verify(req.Password, user.HashedPassword)) return (null, 401, "Invalid credentials");

        var (access, atExp) = IssueAccessJwt(user);
        var (refresh, rtExp) = await RotateNewRefreshTokenAsync(user, ct);

        var resp = new LoginResponse(
            AccessToken: access,
            AccessExpiresAt: atExp,
            RefreshToken: refresh,
            RefreshExpiresAt: rtExp,
            UserId: user.Id.ToString(),
            Role: user.Role.ToString(),
            Name: user.Name,
            Surname: user.Surname
        );
        return (resp, 200, null);
    }

    public async Task<(RefreshResponse? ok, int status, string? err)> RefreshAsync(RefreshRequest req, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(req.RefreshToken))
            return (null, 400, "Missing refresh token");

        // validate refresh JWT signature/lifetime and read userId
        var (valid, userId, reason) = ValidateRefreshJwt(req.RefreshToken);
        if (!valid || userId is null) return (null, 401, reason ?? "Invalid refresh token");

        // Load user and compare stored hash
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Id == userId, ct);
        if (user is null) return (null, 401, "Invalid refresh token");

        // compare tokens
        var presentedHash = TokenUtils.Sha256Hex(System.Text.Encoding.UTF8.GetBytes(req.RefreshToken));

        // if user has no token
        var hasStored = !string.IsNullOrEmpty(user.RefreshToken) && user.RefreshTokenExpiresAt != null && user.RefreshTokenExpiresAt > DateTimeOffset.UtcNow;
        if (!hasStored)
            return (null, 401, "Refresh token revoked");

        if (!string.Equals(user.RefreshToken, presentedHash, StringComparison.Ordinal))
        {
            // revoke if token mismatch
            user.RefreshToken = string.Empty;
            user.RefreshTokenExpiresAt = null;
            await _db.SaveChangesAsync(ct);
            return (null, 401, "Refresh token mismatch; revoked");
        }

        // rotate tokens
        var (access, atExp) = IssueAccessJwt(user);
        var (refresh, rtExp) = await RotateNewRefreshTokenAsync(user, ct);

        var resp = new RefreshResponse(
            AccessToken: access,
            AccessExpiresAt: atExp,
            RefreshToken: refresh,
            RefreshExpiresAt: rtExp,
            UserId: user.Id.ToString()
        );
        return (resp, 200, null);
    }

    public async Task<int> LogoutAsync(Guid userId, CancellationToken ct)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.Id == userId, ct);
        if (user is not null)
        {
            user.RefreshToken = string.Empty;
            user.RefreshTokenExpiresAt = null;
            await _db.SaveChangesAsync(ct);
        }
        return 204;
    }

    public async Task<int> LogoutAllAsync(Guid userId, CancellationToken ct)
    {
        return await LogoutAsync(userId, ct);
    }


    private (string jwt, DateTimeOffset exp) IssueAccessJwt(User user)
    {
        var now = DateTimeOffset.UtcNow;
        var exp = now.AddMinutes(_accessMinutes);

        var creds = new SigningCredentials(new RsaSecurityKey(_rsa), SecurityAlgorithms.RsaSha256);
        var claims = new List<Claim>
        {
            // metadata
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("typ", "access")
        };

        var token = new JwtSecurityToken(
            issuer: _issuer, audience: _audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: exp.UtcDateTime,
            signingCredentials: creds);

        return (_jwt.WriteToken(token), exp);
    }
    
    private async Task<(string refresh, DateTimeOffset exp)> RotateNewRefreshTokenAsync(User user, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var exp = now.Add(_refreshTtl);

        var creds = new SigningCredentials(new RsaSecurityKey(_rsa), SecurityAlgorithms.RsaSha256);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("typ", "refresh")
        };
        var jwt = new JwtSecurityToken(
            issuer: _issuer, audience: _audience,
            claims: claims,
            notBefore: now.UtcDateTime,
            expires: exp.UtcDateTime,
            signingCredentials: creds);

        var refreshPlain = _jwt.WriteToken(jwt);
        var hashHex = TokenUtils.Sha256Hex(System.Text.Encoding.UTF8.GetBytes(refreshPlain));

        user.RefreshToken = hashHex;
        user.RefreshTokenExpiresAt = exp;
        await _db.SaveChangesAsync(ct);

        return (refreshPlain, exp);
    }

    private (bool ok, Guid? userId, string? reason) ValidateRefreshJwt(string token)
    {
        try
        {
            var key = new RsaSecurityKey(_rsa);
            var parms = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _issuer,
                ValidateAudience = true,
                ValidAudience = _audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = key,
                RoleClaimType = ClaimTypes.Role,
                NameClaimType = ClaimTypes.NameIdentifier
            };
            var principal = _jwt.ValidateToken(token, parms, out var validated);
            if (validated is not JwtSecurityToken) return (false, null, "Invalid token");
            
            var typ = principal.FindFirst("typ")?.Value;
            if (!string.Equals(typ, "refresh", StringComparison.Ordinal))
                return (false, null, "Wrong token type");

            var sub = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return Guid.TryParse(sub, out var uid) ? (true, uid, null) : (false, null, "Invalid subject");
        }
        catch (SecurityTokenExpiredException) { return (false, null, "Expired"); }
        catch { return (false, null, "Invalid"); }
    }
}
