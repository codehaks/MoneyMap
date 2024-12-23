namespace MoneyMap.Application;

public interface ICalendarService
{
    int DaysLeftInYear();
    bool IsNewYear();
}