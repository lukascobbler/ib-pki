using SudoBox.UnifiedModule.Infrastructure.CertificateRequests.DomainDatabaseSetup;
using SudoBox.UnifiedModule.Infrastructure.Certificates.DomainDatabaseSetup;
using SudoBox.UnifiedModule.Infrastructure.Users.DomainDatabaseSetup;
using SudoBox.UnifiedModule.Infrastructure.Certificates.Interceptors;
using SudoBox.UnifiedModule.Infrastructure.Crl.DomainDatabaseSetup;
using SudoBox.UnifiedModule.Domain.Certificates.KeyManagement;
using SudoBox.UnifiedModule.Domain.CertificateRequests;
using SudoBox.UnifiedModule.Application.Abstractions;
using SudoBox.UnifiedModule.Domain.Certificates;
using Microsoft.Extensions.Configuration;
using SudoBox.UnifiedModule.Domain.Users;
using SudoBox.UnifiedModule.Domain.CRL;
using Microsoft.EntityFrameworkCore;

namespace SudoBox.UnifiedModule.Infrastructure.DbContext;

public class UnifiedDbContext(DbContextOptions<UnifiedDbContext> options, IConfiguration cfg,
    PrivateKeyMaterializationInterceptor? materializationInterceptor, PrivateKeySaveInterceptor? saveInterceptor)
    : Microsoft.EntityFrameworkCore.DbContext(options), IUnifiedDbContext {

    private readonly string _schema = cfg["Database:Schema"] ?? "unified";

    public DbSet<User> Users => Set<User>();
    public DbSet<VerificationToken> VerificationTokens => Set<VerificationToken>();
    public DbSet<Certificate> Certificates => Set<Certificate>();
    public DbSet<RevokedCertificate> RevokedCertificates => Set<RevokedCertificate>();
    public DbSet<CertificateRequest> CertificateRequests => Set<CertificateRequest>();
    public DbSet<UserKey> UserKeys => Set<UserKey>();
    public DbSet<MasterKey> MasterKey => Set<MasterKey>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (materializationInterceptor != null && saveInterceptor != null)
            optionsBuilder.AddInterceptors(materializationInterceptor, saveInterceptor);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.HasDefaultSchema(_schema);
        modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new VerificationTokenEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RefreshTokenEntityTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CertificateTypeConfiguration());
        modelBuilder.ApplyConfiguration(new RevokedCertificateTypeConfiguration());
        modelBuilder.ApplyConfiguration(new CertificateRequestTypeConfiguration());
        modelBuilder.ApplyConfiguration(new UserKeyTypeConfiguration());
        modelBuilder.ApplyConfiguration(new MasterKeyTypeConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
