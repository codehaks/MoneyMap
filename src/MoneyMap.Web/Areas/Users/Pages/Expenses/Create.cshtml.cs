using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoneyMap.Application;
using MoneyMap.Core;
using MoneyMap.Core.DataModels;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MoneyMap.Web.Areas.Users.Pages.Expenses;

[BindProperties]
public class CreateModel : PageModel
{
    private readonly IExpenseService _expenseService;

    public CreateModel(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [BindNever]
    public SelectList CategorySelectList { get; private set; } = new(Array.Empty<ExpenseCategory>(), nameof(ExpenseCategory.Id), nameof(ExpenseCategory.Name));

    public int CategoryId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Required]
    [StringLength(Expense.MaxNoteLength, MinimumLength = 1)]
    public string Note { get; set; } = string.Empty;

    public async Task OnGetAsync(CancellationToken ct)
    {
        await PopulateCategoriesAsync(ct);
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateCategoriesAsync(ct);
            return Page();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        try
        {
            await _expenseService.CreateAsync(userId, Amount, Date.ToUniversalTime(), CategoryId, Note, ct);
        }
        catch (DomainException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await PopulateCategoriesAsync(ct);
            return Page();
        }

        return RedirectToPage("Index");
    }

    private async Task PopulateCategoriesAsync(CancellationToken ct)
    {
        var categories = await _expenseService.GetCategoriesAsync(ct);
        CategorySelectList = new SelectList(categories, nameof(ExpenseCategory.Id), nameof(ExpenseCategory.Name));
    }
}
