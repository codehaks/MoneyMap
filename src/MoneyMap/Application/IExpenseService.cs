using MoneyMap.Core.DataModels;

namespace MoneyMap.Application;

public interface IExpenseService
{
    Task<IReadOnlyList<Expense>> GetAllAsync(string userId, string? searchTerm, int? categoryId, CancellationToken ct = default);
    Task CreateAsync(string userId, decimal amount, DateTime dateUtc, int categoryId, string note, CancellationToken ct = default);
    Task<Expense?> FindByIdAsync(string userId, int id, CancellationToken ct = default);
    Task<bool> UpdateAsync(string userId, int id, decimal amount, DateTime dateUtc, int categoryId, string note, CancellationToken ct = default);
    Task<bool> RemoveAsync(string userId, int id, CancellationToken ct = default);
    Task<IReadOnlyList<ExpenseCategory>> GetCategoriesAsync(CancellationToken ct = default);
}
