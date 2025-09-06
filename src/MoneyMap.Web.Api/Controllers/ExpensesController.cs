using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMap.Application;
using MoneyMap.Core.DataModels;
using MoneyMap.Web.Api.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MoneyMap.Web.Api.Controllers;

/// <summary>
/// Controller for managing expenses
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;
    private readonly ILogger<ExpensesController> _logger;

    public ExpensesController(IExpenseService expenseService, ILogger<ExpensesController> logger)
    {
        _expenseService = expenseService;
        _logger = logger;
    }

    /// <summary>
    /// Gets the current user ID from the authorization context
    /// </summary>
    /// <returns>The user ID</returns>
    private string GetCurrentUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value 
            ?? throw new UnauthorizedAccessException("User ID not found in claims");
    }

    /// <summary>
    /// Get all expenses for the current user with optional filtering and pagination
    /// </summary>
    /// <param name="searchTerm">Optional search term to filter by note</param>
    /// <param name="categoryId">Optional category ID to filter by</param>
    /// <param name="pageSize">Number of items per page (default: 50, max: 1000)</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="startDate">Optional start date filter</param>
    /// <param name="endDate">Optional end date filter</param>
    /// <returns>Paginated list of expenses</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ExpenseDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public ActionResult<ApiResponse<PagedResult<ExpenseDto>>> GetExpenses(
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? categoryId = null,
        [FromQuery, Range(1, 1000)] int pageSize = 50,
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var userId = GetCurrentUserId();
        try
        {
          

            _logger.LogInformation("Getting expenses for user {UserId} with filters: searchTerm={SearchTerm}, categoryId={CategoryId}, page={PageNumber}, pageSize={PageSize}", 
                userId, searchTerm, categoryId, pageNumber, pageSize);

            // Get all expenses (the service doesn't support pagination yet, so we'll do it here)
            var allExpenses = _expenseService.GetAll(userId, searchTerm, categoryId);

            // Apply date filtering if provided
            if (startDate.HasValue)
            {
                allExpenses = allExpenses.Where(e => e.Date >= startDate.Value).ToList();
            }
            if (endDate.HasValue)
            {
                allExpenses = allExpenses.Where(e => e.Date <= endDate.Value).ToList();
            }

            // Apply pagination
            var totalCount = allExpenses.Count;
            var expenses = allExpenses
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var expenseDtos = expenses.Select(e => new ExpenseDto
            {
                Id = e.Id,
                Amount = e.Amount,
                Date = e.Date,
                Note = e.Note ?? string.Empty,
                CategoryId = e.CategoryId,
                CategoryName = e.Category?.Name ?? "Unknown",
                UserId = e.UserId,
                UserName = e.UserName ?? string.Empty
            }).ToList();

            var pagedResult = new PagedResult<ExpenseDto>
            {
                Items = expenseDtos,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Ok(new ApiResponse<PagedResult<ExpenseDto>>
            {
                Success = true,
                Message = $"Retrieved {expenseDtos.Count} expenses",
                Data = pagedResult
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving expenses for user {UserId}", userId);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while retrieving expenses",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    /// <summary>
    /// Get a specific expense by ID for the current user
    /// </summary>
    /// <param name="id">The expense ID</param>
    /// <returns>The expense if found</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ExpenseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public ActionResult<ApiResponse<ExpenseDto>> GetExpense(int id)
    {
        var userId = GetCurrentUserId();
        try
        {


            _logger.LogInformation("Getting expense {ExpenseId} for user {UserId}", id, userId);

            var expense = _expenseService.FindById(userId, id);
            if (expense == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Expense with ID {id} not found for user {userId}",
                    Errors = new List<string> { "Expense not found or does not belong to the specified user" }
                });
            }

            var expenseDto = new ExpenseDto
            {
                Id = expense.Id,
                Amount = expense.Amount,
                Date = expense.Date,
                Note = expense.Note ?? string.Empty,
                CategoryId = expense.CategoryId,
                CategoryName = expense.Category?.Name ?? "Unknown",
                UserId = expense.UserId,
                UserName = expense.UserName ?? string.Empty
            };

            return Ok(new ApiResponse<ExpenseDto>
            {
                Success = true,
                Message = "Expense retrieved successfully",
                Data = expenseDto
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving expense {ExpenseId} for user {UserId}", id, userId);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while retrieving the expense",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    /// <summary>
    /// Create a new expense
    /// </summary>
    /// <param name="createExpenseDto">The expense data</param>
    /// <returns>The created expense</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ExpenseDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public ActionResult<ApiResponse<ExpenseDto>> CreateExpense([FromBody] CreateExpenseDto createExpenseDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                });
            }

            var userId = GetCurrentUserId();

            _logger.LogInformation("Creating expense for user {UserId} with amount {Amount}", userId, createExpenseDto.Amount);

            var expense = new Expense
            {
                Amount = createExpenseDto.Amount,
                Date = createExpenseDto.Date,
                Note = createExpenseDto.Note,
                CategoryId = createExpenseDto.CategoryId,
                UserId = userId,
                UserName = createExpenseDto.UserName
            };

            _expenseService.Create(userId, expense);

            // Retrieve the created expense to get the ID and category info
            var createdExpense = _expenseService.FindById(userId, expense.Id);
            if (createdExpense == null)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error retrieving created expense",
                    Errors = new List<string> { "The expense was created but could not be retrieved" }
                });
            }

            var expenseDto = new ExpenseDto
            {
                Id = createdExpense.Id,
                Amount = createdExpense.Amount,
                Date = createdExpense.Date,
                Note = createdExpense.Note ?? string.Empty,
                CategoryId = createdExpense.CategoryId,
                CategoryName = createdExpense.Category?.Name ?? "Unknown",
                UserId = createdExpense.UserId,
                UserName = createdExpense.UserName ?? string.Empty
            };

            return CreatedAtAction(nameof(GetExpense), 
                new { id = expense.Id }, 
                new ApiResponse<ExpenseDto>
                {
                    Success = true,
                    Message = "Expense created successfully",
                    Data = expenseDto
                });
        }
        catch (Exception ex)
        {
            var userId = GetCurrentUserId();
            _logger.LogError(ex, "Error creating expense for user {UserId}", userId);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while creating the expense",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    /// <summary>
    /// Update an existing expense
    /// </summary>
    /// <param name="id">The expense ID</param>
    /// <param name="updateExpenseDto">The updated expense data</param>
    /// <returns>The updated expense</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ExpenseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public ActionResult<ApiResponse<ExpenseDto>> UpdateExpense(int id, [FromBody] UpdateExpenseDto updateExpenseDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Validation failed",
                    Errors = errors
                });
            }

            var userId = GetCurrentUserId();

            _logger.LogInformation("Updating expense {ExpenseId} for user {UserId}", id, userId);

            // Check if expense exists
            var existingExpense = _expenseService.FindById(userId, id);
            if (existingExpense == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Expense with ID {id} not found for user {userId}",
                    Errors = new List<string> { "Expense not found or does not belong to the specified user" }
                });
            }

            var expense = new Expense
            {
                Id = id,
                Amount = updateExpenseDto.Amount,
                Date = updateExpenseDto.Date,
                Note = updateExpenseDto.Note,
                CategoryId = updateExpenseDto.CategoryId,
                UserId = userId,
                UserName = existingExpense.UserName // Keep existing username
            };

            _expenseService.Update(userId, expense);

            // Retrieve the updated expense
            var updatedExpense = _expenseService.FindById(userId, id);
            if (updatedExpense == null)
            {
                return StatusCode(500, new ApiResponse<object>
                {
                    Success = false,
                    Message = "Error retrieving updated expense",
                    Errors = new List<string> { "The expense was updated but could not be retrieved" }
                });
            }

            var expenseDto = new ExpenseDto
            {
                Id = updatedExpense.Id,
                Amount = updatedExpense.Amount,
                Date = updatedExpense.Date,
                Note = updatedExpense.Note ?? string.Empty,
                CategoryId = updatedExpense.CategoryId,
                CategoryName = updatedExpense.Category?.Name ?? "Unknown",
                UserId = updatedExpense.UserId,
                UserName = updatedExpense.UserName ?? string.Empty
            };

            return Ok(new ApiResponse<ExpenseDto>
            {
                Success = true,
                Message = "Expense updated successfully",
                Data = expenseDto
            });
        }
        catch (Exception ex)
        {
            var userId = GetCurrentUserId();
            _logger.LogError(ex, "Error updating expense {ExpenseId} for user {UserId}", id, userId);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while updating the expense",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    /// <summary>
    /// Delete an expense for the current user
    /// </summary>
    /// <param name="id">The expense ID</param>
    /// <returns>Success response</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 204)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public ActionResult<ApiResponse<object>> DeleteExpense(int id)
    {
        try
        {
            var userId = GetCurrentUserId();

            _logger.LogInformation("Deleting expense {ExpenseId} for user {UserId}", id, userId);

            // Check if expense exists
            var expense = _expenseService.FindById(userId, id);
            if (expense == null)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Expense with ID {id} not found for user {userId}",
                    Errors = new List<string> { "Expense not found or does not belong to the specified user" }
                });
            }

            _expenseService.Remove(userId, id);

            return NoContent();
        }
        catch (Exception ex)
        {
            var userId = GetCurrentUserId();
            _logger.LogError(ex, "Error deleting expense {ExpenseId} for user {UserId}", id, userId);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while deleting the expense",
                Errors = new List<string> { ex.Message }
            });
        }
    }

    /// <summary>
    /// Get all expense categories
    /// </summary>
    /// <returns>List of expense categories</returns>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(ApiResponse<List<ExpenseCategoryDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 500)]
    public ActionResult<ApiResponse<List<ExpenseCategoryDto>>> GetCategories()
    {
        try
        {
            _logger.LogInformation("Getting all expense categories");

            var categories = _expenseService.GetCategories();
            var categoryDtos = categories.Select(c => new ExpenseCategoryDto
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();

            return Ok(new ApiResponse<List<ExpenseCategoryDto>>
            {
                Success = true,
                Message = $"Retrieved {categoryDtos.Count} categories",
                Data = categoryDtos
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving expense categories");
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while retrieving categories",
                Errors = new List<string> { ex.Message }
            });
        }
    }
}
