using Microsoft.AspNetCore.Mvc.RazorPages;
using MoneyMap.Application;
using MoneyMap.Infrastructure.Data;

namespace MoneyMap.Web.Areas.Admin.Pages.Users;

public class IndexModel : PageModel
{
    private readonly IUserService _userService;

    public IndexModel(IUserService userService)
    {
        _userService = userService;
    }

    public IReadOnlyList<ApplicationUser> Users { get; private set; } = Array.Empty<ApplicationUser>();

    public async Task OnGetAsync(CancellationToken ct)
    {
        Users = await _userService.GetAllAsync(ct);
    }
}
