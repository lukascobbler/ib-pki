using Microsoft.EntityFrameworkCore;
using SudoBox.UnifiedModule.Domain.CertificateRequests;
using SudoBox.UnifiedModule.Domain.Certificates;
using SudoBox.UnifiedModule.Domain.Users;

namespace SudoBox.UnifiedModule.Application.Abstractions;

public interface IUnifiedDbContext
{
    DbSet<User> Users { get; }
    DbSet<VerificationToken> VerificationTokens { get; }
    DbSet<Certificate> Certificates { get; }
    DbSet<CertificateRequest> CertificateRequests { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}