using Microsoft.EntityFrameworkCore;
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

    public DbSet<ExpenseCategory> ExpenseCategories { get; set; }

}

public class ExpenseCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}