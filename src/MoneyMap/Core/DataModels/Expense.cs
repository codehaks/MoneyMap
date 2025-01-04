namespace MoneyMap.Core.DataModels;

public class Expense
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public string Note { get; set; } = default!;

    public int CategoryId { get; set; }
    public ExpenseCategory Category { get; set; } = default!;

}
