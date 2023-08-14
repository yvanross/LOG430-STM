using Domain.Common.Exceptions;

namespace Application.Common.Exceptions;

public class KeyNotFoundException : StmException
{
    public KeyNotFoundException() : base("No Entity was found for the provided primary key") { }
}