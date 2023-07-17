using Microsoft.Extensions.Logging;

namespace Infrastructure.L4ConnectionListener;

public class L4Logger : ILogger
{
    private readonly ILogger _baseLogger;

    private string _connectionId;

    private DateTime _lock = DateTime.MinValue;

    private static bool _firstBoot = true;

    public L4Logger(ILogger<L4LoadBalancer> baseLogger)
    {
        _baseLogger = baseLogger;
    }

    public IDisposable BeginScope<TState>(TState state) => _baseLogger.BeginScope(state);

    public bool IsEnabled(LogLevel logLevel) => _baseLogger.IsEnabled(logLevel);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (DateTime.UtcNow > _lock)
        {
            string message = formatter(state, exception);
            message += $" ({_connectionId})";
            _baseLogger.Log(logLevel, eventId, state, exception, (s, e) => message);
        }
    }

    public void SetConnectionId(string connectionId)
    {
        _connectionId = connectionId;
    }

    public void Lock(DateTime dateTime)
    {
        if(_firstBoot) _baseLogger.LogInformation("Waiting for TCP chatter to settle, temporarily locking connection logs");

        _firstBoot = false;

        _lock = dateTime;
    }
}