namespace Domain.Common.Interfaces;

public interface IDatetimeProvider
{
    DateTime GetCurrentTime();

    int GetUtcDifference();

    DateTime GetMontrealTime();

    DateTime GetMontrealTime(DateTime from);
}