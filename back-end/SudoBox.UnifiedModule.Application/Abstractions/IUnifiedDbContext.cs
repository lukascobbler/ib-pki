using SudoBox.UnifiedModule.Domain.Certificates.KeyManagement;
using SudoBox.UnifiedModule.Domain.CertificateRequests;
using SudoBox.UnifiedModule.Domain.Certificates;
using SudoBox.UnifiedModule.Domain.Users;
using SudoBox.UnifiedModule.Domain.CRL;
using Microsoft.EntityFrameworkCore;

namespace SudoBox.UnifiedModule.Application.Abstractions;

public interface IUnifiedDbContext {
    DbSet<User> Users { get; }
    DbSet<VerificationToken> VerificationTokens { get; }
    DbSet<Certificate> Certificates { get; }
    DbSet<RevokedCertificate> RevokedCertificates { get; }
    DbSet<CertificateRequest> CertificateRequests { get; }
    DbSet<MasterKey> MasterKey { get; }
    DbSet<UserKey> UserKeys { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
    int SaveChanges();
}