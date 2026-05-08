using MoneyMap.Core;
using MoneyMap.Core.DataModels;
using Xunit;

namespace MoneyMap.UnitTests;

public class ExpenseTests
{
    private const string UserId = "user-1";
    private const int CategoryId = 1;
    private const string Note = "Coffee";

    private static DateTime Today => DateTime.UtcNow.Date;

    [Fact]
    public void Create_WithValidArguments_SetsAllProperties()
    {
        var date = Today.AddDays(-1);

        var expense = Expense.Create(UserId, 12.50m, date, CategoryId, Note);

        Assert.Equal(UserId, expense.UserId);
        Assert.Equal(12.50m, expense.Amount);
        Assert.Equal(date, expense.Date);
        Assert.Equal(CategoryId, expense.CategoryId);
        Assert.Equal(Note, expense.Note);
        Assert.Equal(0, expense.Id);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankUserId_Throws(string? userId)
    {
        var ex = Assert.Throws<DomainException>(() =>
            Expense.Create(userId!, 1m, Today, CategoryId, Note));
        Assert.Equal("UserId is required.", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [InlineData(-100)]
    public void Create_WithNonPositiveAmount_Throws(double amount)
    {
        var ex = Assert.Throws<DomainException>(() =>
            Expense.Create(UserId, (decimal)amount, Today, CategoryId, Note));
        Assert.Equal("Amount must be greater than zero.", ex.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Create_WithInvalidCategoryId_Throws(int categoryId)
    {
        var ex = Assert.Throws<DomainException>(() =>
            Expense.Create(UserId, 1m, Today, categoryId, Note));
        Assert.Equal("Category is required.", ex.Message);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithBlankNote_Throws(string? note)
    {
        var ex = Assert.Throws<DomainException>(() =>
            Expense.Create(UserId, 1m, Today, CategoryId, note!));
        Assert.Equal("Note is required.", ex.Message);
    }

    [Fact]
    public void Create_WithNoteOverMaxLength_Throws()
    {
        var note = new string('a', Expense.MaxNoteLength + 1);

        var ex = Assert.Throws<DomainException>(() =>
            Expense.Create(UserId, 1m, Today, CategoryId, note));
        Assert.Equal($"Note cannot exceed {Expense.MaxNoteLength} characters.", ex.Message);
    }

    [Fact]
    public void Create_WithNoteAtMaxLength_Succeeds()
    {
        var note = new string('a', Expense.MaxNoteLength);

        var expense = Expense.Create(UserId, 1m, Today, CategoryId, note);

        Assert.Equal(note, expense.Note);
    }

    [Fact]
    public void Create_WithFutureDate_Throws()
    {
        var future = DateTime.UtcNow.AddDays(1);

        var ex = Assert.Throws<DomainException>(() =>
            Expense.Create(UserId, 1m, future, CategoryId, Note));
        Assert.Equal("Date cannot be in the future.", ex.Message);
    }

    [Fact]
    public void Create_WithDateOlderThanMaxAge_Throws()
    {
        var tooOld = DateTime.UtcNow.AddYears(-Expense.MaxAgeYears).AddDays(-1);

        var ex = Assert.Throws<DomainException>(() =>
            Expense.Create(UserId, 1m, tooOld, CategoryId, Note));
        Assert.Equal($"Date is older than {Expense.MaxAgeYears} year(s).", ex.Message);
    }

    [Fact]
    public void Modify_WithValidArguments_UpdatesProperties()
    {
        var expense = Expense.Create(UserId, 1m, Today, CategoryId, Note);
        var newDate = Today.AddDays(-3);

        expense.Modify(99.99m, newDate, 2, "Updated");

        Assert.Equal(99.99m, expense.Amount);
        Assert.Equal(newDate, expense.Date);
        Assert.Equal(2, expense.CategoryId);
        Assert.Equal("Updated", expense.Note);
        Assert.Equal(UserId, expense.UserId);
    }

    [Fact]
    public void Modify_WithInvalidAmount_ThrowsAndDoesNotMutate()
    {
        var expense = Expense.Create(UserId, 5m, Today, CategoryId, Note);

        Assert.Throws<DomainException>(() =>
            expense.Modify(0m, Today, CategoryId, Note));

        Assert.Equal(5m, expense.Amount);
        Assert.Equal(Note, expense.Note);
    }
}
