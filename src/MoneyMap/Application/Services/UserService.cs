using Microsoft.AspNetCore.Identity;
using MoneyMap.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMap.Application.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public List<ApplicationUser> GetAll()
    {
        return _userManager.Users.ToList();
    }

    public async Task<IList<string>> GetRoles(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        return await _userManager.GetRolesAsync(user);
    }
}

