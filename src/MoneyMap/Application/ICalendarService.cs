namespace MoneyMap.Application;

public interface ICalendarService
{
    int DaysLeftInYear();
    bool IsNewYear();
    Task<int> DaysLeftInYearAsync();
    Task<bool> IsNewYearAsync();
}