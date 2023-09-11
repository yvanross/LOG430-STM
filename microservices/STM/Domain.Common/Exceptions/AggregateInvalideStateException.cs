namespace Domain.Common.Exceptions;

public class AggregateInvalideStateException : StmException
{
    public AggregateInvalideStateException(string message) : base(message)
    {
    }
}