using Microsoft.EntityFrameworkCore;
using MoneyMap.Core.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyMap.Infrastructure.Data;
public class MoneyMapDbContext : DbContext
{
    public MoneyMapDbContext(DbContextOptions<MoneyMapDbContext> options) : base(options)
    {
    }

    public DbSet<Expense> Expenses { get; set; }

    public DbSet<ExpenseCategory> ExpenseCategories { get; set; }

}
