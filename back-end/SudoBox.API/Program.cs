using Microsoft.EntityFrameworkCore;
using SudoBox.BuildingBlocks.Infrastructure;
using SudoBox.UnifiedModule.Infrastructure;
using SudoBox.UnifiedModule.Infrastructure.Email;
using SudoBox.UnifiedModule.Application.Users;

var builder = WebApplication.CreateBuilder(args);

// EF Core
builder.Services.AddDbContext<UnifiedDbContext>(opt =>
{
    var schema = builder.Configuration.GetValue<string>("Database:Schema") ?? "unified";
    var conn = DbConnectionStringBuilder.Build(schema);
    opt.UseNpgsql(conn, npgsql => npgsql.MigrationsHistoryTable("__EFMigrationsHistory", schema))
       .UseSnakeCaseNamingConvention();
});

// Email + user features (password common-list)
builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
var commonPath = builder.Configuration["Password:CommonListPath"]
                 ?? Path.Combine(AppContext.BaseDirectory, "Users", "data", "common_passwords.txt");
builder.Services.AddUserFeatures(commonPath);

// Swagger/CORS
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UnifiedDbContext>();
    db.Database.Migrate();
}

// Minimal API endpoints
app.MapUserEndpoints();

app.Run();
