namespace Infrastructure.L4ConnectionListener.Exceptions;

public class TcpConnectionException : Exception
{
    public TcpConnectionException(string message) : base(message)
    {

    }
}