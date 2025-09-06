using Microsoft.EntityFrameworkCore;
using MoneyMap.Core.DataModels;
using MoneyMap.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace MoneyMap.Application.Services;
public class ExpenseService : IExpenseService
{
    private readonly MoneyMapDbContext _db;
    private readonly ILogger<ExpenseService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ExpenseService(MoneyMapDbContext db, ILogger<ExpenseService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _db = db;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    private string GetCurrentUserId()
    {
        return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
    }

    public void Create(Expense expense)
    {
        _db.Add(expense);
        _db.SaveChanges();
    }

    public Expense? FindById(int id)
    {
        var currentUserId = GetCurrentUserId();
        _logger.LogDebug("Finding expense by id: {ExpenseId} for user: {UserId}", id, currentUserId);
        var expense = _db.Expenses.FirstOrDefault(e => e.Id == id && e.UserId == currentUserId);
        if (expense == null)
        {
            _logger.LogDebug("Expense with id {ExpenseId} not found for user {UserId}.", id, currentUserId);
        }
        else
        {
            _logger.LogDebug("Expense with id {ExpenseId} found for user {UserId}.", id, currentUserId);
        }
        return expense;
    }

    public IList<Expense> GetAll(string? searchTerm, int? categoryId)
    {
        var currentUserId = GetCurrentUserId();
        var query = _db.Expenses.Include("Category").Where(e => e.UserId == currentUserId);
        
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query
               // .Where(e => e.Note.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
               .Where(e => EF.Functions.Like(e.Note.ToLower(), $"%{searchTerm.ToLower()}%"));

        }

        if (categoryId is not null)
        {
            query = query
                .Where(e => e.CategoryId==categoryId);
        }
        return query.ToList();
    }

    public void Remove(int id)
    {
        var currentUserId = GetCurrentUserId();
        var expense = _db.Expenses.FirstOrDefault(e => e.Id == id && e.UserId == currentUserId);
        if (expense != null)
        {
            _db.Expenses.Remove(expense);
            _db.SaveChanges();
        }
    }

    public void Update(Expense expense)
    {
        var currentUserId = GetCurrentUserId();
        var oldExpense = _db.Expenses.FirstOrDefault(e => e.Id == expense.Id && e.UserId == currentUserId);

        if (oldExpense != null)
        {
            oldExpense.Amount = expense.Amount;
            oldExpense.Note = expense.Note;
            oldExpense.Date = expense.Date;
            oldExpense.CategoryId = expense.CategoryId;
        }

        _db.SaveChanges();
    }

    public List<ExpenseCategory> GetCategories()
    {
        return _db.ExpenseCategories.ToList();
    }
}
