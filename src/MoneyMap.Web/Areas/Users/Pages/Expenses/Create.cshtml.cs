using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MoneyMap.Application.Services;
using System.ComponentModel.DataAnnotations;

namespace MoneyMap.Web.Areas.Users.Pages.Expenses;

[BindProperties]
public class CreateModel : PageModel
{
    private readonly IExpenseService _expenseService;
    public CreateModel(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    public SelectList CategorySelectList { get; set; }

    public int CategoryId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; } // cents

    public DateTime Date { get; set; }=DateTime.UtcNow;

    [Required]
    [StringLength(100)]
    [MinLength(1)]
    public string Note { get; set; } = default!;

    public void OnGet()
    {
        var categories= _expenseService.GetCategories();
        CategorySelectList = new SelectList(categories, "Id", "Name");
    }

    public IActionResult OnPost()
    {

        if (Date < DateTime.Now.AddYears(-1))
        {
            ModelState.AddModelError("Date", "Too old!");

            ModelState.AddModelError("", "Can not add new expense!");
            
            return Page();
        }
        else if (Date.Date>DateTime.UtcNow.Date)
        {
            ModelState.AddModelError("Date", "Can not be in future!");

            return Page();
        }

        if (!ModelState.IsValid)
        {

            return Page();
        }

        _expenseService.Create(new MoneyMap.Core.DataModels.Expense
        {
            Amount = Amount,
            Date = Date.ToUniversalTime(), //DateTime.UtcNow,
            Note = Note
        });

        // redirect to Index page
        return RedirectToPage("Index");
    }


}
