namespace Domain.Common.Exceptions;

public abstract class StmException : Exception
{
    protected StmException(string message) : base(message)
    {
    }
}