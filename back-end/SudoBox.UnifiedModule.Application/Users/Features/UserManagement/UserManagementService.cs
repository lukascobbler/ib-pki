using Microsoft.EntityFrameworkCore;
using SudoBox.UnifiedModule.Application.Abstractions;
using SudoBox.UnifiedModule.Application.Certificates.Features;
using SudoBox.UnifiedModule.Application.Users.Contracts.UserManagement;
using SudoBox.UnifiedModule.Domain.Certificates;
using SudoBox.UnifiedModule.Domain.Users;

namespace SudoBox.UnifiedModule.Application.Users.Features.UserManagement;

public class UserManagementService(IUnifiedDbContext db, CertificateService certificateService) {
    public async Task<List<CaUserResponse>> GetAllCaUsers() {
        var users = await db.Users
            .Where(u => u.Role == Role.CaUser)
            .ToListAsync();

        return users.Select(u => new CaUserResponse(u) {
            Id = u.Id.ToString(),
            Email = u.Email
        }).ToList();
    }

    public async Task<List<CaUserResponse>> GetValidCaUsers() {
        var users = await db.Users
            .Where(u => u.Role == Role.CaUser)
            .Include(u => u.MyCertificates)
            .ToListAsync();

        users = [.. users.Where(u => u.MyCertificates.Any(c => certificateService.GetStatus(c) == CertificateStatus.Active))];

        return [.. users.Select(u => new CaUserResponse(u) {
            Id = u.Id.ToString(),
            Email = u.Email,
            MinValidFrom = u.MyCertificates.Where(c => certificateService.GetStatus(c) == CertificateStatus.Active).Min(c => c.NotBefore),
            MaxValidUntil = u.MyCertificates.Where(c => certificateService.GetStatus(c) == CertificateStatus.Active).Max(c => c.NotAfter)
        })];
    }
}