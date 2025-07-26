using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoneyMap.Application;

namespace MoneyMap.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICalendarService _calendarService;

        public IndexModel(ICalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        public int DaysLeft { get; set; }
        public bool IsNewYear { get; set; }
        public async Task OnGetAsync()
        {
            IsNewYear = await _calendarService.IsNewYearAsync();
            DaysLeft = await _calendarService.DaysLeftInYearAsync();
        }
    }
}
