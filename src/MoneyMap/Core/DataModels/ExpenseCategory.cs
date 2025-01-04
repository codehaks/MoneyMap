namespace MoneyMap.Core.DataModels;

public class ExpenseCategory
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public List<Expense> Expenses { get; set; } = new();
}