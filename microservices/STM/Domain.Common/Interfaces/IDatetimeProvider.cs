namespace Domain.Common.Interfaces;

public interface IDatetimeProvider
{
    DateTime GetCurrentTime();

    int GetUtcDifference();
}