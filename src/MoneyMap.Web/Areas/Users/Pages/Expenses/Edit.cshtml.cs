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
public class EditModel : PageModel
{
    private readonly IExpenseService _expenseService;

    public EditModel(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [BindNever]
    public SelectList CategorySelectList { get; private set; } = new(Array.Empty<ExpenseCategory>(), nameof(ExpenseCategory.Id), nameof(ExpenseCategory.Name));

    public int Id { get; set; }
    public int CategoryId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Required]
    [StringLength(Expense.MaxNoteLength, MinimumLength = 1)]
    public string Note { get; set; } = string.Empty;

    public async Task<IActionResult> OnGetAsync(int id, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var expense = await _expenseService.FindByIdAsync(userId, id, ct);
        if (expense is null)
        {
            return NotFound();
        }

        Id = expense.Id;
        CategoryId = expense.CategoryId;
        Amount = expense.Amount;
        Date = expense.Date;
        Note = expense.Note;

        await PopulateCategoriesAsync(ct);
        return Page();
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
            var updated = await _expenseService.UpdateAsync(userId, Id, Amount, Date.ToUniversalTime(), CategoryId, Note, ct);
            if (!updated)
            {
                return NotFound();
            }
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
