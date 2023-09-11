using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests.Unit.Application;

public class TestLogger : ILogger
{
    private readonly TestContext _testContext;

    public TestLogger(TestContext testContext)
    {
        _testContext = testContext ?? throw new ArgumentNullException(nameof(testContext));
    }

    public IDisposable BeginScope<TState>(TState state)
    {
        return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
        Func<TState, Exception, string> formatter)
    {
        var message = formatter(state, exception);
        _testContext.WriteLine($"{logLevel}: {message}");
    }
}