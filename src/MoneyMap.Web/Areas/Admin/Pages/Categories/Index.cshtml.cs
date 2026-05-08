using Microsoft.AspNetCore.Mvc.RazorPages;
using MoneyMap.Application;
using MoneyMap.Core.DataModels;

namespace MoneyMap.Web.Areas.Admin.Pages.Categories;

public class IndexModel : PageModel
{
    private readonly IExpenseService _expenseService;

    public IndexModel(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    public IReadOnlyList<ExpenseCategory> Categories { get; private set; } = Array.Empty<ExpenseCategory>();

    public async Task OnGetAsync(CancellationToken ct)
    {
        Categories = await _expenseService.GetCategoriesAsync(ct);
    }
}
