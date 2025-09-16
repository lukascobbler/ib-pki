using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SudoBox.UnifiedModule.Domain.Users;
namespace SudoBox.UnifiedModule.Infrastructure;

public class UnifiedDbContext : DbContext
{
    private readonly string _schema;

    public UnifiedDbContext(DbContextOptions<UnifiedDbContext> options, IConfiguration cfg)
        : base(options)
    {
        _schema = cfg["Database:Schema"] ?? "unified";
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(_schema);
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
