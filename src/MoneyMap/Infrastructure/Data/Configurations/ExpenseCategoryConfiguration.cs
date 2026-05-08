using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyMap.Core.DataModels;

namespace MoneyMap.Infrastructure.Data.Configurations;

public class ExpenseCategoryConfiguration : IEntityTypeConfiguration<ExpenseCategory>
{
    public void Configure(EntityTypeBuilder<ExpenseCategory> builder)
    {
        builder.Property(c => c.Name).HasMaxLength(64).IsRequired();
        builder.HasIndex(c => c.Name).IsUnique();

        builder.HasData(
            new ExpenseCategory { Id = 1, Name = "Groceries" },
            new ExpenseCategory { Id = 2, Name = "Rent" },
            new ExpenseCategory { Id = 3, Name = "Utilities" },
            new ExpenseCategory { Id = 4, Name = "Transportation" },
            new ExpenseCategory { Id = 5, Name = "Entertainment" },
            new ExpenseCategory { Id = 6, Name = "Health" },
            new ExpenseCategory { Id = 7, Name = "Insurance" },
            new ExpenseCategory { Id = 8, Name = "Other" }
        );
    }
}
