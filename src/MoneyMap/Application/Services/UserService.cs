using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MoneyMap.Infrastructure.Data;

namespace MoneyMap.Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IReadOnlyList<ApplicationUser>> GetAllAsync(CancellationToken ct = default) =>
        await _userManager.Users.AsNoTracking().ToListAsync(ct);

    public Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken ct = default) =>
        _userManager.FindByIdAsync(userId);

    public async Task<IList<string>> GetRolesAsync(string userId, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return Array.Empty<string>();
        }
        return await _userManager.GetRolesAsync(user);
    }
}
