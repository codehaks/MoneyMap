using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoneyMap.Application;
using MoneyMap.Infrastructure.Data;

namespace MoneyMap.Web.Areas.Admin.Pages.Users;

public class DetailsModel : PageModel
{
    public IList<string> Roles { get; set; } = new List<string>();
    public ApplicationUser? UserDetails { get; set; }
    public string UserId { get; set; } = string.Empty;

    private readonly IUserService _userService;

    public DetailsModel(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<PageResult> OnGet(string id)
    {
        UserId = id;
        Roles = await _userService.GetRoles(id);
        UserDetails = _userService.GetAll().FirstOrDefault(u => u.Id == id);
        return Page();
    }
}
