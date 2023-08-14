namespace Domain.Common.Exceptions;

public class AggregateInvalidStateException : StmException
{
    public AggregateInvalidStateException(string message) : base(message)
    {
    }
}