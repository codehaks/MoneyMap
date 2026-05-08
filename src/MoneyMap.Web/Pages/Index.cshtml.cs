using Microsoft.AspNetCore.Mvc.RazorPages;

namespace MoneyMap.Web.Pages;

public class IndexModel : PageModel
{
    public int DaysLeft { get; private set; }
    public int NextYear { get; private set; }

    public void OnGet()
    {
        var nowUtc = DateTime.UtcNow;
        NextYear = nowUtc.Year + 1;
        DaysLeft = (new DateTime(NextYear, 1, 1, 0, 0, 0, DateTimeKind.Utc) - nowUtc).Days;
    }
}
