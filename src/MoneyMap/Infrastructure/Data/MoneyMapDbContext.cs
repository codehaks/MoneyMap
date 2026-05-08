using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoneyMap.Core.DataModels;

namespace MoneyMap.Infrastructure.Data;

public class MoneyMapDbContext : IdentityDbContext<ApplicationUser>
{
    public MoneyMapDbContext(DbContextOptions<MoneyMapDbContext> options) : base(options)
    {
    }

    public DbSet<Expense> Expenses => Set<Expense>();
    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MoneyMapDbContext).Assembly);
    }
}
