using Microsoft.EntityFrameworkCore;
using SudoBox.UnifiedModule.Infrastructure;
using Sudobox.BuildingBlocks.Infrastructure;

var builder = WebApplication.CreateBuilder(args);


// EF core setup
builder.Services.AddDbContext<UnifiedDbContext>(opt =>
{
    var schema = builder.Configuration.GetValue<string>("Database:Schema") ?? "unified";
    var conn = DbConnectionStringBuilder.Build(schema);

    opt.UseNpgsql(conn, npgsql =>
    {
        // Keep the migrations history table inside the same schema
        npgsql.MigrationsHistoryTable("__EFMigrationsHistory", schema);
    })
    .UseSnakeCaseNamingConvention();
});


builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

// todo fix cors to allow only the client of this server
app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();

// apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UnifiedDbContext>();
    db.Database.Migrate();
}

app.Run();
