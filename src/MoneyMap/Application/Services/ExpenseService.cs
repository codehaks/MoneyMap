using MoneyMap.Core.DataModels;
using MoneyMap.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    public IList<Expense> GetAll()
    {
        return _db.Expenses.ToList();
    }
}
