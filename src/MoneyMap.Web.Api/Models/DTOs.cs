using System.ComponentModel.DataAnnotations;
using MoneyMap.Core.DataModels;

namespace MoneyMap.Web.Api.Models;

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
}

public class ExpenseDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Note { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}

public class CreateExpenseDto
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [StringLength(Expense.MaxNoteLength, MinimumLength = 1)]
    public string Note { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a valid category")]
    public int CategoryId { get; set; }
}

public class UpdateExpenseDto
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [StringLength(Expense.MaxNoteLength, MinimumLength = 1)]
    public string Note { get; set; } = string.Empty;

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a valid category")]
    public int CategoryId { get; set; }
}

public class ExpenseCategoryDto
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = string.Empty;
}

public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}
