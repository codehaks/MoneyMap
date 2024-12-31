using MoneyMap.Core.DataModels;

namespace MoneyMap.Application.Services;
public interface IExpenseService
{
    IList<Expense> GetAll();
    void Create(Expense expense);
    Expense? FindById(int id);
    void Update(Expense expense);
    void Remove(int id);
}