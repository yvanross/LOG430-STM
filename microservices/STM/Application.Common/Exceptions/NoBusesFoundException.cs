using Domain.Common.Exceptions;

namespace Application.Common.Exceptions;

public class NoBusesFoundException : StmException
{
    public NoBusesFoundException() : base("No buses are known for the given coordinates")
    {
    }
}