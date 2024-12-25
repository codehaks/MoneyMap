using MoneyMap.Core.DataModels;

namespace MoneyMap.Application.Services;
public interface IExpenseService
{
    IList<Expense> GetAll();
}