using Microsoft.EntityFrameworkCore;
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

    public IList<Expense> GetAll(string searchTerm)
    {
        var query = _db.Expenses.Include("Category");
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query
               // .Where(e => e.Note.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
               .Where(e => EF.Functions.Like(e.Note.ToLower(), $"%{searchTerm.ToLower()}%"));

        }
        return query.ToList();
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

    public List<ExpenseCategory> GetCategories()
    {
        return _db.ExpenseCategories.ToList();
    }
}
