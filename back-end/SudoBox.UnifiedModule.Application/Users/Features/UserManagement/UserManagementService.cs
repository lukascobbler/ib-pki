using Microsoft.EntityFrameworkCore;
using SudoBox.UnifiedModule.Application.Abstractions;
using SudoBox.UnifiedModule.Application.Users.Contracts.Registration;
using SudoBox.UnifiedModule.Application.Users.Contracts.UserManagement;
using SudoBox.UnifiedModule.Application.Users.Features.Registration;
using SudoBox.UnifiedModule.Domain.Users;

namespace SudoBox.UnifiedModule.Application.Users.Features.UserManagement;

public class UserManagementService(IUnifiedDbContext db)
{
    public async Task<List<CaUserResponse>> GetAllCaUsers()
    {
        var users = await db.Users
            .Where(u => u.Role == Role.CaUser)
            .ToListAsync();

        return users.Select(u => new CaUserResponse(u)
        {
            Id = u.Id.ToString(),
            Email = u.Email
        }).ToList();
    }
}