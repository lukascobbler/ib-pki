using Microsoft.EntityFrameworkCore;
using SudoBox.UnifiedModule.Infrastructure;
using SudoBox.UnifiedModule.Application.Users.Utils.Email;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using SudoBox.UnifiedModule.API.Users;
using SudoBox.UnifiedModule.Infrastructure.Users;


var builder = WebApplication.CreateBuilder(args);

// https
builder.WebHost.ConfigureKestrel((ctx, kestrel) =>
{
    var pfxRel = "secrets/dev-tls.pfx";
    var pfxAbs = Path.Combine(builder.Environment.ContentRootPath, pfxRel);
    var pass = "change-me";

    if (File.Exists(pfxAbs))
    {
        var tlsCert = X509CertificateLoader.LoadPkcs12FromFile(pfxAbs, pass);
        kestrel.ListenAnyIP(8080);
        kestrel.ListenAnyIP(8081, o => o.UseHttps(tlsCert));
    }
});


// EF Core
builder.Services.AddInfrastructure(builder.Configuration);

// Email + user features (password common-list)
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
var commonPath = builder.Configuration["Password:CommonListPath"]
                 ?? Path.Combine(AppContext.BaseDirectory, "Users", "data", "common_passwords.txt");

// Auth

var cfg = builder.Configuration;
var certPath = cfg["Auth:Jwt:SigningCertificate:Path"]!;
var certPass = cfg["Auth:Jwt:SigningCertificate:Password"]!;

if (!Path.IsPathRooted(certPath))
    certPath = Path.Combine(builder.Environment.ContentRootPath, certPath);

var cert = X509CertificateLoader.LoadPkcs12FromFile(certPath, certPass);
var rsaPublic = cert.GetRSAPublicKey()!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = cfg["Auth:Jwt:Issuer"],
            ValidAudience = cfg["Auth:Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(rsaPublic),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(opt =>
{
    opt.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();

    opt.AddPolicy("Admin", p => p.RequireRole("Admin"));
    opt.AddPolicy("CaUser", p => p.RequireRole("CaUser"));
    opt.AddPolicy("EeUser", p => p.RequireRole("EeUser"));
});



builder.Services.AddUserFeatures(commonPath);


// Swagger/CORS
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SudoBox API", Version = "v1" });

    c.AddServer(new OpenApiServer { Url = "/" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Example: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        }] = Array.Empty<string>()
    });
});
builder.Services.AddCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(o =>
    {
        o.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        o.RoutePrefix = string.Empty;
    });
}

app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();



// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UnifiedDbContext>();
    db.Database.Migrate();
}

// Minimal API endpoints
app.MapUserEndpoints();

// who am i test
var api = app.MapGroup("/api");
api.MapGet("/me", (ClaimsPrincipal u) =>
{
    var id = u.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? u.FindFirst("sub")?.Value;
    var roles = u.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();
    return Results.Json(new { id, roles });
});

app.Run();
