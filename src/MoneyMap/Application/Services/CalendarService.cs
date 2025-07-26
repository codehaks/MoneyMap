namespace MoneyMap.Application.Services;
public class CalendarService : ICalendarService
{
    public int DaysLeftInYear()
    {
        var newYear = new DateTime(DateTime.Now.Year + 1, 1, 1);
        return (newYear - DateTime.Now).Days;
    }

    public bool IsNewYear()
    {
        return DateTime.Now.Year < 2025;
    }

    public Task<int> DaysLeftInYearAsync()
    {
        return Task.FromResult(DaysLeftInYear());
    }

    public Task<bool> IsNewYearAsync()
    {
        return Task.FromResult(IsNewYear());
    }
}
