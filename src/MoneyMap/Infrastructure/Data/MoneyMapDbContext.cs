using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoneyMap.Core.DataModels;

namespace MoneyMap.Infrastructure.Data;
public class MoneyMapDbContext : IdentityDbContext<ApplicationUser>
{
    public MoneyMapDbContext(DbContextOptions<MoneyMapDbContext> options) : base(options)
    {
    }

    public DbSet<Expense> Expenses { get; set; }

    public DbSet<ExpenseCategory> ExpenseCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ExpenseCategory>().HasData(
            new ExpenseCategory { Id = 1, Name = "Groceries" },
            new ExpenseCategory { Id = 2, Name = "Rent" },
            new ExpenseCategory { Id = 3, Name = "Utilities" },
            new ExpenseCategory { Id = 4, Name = "Transportation" },
            new ExpenseCategory { Id = 5, Name = "Entertainment" },
            new ExpenseCategory { Id = 6, Name = "Health" },
            new ExpenseCategory { Id = 7, Name = "Insurance" },
            new ExpenseCategory { Id = 8, Name = "Other" }
        );

        base.OnModelCreating(modelBuilder);
    }

}

public class ApplicationUser:IdentityUser
{
    //public string FirstName { get; set; }
    //public string LastName { get; set; }
}
