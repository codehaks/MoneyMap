using MoneyMap.Core.DataModels;

namespace MoneyMap.Application;
public interface IExpenseService
{
    IList<Expense> GetAll(string? searchTerm,int? categoryId);
    void Create(Expense expense);
    Expense? FindById(int id);
    void Update(Expense expense);
    void Remove(int id);
    List<ExpenseCategory> GetCategories();
}