using System.ComponentModel.DataAnnotations;

namespace MoneyMap.Web.Api.Models;

/// <summary>
/// Standard API response wrapper
/// </summary>
/// <typeparam name="T">The type of data being returned</typeparam>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// DTO for returning expense data
/// </summary>
public class ExpenseDto
{
    public int Id { get; set; }
    
    [Required]
    public decimal Amount { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    public string Note { get; set; } = string.Empty;
    
    [Required]
    public int CategoryId { get; set; }
    
    public string CategoryName { get; set; } = string.Empty;
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    public string UserName { get; set; } = string.Empty;
}

/// <summary>
/// DTO for creating new expenses
/// </summary>
public class CreateExpenseDto
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
    public string Note { get; set; } = string.Empty;
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a valid category")]
    public int CategoryId { get; set; }
    
    [Required]
    [StringLength(450, ErrorMessage = "UserId cannot exceed 450 characters")]
    public string UserId { get; set; } = string.Empty;
    
    [Required]
    [StringLength(256, ErrorMessage = "UserName cannot exceed 256 characters")]
    public string UserName { get; set; } = string.Empty;
}

/// <summary>
/// DTO for updating existing expenses
/// </summary>
public class UpdateExpenseDto
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    [StringLength(500, ErrorMessage = "Note cannot exceed 500 characters")]
    public string Note { get; set; } = string.Empty;
    
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a valid category")]
    public int CategoryId { get; set; }
    
    [Required]
    [StringLength(450, ErrorMessage = "UserId cannot exceed 450 characters")]
    public string UserId { get; set; } = string.Empty;
}

/// <summary>
/// DTO for expense category data
/// </summary>
public class ExpenseCategoryDto
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// DTO for filtering and searching expenses
/// </summary>
public class ExpenseFilterDto
{
    [Required]
    [StringLength(450, ErrorMessage = "UserId cannot exceed 450 characters")]
    public string UserId { get; set; } = string.Empty;
    
    [StringLength(100, ErrorMessage = "Search term cannot exceed 100 characters")]
    public string? SearchTerm { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "CategoryId must be a valid category")]
    public int? CategoryId { get; set; }
    
    [Range(1, 1000, ErrorMessage = "Page size must be between 1 and 1000")]
    public int PageSize { get; set; } = 50;
    
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
    public int PageNumber { get; set; } = 1;
    
    public DateTime? StartDate { get; set; }
    
    public DateTime? EndDate { get; set; }
}

/// <summary>
/// DTO for paginated results
/// </summary>
/// <typeparam name="T">The type of items in the result</typeparam>
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
