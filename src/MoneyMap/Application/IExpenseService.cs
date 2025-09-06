using MoneyMap.Core.DataModels;

namespace MoneyMap.Application;
public interface IExpenseService
{
    IList<Expense> GetAll(string userId, string? searchTerm, int? categoryId);
    void Create(string userId, Expense expense);
    Expense? FindById(string userId, int id);
    void Update(string userId, Expense expense);
    void Remove(string userId, int id);
    List<ExpenseCategory> GetCategories();
}