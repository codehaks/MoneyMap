using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoneyMap.Application;
using MoneyMap.Core.DataModels;
using System.Security.Claims;

namespace MoneyMap.Web.Areas.Users.Pages.Expenses;

public class IndexModel : PageModel
{
    private readonly IExpenseService _expenseService;

    public IndexModel(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    public IReadOnlyList<Expense> Expenses { get; private set; } = Array.Empty<Expense>();

    [BindProperty(SupportsGet = true)]
    public string? SearchTerm { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? CategoryId { get; set; }

    [BindNever]
    public SelectList CategorySelectList { get; private set; } = new(Array.Empty<ExpenseCategory>(), nameof(ExpenseCategory.Id), nameof(ExpenseCategory.Name));

    [BindProperty]
    public int Id { get; set; }

    public async Task OnGetAsync(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var categories = await _expenseService.GetCategoriesAsync(ct);
        CategorySelectList = new SelectList(categories, nameof(ExpenseCategory.Id), nameof(ExpenseCategory.Name));
        Expenses = await _expenseService.GetAllAsync(userId, SearchTerm, CategoryId, ct);
    }

    public async Task<IActionResult> OnPostDeleteAsync(CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var removed = await _expenseService.RemoveAsync(userId, Id, ct);
        if (!removed)
        {
            return NotFound();
        }
        return RedirectToPage();
    }
}
