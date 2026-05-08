using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MoneyMap.Core.DataModels;

namespace MoneyMap.Infrastructure.Data.Configurations;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.Property(e => e.Amount).HasPrecision(18, 2);
        builder.Property(e => e.Note).HasMaxLength(Expense.MaxNoteLength).IsRequired();
        builder.Property(e => e.UserId).IsRequired();

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => new { e.UserId, e.Date });

        builder.HasOne(e => e.Category)
               .WithMany(c => c.Expenses)
               .HasForeignKey(e => e.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
