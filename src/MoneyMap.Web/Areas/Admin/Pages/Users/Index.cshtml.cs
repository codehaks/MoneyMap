using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoneyMap.Application.Services;

namespace MoneyMap.Web.Areas.Admin.Pages.Users
{
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;

        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }

        public List<ApplicationUser> Users { get; set; }

        public void OnGet()
        {
            Users = _userService.GetAll();
        }
    }
}
