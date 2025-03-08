using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoneyMap.Application;
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

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindNever]
    public SelectList CategorySelectList { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? CategoryId { get; set; }

    public void OnGet()
    {
        var categories = _expenseService.GetCategories();
        CategorySelectList = new SelectList(categories, "Id", "Name");

        var expenses = _expenseService.GetAll(SearchTerm,CategoryId);
        ExpenseList = expenses;
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
