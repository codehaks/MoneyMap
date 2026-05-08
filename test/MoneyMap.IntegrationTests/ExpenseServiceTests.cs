using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using MoneyMap.Application.Services;
using MoneyMap.Core;
using MoneyMap.Core.DataModels;
using MoneyMap.Infrastructure.Data;
using Xunit;

namespace MoneyMap.IntegrationTests;

public class ExpenseServiceTests : IAsyncLifetime
{
    private const string UserA = "user-a";
    private const string UserB = "user-b";

    private SqliteConnection _connection = default!;
    private DbContextOptions<MoneyMapDbContext> _options = default!;

    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        await _connection.OpenAsync();

        _options = new DbContextOptionsBuilder<MoneyMapDbContext>()
            .UseSqlite(_connection)
            .Options;

        await using var db = new MoneyMapDbContext(_options);
        await db.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
    }

    private MoneyMapDbContext NewContext() => new(_options);

    private ExpenseService NewService(MoneyMapDbContext db) =>
        new(db, NullLogger<ExpenseService>.Instance);

    [Fact]
    public async Task CreateAsync_PersistsExpense()
    {
        await using var db = NewContext();
        var sut = NewService(db);

        await sut.CreateAsync(UserA, 25m, DateTime.UtcNow.Date, 1, "Lunch");

        await using var verify = NewContext();
        var stored = await verify.Expenses.SingleAsync();
        Assert.Equal(UserA, stored.UserId);
        Assert.Equal(25m, stored.Amount);
        Assert.Equal(1, stored.CategoryId);
        Assert.Equal("Lunch", stored.Note);
    }

    [Fact]
    public async Task CreateAsync_WithInvalidData_ThrowsDomainExceptionAndSavesNothing()
    {
        await using var db = NewContext();
        var sut = NewService(db);

        await Assert.ThrowsAsync<DomainException>(() =>
            sut.CreateAsync(UserA, -1m, DateTime.UtcNow.Date, 1, "Bad"));

        await using var verify = NewContext();
        Assert.Equal(0, await verify.Expenses.CountAsync());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyOwnedExpenses_OrderedByDateDesc()
    {
        var today = DateTime.UtcNow.Date;
        await SeedAsync(
            (UserA, 10m, today.AddDays(-2), 1, "older"),
            (UserA, 20m, today, 1, "newer"),
            (UserB, 99m, today, 1, "other-user"));

        await using var db = NewContext();
        var sut = NewService(db);

        var result = await sut.GetAllAsync(UserA, null, null);

        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.Equal(UserA, e.UserId));
        Assert.Equal("newer", result[0].Note);
        Assert.Equal("older", result[1].Note);
        Assert.NotNull(result[0].Category);
    }

    [Fact]
    public async Task GetAllAsync_WithSearchTerm_FiltersByNoteCaseInsensitively()
    {
        var today = DateTime.UtcNow.Date;
        await SeedAsync(
            (UserA, 1m, today, 1, "Coffee at Cafe"),
            (UserA, 2m, today, 1, "Groceries"),
            (UserA, 3m, today, 1, "coffee beans"));

        await using var db = NewContext();
        var sut = NewService(db);

        var result = await sut.GetAllAsync(UserA, "COFFEE", null);

        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.Contains("coffee", e.Note, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetAllAsync_WithCategoryFilter_FiltersByCategory()
    {
        var today = DateTime.UtcNow.Date;
        await SeedAsync(
            (UserA, 1m, today, 1, "groceries"),
            (UserA, 2m, today, 2, "rent"),
            (UserA, 3m, today, 1, "more groceries"));

        await using var db = NewContext();
        var sut = NewService(db);

        var result = await sut.GetAllAsync(UserA, null, 1);

        Assert.Equal(2, result.Count);
        Assert.All(result, e => Assert.Equal(1, e.CategoryId));
    }

    [Fact]
    public async Task FindByIdAsync_ReturnsExpense_WhenOwnedByUser()
    {
        var id = await SeedOneAsync(UserA);

        await using var db = NewContext();
        var sut = NewService(db);

        var result = await sut.FindByIdAsync(UserA, id);

        Assert.NotNull(result);
        Assert.Equal(id, result!.Id);
        Assert.NotNull(result.Category);
    }

    [Fact]
    public async Task FindByIdAsync_ReturnsNull_WhenOwnedByDifferentUser()
    {
        var id = await SeedOneAsync(UserA);

        await using var db = NewContext();
        var sut = NewService(db);

        var result = await sut.FindByIdAsync(UserB, id);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingExpense_AndReturnsTrue()
    {
        var id = await SeedOneAsync(UserA);

        await using (var db = NewContext())
        {
            var sut = NewService(db);
            var ok = await sut.UpdateAsync(UserA, id, 77m, DateTime.UtcNow.Date, 2, "updated");
            Assert.True(ok);
        }

        await using var verify = NewContext();
        var stored = await verify.Expenses.SingleAsync(e => e.Id == id);
        Assert.Equal(77m, stored.Amount);
        Assert.Equal(2, stored.CategoryId);
        Assert.Equal("updated", stored.Note);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFalse_WhenExpenseDoesNotExistForUser()
    {
        var id = await SeedOneAsync(UserA);

        await using var db = NewContext();
        var sut = NewService(db);

        var ok = await sut.UpdateAsync(UserB, id, 77m, DateTime.UtcNow.Date, 2, "updated");

        Assert.False(ok);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidData_Throws_AndDoesNotPersist()
    {
        var id = await SeedOneAsync(UserA, amount: 5m, note: "original");

        await using (var db = NewContext())
        {
            var sut = NewService(db);
            await Assert.ThrowsAsync<DomainException>(() =>
                sut.UpdateAsync(UserA, id, -1m, DateTime.UtcNow.Date, 1, "bad"));
        }

        await using var verify = NewContext();
        var stored = await verify.Expenses.SingleAsync(e => e.Id == id);
        Assert.Equal(5m, stored.Amount);
        Assert.Equal("original", stored.Note);
    }

    [Fact]
    public async Task RemoveAsync_DeletesExpense_AndReturnsTrue()
    {
        var id = await SeedOneAsync(UserA);

        await using (var db = NewContext())
        {
            var sut = NewService(db);
            var ok = await sut.RemoveAsync(UserA, id);
            Assert.True(ok);
        }

        await using var verify = NewContext();
        Assert.False(await verify.Expenses.AnyAsync(e => e.Id == id));
    }

    [Fact]
    public async Task RemoveAsync_ReturnsFalse_WhenExpenseDoesNotExistForUser()
    {
        var id = await SeedOneAsync(UserA);

        await using var db = NewContext();
        var sut = NewService(db);

        var ok = await sut.RemoveAsync(UserB, id);

        Assert.False(ok);
        await using var verify = NewContext();
        Assert.True(await verify.Expenses.AnyAsync(e => e.Id == id));
    }

    [Fact]
    public async Task GetCategoriesAsync_ReturnsSeededCategoriesOrderedByName()
    {
        await using var db = NewContext();
        var sut = NewService(db);

        var result = await sut.GetCategoriesAsync();

        Assert.Equal(8, result.Count);
        var names = result.Select(c => c.Name).ToList();
        Assert.Equal(names.OrderBy(n => n, StringComparer.Ordinal).ToList(), names);
    }

    private async Task<int> SeedOneAsync(string userId, decimal amount = 10m, string note = "seed")
    {
        await using var db = NewContext();
        var expense = Expense.Create(userId, amount, DateTime.UtcNow.Date, 1, note);
        db.Expenses.Add(expense);
        await db.SaveChangesAsync();
        return expense.Id;
    }

    private async Task SeedAsync(params (string UserId, decimal Amount, DateTime Date, int CategoryId, string Note)[] rows)
    {
        await using var db = NewContext();
        foreach (var r in rows)
        {
            db.Expenses.Add(Expense.Create(r.UserId, r.Amount, r.Date, r.CategoryId, r.Note));
        }
        await db.SaveChangesAsync();
    }
}
