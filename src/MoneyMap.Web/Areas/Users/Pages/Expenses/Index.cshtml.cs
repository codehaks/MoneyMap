using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MoneyMap.Application.Services;
using MoneyMap.Core.DataModels;

namespace MoneyMap.Web.Areas.Users.Pages.Expenses;

[Authorize]
public class IndexModel : PageModel
{
    private readonly IExpenseService _expenseService;

    public IndexModel(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    public IList<Expense> ExpenseList { get; set; }

    public void OnGet()
    {

        ExpenseList = _expenseService.GetAll();
    }

    [BindProperty]
    public int Id { get; set; }

    public IActionResult OnPostDelete()
    {
        _expenseService.Remove(Id);
        // ExpenseList = _expenseService.GetAll();
        return RedirectToPage();
    }
}
