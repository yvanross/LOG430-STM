using Microsoft.Extensions.Logging;

namespace Infrastructure.L4ConnectionListener;

public class L4Logger : ILogger
{
    private readonly ILogger _baseLogger;

    private string _connectionId;

    public L4Logger(ILogger<L4LoadBalancer> baseLogger)
    {
        _baseLogger = baseLogger;
    }

    public IDisposable BeginScope<TState>(TState state) => _baseLogger.BeginScope(state);

    public bool IsEnabled(LogLevel logLevel) => _baseLogger.IsEnabled(logLevel);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        string message = formatter(state, exception);
        message += $" ({_connectionId})";
        _baseLogger.Log(logLevel, eventId, state, exception, (s, e) => message);
    }

    public void SetConnectionId(string connectionId)
    {
        _connectionId = connectionId;
    }
}