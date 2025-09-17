using Microsoft.EntityFrameworkCore;
using SudoBox.UnifiedModule.Domain.Users;

namespace SudoBox.UnifiedModule.Application.Abstractions;

public interface IUnifiedDbContext
{
    DbSet<User> Users { get; }
    DbSet<VerificationToken> VerificationTokens { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}