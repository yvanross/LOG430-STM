namespace Domain.Common.Exceptions;

public class StopNotFoundException : StmException
{
    private const string DefaultMessage = "No Stops where found for the given parameters";

    public StopNotFoundException() : base(DefaultMessage)
    {
    }
}