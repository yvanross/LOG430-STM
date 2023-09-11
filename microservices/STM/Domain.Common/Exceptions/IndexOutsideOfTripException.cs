namespace Domain.Common.Exceptions;

public class IndexOutsideOfTripException : StmException
{
    public IndexOutsideOfTripException(string message) : base(message)
    {
    }
}