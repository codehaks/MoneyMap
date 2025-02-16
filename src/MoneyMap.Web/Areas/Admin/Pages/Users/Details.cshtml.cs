using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoneyMap.Application;

namespace MoneyMap.Web.Areas.Admin.Pages.Users;

public class DetailsModel : PageModel
{
    public IList<string> Roles { get; set; }

    private readonly IUserService _userService;

    public DetailsModel(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<PageResult> OnGet(string id)
    {
        Roles =await _userService.GetRoles(id);
        return Page();
    }
}
