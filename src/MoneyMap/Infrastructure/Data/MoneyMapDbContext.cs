using Microsoft.EntityFrameworkCore;
using MoneyMap.Core.DataModels;

namespace MoneyMap.Infrastructure.Data;
public class MoneyMapDbContext : DbContext
{
    public MoneyMapDbContext(DbContextOptions<MoneyMapDbContext> options) : base(options)
    {
    }

    public DbSet<Expense> Expenses { get; set; }

    public DbSet<ExpenseCategory> ExpenseCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<Expense>()
        //    .HasOne(e => e.Category)
        //    .WithMany(c => c.Expenses)
        //    .HasForeignKey(e => e.CategoryId);
        base.OnModelCreating(modelBuilder);
    }

}
