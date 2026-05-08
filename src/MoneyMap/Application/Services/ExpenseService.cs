using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MoneyMap.Core.DataModels;
using MoneyMap.Infrastructure.Data;

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

    public async Task<IReadOnlyList<Expense>> GetAllAsync(string userId, string? searchTerm, int? categoryId, CancellationToken ct = default)
    {
        var query = _db.Expenses
            .Include(e => e.Category)
            .Where(e => e.UserId == userId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var pattern = $"%{searchTerm.ToLower()}%";
            query = query.Where(e => EF.Functions.Like(e.Note.ToLower(), pattern));
        }

        if (categoryId is not null)
        {
            query = query.Where(e => e.CategoryId == categoryId);
        }

        return await query.OrderByDescending(e => e.Date).ToListAsync(ct);
    }

    public async Task CreateAsync(string userId, decimal amount, DateTime dateUtc, int categoryId, string note, CancellationToken ct = default)
    {
        var expense = Expense.Create(userId, amount, dateUtc, categoryId, note);
        _db.Expenses.Add(expense);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Created expense {ExpenseId} for user {UserId}.", expense.Id, userId);
    }

    public Task<Expense?> FindByIdAsync(string userId, int id, CancellationToken ct = default) =>
        _db.Expenses
           .Include(e => e.Category)
           .FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId, ct);

    public async Task<bool> UpdateAsync(string userId, int id, decimal amount, DateTime dateUtc, int categoryId, string note, CancellationToken ct = default)
    {
        var existing = await _db.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId, ct);
        if (existing is null)
        {
            _logger.LogWarning("Update skipped: expense {ExpenseId} not found for user {UserId}.", id, userId);
            return false;
        }

        existing.Modify(amount, dateUtc, categoryId, note);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Updated expense {ExpenseId} for user {UserId}.", id, userId);
        return true;
    }

    public async Task<bool> RemoveAsync(string userId, int id, CancellationToken ct = default)
    {
        var existing = await _db.Expenses.FirstOrDefaultAsync(e => e.Id == id && e.UserId == userId, ct);
        if (existing is null)
        {
            _logger.LogWarning("Remove skipped: expense {ExpenseId} not found for user {UserId}.", id, userId);
            return false;
        }

        _db.Expenses.Remove(existing);
        await _db.SaveChangesAsync(ct);
        _logger.LogInformation("Removed expense {ExpenseId} for user {UserId}.", id, userId);
        return true;
    }

    public async Task<IReadOnlyList<ExpenseCategory>> GetCategoriesAsync(CancellationToken ct = default) =>
        await _db.ExpenseCategories.OrderBy(c => c.Name).ToListAsync(ct);
}
