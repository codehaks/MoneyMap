using MoneyMap.Core.DataModels;
using MoneyMap.Infrastructure.Data;

namespace MoneyMap.Application.Services;
public class ExpenseService : IExpenseService
{
    private readonly MoneyMapDbContext _db;

    public ExpenseService(MoneyMapDbContext db)
    {
        _db = db;
    }

    public void Create(Expense expense)
    {
        _db.Add(expense);
        _db.SaveChanges();
    }

    public Expense? FindById(int id)
    {
        var expense = _db.Expenses.Find(id);
        return expense;
    }

    public IList<Expense> GetAll()
    {
        return _db.Expenses.ToList();
    }

    public void Remove(int id)
    {
        var expense = _db.Expenses.Find(id);
        if (expense != null)
        {
            _db.Expenses.Remove(expense);
            _db.SaveChanges();
        }
    }

    public void Update(Expense expense)
    {
        var oldExpense = _db.Expenses.Find(expense.Id);

        if (oldExpense != null)
        {

            oldExpense.Amount = expense.Amount;
            oldExpense.Note = expense.Note;
            oldExpense.Date = expense.Date;
        }

        _db.SaveChanges();
    }
}
