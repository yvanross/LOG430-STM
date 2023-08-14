namespace Domain.Common.Exceptions;

public class TripHasNoScheduledStopException : StmException
{
    private const string DefaultMessage = "Trip contained not scheduled stops";

    public TripHasNoScheduledStopException() : base(DefaultMessage)
    {
    }
}