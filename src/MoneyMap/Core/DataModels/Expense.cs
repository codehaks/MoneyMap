namespace MoneyMap.Core.DataModels;

public class Expense
{
    public const int MaxNoteLength = 100;
    public const int MaxAgeYears = 1;

    public int Id { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }
    public string Note { get; private set; } = default!;

    public int CategoryId { get; private set; }
    public ExpenseCategory Category { get; private set; } = default!;

    public string UserId { get; private set; } = default!;

    private Expense() { }

    public static Expense Create(string userId, decimal amount, DateTime dateUtc, int categoryId, string note)
    {
        ValidateInvariants(userId, amount, dateUtc, categoryId, note);
        return new Expense
        {
            UserId = userId,
            Amount = amount,
            Date = dateUtc,
            CategoryId = categoryId,
            Note = note,
        };
    }

    public void Modify(decimal amount, DateTime dateUtc, int categoryId, string note)
    {
        ValidateInvariants(UserId, amount, dateUtc, categoryId, note);
        Amount = amount;
        Date = dateUtc;
        CategoryId = categoryId;
        Note = note;
    }

    private static void ValidateInvariants(string userId, decimal amount, DateTime dateUtc, int categoryId, string note)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new DomainException("UserId is required.");
        if (amount <= 0m)
            throw new DomainException("Amount must be greater than zero.");
        if (categoryId <= 0)
            throw new DomainException("Category is required.");
        if (string.IsNullOrWhiteSpace(note))
            throw new DomainException("Note is required.");
        if (note.Length > MaxNoteLength)
            throw new DomainException($"Note cannot exceed {MaxNoteLength} characters.");

        var todayUtc = DateTime.UtcNow.Date;
        if (dateUtc.Date > todayUtc)
            throw new DomainException("Date cannot be in the future.");
        if (dateUtc < DateTime.UtcNow.AddYears(-MaxAgeYears))
            throw new DomainException($"Date is older than {MaxAgeYears} year(s).");
    }
}
