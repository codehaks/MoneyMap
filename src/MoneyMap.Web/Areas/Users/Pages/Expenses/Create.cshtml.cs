using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoneyMap.Application;
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
    public SelectList CategorySelectList { get; set; }

    public int CategoryId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; } // cents

    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Required]
    [StringLength(100)]
    [MinLength(1)]
    public string Note { get; set; } = default!;

    public void OnGet()
    {
        var categories = _expenseService.GetCategories();
        CategorySelectList = new SelectList(categories, "Id", "Name");
    }

    public IActionResult OnPost()
    {

        if (Date < DateTime.Now.AddYears(-1))
        {
            ModelState.AddModelError("Date", "Too old!");
            ModelState.AddModelError("", "Can not add new expense!");

            var categories = _expenseService.GetCategories();
            CategorySelectList = new SelectList(categories, "Id", "Name");
            return Page();
        }
        else if (Date.Date > DateTime.UtcNow.Date)
        {
            ModelState.AddModelError("Date", "Can not be in future!");

            var categories = _expenseService.GetCategories();
            CategorySelectList = new SelectList(categories, "Id", "Name");
            return Page();
        }

        if (!ModelState.IsValid)
        {
            var categories = _expenseService.GetCategories();
            CategorySelectList = new SelectList(categories, "Id", "Name");
            return Page();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var userName = User.FindFirstValue(ClaimTypes.Name);

        _expenseService.Create(userId, new MoneyMap.Core.DataModels.Expense
        {
            CategoryId = CategoryId,
            Amount = Amount,
            Date = Date.ToUniversalTime(), //DateTime.UtcNow,
            Note = Note,
            UserId = userId,
            UserName = userName!
        });

        // redirect to Index page
        return RedirectToPage("Index");
    }


}
