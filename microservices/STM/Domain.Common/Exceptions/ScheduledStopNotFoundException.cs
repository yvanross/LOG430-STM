namespace Domain.Common.Exceptions;

public class ScheduledStopNotFoundException : StmException
{
    public ScheduledStopNotFoundException() : base("No scheduled stop was found for the given parameters")
    {
    }
}