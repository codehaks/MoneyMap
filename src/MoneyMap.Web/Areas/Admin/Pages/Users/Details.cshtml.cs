using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoneyMap.Application;
using MoneyMap.Infrastructure.Data;

namespace MoneyMap.Web.Areas.Admin.Pages.Users;

public class DetailsModel : PageModel
{
    private readonly IUserService _userService;

    public DetailsModel(IUserService userService)
    {
        _userService = userService;
    }

    public IList<string> Roles { get; private set; } = new List<string>();
    public ApplicationUser? UserDetails { get; private set; }
    public string UserId { get; private set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(string id, CancellationToken ct)
    {
        UserId = id;
        UserDetails = await _userService.FindByIdAsync(id, ct);
        if (UserDetails is null)
        {
            return NotFound();
        }
        Roles = await _userService.GetRolesAsync(id, ct);
        return Page();
    }
}
