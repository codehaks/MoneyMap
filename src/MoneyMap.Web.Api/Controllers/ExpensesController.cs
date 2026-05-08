using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyMap.Application;
using MoneyMap.Core;
using MoneyMap.Core.DataModels;
using MoneyMap.Web.Api.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace MoneyMap.Web.Api.Controllers;

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

    private string GetCurrentUserId() =>
        User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value
            ?? throw new UnauthorizedAccessException("User ID not found in claims");

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ExpenseDto>>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<PagedResult<ExpenseDto>>>> GetExpenses(
        [FromQuery] string? searchTerm = null,
        [FromQuery] int? categoryId = null,
        [FromQuery, Range(1, 1000)] int pageSize = 50,
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        CancellationToken ct = default)
    {
        var userId = GetCurrentUserId();
        _logger.LogInformation("Getting expenses for user {UserId} page {PageNumber} size {PageSize}", userId, pageNumber, pageSize);

        var allExpenses = await _expenseService.GetAllAsync(userId, searchTerm, categoryId, ct);

        IEnumerable<Expense> filtered = allExpenses;
        if (startDate.HasValue) filtered = filtered.Where(e => e.Date >= startDate.Value);
        if (endDate.HasValue) filtered = filtered.Where(e => e.Date <= endDate.Value);

        var materialized = filtered.ToList();
        var totalCount = materialized.Count;
        var page = materialized
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(ToDto)
            .ToList();

        return Ok(new ApiResponse<PagedResult<ExpenseDto>>
        {
            Success = true,
            Message = $"Retrieved {page.Count} expenses",
            Data = new PagedResult<ExpenseDto>
            {
                Items = page,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
            },
        });
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ExpenseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<ExpenseDto>>> GetExpense(int id, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var expense = await _expenseService.FindByIdAsync(userId, id, ct);
        if (expense is null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = $"Expense with ID {id} not found.",
            });
        }

        return Ok(new ApiResponse<ExpenseDto>
        {
            Success = true,
            Message = "Expense retrieved successfully",
            Data = ToDto(expense),
        });
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ExpenseDto>), 201)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    public async Task<ActionResult<ApiResponse<ExpenseDto>>> CreateExpense([FromBody] CreateExpenseDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var userId = GetCurrentUserId();
        try
        {
            await _expenseService.CreateAsync(userId, dto.Amount, dto.Date.ToUniversalTime(), dto.CategoryId, dto.Note, ct);
        }
        catch (DomainException ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message },
            });
        }

        var created = (await _expenseService.GetAllAsync(userId, null, dto.CategoryId, ct))
            .OrderByDescending(e => e.Id)
            .FirstOrDefault();

        if (created is null)
        {
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "Expense was created but could not be retrieved.",
            });
        }

        return CreatedAtAction(nameof(GetExpense), new { id = created.Id }, new ApiResponse<ExpenseDto>
        {
            Success = true,
            Message = "Expense created successfully",
            Data = ToDto(created),
        });
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponse<ExpenseDto>), 200)]
    [ProducesResponseType(typeof(ApiResponse<object>), 400)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<ActionResult<ApiResponse<ExpenseDto>>> UpdateExpense(int id, [FromBody] UpdateExpenseDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var userId = GetCurrentUserId();
        try
        {
            var updated = await _expenseService.UpdateAsync(userId, id, dto.Amount, dto.Date.ToUniversalTime(), dto.CategoryId, dto.Note, ct);
            if (!updated)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = $"Expense with ID {id} not found.",
                });
            }
        }
        catch (DomainException ex)
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = ex.Message,
                Errors = new List<string> { ex.Message },
            });
        }

        var fresh = await _expenseService.FindByIdAsync(userId, id, ct);
        return Ok(new ApiResponse<ExpenseDto>
        {
            Success = true,
            Message = "Expense updated successfully",
            Data = fresh is null ? null : ToDto(fresh),
        });
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(typeof(ApiResponse<object>), 404)]
    public async Task<IActionResult> DeleteExpense(int id, CancellationToken ct)
    {
        var userId = GetCurrentUserId();
        var removed = await _expenseService.RemoveAsync(userId, id, ct);
        if (!removed)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = $"Expense with ID {id} not found.",
            });
        }
        return NoContent();
    }

    [HttpGet("categories")]
    [ProducesResponseType(typeof(ApiResponse<List<ExpenseCategoryDto>>), 200)]
    public async Task<ActionResult<ApiResponse<List<ExpenseCategoryDto>>>> GetCategories(CancellationToken ct)
    {
        var categories = await _expenseService.GetCategoriesAsync(ct);
        var dtos = categories.Select(c => new ExpenseCategoryDto { Id = c.Id, Name = c.Name }).ToList();
        return Ok(new ApiResponse<List<ExpenseCategoryDto>>
        {
            Success = true,
            Message = $"Retrieved {dtos.Count} categories",
            Data = dtos,
        });
    }

    private static ExpenseDto ToDto(Expense e) => new()
    {
        Id = e.Id,
        Amount = e.Amount,
        Date = e.Date,
        Note = e.Note,
        CategoryId = e.CategoryId,
        CategoryName = e.Category?.Name ?? string.Empty,
        UserId = e.UserId,
    };
}
