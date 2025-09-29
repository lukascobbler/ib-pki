using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SudoBox.UnifiedModule.Domain.Users;
using SudoBox.UnifiedModule.Domain.Certificates;
using SudoBox.UnifiedModule.Domain.CertificateRequests;
using SudoBox.UnifiedModule.Application.Abstractions;
using SudoBox.UnifiedModule.Infrastructure.Certificates.DomainDatabaseSetup;
using SudoBox.UnifiedModule.Infrastructure.CertificateRequests.DomainDatabaseSetup;
using SudoBox.UnifiedModule.Infrastructure.Users.DomainDatabaseSetup;

namespace SudoBox.UnifiedModule.Infrastructure.DbContext;

public class UnifiedDbContext : Microsoft.EntityFrameworkCore.DbContext, IUnifiedDbContext
{
    private readonly string _schema;

    public UnifiedDbContext(DbContextOptions<UnifiedDbContext> options, IConfiguration cfg)
        : base(options)
    {
        _schema = cfg["Database:Schema"] ?? "unified";
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<VerificationToken> VerificationTokens => Set<VerificationToken>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<CertificateRequest> CertificateRequests => Set<CertificateRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(_schema);
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new VerificationTokenEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CertificateTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CertificateRequestTypeConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
