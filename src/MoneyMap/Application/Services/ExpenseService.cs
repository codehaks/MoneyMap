using Microsoft.EntityFrameworkCore;
using MoneyMap.Core.DataModels;
using MoneyMap.Infrastructure.Data;
using Microsoft.Extensions.Logging;

namespace MoneyMap.Application.Services;
public class ExpenseService : IExpenseService
{
    private readonly MoneyMapDbContext _db;
    private readonly ILogger<ExpenseService> _logger;

    public ExpenseService(MoneyMapDbContext db, ILogger<ExpenseService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public void Create(string userId, Expense expense)
    {
        _db.Add(expense);
        _db.SaveChanges();
    }

    public Expense? FindById(string userId, int id)
    {
        _logger.LogDebug("Finding expense by id: {ExpenseId} for user: {UserId}", id, userId);
        var expense = _db.Expenses.FirstOrDefault(e => e.Id == id && e.UserId == userId);
        if (expense == null)
        {
            _logger.LogDebug("Expense with id {ExpenseId} not found for user {UserId}.", id, userId);
        }
        else
        {
            _logger.LogDebug("Expense with id {ExpenseId} found for user {UserId}.", id, userId);
        }
        return expense;
    }

    public IList<Expense> GetAll(string userId, string? searchTerm, int? categoryId)
    {
        var query = _db.Expenses.Include("Category").Where(e => e.UserId == userId);
        
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

    public void Remove(string userId, int id)
    {
        var expense = _db.Expenses.FirstOrDefault(e => e.Id == id && e.UserId == userId);
        if (expense != null)
        {
            _db.Expenses.Remove(expense);
            _db.SaveChanges();
        }
    }

    public void Update(string userId, Expense expense)
    {
        var oldExpense = _db.Expenses.FirstOrDefault(e => e.Id == expense.Id && e.UserId == userId);

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
